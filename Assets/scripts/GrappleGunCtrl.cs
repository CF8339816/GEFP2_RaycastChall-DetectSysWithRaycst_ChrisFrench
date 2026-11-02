using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GrappleGunCtrl : MonoBehaviour
{
    [Header("References")]
    public LineRenderer lineRender;
    public Transform barrel;
    public Transform firstPersonCam;
    public Transform character;
    public playercontroler playerScript;
    public LayerMask Grappleable;
    public TextMeshProUGUI textGrappleGun;
    [Header("Settings")]
    public float maxDistance = 100f;
    public float pullSpeed = 200f;
    //public GameObject character.LineRenderer;
    [Header("Crosshair Settings")]
    public Image canGrappleCrosshair;
    public Image NormalDotSite;
    public Image Grapple;
    public Image Pull;
    private Vector3 grapplePoint;
    // private SpringJoint joint;
    private bool isPullingPlayer = false;
    private bool isPullingObject = false;
    private Rigidbody targetBody;

    void Start()
    {

        if (canGrappleCrosshair != null)
        {
         canGrappleCrosshair.enabled = false;

        NormalDotSite.enabled = true;
        }
    }
    void Update()
    {

        UpdateCrosshairVisibility();


        if (Input.GetMouseButtonDown(2)) StartPullPlayer();
        if (Input.GetMouseButtonUp(2)) StopGrapple();


        if (Input.GetMouseButtonDown(1)) StartPullObject();
        if (Input.GetMouseButtonUp(1)) StopGrapple();
    }

    void LateUpdate()
    {


        if (isPullingPlayer)
        {
            DrawRope(grapplePoint);
            character.position = Vector3.MoveTowards(character.position, grapplePoint, pullSpeed * Time.deltaTime);
            if (Vector3.Distance(character.position, grapplePoint) < 1.5f) StopGrapple();
        }

        if (isPullingObject && targetBody != null)
        {
            DrawRope(targetBody.position);

            targetBody.position = Vector3.MoveTowards(targetBody.position, barrel.position, pullSpeed * Time.deltaTime);

            targetBody.linearVelocity = Vector3.zero;

            if (Vector3.Distance(targetBody.position, barrel.position) < 1.5f) StopGrapple();
        }


    }


    void UpdateCrosshairVisibility()
    {

        if (canGrappleCrosshair == null ) return;
        {
            if( !playerScript.PowerOn)
            {
                canGrappleCrosshair.enabled = false;
                return;

            }
            
            
            bool canGrapple = Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable);

            canGrappleCrosshair.enabled = canGrapple;

           

        }


    }
    void StartPullPlayer()
    {
        if (Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
        {
            if (hit.collider.CompareTag("GrapplePoint"))
            {
                canGrappleCrosshair.enabled = false;
                Grapple.enabled = true;
                grapplePoint = hit.point;
                isPullingPlayer = true;
                lineRender.positionCount = 2;
            }
            else
            {
                canGrappleCrosshair.enabled = true;
                Grapple.enabled = false;
                Pull.enabled = false;
            }
        }
    }
    void StartPullObject()
    {
        if ( Physics.Raycast(firstPersonCam.position, firstPersonCam.forward, out RaycastHit hit, maxDistance, Grappleable))
        {

            if (hit.collider.CompareTag("Pullable") && hit.collider.GetComponent<Rigidbody>())
            {
                canGrappleCrosshair.enabled = false;
                Grapple.enabled = true;
                targetBody = hit.collider.GetComponent<Rigidbody>();
                isPullingObject = true;
                lineRender.positionCount = 2;
            }

            else
            {
                canGrappleCrosshair.enabled = true;

                Grapple.enabled = false;
                Pull.enabled = false;

            }
        }
    }


    void StopGrapple()
    {
        lineRender.positionCount = 0;
        isPullingPlayer = false;
        isPullingObject = false;
        targetBody = null;
        lineRender.positionCount = 0;

    }


    void DrawRope(Vector3 targetPos)
    {
        lineRender.SetPosition(0, barrel.position);
        lineRender.SetPosition(1, targetPos);
    }
}
   