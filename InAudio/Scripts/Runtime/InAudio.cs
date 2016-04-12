using System;
using System.Collections;
using System.Collections.Generic;
using InAudioLeanTween;
using InAudioSystem;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using InAudioSystem.Runtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using Object = UnityEngine.Object;

/// <summary>
/// The InAudio class it the primary way to interact with InAudio via code. 
/// It is used to play all audio & events, while the music player can be accessed via InAudio.Music.
/// </summary>
#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class InAudio : MonoBehaviour
{
    /******************/
    /*Public interface*/
    /******************/
    #region Public fields
    /// <summary>
    /// The active listener in the scene. Some features will not work if not set, like the spline system.
    /// </summary>
    public AudioListener activeAudioListener;

    public static AudioListener ActiveListener
    {
        get
        {
            if (instance != null)
                return instance.activeAudioListener;
            return null;
        }
        set
        {
            if (instance != null)
                instance.activeAudioListener = ActiveListener;
        }
    }

    public static bool DoesExist
    {
        get { return instance != null; }
    }

    #endregion

    #region Music

    private static MusicPlayer music;

    public static MusicPlayer Music
    {
        get
        {
            if (music == null)
            {
#if UNITY_5_2
                Debug.LogError("InAudio: Could not find music player. Please ensure that InAudio is loaded before accessing it.");
#else
                Debug.LogError("InAudio: Could not find music player. Please ensure that InAudio is loaded before accessing it.\nIs the scene with InAudio loaded after the script trying to access it?");
#endif
            }
            return music;
        }
        set { music = value; }
    }
#endregion

#region Audio player
#region Play

    /// <summary>
    /// Play an audio node directly
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer Play(GameObject gameObject, InAudioNode audioNode, AudioParameters parameters = null)
    {
        if (instance != null && gameObject != null && audioNode != null)
            return instance._inAudioEventWorker.PlayConnectedTo(gameObject, audioNode, gameObject, parameters);
        else
            InDebug.MissingArguments("Play", gameObject, audioNode);
        return null;
    }


    /// <summary>
    /// Play an audio node directly, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="attachedTo">The object to be attached to</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    [Obsolete("Use PlayFollowing() instead.")]
    public static InPlayer PlayAttachedTo(GameObject gameObject, InAudioNode audioNode, GameObject attachedTo, AudioParameters parameters = null)
    {
        if (instance != null && gameObject != null && audioNode != null)
            return instance._inAudioEventWorker.PlayConnectedTo(gameObject, audioNode, attachedTo, parameters);
        else
            InDebug.MissingArguments("PlayAttachedTo", gameObject, audioNode);
        return null;
    }

    /// <summary>
    /// Play an audio node directly with a custom fade, following a game object and persists even if the GO is destroyed.
    /// </summary>
    /// <param name="gameObject">The game object to be controlled by and follow</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayFollowing(GameObject gameObject, InAudioNode audioNode, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder || gameObject == null)
        {
            InDebug.MissingArguments("PlayFollowing", gameObject, audioNode);
            return null;
        }

        InPlayer player = instance._inAudioEventWorker.PlayFollowing(gameObject, audioNode, parameters);

        return player;
    }


    /// <summary>
    /// Play an audio node directly, at this position in world space
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="position">The world position to play at</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAtPosition(GameObject gameObject, InAudioNode audioNode, Vector3 position, AudioParameters parameters = null)
    {
        if (instance != null && gameObject != null && audioNode != null)
            return instance._inAudioEventWorker.PlayAtPosition(gameObject, audioNode, position, parameters);
        else
            InDebug.MissingArguments("PlayAtPosition", gameObject, audioNode);
        return null;
    }


    /// <summary>
    /// Play an audio node directly with fade in time
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer Play(GameObject gameObject, InAudioNode audioNode, float fadeTime, LeanTweenType tweeenType, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
        {
            InDebug.MissingArguments("Play (tween)", gameObject, audioNode);
            return null;
        }

        InPlayer player = instance._inAudioEventWorker.PlayConnectedTo(gameObject, audioNode, gameObject, parameters);
        player.Volume = 0.0f;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, 0, 1.0f, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node directly with fade in time, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="attachedTo">The game object to attach to</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    [Obsolete("Use PlayFollowing() instead.")]
    public static InPlayer PlayAttachedTo(GameObject gameObject, InAudioNode audioNode, GameObject attachedTo, float fadeTime, LeanTweenType tweeenType, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
        {
            InDebug.MissingArguments("PlayAttachedTo (tween)", gameObject, audioNode);
        }

        InPlayer player = instance._inAudioEventWorker.PlayConnectedTo(gameObject, audioNode, attachedTo, parameters);
        player.Volume = 0.0f;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, 0, 1.0f, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }


    /// <summary>
    /// Play an audio node directly with a fade, following a game object and persists even if the GO is destroyed.
    /// </summary>
    /// <param name="gameObject">The game object to be controlled by and follow</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayFollowing(GameObject gameObject, InAudioNode audioNode, float fadeTime, LeanTweenType tweeenType, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder || gameObject == null)
        {
            InDebug.MissingArguments("PlayFollowing (tween)", gameObject, audioNode);
            return null;
        }

        InPlayer player = instance._inAudioEventWorker.PlayFollowing(gameObject, audioNode, parameters);
        player.Volume = 0.0f;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, 0.0f, 1f, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node directly with fade in time, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="position">The position in world space to play the audio</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAtPosition(GameObject gameObject, InAudioNode audioNode, Vector3 position, float fadeTime, LeanTweenType tweeenType, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
        {
            InDebug.MissingArguments("PlayAtPosition (tween)", gameObject, audioNode);
            return null;
        }

        InPlayer player = instance._inAudioEventWorker.PlayAtPosition(gameObject, audioNode, position, parameters);
        player.Volume = 0.0f;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, 0, 1.0f, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }


    /// <summary>
    /// Play an audio node directly with custom fading
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="startVolume">The starting volume</param>
    /// <param name="endVolume">The end volume</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer Play(GameObject gameObject, InAudioNode audioNode, float fadeTime, LeanTweenType tweeenType, float startVolume, float endVolume, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
        {
            InDebug.MissingArguments("Play (tween specific)", gameObject, audioNode);
            return null;
        }

        InPlayer player = instance._inAudioEventWorker.PlayConnectedTo(gameObject, audioNode, gameObject, parameters);
        player.Volume = startVolume;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, startVolume, endVolume, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node directly with a custom fade, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="attachedTo">The game object to attach to</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="startVolume">The starting volume</param>
    /// <param name="endVolume">The end volume</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    [Obsolete("Use PlayFollowing() instead.")]
    public static InPlayer PlayAttachedTo(GameObject gameObject, InAudioNode audioNode, GameObject attachedTo, float fadeTime, LeanTweenType tweeenType, float startVolume, float endVolume, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
        {
            InDebug.MissingArguments("PlayAttachedTo (tween specific)", gameObject, audioNode);
            return null;
        }

        InPlayer player = instance._inAudioEventWorker.PlayConnectedTo(gameObject, audioNode, attachedTo, parameters);
        player.Volume = startVolume;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, startVolume, endVolume, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node directly with a custom fade, following a game object and persists even if the GO is destroyed.
    /// </summary>
    /// <param name="gameObject">The game object to be controlled by and follow</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="startVolume">The starting volume</param>
    /// <param name="endVolume">The end volume</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayFollowing(GameObject gameObject, InAudioNode audioNode, float fadeTime, LeanTweenType tweeenType, float startVolume, float endVolume, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder || gameObject == null)
        {
            InDebug.MissingArguments("PlayFollowing (tween specific)", gameObject, audioNode);
            return null;
        }

        InPlayer player = instance._inAudioEventWorker.PlayFollowing(gameObject, audioNode, parameters);
        player.Volume = startVolume;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, startVolume, endVolume, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node in world space with a custom fade, attached to another game object
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <param name="position">The world position to play at</param>
    /// <param name="fadeTime">How long it should take to fade in from 0 to 1 in volume</param>
    /// <param name="tweeenType">The curve of fading</param>
    /// <param name="startVolume">The starting volume</param>
    /// <param name="endVolume">The end volume</param>
    /// <param name="parameters">Parameters to set initial values directly</param>
    /// <returns>A controller for the playing node</returns>
    public static InPlayer PlayAtPosition(GameObject gameObject, InAudioNode audioNode, Vector3 position, float fadeTime, LeanTweenType tweeenType, float startVolume, float endVolume, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
        {
            InDebug.MissingArguments("PlayAtPosition (tween specific)", gameObject, audioNode);
            return null;
        }

        InPlayer player = instance._inAudioEventWorker.PlayAtPosition(gameObject, audioNode, position, parameters);
        player.Volume = startVolume;
        LTDescr tweever = LeanTween.value(gameObject, (f, o) => { (o as InPlayer).Volume = f; }, startVolume, endVolume, fadeTime);
        tweever.onUpdateParam = player;

        tweever.tweenType = tweeenType;

        return player;
    }

    /// <summary>
    /// Play an audio node on InAudio directly so it does not get destroyed in scene transition.
    /// No fade in as code would not get called during scene transition. Works best with simple sound effects
    /// </summary>
    /// <param name="gameObject">The game object to attach to and be controlled by</param>
    /// <param name="audioNode">The node to play</param>
    /// <returns>A controller for the playing node</returns>
    /// <param name="parameters">Parameters to set initial values directly</param>
    public static InPlayer PlayPersistent(Vector3 position, InAudioNode audioNode, AudioParameters parameters = null)
    {
        if (instance == null || audioNode == null || audioNode.IsRootOrFolder)
        {
            InDebug.MissingArgumentsForNode("PlayPersistent", audioNode);
            return null;
        }


        InPlayer player = instance._inAudioEventWorker.PlayAtPosition(instance.gameObject, audioNode, position, parameters);

        return player;
    }

#endregion

#region Stop

    /// <summary>
    /// Stop all instances of the this audio node on the game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="audioNode"></param>
    public static void Stop(GameObject gameObject, InAudioNode audioNode)
    {
        if (instance != null && gameObject != null && audioNode != null)
            instance._inAudioEventWorker.StopByNode(gameObject, audioNode);
        else
        {
            InDebug.MissingArguments("Stop", gameObject, audioNode);
        }
    }

    /// <summary>
    /// Stop all instances of the this audio node on the game object with a fade out time
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="audioNode"></param>
    /// <param name="fadeOutTime"></param>
    public static void Stop(GameObject gameObject, InAudioNode audioNode, float fadeOutTime)
    {
        if (instance != null && gameObject != null && audioNode != null)
            instance._inAudioEventWorker.StopByNode(gameObject, audioNode, fadeOutTime);
        else
        {
            InDebug.MissingArguments("Stop (Fadeout)", gameObject, audioNode);
        }
    }

    /// <summary>
    /// Breaks all looping instances of this node on the game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="audioNode"></param>
    public static void Break(GameObject gameObject, InAudioNode audioNode)
    {
        if (instance != null && gameObject != null && audioNode != null)
            instance._inAudioEventWorker.Break(gameObject, audioNode);
        else
        {
            InDebug.MissingArguments("Break", gameObject, audioNode);
        }
    }

    /// <summary>
    /// Stop all sound effects
    /// </summary>
    /// <param name="gameObject"></param>
    public static void StopAllOfNode(InAudioNode audioNode)
    {
        if (instance != null && audioNode != null)
            instance._inAudioEventWorker.StopAll(0, LeanTweenType.notUsed);
        else
        {
            InDebug.MissingArgumentsForNode("StopAllOfNode", audioNode);
        }
    }

    /// <summary>
    /// Stop all sound effects
    /// </summary>
    /// <param name="gameObject"></param>
    public static void StopAllOfNode(InAudioNode audioNode, float fadeOutDuration, LeanTweenType leanTweenType = LeanTweenType.easeInOutQuad)
    {
        if (instance != null)
            instance._inAudioEventWorker.StopAll(0, leanTweenType);
        else
        {
            InDebug.MissingArgumentsForNode("StopAllOfNode", audioNode);
        }
    }

    /// <summary>
    /// Stop all sound effects
    /// </summary>
    /// <param name="gameObject"></param>
    public static void StopAll()
    {
        if (instance != null)
            instance._inAudioEventWorker.StopAll(0, LeanTweenType.notUsed);
        else
        {
            InDebug.InstanceMissing("StopAll");
        }
    }

    /// <summary>
    /// Stop all sounds & music
    /// </summary>
    /// <param name="gameObject"></param>
    public static void StopAllAndMusic()
    {
        if (instance != null)
        {
            instance._inAudioEventWorker.StopAll(0, LeanTweenType.notUsed);
            Music.StopAll(0, LeanTweenType.notUsed);
        }
        else
        {
            InDebug.InstanceMissing("StopAllAndMusic");
        }
    }


    /// <summary>
    /// Stop all sounds playing with fade out time
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="fadeOut">Time to fade out</param>
    /// <param name="fadeType">Fade type</param>
    public static void StopAll(float fadeOut, LeanTweenType fadeType)
    {
        if (instance != null)
        {
            instance._inAudioEventWorker.StopAll(fadeOut, fadeType);
        }
        else
        {
            InDebug.InstanceMissing("StopAll (fade)");
        }

    }

    /// <summary>
    /// Stop all sounds playing on this game object
    /// </summary>
    /// <param name="gameObject"></param>
    public static void StopAll(GameObject gameObject)
    {
        if (instance != null && gameObject != null)
        {
            instance._inAudioEventWorker.StopAll(gameObject, 0, LeanTweenType.notUsed);
        }
        else
        {
            InDebug.MissingArguments("StopAll (on GameObject)", gameObject, null);
        }
    }

    /// <summary>
    /// Stop all sounds playing on this game object with fade out time
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="fadeOut">Time to fade out</param>
    /// <param name="fadeType">Fade type</param>
    public static void StopAll(GameObject gameObject, float fadeOut, LeanTweenType fadeType)
    {
        if (instance != null && gameObject != null)
        {
            instance._inAudioEventWorker.StopAll(gameObject, 0, LeanTweenType.notUsed);
        }
        else
        {
            InDebug.MissingArguments("StopAll (on GameObject)", gameObject, null);
        }
    }

#endregion

#region Players
    /// <summary>
    /// Get a list of all players attached to this game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    public static InPlayer[] PlayersOnGO(GameObject gameObject)
    {
        if (instance != null && gameObject != null)
        {
            return instance._inAudioEventWorker.GetPlayers(gameObject);
        }
        else
        {
            InDebug.MissingArguments("PlayersOnGO", gameObject, null);
        }
        return null;
    }

    /// <summary>
    /// Copy the list of playing sounds on this game object to a preallocated array
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="copyToList">If the list is too short, the partial list will be copied</param>
    public static void PlayersOnGO(GameObject gameObject, IList<InPlayer> copyToList)
    {
        if (instance != null && gameObject != null && copyToList != null)
        {
            instance._inAudioEventWorker.GetPlayers(gameObject, copyToList);
        }
        else
        {
            if (copyToList == null)
            {
                Debug.LogWarning("InAudio: Missing argument CopyToList on function PlayersOnGO");
            }
            else
            {
                InDebug.MissingArguments("PlayersOnGO", gameObject, null);
            }
        }
    }

#endregion

#endregion

#region Set/Get Parameters

    /// <summary>
    /// Sets the volume for all instances of this audio node on the object. 
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="audioNode"></param>
    /// <param name="volume"></param>
    public static void SetVolumeForNode(GameObject gameObject, InAudioNode audioNode, float volume)
    {
        if (instance != null && gameObject != null && audioNode != null)
        {
            if (!audioNode.IsRootOrFolder)
            {
                instance._inAudioEventWorker.SetVolumeForNode(gameObject, audioNode, volume);
            }
            else
            {
                Debug.LogWarning("InAudio: Cannot change volume for audio folders here, use SetVolumeForAudioFolder() instead.");
            }
        }
        else
        {
            InDebug.MissingArguments("SetVolumeForNode", gameObject, audioNode);
        }
    }

    public static void SetVolumeForAudioFolder(InAudioNode folderNode, float volume)
    {
        if (instance != null && folderNode != null && folderNode.IsRootOrFolder)
        {
            var data = folderNode._nodeData as InFolderData;
            if (data != null)
            {
                data.runtimeVolume = Mathf.Clamp01(volume);
            }
            else
            {
                Debug.LogWarning("InAudio: Cannot set folder volume node that isn't a folder");
            }
        }
        else
        {
            InDebug.MissingArgumentsForNode("SetVolumeForNode", folderNode);
        }
    }

    /// <summary>
    /// Sets the volume for all audio playing on this game object
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="volume"></param>
    public static void SetVolumeForAll(GameObject gameObject, float volume)
    {
        if (instance != null && gameObject != null)
        {
            instance._inAudioEventWorker.SetVolumeForGameObject(gameObject, volume);
        }
        else
        {
            InDebug.MissingArguments("SetVolumeForAll", gameObject, null);
        }
    }

#endregion

#region Post Event by reference
    /// <summary>
    /// Post all actions in this event attached to this gameobject
    /// </summary>
    /// <param name="controllingObject">The controlling object and the future parent the played audio files</param>
    /// <param name="postEvent">The event to post the actions of</param>
    public static void PostEvent(GameObject controllingObject, InAudioEventNode postEvent)
    {
        if (instance != null && controllingObject != null && postEvent != null)
            instance.OnPostEvent(controllingObject, postEvent, controllingObject);
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (postEvent == null)
            {
                InDebug.MissingEvent(controllingObject);
            }
        }
    }

    /// <summary>
    /// Post all actions in this event attached to this another game object than the one controlling it
    /// </summary>
    /// <param name="controllingObject">The controlling object of the played sources</param>
    /// <param name="postEvent">The event to post the actions of</param>
    /// <param name="attachedToOther">The audio source to attach any audio sources to</param>
    public static void PostEventAttachedTo(GameObject controllingObject, InAudioEventNode postEvent, GameObject attachedToOther)
    {
        if (instance != null && controllingObject != null && postEvent != null)
            instance.OnPostEvent(controllingObject, postEvent, attachedToOther);
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (postEvent == null)
            {
                InDebug.MissingEvent(controllingObject);
            }
        }
    }

    /// <summary>
    /// Post all actions in this event at this position with this game object as the controller
    /// </summary>
    /// <param name="controllingObject">The controlling object of the played audio files</param>
    /// <param name="postEvent">The event to post the actions of</param>
    /// <param name="position">The position in world space of the sound</param>
    public static void PostEventAtPosition(GameObject controllingObject, InAudioEventNode postEvent, Vector3 position)
    {
        if (instance != null && controllingObject != null && postEvent != null)
            instance.OnPostEventAtPosition(controllingObject, postEvent, position);
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (postEvent == null)
            {
                InDebug.MissingEvent(controllingObject);
            }
        }
    }

