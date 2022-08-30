using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{   
    public float horizontalInput;
    public float verticalInput;
    public bool isBraking;
    private float steeringAngle;
    private float brakeForce;
    public Vector3 comAdjust;
    public Quaternion vanRotation;

    public float motorPower = 10000f;
    public float maxSteeringAngle = 35f;
    public float brakePower = 1200000f;
    public float carXRot;
    public float carZRot;

    public Rigidbody carRigidbody;
    public Transform car;
    public Transform wholeCar;
    public float carVelocity;
    public WheelCollider FRWheelCollider;
    public WheelCollider FLWheelCollider;
    public WheelCollider BRWheelCollider;
    public WheelCollider BLWheelCollider;
    public Transform FRWheelTransform;
    public Transform FLWheelTransform;
    public Transform BRWheelTransform;
    public Transform BLWheelTransform;

    private void Start()
    {
        // declaring centre of mass so it can be adjsuted
        carRigidbody = GetComponent<Rigidbody>();
    }
    private void FixedUpdate()
    {
        GetInput();
        handleMotor();
        Steering();
        UpdateWheels();
    }
    private void GetInput()
    {
        // declaring rotation for the cannon
        vanRotation = transform.rotation;
        // adjusting centre of mass
        carRigidbody.centerOfMass = comAdjust;
        carRigidbody.WakeUp();
        // getting input of forward, backward, left and right (W, A, S, D)
        horizontalInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        // finding car velocity
        carVelocity = transform.InverseTransformDirection(carRigidbody.velocity).z;
        // finding if the car is going forward, backward, or is still and using W or S to brake accordingly
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
        // converting quaternion rotation to euler angles
        float X = car.transform.rotation.x;
        float Y = car.transform.rotation.y;
        float Z = car.transform.rotation.z;
        float W = car.transform.rotation.w;
        float carXRotRad = Mathf.Atan2(2*X*W-2*Y*Z , 1 - 2*X*X - 2*Z*Z);
        carXRot = Mathf.Rad2Deg * carXRotRad + 90;
        float carZRotRad = Mathf.Asin(2*X*Y + 2*Z*W);
        carZRot = Mathf.Rad2Deg * carZRotRad;
        // finding if the car is flipped and if the flipped key is pressed
        bool flipCar = Input.GetKey(KeyCode.F);
        if (((carXRot > 175 & carXRot < 185) || carZRot > 65 || carZRot < -65) & flipCar == true){
            if (carZRot > 65){  // on its left side
            wholeCar.transform.position = Vector3.Lerp(wholeCar.transform.position, wholeCar.TransformPoint(1, 0, 0), 1);
            wholeCar.transform.rotation = Quaternion.Lerp(wholeCar.transform.rotation, new Quaternion(0, 0, 0, 1), 1);
            return;
            }
            if (carZRot < -65){  // on its right side
            wholeCar.transform.position = Vector3.Lerp(wholeCar.transform.position, wholeCar.TransformPoint(1, 0, 0), 1);
            wholeCar.transform.rotation = Quaternion.Lerp(wholeCar.transform.rotation, new Quaternion(0, 0, 0, 1), 1);
            return;
            }
            if (carXRot > 175 & carXRot < 185){ // upside down
            wholeCar.transform.position = Vector3.Lerp(wholeCar.transform.position, wholeCar.TransformPoint(0, -1, 0), 1);
            wholeCar.transform.rotation = Quaternion.Lerp(wholeCar.transform.rotation, new Quaternion(0, 0, 0, 1), 1);
            return;
            }
            return;
        }
    }
    private void handleMotor()
    {
        FRWheelCollider.motorTorque = verticalInput * motorPower;
        FLWheelCollider.motorTorque = verticalInput * motorPower;
        BRWheelCollider.motorTorque = verticalInput * motorPower;
        BLWheelCollider.motorTorque = verticalInput * motorPower;

        brakeForce = isBraking ? brakePower : 0f;
        FRWheelCollider.brakeTorque = brakeForce;
        FLWheelCollider.brakeTorque = brakeForce;
        BRWheelCollider.brakeTorque = brakeForce;
        BLWheelCollider.brakeTorque = brakeForce;
    }
    private void Steering()
    {
        steeringAngle = maxSteeringAngle * horizontalInput;
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
