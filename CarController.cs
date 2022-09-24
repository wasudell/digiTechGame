using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{   
    // inputs and outputs for/of van
    public float horizontalInput; // horizontal (steering) input from player
    public float verticalInput; // vertical (accelerator) input from player
    public bool isBraking; // whether or not the brake key is being pressed
    private float steeringAngle; // steering angle output from steering input values
    private float brakeForce; // force of brakes
    public Vector3 comAdjust; // adjustment of the centre of mass of the van
    public Quaternion vanRotation; // rotation of the van
    public float motorPower; // power of the motor
    public float maxSteeringAngle; // maximum angle the wheels can turn
    public float brakePower; // force of brakes
    private float carXRot; // X rotation of the van
    private float carYRot; // Y rotation of the van
    private float carZRot; // Z rotation of the van
    public float carVelocity; // velocity of the van
    public float impulse; // change in momentum in a collision
    public float health = 100; // health of the van

    // physcial aspects of the van and things attached to it
    public GameObject bodyGameObject; // game object of the body of the van
    public GameObject cannonGameObject; // game object of the cannon
    public GameObject wheelGameObjects; // all game objects of the wheels
    // all above game objects are for getting destroyed in the explosion
    private Rigidbody carRigidbody; // rigidbody of the van
    public Transform car; // transform of the body of the van
    public Transform wholeCar; // transform of the whole van
    public WheelCollider FRWheelCollider; // wheel colliders of all the wheels
    public WheelCollider FLWheelCollider;
    public WheelCollider BRWheelCollider;
    public WheelCollider BLWheelCollider;
    public Transform FRWheelTransform; // transforms (visuals) of all the wheels
    public Transform FLWheelTransform;
    public Transform BRWheelTransform;
    public Transform BLWheelTransform;

    // Audio
    private AudioSource sound; // thing on the van that produces the sound (speaker) which is picked up by
    // an audio listener (microphone) on the camera
    private AudioSource collisionSound; // getting audio source on body to produce the collision sound
    public UIScore score; // score script in order to find when the pause menu is activated so sound can be paused

    // explosion and game over things
    public ParticleSystem explosion; // particle system that plays on death (explosion)
    private bool deathCheck; // a checker to show when the player is dead (health = 0)
    public bool endScreen = false; // whether or not the end screen should appear

    private void Start()
    {
        // declaring centre of mass so it can be adjsuted
        carRigidbody = GetComponent<Rigidbody>();
        // getting the audio source attached to the van
        sound = GetComponent<AudioSource>();
        // getting the audio souce on the body object, which produces the collision sound
        // this simplifies things as compared to adding two sources on the van
        collisionSound = bodyGameObject.GetComponent<AudioSource>();
        // resetting the deathCheck cause van is alive at start
        deathCheck = false;
    }
    private void FixedUpdate()
    { 
        GetInput(); // getting the input from the player or sensors
        handleMotor(); // translating those inputs into moving the van
        Steering(); // translating steering inputs into steering
        UpdateWheels(); // updating the visuals of the wheels
    }
    private void GetInput()
    {
        // adjusting centre of mass
        carRigidbody.centerOfMass = comAdjust;
        carRigidbody.WakeUp();

        // getting input of forward, backward, left and right (W, A, S, D)
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");

        // finding van's velocity
        carVelocity = transform.InverseTransformDirection(carRigidbody.velocity).z;
        // limiting the van's speed
        if (carVelocity > 35){
            verticalInput = 0;
        }

        // finding if the van is going forward, backward, or is still and using W or S to brake accordingly
        if (carVelocity > 0.05){
            isBraking = Input.GetKey(KeyCode.S);
        }
        if (carVelocity < 0.05){
            isBraking = Input.GetKey(KeyCode.W);
        }
        if (carVelocity < 0.05 && carVelocity > -0.05){
            isBraking = Input.GetKey(KeyCode.W);
            isBraking = Input.GetKey(KeyCode.S);
        }
        
        // declaring rotation for the cannon
        vanRotation = transform.rotation;
        // declaring the individual rotation values of the van
        carXRot = transform.rotation.eulerAngles.x;
        carYRot = transform.rotation.eulerAngles.y;
        carZRot = transform.rotation.eulerAngles.z;
        // finding if the van is flipped and if the flipped key is pressed
        bool flipCar = Input.GetKey(KeyCode.F);
        if (((carXRot > 175 & carXRot < 185) || carZRot > 65 || carZRot < -65) & flipCar == true){
            if (carZRot > 65){  // on its left side
            wholeCar.transform.position = Vector3.Lerp(wholeCar.transform.position, wholeCar.TransformPoint(1, 0, 0), 1);
            wholeCar.transform.rotation = Quaternion.Lerp(wholeCar.transform.rotation, Quaternion.Euler(0, carYRot, 0), 1);
            return;
            }
            if (carZRot < -65){  // on its right side
            wholeCar.transform.position = Vector3.Lerp(wholeCar.transform.position, wholeCar.TransformPoint(1, 0, 0), 1);
            wholeCar.transform.rotation = Quaternion.Lerp(wholeCar.transform.rotation, Quaternion.Euler(0, carYRot, 0), 1);
            return;
            }
            if (carXRot > 175 & carXRot < 185){ // upside down
            wholeCar.transform.position = Vector3.Lerp(wholeCar.transform.position, wholeCar.TransformPoint(0, -1, 0), 1);
            wholeCar.transform.rotation = Quaternion.Lerp(wholeCar.transform.rotation, Quaternion.Euler(0, carYRot, 0), 1);
            return;
            }
            return;
        }

        // exploding the van if health is equal to or less than 0
        if (health <= 0 && deathCheck == false){
            Explode();
            // making sure the explode script only runs once
            deathCheck = true;
            // turning off the engine sound upon death
            sound.mute = !sound.mute;
        }

        // increasing the sound's (engine's) pitch as the van moves faster, as that makes it sounds like the engine is working harder and making it move faster
        sound.pitch = 1 + ((carVelocity / 100) * verticalInput); // multiplied by vertical input so that if the accelerator key isn't being pressed, it doesn't sound like the engine is working hard
        if (carVelocity >= 30){
            sound.pitch = 1 + 30/100; // vertical input is set to 0 above 30 to set a max speed so this ensures that the engine doesn't sound like it's turning off
        }
    }

    private void handleMotor()
    {
        FRWheelCollider.motorTorque = verticalInput * motorPower; // moving the van forward based on accelerator input and the power of the van's motor
        FLWheelCollider.motorTorque = verticalInput * motorPower;
        BRWheelCollider.motorTorque = verticalInput * motorPower;
        BLWheelCollider.motorTorque = verticalInput * motorPower;


        brakeForce = isBraking ? brakePower : 0f; // turning the brakes on if the brake input is pressed
        FRWheelCollider.brakeTorque = brakeForce; // applying that value as a force
        FLWheelCollider.brakeTorque = brakeForce;
        BRWheelCollider.brakeTorque = brakeForce;
        BLWheelCollider.brakeTorque = brakeForce;
    }

    private void Steering()
    {
        steeringAngle = maxSteeringAngle * horizontalInput; // steering the van based on the steering input and the max steering the van can do
        FRWheelCollider.steerAngle = steeringAngle; // telling the wheel colliders to steer
        FLWheelCollider.steerAngle = steeringAngle;
    }

    private void UpdateWheels()
    {
        UpdateSingleWheel(FLWheelCollider, FLWheelTransform); // updating the visuals of the wheels based on steering angle
        UpdateSingleWheel(FRWheelCollider, FRWheelTransform);
        UpdateSingleWheel(BLWheelCollider, BLWheelTransform);
        UpdateSingleWheel(BRWheelCollider, BRWheelTransform);
    }
    
    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        // matching the wheel visual's position and rotation to that of the wheel collider
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }

    void OnCollisionEnter(Collision crash){ // called when the van collides with another object
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

    private void Explode(){ // called when health = 0
        // creating the explosion
        Instantiate(explosion, transform);
        // telling the explosion particle system to play
        explosion.Play();
        // making the van disappear
        Destroy(bodyGameObject, 0f);
        Destroy(cannonGameObject, 0f);
        Destroy(wheelGameObjects, 0f);
        // Making the end screen wait a few seconds to activate
        StartCoroutine(EndScreenWait());
    }

    IEnumerator EndScreenWait(){ // telling the end screen to show after a few seconds so it doesn't appear immediately
        yield return new WaitForSeconds(3);
        endScreen = true; // indicates to Score.cs script to show the end screen
    }
}
