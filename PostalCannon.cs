using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PostalCannon : MonoBehaviour
{
    // script to get rotation of van
    public CarController van;
    public GameObject previousHouse; // previous house so that the correct address doesn't get changed while the package is flying
    // array of all addresses
    public string[] addresses = System.IO.File.ReadAllLines(@"Assets/Addresses.txt");
    public string addrName; // name of randomly selected address
    public GameObject house; // game object that has that name
    // getting values for cannon rotation speed and distance
    public float distance; // distance to house
    public float rotationSpeed; // rotation speed of the cannon
    
    // package firing
    public GameObject package; // package prefab
    public float packageSpeed; // speed that package is fired out of cannon

    
    void Start()
    {
        // choosing a random number, grabbing that number from the array,
        // and tying it to the right game object at the start of the game
        int addrNum = Random.Range(0, addresses.Length);
            addrName = addresses[addrNum];
            house = GameObject.Find(addrName);
    }
    
    void Update()
    {
        FindHouse();
    }

    void FixedUpdate()
    {
        PointAtHouse();
    }

    void FindHouse()
    {
        if (Input.GetButtonDown("Jump")){ // only allows the button to be pressed against once the button has been released first (so not "pressed" over multiple frames)
        // Jump bound to space key
            // stating the previous house so that the package knows if it hit the right house
            previousHouse = house;
            // finding house when space pressed (when package fires), doing same thing as when Start() is called
            int addrNum = Random.Range(0, addresses.Length); // choosing random address
            addrName = addresses[addrNum]; // tying that address to a string
            house = GameObject.Find(addrName); // tying that string to a game object
            // firing package
            SpawnPackage();
        }
    }

    void PointAtHouse()
    {
        // finding distance between cannon and house
        float xDifference = transform.position.x - house.transform.position.x;
        float zDifference = transform.position.z - house.transform.position.z;
        distance = Mathf.Sqrt(xDifference * xDifference + zDifference * zDifference);

        if (distance < 80){
            // pointing the cannon towards the house when it is less than 80 distance away
            var direction = transform.position - house.transform.position;
            var rotation = Quaternion.LookRotation(direction, Vector3.up);
            var rotation2 = rotation * Quaternion.Euler(-90,180,0);
            transform.rotation = Quaternion.Lerp(transform.rotation, rotation2, rotationSpeed * Time.deltaTime);
        }

        else {
            // resetting the rotation to that of the van when the distance is more than 80
            var resetRotation = van.vanRotation * Quaternion.Euler(-90,0,0);
            transform.rotation = Quaternion.Lerp(transform.rotation, resetRotation, rotationSpeed * Time.deltaTime);
        }
        
    }

    void SpawnPackage(){
        GameObject newPackage = Instantiate(package, transform.position, Quaternion.identity); // creating the package
        newPackage.GetComponent<Rigidbody>().velocity = -(transform.up) * packageSpeed; // adding velocity to the new package
    }
}
