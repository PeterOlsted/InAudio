using System.Collections.Generic;
using InAudioSystem;
using InAudioSystem.ExtensionMethods;
using UnityEngine;

[AddComponentMenu("InAudio/Spline/Audio Spline")]
public class InSpline : MonoBehaviour
{
    public InAudioEvent SplineAudioEvent;

    public List<InSplineNode> Nodes = new List<InSplineNode>();
    public List<InSplineConnection> Connections = new List<InSplineConnection>();

    private GameObject movingObject;
    private Transform movingTransform;

    public Vector3 SoundPosition
    {
        get { return movingTransform.position; }
    }

    public Quaternion SoundRotation
    {
        get { return movingTransform.rotation; }
    }

    void Awake()
    {
        movingObject = new GameObject();
        movingObject.transform.parent = transform;
        movingObject.transform.localPosition = new Vector3();
        movingObject.name = "Audio source";
        movingTransform = movingObject.transform;
    }


    void OnEnable()
    {
        InAudio.PostEventAttachedTo(gameObject, SplineAudioEvent, movingObject);
    }

    void LateUpdate()
    {
        AudioListener listener = InAudio.ActiveListener;
        if (Connections.Count >= 1)
        {
            PlaceSource(listener);
        }
        else if (Nodes.Count == 1)
        {
            movingTransform.position = Nodes[0].GetTransform.position;
        }
    }

    private void PlaceSource(AudioListener listener)
    {
        if (listener != null)
        {
            Vector3 listenerPos = listener.transform.position;
            float minDistance = float.MaxValue;
            Vector3 closetsPoint = Vector3.zero;

            int count = Connections.Count;
            for (int i = 0; i < count; i++)
            {
                var connection = Connections[i];
                Vector3 posA;
                Vector3 posB;
                if (connection.NodeA != null)
                    posA = connection.NodeA.transform.position;
                else
                    continue;

                if (connection.NodeB != null)
                    posB = connection.NodeB.transform.position;
                else
                    continue;

                Vector3 minPos;
                float distance = PointToSegmentDistance(listenerPos, posA, posB, out minPos);

                if (distance < minDistance)
                {
                    minDistance = distance;
                    closetsPoint = minPos;
                }
            }

            movingTransform.position = closetsPoint;
            movingTransform.rotation = Quaternion.LookRotation((InAudio.ActiveListener.transform.position - closetsPoint).normalized);
        }
    }

    public InSplineNode[] FindConnectedNodes(InSplineNode node)
    {
        List<InSplineNode> nodes = new List<InSplineNode>();
        foreach (var connection in Connections)
        {
            if(connection.NodeA == node)
                nodes.Add(connection.NodeB);
            else if (connection.NodeB == node)
                nodes.Add(connection.NodeA);
        }
        return nodes.ToArray();
    }

    private float PointToSegmentDistance(Vector3 P, Vector3 S0, Vector3 S1, out Vector3 point)
    {
        Vector3 v = S1 - S0;
        Vector3 w = P - S0;


        float c1 = Vector3.Dot(w, v);
        if (c1 <= 0)
        {
            point = S0;
            return Vector3.Distance(P, S0);
        }

        float c2 = Vector3.Dot(v, v);
        if (c2 <= c1)
        {
            point = S1;
            return Vector3.Distance(P, S1);
        }

        float b = c1 / c2;
        point = S0 + b * v;
        return Vector3.Distance(P, point);
    }
     
    void OnDrawGizmos()
    {
        if (movingTransform != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawRay(movingTransform.position, movingTransform.forward);
            if (InAudio.ActiveListener != null)
                Gizmos.DrawCube(InAudio.ActiveListener.gameObject.transform.position, new Vector3(1, 1, 1) * 0.3f);
        }
    }



    public void AddConnection(InSplineNode nodeA, InSplineNode nodeB)
    {
        bool contains = false;
        for (int i = 0; i < Connections.Count; i++)
        {
            var connection = Connections[i];
            if ((connection.NodeA == nodeA && connection.NodeB == nodeB) || (connection.NodeA == nodeB && connection.NodeB == nodeA))
            {
                contains = true;
                break;
            }
        }
        if(!contains)
            Connections.Add(new InSplineConnection(nodeA, nodeB));
    }

    public bool ContainsConnection(InSplineNode a, InSplineNode b)
    {
        for (int i = 0; i < Connections.Count; i++)
        {
            var con = Connections[i];
            if (con.NodeA == a && con.NodeB == b || con.NodeA == b && con.NodeB == a)
                return true;
        }
        return false;
    }

    public void RemoveConnections(InSplineConnection connection)
    {
        for (int i = 0; i < Connections.Count; i++)
        {
            var curConnection = Connections[i];
            if (connection == curConnection)
            {
                Connections.SafeRemoveAt(ref i);
            }
        }
    }

    public void RemoveConnections(InSplineNode node)
    {
        for (int i = 0; i < Connections.Count; i++)
        {
            var connection = Connections[i];
            if (connection.NodeA == node || connection.NodeB == node)
            {
                Connections.SafeRemoveAt(ref i);
            }
        }
    }

    public void RemoveConnections(InSplineNode nodeA, InSplineNode nodeB)
    {
        for (int i = 0; i < Connections.Count; i++)
        {
            var connection = Connections[i];
            if (connection.NodeA == nodeA && connection.NodeB == nodeB || connection.NodeA == nodeB && connection.NodeB == nodeA)
            {
                Connections.SafeRemoveAt(ref i);
            }
        }
    }
}
