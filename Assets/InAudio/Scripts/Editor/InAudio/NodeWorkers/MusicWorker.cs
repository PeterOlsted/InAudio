using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{

    public static class MusicWorker
    {
        internal static InMusicNode CreateTree(GameObject go, int levelSize)
        {
            var Tree = CreateRoot(go, GUIDCreator.Create());
            for (int i = 0; i < levelSize; ++i)
            {
                var node = CreateFolder(go, Tree);
                node._name = "Music Folder " + i;
            }
            return Tree;
        }

        public static InMusicNode CreateRoot(GameObject go, int guid)
        {
            var node = go.AddComponent<InMusicFolder>();
            node._guid = guid;
            node._type = MusicNodeType.Root;
            node._overrideParentBank = true;
            node.FoldedOut = true;
            node._name = "Music Root";
            TreeWalker.FindFirst(InAudioInstanceFinder.DataManager.BankLinkTree, link => link._type == AudioBankTypes.Bank);
            return node;
        }


        public static InMusicFolder CreateFolder(GameObject go, InMusicNode parent)
        {
            var newNode = CreateNode <InMusicFolder>(go, parent, GUIDCreator.Create());
            newNode._type = MusicNodeType.Folder;
            newNode._bankLink = TreeWalker.FindFirst(InAudioInstanceFinder.DataManager.BankLinkTree,
                link => link._type == AudioBankTypes.Bank);
            return newNode;
        }

        public static InMusicGroup CreateMusicGroup(InMusicNode parent, string name)
        {
            var newNode = CreateMusicGroup(parent);
            newNode._name = name;
            return newNode;
        }

        public static InMusicGroup CreateMusicGroup(InMusicNode parent)
        {
            var newNode = CreateNode<InMusicGroup>(parent.gameObject, parent, GUIDCreator.Create());
            newNode._type = MusicNodeType.Music;
            return newNode;
        }

        private static T CreateNode<T>(GameObject go, InMusicNode parent, int guid) where T : InMusicNode
        {
            var node = go.AddComponentUndo<T>();

            node._guid = guid;
            node._name = parent._name + " Child";
            node._mixerGroup = parent._mixerGroup;

            node.AssignParent(parent);

            return node;
        }

        public static void Duplicate(GameObject go, InMusicNode current, InMusicNode parent)
        {
            NodeWorker.DuplicateHierarchy(current, parent, go, (node, musicNode) => {});
        }
    }

}