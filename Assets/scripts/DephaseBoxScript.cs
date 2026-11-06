
using TMPro;
using UnityEngine;

public class DephaseBoxScript : MonoBehaviour
{
    [Header("Settings")]
    
    public DensityManager densityManager;
    public string interactKey = "P";

    [Header("UI Reference")]
    public GameObject popupUI;
    public TextMeshProUGUI popupText;

    private bool isPlayerNear = false;

    void Start()
    {
        if (popupUI != null)
        {
            popupUI.SetActive(false);
        }
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(KeyCode.P))
        {
            Interact();
        }
    }

    void Interact()
    {

        densityManager.DensityShifter();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (popupUI != null)
            {
                popupText.text = "Press [" + interactKey + "] to \nde-/-phase\n blocks ";
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
