using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoahsNavHelper : MonoBehaviour
{



    public static int RandomChoice(int[] numbers)
    {
        if (numbers == null || numbers.Length == 0)
        {
            Debug.LogError("Array must not be null or empty.");
            return -1; // Return a default value or handle appropriately
        }

        int randomIndex = UnityEngine.Random.Range(0, numbers.Length);
        return numbers[randomIndex];
    }

    public static float RandomFloat(float a, float b)
    {
        return UnityEngine.Random.Range(a, b);
    }


    public static GameObject FindClosestGameObjectWithTag(GameObject origin, string tag)
    {
        // Get all active GameObjects with the specified tag
        GameObject[] allTaggedObjects = GameObject.FindGameObjectsWithTag(tag);

        GameObject closestObject = null;
        float shortestDistance = Mathf.Infinity;

        // Iterate through all GameObjects with the specified tag
        foreach (GameObject obj in allTaggedObjects)
        {
            // Skip self
            if (obj == origin)
            {
                continue;
            }

            // Calculate the distance between the current GameObject and the tagged GameObject
            float distance = Vector3.Distance(origin.transform.position, obj.transform.position);

            // If this GameObject is closer than the current closest, update the closestObject and shortestDistance
            if (distance < shortestDistance)
            {
                closestObject = obj;
                shortestDistance = distance;
            }
        }

        // Return the closest GameObject with the specified tag, or null if none are found
        return closestObject;
    }


    public static GameObject[] FlattenGrid(GameObject[,] grid)
    {
        List<GameObject> flattenedList = new List<GameObject>();
        foreach (GameObject obj in grid)
        {
            if (obj != null) // Optional: Skip null entries if grid might contain them
            {
                flattenedList.Add(obj);
            }
        }
        GameObject[] flat = flattenedList.ToArray();
        return flat;
    }

    public static GameObject FindClosestTile(GameObject origin, GameObject[] flat_grid)
    {
        // Get all active GameObjects with the specified tag

        GameObject[] allTaggedObjects = flat_grid;

        GameObject closestObject = null;
        float shortestDistance = Mathf.Infinity;

        // Iterate through all GameObjects with the specified tag
        foreach (GameObject obj in allTaggedObjects)
        {
            // Skip self
            if (obj == origin)
            {
                continue;
            }

            // Calculate the distance between the current GameObject and the tagged GameObject
            float distance = Vector3.Distance(origin.transform.position, obj.transform.position);

            // If this GameObject is closer than the current closest, update the closestObject and shortestDistance
            if (distance < shortestDistance)
            {
                closestObject = obj;
                shortestDistance = distance;
            }
        }

        // Return the closest GameObject with the specified tag, or null if none are found
        return closestObject;
    }

    public static GameObject FindClosestWalkableTile(GameObject origin, GameObject[] flat_grid)
    {
        // Get all GameObjects in the flat grid
        GameObject[] allTaggedObjects = flat_grid;

        GameObject closestObject = null;
        float shortestDistance = Mathf.Infinity;

        // Iterate through all GameObjects in the flat grid
        foreach (GameObject obj in allTaggedObjects)
        {
            // Skip self
            if (obj == origin)
            {
                continue;
            }

            // Check if the GameObject has a NoahsTile component
            NoahsTile tile = obj.GetComponent<NoahsTile>();
            if (tile == null || !tile.is_walkable)
            {
                // Skip this GameObject if it doesn't have a NoahsTile component or is not walkable
                continue;
            }

            // Calculate the distance between the current GameObject and the origin
            float distance = Vector3.Distance(origin.transform.position, obj.transform.position);

            // If this GameObject is closer than the current closest, update the closestObject and shortestDistance
            if (distance < shortestDistance)
            {
                closestObject = obj;
                shortestDistance = distance;
            }
        }

        // Return the closest walkable GameObject, or null if none are found
        return closestObject;
    }


    public static GameObject NextTile(List<GameObject> path, GameObject my_tile)
    {
        // Check if my_tile exists in the path
        int index = path.IndexOf(my_tile);

        // If my_tile is not in the list or is the last element, return null
        if (index == -1 || index >= path.Count - 1)
        {
            return null;
        }

        // Return the next GameObject in the path
        return path[index + 1];
    }


    public static void TurnAgent(GameObject thisthing, string dir, float degrees)
    {
        if (dir == "right")
        {
            thisthing.transform.Rotate(0, degrees * Time.timeScale * Time.deltaTime, 0);
        }
        else if (dir == "left")
        {
            thisthing.transform.Rotate(0, -degrees * Time.timeScale * Time.deltaTime, 0);
        }
        else if (dir == "up")
        {
            thisthing.transform.Rotate(-degrees * Time.timeScale * Time.deltaTime, 0, 0);
        }
        else if (dir == "down")
        {
            thisthing.transform.Rotate(degrees * Time.timeScale * Time.deltaTime, 0, 0);
        }

    }

    public static void Turn(GameObject thisthing, string dir, float degrees)
    {
        if (dir == "right")
        {
            thisthing.transform.Rotate(0, degrees * Time.timeScale, 0);
        }
        else if (dir == "left")
        {
            thisthing.transform.Rotate(0, -degrees * Time.timeScale, 0);
        }
        else if (dir == "up")
        {
            thisthing.transform.Rotate(-degrees * Time.timeScale, 0, 0);
        }
        else if (dir == "down")
        {
            thisthing.transform.Rotate(degrees * Time.timeScale, 0, 0);
        }

    }



        public static void TurnTowards(GameObject me, Vector3 target, float turn_speed)
    {
        // makes me turn towards the target by the turn_speed
        // note: this assumes that the local Z axis is forward
        // example: TurnTowards(this.gameObject, target.transform.position, 5) // assuming target is a GameObject
        Vector3 targetDir = target - me.transform.position;
        float step = turn_speed * Time.deltaTime * Time.timeScale;
        Vector3 newDir = Vector3.RotateTowards(me.transform.forward, targetDir, step, 0.0f);
        me.transform.rotation = Quaternion.LookRotation(newDir);
    }


    public static void AllowRotation(GameObject me, bool x, bool y, bool z)
    {
        // used to lock rotation on x,y or z axis by setting input to false
        // note: only stops rotation of 1 frame, so would need to be run continuously to fully lock rotation
        // example AllowRotation(this.gameObject, false, true, false) would only allow rotation on the y axis, x and y would lock at 0

        Quaternion currentRotation = me.transform.localRotation;
        Vector3 eulerRotation = currentRotation.eulerAngles;

        float use_x = 0;
        float use_y = 0;
        float use_z = 0;

        if (x)
        {
            use_x = eulerRotation.x; // Keep the current x rotation
        }
        if (y)
        {
            use_y = eulerRotation.y; // Keep the current y rotation
        }
        if (z)
        {
            use_z = eulerRotation.z; // Keep the current z rotation
        }

        me.transform.localRotation = Quaternion.Euler(use_x, use_y, use_z);
    }

    public static void PointTowards(GameObject a, GameObject b)
    {
        if (a == null || b == null)
        {
            Debug.LogWarning("One or both GameObjects are null. Cannot point towards.");
            return;
        }

        // Calculate the direction vector from A to B
        Vector3 direction = b.transform.position - a.transform.position;

        // Ensure the direction has no vertical tilt (if needed, optional)
        // direction.y = 0; // Uncomment this line if you want to constrain rotation to the XZ plane.

        // Check if the direction is not zero
        if (direction != Vector3.zero)
        {
            // Calculate the rotation to look in the direction
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Apply the rotation to A
            a.transform.rotation = targetRotation;
        }
    }


    public static void MoveTowardsWithRotation(GameObject a, GameObject b, float stepSize)
    {
        stepSize = stepSize*Time.deltaTime*Time.timeScale;
        if (a == null || b == null)
        {
            Debug.LogWarning("One or both GameObjects are null. Cannot move or rotate.");
            return;
        }

        // Calculate the direction vector from A to B
        Vector3 direction = b.transform.position - a.transform.position;

        // Check if the distance is greater than the step size
        if (direction.magnitude > stepSize)
        {
            // Rotate A to face B
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            a.transform.rotation = targetRotation;

            // Move A towards B
            a.transform.position += direction.normalized * stepSize;
        }
        else
        {
            // If the step size exceeds the distance, snap A to B's position
            a.transform.position = b.transform.position;
        }
    }



    public static void SimpleMove(GameObject me, Vector3 direction, float speed)
    {
        // moves this gameObject in a direct by speed by directly changing the transform.position
        // example: SimpleMove(this.gameObject, this.transform.forward, 5); // would move object forward along its local z axis
        me.transform.Translate(direction * speed * Time.deltaTime * Time.timeScale);
    }

    public static void MoveAgent(GameObject me, Vector3 direction, float speed)
    {
        // moves this gameObject in a direct by speed by altering the velocity of the Rigidbody
        // example: PhysicalMove(this.gameObject, this.transform.forward, 5) // would move object forward along its local z axis
        // the y velocity of the object is not affected to allow for jumping
        if (me.gameObject.GetComponent<Rigidbody>() != null)
        {
            Rigidbody rb = me.gameObject.GetComponent<Rigidbody>();
            Vector3 vel = direction * speed;
            Vector3 this_vel = rb.linearVelocity;
            vel = new Vector3(vel.x, this_vel.y, vel.z);
            rb.linearVelocity = vel;
        }
        else
        {
            Debug.Log("GameObject must have Rigidbody to MoveAgent");
        }
    }



    public static bool Sensor(GameObject sensor, float dist, float rot)
    {
        // returns true if the sensor GameObjects senses an object within dist steps in rot direction
        // example:  Sensor(this.gameObject,10,90) will return true if there is something within 10 steps 90 degrees to this gameObject's right
        // example:  Sensor(this.gameObject,10,-90) will return true if there is something within 10 steps 90 degrees to this gameObject's left
        // example:  Sensor(this.gameObject,10,0) will return true if there is something within 10 steps directly in front of this gameObject
        GameObject spot = new GameObject();
        spot.transform.position = sensor.transform.position;
        spot.transform.rotation = sensor.transform.rotation;
        spot.transform.Rotate(0, rot, 0);
        spot.transform.Translate(0, 0, dist);
        Vector3 spot_direction = spot.transform.position - sensor.transform.position;
        RaycastHit[] hits = Physics.RaycastAll(sensor.transform.position, spot_direction, dist);
        Destroy(spot);
        foreach (RaycastHit hit in hits)
        {
            if (hit.transform.gameObject != sensor.gameObject && hit.transform.gameObject != null)
            {
                return true;
            }
        }
        return false;
    }


    public static Vector3 GetDirectionFrom(GameObject a, GameObject b)
    {
        //returns the direction you need to go in to get from "a" to "b"
        return b.transform.position - a.transform.position;
    }



    public static bool CanSee(GameObject eye, GameObject target, float range)
    {
        // returns true if the eye gameObject can see the target and false if it cannot
        int original_layer = eye.layer;
        eye.layer = LayerMask.NameToLayer("Ignore Raycast");
        RaycastHit hit;
        if (Physics.Raycast(eye.transform.position, GetDirectionFrom(eye, target), out hit, range))
        {
            if (hit.transform.gameObject == target)
            {
                eye.layer = original_layer;
                return true;
            }
            else
            {
                eye.layer = original_layer;
                return false;
            }
        }
        else
        {
            eye.layer = original_layer;
            return false;
        }

    }


    public static GameObject WhatISee(GameObject eye, float range)
    {
        // returns the gameObject the eye is currently looking at.
        int original_layer = eye.layer;
        eye.layer = LayerMask.NameToLayer("Ignore Raycast");
        RaycastHit hit;
        if (Physics.Raycast(eye.transform.position, eye.transform.forward, out hit, range))
        {
            return hit.transform.gameObject;
        }
        else
        {
            eye.layer = original_layer;
            return null;

        }

    }



    public static bool Scan(GameObject scanner, float dist, int start_y, int stop_y)
    // example scan(eye,5,-45,45) will scan everything from left 45 degrees and right 45 degrees
    // and return true if anything is within 5 in that 90 degree scan area in fron of the the eye
    {
        bool any_hits = false;
        for (int rot = start_y; rot <= stop_y; rot++)
        {
            bool check = Sensor(scanner,dist,(float)rot);
            if (check)
            {
                any_hits = check;
                return any_hits;
            }
        }
        return any_hits;
    }




    public static string IsToLeftOrRight(GameObject A, GameObject B)
    {
        // Get the local position vector pointing from A to B
        Vector3 localDirection = A.transform.InverseTransformPoint(B.transform.position);

        // Determine the side using the X component of the local direction
        float side = localDirection.x;

        // Return "left" or "right" based on the sign of the X component
        if (side < 0)
        {
            return "left";
        }
        else if (side > 0)
        {
            return "right";
        }
        else
        {
            return "center"; // Optional: if exactly aligned, return "center"
        }
    }









}
