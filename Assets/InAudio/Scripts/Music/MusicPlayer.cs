using System;
using InAudioLeanTween;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using InAudioSystem.Runtime;
using UnityEngine;

namespace InAudioSystem
{
    public class MusicPlayer : MonoBehaviour
    {
        #region Play
        public void Play(InMusicGroup toPlay)
        {
            if (toPlay == null)
            {
                Debug.LogError("InAudio: Cannot play Null Music group");
                return;
            }

            PlayAt(GetParent(toPlay), AudioSettings.dspTime + 0.1f);
        }

        public void PlayWithFadeIn(InMusicGroup musicGroup, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            var parent = GetParent(musicGroup);
            Play(parent);
            SetVolume(parent, 0);
            Fade(parent, 1f, duration, tweenType);
        }

        public void PlayWithFadeIn(InMusicGroup musicGroup, float targetVolume, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            var parent = GetParent(musicGroup);
            Play(parent);
            SetVolume(parent, 0);
            Fade(parent, targetVolume, duration, tweenType);
        }

        public void PlayAt(InMusicGroup musicGroup, double absoluteDSPTime)
        {
            if (musicGroup == null)
            {
                Debug.LogError("InAudio: Cannot play 'Null' music group");
                return;
            }

            var parent = GetParent(musicGroup);
            var playingInfo = parent.PlayingInfo;
            if (HandleExcistingPlay(parent, ref playingInfo))
                return;

            double playTime = absoluteDSPTime;
            PlayMusicGroup(musicGroup, playTime, 0);
        }

        public void PlayAtBar(InMusicGroup musicGroup)
        {
            if (musicGroup == null)
            {
                Debug.LogError("InAudio: Cannot play 'Null' music group");
                return;
            }

            var parent = GetParent(musicGroup);
            var playingInfo = parent.PlayingInfo;
            if (HandleExcistingPlay(parent, ref playingInfo))
                return;

            //double playTime = absoluteDSPTime;
            PlayMusicGroup(musicGroup, 0, 0);
        }

        //private SignaturePlayer signaturePlayer;
        //public SignaturePlayer Signature
        //{
        //    get
        //    {
        //        if(signaturePlayer == null)
        //        {
        //            signaturePlayer = new SignaturePlayer();
        //        }
        //        return signaturePlayer;
        //    }
        //}

        public class SignaturePlayer
        {
            public void PlayAtBeat(InMusicGroup musicGroup)
            {
                if (musicGroup == null)
                {
                    Debug.LogError("InAudio: Cannot play 'Null' music group");
                    return;
                }

                var parent = GetParent(musicGroup);
                var playingInfo = parent.PlayingInfo;
                if (HandleExcistingPlay(parent, ref playingInfo))
                    return;

                //double playTime = absoluteDSPTime;
                PlayMusicGroup(musicGroup, 0, 0);
            }
        }
        

        public void PlayAt(InMusicGroup musicGroup, double absoluteDSPTime, int skipSamples)
        {
            if (musicGroup == null)
            {
                Debug.LogError("InAudio: Cannot play 'Null' music group");
                return;
            }

            var parent = GetParent(musicGroup);
            var playingInfo = parent.PlayingInfo;
            if (HandleExcistingPlay(parent, ref playingInfo))
            {
                return;
            }

            double playTime = absoluteDSPTime;
            PlayMusicGroup(musicGroup, playTime, skipSamples);
        }

        #endregion

        #region Volume

        public void SetVolume(InMusicNode musicNode, float volume)
        {
            musicNode.Volume = volume;
            //Is updated in the beginning of the next frame in InAudio.Update via the MusicUpdater class
        }

        public float GetVolume(InMusicNode musicNode)
        {
            return musicNode.Volume;
        }

        /// <summary>
        /// Get the initial volume of this node, independent from the volume while playing and the hierarchy
        /// </summary>
        /// <param name="musicNode"></param>
        /// <returns></returns>
        public float GetInitialVolume(InMusicNode musicNode)
        {
            //Do not change this value, except from the GUI as it will change the prefab, even in Play mode.
            return musicNode._minVolume;
        }

