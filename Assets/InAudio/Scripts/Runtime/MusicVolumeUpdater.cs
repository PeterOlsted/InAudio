using System.Collections.Generic;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEngine;

namespace InAudioSystem.Runtime
{
    public static class MusicVolumeUpdater
    {
        public static void SetInitialSettings(InMusicNode node, float parentVolume, float parentPitch)
        {
            var group = node as InMusicGroup;
            float volume = node._minVolume * parentVolume;
            float pitch = node._minPitch * parentPitch;
            if (group != null && Application.isPlaying)
            {
                group.runtimeMute = group._mute;
                group.runtimeSolo = group._solo;
                group.runtimeVolume = node._minVolume;
                group.runtimePitch = node._minPitch;
                var playingInfo = group.PlayingInfo;
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
            float volume = 1f;
            float pitch = 1f;
            if (group != null)
            {
                var playingInfo = group.PlayingInfo;

                bool checkPlayer;
#if UNITY_EDITOR
                checkPlayer = Application.isPlaying;
#else
                checkPlayer = true;
#endif

                if (group.Playing && checkPlayer)
                {
                    var players = playingInfo.Players;
                    var playerCount = players.Count;
                    if (players.Count > 0)
                    {
                        var player = players[0];
                        if (!player.isPlaying)
                        {
                            for (int i = 0; i < playerCount; i++)
                            {
                                player = players[i];
                                player.clip = null;
                                playingInfo.State = MusicState.Stopped;
                                playingInfo.Fading = false;
                                InAudioInstanceFinder.InMusicPlayerPool.ImmidiateRelease(player);
                            }
                            players.Clear();
                        }
                    }
                }

                if (playingInfo.Fading)
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
                    else
                    {
                        var duration = playingInfo.EndTime - playingInfo.StartTime;
                        var left = playingInfo.EndTime - currentTime;
                        group.runtimeVolume = AudioTween.DirectTween(playingInfo.TweenType, playingInfo.StartVolume, playingInfo.TargetVolume, 1 - left / duration);
                    }
                }
                
                if(!playingInfo.AffectedByMute)
                    volume = GetVolume(group) * parentVolume;
                else
                    volume = 0f;

                if (areAnySolo && !playingInfo.IsSoloed && !playingInfo.HasSoloedChild)
                {
                    volume = 0;
                }

                var sources = playingInfo.Players;
                int sourceCount = sources.Count;


                pitch = GetPitch(group) * parentPitch;
                if (Application.isPlaying)
                {
                    for (int i = 0; i < sourceCount; i++)
                    {
                        sources[i].pitch = pitch;
                        sources[i].SetLoudness(volume);
                    }
                }
                playingInfo.FinalVolume = volume;

                
            }

            for (int i = 0; i < childCount; i++)
            {
                UpdateVolumePitch(children[i], volume, pitch, areAnySolo);
            }
        }

        public static float GetVolume(InMusicGroup group)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return group.runtimeVolume;
            else
                return group._minVolume;
#else 
            return group.runtimeVolume;
#endif

        }

        public static bool IsMute(InMusicGroup group)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return group.runtimeMute;
            else
                return group._mute;
#else 
            return group.runtimeMute;
#endif

        }

        public static void SetMute(InMusicGroup group, bool value)
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

        public static void FlipMute(InMusicGroup group)
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

        public static bool IsSolo(InMusicGroup group)
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

        public static void SetSolo(InMusicGroup group, bool value)
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

        public static void FlipSolo(InMusicGroup group)
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

        public static float GetPitch(InMusicGroup group)
        {
#if UNITY_EDITOR
            if (Application.isPlaying)
                return group.runtimePitch;
            else
                return group._minPitch;
#else 
            return group.runtimePitch;
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
            var group = node as InMusicGroup;
            var children = node._children;
            int childCount = children.Count;
            bool muted = false;
            if (group != null)
            {
                muted = IsMute(group) | mutedParent;
                group.PlayingInfo.AffectedByMute = muted;
                
            }

            for (int i = 0; i < childCount; i++)
            {
                UpdateMute(children[i], muted);
            }
        }

        private static void UpdateSolo(InMusicNode node, bool soloedParent, ref bool anySoloed, out bool hasSoloedChild)
        {
            var group = node as InMusicGroup;
            var children = node._children;
            int childCount = children.Count;

            bool solo = soloedParent;
            if (group != null)
            {
                var playingInfo = group.PlayingInfo;
                solo = soloedParent | IsSolo(group);
                playingInfo.IsSoloed = solo;
                anySoloed |= solo;
            }
            bool solodChild = false;
            for (int i = 0; i < childCount; i++)
            {
                bool solod;
                UpdateSolo(children[i], solo, ref anySoloed, out solod);
                solodChild |= solod;
            }
            hasSoloedChild = solo | solodChild;
            if (group != null && solodChild)
            {
                var playingInfo = group.PlayingInfo;
                playingInfo.HasSoloedChild = true;
            }
        }
    }
}