#endregion

#region Post Event lists (inspector lists)

    /// <summary>
    /// Post all actions in this event in accordance to the data specified in the inspector, but overrides which object is it attached to.
    /// </summary>
    /// <param name="controllingObject">The controlling game object and the future parent of the audio files</param>
    /// <param name="eventList">All the events to post as added in the inspector</param>
    public static void PostEvent(GameObject controllingObject, InAudioEvent eventList)
    {
        if (instance != null && controllingObject != null && eventList != null && eventList.Events != null)
        {
            int count = eventList.Events.Count;
            Vector3 position = controllingObject.transform.position;
            for (int i = 0; i < count; i++)
            {
                EventHookListData eventData = eventList.Events[i];
                if (eventData != null && eventData.Event != null)
                {
                    if (eventData.PostAt == EventHookListData.PostEventAt.AttachedTo)
                        instance.OnPostEvent(controllingObject, eventData.Event, controllingObject);
                    else //if post at position
                        instance.OnPostEvent(controllingObject, eventData.Event, position);
                }
            }
        }
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (eventList == null || eventList.Events == null)
            {
                InDebug.MissingEventList(controllingObject);
            }
        }
    }

    /// <summary>
    /// Post all actions in this event in accordance to the data specified in the inspector, but overrides which object is it attached to.
    /// </summary>
    /// <param name="controllingObject">The controlling game object and the future parent of the audio files</param>
    /// <param name="eventList">All the events to post as added in the inspector</param>
    /// <param name="attachedToOther">The object to attach the events to</param>
    public static void PostEventAttachedTo(GameObject controllingObject, InAudioEvent eventList, GameObject attachedToOther)
    {
        if (instance != null && controllingObject != null && eventList != null && eventList.Events != null)
        {
            int count = eventList.Events.Count;
            Vector3 position = controllingObject.transform.position;
            for (int i = 0; i < count; i++)
            {
                EventHookListData eventData = eventList.Events[i];
                if (eventData != null && eventData.Event != null)
                {
                    if (eventData.PostAt == EventHookListData.PostEventAt.AttachedTo)
                        instance.OnPostEvent(controllingObject, eventData.Event, attachedToOther);
                    else //if post at position
                        instance.OnPostEvent(controllingObject, eventData.Event, position);
                }
            }

        }
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (eventList == null || eventList.Events == null)
            {
                InDebug.MissingEventList(controllingObject);
            }
        }
    }

    /// <summary>
    /// Post all actions in this event in accordance to the data specified in the inspector, but overrides another place to fire the sound from
    /// </summary>
    /// <param name="controllingObject">The controlling game object and the future parent of the audio files</param>
    /// <param name="eventList">All the events to post as added in the inspector</param>
    /// <param name="postAt">The new position to play at</param>
    public static void PostEventAtPosition(GameObject controllingObject, InAudioEvent eventList, Vector3 postAt)
    {
        if (instance != null && controllingObject != null && eventList != null && eventList.Events != null)
        {
            int count = eventList.Events.Count;
            for (int i = 0; i < count; i++)
            {
                EventHookListData eventData = eventList.Events[i];
                if (eventData != null && eventData.Event != null)
                {
                    instance.OnPostEvent(controllingObject, eventData.Event, postAt);
                }
            }
        }
        else
        {
            if (instance == null)
                InDebug.InAudioInstanceMissing(controllingObject);
            else if (controllingObject == null)
            {
                InDebug.MissingControllingObject();
            }
            else if (eventList == null || eventList.Events == null)
            {
                InDebug.MissingEventList(controllingObject);
            }
        }
    }

