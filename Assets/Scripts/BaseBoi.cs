using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseBoi : MonoBehaviour
{
    protected float moveSpeed;
    protected int pointValue;
    protected float minDistancePulledPerClick;
    protected float maxDistancePulledPerClick;
    protected string boiName;

    public delegate void BoiCaptured();
    public event BoiCaptured OnCapture;

    private float GetDistance(Vector3 position1, Vector3 position2)
    {
        float xValue = Mathf.Pow(position2.x - position1.x, 2);
        float zValue = Mathf.Pow(position2.z - position1.z, 2);

        return Mathf.Sqrt(xValue + zValue);
    }

    public void PullTowardsPosition(Vector3 targetPosition)
    {
        Vector3 targetDirection = targetPosition - this.transform.position;
        float distancePulled = Random.Range(minDistancePulledPerClick, maxDistancePulledPerClick);

        this.transform.Translate(targetDirection.normalized * distancePulled);

        if (this.GetDistance(this.transform.position, targetPosition) < 0.5f)
        {
            this.OnCapture();
            Destroy(this.gameObject);
            Debug.LogError("EARNED " + this.pointValue + " POINTS!");
            //Assign Point value
        }
    }
}
