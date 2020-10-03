using UnityEngine;

public class HorseController : MonoBehaviour
{
    [SerializeField, Range(0, 100)]
    private float maxSpeed = 10f;
    [SerializeField, Range(0, 100)]
    private float maxAcceleration = 30f;
    [SerializeField, Range(0, 100)]
    private float maxDeceleration = 10f;

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

    private void PauseVelocityMovement()
    {
        this.Body.velocity = Vector3.zero;
        this.Body.angularVelocity = Vector3.zero;
    }

    private void Start()
    {
        this.Body = GetComponent<Rigidbody>();
    }

    private void AccelerateTowardsDesiredVelocity(Vector3 desiredVelocity, float maxSpeedChange)
    {
        this.playerVelocity.x = Mathf.MoveTowards(this.playerVelocity.x, desiredVelocity.x, maxSpeedChange);
        this.playerVelocity.z = Mathf.MoveTowards(this.playerVelocity.z, desiredVelocity.z, maxSpeedChange);
    }

    private void Update()
    {
        this.playerVelocity = this.Body.velocity;
        Vector3 compositeDirectionVector = this.GetCompositeDirectionVector();

        this.desiredPlayerVelocity = compositeDirectionVector.normalized * this.maxSpeed;
        this.desiredMaxSpeedChange = this.maxAcceleration;

        if (Input.GetKeyDown(KeyCode.LeftShift))
       {
            GameObject lassoInstance = Instantiate(this.lassoObject, this.transform.position, new Quaternion(), this.transform);
            lassoInstance.GetComponent<LassoHook>().FireLasso(this.transform, Vector3.zero, 1.0f);
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