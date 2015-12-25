using System.Collections.Generic;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEditor;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
 
public static class AudioBankWorker {
    private static InAudioBankLink CreateNode(GameObject go, InAudioBankLink parent, int guid)
    {
        var node = go.AddComponentUndo<InAudioBankLink>();
        node._guid = guid;
        node.IsFoldedOut = true;
        node.AssignParent(parent);
        return node;
    }

    private static InAudioBankLink CreateRoot(GameObject go, int guid)
    {
        var node = go.AddComponent<InAudioBankLink>();
        node._guid = guid;
        node.IsFoldedOut = true;
        node._name = "Root";
        node._type = AudioBankTypes.Folder;
        return node;
    }
 
    public static InAudioBankLink CreateFolder(GameObject go, InAudioBankLink parent, int guid)
    {
        var node = CreateNode(go, parent, guid);
        node._name = parent._name + " Child Folder";
        node._type = AudioBankTypes.Folder;
        return node;
    }

    public static InAudioBankLink CreateBankLink(GameObject go, InAudioBankLink parent, int guid)
    {
        var node = CreateNode(go, parent, guid);
        node._name = parent._name + " Child"; 
        node._type = AudioBankTypes.Bank;
        return node;
    }

    public static InAudioBankLink CreateTree(GameObject go)
    {
        var root = CreateRoot(go, GUIDCreator.Create());
        CreateBankLink(go, root, GUIDCreator.Create());
        return root;
    }

    public static void AddNodeToBank(InMusicGroup node)
    {
        var bank = node.GetBank();
        if (bank != null)
        {
            bank._bankData.Add(CreateBankDataItem(node));
            EditorUtility.SetDirty(bank);
        }
        else
        {
            Debug.LogError("InAudio: Could not add node to bank as bank could not be found");
        }
    }

    public static void AddNodeToBank(InAudioNode node)
    {
        var bank = node.GetBank();
        if (bank != null)
        {
            bank._bankData.Add(CreateBankDataItem(node));
            EditorUtility.SetDirty(bank);
        }
        else
        {
            Debug.LogError("InAudio: Could not add node to bank as bank could not be found");
        }
    }

    public static void AddMusicNodeToBank(InMusicFolder node, AudioClip clip)
    {
        var bank = node.BankConnection;
        if (bank != null)
        {
            //TODO FIX
            //if (bank.LazyBankFetch == null)
            //{
            //    Debug.LogError("Please open the InAudio Integrity window and \"Fix Bank integrity\"\n"
            //        + "Bank " + bank.Name + " with id " + bank.ID + " does not have an attached bank storage.\n");
            //}
            //else
            //{
            //    bank.LazyBankFetch.Clips.Add(CreateTuple(node, clip));
            //    EditorUtility.SetDirty(bank.LazyBankFetch);
            //}

        }
    }

    public static void RemoveNodeFromBank(InAudioNode node)
    {
        var bankLink = node.GetBank();
        if (bankLink != null)
        {
            InUndoHelper.RecordObjectFull(bankLink, "Node from bank removal");
            var bank = bankLink._bankData;
            bank.RemoveAll(b => b.AudioNode == node);
        }
    }

    private static BankData CreateBankDataItem(InMusicGroup node)
    {
        BankData data = new BankData();
        data.MusicNode = node;
        return data;
    }

    private static BankData CreateBankDataItem(InAudioNode node)
    {
        BankData data = new BankData();
        data.AudioNode = node;
        return data;
    }


    //TODO MUSICUPDATE
    public static void RebuildBanks()
    {
        InAudioBankLink rootBank = InAudioInstanceFinder.DataManager.BankLinkTree; 
        InAudioNode audioRoot = InAudioInstanceFinder.DataManager.AudioTree;
        InMusicNode musicRoot = InAudioInstanceFinder.DataManager.MusicTree; 
        TreeWalker.ForEach(rootBank, DeleteAllNodesFromBanks);

        TreeWalker.ForEach(audioRoot, node =>
        {
            var folderData = node._nodeData as InFolderData;
            SetBankIfNotNull(folderData, rootBank);
        });
        TreeWalker.ForEach(musicRoot, node =>
        {
            var folderData = node as InMusicFolder;
            SetBankIfNotNull(folderData, rootBank);
        });

        TreeWalker.ForEach(audioRoot, AddNodesToBank);
        TreeWalker.ForEach(musicRoot, AddNodesToBank);

        //Set the bank of the root node if it is missing
        InFolderData inFolderData = audioRoot._nodeData as InFolderData;
        if(inFolderData != null && inFolderData.BankLink == null)
        {
            inFolderData.BankLink = TreeWalker.FindFirst(rootBank, link => link._type == AudioBankTypes.Bank);
        }
        if (inFolderData.BankLink != null)
            TreeWalker.ForEach(audioRoot, SetBanks);
    }