        /// <summary>
        /// Gets the parent of the musicGroup, and fades all other nodes out than the parameter
        /// </summary>
        /// <param name="musicNode">The child to focus on</param>
        /// <param name="targetVolume">The new volume of it</param>
        /// <param name="otherTargetVolume">The volume of the other nodes than the musicGroup parameter</param>
        /// <param name="duration">How long it should take to fade to it</param>
        /// <param name="twenType">How to tween the values</param>
        public void FocusOn(InMusicNode musicNode, float targetVolume, float otherTargetVolume, float duration, LeanTweenType twenType = LeanTweenType.easeInOutQuad)
        {
            var parent = musicNode._parent;
            if (parent.IsRootOrFolder)
            {
                throw new ArgumentException("InAudio: Cannot pass MusicGroup which parent is not a Music Group");
            }

            for (int i = 0; i < parent._children.Count; i++)
            {
                var child = parent._children[i];
                if (child != musicNode)
                {
                    FadeVolume(child as InMusicGroup, otherTargetVolume, duration);
                }
                else
                {
                    FadeVolume(child as InMusicGroup, targetVolume, duration);
                }
            }
        }

        public void SetVolumeOfAllChildren(InMusicNode musicNode, float targetVolume, float duration, LeanTweenType twenType = LeanTweenType.easeInOutQuad)
        {
            for (int i = 0; i < musicNode._children.Count; i++)
            {
                FadeVolume(musicNode._children[i], targetVolume, duration);
            }
        }

        #endregion

        #region Pitch

        public void SetPitch(InMusicGroup musicGroup, float pitch)
        {
            musicGroup.Pitch = pitch;
            //Is updated in the beginning of the next frame in InAudio.Update via the MusicUpdater class
        }

        public float GetPitch(InMusicGroup musicGroup)
        {
            return musicGroup.runtimePitch;
        }


        /// <summary>
        /// Get the initial pitch of this node, independent from the volume pitch playing and the hierarchy
        /// </summary>
        /// <param name="musicGroup"></param>
        /// <returns></returns>
        public float GetInitialPitch(InMusicGroup musicGroup)
        {
            //Do not change this value, except from the GUI as it will change the prefab, even in Play mode.
            return musicGroup._minPitch;
        }

        #endregion

        #region Stop & Pause

        public void Pause(InMusicGroup musicGroup)
        {
            if (musicGroup == null)
            {
                return;
            }

            PausePlayers(GetParent(musicGroup));
        }

        public void Stop(InMusicGroup musicGroup)
        {
            var playingInfo = musicGroup.PlayingInfo;
            if (playingInfo != null)
            {
                StopPlayersNow(GetParent(musicGroup));
            }
        }

        public void StopAt(InMusicGroup musicGroup, double absoluteDSPTime)
        {
            StopPlayersAt(GetParent(musicGroup), absoluteDSPTime);
        }

        public bool IsPlaying(InMusicGroup musicGroup)
        {
            return musicGroup.Playing;
        }

        public bool IsPaused(InMusicGroup musicGroup)
        {
            return musicGroup.Paused;
        }

        public bool IsStopped(InMusicGroup musicGroup)
        {
            return musicGroup.Stopped;
        }


        public void StopAll()
        {
            TreeWalker.ForEach(InAudioInstanceFinder.DataManager.MusicTree, node =>
            {
                var music = node as InMusicGroup;
                if (music != null)
                {
                    if (music.Playing || music.Paused)
                    {
                        Stop(music);
                    }
                }
            });
        }

        public void StopAll(float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            TreeWalker.ForEach(InAudioInstanceFinder.DataManager.MusicTree, node =>
            {
                var music = node as InMusicGroup;
                if (music != null)
                {
                    if (music.Playing || music.Paused)
                    {

                        FadeAndStop(music, 0f, duration, tweenType);
                    }   
                }
            });
        }

        public void PauseAll()
        {
            TreeWalker.ForEach(InAudioInstanceFinder.DataManager.MusicTree, node =>
            {
                var music = node as InMusicGroup;
                if (music != null)
                {
                    if (music.Playing)
                    {
                        Pause(music);
                    }
                }
            });
        }

        #endregion

        #region Fading

        public void StopFade(InMusicGroup stopFade)
        {
            stopFade.PlayingInfo.Fading = false;
        }

        public void FadeVolume(InMusicNode toFade, float targetVolume, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            //There are no guarantees that the music is playing, 
            Fade(toFade, targetVolume, duration, tweenType);
        }

