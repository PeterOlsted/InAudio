using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public class InteractiveMusicWorker : MonoBehaviour
    {
        public static InInteractiveMusic CreateTree(GameObject go, int levelSize)
        {
            var Tree = CreateRoot(go, GUIDCreator.Create());
            for (int i = 0; i < levelSize; ++i)
            {
                CreateFolder(go, Tree);
            }
            return Tree;
        }

        public static InInteractiveMusic CreateRoot(GameObject go, int guid)
        {
            var node = go.AddComponent<InInteractiveMusic>();
            node.guid = guid;
            node.type = InteractiveMusicNodeType.Root;
            node.foldedOut = true;
            node.nodeName = "Interactive Music Root";
            return node;
        }

        public static InInteractiveMusic CreateFolder(GameObject go, InInteractiveMusic parent)
        {
            var newNode = CreateNode<InInteractiveMusic>(go, parent, GUIDCreator.Create());
            newNode.type = InteractiveMusicNodeType.Folder;
            return newNode;
        }

        private static T CreateNode<T>(GameObject go, InInteractiveMusic parent, int guid) where T : InInteractiveMusic
        {
            var node = go.AddComponentUndo<T>();

            node.guid = guid;
            node.nodeName = parent.name + " Child";

            node.parent = parent;

            return node;
        }

    }
}