using System.Collections;
using UnityEngine;
using PolarCoordinates;

public class HorseController : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private float maxSpeed = 10f;
    [SerializeField, Range(0, 100)]
    private float maxAcceleration = 30f;
    [SerializeField, Range(0, 100)]
    private float maxDeceleration = 20f;

    private Vector3 playerVelocity;
    private Vector3 desiredPlayerVelocity;
    private float desiredMaxSpeedChange;

    public Rigidbody Body { get; private set; }

    [SerializeField]
    private KeyCode upKey;
    [SerializeField]
    private KeyCode downKey;
    [SerializeField]
    private KeyCode leftKey;
    [SerializeField]
    private KeyCode rightKey;

    [SerializeField]
    private GameObject lassoObject;
    private GameObject lassoInstance;

    private enum PlayerState { Moving, Latched };
    private PlayerState currentPlayerState;

    private float maxSlingshotLaunchRadius = 0.3f;

    [SerializeField]
    private LineRenderer aimingLine;

    private BaseBoi latchedBoi;

    private void PauseVelocityMovement()
    {
        this.Body.velocity = Vector3.zero;
        this.Body.angularVelocity = Vector3.zero;
        this.desiredPlayerVelocity = Vector3.zero;
        this.desiredMaxSpeedChange = 0;
        this.playerVelocity = Vector3.zero;
    }

    private float GetDistance(Vector3 position1, Vector3 position2)
    {
        float xValue = Mathf.Pow(position2.x - position1.x, 2);
        float yValue = Mathf.Pow(position2.y - position1.y, 2);

        return Mathf.Sqrt(xValue + yValue);
    }

    private void Start()
    {
        this.Body = GetComponent<Rigidbody>();
        this.currentPlayerState = PlayerState.Moving;
        LassoTip.OnLatched += this.LatchInitiated;
    }

    private void OnDestroy()
    {
        LassoTip.OnLatched -= this.LatchInitiated;
    }

    private void AccelerateTowardsDesiredVelocity(Vector3 desiredVelocity, float maxSpeedChange)
    {
        this.playerVelocity.x = Mathf.MoveTowards(this.playerVelocity.x, desiredVelocity.x, maxSpeedChange);
        this.playerVelocity.z = Mathf.MoveTowards(this.playerVelocity.z, desiredVelocity.z, maxSpeedChange);
    }

    private void Update()
    {
        if (GameManager.gameFinished == true)
        {
            this.PauseVelocityMovement();
            return;
        }

        this.aimingLine.SetPosition(0, this.transform.position);
        this.aimingLine.SetPosition(1, this.transform.position);

        switch (this.currentPlayerState)
        {
            case PlayerState.Moving:
                this.HandlePlayerMovement();
                this.HandlePlayerAiming();
                break;
            case PlayerState.Latched:
                this.HandleLatchControls();
                break;
            default:
                Debug.LogError("ERROR: Invalid Player State=" + this.currentPlayerState);
                break;
        }
    }

    private void HandlePlayerMovement()
    {
        this.playerVelocity = this.Body.velocity;
        Vector3 compositeDirectionVector = this.GetCompositeDirectionVector();

        this.desiredPlayerVelocity = compositeDirectionVector.normalized * this.maxSpeed;
        this.desiredMaxSpeedChange = this.maxAcceleration;
    }

    private void HandlePlayerAiming()
    {
        if (Input.GetMouseButtonDown(0) && this.lassoInstance == null)
        {
            StartCoroutine(HandleAiming(Camera.main.ScreenToViewportPoint(Input.mousePosition)));
        }
    }

    private IEnumerator HandleAiming(Vector3 startingPosition)
    {
        float normalizedMagnitude = 0;
        Vector3 finalAimDirection = Vector3.zero;

        while (Input.GetMouseButton(0))
        {
            Vector3 currentMousePosition = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            Vector3 positionDifference = currentMousePosition - startingPosition;

            float currentRadius = this.maxSlingshotLaunchRadius;

            if (positionDifference.magnitude < this.maxSlingshotLaunchRadius)
            {
                currentRadius = positionDifference.magnitude;
            }

            PolarCoordinate polarPositionDifference = new PolarCoordinate(currentRadius, positionDifference);

            Vector3 endPosition = startingPosition + polarPositionDifference.PolarToCartesian();

            //These are reverse because the launch direction will be the opposite direction of the slingshot
            Vector3 aimDirection = startingPosition - endPosition;
            finalAimDirection = new Vector3(aimDirection.x, 0, aimDirection.y);
            normalizedMagnitude = this.GetDistance(startingPosition, endPosition) / this.maxSlingshotLaunchRadius;

            this.aimingLine.SetPosition(1, this.aimingLine.GetPosition(0) + finalAimDirection* 10);

            yield return null;
        }

        this.aimingLine.SetPosition(1, this.aimingLine.GetPosition(0));
        this.LaunchLasso(finalAimDirection, normalizedMagnitude);
    }

    private void LaunchLasso(Vector3 aimDirection, float magnitude)
    {
        this.lassoInstance = Instantiate(this.lassoObject, this.transform.position, new Quaternion(), this.transform);
        this.lassoInstance.GetComponent<LassoHook>().FireLasso(this.transform, aimDirection, magnitude);
    }

    private void HandleLatchControls()
    {
        this.latchedBoi.OnCapture += LatchDisengaged;
        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            GameManager.instance.IncrementScore(2);
            this.latchedBoi.PullTowardsPosition(this.transform.position);
        }
    }

    private Vector3 GetCompositeDirectionVector()
    {
        Vector3 compositeVector = new Vector3();

        if (Input.GetKey(this.upKey))
        {
            compositeVector += Vector3.forward;
        }
        if (Input.GetKey(this.downKey))
        {
            compositeVector += Vector3.back;
        }
        if (Input.GetKey(this.leftKey))
        {
            compositeVector += Vector3.left;
        }
        if (Input.GetKey(this.rightKey))
        {
            compositeVector += Vector3.right;
        }

        return compositeVector;
    }

    private void LatchInitiated(GameObject target)
    {
        this.PauseVelocityMovement();
        this.currentPlayerState = PlayerState.Latched;
        this.latchedBoi = target.GetComponent<BaseBoi>();
        this.latchedBoi.OnCapture += LatchDisengaged;
        this.latchedBoi.StopAllCoroutines();
    }

    private void LatchDisengaged()
    {
        //this.latchedBoi.OnCapture -= LatchDisengaged;
        this.latchedBoi = null;
        this.currentPlayerState = PlayerState.Moving;
    }

    private void FixedUpdate()
    {
        float maxSpeedChange = this.desiredMaxSpeedChange * Time.deltaTime;

        this.AccelerateTowardsDesiredVelocity(this.desiredPlayerVelocity, maxSpeedChange);
        if (this.playerVelocity.magnitude < 0)
        {
            this.playerVelocity = Vector3.zero;
        }

        this.Body.velocity = this.playerVelocity;
    }
}