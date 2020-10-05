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
    [SerializeField]
    private Transform aimingLineOriginTransform;

    private BaseBoi latchedBoi;
    private string latchedBoiName;

    public delegate void PullInitiated();
    public static event PullInitiated OnPull;

    public Animator animatorStateMachine;
    public SpriteRenderer characterSprite;

    [SerializeField]
    private ParticleSystem pullPointsParticles;
    [SerializeField]
    private ParticleSystem wrangledPointsParticles;

    public AudioSource wrangledSound;
    public AudioSource coinSound;
    public AudioSource rareBoiSound;
    public AudioSource lassoThrowSound;
    public AudioSource stepSound;

    private Coroutine stepSoundCoroutine;

    public float GetRandomSoundPitch()
    {
        return Random.Range(0.9f, 1.1f);
    }

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

    //Gets midpoint between this game object and another designated position
    private Vector3 GetMidpoint(Vector3 otherObjectPosition)
    {
        return new Vector3((this.transform.position.x + otherObjectPosition.x) / 2, Camera.main.transform.parent.transform.position.y, (this.transform.position.z + otherObjectPosition.z) / 2);
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
        this.HandleAnimations();

        if (this.playerVelocity.magnitude > 0 && this.stepSoundCoroutine == null)
        {
            this.stepSoundCoroutine = StartCoroutine(this.PlayStepSounds());
        }

        if (GameManager.gameFinished == true)
        {
            this.PauseVelocityMovement();
            return;
        }

        this.aimingLine.SetPosition(0, this.aimingLineOriginTransform.position);
        this.aimingLine.SetPosition(1, this.aimingLineOriginTransform.position);

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

    private void HandleAnimations()
    {
        if (this.playerVelocity.magnitude > 0)
        {
            this.animatorStateMachine.SetBool("moving", true);
            if (playerVelocity.x > 0)
            {
                this.characterSprite.flipX = true;
            }
            else
            {
                this.characterSprite.flipX = false;
            }
        }
        else
        {
            this.animatorStateMachine.SetBool("moving", false);
        }

        if (Input.GetMouseButton(0) || this.lassoInstance != null)
        {
            this.animatorStateMachine.SetBool("aimingLasso", true);
        }
        else
        {
            this.animatorStateMachine.SetBool("aimingLasso", false);
        }

        if (this.currentPlayerState == PlayerState.Latched)
        {
            this.animatorStateMachine.SetBool("latched", true);
        }
        else
        {
            this.animatorStateMachine.SetBool("latched", false);
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
            //This is really stupid and I'm sorry
            if (GameManager.instance == null || GameManager.instance.preGameState == false)
            {
                StartCoroutine(HandleAiming(Camera.main.ScreenToViewportPoint(Input.mousePosition)));
            }
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

            this.aimingLine.SetPosition(1, this.aimingLine.GetPosition(0) + finalAimDirection* 30);

            yield return null;
        }
        if (normalizedMagnitude > 0.1f)
        {
            this.aimingLine.SetPosition(1, this.aimingLine.GetPosition(0));
            this.LaunchLasso(finalAimDirection, normalizedMagnitude);
        }
    }

    private void LaunchLasso(Vector3 aimDirection, float magnitude)
    {
        this.lassoInstance = Instantiate(this.lassoObject, this.transform.position, new Quaternion(), this.transform);
        this.lassoInstance.GetComponent<LassoHook>().FireLasso(this.transform, aimDirection, magnitude);

        this.lassoThrowSound.pitch = this.GetRandomSoundPitch();
        this.lassoThrowSound.Play();
    }

    private void HandleLatchControls()
    {
        this.latchedBoi.OnCapture += LatchDisengaged;

        if (Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            this.HandleScore();
            this.latchedBoi.PullTowardsPosition(this.transform.position);

            //Latched boi could be destroyed after the most recent pull
            //Make sure it isn't before you try to do an impact zoom 
            if (this.latchedBoi != null)
            {
                //Just do this to update the midpoint
                CameraFollow.InitiateShowcaseSnap(this.GetMidpoint(this.latchedBoi.transform.position));
                if (OnPull != null)
                {
                    OnPull();
                }
            }
        }
    }

    private void HandleScore()
    {
        this.pullPointsParticles.Stop();
        this.pullPointsParticles.Play();

        this.coinSound.pitch = this.GetRandomSoundPitch();
        this.coinSound.Play();

        if (GameManager.instance != null)
        {
            GameManager.instance.IncrementScore(2);
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
        //Only latch one boi at a time
        if (this.latchedBoi != null)
        {
            return;
        }

        this.PauseVelocityMovement();
        this.currentPlayerState = PlayerState.Latched;
        this.latchedBoi = target.GetComponent<BaseBoi>();
        this.latchedBoi.OnCapture += LatchDisengaged;
        this.latchedBoi.StopAllCoroutines();
        this.latchedBoi.SignalLatch();
        this.latchedBoiName = this.latchedBoi.boiStats.name;
        CameraFollow.InitiateShowcaseSnap(this.GetMidpoint(target.transform.position));

        this.wrangledSound.pitch = this.GetRandomSoundPitch();
        this.wrangledSound.Play();
    }

    private void LatchDisengaged()
    {
        ParticleSystemRenderer particleRenderer = this.wrangledPointsParticles.GetComponent<ParticleSystemRenderer>();
        //this.latchedBoi.OnCapture -= LatchDisengaged;
        switch (this.latchedBoiName)
        {
            case "EasyPinkie":
            case "TutorialBoi":
                particleRenderer.material = Resources.Load<Material>("Materials/Plus10");
                break;
            case "GoldenBoi":
                this.rareBoiSound.Play();
                particleRenderer.material = Resources.Load<Material>("Materials/GoldBar");
                break;
            case "Beefcake":
                this.rareBoiSound.Play();
                particleRenderer.material = Resources.Load<Material>("Materials/MoneyBag");
                break;
            default:
                Debug.LogError("Error: Unknown Boi Type=" + this.latchedBoi.boiStats.boiName);
                break;
        }

        this.latchedBoi = null;
        this.currentPlayerState = PlayerState.Moving;
        CameraFollow.ReturnToFollow();

        this.wrangledPointsParticles.Play();
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

    private IEnumerator PlayStepSounds()
    {
        while (this.playerVelocity.magnitude > 0)
        {
            this.stepSound.pitch = this.GetRandomSoundPitch();
            this.stepSound.Play();
            yield return new WaitForSeconds(0.3f);
        }

        this.stepSoundCoroutine = null;
    }
}