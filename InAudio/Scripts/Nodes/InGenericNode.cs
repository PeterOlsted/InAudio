using System.Collections.Generic;
using UnityEngine;

namespace InAudioSystem.Internal
{
    public abstract class InGenericNode<T> : MonoBehaviour //Implicitly implements InTreeNode<T>
    {
        [SerializeField]
        protected string nodeName;
        [SerializeField]
        protected List<T> children;
        [SerializeField]
        protected T parent;
        [SerializeField]
        protected int id;

        public string GetName
        {
            get { return nodeName; }
            set {
                if (nodeName != null)
                {
                    nodeName = value;
                }
            }
        }

        public List<T> _getChildren
        {
            get { return children; }
        }

        public T _getParent
        {
            get { return parent; }
            set { parent = value; }
        }

        public int _ID
        {
            get { return id; }
            set { id = value; }
        }

        private EditorSettings editorSettings = new EditorSettings();
        public EditorSettings EditorSettings
        {
            get { return editorSettings; }
            set { editorSettings = value; }
        }
    }
}
