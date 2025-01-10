using UnityEngine;

public class NoahsTile : MonoBehaviour
{
    public bool is_walkable = true;
    public Material walkable_material;
    public Material not_walkable_material;

    private MeshRenderer meshRenderer;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        if (is_walkable)
        {
            makeWalkable();
        }
        else
        {
            makeWall();
        }

    }

    public void makeWalkable()
    {
        // Check if the MeshRenderer component exists
        if (meshRenderer != null && walkable_material != null)
        {
            // Change the material
            meshRenderer.material = walkable_material;
        }
    }


    public void makeWall()
    {
        // Check if the MeshRenderer component exists
        if (meshRenderer != null && not_walkable_material != null)
        {
            // Change the material
            meshRenderer.material = not_walkable_material;
        }
    }


}
