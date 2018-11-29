using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobileControl : MonoBehaviour
{
    ////////////////////////////////////////////// TouchMode (Control) ////////////////////////////////////////////////////////////////////

    public void CarAccelForward(float amount)
    {
        AIControl.CurrentVehicle.accelFwd = amount;
    }
    public void CarAccelBack(float amount)
    {
        AIControl.CurrentVehicle.accelBack = amount;
    }
    public void CarSteer(float amount)
    {
        AIControl.CurrentVehicle.steerAmount = amount;
    }
    public void CarHandBrake(bool HBrakeing)
    {
        AIControl.CurrentVehicle.brake = HBrakeing;
    }
    public void CarShift(bool Shifting)
    {
        AIControl.CurrentVehicle.shift = Shifting;
    }

}