#endregion

#region Find Event by ID

    /// <summary>
    /// Find an Audio Event by id so it can be posted directly
    /// </summary>
    /// <param name="id">The ID of the event to post. The ID is found in the InAudio Event window</param>
    /// <returns>The found audio event. Returns null if not found</returns>
    public static InAudioEventNode FindEventByID(int id)
    {
        InAudioEventNode postEvent = null;
        if (instance != null)
        {
            instance.runtimeData.Events.TryGetValue(id, out postEvent);
        }
        else
        {
            Debug.LogWarning("InAudio: Could not try to find event with id " + id + " as no InAudio instance was found");
        }
        return postEvent;
    }
#endregion

#region Find audio node by ID

    /// <summary>
    /// Finds an audio node based on the ID specified
    /// </summary>
    /// <param name="id">The id to search for</param>
    /// <returns>The found bus, null if not found</returns>
    public static InAudioNode FindAudioNodeById(int id)
    {
        if (instance != null)
        {
            return TreeWalker.FindById(InAudioInstanceFinder.DataManager.AudioTree, id);
        }
        else
        {
            Debug.LogWarning("InAudio: Could not bus with id " + id);
        }
        return null;
    }
#endregion

#region Banks
    /// <summary>
    /// Load all audio clips in this bank
    /// </summary>
    /// <param name="bank">The reference to the bank to load</param>
    public static void LoadBank(InAudioBankLink bank)
    {
        if (bank != null)
            BankLoader.Load(bank);
        else
        {
            InDebug.BankLoadMissing();
        }
    }

    /// <summary>
    /// Unload all audio clips in this banks. Also calls Resources.UnloadUnusedAssets(). Clips will autoreimport if any audio source still referencs this clip
    /// </summary>
    /// <param name="bank">The reference to the bank to unload</param>
    public static void UnloadBank(InAudioBankLink bank)
    {
        if (bank != null)
            BankLoader.Unload(bank);
        else
        {
            InDebug.BankUnloadMissing();
        }
    }

