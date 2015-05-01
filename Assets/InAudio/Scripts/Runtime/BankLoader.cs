using System;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEngine;

namespace InAudioSystem
{
public static class BankLoader
{

    public static bool Load(InAudioBankLink bankLink)
    {
        if (bankLink == null)
            return false;

        return LoadBank(bankLink);
    }

    public static bool IsLoaded(InAudioBankLink bankLink)
    {
        if (bankLink == null)
            return false;

        return bankLink.IsLoaded;
    }

    private static bool LoadBank(InAudioBankLink bankLink)
    {
        if (bankLink == null)
            return false;
        var bankData = bankLink._bankData;
        for (int i = 0; i < bankData.Count; i++)
        {
            var audioNode = bankData[i].AudioNode;
            var musicNode = bankData[i].MusicNode;

            if (audioNode != null)
            {
                var data = audioNode._nodeData as InAudioData;
                if(data != null && data._clip != null)
                    data._clip.LoadIfPossible();
            } else if (musicNode != null)
            {
                var clips = musicNode._clips;
                for (int j = 0; j < clips.Count; j++)
                {
                    if(clips[j] != null)
                        clips[j].LoadIfPossible();
                }
            }

        }
        bankLink.IsLoaded = true;
        return true;
    }


    public static void Unload(InAudioBankLink bank)
    {
        var bankData = bank._bankData;
        for (int i = 0; i < bankData.Count; i++)
        {
            var data = bankData[i];
            var node = data.AudioNode;
            if (node != null)
            {
                var audioData = node._nodeData as InAudioData;
                if (audioData != null && audioData._clip != null)
                {
                    audioData._clip.UnloadIfPossible();
                }
            }
            else if (data.MusicNode != null)
            {
                var clips = data.MusicNode._clips;
                for (int j = 0; j < clips.Count; j++)
                {
                    var clip = clips[j];
                    if (clip != null)
                        clip.UnloadIfPossible();
                }
            }
        }
        Resources.UnloadUnusedAssets();
        bank.IsLoaded = false;
    }

    public static void LoadAutoLoadedBanks()
    {
        LoadAuto(InAudioInstanceFinder.DataManager.BankLinkTree);
    }

    private static void LoadAuto(InAudioBankLink bankLink)
    {

        if (bankLink == null)
        {
            return;
        }
        if (bankLink._autoLoad)
            Load(bankLink);

        for (int i = 0; i < bankLink._children.Count; ++i)
        {
            LoadAuto(bankLink._children[i]);
        }
    }
}
}