    private static void SetBankIfNotNull(IBankUsage folderData, InAudioBankLink rootBank)
    {
        if (folderData != null && folderData.BankConnection == null)
        {
            folderData.BankConnection = rootBank;
        }
    }

  private static void SetBanks(InAudioNode node)
    {
        if(node.IsRootOrFolder)
        {
            InFolderData inFolderData = (node._nodeData as InFolderData);
            if(inFolderData != null)
            {
                if (inFolderData.BankLink == null)
                    inFolderData.BankLink = node.GetBank();
            }
        }
    }

    public static void ChangeAudioNodeBank(InAudioNode node, InAudioBankLink newBank)
    {
        var all = GetAllBanks();
        InUndoHelper.RecordObject(all.ToArray().AddObj(node._nodeData), "Changed Bank");
        InFolderData data = (node._nodeData as InFolderData);
        data.BankLink = newBank;
        RebuildBanks();
    }

    public static void ChangeMusicNodeBank(InMusicNode node, InAudioBankLink newBank)
    {
        var all = GetAllBanks();
        InUndoHelper.RecordObject(all.ToArray().AddObj(node), "Changed Bank");
        InMusicFolder data = (node as InMusicFolder);
        data._bankLink = newBank;
        RebuildBanks();
    }

    public static void ChangeBankOverride(InAudioNode node)
    {
        var all = GetAllBanks();
        InUndoHelper.RecordObject(all.ToArray().AddObj(node._nodeData), "Changed Bank");
        InFolderData data = (node._nodeData as InFolderData);
        data.OverrideParentBank = !data.OverrideParentBank;
        RebuildBanks();        
    }

    public static void ChangeBankOverride(InMusicFolder node)
    {
        var all = GetAllBanks();
        InUndoHelper.RecordObject(all.ToArray().AddObj(node), "Changed Bank");
        node._overrideParentBank = !node._overrideParentBank;
        RebuildBanks();
    }

    private static void AddNodesToBank(InAudioNode audioNode)
    {
        if (audioNode._type == AudioNodeType.Audio)
        {
            var nodeData = audioNode._nodeData as InAudioData;
            if (nodeData != null)
            {
                AddNodeToBank(audioNode);
            }
        }
    }

    private static void AddNodesToBank(InMusicNode musicNode)
    {
        if (musicNode._type == MusicNodeType.Music)
        {
            var musicGroup = musicNode as InMusicGroup;
            if (musicGroup != null)
            {
                AddNodeToBank(musicGroup);
            }
        }
    }

    public static List<InAudioBankLink> GetAllBanks()
    {
        return TreeWalker.FindAll(InAudioInstanceFinder.DataManager.BankLinkTree, node => node);
    }

    public static void MarkAllBanksForUndo()
    {
        List<Object> toUndo = new List<Object>(GetAllBanks().ConvertAll(b => b as Object));

        InUndoHelper.RecordObjectFull(toUndo.ToArray(), "Undo Duplication");
    }

    private static void DeleteAllNodesFromBanks(InAudioBankLink audioBankLink)
    {
        if (audioBankLink._type == AudioBankTypes.Bank)
        {
            audioBankLink._bankData.Clear();
        }
    }

    public static void DeleteBank(InAudioBankLink toDelete)
    {        
        InUndoHelper.DoInGroup(() =>
        {
            InUndoHelper.RecordObject(InAudioInstanceFinder.DataManager.AudioTree.gameObject.GetComponents<MonoBehaviour>().Add(toDelete).Add(InAudioInstanceFinder.DataManager.MusicTree.gameObject.GetComponents<MonoBehaviour>()), "Bank detele");
            toDelete._parent._getChildren.Remove(toDelete);
            InUndoHelper.Destroy(toDelete);
        });
        
        
    }

    public static void DeleteFolder(InAudioBankLink toDelete)
    {
        InUndoHelper.DoInGroup(() =>
        {
            InUndoHelper.RecordObjectFull(toDelete._parent, "Delete Bank Folder");
            toDelete._parent._getChildren.Remove(toDelete);
            InUndoHelper.Destroy(toDelete);
        });
    }
}
}
