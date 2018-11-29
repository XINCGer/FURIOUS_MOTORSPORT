using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class WheelSkidmarks : MonoBehaviour {

#pragma strict

//@script RequireComponent(WheelCollider)//We need a wheel collider

public GameObject skidCaller;//The parent oject having a rigidbody attached to it.
public float startSlipValue = 0.2f;
private Skidmarks skidmarks = null;//To hold the skidmarks object
private int lastSkidmark = -1;//To hold last skidmarks data
private WheelCollider wheel_col;//To hold self wheel collider


void Start()
{
    //Get the Wheel Collider attached to self
    skidCaller = transform.root.gameObject;
    wheel_col = GetComponent<WheelCollider>();
    //find object "Skidmarks" from the game
    if (FindObjectOfType(typeof(Skidmarks)))
    {
        skidmarks = FindObjectOfType(typeof(Skidmarks)) as Skidmarks;
    }
    else
        Debug.Log("No skidmarks object found. Skidmarks will not be drawn");
}

void FixedUpdate () //This has to be in fixed update or it wont get time to make skidmesh fully.
	{
	WheelHit GroundHit; //variable to store hit data
	wheel_col.GetGroundHit(out GroundHit );//store hit data into GroundHit
    var wheelSlipAmount = Mathf.Abs(GroundHit.sidewaysSlip);

    if (wheelSlipAmount > startSlipValue) //if sideways slip is more than desired value
	{
	/*Calculate skid point:
	Since the body moves very fast, the skidmarks would appear away from the wheels because by the time the
	skidmarks are made the body would have moved forward. So we multiply the rigidbody's velocity vector x 2 
	to get the correct position
	*/

	Vector3 skidPoint  = GroundHit.point + 2*(skidCaller.GetComponent<Rigidbody>().velocity) * Time.deltaTime;
	
	//Add skidmark at the point using AddSkidMark function of the Skidmarks object
	//Syntax: AddSkidMark(Point, Normal, Intensity(max value 1), Last Skidmark index);
	lastSkidmark = skidmarks.AddSkidMark(skidPoint, GroundHit.normal, wheelSlipAmount/2.0f, lastSkidmark);	
	}
	else
	{
	//stop making skidmarks
	lastSkidmark = -1;
	}
			
	}


}
