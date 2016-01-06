using System;
using InAudioSystem.ExtensionMethods;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace InAudioSystem
{
    public static class FolderSettings
    {

        private const string Name = "InAudio"; 

        private const string RelativePathResources = "InAudio/";

        public const string AudioLoadData = RelativePathResources + "InAudio AudioSave";
        public const string EventLoadData = RelativePathResources + "InAudio EventSave";
        public const string BankLinkLoadData = RelativePathResources + "InAudio BankLinkSave";
        public const string MusicLoadData = RelativePathResources + "InAudio MusicSave";

#if UNITY_EDITOR
        private readonly static string FullPathResources = AssetFolder + "Resources/InAudio/"; 

        //Prefabs
        public readonly static string GUIUserPrefs = AssetFolder + "Prefabs/GUIUserPrefs.prefab";
        public readonly static string AudioManagerPath = AssetFolder + "Prefabs/InAudio Manager.prefab";
        public readonly static string EmptyTemplatePath = AssetFolder + "Prefabs/PrefabTemplate.prefab";

        //Data
        public readonly static string AudioSaveDataPath = FullPathResources + "InAudio AudioSave.prefab";
        public readonly static string EventSaveDataPath = FullPathResources + "InAudio EventSave.prefab";
        public readonly static string MusicSaveDataPath = FullPathResources + "InAudio MusicSave.prefab";
        public readonly static string InteractiveMusicSaveDataPath = FullPathResources + "InAudio InteractiveMusicSave.prefab";
        public readonly static string BankLinkSaveDataPath = FullPathResources + "InAudio BankLinkSave.prefab";
        public readonly static string AudioDataPath = FullPathResources + "AudioData/";

        //Creating the folders
        public static readonly string CreateFolderResources = AssetFolder + Name + "/Resources/" + Name + "/";
        public static string AudioDataCreateFolder = CreateFolderResources + "AudioData/";

        private static string relativePath;
        private static string AssetFolder
        {
            get
            {
                if (relativePath == null)
                    relativePath = BuildPath();
                return relativePath;
            }
        } 

        public static string GetIconPath(string name)
        {
            return AssetFolder + "Icons/" + name + ".png"; 
        } 

        private static string BuildPath()
        {
            string[] assets = AssetDatabase.FindAssets("RootInAudio");
            if(assets.Length == 1)
            {
                string path = AssetDatabase.GUIDToAssetPath(assets[0]); 
                if (!path.IsNullOrWhiteSpace())
                {
                    string[] inAudioPath = path.Split(new[] {"Resources/InAudio"}, StringSplitOptions.None);
                    if (inAudioPath.Length > 0)
                    {
                        return inAudioPath[0];
                    }
                   
                }
            }
            else
            {
                Debug.LogError("InAudio: Failed to find the root of InAudio. Please reimport the project from the asset store.");
            }

            return "";
        }
#endif
    }

}
