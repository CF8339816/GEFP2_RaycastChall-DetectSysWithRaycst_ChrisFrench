using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GrappleGunCtrl : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRender;
    public Transform barrel;
    public Transform firstPersonCam;

    public playercontroler playerScript;
    public LayerMask Grappleable;
    public CharacterController characterController;

    [Header("Settings")]
    public float maxDistance = 100f;
    public float pullSpeed = 200f;

    [Header("Crosshair Settings")]

    public Image Density;
    public Image Firework;
    public Image Crosshairs;
    public Image Grapple;
    public Image Pull;

    private Vector3 grapplePoint;
    private bool isPullingPlayer = false;
    private bool isPullingObject = false;
    private Rigidbody targetBody;

    [Header("Settings")]
    public GameObject prefabToSpawn;
    public Transform spawnLocation;
    public string interactFireworkKey = "E";

    [Header("UI Reference")]
    public GameObject popupDenUI;
    public TextMeshProUGUI popupDenText;

    private bool isPlayerNear = false;

    [Header("Settings")]

    public DensityManager densityManager;
    public string interactDensityKey = "P";

    [Header("UI Reference")]
    public GameObject popupFWUI;
    public TextMeshProUGUI popupFWText;



    void Start()
    {
        ResetCrosshairUI();
        if (popupDenUI != null) popupDenUI.SetActive(false);
    }
    void Update()
    {
        if (!isPullingPlayer && !isPullingObject)
        {
            UpdateCrosshairVisibility();
        }

        if (Input.GetMouseButtonDown(2)) StartPullPlayer();
        if (Input.GetMouseButtonUp(2)) StopGrapple();
        if (Input.GetMouseButtonDown(1)) StartPullObject();
        if (Input.GetMouseButtonUp(1)) StopGrapple();

        if (isPlayerNear && Input.GetKeyDown(KeyCode.E))
        {
            InteractFirework();
        }

        if (isPlayerNear && Input.GetKeyDown(KeyCode.P))
        {
            InteractDensity();
        }
    }
    void LateUpdate()
    {
        if (isPullingPlayer)
        {
            DrawRope(grapplePoint);
            Vector3 direction = (grapplePoint - characterController.transform.position).normalized;
            characterController.Move(direction * pullSpeed * Time.deltaTime);

            if (Vector3.Distance(characterController.transform.position, grapplePoint) < 1.5f)
            {
                StopGrapple();
            }
        }

        if (isPullingObject && targetBody != null)
        {
            DrawRope(targetBody.position);
            targetBody.position = Vector3.MoveTowards(targetBody.position, barrel.position, pullSpeed * Time.deltaTime);
            targetBody.linearVelocity = Vector3.zero;

            if (Vector3.Distance(targetBody.position, barrel.position) < 1.5f)
            {
                StopGrapple();
            }
        }
    }
    public void UpdateCrosshairVisibility()
    {
        if (playerScript != null && !playerScript.PowerOn)
        {
            ResetCrosshairUI();
            return;
        }
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
        {
            Debug.Log($"Raycast hit: {hit.collider.name} with tag: {hit.collider.tag}");

            if (hit.collider.tag == "Grapplepoint")
            {
                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = true;
                if (Pull != null) Pull.enabled = false;
                if (Density != null) Density.enabled = false;
                if (Firework != null) Firework.enabled = false;
            }
            else if (hit.collider.tag == "Pullable" && hit.collider.GetComponentInParent<Rigidbody>() != null)
            {
                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = false;
                if (Pull != null) Pull.enabled = true;
                if (Density != null) Density.enabled = false;
                if (Firework != null) Firework.enabled = false;
            }
            else if (hit.collider.tag == "Trigger" && hit.collider.GetComponentInParent<Rigidbody>() != null)
            {
                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = false;
                if (Pull != null) Pull.enabled = false;
                if (Density != null) Density.enabled = false;
                if (Firework != null) Firework.enabled = true;

            }
            else if (hit.collider.tag == "Switch" && hit.collider.GetComponentInParent<Rigidbody>() != null)
            {
                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = false;
                if (Pull != null) Pull.enabled = false;
                if (Density != null) Density.enabled = true;
                if (Firework != null) Firework.enabled = false;

            }
            else
            {
                ResetCrosshairUI();
            }
        }
        else
        {
            ResetCrosshairUI();
        }
    }
    public void StartPullPlayer()
    {
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
        {
            if (hit.collider.tag == ("Grapplepoint"))
            {

                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = true;
                if (Pull != null) Pull.enabled = false;
                if (Density != null) Density.enabled = false;
                if (Firework != null) Firework.enabled = false;

                grapplePoint = hit.point;
                isPullingPlayer = true;
                lineRender.positionCount = 2;
            }
        }
    }
    public void StartPullObject()
    {
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
        {
            //if (hit.collider.tag == "Pullable" && hit.collider.TryGetComponent<Rigidbody>(out Rigidbody rb))
            if (hit.collider.tag == "Pullable" && hit.collider.GetComponentInParent<Rigidbody>() != null)

            {
                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = false;
                if (Pull != null) Pull.enabled = true;
                if (Density != null) Density.enabled = false;
                if (Firework != null) Firework.enabled = false;
                //targetBody = rb;
                targetBody = hit.collider.GetComponentInParent<Rigidbody>();
                isPullingObject = true;
                lineRender.positionCount = 2;
            }
        }
    }
    public void StopGrapple()
    {
        isPullingPlayer = false;
        isPullingObject = false;
        targetBody = null;
        lineRender.positionCount = 0;

        ResetCrosshairUI();
    }
    public void ResetCrosshairUI()
    {
        if (Crosshairs != null) Crosshairs.enabled = true;
        if (Grapple != null) Grapple.enabled = false;
        if (Pull != null) Pull.enabled = false;
        if (Density != null) Density.enabled = false;
        if (Firework != null) Firework.enabled = false;
    }
    public void DrawRope(Vector3 targetPos)
    {
        lineRender.SetPosition(0, barrel.position);
        lineRender.SetPosition(1, targetPos);
    }
    void InteractFirework()
    {
        Instantiate(prefabToSpawn, spawnLocation.position, spawnLocation.rotation);

        popupFWUI.SetActive(false);

    }



    private void OnTriggerEnter(Collider other)
    {
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Switches))
        {
            if (other.CompareTag("Player"))
            {
                isPlayerNear = true;
                if (popupFWUI != null)
                {
                    popupFWText.text = "Press [" + interactKey + "] to Interact";
                    popupFWUI.SetActive(true);
                }
            }
        }

      
    }
        
    //    private void OnTriggerEnter(Collider other)
    //{
        
    //    {
          
    //    }
    //}
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (popupFWUI != null) popupFWUI.SetActive(false);
        }
    }


    void InteractDensity()
    {

        densityManager.DensityShifter();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = true;
            if (popupDenUI != null)
            {
                popupDenText.text = "Press [" + interactKey + "] to \nde-/-phase\n blocks ";
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
