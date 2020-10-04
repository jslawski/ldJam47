using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PolarCoordinates;

public class BaseBoi : MonoBehaviour
{
    public BoiStats boiStats;

    public delegate void BoiCaptured();
    public event BoiCaptured OnCapture;
    private Vector3 currentMoveDirection;

    public LayerMask barrierLayerMask;

    public Animator characterAnimator;
    public SpriteRenderer characterSprite;

    public ParticleSystem goldenBoiSparkle;

    private void Start()
    {
        this.characterAnimator.runtimeAnimatorController = Resources.Load<RuntimeAnimatorController>("Animators/" + this.boiStats.animatorName);

        if (this.boiStats.animatorName == "GBAnimator")
        {
            this.goldenBoiSparkle.Play();
        }

        StartCoroutine(this.MoveInDirection());
    }

    //Use this to handle animation stuff
    private void Update()
    {
        if (this.currentMoveDirection.x > 0)
        {
            this.characterSprite.flipX = true;
        }
        else
        {
            this.characterSprite.flipX = false;
        }

        if (this.currentMoveDirection.magnitude > 0)
        {
            this.characterAnimator.SetBool("moving", true);
        }
        else
        {
            this.characterAnimator.SetBool("moving", false);
        }
    }

    private Vector3 GetRandomMoveDirection()
    {
        if (this.currentMoveDirection == null)
        {
            float randomXValue = Random.Range(-1.0f, 1.0f);
            float randomZValue = Random.Range(-1.0f, 1.0f);
            return new Vector3(randomXValue, 0, randomZValue).normalized;
        }
        else //Pick a new vector that is significantly different from the original direction vector
        {
            PolarCoordinate polarDirectionVector = new PolarCoordinate(this.currentMoveDirection, Orientation.XZ);
            float randomAngle = Random.Range((Mathf.PI / 6.0f), ((Mathf.PI * 11.0f) / 6.0f));
            PolarCoordinate newDirectionVector = new PolarCoordinate(1.0f, randomAngle + polarDirectionVector.angleInRadians);
            return newDirectionVector.PolarToCartesian(Orientation.XZ).normalized;
        }
    }

    private IEnumerator MoveInDirection()
    {
        this.currentMoveDirection = this.GetRandomMoveDirection();
        float totalTimeMoving = this.boiStats.GetRandomTimeSpentMoving();
        float timeBeforeDirectionChange = this.boiStats.GetRandomTimeBeforeDirectionChange();

        for (float i = 0; i < totalTimeMoving; i += Time.deltaTime)
        {
            this.transform.Translate(this.currentMoveDirection * this.boiStats.moveSpeed * Time.deltaTime);
            if (timeBeforeDirectionChange <= 0)
            {
                this.currentMoveDirection = this.GetRandomMoveDirection();
                timeBeforeDirectionChange = this.boiStats.GetRandomTimeBeforeDirectionChange();
            }
            timeBeforeDirectionChange -= Time.deltaTime;
            yield return null;
        }

        this.currentMoveDirection = Vector3.zero;
        StartCoroutine(this.StayIdle());
    }

    private IEnumerator StayIdle()
    {
        float timeSpentIdle = this.boiStats.GetRandomTimeSpentIdling();

        for (float i = 0; i < timeSpentIdle; i += Time.deltaTime)
        {
            yield return null;
        }

        StartCoroutine(this.MoveInDirection());
    }

    private float GetDistance(Vector3 position1, Vector3 position2)
    {
        float xValue = Mathf.Pow(position2.x - position1.x, 2);
        float zValue = Mathf.Pow(position2.z - position1.z, 2);

        return Mathf.Sqrt(xValue + zValue);
    }

    public void SignalLatch()
    {
        this.characterAnimator.SetBool("latched", true);
        StopAllCoroutines();
    }

    public void PullTowardsPosition(Vector3 targetPosition)
    {
        Vector3 targetDirection = targetPosition - this.transform.position;
        float distancePulled = this.boiStats.GetRandomDistancePulled();

        this.transform.Translate(targetDirection.normalized * distancePulled);

        if (this.GetDistance(this.transform.position, targetPosition) < 0.5f)
        {
            this.OnCapture();
            RandomBoiGenerator.boiCount--;
            if (GameManager.instance != null)
            { 
                GameManager.instance.IncrementScore(this.boiStats.pointValue);
            }
            Destroy(this.gameObject);
        }
    }

    //Change direction if a boi enters a barrier collider
    public void OnCollisionEnter(Collision collision)
    {
        this.currentMoveDirection = -this.currentMoveDirection;
    }
}
