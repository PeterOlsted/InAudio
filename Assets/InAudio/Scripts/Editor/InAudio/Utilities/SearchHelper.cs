using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace InAudioSystem.InAudioEditor
{

public static class SearchHelper
{

    public static void SearchForActionTarget(AudioEventAction action)
    {
        var audioAction = action as InEventAudioAction;
        if (audioAction != null && audioAction.Node != null)
        {
            InAudioWindow.Launch().Find(audioAction.Node);
        }
        var bankAction = action as InEventBankLoadingAction;
        if (bankAction != null && bankAction.BankLink != null)
        {
            AuxWindow.Launch().FindBank(bankAction.BankLink);
        }
        var musicControl = action as InEventMusicControl;
        if (musicControl != null && musicControl.Target != null)
        {
            SearchFor(musicControl.MusicGroup);
        }
        var musicFade = action as InEventMusicFade;
        if (musicFade != null && musicFade.Target != null)
        {
            SearchFor(musicFade.To);
        }
        var mixerValue = action as InEventMixerValueAction;
        if (mixerValue != null && mixerValue.Target != null)
        {
            SearchFor(mixerValue.Mixer);
        }

        var soloMuteValue = action as InEventSoloMuteMusic;
        if (soloMuteValue != null && soloMuteValue.Target != null)
        {
            SearchFor(soloMuteValue.MusicGroup);
        }
    }

    public static void SearchFor(AudioMixerGroup bus)
    {
        //TODO Update for audio mixer
        EditorApplication.ExecuteMenuItem("Window/Audio Mixer");
    }

    public static void SearchFor(AudioMixerSnapshot bus)
    {
        //TODO Update for audio mixer
        EditorApplication.ExecuteMenuItem("Window/Audio Mixer");
    }

    public static void SearchFor(AudioMixer bus)
    {
        //TODO Update for audio mixer
        EditorApplication.ExecuteMenuItem("Window/Audio Mixer");
    }

    public static void SearchFor(InAudioBankLink bank)
    {
        AuxWindow.Launch().FindBank(bank);

    }

    public static void SearchFor(InAudioNode node)
    {
        InAudioWindow.Launch().Find(node);
    }

    public static void SearchFor(InMusicNode node)
    {
        InMusicWindow.Launch().Find(node);
    }

    public static void SearchForObject<T>(T node) where T : Object, InITreeNode<T>
    {
        if (node is InAudioNode)
            SearchFor(node as InAudioNode);
        if (node is InAudioBankLink)
            SearchFor(node as InAudioBankLink);
    }
}
}