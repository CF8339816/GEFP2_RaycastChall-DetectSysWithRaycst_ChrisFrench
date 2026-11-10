using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Interaction_Manager : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRender;
    public Transform barrel;
    public Transform firstPersonCam;

    public playercontroler playerScript;
    public LayerMask Grappleable;
    public LayerMask Switches;
    public LayerMask combinedMask;
    public CharacterController characterController;

    [Header("Settings")]
    public float grappleRange = 100f;//longer range for grapple and pull
    public float switchRange = 4f; // shorter range for button and switch
    public float pullSpeed = 200f;

    [Header("Crosshair Settings")]
    public Image Density;
    public Image Firework;
    public Image Crosshairs;
    public Image Grapple;
    public Image Pull;

    [Header("Switches")]
    public GameObject prefabToSpawn;
    public Transform spawnLocation;
    //public string interactFireworkKey = "E";
    public DensityManager densityManager;
    //public string interactDensityKey = "P";
    //public KeyCode interactFireworkKey = KeyCode.E;
    //public KeyCode interactDensityKey = KeyCode.P;

    [Header("UI Reference")]
    public GameObject popupDenUI;
    public TextMeshProUGUI popupDenText;
    public GameObject popupFWUI;
    public TextMeshProUGUI popupFWText;

    private Vector3 grapplePoint;
    private bool isPullingPlayer = false;
    private bool isPullingObject = false;
    private Rigidbody targetBody;

    private bool lookingAtFireworkSwitch = false;
    private bool lookingAtDensitySwitch = false;
    void Start()
    {
        ResetCrosshairUI();
        HidePopups();
        //if (popupDenUI != null) popupDenUI.SetActive(false);
    }
    private void HidePopups()
    {
        if (popupFWUI != null) popupFWUI.SetActive(false);
        if (popupDenUI != null) popupDenUI.SetActive(false);
    }
    void Update()
    {
        if (!isPullingPlayer && !isPullingObject)
        {
            CheckTargets();
            UpdateCrosshairVisibility();
        }

        if (Input.GetMouseButtonDown(2)) StartPullPlayer();
        if (Input.GetMouseButtonUp(2) && isPullingPlayer) StopGrapple();
        if (Input.GetMouseButtonDown(1)) StartPullObject();
        if (Input.GetMouseButtonUp(1) && isPullingObject) StopGrapple();

        if (Input.GetMouseButtonDown(0))
        {
            if (lookingAtFireworkSwitch)
            {
                InteractFirework();
            }

            if (lookingAtDensitySwitch)
            {
                InteractDensity();
            }
        }
    }
    private void CheckTargets()
    {
        lookingAtFireworkSwitch = false;
        lookingAtDensitySwitch = false;
        HidePopups();

        combinedMask = Grappleable | Switches;// combines both layer masks for distance checking

        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, grappleRange, combinedMask))
        {
            if (hit.distance <= switchRange)// checks for  hitting a sswitch in range
            {
                if (hit.collider.CompareTag("Trigger"))
                {
                    lookingAtFireworkSwitch = true;
                    UpdateCrosshairVisibility();
                    if (popupFWUI != null && popupFWText != null)
                    {
                        popupFWText.text = $"Left-Click to Interact";
                        popupFWUI.SetActive(true);
                    }
                    return;
                }
                else if (hit.collider.CompareTag("Switch"))
                {
                    lookingAtDensitySwitch = true;
                    UpdateCrosshairVisibility();
                    if (popupDenUI != null && popupDenText != null)
                    {
                        popupDenText.text = $"Left-Click to \nde-/-phase\n blocks";
                        popupDenUI.SetActive(true);
                    }
                    return;
                }
            }

            if (hit.collider.CompareTag("Grapplepoint"))// Checks for longer range targets if no short range ones found
            {
                UpdateCrosshairVisibility();
                if (popupFWUI != null && popupFWText != null)
                {
                    popupFWText.text = $"Middle click to grapple";
                    popupFWUI.SetActive(true);
                }
                return;
            }
            else if (hit.collider.CompareTag("Pullable") && hit.collider.GetComponentInParent<Rigidbody>() != null)
            {
                UpdateCrosshairVisibility();
                if (popupDenUI != null && popupDenText != null)
                {
                    popupDenText.text = $"right click to pull";
                    popupDenUI.SetActive(true);
                }
                return;
            }
        }
        ResetCrosshairUI(); //ensures reset crosshairs if none found.
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
    void UpdateCrosshairVisibility()
    {
        if (playerScript != null )
        {
            ResetCrosshairUI();
            return;
        }
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, grappleRange, combinedMask))
        //if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
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
            else if (hit.collider.tag == "Pullable")// && hit.collider.GetComponentInParent<Rigidbody>() != null)
            {
                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = false;
                if (Pull != null) Pull.enabled = true;
                if (Density != null) Density.enabled = false;
                if (Firework != null) Firework.enabled = false;
            }
            else if (hit.collider.tag == "Trigger")// && hit.collider.GetComponentInParent<Rigidbody>() != null)
            {
                if (Firework != null) Firework.enabled = true;
                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = false;
                if (Pull != null) Pull.enabled = false;
                if (Density != null) Density.enabled = false;
            }
            else if (hit.collider.tag == "Switch")// && hit.collider.GetComponentInParent<Rigidbody>() != null)
            {
                if (Density != null) Density.enabled = true;
                if (Crosshairs != null) Crosshairs.enabled = false;
                if (Grapple != null) Grapple.enabled = false;
                if (Pull != null) Pull.enabled = false;
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
    void StartPullPlayer()
    {
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, grappleRange, combinedMask))
        //if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
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
    void StartPullObject()
    {
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, grappleRange, combinedMask))
        //if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
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
    void StopGrapple()
    {
        isPullingPlayer = false;
        isPullingObject = false;
        targetBody = null;
        lineRender.positionCount = 0;

        ResetCrosshairUI();
    }
    void ResetCrosshairUI()
    {
        if (Crosshairs != null) Crosshairs.enabled = true;
        if (Grapple != null) Grapple.enabled = false;
        if (Pull != null) Pull.enabled = false;
        if (Density != null) Density.enabled = false;
        if (Firework != null) Firework.enabled = false;
    }
    void DrawRope(Vector3 targetPos)
    {
        lineRender.SetPosition(0, barrel.position);
        lineRender.SetPosition(1, targetPos);
    }
    void InteractFirework()
    {
        Instantiate(prefabToSpawn, spawnLocation.position, spawnLocation.rotation);
        popupFWUI.SetActive(false);
    }

    void InteractDensity()
    {
        densityManager.DensityShifter();
    }
}