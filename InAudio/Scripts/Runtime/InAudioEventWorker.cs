using System;
using System.Collections.Generic;
using InAudioLeanTween;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.InAudioSystem;
using InAudioSystem.Runtime;
using UnityEngine;

namespace InAudioSystem.Internal
{
    public class InAudioEventWorker : MonoBehaviour
    {
        public InPlayer PlayConnectedTo(GameObject controllingObject, InAudioNode audioNode, GameObject attachedTo, AudioParameters audioParameters, float fade = 0f, LeanTweenType fadeType = LeanTweenType.notUsed)
        {
            if (audioNode.IsRootOrFolder)
            {
                Debug.LogWarning("InAudio: Cannot play \""+audioNode.GetName+"\" as it is a folder");
                return null;
            }

                List<InstanceInfo> currentInstances = audioNode.CurrentInstances;
            if (!AllowedStealing(audioNode, currentInstances))
            {
                return null;
            }

            var runtimePlayer = InAudioInstanceFinder.RuntimePlayerControllerPool.GetObject();
            if (runtimePlayer == null)
            {
                Debug.LogWarning("InAudio: A pooled objected was not initialized. Try to restart play mode. If the problem persists, please submit a bug report.");
            }
            currentInstances.Add(new InstanceInfo(AudioSettings.dspTime, runtimePlayer));
            runtimePlayer.transform.parent = attachedTo.transform;
            runtimePlayer.transform.localPosition = new Vector3();
            Play(controllingObject, audioNode, runtimePlayer, fade, fadeType, audioParameters);
            return runtimePlayer;
        }

        public InPlayer PlayFollowing(GameObject controllingObject, InAudioNode audioNode, AudioParameters audioParameters, float fade = 0f, LeanTweenType fadeType = LeanTweenType.notUsed)
        {
            if (audioNode.IsRootOrFolder)
            {
                Debug.LogWarning("InAudio: Cannot play \"" + audioNode.GetName + "\" as it is a folder");
                return null;
            }

            List<InstanceInfo> currentInstances = audioNode.CurrentInstances;
            if (!AllowedStealing(audioNode, currentInstances))
            {
                return null;
            }

            var runtimePlayer = InAudioInstanceFinder.RuntimePlayerControllerPool.GetObject();
            if (runtimePlayer == null)
            {
                Debug.LogWarning("InAudio: A pooled objected was not initialized. Try to restart play mode. If the problem persists, please submit a bug report.");
            }
            currentInstances.Add(new InstanceInfo(AudioSettings.dspTime, runtimePlayer));
            PlayFollowing(controllingObject, audioNode, runtimePlayer, fade, fadeType, audioParameters);
            return runtimePlayer;
        }

        public InPlayer PlayAtPosition(GameObject controllingObject, InAudioNode audioNode, Vector3 position, AudioParameters audioParameters, float fade = 0f, LeanTweenType fadeType = LeanTweenType.notUsed)
        {
            if (audioNode.IsRootOrFolder)
            {
                Debug.LogWarning("InAudio: Cannot play \"" + audioNode.GetName + "\" as it is a folder");
                return null;
            }

            List<InstanceInfo> currentInstances = audioNode.CurrentInstances;
            if (!AllowedStealing(audioNode, currentInstances))
                return null;
            var runtimePlayer = InAudioInstanceFinder.RuntimePlayerControllerPool.GetObject();
            runtimePlayer.transform.position = position;
            currentInstances.Add(new InstanceInfo(AudioSettings.dspTime, runtimePlayer));
            Play(controllingObject, audioNode, runtimePlayer, fade, fadeType, audioParameters);
            return runtimePlayer;
        }

        public void StopAll(InAudioNode node, float fadeOutTime, LeanTweenType type)
        {
            if (node.IsRootOrFolder)
            {
                Debug.LogWarning("InAudio: Cannot stop audio on \"" + node.GetName + "\" as it is a folder");
            }

            foreach (var audioNode in GOAudioNodes)
            {
                var infoList = audioNode.Value;
                if (infoList != null)
                {
                    int count = infoList.InfoList.Count;
                    for (int i = 0; i < count; i++)
                    {
                        if (infoList.InfoList[i].Node == node)
                        {
                            infoList.InfoList[i].Player.Stop();
                        }

                    }
                }

            }

        }

