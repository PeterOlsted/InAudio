using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEngine;

namespace InAudioSystem.Runtime
{
    public static class MusicUpdater
    {
        public static void SetInitialSettings(InMusicNode node, float parentVolume, float parentPitch)
        {
            float volume = node._minVolume * parentVolume;
            float pitch = node._minPitch * parentPitch;
            if (Application.isPlaying)
            {
                node.runtimeMute = node._mute;
                node.runtimeSolo = node._solo;
                node.runtimeVolume = node._minVolume;
                node.runtimePitch = node._minPitch;
                var playingInfo = node.PlayingInfo;
                var sources = playingInfo.Players;
                int sourceCount = sources.Count;
                for (int i = 0; i < sourceCount; i++)
                {
                    sources[i].pitch = pitch;
                    sources[i].SetLoudness(volume);
                }
            }
            var children = node._children;
            int childCount = children.Count;

            for (int i = 0; i < childCount; i++)
            {
                SetInitialSettings(children[i], volume, pitch);
            }
        }

        public static void UpdateVolumePitch(InMusicNode node, float parentVolume, float parentPitch, bool areAnySolo)
        {
            var group = node as InMusicGroup;
            var children = node._children;
            int childCount = children.Count;
            float volume;
            var playingInfo = node.PlayingInfo;
            if (group != null)
            {
                bool checkPlayer;
#if UNITY_EDITOR
                checkPlayer = Application.isPlaying;
#else
                checkPlayer = true;
#endif

                if (group.Playing && checkPlayer)
                {
                    var players = playingInfo.Players;

                    if (players.Count > 0)
                    {
                        var player = players[0];
                        if (!player.isPlaying)
                        {
                            CleanupMusicNode(group);
                        }
                    }
                }
            }

            if (playingInfo.Fading && group != null)
            {
                float currentTime = Time.time;
                if (currentTime >= playingInfo.EndTime)
                {
                    
                    group.runtimeVolume = playingInfo.TargetVolume;
                    playingInfo.Fading = false;

                    if (playingInfo.DoAtEnd == MusicState.Stopped)
                    {
                        InAudio.Music.Stop(group);
                    }
                    else if (playingInfo.DoAtEnd == MusicState.Paused)
                    {
                        InAudio.Music.Pause(group);
                    }
                    
                }
                else if (currentTime < playingInfo.StartTime)
                {
                    //Do nothing
                }
                else
                {
                    var duration = playingInfo.EndTime - playingInfo.StartTime;
                    var left = playingInfo.EndTime - currentTime;
                    node.runtimeVolume = AudioTween.DirectTween(playingInfo.TweenType, playingInfo.StartVolume, playingInfo.TargetVolume, 1 - left / duration);
                }
            }
                
            if(!playingInfo.AffectedByMute)
                volume = GetVolume(node) * parentVolume;
            else
                volume = 0f;

            if (areAnySolo && !playingInfo.IsSoloed && !playingInfo.HasSoloedChild)
            {
                volume = 0;
            }

            var sources = playingInfo.Players;
            int sourceCount = sources.Count;

            var pitch = GetPitch(node) * parentPitch;
            if (Application.isPlaying)
            {
                for (int i = 0; i < sourceCount; i++)
                {
                    sources[i].pitch = pitch;
                    sources[i].SetLoudness(volume);
                }
            }
            playingInfo.FinalVolume = volume;

            for (int i = 0; i < childCount; i++)
            {
                UpdateVolumePitch(children[i], volume, pitch, areAnySolo);
            }
        }

       

        //Only handles one node, remember to clear its children
        public static void CleanupMusicNode(InMusicGroup musicGroup)
        {
            AudioSource player;
            var playingInfo = musicGroup.PlayingInfo;
            var players = playingInfo.Players;
            playingInfo.State = MusicState.Stopped;
            playingInfo.Fading = false;
            for (int i = 0; i < players.Count; i++)
            {
                player = players[i];
                if (player != null)
                {
                    player.clip = null;

                    InAudioInstanceFinder.InMusicPlayerPool.ImmidiateRelease(player);
                }
            }
            players.Clear();
        }

        public static float GetVolume(InMusicNode node)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return node.runtimeVolume;
            else
                return node._minVolume;
#else 
            return node.runtimeVolume;
#endif

        }

        public static bool IsMute(InMusicNode node)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return node.runtimeMute;
            else
                return node._mute;
#else 
            return node.runtimeMute;
#endif

        }

        public static void SetMute(InMusicNode group, bool value)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                group.runtimeMute = value;
            else
                group._mute = value;
#else 
            group.runtimeMute = value;
#endif

        }

        public static void FlipMute(InMusicNode group)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                group.runtimeMute = !group.runtimeMute;
            else
                group._mute = !group._mute;
#else 
            group.runtimeMute = !group.runtimeMute;
#endif

        }

        public static bool IsSolo(InMusicNode group)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return group.runtimeSolo;
            else
                return group._solo;
#else 
            return group.runtimeSolo;
#endif

        }

        public static void SetSolo(InMusicNode group, bool value)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                group.runtimeSolo = value;
            else
                group._solo = value;
#else 
            group.runtimeSolo = value;
#endif

        }

        public static void FlipSolo(InMusicNode group)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                group.runtimeSolo = !group.runtimeSolo;
            else
                group._solo = !group._solo;
#else 
            group.runtimeSolo = !group.runtimeSolo;
#endif

        }

        public static float GetPitch(InMusicNode node)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return node.runtimePitch;
            else
                return node._minPitch;
#else 
            return node.runtimePitch;
#endif

        }

        public static bool UpdateSoloMute(InMusicNode root)
        {
            bool anySolo = false;
            bool hasSolodChild;
            UpdateSolo(root, false, ref anySolo, out hasSolodChild);
            UpdateMute(root, false);
            return anySolo;
        }

        private static void UpdateMute(InMusicNode node, bool mutedParent)
        {
            var children = node._children;
            int childCount = children.Count;
            bool muted = false;

            muted = IsMute(node) | mutedParent;
            node.PlayingInfo.AffectedByMute = muted;
                

            for (int i = 0; i < childCount; i++)
            {
                UpdateMute(children[i], muted);
            }
        }

        private static void UpdateSolo(InMusicNode node, bool soloedParent, ref bool anySoloed, out bool hasSoloedChild)
        {
            var children = node._children;
            int childCount = children.Count;

            bool solo = soloedParent;
            
            var playingInfo = node.PlayingInfo;
            solo = soloedParent | IsSolo(node);
            playingInfo.IsSoloed = solo;
            anySoloed |= solo;
            
            bool solodChild = false;
            for (int i = 0; i < childCount; i++)
            {
                bool solod;
                UpdateSolo(children[i], solo, ref anySoloed, out solod);
                solodChild |= solod;
            }
            hasSoloedChild = solo | solodChild;
            if (solodChild)
            {
                node.PlayingInfo.HasSoloedChild = true;
            }
        }
    }
}