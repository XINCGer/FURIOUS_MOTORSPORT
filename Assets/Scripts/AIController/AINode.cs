using UnityEngine;
using System.Collections;

public class AINode : MonoBehaviour
{


    public Transform nextNode;
    public Transform previousNode;

    public NodeSetting nodeSetting;

    [HideInInspector]
    public Color GizmosColor = new Color(1, 1, 1);

    [System.Serializable]
    public class NodeSetting
    {
        public bool brakeing;
    }

    void OnDrawGizmos()
    {
        transform.LookAt(nextNode);

        if (nodeSetting.brakeing)
            Gizmos.color = Color.red;
        else
            Gizmos.color = GizmosColor;

        Gizmos.DrawSphere(transform.position, 1.0f);
    }
}
