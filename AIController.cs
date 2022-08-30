using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{   
    public bool doingSomething;
    public bool isBraking;
    private float steeringAngle;
    private float brakeForce;

    public float motorPower = 10000f;
    public float maxSteeringAngle = 35f;
    public float brakePower = 120000f;

    // transforms and rigidbodies
    public Rigidbody aiCarRB;
    public Transform playerCar;
    public Transform aiCar;
    public Transform wholeAiCar;
    // velocity and braking
    public float velocity;
    private float velocityNeededToBrake;
    // driving outputs
    public float vroomFactor;
    public float turnFactor;
    // calculating for outputs
    public float distance;
    public float relativeAngle;
    private float aiCarFacing;
    private float eulerAngleAiCar;
    private float globalAngle;
    private float xDifference;
    private float zDifference;
    private float globalAngleRad;
    private float globalAngleInv;
    // flipping calculations
    private bool carFlipped;
    public float aiCarXRot;
    public float aiCarZRot;
    // collision detection and prevention
    public Collider ground;
    public Collider playerCarCollider;
    public float currentDistance;
    public float timeToCollision;
    public float objectAngle;
    private float objectPlane;
    public Collider objectCollider;
    public bool drivable;
    private float leftDistance;
    private float rightDistance;
    private float leftCornerDistance;
    private float rightCornerDistance;
    private bool obstacleCheck;
    private bool leftCornerHit;
    private bool rightCornerHit;
    private bool finishedReversing;
    private bool playerCarToRight;

    // stuff for wheels
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

        // seeing how far an object is from the car
        RaycastHit frontRaycast;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out frontRaycast, Mathf.Infinity)){
            currentDistance = frontRaycast.distance;
        }
        RaycastHit leftRaycast;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) + new Vector3(0, 1, 0), out leftRaycast, Mathf.Infinity)){
            leftDistance = leftRaycast.distance;
        }
        RaycastHit rightRaycast;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) + new Vector3(0, -1, 0), out rightRaycast, Mathf.Infinity)){
            rightDistance = rightRaycast.distance;
        }
        RaycastHit leftCornerRaycast;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) + new Vector3(0, 20, 0), out leftCornerRaycast, Mathf.Infinity)){
            leftCornerDistance = leftCornerRaycast.distance;
            leftCornerHit = true;
        }
        RaycastHit rightCornerRaycast;
        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward) + new Vector3(0, -20, 0), out rightCornerRaycast, Mathf.Infinity)){
            rightCornerDistance = rightCornerRaycast.distance;
            rightCornerHit = true;
        }
        // calculating how long it will take to hit the object at the current velocity
        timeToCollision = currentDistance / velocity;
        // finding angle of detected surface and whether the car can drive up it
        float normalX = frontRaycast.normal.x;
        float normalY = frontRaycast.normal.y;
        float normalZ = frontRaycast.normal.z;
        // finding length of normal in the combined direction of x and z (refer to photo on trello for better explanation)
        objectPlane = Mathf.Sqrt(normalX*normalX + normalZ*normalZ);
        // finding angle of object relative to flat x and z plane
        float normalAngleRad = Mathf.Atan(normalY / objectPlane);
        float normalAngle = Mathf.Rad2Deg * normalAngleRad;
        objectAngle = 90 - normalAngle;
        if (objectAngle > 90){
            objectAngle = 180 - objectAngle;
        }
        // if angle over 34 it is not drivable
        if (objectAngle > 34 || objectAngle < -34){
            drivable = false;
        }
        else {
            drivable = true;
        }
        // finding what object is in front of the car
        objectCollider = frontRaycast.collider;
    }
    private void calculateAction()
    {
        // finding car velocity
        velocity = transform.InverseTransformDirection(aiCarRB.velocity).z;
        // finding whether the car should turn left or right and setting the speed according to the angle
        if (timeToCollision < 3 && timeToCollision > 0 && drivable == false && objectCollider != ground && objectCollider != playerCarCollider){
            if (leftDistance > rightDistance){
                doingSomething = true;
                turnFactor = -1;
            }
            else {
                doingSomething = true;
                turnFactor = 1;
            }
            // checking if the obstacle is still there even though the central raycast cant see it
            StartCoroutine(checkForObstacle());
        }
        else {
            doingSomething = false;
        }
        // if the obstacle is still there turn more
        if (obstacleCheck == true && (leftCornerHit == true || rightCornerHit == true)){
            if (leftCornerDistance > rightCornerDistance){
                doingSomething = true;
                turnFactor = -1;
            }
            else {
                doingSomething = true;
                turnFactor = 1;
            }
        }
        // if stuck on an obstacle, reverse and keep going
        if (((timeToCollision < 0.1 && timeToCollision >= 0)  || timeToCollision > 500) && (velocity < 0.1 && velocity > 0) && objectCollider != playerCarCollider){
        // for some reason if pressed up against an object the raycast goes through the obstacle and the
        // time to collision is really high so instead of fixing the problem doing the easier, worse thing
            StartCoroutine(backUp());
            doingSomething = true;
        }
        if (velocity < 0 && finishedReversing == false && objectCollider != playerCarCollider){
            StartCoroutine(reverseSomeMore());
            doingSomething = true;
        }
        // if no obstacle, drive normally (towards player car)
        if (doingSomething == false){
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
            if ((aiCarXRot > 150 & aiCarXRot < 210) || aiCarZRot > 55 || aiCarZRot < -55){
                carFlipped = true;
                StartCoroutine(Flipped());
            }
            else {
                carFlipped = false;
            }
            
        }
    }
    // checking if the obstacle is still there even if not detected by the Front Raycast
    IEnumerator checkForObstacle(){
        obstacleCheck = true;
        yield return new WaitForSeconds(2);
        obstacleCheck = false;
    }
    IEnumerator backUp(){
        yield return new WaitForSeconds ((float)0.2);
        if ((timeToCollision < 0.1 || timeToCollision > 500) && velocity < 0.1){
            doingSomething = true;
            vroomFactor = -1;
            if (relativeAngle > 0 && relativeAngle < 180){
                playerCarToRight = true;
            }
            else {
                playerCarToRight = false;
            }
        }
    }
    IEnumerator reverseSomeMore(){
        doingSomething = true;
        vroomFactor = -1;
        if (playerCarToRight == true){
            turnFactor = -1;
        }
        else {
            turnFactor = 1;
        }
        yield return new WaitForSeconds((float)1);
        doingSomething = false;
        finishedReversing = true;
        yield return new WaitForSeconds(5);
        finishedReversing = false;
    }
    IEnumerator Braking(){
        isBraking = true;
        yield return new WaitForSeconds((float)0.8);
        isBraking = false;
    }
    IEnumerator Flipped(){
        yield return new WaitForSeconds(4);
        if (carFlipped == true & aiCarZRot > 55){  // on its left side
            wholeAiCar.transform.position = Vector3.Lerp(wholeAiCar.transform.position, wholeAiCar.TransformPoint(1, 0, 0), 1);
            wholeAiCar.transform.rotation = Quaternion.Lerp(wholeAiCar.transform.rotation, new Quaternion(0, 0, 0, 1), 1);
            yield break;
        }
        if (carFlipped == true & aiCarZRot < -55){  // on its right side
            wholeAiCar.transform.position = Vector3.Lerp(wholeAiCar.transform.position, wholeAiCar.TransformPoint(1, 0, 0), 1);
            wholeAiCar.transform.rotation = Quaternion.Lerp(wholeAiCar.transform.rotation, new Quaternion(0, 0, 0, 1), 1);
            yield break;
        }
        if (carFlipped == true & (aiCarXRot > 150 & aiCarXRot < 210)){ // upside down
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
