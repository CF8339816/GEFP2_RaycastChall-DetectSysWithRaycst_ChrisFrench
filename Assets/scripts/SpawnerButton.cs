using UnityEngine;
using TMPro; // Required for the popup text

public class SpawnerButton : MonoBehaviour
{
    [Header("Settings")]
    public GameObject prefabToSpawn;
    public Transform spawnLocation;
    public string interactKey = "E"; 

    [Header("UI Reference")]
    public GameObject popupUI; 
    public TextMeshProUGUI popupText;

    private bool isPlayerNear = false;

    void Start()
    {
        if (popupUI != null) popupUI.SetActive(false);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.E)) 
        {
            Interact();
        }
    }

    void Interact()
    {
        
        Instantiate(prefabToSpawn, spawnLocation.position, spawnLocation.rotation);

        
        popupUI.SetActive(false);
     
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (popupUI != null)
            {
                popupText.text = "Press [" + interactKey + "] to Interact";
                popupUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (popupUI != null) popupUI.SetActive(false);
        }
    }
}