#endregion

    /*Internal systems*/
#region Internal system

    private void HandleEventAction(GameObject controllingObject, AudioEventAction eventData, GameObject attachedTo, Vector3 playAt = new Vector3())
    {
        InAudioNode audioNode; //Because we can't create variables in the scope of the switch with the same name
        InEventBankLoadingAction bankLoadingData;
        InEventSnapshotAction snapshotData;
        InEventMixerValueAction mixerData;
        InEventMusicControl musicControl;
        InEventMusicFade musicFade;
        InEventSoloMuteMusic musicSoloMute;

        if (eventData.Target == null && eventData._eventActionType != EventActionTypes.StopAll)
        {
            InDebug.MissingActionTarget(controllingObject, eventData);
            return;
        }

        switch (eventData._eventActionType)
        {
            case EventActionTypes.Play:
                var audioPlayData = ((InEventAudioAction)eventData);
                audioNode = audioPlayData.Node;
                if (audioNode != null)
                {
                    if (attachedTo != null)
                        _inAudioEventWorker.PlayConnectedTo(controllingObject, audioNode, attachedTo, null, audioPlayData.Fadetime, audioPlayData.TweenType);
                    else
                        _inAudioEventWorker.PlayAtPosition(controllingObject, audioNode, playAt, null, audioPlayData.Fadetime, audioPlayData.TweenType);
                }
                break;
            case EventActionTypes.Stop:
                var data = ((InEventAudioAction)eventData);
                audioNode = data.Node;
                _inAudioEventWorker.StopByNode(controllingObject, audioNode, data.Fadetime, data.TweenType);
                break;
            case EventActionTypes.StopAll:
                var stopAlLData = ((InEventAudioAction)eventData);
                _inAudioEventWorker.StopAll(controllingObject, stopAlLData.Fadetime, stopAlLData.TweenType);
                break;
            case EventActionTypes.Break:
                audioNode = ((InEventAudioAction)eventData).Node;
                _inAudioEventWorker.Break(controllingObject, audioNode);
                break;
            case EventActionTypes.SetSnapshot:
                snapshotData = ((InEventSnapshotAction)eventData);
                snapshotData.Snapshot.TransitionTo(snapshotData.TransitionTime);
                break;
            case EventActionTypes.MixerValue:
                mixerData = ((InEventMixerValueAction)eventData);
                var mixer = mixerData.Mixer;
                var parameter = mixerData.Parameter;
                if (mixerData.TransitionTime > 0)
                {
                    float v;
                    if (mixer.GetFloat(parameter, out v))
                    {
                        var tween = LeanTween.value(gameObject, f => mixer.SetFloat(parameter, f), v, mixerData.Value,
                            mixerData.TransitionTime);
                        tween.onUpdateParam = this;
                        tween.tweenType = mixerData.TransitionType;
                    }
                    else
                    {
                        Debug.LogError("InAudio: Could not find parameter \"" + parameter + "\" on \"" + mixer + "\"");
                    }
                }
                else
                    mixerData.Mixer.SetFloat(mixerData.Parameter, mixerData.Value);
                break;
            case EventActionTypes.BankLoading:
                bankLoadingData = eventData as InEventBankLoadingAction;
                if (bankLoadingData != null)
                {
                    if (bankLoadingData.LoadingAction == BankHookActionType.Load)
                        BankLoader.Load(bankLoadingData.BankLink);
                    else
                    {
                        BankLoader.Unload(bankLoadingData.BankLink);
                    }
                }
                break;
            case EventActionTypes.CrossfadeMusic:
                musicFade = eventData as InEventMusicFade;
                InAudio.Music.SwapCrossfadeVolume(musicFade.From, musicFade.To, musicFade.Duration, musicFade.TweenType);
                break;
            case EventActionTypes.FadeMusic:
                musicFade = eventData as InEventMusicFade;
                if (musicFade.Target != null)
                {
                    switch (musicFade.DoAtEndTo)
                    {
                        case MusicState.Playing:
                        case MusicState.Nothing:
                            InAudio.Music.FadeVolume(musicFade.To, musicFade.ToVolumeTarget, musicFade.Duration,
                                musicFade.TweenType);
                            break;
                        case MusicState.Paused:
                            InAudio.Music.FadeAndPause(musicFade.To, musicFade.ToVolumeTarget, musicFade.Duration,
                                musicFade.TweenType);
                            break;
                        case MusicState.Stopped:
                            InAudio.Music.FadeAndStop(musicFade.To, musicFade.ToVolumeTarget, musicFade.Duration,
                                musicFade.TweenType);
                            break;
                        default:
                            Debug.LogError("InAudio: Unsuported action at end of fade");
                            break;
                    }
                }
                else
                {
                    InDebug.MissingActionTarget(controllingObject, eventData);
                }
                break;
            case EventActionTypes.PlayMusic:
                musicControl = eventData as InEventMusicControl;
                if (musicControl.Target != null)
                {
                    if (!musicControl.Fade)
                    {
                        if (musicControl.ChangeVolume)
                        {
                            InAudio.Music.SetVolume(musicControl.MusicGroup, musicControl.VolumeTarget);
                        }
                        InAudio.Music.Play(musicControl.MusicGroup);
                    }
                    else
                    {
                        if (musicControl.ChangeVolume)
                            InAudio.Music.PlayWithFadeIn(musicControl.MusicGroup, musicControl.VolumeTarget,
                                musicControl.Duration, musicControl.TweenType);
                        else
                            InAudio.Music.PlayWithFadeIn(musicControl.MusicGroup, musicControl.Duration,
                                musicControl.TweenType);
                    }
                }
                else
                {
                    InDebug.MissingActionTarget(controllingObject, eventData);
                }
                break;
            case EventActionTypes.StopMusic:
                musicControl = eventData as InEventMusicControl;
                if (musicControl.Target != null)
                {
                    if (!musicControl.Fade)
                    {
                        if (musicControl.ChangeVolume)
                        {
                            InAudio.Music.SetVolume(musicControl.MusicGroup, musicControl.VolumeTarget);
                        }
                        InAudio.Music.Stop(musicControl.MusicGroup);
                    }
                    else
                    {
                        if (musicControl.ChangeVolume)
                            InAudio.Music.FadeAndStop(musicControl.MusicGroup, musicControl.VolumeTarget,
                                musicControl.Duration, musicControl.TweenType);
                        else
                            InAudio.Music.FadeAndStop(musicControl.MusicGroup, musicControl.Duration,
                                musicControl.TweenType);
                    }
                }
                else
                {
                    InDebug.MissingActionTarget(controllingObject, eventData);
                }
                break;
            case EventActionTypes.PauseMusic:
                musicControl = eventData as InEventMusicControl;
                if (musicControl.Target != null)
                {
                    if (!musicControl.Fade)
                    {
                        if (musicControl.ChangeVolume)
                        {
                            InAudio.Music.SetVolume(musicControl.MusicGroup, musicControl.VolumeTarget);
                        }
                        InAudio.Music.Pause(musicControl.MusicGroup);
                    }
                    else
                    {
                        if (musicControl.ChangeVolume)
                            InAudio.Music.FadeAndPause(musicControl.MusicGroup, musicControl.VolumeTarget,
                                musicControl.Duration, musicControl.TweenType);
                        else
                            InAudio.Music.FadeAndPause(musicControl.MusicGroup, musicControl.Duration,
                                musicControl.TweenType);
                    }
                }
                else
                {
                    InDebug.MissingActionTarget(controllingObject, eventData);
                }
                break;
            case EventActionTypes.SoloMuteMusic:
                {
                    musicSoloMute = eventData as InEventSoloMuteMusic;
                    if (musicSoloMute.Target != null)
                    {
                        if (musicSoloMute.SetSolo)
                            Music.Solo(musicSoloMute.MusicGroup, musicSoloMute.SoloTarget);
                        if (musicSoloMute.SetMute)
                            Music.Solo(musicSoloMute.MusicGroup, musicSoloMute.MuteTarget);
                    }
                    else
                    {
                        InDebug.MissingActionTarget(controllingObject, eventData);
                    }
                    break;
                }
            case EventActionTypes.StopAllMusic:
                musicFade = eventData as InEventMusicFade;
                if (musicFade.Target != null)
                {
                    if (musicFade.Duration > 0)
                    {
                        InAudio.Music.StopAll(musicFade.Duration, musicFade.TweenType);
                    }
                    else
                    {
                        InAudio.Music.StopAll();
                    }
                }
                else
                {
                    InDebug.MissingActionTarget(controllingObject, eventData);
                }
                break;
            default:
                InDebug.UnusedActionType(gameObject, eventData);
                break;
        }

    }

