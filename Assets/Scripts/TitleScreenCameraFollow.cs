using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleScreenCameraFollow : MonoBehaviour
{
    public static TitleScreenCameraFollow instance;

    public GameObject playerCharacter;

    private Camera thisCamera;
    private Transform cameraTransform;
    private float cameraDistance;

    public float verticalViewportThreshold = 0.48f;
    public float horizontalViewportThreshold = 0.48f;

    private static bool followPlayer = true;
    private static Vector3 showcasePoint;
    private static bool snapInitiated = false;
    private float snapSpeed = 0.3f;
    private float originalYValue;
    private bool returnZoomInitiated = false;
    private Coroutine SnapCoroutine;

    //Impact zoom stuff
    private float impactZoomAmount = 0.8f;
    private float impactZoomSpeed = 0.4f;
    private float impactReturnZoomSpeed = 0.01f;
    private float zoomedInYValue;
    private Coroutine ImpactZoomCoroutine;
    private Coroutine ImpactReturnCoroutine;
    private float cumulativeYZoom;

    private static float isometricZOffset;

    public float followSpeed = 10.0f;

    public FadePanelManager fadePanel;

    void Awake()
    {
        TitleScreenCameraFollow.instance = this;
        this.thisCamera = this.gameObject.GetComponentInChildren<Camera>();
        this.cameraTransform = this.gameObject.transform;
        this.cameraDistance = this.cameraTransform.position.y;
        this.originalYValue = this.transform.position.y;

        HorseController.OnPull += this.InitiateImpactZoom;

        isometricZOffset = this.GetIsometricZOffset();
    }

    private float GetIsometricZOffset()
    {
        return Camera.main.transform.position.y * Mathf.Tan(Camera.main.transform.rotation.x);
    }

    private void OnDestroy()
    {
        HorseController.OnPull -= this.InitiateImpactZoom;
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
        return (playerViewportXPosition >= (1.0f - this.horizontalViewportThreshold)) ||
            (playerViewportXPosition <= (0.0f + this.horizontalViewportThreshold));
    }

    private bool IsPlayerPastVerticalThreshold(float playerViewportYPosition)
    {
        return (playerViewportYPosition >= (1.0f - this.verticalViewportThreshold)) ||
            (playerViewportYPosition <= (0.0f + this.verticalViewportThreshold));
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
                    StopAllCoroutines();
                    this.SnapCoroutine = StartCoroutine(this.SnapToPoint(new Vector3(this.transform.position.x, this.originalYValue, this.transform.position.z)));
                    this.returnZoomInitiated = true;
                    this.cumulativeYZoom = 0;
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
                this.SnapCoroutine = StartCoroutine(this.SnapToPoint(showcasePoint));
                snapInitiated = true;
            }
        }
    }

    private IEnumerator SnapToPoint(Vector3 targetPoint)
    {
        targetPoint = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z);

        while (this.GetDistance(this.transform.position, targetPoint) > 0.01f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPoint, this.snapSpeed);
            yield return null;
        }

        this.transform.position = targetPoint;
        this.returnZoomInitiated = false;
        this.zoomedInYValue = this.transform.position.y;

        this.SnapCoroutine = null;
        this.ImpactZoomCoroutine = null;
        this.ImpactReturnCoroutine = null;
    }

    public static void InitiateShowcaseSnap(Vector3 targetPoint)
    {
        showcasePoint = new Vector3(targetPoint.x, targetPoint.y, targetPoint.z - (isometricZOffset / 2));
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

        this.cameraTransform.Translate(shiftVector.normalized * this.followSpeed * Time.deltaTime);
    }

    private void UpdateCameraHorizontalPosition()
    {
        Vector3 worldSpaceCenteredPosition = this.thisCamera.ViewportToWorldPoint(new Vector3(0.5f, this.cameraDistance, this.verticalViewportThreshold));

        Vector3 shiftVector = new Vector3(this.playerCharacter.transform.position.x - worldSpaceCenteredPosition.x, 0, 0);

        this.cameraTransform.Translate(shiftVector.normalized * this.followSpeed * Time.deltaTime);
    }

    private void InitiateImpactZoom()
    {
        if (this.ImpactZoomCoroutine == null && this.SnapCoroutine == null)
        {
            this.ImpactZoomCoroutine = StartCoroutine(this.ImpactZoom());
        }
    }

    private IEnumerator ImpactZoom()
    {
        if (this.ImpactReturnCoroutine != null)
        {
            StopCoroutine(this.ImpactReturnCoroutine);
        }

        this.cumulativeYZoom += this.impactZoomAmount;
        Vector3 targetPoint = new Vector3(showcasePoint.x, this.transform.position.y - this.impactZoomAmount, showcasePoint.z);

        while (Mathf.Abs(this.transform.position.y - targetPoint.y) > 0.01f)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPoint, this.impactZoomSpeed);
            yield return null;
        }

        this.transform.position = targetPoint;

        this.ImpactReturnCoroutine = StartCoroutine(this.ReturnFromImpactZoom());
        this.ImpactZoomCoroutine = null;
    }

    private IEnumerator ReturnFromImpactZoom()
    {
        Vector3 targetPoint = new Vector3(this.transform.position.x, this.zoomedInYValue, this.transform.position.z);

        while (this.transform.position.y < targetPoint.y)
        {
            this.transform.position = Vector3.Lerp(this.transform.position, targetPoint, this.impactReturnZoomSpeed);
            yield return null;
        }

        this.transform.position = targetPoint;
        this.ImpactReturnCoroutine = null;
    }

    public void OnStartGameButtonClicked()
    {
        this.fadePanel.OnFadeSequenceComplete += this.LoadTutorialScene;
        this.fadePanel.FadeToBlack();
    }

    private void LoadTutorialScene()
    {
        this.fadePanel.OnFadeSequenceComplete -= this.LoadTutorialScene;
        SceneManager.LoadScene(1);
    }
}