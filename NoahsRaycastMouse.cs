using UnityEngine;

public class NoahsRaycastMouse : MonoBehaviour
{
    public GameObject pointer; // Assign the pointer GameObject in the Inspector

    void Update()
    {
        // Ensure the pointer GameObject is assigned
        if (pointer == null)
        {
            Debug.LogWarning("Pointer GameObject is not assigned!");
            return;
        }


        if(Input.GetMouseButtonDown(0))
        {
            // Check for mouse position and create a ray from the camera
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // Raycast hit information
            RaycastHit hit;

            // Perform the raycast
            if (Physics.Raycast(ray, out hit))
            {
                // Set the position of the pointer to the hit point
                pointer.transform.position = hit.point;
            }
        }
       
    }
}