#region Debug

    public static InDebug InDebug = new InDebug();
#endregion

#region Internal event handling


#region Post attached to
    private void OnPostEvent(GameObject controllingObject, InAudioEventNode postEvent, GameObject attachedToOther)
    {
        bool areAnyDelayed = false;
        if (postEvent.Delay > 0)
        {
            StartCoroutine(DelayedEvent(controllingObject, postEvent, attachedToOther));
        }
        else
        {
            areAnyDelayed = PostUndelayedActions(controllingObject, postEvent, attachedToOther);
        }

        if (areAnyDelayed)
        {
            for (int i = 0; i < postEvent._actionList.Count; ++i)
            {
                var eventData = postEvent._actionList[i];
                if (eventData != null && eventData.Delay > 0)
                    StartCoroutine(PostDelayedActions(controllingObject, eventData, attachedToOther));
            }
        }
    }

    private bool PostUndelayedActions(GameObject controllingObject, InAudioEventNode postEvent, GameObject attachedToOther)
    {
        bool areAnyDelayed = false;
        for (int i = 0; i < postEvent._actionList.Count; ++i)
        {
            var eventData = postEvent._actionList[i];
            if (eventData == null)
                continue;
            if (eventData.Delay > 0)
            {
                areAnyDelayed = true;
            }
            else
                HandleEventAction(controllingObject, eventData, attachedToOther);
        }
        return areAnyDelayed;
    }

    private IEnumerator DelayedEvent(GameObject controllingObject, InAudioEventNode postEvent, GameObject attachedToOther)
    {
        yield return new WaitForSeconds(postEvent.Delay);
        PostUndelayedActions(controllingObject, postEvent, attachedToOther);
    }

    private IEnumerator PostDelayedActions(GameObject controllingObject, AudioEventAction eventData, GameObject attachedToOther)
    {
        yield return new WaitForSeconds(eventData.Delay);
        HandleEventAction(controllingObject, eventData, attachedToOther);
    }
