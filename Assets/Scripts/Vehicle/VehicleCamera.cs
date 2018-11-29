using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
 using UnityStandardAssets.ImageEffects;

public class VehicleCamera : MonoBehaviour
{
    public static VehicleCamera manage;


    public float smooth = 0.3f;

    public float distance = 5.0f;
    public float height = 1.0f;
    public float angle = 20;

    public LayerMask lineOfSightMask = 0;

    [HideInInspector]
    public Transform target;
    [HideInInspector]
    public List<Transform> cameraSwitchView;

    private bool farCameraView = false;

    private Vector3 farCameraPosition;
    private Vector3 velocity = Vector3.zero;


    private float Xsmooth;
    private float farDistance = 0.0f;
    private float zAngleAmount = 0.0f;
    private float timeScale = 0.0f;
    private float currentDistance;

    private int Switch = -1;
   

    void Awake()
    {
        manage = this;
        farCameraPosition = transform.position;
    }


    void Start()
    {
        farCameraView = true;
        farCameraPosition = (AIControl.manage.firstAINode.GetComponent<AINode>().nextNode
            .GetComponent<AINode>().nextNode
            .GetComponent<AINode>().nextNode
            .GetComponent<AINode>().nextNode
            .GetComponent<AINode>().nextNode
            .GetComponent<AINode>().nextNode.position)
            + new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(5.0f, 10.0f), Random.Range(-5.0f, 5.0f));
    }



    void LateUpdate()
    {

        if (GameUI.manage.gameFinished)
            Switch = 4;

        farDistance = Vector3.Distance(farCameraPosition, target.position);
        if (farDistance < 100.0f && farCameraView) farCameraView = false;

        transform.GetComponent<Blur>().enabled = GameUI.manage.gamePaused ? true : false;

        // add MotionBlur effect to camera
        if (AIControl.CurrentVehicle.shifting || GameUI.manage.driftAmount > 25)
            transform.GetComponent<MotionBlur>().blurAmount = Mathf.Lerp(transform.GetComponent<MotionBlur>().blurAmount, 0.5f, Time.deltaTime * 5);
        else
           transform.GetComponent<MotionBlur>().blurAmount = Mathf.Lerp(transform.GetComponent<MotionBlur>().blurAmount, 0.0f, Time.deltaTime);



        if (Switch == -1)
        {

            RenderSettings.flareStrength = 0.3f;

            GetComponent<Camera>().fieldOfView = Mathf.Clamp(AIControl.CurrentVehicle.speed / 10.0f + 60.0f, 60, 90.0f);

            currentDistance = distance;
            float yAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y,
            target.eulerAngles.y, ref velocity.y, smooth);

            float xAngle = Mathf.SmoothDampAngle(transform.eulerAngles.x,
            target.eulerAngles.x + (angle), ref velocity.x, smooth);

            // Look at the target
            transform.eulerAngles = new Vector3(xAngle, yAngle, AccelerationAngle());

            Xsmooth = Mathf.Lerp(Xsmooth, velocity.y, Time.deltaTime * 10.0f);
            var direction = transform.rotation * -new Vector3(-Xsmooth / 300.0f, 0, 1);
            var targetDistance = AdjustLineOfSight(target.position + new Vector3(0, height, 0), direction);

            transform.position = target.position + new Vector3(0, height, 0) + direction * targetDistance;

        }
        else if (Switch < AIControl.CurrentVehicle.cameraView.cameraSwitchView.Count)
        {

            RenderSettings.flareStrength = 0.3f;
            GetComponent<Camera>().fieldOfView = 60;
            transform.position = cameraSwitchView[Switch].position;
            transform.rotation = Quaternion.Lerp(transform.rotation, cameraSwitchView[Switch].rotation, Time.deltaTime * 5.0f);

        }
        else {

            if (farDistance > 120.0f && !farCameraView)
            { 
                farCameraPosition = (AIControl.CurrentVehicle.AIVehicle.nextNode
                    .GetComponent<AINode>().nextNode
                    .GetComponent<AINode>().nextNode
                    .GetComponent<AINode>().nextNode
                    .GetComponent<AINode>().nextNode
                    .GetComponent<AINode>().nextNode
                    .GetComponent<AINode>().nextNode.position)
                    + new Vector3(Random.Range(-5.0f, 5.0f), Random.Range(10.0f, 15.0f), Random.Range(-5.0f, 5.0f));

                farCameraView = true;

            }
            

            RenderSettings.flareStrength = 0.0f;

            GetComponent<Camera>().fieldOfView = Mathf.Clamp(50.0f - (farDistance / 2.0f), 10.0f, 120.0f);

            var newRotation = Quaternion.LookRotation(target.position - transform.position);

            transform.position = farCameraPosition;
            transform.rotation = Quaternion.Slerp(transform.rotation, newRotation, Time.deltaTime * 15);
        }

    }



    public void CameraSwitch()
    {
        Switch++;
        if (Switch > AIControl.CurrentVehicle.cameraView.cameraSwitchView.Count) { Switch = -1; }
    }

    ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

    private float AccelerationAngle()
    {
        zAngleAmount = Mathf.Clamp(zAngleAmount, -45.0f, 45.0f);
        zAngleAmount = Mathf.Lerp(zAngleAmount, Input.acceleration.x * -70.0f, Time.deltaTime * 2.0f);
        return zAngleAmount;
    }


    float AdjustLineOfSight(Vector3 target, Vector3 direction)
    {
        RaycastHit hit;

        if (Physics.Raycast(target, direction, out hit, currentDistance, lineOfSightMask.value))
            return hit.distance;
        else
            return currentDistance;
    }


}


