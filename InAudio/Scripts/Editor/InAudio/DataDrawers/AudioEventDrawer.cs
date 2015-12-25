using System;
using System.Collections.Generic;
using InAudioLeanTween;
using InAudioSystem.ExtensionMethods;
using InAudioSystem.ReorderableList;
using InAudioSystem.Runtime;
using UnityEditor;
using UnityEngine;
using UnityEngine.Audio;

namespace InAudioSystem.InAudioEditor
{

public static class AudioEventDrawer
{
    private static InAudioEventNode lastEvent;
    private static AudioEventAction audioEventAction = null;
    private static Vector2 scrollPos;
    //private static Rect drawArea;
    private static AudioEventAction toRemove = null;
    private static EventActionListAdapter ListAdapter ;

    private static GUIStyle leftStyle = new GUIStyle(GUI.skin.label);

    private static GameObject eventObjectTarget;


    public static bool Draw(InAudioEventNode audioevent)
    {
        if (ListAdapter == null)
        {
            ListAdapter = new EventActionListAdapter();
            ListAdapter.DrawEvent = DrawItem;
            ListAdapter.ClickedInArea = ClickedInArea;
        }

        if (lastEvent != audioevent)
        {
            ListAdapter.Event = audioevent;

            audioEventAction = null;
            if (audioevent._actionList.Count > 0)
            {
                audioEventAction = audioevent._actionList[0];
            }
        }
        EditorGUILayout.BeginVertical();

        lastEvent = audioevent;
        InUndoHelper.GUIUndo(audioevent, "Name Change", ref audioevent.Name, () => 
            EditorGUILayout.TextField("Name", audioevent.Name));
        
        bool repaint = false;
      
        if (audioevent._type == EventNodeType.Event)
        {
            EditorGUIHelper.DrawID(audioevent._guid);

            if (Application.isPlaying)
            {
                eventObjectTarget = EditorGUILayout.ObjectField("Event preview target", eventObjectTarget, typeof(GameObject), true) as GameObject;
                
                if (eventObjectTarget != null )
                {
                    bool prefab = PrefabUtility.GetPrefabParent(eventObjectTarget) == null && PrefabUtility.GetPrefabObject(eventObjectTarget) != null;
                    if (!prefab)
                    {
                        EditorGUILayout.BeginHorizontal();
                        if (GUILayout.Button("Post event"))
                        {
                            InAudio.PostEvent(eventObjectTarget, audioevent);
                        }
                        if (GUILayout.Button("Stop All Sounds/Music in Event"))
                        {
                            InAudio.StopAll(eventObjectTarget);
                            foreach (var eventAction in audioevent._actionList)
                            {
                                var music = eventAction.Target as InMusicGroup;
                                if (music != null)
                                {
                                    InAudio.Music.Stop(music);
                                }
                            }
                        }
                        EditorGUILayout.EndHorizontal();
                    }
                    else
                    {
                        EditorGUILayout.HelpBox("Cannot post events on Prefab", MessageType.Error);
                    }
                }
                EditorGUILayout.Separator();
            }

            

            InUndoHelper.GUIUndo(audioevent, "Delay", ref audioevent.Delay, () =>
                Mathf.Max(EditorGUILayout.FloatField("Delay", audioevent.Delay), 0));
          
            NewEventArea(audioevent);

            EditorGUILayout.Separator();
            EditorGUILayout.Separator();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);

                repaint = DrawContent();

            EditorGUILayout.EndScrollView();
            DrawSelected(audioEventAction);
        }
        else if (audioevent._type == EventNodeType.Folder)
        {
            if (audioevent.PlacedExternaly)
            {
                EditorGUILayout.Separator();

                GUI.enabled = false;
                EditorGUILayout.ObjectField("Placed on", audioevent.gameObject, typeof(GameObject), false);
                GUI.enabled = true;

                EditorGUILayout.Separator();
            }
        }

        EditorGUILayout.EndVertical(); 