        public void StopAll(float fadeOutTime, LeanTweenType type)
        {
            foreach (var audioNode in GOAudioNodes)
            {
                var infoList = audioNode.Value;

                if (infoList != null)
                {
                    var list = infoList.InfoList;
                    for (int i = 0; i < list.Count; ++i)
                    {
                        if (fadeOutTime > 0)
                            list[i].Player.Stop(fadeOutTime, type);
                        else
                            list[i].Player.Stop();
                    }
                }
            }
            
        }

        public void StopAll(GameObject controllingObject, float fadeOutTime, LeanTweenType type)
        {
            ObjectAudioList infoList;
            GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out infoList);

            if (infoList != null)
            {
                var list = infoList.InfoList;
                for (int i = 0; i < list.Count; ++i)
                {
                    if (fadeOutTime > 0)
                        list[i].Player.Stop(fadeOutTime, type);
                    else
                        list[i].Player.Stop();
                }
            }
        }

        public void SetVolumeForNode(GameObject controllingObject, InAudioNode node, float newVolume)
        {
            ObjectAudioList outInfoList;
            GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out outInfoList);
            if (outInfoList != null)
            {
                List<RuntimeInfo> infoList = outInfoList.InfoList;
                int count = infoList.Count;
                for (int i = 0; i < count; ++i)
                {
                    var player = infoList[i].Player;
                    if (player.NodePlaying == node)
                    {
                        player.Volume = newVolume;
                    }
                }
            }
        }

        public void SetVolumeForGameObject(GameObject controllingObject, float newVolume)
        {
            ObjectAudioList outInfoList;
            GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out outInfoList);
            if (outInfoList != null)
            {
                List<RuntimeInfo> infoList = outInfoList.InfoList;
                int count = infoList.Count;
                for (int i = 0; i < count; ++i)
                {
                    var player = infoList[i].Player;
                    player.Volume = newVolume;
                }
            }
        }

        public InPlayer[] GetPlayers(GameObject go)
        {
            ObjectAudioList infoList;
            GOAudioNodes.TryGetValue(go.GetInstanceID(), out infoList);

            if (infoList != null)
            {
                InPlayer[] players = new InPlayer[infoList.InfoList.Count];
                var list = infoList.InfoList;
                for (int i = 0; i < list.Count; i++)
                {
                    players[i] = infoList.InfoList[i].Player;
                }
                return players;
            }
            return null;
        }

        public void GetPlayers(GameObject go, IList<InPlayer> copyTo)
        {
            ObjectAudioList infoList;
            GOAudioNodes.TryGetValue(go.GetInstanceID(), out infoList);

            if (infoList != null)
            {
                var list = infoList.InfoList;
                for (int i = 0; i < list.Count && i < copyTo.Count; i++)
                {
                    copyTo[i] = infoList.InfoList[i].Player;
                }
            }
        }

        public void Break(GameObject controllingObject, InAudioNode toBreak)
        {
            ObjectAudioList infoList;
            GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out infoList);
            if (infoList != null)
            {
                var list = infoList.InfoList;
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].Node == toBreak)
                    {
                        list[i].Player.Break();
                    }
                }
            }
        }

        public void StopByNode(GameObject controllingObject, InAudioNode nodeToStop, float fadeOutTime = 0.0f, LeanTweenType tweenType = LeanTweenType.easeOutCubic)
        {
            ObjectAudioList infoList;
            GOAudioNodes.TryGetValue(controllingObject.GetInstanceID(), out infoList);

            if (infoList != null)
            {
                var list = infoList.InfoList;
                for (int i = 0; i < list.Count; ++i)
                {
                    if (list[i].Node == nodeToStop)
                    {
                        list[i].Player.Stop(fadeOutTime, tweenType);
                    }
                }
            }
        }

        private void Play(GameObject controllingObject, InAudioNode audioNode, InPlayer player, float fade, LeanTweenType fadeType, AudioParameters parameters)
        {
            var runtimeInfo = PreparePlay(controllingObject, audioNode, player);

            player._internalPlay(audioNode, controllingObject, runtimeInfo, fade, fadeType, parameters);
        }

        
        private void PlayFollowing(GameObject controllingObject,InAudioNode audioNode, InPlayer player, float fade, LeanTweenType fadeType, AudioParameters parameters)
        {
            var runtimeInfo = PreparePlay(controllingObject, audioNode, player);

            player._internalPlayFollowing(audioNode, controllingObject, runtimeInfo, fade, fadeType, parameters);
        }

        private RuntimeInfo PreparePlay(GameObject controllingObject, InAudioNode audioNode, InPlayer player)
        {
            ObjectAudioList tupleList = GetValue(GOAudioNodes, controllingObject);

            RuntimeInfo runtimeInfo = new RuntimeInfo();
            tupleList.InfoList.Add(runtimeInfo);
            runtimeInfo.Node = audioNode;
            runtimeInfo.Player = player;
            runtimeInfo.PlacedIn = tupleList;
            return runtimeInfo;
        }


        private ObjectAudioList GetValue(Dictionary<int, ObjectAudioList> dictionary, GameObject go)
        {
            ObjectAudioList infoList;
            int instanceID = go.GetInstanceID();
            if (!dictionary.TryGetValue(instanceID, out infoList))
            {
                infoList = AudioListPool.GetObject();
                dictionary.Add(instanceID, infoList);
            }
            return infoList;
        }

        private InRuntimeInfoPool runtimeInfoPool;
        private ObjectPool<ObjectAudioList> AudioListPool = new ObjectPool<ObjectAudioList>(10);

        //Dictionary<int, List<RuntimeInfo>> 
        private Dictionary<int, ObjectAudioList> GOAudioNodes = new Dictionary<int, ObjectAudioList>();

        private static bool AllowedStealing(InAudioNode audioNode, List<InstanceInfo> currentInstances)
        {
            var data = (InAudioNodeData) audioNode._nodeData;
            if (data.LimitInstances && currentInstances.Count >= data.MaxInstances)
            {
                InPlayer player = null;
                var stealType = data.InstanceStealingTypes;
                if (stealType == InstanceStealingTypes.NoStealing)
                    return false;

                int index = 0;
                InstanceInfo foundInfo;
                if (stealType == InstanceStealingTypes.Newest)
                {
                    double newestTime = 0;

                    for (int i = 0; i < currentInstances.Count; i++)
                    {
                        InstanceInfo instanceInfo = currentInstances[i];
                        if (instanceInfo.Timestamp > newestTime)
                        {
                            newestTime = instanceInfo.Timestamp;
                            index = i;
                        }
                    }
                }
                else if (stealType == InstanceStealingTypes.Oldest)
                {
                    double oldestTime = Double.MaxValue;
                    for (int i = 0; i < currentInstances.Count; i++)
                    {
                        InstanceInfo instanceInfo = currentInstances[i];
                        if (instanceInfo.Timestamp < oldestTime)
                        {
                            oldestTime = instanceInfo.Timestamp;
                            index = i;
                        }
                    }
                }

                foundInfo = currentInstances[index];
                player = foundInfo.Player;
                currentInstances.SwapRemoveAt(ref index);
                if (player != null)
                    player.Stop();
            }
            return true;
        }

        public float GetMinVolume(GameObject go, InAudioNode node)
        {

            ObjectAudioList infoList;
            int instanceID = go.GetInstanceID();
            float minVolume = -1; //volume should be (0-1)

            if (GOAudioNodes.TryGetValue(instanceID, out infoList))
            {
                var list = infoList.InfoList;
                int count = list.Count;
                minVolume = 10000f; //volume should be (0-1)
                for (int i = 0; i < count; i++)
                {
                    var info = list[i];
                    float volume = info.Player.Volume;
                    if (volume < minVolume)
                    {
                        minVolume = volume;
                    }
                }
            }
            else
            {
                Debug.LogWarning("InAudio: Node not found");
            }
            return minVolume;
        }

        private void InitializeController(InPlayer controller)
        {
            controller.internalInitialize(FreeController);
        }

        private void FreeController(ObjectAudioList audioList)
        {
            AudioListPool.ReleaseObject(audioList);
        }

        private void Awake()
        {
            if (runtimeInfoPool == null)
            {
                runtimeInfoPool = GetComponent<InRuntimeInfoPool>();
                InAudioInstanceFinder.RuntimePlayerControllerPool.Initialization = InitializeController;
            }
        }
    }
}