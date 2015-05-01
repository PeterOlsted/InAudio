using System;
using System.Collections.Generic;
using InAudioSystem.ExtensionMethods;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{
public static class AudioNodeWorker  {
    public static InAudioNode CreateNode(GameObject go, InAudioNode parent, int guid, AudioNodeType type)
    {
        var node = go.AddComponentUndo<InAudioNode>();

        node._guid = guid;
        node._type = type;
        node.Name = parent.Name + " Child";
        node.MixerGroup = parent.MixerGroup;

        node.AssignParent(parent);

        return node;
    }

    public static InAudioNode CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponent<InAudioNode>();
        AddDataClass(node);
        node._guid = guid;
        node._type = AudioNodeType.Root;
        node.FoldedOut = true;
        node.Name = "Root";
        return node;
    }

    public static InAudioNode CreateTree(GameObject go, int numberOfChildren)
    {
        var Tree = CreateRoot(go, GUIDCreator.Create());
        for (int i = 0; i < numberOfChildren; ++i)
        {
            var newNode = CreateNode(go, Tree, GUIDCreator.Create(), AudioNodeType.Folder);
            AddDataClass(newNode);
        }
        return Tree;
    }

    public static InAudioNode CreateNode(GameObject go, InAudioNode parent, AudioNodeType type)
    {
        var newNode = CreateNode(go, parent, GUIDCreator.Create(), type);
        AddDataClass(newNode);
        return newNode;
    }

    public static InAudioNodeBaseData AddDataClass(InAudioNode node)
    {
        switch (node._type)
        {
            case AudioNodeType.Root:
                node._nodeData = node.gameObject.AddComponentUndo<InFolderData>();
                break;
            case AudioNodeType.Audio:
                node._nodeData = node.gameObject.AddComponentUndo<InAudioData>();
                break;
            case AudioNodeType.Random:
                node._nodeData = node.gameObject.AddComponentUndo<RandomData>();
                for (int i = 0; i < node._children.Count; ++i)
                    (node._nodeData as RandomData).weights.Add(50);
                break;
            case AudioNodeType.Sequence:
                node._nodeData = node.gameObject.AddComponentUndo<InSequenceData>();
                break;
            case AudioNodeType.Multi:
                node._nodeData = node.gameObject.AddComponentUndo<MultiData>();
                break;
            case AudioNodeType.Track:
                node._nodeData = node.gameObject.AddComponentUndo<InTrackData>();
                break;
            case AudioNodeType.Folder:
                var folderData = node.gameObject.AddComponentUndo<InFolderData>();
                //folderData.BankLink = node.GetBank();
                node._nodeData = folderData;
                break;
        }
        return node._nodeData;
    }

    public static void AddNewParent(InAudioNode node, AudioNodeType parentType)
    {
        UndoHelper.RecordObject(new Object[] { node, node._parent, node.GetBank() }, "Undo Add New Parent for " + node.Name);
        var newParent = CreateNode(node.gameObject, node._parent, parentType);
        var oldParent = node._parent;
        newParent.MixerGroup = node.MixerGroup;
        newParent.FoldedOut = true;
        if (node._type == AudioNodeType.Folder)
        {
            InFolderData data = (InFolderData)newParent._nodeData;
            data.BankLink = oldParent.GetBank();
        }
        int index = oldParent._children.FindIndex(node);
        NodeWorker.RemoveFromParent(node);
        node.AssignParent(newParent);

        OnRandomNode(newParent);

        NodeWorker.RemoveFromParent(newParent);
        oldParent._children.Insert(index, newParent);
    }

    private static void OnRandomNode(InAudioNode parent)
    {
        if (parent._type == AudioNodeType.Random)
            (parent._nodeData as RandomData).weights.Add(50);
    }
     
    public static InAudioNode CreateChild(InAudioNode parent, AudioNodeType newNodeType)
    {
        var bank = parent.GetBank();
        UndoHelper.RecordObject(UndoHelper.Array(parent, parent._nodeData, bank), "Undo Node Creation");
        OnRandomNode(parent);

        var child = CreateNode(parent.gameObject, parent, GUIDCreator.Create(), newNodeType);
        parent.FoldedOut = true;
        child.Name = parent.Name + " Child";
        var data = AddDataClass(child);
        if (newNodeType == AudioNodeType.Folder)
        {
            (data as InFolderData).BankLink = parent.GetBank();
        }
        return child;
    }

    public static void ConvertNodeType(InAudioNode node, AudioNodeType newType)
    {
        if (newType == node._type)
            return;
        UndoHelper.DoInGroup(() =>
        {
            UndoHelper.RecordObjectFull(new Object[] {node, node._nodeData}, "Change Node Type");
            
            AudioBankWorker.RemoveNodeFromBank(node);
            
            node._type = newType;
            UndoHelper.Destroy(node._nodeData);
            AddDataClass(node);
            
        });
        
    }

    public static void Duplicate(InAudioNode audioNode)
    {
        UndoHelper.DoInGroup(() =>
        {

            List<Object> toUndo = new List<Object>(AudioBankWorker.GetAllBanks().ConvertAll(b => b as Object));

            toUndo.Add(audioNode._parent);
            toUndo.Add(audioNode.GetBank());

            UndoHelper.RecordObjectFull(toUndo.ToArray(), "Undo Duplication Of " + audioNode.Name);

            if (audioNode._parent._type == AudioNodeType.Random)
            {
                (audioNode._parent._nodeData as RandomData).weights.Add(50);   
            }
            NodeWorker.DuplicateHierarchy(audioNode, (@oldNode, newNode) =>
            { 
                var gameObject = audioNode.gameObject;
                if(oldNode._nodeData != null)
                { 
                    Type type = oldNode._nodeData.GetType();
                    newNode._nodeData = gameObject.AddComponentUndo(type) as InAudioNodeBaseData;
                    EditorUtility.CopySerialized(oldNode._nodeData, newNode._nodeData);
                    if (newNode._type == AudioNodeType.Audio)
                    {
                        AudioBankWorker.AddNodeToBank(newNode);
                    }
                }
            });
        });
    }

    public static void DeleteNode(InAudioNode node)
    {
        UndoHelper.DoInGroup(() =>
        {           
         
            //UndoHelper.RecordObjectFull(UndoHelper.Array(node.Parent, node.Parent.AudioData), "Undo Deletion of " + node.Name);

            if (node._parent._type == AudioNodeType.Random) //We also need to remove the child from the weight list
            {
                var data = node._parent._nodeData as RandomData;
                if (data != null)
                    data.weights.RemoveAt(node._parent._children.FindIndex(node)); //Find in parent, and then remove the weight in the random node
                node._parent._children.Remove(node);
            }
            
            DeleteNodeRec(node);
        
        });
    }

    private static void DeleteNodeRec(InAudioNode node)
    {
        AudioBankWorker.RemoveNodeFromBank(node);

        /*TreeWalker.ForEach(InAudioInstanceFinder.DataManager.EventTree, @event =>
        {
            for (int i = 0; i < @event.ActionList.Count; i++)
            {
                var action = @event.ActionList[i];
                if (action.Target == node)
                {
                    UndoHelper.RegisterFullObjectHierarchyUndo(action);
                }
            }
        });*/

        for (int i = 0; i < node._children.Count; i++)
        {
            DeleteNodeRec(node._children[i]);
        }


        UndoHelper.Destroy(node._nodeData);
        UndoHelper.Destroy(node);
    }
}
}