        if (toRemove != null)
        {
            InUndoHelper.DoInGroup(() =>
            {
                //Remove the required piece
                int index = audioevent._actionList.FindIndex(toRemove);
                AudioEventWorker.DeleteActionAtIndex(audioevent, index);
                
            });
            toRemove = null;

        }
        else //Remove all actions that does not excist. 
        {
            audioevent._actionList.RemoveAll(p => p == null);
        }
        return repaint;
    }

    private static void ClickedInArea(int i)
    {
        audioEventAction = lastEvent._actionList[i];
    }

    private static void DrawSelected(AudioEventAction eventAction)
    {
        if (eventAction != null)
        {
            Rect thisArea = EditorGUILayout.BeginVertical(GUILayout.Height(120));
            EditorGUILayout.LabelField("");
            var buttonArea = thisArea;
            buttonArea.height = 16;

            GUI.skin.label.alignment = TextAnchor.UpperLeft;

            InUndoHelper.GUIUndo(eventAction, "Event Action Delay", ref eventAction.Delay, () =>
                Mathf.Max(EditorGUI.FloatField(buttonArea, "Seconds Delay", eventAction.Delay), 0));
            
            buttonArea.y += 33;

            var bankLoadingAction = eventAction as InEventBankLoadingAction;
            var audioAction = eventAction as InEventAudioAction;
            var snapshotAction = eventAction as InEventSnapshotAction;
            var mixerAction = eventAction as InEventMixerValueAction;
            var musicControlAction = eventAction as InEventMusicControl;
            var musicFadeAction = eventAction as InEventMusicFade;
            var musicSoloMuteAction = eventAction as InEventSoloMuteMusic;

            if (audioAction != null)
            {
                if (audioAction._eventActionType == EventActionTypes.Play || audioAction._eventActionType == EventActionTypes.Stop || audioAction._eventActionType == EventActionTypes.StopAll)
                {
                    InUndoHelper.GUIUndo(audioAction, "Fade Time", ref audioAction.Fadetime,
                        () => Mathf.Max(0, EditorGUILayout.FloatField("Fade Time", audioAction.Fadetime)));
                    InUndoHelper.GUIUndo(audioAction, "Fade Type", ref audioAction.TweenType,
                        () => (LeanTweenType) EditorGUILayout.EnumPopup("Fade Type", audioAction.TweenType));
                    if (audioAction.TweenType == LeanTweenType.animationCurve)
                    {
                        EditorGUILayout.HelpBox("Animation curve type is not supported", MessageType.Warning);
                    }
                }
            }
            else if (bankLoadingAction != null)
            {
                InUndoHelper.GUIUndo(bankLoadingAction, "Bank Loading Action", ref bankLoadingAction.LoadingAction, () =>
                    (BankHookActionType)EditorGUI.EnumPopup(buttonArea, "Load Action", bankLoadingAction.LoadingAction));
            }
            else if (snapshotAction != null)
            {
                InUndoHelper.GUIUndo(snapshotAction, "Snapshot Transition Action", ref snapshotAction.Snapshot, () =>
                    (AudioMixerSnapshot)EditorGUILayout.ObjectField("Transition Action", snapshotAction.Snapshot, typeof(AudioMixerSnapshot), false));
                InUndoHelper.GUIUndo(snapshotAction, "Snapshot Transition Time", ref snapshotAction.TransitionTime, () => 
                    EditorGUILayout.FloatField("Transition Time", snapshotAction.TransitionTime));
            }
            else if (mixerAction != null)
            {
                InUndoHelper.GUIUndo(mixerAction, "Mixer Value", ref mixerAction.Mixer, () =>
                    (AudioMixer)EditorGUILayout.ObjectField("Audio Mixer", mixerAction.Mixer, typeof(AudioMixer), false));
                InUndoHelper.GUIUndo(mixerAction, "Parameter", ref mixerAction.Parameter, () =>
                    EditorGUILayout.TextField("Parameter", mixerAction.Parameter));
                InUndoHelper.GUIUndo(mixerAction, "Value", ref mixerAction.Value, () =>
                    EditorGUILayout.FloatField("Value", mixerAction.Value));
                EditorGUILayout.Separator();
                InUndoHelper.GUIUndo(mixerAction, "Transition Time", ref mixerAction.TransitionTime, () =>
                    Mathf.Max(0, EditorGUILayout.FloatField("Transition Time", mixerAction.TransitionTime)));
                InUndoHelper.GUIUndo(mixerAction, "Transition Type", ref mixerAction.TransitionType, () =>
                    (LeanTweenType)EditorGUILayout.EnumPopup("Transition Type", mixerAction.TransitionType));
                if (mixerAction.TransitionType == LeanTweenType.animationCurve)
                {
                    EditorGUILayout.HelpBox("Animation curve type is not supported", MessageType.Warning);
                }
            }
            else if (musicControlAction != null)
            {
                InUndoHelper.GUIUndo(musicControlAction, "Fade Action", ref musicControlAction.Fade, () =>
                        EditorGUILayout.Toggle("Fade Action", musicControlAction.Fade));

                if (musicControlAction.Fade)
                {
                    InUndoHelper.GUIUndo(musicControlAction, "Transition Time", ref musicControlAction.Duration, () =>
                        Mathf.Max(0, EditorGUILayout.FloatField("Transition Time", musicControlAction.Duration)));

                    InUndoHelper.GUIUndo(musicControlAction, "Transition Type", ref musicControlAction.TweenType, () =>
                        (LeanTweenType) EditorGUILayout.EnumPopup("Transition Type", musicControlAction.TweenType));

                   
                }

                
                InUndoHelper.GUIUndo(musicControlAction, "Set Volume Target", ref musicControlAction.ChangeVolume,
                    () =>
                        EditorGUILayout.Toggle("Set Volume Target", musicControlAction.ChangeVolume));
                if (musicControlAction.ChangeVolume)
                {
                    InUndoHelper.GUIUndo(musicControlAction, "Volume Target", ref musicControlAction.Duration,
                        () => Mathf.Clamp01(EditorGUILayout.Slider("Volume Target", musicControlAction.Duration, 0, 1)));
                }
                
            }
            else if (musicFadeAction != null)
            {
                if (musicFadeAction._eventActionType == EventActionTypes.CrossfadeMusic)
                {
                    InUndoHelper.GUIUndo(musicFadeAction, "Fade Target", ref musicFadeAction.From, () => EditorGUILayout.ObjectField("From Target", musicFadeAction.To, typeof(InEventMusicFade),false) as InMusicGroup);
                    InUndoHelper.GUIUndo(musicFadeAction, "Fade Target", ref musicFadeAction.To, () => EditorGUILayout.ObjectField("To Target", musicFadeAction.To, typeof(InEventMusicFade), false) as InMusicGroup);    
                }

                if (musicFadeAction._eventActionType == EventActionTypes.FadeMusic)
                {
                    InUndoHelper.GUIUndo(musicFadeAction, "Volume Target", ref musicFadeAction.ToVolumeTarget, () =>
                        EditorGUILayout.Slider("Volume Target", musicFadeAction.ToVolumeTarget, 0f, 1f));
                }

                InUndoHelper.GUIUndo(musicFadeAction, "Transition Time", ref musicFadeAction.Duration, () =>
                    Mathf.Max(0, EditorGUILayout.FloatField("Transition Time", musicFadeAction.Duration)));

                InUndoHelper.GUIUndo(musicFadeAction, "Transition Type", ref musicFadeAction.TweenType, () =>
                    (LeanTweenType)EditorGUILayout.EnumPopup("Transition Type", musicFadeAction.TweenType));
                
                if (musicFadeAction.TweenType == LeanTweenType.animationCurve)
                {
                    EditorGUILayout.HelpBox("Animation curve type is not supported", MessageType.Warning);
                }

                if (musicFadeAction._eventActionType == EventActionTypes.FadeMusic)
                {
                    InUndoHelper.GUIUndo(musicFadeAction, "Do At End", ref musicFadeAction.DoAtEndTo, () =>
                        (MusicState)EditorGUILayout.EnumPopup("Do At End", musicFadeAction.DoAtEndTo));
                    if (musicFadeAction.DoAtEndTo == MusicState.Playing)
                    {
                        EditorGUILayout.HelpBox("\"Playing\" does the same as \"Nothing\", it does not start playing", MessageType.Info );
                    }
                }
                
            }
            else if (musicSoloMuteAction != null)
            {
                InUndoHelper.GUIUndo(musicSoloMuteAction, "Set Solo", ref musicSoloMuteAction.SetSolo, () =>
                        EditorGUILayout.Toggle("Set Solo", musicSoloMuteAction.SetSolo));
                if (musicSoloMuteAction.SetSolo)
                {
                    InUndoHelper.GUIUndo(musicSoloMuteAction, "Solo Target", ref musicSoloMuteAction.SoloTarget, () =>
                        EditorGUILayout.Toggle("Solo Target", musicSoloMuteAction.SoloTarget));
                }
                EditorGUILayout.Separator();
                InUndoHelper.GUIUndo(musicSoloMuteAction, "Set Mute", ref musicSoloMuteAction.SetMute, () =>
                        EditorGUILayout.Toggle("Set Mute", musicSoloMuteAction.SetMute));
                if (musicSoloMuteAction.SetMute)
                {
                    InUndoHelper.GUIUndo(musicSoloMuteAction, "Solo Mute", ref musicSoloMuteAction.MuteTarget, () =>
                        EditorGUILayout.Toggle("Solo Mute", musicSoloMuteAction.MuteTarget));
                }
            }
            EditorGUILayout.EndVertical();
        }
    }

