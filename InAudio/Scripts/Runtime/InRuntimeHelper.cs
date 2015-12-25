using System;
using System.Collections.Generic;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using InAudioSystem.Runtime;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InAudioSystem 
{
public static class RuntimeHelper
{
    public static int SelectRandom(RandomData randomNode)
    {       
        var weights = randomNode.weights;
        int childCount = weights.Count;

        if (childCount == 0)
            return -1;

        int sum = 0;
        
        if(randomNode.lastPlayed == null)
            randomNode.lastPlayed = new Queue<int>();

        int doNotRepeat = Math.Min(childCount - 1, randomNode.doNotRepeat); //To make sure it still works if the DoNotRepeat number is set higher than the node cound

        var lastPlayed = randomNode.lastPlayed;

        for (int i = 0; i < childCount; ++i)
        {
            if(!lastPlayed.Contains(i))
                sum += weights[i];
        }
        
        int randomArea = Random.Range(0, sum + 1); //+1 because range is non-inclusive
        int picked = -1;
        int currentMax = 0;
        for (int i = 0; i < childCount; ++i)
        {
            if (!lastPlayed.Contains(i) || doNotRepeat == 0)
            {
                currentMax += weights[i];
                if (weights[i] != 0 && randomArea <= currentMax)
                {
                    picked = i;
                    break;
                }
            }
        }
        if (picked != -1 && doNotRepeat > 0) //If we picked one
        {
            while (lastPlayed.Count >= doNotRepeat && doNotRepeat > 0)
            {
                lastPlayed.Dequeue();
            }

            lastPlayed.Enqueue(picked);
        }

        return picked;
    }

    public static AudioRolloffMode ApplyRolloffData(InAudioNode current, InAudioNodeData data, AudioSource workOn)
    {
        workOn.rolloffMode = data.RolloffMode;
        workOn.maxDistance = data.MaxDistance;
        workOn.minDistance = data.MinDistance;

        if (data.RolloffMode == AudioRolloffMode.Custom)
        {
            workOn.maxDistance = float.MaxValue;//Set to max so we can use our own animation curve
        }
        return data.RolloffMode;
    }


    public static AudioRolloffMode CalcAttentutation(InAudioNode root, InAudioNode current, AudioSource workOn)
    {
        var data = (InAudioNodeData)current._nodeData;
        if (current == root || data.OverrideAttenuation)
        {
            return ApplyRolloffData(current, data, workOn);
        }
        else
        {
            return CalcAttentutation(root, current._parent, workOn);
        }
    }

    public static void ReleaseRuntimeInfo(RuntimeInfo info)
    {
        //Swap remove and set the new position
        if (info != null)
        {
            info.PlacedIn.InfoList.FindSwapRemove(info);
            if (InAudioInstanceFinder.RuntimeInfoPool != null)
                InAudioInstanceFinder.RuntimeInfoPool.ReleaseObject(info);
        }
    }

    public static float InitialDelay(InAudioNodeData data)
    {
        if (!data.RandomizeDelay)
            return data.InitialDelayMin;

        return Random.Range(data.InitialDelayMin, data.InitialDelayMax);
    }

    public static float Offset(InAudioNodeData data)
    {
        if (!data.RandomSecondsOffset)
            return data.MinSecondsOffset;

        return Random.Range(data.MinSecondsOffset, data.MaxSecondsOffset);
    }

    public static float CalcBlend(InAudioNode root, InAudioNode current)
    {
        var data = (InAudioNodeData)current._nodeData;
        if (current == root)
        {
            if (!data.RandomBlend)
                return data.MinBlend;
            else
                return Random.Range(data.MinBlend, data.MaxBlend);
        }

        if (!data.RandomBlend)
            return data.MinBlend * CalcBlend(root, current._parent);
        else
            return Random.Range(data.MinBlend, data.MaxBlend) * CalcBlend(root, current._parent);

    }


    public static float CalcVolume(InAudioNode root, InAudioNode current)
    {
        var data = (InAudioNodeData) current._nodeData;
        if (current == root)
        {
            if (!data.RandomVolume)
                return data.MinVolume;
            else
                return Random.Range(data.MinVolume, data.MaxVolume);
        }

        if (!data.RandomVolume)
            return data.MinVolume * CalcVolume(root, current._parent);
        else
            return Random.Range(data.MinVolume, data.MaxVolume) * CalcVolume(root, current._parent);

    }

    public static byte GetLoops(InAudioNode node)
    {
        byte loops;
        var data = (InAudioNodeData)node._nodeData;
        if (data.RandomizeLoops)
            loops = (byte)Mathf.Min(Random.Range(data.MinIterations, data.MaxIterations + 1), 255);
        else
            loops = data.MinIterations;
        return loops;
    }

    public static float CalcPitch(InAudioNode root, InAudioNode current)
    {
        InAudioNodeData inAudioNodeData = (InAudioNodeData)current._nodeData;
        float minPitch = inAudioNodeData.MinPitch;
        float maxPitch = inAudioNodeData.MaxPitch;
        bool isRandom = inAudioNodeData.RandomPitch;
        if (current == root)
        {
            if(!isRandom)
                return minPitch;
            else
            {
                return Random.Range(minPitch, maxPitch);
            }
        }

        if (!isRandom)
            return inAudioNodeData.MinPitch + CalcPitch(root, current._parent) - 1;
        else
        {
            return Random.Range(minPitch, maxPitch) + CalcPitch(root, current._parent) - 1;
        }

        
    }

    public static float LengthFromPitch(float length, float pitch)
    {
        return length / pitch;
    }
}
}
