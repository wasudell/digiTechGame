using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{   
    public bool avoiding; // whether the car is avoiding an obstacle or not
    public bool reversing; // whether the car is reversing or not
    public bool isBraking; // whether the car is braking or not
    private float steeringAngle; // how much the metaphorical steering wheel is turned
    private float brakeForce; // force of the brakes

    public float motorPower; // power of the motor
    public float maxSteeringAngle; // maximum angle the car can steer
    public float brakePower; // force of the brakes but different
    public Vector3 comAdjust; // adjustment to the centre of mass

    // transforms, rigidbodies and audio
    public Rigidbody aiCarRB; // Rigidbody of the ai Car
    public Transform playerCar; // Transform of the player car body (position and stuff)
    public Transform aiCar; // Transform of the aiCar body (position and stuff)
    public Transform wholeAiCar; // Transform of the whole Ai Car group
    private AudioSource sound; // thing on the car that produces the sound (speaker) which is picked up by
    // an audio listener (microphone) on the camera
    private AudioSource collisionSound; // getting audio source on body to produce the collision sound
    public UIScore score; // score script in order to find when the pause menu is activated so sound can be paused\

    // velocity and braking
    public float health = 20; // health of car
    private float impulse; // change in momentum of a collision
    public float velocity; // velocity of car
    private float velocityNeededToBrake; // setting a max velocity to turn safely
    // driving outputs
    public float vroomFactor; // accelerator
    public float turnFactor; // steering wheel

    // calculating for outputs
    public float distance; // distance to van
    public float relativeAngle; // angle to van
    private float aiCarFacing; // direction car is facing relative locally
    private float aiCarYRot; // direction car is facing globally relative
    private float globalAngle; // angle between car and van globally
    private float xDifference; // difference of car position and van position on the x axis
    private float zDifference; // difference of car position and van position on the z axis
    private float globalAngleRad; // globalAngle but in radians
    private float globalAngleInv; // global angle that can be minused from 360 depending on van position
    public float maxSpeed; // maximum speed of car
    // stuff for figuring out if the car is flipped
    private bool carFlipped; // whether or not the car has been flipped so code doesnt loop
    private float aiCarZRot; // rotation of the car on the z axis

    // bunch of raycast values for collision detection and prevention
    public bool frontHit; // whether or not the front raycast hit something
    public float frontDistance; // the distance that the thing hit is from the car
    private float leftDistance; // distance that an obstacle is to the left at an angle
    private float rightDistance; // distance that an obstacle is to the left at an angle
    public bool leftCornerHit; // whether or not the raycast from the left corner hit anything
    public bool rightCornerHit; // whether or not the raycast from the right corner hit anything
    private float leftCornerDistance; // distance that an obstacle is from the left corner of the car
    private float rightCornerDistance; // distance that an obstacle is from the right corner of the car

    // wheel visuals as well as wheel colliders
    public WheelCollider FRWheelCollider; // collider for front right wheel , front left wheel and so on
    public WheelCollider FLWheelCollider;
    public WheelCollider BRWheelCollider;
    public WheelCollider BLWheelCollider;
    public Transform FRWheelTransform; // visual for front right wheel, front left wheel and so on
    public Transform FLWheelTransform;
    public Transform BRWheelTransform;
    public Transform BLWheelTransform;

    // explosion stuff
    public ParticleSystem explosion; // explosion particle system
    private bool deathCheck; // check to see if car is dead so code doesn't run again
    public GameObject bodyGameObject; // objects to destroy
    public GameObject wheelGameObjects;

    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass = comAdjust; // adjusting the centre of mass of the car so that it doesn't flip over as easily
        // getting the audio source attached to the car
        sound = GetComponent<AudioSource>();
        // getting the audio souce on the body object, which produces the collision sound
        // this simplifies things as compared to adding two sources on the car
        collisionSound = bodyGameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        GetInput(); // getting input of van position, car rotation, and obstacle distance
        calculateAction(); // calculating actions car should make based on inputs
        handleMotor(); // making the car move based on actions
        Steering(); // making th car steer based on actions
        UpdateWheels(); // visually updating the wheels
    }
    private void GetInput()
    {
        // calculating angle between centre of AI Car and Player Car
        xDifference = aiCar.transform.position.x - playerCar.transform.position.x;
        zDifference = aiCar.transform.position.z - playerCar.transform.position.z;
        globalAngleRad = Mathf.Atan(zDifference / xDifference); // trigenometry
        // changing the angle correctly depending on the position of the van and car (traingle stuff)
        if (xDifference >= 0){
            globalAngleInv = globalAngleRad * Mathf.Rad2Deg + 90;
        }
        if (xDifference < 0){
            globalAngleInv = globalAngleRad * Mathf.Rad2Deg + 270;
        }
        globalAngle = 360 - globalAngleInv;

        // finding distance between AI Car and Player Car
        distance = Mathf.Sqrt(xDifference * xDifference + zDifference * zDifference); //pythagorean theorm

        // determining the angle of the player to the car, and also making it so the value always stays positive
        aiCarYRot = transform.rotation.eulerAngles.y;
        if (aiCarFacing >= 0){
            aiCarFacing = aiCarYRot;
        }
        if (aiCarFacing < 0){
            aiCarFacing = 360 + aiCarYRot;
        }

        // finding the angle between where the AI Car is facing and the Player Car so
        // that we know the relaitve angle between where the AI car is facing and where the player car is.
        // Changing calculation method pased on positioning (more traingle stuff)
        if (globalAngle > aiCarFacing){
            relativeAngle = globalAngle - aiCarFacing;
        }
        if (aiCarFacing > globalAngle){
            float angleDifference = aiCarFacing - globalAngle;
            relativeAngle = 360 - angleDifference;
        }

        // finding the Z angle of the car to see whether it is flipped or not
        aiCarZRot = transform.rotation.eulerAngles.z;

        // seeing how far an obstacle is from the car
        RaycastHit frontRaycast; // front raycast for directly in front of car, distance to a function that increases then levels out as velocity increases
        if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.forward), out frontRaycast, Mathf.Abs(Mathf.Pow(velocity, 0.7f) * 4))){
            frontHit = true;
            Debug.DrawRay(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(Vector3.forward) * frontRaycast.distance, Color.red);
        }
        else {
            frontHit = false;
        }
        RaycastHit leftCornerRaycast; // left corner for if the front raycast can't see an obstacle, 1/4 velocity distance as little adjustment is needed to avoid here
        if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(new Vector3(0.5f, 0, 1)), out leftCornerRaycast, Mathf.Abs(velocity / 4))){
            leftCornerDistance = leftCornerRaycast.distance;
            leftCornerHit = true;
            Debug.DrawRay(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(new Vector3(0.5f, 0, 1)) * leftCornerRaycast.distance, Color.red);
        }
        else {
            leftCornerHit = false;
        }
        RaycastHit rightCornerRaycast; // same as left corner but now for the right corner
        if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(new Vector3(-0.5f, 0, 1)), out rightCornerRaycast, Mathf.Abs(velocity / 4))){
            rightCornerDistance = rightCornerRaycast.distance;
            rightCornerHit = true;
            Debug.DrawRay(transform.position + new Vector3(0, 0.2f, 0), transform.TransformDirection(new Vector3(-0.5f, 0, 1)) * leftCornerRaycast.distance, Color.red);
        }
        else {
            rightCornerHit = false;
        }
        // exploding the car if health is equal to or less than 0
        if (health <= 0 && deathCheck == false){
            Explode();
            // making sure the explode script only runs once
            deathCheck = true;
            // turning off the engine sound upon death
            sound.mute = !sound.mute;
        }

        // setting a max speed based on distance to the player
        if (distance < 6){ // basically right next to the car, so once it collides it backs off
            maxSpeed = 15;
        }
        else {
            maxSpeed = 35;
        }

        // increasing the sound's (engine's) pitch as the van moves faster, as that makes it sounds like the engine is working harder and making it move faster
        sound.pitch = 1 + ((velocity / 100) * vroomFactor); // multiplied by vertical input so that if the accelerator key isn't being pressed, it doesn't sound like the engine is working hard
        if (velocity >= 30){
            sound.pitch = 1 + 30/100; // vertical input is set to 0 above 30 to set a max speed so this ensures that the engine doesn't sound like it's turning off
        }
    }
    private void calculateAction()
    {
        // finding car velocity
        velocity = transform.InverseTransformDirection(aiCarRB.velocity).z;
        // finding whether the car should turn left or right and setting the speed according to the angle
        if (frontHit == true && reversing != true){
            RaycastHit leftRaycast; // sending a raycast slightly to the left if an obstacle is detected by directly in front ray
            if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), (transform.TransformDirection(new Vector3(-0.1f, 0, 1))), out leftRaycast, Mathf.Infinity)){
                leftDistance = leftRaycast.distance;
                Debug.DrawRay(transform.position + new Vector3(0, 0.2f, 0), (transform.TransformDirection(new Vector3(-0.1f, 0, 1))) * leftRaycast.distance, Color.red);
            }
            RaycastHit rightRaycast; // same as left raycast but for right
            if (Physics.Raycast(transform.position + new Vector3(0, 0.2f, 0), (transform.TransformDirection(new Vector3(0.1f, 0, 1))), out rightRaycast, Mathf.Infinity)){
                rightDistance = rightRaycast.distance;
                Debug.DrawRay(transform.position + new Vector3(0, 0.2f, 0), (transform.TransformDirection(new Vector3(0.1f, 0, 1))) * rightRaycast.distance, Color.red);
            }  
            if (leftDistance > rightDistance){ // turning left if greater distance from left raycast
                avoiding = true;
                vroomFactor = 0.5f; // this ensures that the car doesn't slow down too much when turning
                turnFactor = 1;
            }
            else { // vice versa
                avoiding = true;
                vroomFactor = 0.5f;
                turnFactor = -1;
            }
        }
        else { // setting an avoiding variable so that the normal following code doesn't run and change values
            avoiding = false;
        }
        // turning the car when something out of the corner is detected (out of view of front raycast)
        if ((leftCornerHit == true || rightCornerHit == true) && reversing != true){ 
            avoiding = true;
            if (leftCornerHit == true){
                turnFactor = -1;
            }
            else {
                turnFactor = 1;
            }
        }
        else {
            avoiding = false;
        }
        // reversing car if it is stuck on an obstacle
        if ((velocity < 0.1 && velocity > 0) && vroomFactor > 0){
            StartCoroutine(backUp());
            reversing = true; // reversing variable so that avoiding code or normal following code doesn't run and change values
        }
        // if no obstacle, drive normally (towards player car)
        if (avoiding == false && reversing == false){
            // turn right slightly with speed tied to angle turning at
            if (35 > relativeAngle & 1 < relativeAngle){
                turnFactor = relativeAngle / 70;
                vroomFactor = 1 - turnFactor;
            }
            // turn left slightly with speed tied to angle turning at
            if (325 <= relativeAngle & 359 > relativeAngle){
                float leftRelativeAngle = 360 - relativeAngle;
                turnFactor = -leftRelativeAngle / 70;
                vroomFactor = 1 + turnFactor;
            }
            // turn right a lot as angle to van is greater than max steering angle, full speed
            if (relativeAngle >= 35 & relativeAngle <= 180){
                turnFactor = 1;
                vroomFactor = 1;
            }
            // turn left a lot as angle to van is greater than max steering angle, full speed
            if (relativeAngle <= 325 & relativeAngle > 180){
                turnFactor = -1;
                vroomFactor = 1;
            }
            // drive full speed when player is directly in front
            if (relativeAngle <= 1 & relativeAngle >= 0 || relativeAngle >= 359 & relativeAngle <= 360){
                turnFactor = 0;
                vroomFactor = 1;
            }
            // braking if turn angle is too high so car doesn't flip over
            velocityNeededToBrake = (1 / 10 * turnFactor) + velocity;
            if (velocity > velocityNeededToBrake){
                isBraking = true;
            }
            else {
                isBraking = false;
            }
            // resetting the car's position to an upright position if it is flipped over
            if ((aiCarZRot > 70 || aiCarZRot < 290) && carFlipped == false){
                carFlipped = true;
                StartCoroutine(Flipped());
            }
            // limiting car's speed
            if (velocity > maxSpeed){
                vroomFactor = 0;
            }
            
        }
    }
    IEnumerator backUp(){
        yield return new WaitForSeconds (0.2f);
        // putting car into reverse and turning in correct direction depening on where van is
        if (velocity < 0.1){
            vroomFactor = -1;
            if (relativeAngle > 0 && relativeAngle < 180){
                turnFactor = -1;
            }
            else {
                turnFactor = 1;
            }
        }
        yield return new WaitForSeconds(1);
        reversing = false; // setting variable to false so other code can run
    }

    IEnumerator Flipped(){
        // each section resets car's rotation if flipped (direction facing preserved), moved upwards so car doesn't intersect with ground
        yield return new WaitForSeconds(4);
        if (carFlipped == true && (aiCarZRot < 135 || aiCarZRot > 225)){  // on its left or right side
            wholeAiCar.transform.position = Vector3.Lerp(wholeAiCar.transform.position, wholeAiCar.TransformPoint(1, 0, 0), 1);
            wholeAiCar.transform.rotation = Quaternion.Lerp(wholeAiCar.transform.rotation, Quaternion.Euler(0, aiCarYRot, 0), 1);
            yield break;
        }
        if (carFlipped == true & (aiCarZRot > 135 & aiCarZRot < 225)){ // upside down
            wholeAiCar.transform.position = Vector3.Lerp(wholeAiCar.transform.position, wholeAiCar.TransformPoint(0, -1, 0), 1);
            wholeAiCar.transform.rotation = Quaternion.Lerp(wholeAiCar.transform.rotation, Quaternion.Euler(0, aiCarYRot, 0), 1);
            yield break;
        }
        carFlipped = false;
    }
    private void handleMotor()
    {
        // turning wheels based on input from code
        BRWheelCollider.motorTorque = vroomFactor * motorPower;
        BLWheelCollider.motorTorque = vroomFactor * motorPower;

        // braking when braking tuned on
        brakeForce = isBraking ? brakePower : 0f;
        FRWheelCollider.brakeTorque = brakeForce;
        FLWheelCollider.brakeTorque = brakeForce;
        BRWheelCollider.brakeTorque = brakeForce;
        BLWheelCollider.brakeTorque = brakeForce;
    }
    private void Steering()
    {
        // steering when steering designated
        steeringAngle = maxSteeringAngle * turnFactor;
        FRWheelCollider.steerAngle = steeringAngle;
        FLWheelCollider.steerAngle = steeringAngle;
    }
    private void UpdateWheels()
    {
        // setting the visuals of the wheels to what the code has determined
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

    // activated when the car collides with anything
    void OnCollisionEnter(Collision crash){
        // getting impulse of collision (change in momentum)
        impulse = Mathf.Abs(crash.impulse.x) + Mathf.Abs(crash.impulse.y) + Mathf.Abs(crash.impulse.z);
        // minusing impulse from health (divided by 10000 as impulse is a big number)
        health = health - impulse / 10000;
        // playing the collision sound
        collisionSound.Play();
        // changing the volume of the collision sound based on how hard the collision was (impulse)
        if ((impulse / 100000) < 1){ // making sure the volume doesn't go above 1
            collisionSound.volume = impulse / 100000;
        }
        else {
            collisionSound.volume = 1;
        }
    }

    private void Explode(){
        // creating the explosion
        Instantiate(explosion, transform);
        // telling the explosion particle system to play
        explosion.Play();
        // making the car disappear
        Destroy(bodyGameObject, 0f);
        Destroy(wheelGameObjects, 0f);
    }
}
