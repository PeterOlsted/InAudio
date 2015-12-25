#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
    [ExecuteInEditMode]
    public class EditorResources : MonoBehaviour
    {
            
#if UNITY_EDITOR

        public Texture2D DarkSkinBackground;
        public Texture2D LightSkinBackground;
        public Texture White;

        public Texture Plus;
        public Texture Minus;

        public Texture Up;
        public Texture Down;

        public Texture Bank;
        public Texture Dice;
        public Texture List;
        public Texture Event;
        public Texture Tree;
        public Texture Audio;

        public Texture Pause;
        public Texture Play;
        public Texture Stop;

        public Texture Folder;

        public Texture Soloed;
        public Texture NotSolo;

        public Texture Muted;
        public Texture NotMute;

        private static Texture question;
        public static Texture Question
        {
            get
            {
                if(question == null)
                {
                    question = LoadTexture("Question");
                }
                return question;
            }
        }

        private static Texture support;
        public static Texture Support
        {
            get
            {
                if (support == null)
                { 
                    support = LoadTexture("Support");
                }
                return support;
            }
        }

        private static Texture book;
        public static Texture Book
        {
            get
            {
                if (book == null)
                {
                    book = LoadTexture("Book");
                }
                return book;
            }
        }

        //Reordlist icons
        public Texture2D ContainerBackground;
        public Texture2D ContainerBackgroundWhite;
        public Texture2D GrabHandle;
        public Texture2D TitleBackground;
        public Texture2D ItemSplitter;

        public Texture2D GenericColor;

        public Texture2D GetBackground()
        {
            if (EditorGUIUtility.isProSkin)
            {
                return DarkSkinBackground;
            }
            else
            {
                return LightSkinBackground;
            }
        }

        public static EditorResources Instance { get; private set; }

        void Awake()
        {
            Instance = this;
        }

        public void Update()
        {
            Instance = this;
        }

        private static Texture LoadTexture(string name)
        {		
            return AssetDatabase.LoadAssetAtPath(FolderSettings.GetIconPath(name), typeof(Texture2D)) as Texture2D;		
        }
#endif
}
}
