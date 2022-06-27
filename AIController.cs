using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{   
    public bool isBraking;
    private float steeringAngle;
    private float brakeForce;

    public float motorPower = 10000f;
    public float maxSteeringAngle = 35f;
    public float brakePower = 120000f;

    public Rigidbody aiCarRB;
    public Transform playerCar;
    public Transform aiCar;
    public Transform wholeAiCar;
    public float velocity;
    private float velocityNeededToBrake;
    public float vroomFactor;
    public float turnFactor;
    public float distance;
    public float relativeAngle;
    public bool carFlipped;
    public float aiCarXRot;
    public float aiCarZRot;
    private float aiCarFacing;
    private float eulerAngleAiCar;
    private float globalAngle;
    private float xDifference;
    private float zDifference;
    private float globalAngleRad;
    private float globalAngleInv;

    public WheelCollider FRWheelCollider;
    public WheelCollider FLWheelCollider;
    public WheelCollider BRWheelCollider;
    public WheelCollider BLWheelCollider;
    public Transform FRWheelTransform;
    public Transform FLWheelTransform;
    public Transform BRWheelTransform;
    public Transform BLWheelTransform;

    private void FixedUpdate()
    {
        GetInput();
        calculateAction();
        handleMotor();
        Steering();
        UpdateWheels();
    }
    private void GetInput()
    {
        // calculating angle between centre of AI Car and Player Car
        xDifference = aiCar.transform.position.x - playerCar.transform.position.x;
        zDifference = aiCar.transform.position.z - playerCar.transform.position.z;
        globalAngleRad = Mathf.Atan(zDifference / xDifference);
        if (xDifference >= 0){
            globalAngleInv = globalAngleRad * Mathf.Rad2Deg + 90;
        }
        if (xDifference < 0){
            globalAngleInv = globalAngleRad * Mathf.Rad2Deg + 270;
        }
        globalAngle = 360 - globalAngleInv;

        // finding distance between AI Car and Player Car
        distance = Mathf.Sqrt(xDifference * xDifference + zDifference * zDifference);

        // converting quaternion values of AI car to Euler angles and outputting the angle the car is facing
        float X = aiCar.transform.rotation.x;
        float Y = aiCar.transform.rotation.y;
        float Z = aiCar.transform.rotation.z;
        float W = aiCar.transform.rotation.w;
        float aiCarFacingRad = Mathf.Atan2(2*Y*W - 2*X*Z, 1 - 2*Y*Y - 2*Z*Z); // converting quaternion to euler angle
        eulerAngleAiCar = Mathf.Rad2Deg * aiCarFacingRad;
        if (aiCarFacing >= 0){
            aiCarFacing = eulerAngleAiCar;
        }
        if (aiCarFacing < 0){
            aiCarFacing = 360 + eulerAngleAiCar;
        }

        // finding the angle between where the AI Car is facing and the Player Car so
        // that we know the relaitve angle between where the AI car is facing and where the player car is
        if (globalAngle > aiCarFacing){
            relativeAngle = globalAngle - aiCarFacing;
        }
        if (aiCarFacing > globalAngle){
            float angleDifference = aiCarFacing - globalAngle;
            relativeAngle = 360 - angleDifference;
        }

        // finding if the car is flipped or on its side
        float aiCarXRotRad = Mathf.Atan2(2*X*W-2*Y*Z , 1 - 2*X*X - 2*Z*Z);
        aiCarXRot = Mathf.Rad2Deg * aiCarXRotRad + 90;
        float aiCarZRotRad = Mathf.Asin(2*X*Y + 2*Z*W);
        aiCarZRot = Mathf.Rad2Deg * aiCarZRotRad;
    }
    private void calculateAction()
    {
        // finding car velocity
        velocity = transform.InverseTransformDirection(aiCarRB.velocity).z;
        // finding whether the car should turn left or right and setting the speed according to the angle
        if (35 > relativeAngle & 1 < relativeAngle){
            turnFactor = relativeAngle / 70;
            vroomFactor = 1 - turnFactor;
        }
        if (325 <= relativeAngle & 359 > relativeAngle){
            float leftRelativeAngle = 360 - relativeAngle;
            turnFactor = -leftRelativeAngle / 70;
            vroomFactor = 1 + turnFactor;
        }
        if (relativeAngle >= 35 & relativeAngle <= 180){
            turnFactor = 1;
            vroomFactor = 0.3f;
        }
        if (relativeAngle <= 325 & relativeAngle > 180){
            turnFactor = -1;
            vroomFactor = 0.3f;
        }
        if (relativeAngle <= 1 & relativeAngle >= 0 || relativeAngle >= 359 & relativeAngle <= 360){
            turnFactor = 0;
            vroomFactor = 1;
        }
        // braking if turn angle is too high so it doesn't flip over
        velocityNeededToBrake = Mathf.Abs(1 / ((float)0.6 * turnFactor)) + 12;
        if (velocity > velocityNeededToBrake){
            StartCoroutine(Braking());
        }
        // resetting the car's position to an upright position if it is flipped over
        if (aiCarXRot > 175 & aiCarXRot < 185){
            carFlipped = true;
            StartCoroutine(Flipped());
        }
        else {
            carFlipped = false;
        }
    }
    IEnumerator Braking(){
        isBraking = true;
        yield return new WaitForSeconds((float)0.8);
        isBraking = false;
    }
    IEnumerator Flipped(){
        yield return new WaitForSeconds(4);
        if (carFlipped == true & aiCarZRot > 65){  // on its left side
            wholeAiCar.transform.position = Vector3.Lerp(wholeAiCar.transform.position, wholeAiCar.TransformPoint(1, 0, 0), 1);
            wholeAiCar.transform.rotation = Quaternion.Lerp(wholeAiCar.transform.rotation, new Quaternion(0, 0, 0, 1), 1);
            yield break;
        }
        if (carFlipped == true & aiCarZRot < -65){  // on its right side
            wholeAiCar.transform.position = Vector3.Lerp(wholeAiCar.transform.position, wholeAiCar.TransformPoint(1, 0, 0), 1);
            wholeAiCar.transform.rotation = Quaternion.Lerp(wholeAiCar.transform.rotation, new Quaternion(0, 0, 0, 1), 1);
            yield break;
        }
        if (carFlipped == true & (aiCarXRot > 175 & aiCarXRot < 185)){ // upside down
            wholeAiCar.transform.position = Vector3.Lerp(wholeAiCar.transform.position, wholeAiCar.TransformPoint(0, -1, 0), 1);
            wholeAiCar.transform.rotation = Quaternion.Lerp(wholeAiCar.transform.rotation, new Quaternion(0, 0, 0, 1), 1);
            yield break;
        }
    }
    private void handleMotor()
    {
        BRWheelCollider.motorTorque = vroomFactor * motorPower;
        BLWheelCollider.motorTorque = vroomFactor * motorPower;

        brakeForce = isBraking ? brakePower : 0f;
        FRWheelCollider.brakeTorque = brakeForce;
        FLWheelCollider.brakeTorque = brakeForce;
        BRWheelCollider.brakeTorque = brakeForce;
        BLWheelCollider.brakeTorque = brakeForce;
    }
    private void Steering()
    {
        steeringAngle = maxSteeringAngle * turnFactor;
        FRWheelCollider.steerAngle = steeringAngle;
        FLWheelCollider.steerAngle = steeringAngle;
    }
    private void UpdateWheels()
    {
        UpdateSingleWheel(FLWheelCollider, FLWheelTransform);
        UpdateSingleWheel(FRWheelCollider, FRWheelTransform);
        UpdateSingleWheel(BLWheelCollider, BLWheelTransform);
        UpdateSingleWheel(BRWheelCollider, BRWheelTransform);
    }
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }
}