        public void FadeAndStop(InMusicNode toFade, float targetVolume, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            Fade(toFade, targetVolume, duration, tweenType);
            toFade.PlayingInfo.DoAtEnd = MusicState.Stopped;
        }

        public void FadeAndStop(InMusicNode toFade, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            Fade(toFade, 0f, duration, tweenType);
            toFade.PlayingInfo.DoAtEnd = MusicState.Stopped;
        }

        public void FadeAndPause(InMusicNode toFade, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            Fade(toFade, 0f, duration, tweenType);
            toFade.PlayingInfo.DoAtEnd = MusicState.Paused;
        }

        public void FadeAndPause(InMusicNode toFade, float targetVolume, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            Fade(toFade, targetVolume, duration, tweenType);
            toFade.PlayingInfo.DoAtEnd = MusicState.Paused;
        }

        public void FadeToInitialVolume(InMusicNode toFade, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            Fade(toFade, toFade._minVolume, duration, tweenType);
        }

        public void CrossfadeVolume(InMusicNode from, InMusicGroup to, float targetVolume, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            Fade(from, 0f, duration, tweenType);
            Fade(to, targetVolume, duration, tweenType);
        }

        public void SwapCrossfadeVolume(InMusicNode from, InMusicGroup to, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            Fade(from, to.Volume, duration, tweenType);
            Fade(to, from.Volume, duration, tweenType);
        }

        public void CrossfadeVolume(InMusicNode from, InMusicGroup to, float duration, LeanTweenType tweenType = LeanTweenType.easeInOutQuad)
        {
            Fade(from, 0f, duration, tweenType);
            Fade(to, 1f, duration, tweenType);
        }


        #endregion

        #region Solo & Mute
        //These settings are applied in the beginning of the next frame in the update loop in InAudio.cs

        public void Solo(InMusicGroup musicGroup, bool solo)
        {
            musicGroup.runtimeSolo = true;
        }

        public bool IsSolo(InMusicGroup musicGroup)
        {
            return musicGroup.runtimeSolo;
        }

        public void Mute(InMusicGroup musicGroup, bool mute)
        {
            musicGroup.runtimeMute = mute;
        }

        public bool IsMuted(InMusicGroup musicGroup)
        {
            return musicGroup.runtimeMute;
        }

        #endregion

        #region Memory

        public bool IsLoaded(InMusicGroup musicGroup)
        {
            return BankLoader.IsLoaded(musicGroup.GetBank());
        }

        public bool Load(InMusicGroup musicGroup)
        {
            return BankLoader.Load(musicGroup.GetBank());
        }

        public void UnLoad(InMusicGroup musicGroup)
        {
            BankLoader.Unload(musicGroup.GetBank());
        }
        
        #endregion

        #region Non-public

        private static InMusicGroup GetParent(InMusicGroup group)
        {

            if (group._parent.IsRootOrFolder)
                return group;
            else 
                return GetParent(group._parent as InMusicGroup);
        }

        private static void Fade(InMusicNode toFade, float targetVolume, float duration, LeanTweenType tweenType)
        {
            if (tweenType == LeanTweenType.animationCurve)
            {
                Debug.LogError("InAudio: AnimationCurve is not supported in fading. Used on Music Group \"" + toFade.GetName + "\"");
                return;
            }
            var runInfo = toFade.PlayingInfo;

            runInfo.Fading = true;
            float currentTime = Time.time;
            runInfo.StartTime = currentTime;
            runInfo.EndTime = currentTime + duration;
            runInfo.TweenType = tweenType;
            runInfo.TargetVolume = targetVolume;
            runInfo.StartVolume = toFade.runtimeVolume;
        }

        private static bool HandleExcistingPlay(InMusicGroup musicGroup, ref PlayingMusicInfo playingInfo)
        {
            if (playingInfo.State == MusicState.Playing)
            {
                StopPlayersNow(musicGroup);
            }
            else if (playingInfo.State == MusicState.Paused)
            {
                UnpausePlayers(musicGroup);
                return true;
            }

            return false;
        }