    private static void NewEventArea(InAudioEventNode audioevent)
    {
        var defaultAlignment = GUI.skin.label.alignment;

        EditorGUILayout.BeginHorizontal();

        GUI.skin.label.alignment = TextAnchor.MiddleLeft;

        EditorGUILayout.LabelField("");

        EditorGUILayout.EndHorizontal();
        Rect lastArea = GUILayoutUtility.GetLastRect();
        lastArea.height *= 1.5f;

        if (GUI.Button(lastArea, "Click or drag here to add event action"))
        {
            ShowCreationContext(audioevent);
        }
        
        var dragging = DragAndDrop.objectReferences;
        OnDragging.OnDraggingObject(dragging, lastArea,
            objects => AudioEventWorker.CanDropObjects(audioevent, dragging),
            objects => AudioEventWorker.OnDrop(audioevent, dragging));
        
        GUI.skin.label.alignment = defaultAlignment;
    }

    private static bool DrawContent()
    {
        ReorderableListGUI.ListField(ListAdapter, ReorderableListFlags.DisableContextMenu | ReorderableListFlags.HideAddButton | ReorderableListFlags.HideRemoveButtons );
    
        return false;
    }

    private static void DrawItem(Rect position, int i)
    {
        var item = lastEvent._actionList[i];
        leftStyle.alignment = TextAnchor.MiddleLeft;
        if(item == audioEventAction)
            DrawBackground(position);

        Rect fullArea = position;
        Rect typePos = position;
        typePos.width = 110;
        if (item != null)
        {
            
            if (GUI.Button(typePos, EventActionExtension.GetList().Find(p => p.Value == (int)item._eventActionType).Name))
            {
                ShowChangeContext(lastEvent, item);
                Event.current.UseEvent();
            }
        }
        else
            GUI.Label(position, "Missing data", leftStyle);


        typePos.x += 130;
        if (item != null && item._eventActionType != EventActionTypes.StopAll && item._eventActionType != EventActionTypes.StopAllMusic)
        {
            Rect area = typePos;
            area.width = position.width - 200;
            GUI.Label(area, item.ObjectName);
        }
        HandleDragging(item, typePos);

        position.x = position.x + position.width - 75;
        position.width = 45;

        if (item != null && item._eventActionType != EventActionTypes.StopAll && item._eventActionType != EventActionTypes.StopAllMusic)
        {
            if (GUI.Button(position, "Find"))
            {
                SearchHelper.SearchForActionTarget(item);
            }
        }

        position.x += 50;
        position.width = 20;

        if (audioEventAction == item)
        {
            DrawBackground(position);
        }

        if (GUI.Button(position, "X"))
        {
            toRemove = item;
        }

        if (Event.current.ClickedWithin(fullArea))
        {
            audioEventAction = item;
            Event.current.UseEvent();
        }
    }

