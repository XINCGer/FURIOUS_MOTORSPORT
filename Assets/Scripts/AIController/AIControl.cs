using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VehicleMode { Player = 0, AICar = 1 }
public enum ControlMode { Simple = 1, Mobile = 2 }

public class AIControl : MonoBehaviour
{

    public static AIControl manage;
    public static VehicleControl CurrentVehicle;

    public ControlMode controlMode = ControlMode.Simple;

    public Transform firstAINode;
    public Transform startPoint;

    public GameObject[] CarsPrefabs;

    void Awake()
    {
        manage = this;
    }

    void Start()
    {

        GameObject InstantiatedCar = Instantiate(CarsPrefabs[PlayerPrefs.GetInt("CurrentVehicle")], startPoint.position, startPoint.rotation) as GameObject;

        InstantiatedCar.GetComponent<AIVehicle>().nextNode = firstAINode;

        CurrentVehicle = InstantiatedCar.GetComponent<VehicleControl>();

        VehicleCamera.manage.target = InstantiatedCar.transform;
        VehicleCamera.manage.cameraSwitchView = CurrentVehicle.cameraView.cameraSwitchView;

        VehicleCamera.manage.distance = CurrentVehicle.cameraView.distance;
        VehicleCamera.manage.height = CurrentVehicle.cameraView.height;
        VehicleCamera.manage.angle = CurrentVehicle.cameraView.angle;
    }

}
