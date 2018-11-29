using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

    public Transform mapPlane;
    public Transform miniMapCamera;

    void Update()
    {
        miniMapCamera.position = new Vector3(AIControl.CurrentVehicle.transform.position.x, mapPlane.position.y+25, AIControl.CurrentVehicle.transform.position.z);
        miniMapCamera.eulerAngles = new Vector3(90, AIControl.CurrentVehicle.transform.eulerAngles.y, 0);
    }
}
