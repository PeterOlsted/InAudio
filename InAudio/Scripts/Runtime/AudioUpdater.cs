using UnityEngine;

namespace InAudioSystem.Runtime
{
    public static class AudioUpdater
    {

        public static void AudioTreeInitialVolume(InAudioNode node, float parentVolume)
        {
            var folderData = node._nodeData as InFolderData;
            var children = node._children;
            int childCount = children.Count;

            if (folderData != null)
            {
                folderData.runtimeVolume = folderData.VolumeMin;
                folderData.hiearchyVolume = folderData.runtimeVolume * parentVolume;


                for (int i = 0; i < childCount; i++)
                {
                    AudioTreeUpdate(children[i], folderData.hiearchyVolume);
                }
            }
        }

        public static void AudioTreeUpdate(InAudioNode node, float parentVolume)
        {
            var folderData = node._nodeData as InFolderData;
            var children = node._children;
            int childCount = children.Count;


            if (folderData != null)
            {
#if UNITY_EDITOR
                bool checkPlayer = Application.isPlaying;
                if (!Application.isPlaying)
                {
                    folderData.runtimeVolume = folderData.VolumeMin;
                }
#else
                bool checkPlayer = true;
#endif
                float volume = folderData.runtimeVolume * parentVolume;
                folderData.hiearchyVolume = volume;

                for (int i = 0; i < childCount; i++)
                {
                    AudioTreeUpdate(children[i], volume);
                }

                if (checkPlayer)
                {
                    for (int i = 0; i < folderData.runtimePlayers.Count; i++)
                    {
                        var player = folderData.runtimePlayers[i];
                        player.internalSetFolderVolume(folderData.hiearchyVolume);
                        player.internalUpdate();
                    }
                }
            }
        }
    }
}
