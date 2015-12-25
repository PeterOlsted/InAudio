using System;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using InAudioSystem.TreeDrawer;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InAudioSystem.InAudioEditor
{

    public abstract class BaseCreatorGUI<T> where T : Object, InITreeNode<T>
    {
        protected InAudioBaseWindow window;

        public InAudioBaseWindow Window { get { return window; }}

        public TreeDrawer<T> treeDrawer;

        public T SelectedNode { get; set; }

        protected bool isDirty;

        protected string lowercaseSearchingFor;
        protected string searchingFor;

        protected BaseCreatorGUI(InAudioBaseWindow window)
        {
            this.window = window;
            treeDrawer = new TreeDrawer<T>(window);
            InAudioBaseWindow.OnScriptReloaded += OnScriptReloaded;
        }

        protected void OnScriptReloaded()
        {
            if (GUIData != null)
            {
                int id = GUIData.SelectedNode;
                var root = Root();
                if (root != null)
                {
                    var foundNode = TreeWalker.FindFirst(Root(), node => node._ID == id);
                    SelectedNode = foundNode;
                    treeDrawer.SelectedNode = foundNode;
                    treeDrawer.ScrollPosition = GUIData.Position;
                }
            }
        }

        private readonly char[] lineSplit = { '\n' };
        private readonly char[] dotSplit = { '.' };
        public void BaseOnGUI()
        {
            HandleVersionCheck();

            isDirty = false;

            if (Event.current.IsKeyDown(KeyCode.W) && Event.current.control)
            {
                if (window != null)
                    window.Close();
                Event.current.UseEvent();
            }
            if (InAudioInstanceFinder.Instance != null && InAudioInstanceFinder.DataManager != null && InAudioInstanceFinder.DataManager.Loaded)
            {
                var root = Root();
                if (GUIData != null)
                {
                    int id = GUIData.SelectedNode;
                    var selectedNode = UpdateSelectedNode(root, id);
                    GUIData.SelectedNode = selectedNode != null ? selectedNode._ID : 0;
                    GUIData.Position = treeDrawer.ScrollPosition;
                    EditorUtility.SetDirty(InAudioInstanceFinder.InAudioGuiUserPrefs);
                }
                
                
            }
        }

        private void HandleVersionCheck()
        {
            if (PlayerPrefs.HasKey("InAudioUpdateInfo"))
            {
                string info = PlayerPrefs.GetString("InAudioUpdateInfo");

                string version = null;
                if (PlayerPrefs.HasKey("InAudioStoredVersion"))
                {
                    version = PlayerPrefs.GetString("InAudioStoredVersion");
                }
                EditorGUILayout.BeginHorizontal();
                try
                {
                    var split = info.Split(lineSplit);

                    if (IsNewer(version, split[0]))
                    {
                        string text = split[1];

                        EditorGUILayout.LabelField(text, EditorStyles.boldLabel);
                        var rect = GUILayoutUtility.GetLastRect();
                        var dimensions = new Rect(rect.width - 350, 0, 200, EditorGUIUtility.singleLineHeight);

                        if (GUI.Button(dimensions, "Open the Asset Store"))
                        {
                            Application.OpenURL("https://www.assetstore.unity3d.com/en/#!/content/15609");
                        }

                        dimensions.x = rect.width - 25;
                        dimensions.width = 30;
                        if (GUI.Button(dimensions, "X"))
                        {
                            PlayerPrefs.SetString("InAudioStoredVersion", split[0]);
                        }
                    }
                }
                catch (Exception)
                {
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private bool IsNewer(string currentVersion, string newVersion)
        {
            var currentSplit = currentVersion.Split(dotSplit);
            var newSplit = newVersion.Split(dotSplit);

            for (int i = 0; i < Math.Min(currentSplit.Length, newSplit.Length); i++)
            {
                if (Int32.Parse(newSplit[i]) > Int32.Parse(currentSplit[i]))
                    return true;
            }
            return false;
        }


        protected abstract T Root();

        public virtual void OnEnable()
        {
            treeDrawer.Filter(n => false);
            treeDrawer.OnContext = OnContext;
            treeDrawer.OnDrop = OnDrop;
            treeDrawer.CanDropObjects = CanDropObjects;
            treeDrawer.OnNodeDraw = OnNodeDraw;
        }

        protected void DrawSearchBar()
        {
            EditorGUILayout.BeginHorizontal();
            GUI.SetNextControlName("SearchBar");
            EditorGUILayout.LabelField("Filter", GUILayout.Width(45));
            var content = EditorGUILayout.TextField(searchingFor);

            if (content != searchingFor)
            {
                searchingFor = content;
                lowercaseSearchingFor = searchingFor.ToLower().Trim();
                treeDrawer.Filter(SearchFilter);

            }

            if (GUILayout.Button("x", GUILayout.Width(25)) && Event.current.type != EventType.Repaint)
            {
                treeDrawer.Filter(SearchFilter);
                searchingFor = "";
                lowercaseSearchingFor = "";
                treeDrawer.FocusOnSelectedNode();

                GUI.FocusControl(null);
            }
            /**/
            GUILayout.EndHorizontal();

        }

        public virtual void OnUpdate()
        {
        }

        protected virtual bool OnNodeDraw(T node, bool isSelected, out bool clicked)
        {
            return GenericTreeNodeDrawer.Draw(node, isSelected, out clicked);
        }

        protected abstract bool CanDropObjects(T node, Object[] objects);

        protected abstract void OnDrop(T node, Object[] objects);

        protected abstract void OnContext(T node);

        public virtual void Find(T node)
        {
            TreeWalker.ForEach(InAudioInstanceFinder.DataManager.AudioTree, audioNode =>
            {
                if (audioNode != null)
                    audioNode.Filtered = false;
            });
            //searchingFor = node._ID.ToString();
            //lowercaseSearchingFor = searchingFor.ToLower().Trim();
            //treeDrawer.Filter(SearchFilter);
            TreeWalker.ForEachParent(node, n => n.IsFoldedOut = true);
            SelectedNode = node;
            treeDrawer.SelectedNode = node;
            treeDrawer.FocusOnSelectedNode();
        }

        protected virtual bool SearchFilter(T node)
        {
            if (string.IsNullOrEmpty(lowercaseSearchingFor))
                return false;
            else
            {
                //Check name
                bool nameFiltered = !node.GetName.ToLower().Contains(lowercaseSearchingFor);
                //If name doesn't match, check if ID matches
                if (nameFiltered)
                {
                    nameFiltered = !node._ID.ToString().Contains(lowercaseSearchingFor);
                }
                return nameFiltered;
            }
        }

        private T UpdateSelectedNode(T root, int id)
        {
            if (treeDrawer.SelectedNode == null && root != null && root._getChildren != null)
            {
                var found = TreeWalker.FindById(root, id);
                treeDrawer.ScrollPosition = GUIData.Position;
                //Debug.Log("found node", found);
                if (found != null)
                {
                    treeDrawer.SelectedNode = found;
                }
                else
                {
                    treeDrawer.SelectedNode = root;
                }
            }

            SelectedNode = treeDrawer.SelectedNode;

            return treeDrawer.SelectedNode;
        }

        protected abstract GUIPrefs GUIData { get; }
    }
}