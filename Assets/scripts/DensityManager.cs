using UnityEngine;

public class DensityManager : MonoBehaviour
{
    [Header("Settings")]
    public string targetTag = "VariableCubes";
    private bool isDephased = false;

    // Call this function from your UI Button
    public void DensityShifter()
    {
        
        isDephased = !isDephased;

        
        GameObject[] targetObjects = GameObject.FindGameObjectsWithTag(targetTag);

        foreach (GameObject obj in targetObjects)
        {
            ShiftDensity(obj);
        }
    }

    private void ShiftDensity(GameObject obj)
    {
     
        BoxCollider cldrBox = obj.GetComponent<BoxCollider>();

        if (cldrBox != null)

        {            
            if (isDephased)
            {
                cldrBox.isTrigger = true;
            }
            else
            {
                cldrBox.isTrigger = false;
            }
                       
        }
        Rigidbody rigidB = obj.GetComponent<Rigidbody>();
        if (rigidB != null)
        {
            if (isDephased)
            {
                rigidB.mass = 0.25f;
                rigidB.isKinematic = true;
            }
            else
            {
                rigidB.mass = 20.0f;
                rigidB.isKinematic = false; 
            }
        }

        Renderer rend = obj.GetComponent<Renderer>();
        if (rend != null)
        {
            Color color = rend.material.color;

            if (isDephased)
            {
                color.a = 0.25f;
            }
            else
            {
                color.a = 1.0f;
            }
          
            rend.material.color = color;
        }
    }
}