    private static void HandleDragging(AudioEventAction currentAction, Rect dragArea)
    {
        if (currentAction != null)
        {
            
            if (currentAction is InEventAudioAction)
            {
                InAudioNode dragged = OnDragging.DraggingObject<InAudioNode>(dragArea, node => node.IsPlayable);
                if (dragged != null)
                {
                    InUndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }
            else if (currentAction is InEventBankLoadingAction)
            {
                InAudioBankLink dragged = OnDragging.DraggingObject<InAudioBankLink>(dragArea,
                    bank => bank._type == AudioBankTypes.Bank);

                if (dragged != null)
                {
                    InUndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }
            else if(currentAction is InEventMixerValueAction)
            {
                AudioMixer dragged = OnDragging.DraggingObject<AudioMixer>(dragArea);
                if (dragged != null)
                {
                    InUndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }
            else if (currentAction is InEventMusicControl || currentAction is InEventMusicFade || currentAction is InEventSoloMuteMusic)
            {
                InMusicGroup dragged = OnDragging.DraggingObject<InMusicGroup>(dragArea);
                if (dragged != null)
                {
                    InUndoHelper.RecordObject(currentAction, "Change Action Type");
                    currentAction.Target = dragged;
                }
            }
        }
    }


    private static void DrawBackground(Rect area)
    {
        GUI.depth += 10;
        GUI.DrawTexture(area, EditorResources.Instance.GetBackground());
        GUI.depth -= 10;
    }

    private static void ShowChangeContext(InAudioEventNode audioEvent, AudioEventAction action)
    {
        var menu = new GenericMenu();

        List<EventActionExtension.ActionMeta> actionList = EventActionExtension.GetList();
        foreach (EventActionExtension.ActionMeta currentType in actionList)
        {
            var enumType = currentType.ActionType;
            menu.AddItem(
                new GUIContent(currentType.Name),
                false,
                f => ChangeAction(audioEvent, action, enumType),
                currentType.ActionType
                );
        }
        
        menu.ShowAsContext();
    }

    private static void ChangeAction(InAudioEventNode audioEvent, AudioEventAction action, EventActionTypes newEnumType)
    {
        for (int i = 0; i < audioEvent._actionList.Count; ++i)
        {
            if (audioEvent._actionList[i] == action)
            {
                Type oldType = AudioEventAction.ActionEnumToType(action._eventActionType);
                Type newType = AudioEventAction.ActionEnumToType(newEnumType);


                if (oldType != newType)
                {
                    InUndoHelper.DoInGroup(() => AudioEventWorker.ReplaceActionDestructiveAt(audioEvent, newEnumType, i));
                }
                else
                {
                    InUndoHelper.RecordObject(action, "Change Event Action Type");
                    action._eventActionType = newEnumType;
                }    
                
                break;
            }
        }
    }

    private static void ShowCreationContext(InAudioEventNode audioevent)
    {
        var menu = new GenericMenu();

        List<EventActionExtension.ActionMeta> actionList = EventActionExtension.GetList();
        foreach (EventActionExtension.ActionMeta currentType in actionList)
        {
            Type newType = AudioEventAction.ActionEnumToType(currentType.ActionType);
            var enumType = currentType.ActionType;
            menu.AddItem(new GUIContent(currentType.Name), false, f =>
                AudioEventWorker.AddEventAction(audioevent, newType, enumType), currentType);
        }
        menu.ShowAsContext();
    }
}
}