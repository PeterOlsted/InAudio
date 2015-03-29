using InAudioSystem.ExtensionMethods;
using InAudioSystem.Internal;
using UnityEngine;

#if UNITY_EDITOR
[ExecuteInEditMode]
#endif
public class InSplineNode : MonoBehaviour
{
    public InSpline SplineController;

    private Transform Trans;

    public Transform GetTransform
    {
        get { return Trans; }
    }

    void Awake()
    {
        Trans = transform;
    }

    public void ConnectTo(InSplineNode node)
    {
        if (node != this)
        {
            SplineController.AddConnection(this, node);
        }
    }

    public void UnconnectTo(InSplineNode node)
    {
        if (node != this)
        {
            if(SplineController != null)
                SplineController.RemoveConnections(this, node);
        }
    }

    public void UnconnectAll()
    {
        SplineController.RemoveConnections(this);
    }

    void OnDestroy() 
    {
        if (SplineController != null)
        {
            RemoveSplineConnections(); 
        }
    }

    private void RemoveSplineConnections()
    {
        SplineController.Nodes.Remove(this);
        var connections = SplineController.Connections;
        for (int i = 0; i < connections.Count; i++)
        {
            var connection = connections[i];
            if (connection.NodeA == this || connection.NodeB == this)
            {
                connections.SafeRemoveAt(ref i);
            }
        }
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {

        if (InAudioInstanceFinder.IsValid)
        {
            var guiPrefs = InAudioInstanceFinder.InAudioGuiUserPrefs;

            if (guiPrefs != null)
            {
                if (guiPrefs.SelectedSplineController == SplineController)
                    Gizmos.DrawCube(transform.position, Vector3.one * 0.3f);
            }
        }

    }
#endif
}