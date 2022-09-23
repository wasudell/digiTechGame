using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoliceCar : MonoBehaviour
{
    private float horizontalInput;
    private float verticalInput;
    private float motorPower = 8000f;
    private float maxSteeringAngle = 35f;
    private float steeringAngle;
    private Vector3 comAdjust;

    public Transform FLWheelTransform;
    public Transform FRWheelTransform;
    public Transform BLWheelTransform;
    public Transform BRWheelTransform;
    public WheelCollider FLWheelCollider;
    public WheelCollider FRWheelCollider;
    public WheelCollider BLWheelCollider;
    public WheelCollider BRWheelCollider;

    void Start()
    {
        comAdjust = new Vector3(0, -0.6f, 0);
        StartCoroutine(Driving());
        GetComponent<Rigidbody>().centerOfMass = comAdjust;
    }

    void Update()
    {
        handleMotor();
        Steering();
        UpdateWheels();
    }

    private void handleMotor()
    {
        FRWheelCollider.motorTorque = verticalInput * motorPower;
        FLWheelCollider.motorTorque = verticalInput * motorPower;
        BRWheelCollider.motorTorque = verticalInput * motorPower;
        BLWheelCollider.motorTorque = verticalInput * motorPower;
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

    
    IEnumerator Driving()
    {
        transform.position = new Vector3(136, 0.1f, 235);
        transform.rotation = Quaternion.Euler(0, 180, 0);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        verticalInput = 1;
        yield return new WaitForSeconds(10);
        transform.position = new Vector3(72, 0.1f, 135);
        transform.rotation = Quaternion.Euler(0, 90, 0);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield return new WaitForSeconds(3.4f);
        horizontalInput = -1;
        yield return new WaitForSeconds(1.7f);
        horizontalInput = 1;
        yield return new WaitForSeconds(0.8f);
        horizontalInput = 0;
        yield return new WaitForSeconds(6.6f);
        transform.position = new Vector3(136, 0.1f, 60);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield return new WaitForSeconds (3.7f);
        horizontalInput = -1;
        yield return new WaitForSeconds(1.8f);
        horizontalInput = 1;
        yield return new WaitForSeconds(7.6f);
        transform.position = new Vector3(136, 0.1f, 60);
        transform.rotation = Quaternion.Euler(0, 0, 0);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        GetComponent<Rigidbody>().angularVelocity = Vector3.zero;
        horizontalInput = 0;
        yield return new WaitForSeconds (10);
        transform.position = new Vector3(136, 50, 150);
        transform.rotation = Quaternion.Euler(86, 255, 67);
        GetComponent<Rigidbody>().velocity = Vector3.zero;
        yield return new WaitForSeconds(20);
        StartCoroutine(Driving());
    }
}