#endregion
#region Post at position
    private void OnPostEventAtPosition(GameObject controllingObject, InAudioEventNode audioEvent, Vector3 position)
    {
        if (instance != null && controllingObject != null && audioEvent != null)
            instance.OnPostEvent(controllingObject, audioEvent, position);
    }

    private void OnPostEvent(GameObject controllingObject, InAudioEventNode postEvent, Vector3 postAt)
    {
        bool areAnyDelayed = false;
        if (postEvent.Delay > 0)
        {
            StartCoroutine(DelayedEvent(controllingObject, postEvent, postAt));
        }
        else
        {
            areAnyDelayed = PostUndelayedActions(controllingObject, postEvent, postAt);
        }

        if (areAnyDelayed)
        {
            for (int i = 0; i < postEvent._actionList.Count; ++i)
            {
                var eventData = postEvent._actionList[i];
                if (eventData != null && eventData.Delay > 0)
                    StartCoroutine(PostDelayedActions(controllingObject, eventData, postAt));
            }
        }
    }

    private bool PostUndelayedActions(GameObject controllingObject, InAudioEventNode postEvent, Vector3 postAt)
    {
        bool areAnyDelayed = false;
        for (int i = 0; i < postEvent._actionList.Count; ++i)
        {
            var eventData = postEvent._actionList[i];
            if (eventData == null)
                continue;
            if (eventData.Delay > 0)
            {
                areAnyDelayed = true;
            }
            else
                HandleEventAction(controllingObject, eventData, null, postAt);
        }
        return areAnyDelayed;
    }

    private IEnumerator DelayedEvent(GameObject controllingObject, InAudioEventNode postEvent, Vector3 postAt)
    {
        yield return new WaitForSeconds(postEvent.Delay);
        PostUndelayedActions(controllingObject, postEvent, postAt);
    }

    private IEnumerator PostDelayedActions(GameObject controllingObject, AudioEventAction eventData, Vector3 postAt)
    {
        yield return new WaitForSeconds(eventData.Delay);
        HandleEventAction(controllingObject, eventData, null, postAt);
    }
