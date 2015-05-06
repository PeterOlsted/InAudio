using InAudioSystem;
using UnityEngine;

[AddComponentMenu("InAudio/Event Hook/Audio Event Hook")]
public class InAudioEventHook : MonoBehaviour
{
    public InAudioEvent onEnable;

    public InAudioEvent onStart;

    public InAudioEvent onDisable;

    public InAudioEvent onDestroy;

    public InHookMusicControl onEnableMusic;

    public InHookMusicControl onStartMusic;

    public InHookMusicControl onDisableMusic;

    public InHookMusicControl onDestroyMusic;

    public AudioEventCollisionList CollisionList; 

    void OnEnable()
    {
        
        InAudio.PostEvent(gameObject, onEnable);
        if (onEnableMusic != null)
        {
            HandleMusic(onEnableMusic, "OnEnable");
        }
    }

    void Start()
    {
        InAudio.PostEvent(gameObject, onStart);
        if (onEnableMusic != null)
        {
            HandleMusic(onStartMusic, "OnStart");
        }
    }

    void OnDisable()
    {
        InAudio.PostEvent(gameObject, onDisable);
        if (onDisableMusic != null)
        {
            HandleMusic(onDisableMusic, "OnDisable");
        }
    }

    void OnDestroy()
    {
        InAudio.PostEvent(gameObject, onDestroy);
        if (onDestroyMusic != null)
        {
            HandleMusic(onDestroyMusic, "OnDestroy");
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (CollisionList.CollisionReaction)
        {
            if (IsInLayerMask(collision.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsEnter);
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (CollisionList.CollisionReaction)
        {
            if (IsInLayerMask(collision.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsExit);
            }
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (CollisionList.TriggerReaction)
        {
            if (IsInLayerMask(collider.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsEnter);
            }
        }
    }

    void OnTriggerExit(Collider collider)
    {
        if (CollisionList.TriggerReaction)
        {
            if (IsInLayerMask(collider.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsExit);
            }
        }
    }

    private bool IsInLayerMask(GameObject obj, LayerMask layerMask)
    {
        // Convert the object's layer to a bitfield for comparison
        int objLayerMask = (1 << obj.layer);
        if ((layerMask.value & objLayerMask) > 0)  // Extra round brackets required!
            return true;
        else
            return false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (CollisionList.CollisionReaction)
        {
            if (IsInLayerMask(collision.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsEnter);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (CollisionList.CollisionReaction)
        {
            if (IsInLayerMask(collision.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsExit);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (CollisionList.TriggerReaction)
        {
            if (IsInLayerMask(collider.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsEnter);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (CollisionList.TriggerReaction)
        {
            if (IsInLayerMask(collider.gameObject, CollisionList.Layers))
            {
                InAudio.PostEvent(gameObject, CollisionList.EventsExit);
            }
        }
    }

    private void HandleMusic(InHookMusicControl musicControl, string eventType)
    {
        var controls = musicControl.MusicControls;
        int count = controls.Count;
        for (int i = 0; i < count; i++)
        {
            
            var control = controls[i];
            var music = control.MusicGroup;
            if (music == null)
            {
                Debug.LogWarning("InAudio: Event hook missing music entry " + i + " for " + eventType + " on game object \"" + gameObject.name + "\"", gameObject);
                continue;
            }
            if (control.PlaybackControl == MusicState.Playing)
            {
                InAudio.Music.Play(control.MusicGroup);
            }
            else if (control.PlaybackControl == MusicState.Stopped)
            {
                InAudio.Music.Stop(control.MusicGroup);
            }
            else if (control.PlaybackControl == MusicState.Paused)
            {
                InAudio.Music.Pause(control.MusicGroup);
            }
        }
    }
}