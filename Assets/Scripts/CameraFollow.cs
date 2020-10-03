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

    void Awake()
    {
        CameraFollow.instance = this;
        this.thisCamera = this.gameObject.GetComponentInChildren<Camera>();
        this.cameraTransform = this.gameObject.transform;
        this.cameraDistance = this.cameraTransform.position.y;
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