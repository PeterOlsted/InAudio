using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    public static class NodeFactory
    {
        public static InSettingsNode CreateSettingsTree(GameObject go, int level)
        {
            var node = InUndoHelper.AddComponent<InSettingsNode>(go);
            node.GetName = "Settings";
            node._ID = GUIDCreator.Create();
            return node;
        }
    }
}