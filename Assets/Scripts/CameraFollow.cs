using System.Collections;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public static CameraFollow instance;

    public HorseController playerCharacter;

    private Camera thisCamera;
    private Transform cameraTransform;
    private float cameraDistance;

    private float verticalViewportThreshold = 0.3f;
    private float horizontalViewportThreshold = 0.4f;

    private static bool followPlayer = true;
    private static Vector3 showcasePoint;
    private static bool snapInitiated = false;
    private float snapSpeed = 0.3f;
    private float originalYValue;
    private bool returnZoomInitiated = false; 

    void Awake()
    {
        CameraFollow.instance = this;
        this.thisCamera = this.gameObject.GetComponentInChildren<Camera>();
        this.cameraTransform = this.gameObject.transform;
        this.cameraDistance = this.cameraTransform.position.y;
        this.originalYValue = this.transform.position.y;
    }

    private float GetDistance(Vector3 position1, Vector3 position2)
    {
        float xValue = Mathf.Pow(position2.x - position1.x, 2);
        float yValue = Mathf.Pow(position2.y - position1.y, 2);
        float zValue = Mathf.Pow(position2.z - position1.z, 2);

        return Mathf.Sqrt(xValue + yValue + zValue);
    }

    private bool IsPlayerPastHorizontalThreshold(float playerViewportXPosition)
    {
        return (playerViewportXPosition > (1.0f - this.horizontalViewportThreshold)) ||
            (playerViewportXPosition < (0.0f + this.horizontalViewportThreshold));
    }

    private bool IsPlayerPastVerticalThreshold(float playerViewportYPosition)
    {
        return (playerViewportYPosition > (1.0f - this.verticalViewportThreshold)) ||
            (playerViewportYPosition < (0.0f + this.verticalViewportThreshold));
    }

    void FixedUpdate()
    {
        //Normally follow the player
        if (followPlayer == true)
        {
            if (this.transform.position.y != this.originalYValue)
            {
                if (this.returnZoomInitiated == false)
                {
                    StartCoroutine(this.SnapToPoint(new Vector3(this.transform.position.x, this.originalYValue, this.transform.position.z)));
                    this.returnZoomInitiated = true;
                }
                return;
            }

            Vector3 playerViewportPosition = thisCamera.WorldToViewportPoint(this.playerCharacter.gameObject.transform.position);

            if (this.playerCharacter != null && this.IsPlayerPastVerticalThreshold(playerViewportPosition.y))
            {
                this.UpdateCameraVerticalPosition();
            }

            if (this.IsPlayerPastHorizontalThreshold(playerViewportPosition.x))
            {
                this.UpdateCameraHorizontalPosition();
            }
        }
        //Otherwise, focus on the midpoint between the player and a latched boi
        else
        {
            if (snapInitiated == false)
            {
                StartCoroutine(this.SnapToPoint(showcasePoint));
                snapInitiated = true;
            }
        }
    }

    private IEnumerator SnapToPoint(Vector3 targetPoint)
    {
        Debug.LogError("Current Camera Position: " + this.transform.position);
        Debug.LogError("Target Position: " + targetPoint);
        Debug.LogError("Distance: " + this.GetDistance(this.transform.position, targetPoint));

        while (this.GetDistance(this.transform.position, targetPoint) > 0.01f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPoint, this.snapSpeed);
            yield return null;
        }

        this.transform.position = targetPoint;
        this.returnZoomInitiated = false;
    }

    public static void InitiateShowcaseSnap(Vector3 targetPoint)
    {
        showcasePoint = targetPoint;
        followPlayer = false;
    }

    public static void ReturnToFollow()
    {
        showcasePoint = Vector3.zero;
        followPlayer = true;
        snapInitiated = false;
    }

    private void UpdateCameraVerticalPosition()
    {
        Vector3 worldSpaceCenteredPosition = this.thisCamera.ViewportToWorldPoint(new Vector3(0.5f, this.cameraDistance, this.verticalViewportThreshold));

        Vector3 shiftVector = new Vector3(0, 0, this.playerCharacter.transform.position.z - worldSpaceCenteredPosition.z);

        this.cameraTransform.Translate(shiftVector.normalized * Mathf.Abs(this.playerCharacter.Body.velocity.z) * Time.deltaTime);
    }

    private void UpdateCameraHorizontalPosition()
    {
        Vector3 worldSpaceCenteredPosition = this.thisCamera.ViewportToWorldPoint(new Vector3(0.5f, this.cameraDistance, this.verticalViewportThreshold));

        Vector3 shiftVector = new Vector3(this.playerCharacter.transform.position.x - worldSpaceCenteredPosition.x, 0, 0);

        this.cameraTransform.Translate(shiftVector.normalized * Mathf.Abs(this.playerCharacter.Body.velocity.x) * Time.deltaTime);
    }
}