using UnityEngine;

namespace InAudioSystem.InAudioEditor
{
public class InAudioGUIUserPrefs : MonoBehaviour
{
#if UNITY_EDITOR
    public GUIPrefs AudioGUIData = new GUIPrefs();
    public GUIPrefs EventGUIData = new GUIPrefs();
    public GUIPrefs MusicGUIData = new GUIPrefs();
    public GUIPrefs BankGUIData = new GUIPrefs();

    [System.NonSerialized] public InSpline SelectedSplineController;
#endif
}



    [System.Serializable]
    public class GUIPrefs 
    {
        public int SelectedNode;
        public Vector2 Position;
    }
}