        private static void PlayMusicGroup(InMusicGroup toPlay, double playTime, int skipSamples)
        {
            var musicPool = InAudioInstanceFinder.InMusicPlayerPool;
            var mixer = toPlay.GetUsedMixerGroup();

            var editorClips = toPlay._clips;
            int clipCount = editorClips.Count;
            for (int j = 0; j < clipCount; j++)
            {
                var playingInfo = toPlay.PlayingInfo;
                playingInfo.State = MusicState.Playing;
				playingInfo.DoAtEnd = MusicState.Nothing;
                var player = musicPool.GetObject();
                toPlay.PlayingInfo.Players.Add(player);
                player.clip = editorClips[j];
                player.loop = toPlay._loop;
                player.timeSamples = skipSamples;
                player.outputAudioMixerGroup = mixer;
                player.PlayScheduled(playTime);
            }


            for (int i = 0; i < toPlay._children.Count; i++)
            {
                PlayMusicGroup(toPlay._children[i] as InMusicGroup, playTime, skipSamples);
            }
        }

        private static void StopPlayersNow(InMusicGroup musicGroup)
        {
            var playingInfo = musicGroup.PlayingInfo;
            if (playingInfo.State != MusicState.Stopped)
            {
                playingInfo.State = MusicState.Stopped;
                var playing = playingInfo.Players;
                for (int i = 0; i < playing.Count; i++)
                {
                    playing[i].Stop();
                    MusicUpdater.CleanupMusicNode(musicGroup);
                    
                }
            }
            for (int i = 0; i < musicGroup._children.Count; i++)
            {
                StopPlayersNow(musicGroup._children[i] as InMusicGroup);
            }
        }

        private static void StopPlayersAt(InMusicGroup musicGroup, double absoluteDSPTime)
        {
            var playingInfo = musicGroup.PlayingInfo;
            if (playingInfo.State != MusicState.Stopped)
            {
                var playing = playingInfo.Players;
                for (int i = 0; i < playing.Count; i++)
                {
                    playing[i].SetScheduledEndTime(absoluteDSPTime);
                    //cleanup happens in MusicUpdater
                }
            }
            for (int i = 0; i < musicGroup._children.Count; i++)
            {
                StopPlayersAt(musicGroup._children[i] as InMusicGroup, absoluteDSPTime);
            }
        }

       

        private static void UnpausePlayers(InMusicGroup musicGroup)
        {
            var playingInfo = musicGroup.PlayingInfo;
            if (playingInfo.State == MusicState.Paused)
            {
                var playing = playingInfo.Players;
                for (int i = 0; i < playing.Count; i++)
                {
                    playing[i].UnPause();
                    playingInfo.State = MusicState.Playing;
                }
            }
            for (int i = 0; i < musicGroup._children.Count; i++)
            {
                UnpausePlayers(musicGroup._children[i] as InMusicGroup);
            }
        }

        private static void PausePlayers(InMusicGroup musicGroup)
        {
            var playingInfo = musicGroup.PlayingInfo;
            if (playingInfo.State == MusicState.Playing)
            {
                var playing = playingInfo.Players;
                for (int i = 0; i < playing.Count; i++)
                {
                    playing[i].Pause();
                    playingInfo.State = MusicState.Paused;
                }
            }
            for (int i = 0; i < musicGroup._children.Count; i++)
            {
                PausePlayers(musicGroup._children[i] as InMusicGroup);
            }
        }

        //public static AudioSource Max(InMusicGroup node, out AudioSource source, out double currentMax)
        //{
        //    var max = double.MinValue;
        //    AudioSource refSource = null;
        //    var v = FindMax(node, ref refSource, ref max);
        //    currentMax = max;
        //    return v;
        //}

        //private static AudioSource FindMax(InMusicGroup node, ref AudioSource audioSource, ref double currentMax)
        //{
        //    var end = default(T);

        //    U currentVal = predicate(node);

        //    if (currentVal.CompareTo(currentMax) > 0)
        //    {
        //        end = node;
        //        currentMax = currentVal;
        //    }

        //    for (int i = 0; i < node.GetChildren.Count; i++)
        //    {
        //        U max = default(U);
        //        var result = FindMax(node.GetChildren[i], predicate, ref max);
        //        if (max.CompareTo(currentMax) > 0)
        //        {
        //            currentMax = max;
        //            end = result;
        //        }
        //    }
        //    return end;
        //}
        
        

        #endregion
    }


}