#endregion
#endregion

#region Internal data

    private InAudioEventWorker _inAudioEventWorker;

    private InRuntimeAudioData runtimeData;

    private static InAudio instance;


#endregion

#region Unity functions
    void Update()
    {
#if UNITY_EDITOR
        if (InAudioInstanceFinder.Instance == null || InAudioInstanceFinder.DataManager == null || InAudioInstanceFinder.MusicPlayer == null)
        {
            Debug.LogError("There seems to be a problem with the InAudio prefab. Please try to remove it from the scene and re-add it from the prefab.");
            return;
        }
#endif

        var musicTree = InAudioInstanceFinder.DataManager.MusicTree;
        if (musicTree != null)
        {
            bool anySolo = MusicUpdater.UpdateSoloMute(musicTree);

            MusicUpdater.UpdateVolumePitch(musicTree, 1.0f, 1.0f, anySolo);
            AudioUpdater.AudioTreeUpdate(InAudioInstanceFinder.DataManager.AudioTree, 1.0f);
        }
#if UNITY_EDITOR //Remove condition in player, always update in build
        if (Application.isPlaying)
#endif
        {
            var controllerPool = InAudioInstanceFinder.RuntimePlayerControllerPool;
            if (controllerPool != null)
            {
                controllerPool.DelayedRelease();
            }

            var playerPool = InAudioInstanceFinder.InRuntimePlayerPool;
            if (playerPool != null)
            {
                playerPool.DelayedRelease();
            }
        }
    }

    public const string CurrentVersion = "2.6.1";

    private static AudioListener FindActiveListener()
    {
        return Object.FindObjectsOfType(typeof(AudioListener)).FindFirst(activeListener => (activeListener as AudioListener).gameObject.activeInHierarchy) as AudioListener;
    }

    void Awake()
    {
#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            if(InAudio.instance == null)
            { 
                Music = GetComponentInChildren<MusicPlayer>();
                if (Music == null)
                {
                    Debug.LogError(
                        "InAudio: Could not find music player in InAudio Mananger object.\nPlease add the 'InAudio Manager' prefab to the scene again or reimport the project from the Asset Store and try again.");

                }
            }

        }
    }

    void OnEnable()
    {
        if (instance == null || instance == this)
        {
            if (activeAudioListener == null)
                activeAudioListener = FindActiveListener();

            instance = this;

            InitializeInAudio();
        }
        else
        {
            Object.Destroy(transform.gameObject);
        }
    }

    private void InitializeInAudio()
    {

#if UNITY_EDITOR
        if (Application.isPlaying)
#endif
        {
            //Music = InAudioInstanceFinder.MusicPlayer;

            DontDestroyOnLoad(transform.gameObject);


            _inAudioEventWorker = GetComponentInChildren<InAudioEventWorker>();
            runtimeData = GetComponentInChildren<InRuntimeAudioData>();
            BankLoader.LoadAutoLoadedBanks();

            if (InAudioInstanceFinder.DataManager.Loaded)
            {
                runtimeData.UpdateEvents(InAudioInstanceFinder.DataManager.EventTree);

                //See MusicPlayer for initialization of audio
            }
            else
            {
                Debug.LogError("InAudio: There was a problem loading the InAudio project. Have you created one?");
            }
        }

    }




#if UNITY_EDITOR
    void OnApplicationQuit()
    {
        InDebug.DoLog = false;
    }
#endif

#endregion

#region Other

    public static InAudioEventWorker _getEventWorker()
    {
        if (instance != null)
            return instance._inAudioEventWorker;
        return null;
    }

#endregion

#endregion
}