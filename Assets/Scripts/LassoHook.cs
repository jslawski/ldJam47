using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoHook : MonoBehaviour
{
    public float maxDistance = 10f;
    public float maxSpeed = 5f;
    public LineRenderer lassoLine;
    public GameObject lassoTip;

    [Range(0, 1)]
    public float testLaunchMagnitude;
    public Vector3 testLaunchVector;

    private Transform playerTransform;

    private BaseBoi latchedBoi;

    // Start is called before the first frame update
    void Start()
    {
        this.lassoLine.SetPosition(0, this.transform.position);
        this.lassoLine.SetPosition(1, this.lassoTip.transform.position);
        LassoTip.OnLatched -= this.InitiateLatch;
        LassoTip.OnLatched += this.InitiateLatch;
    }

    private void OnDestroy()
    {
        LassoTip.OnLatched -= this.InitiateLatch;
    }

    private float GetDistance(Vector3 position1, Vector3 position2)
    {
        float xValue = Mathf.Pow(position2.x - position1.x, 2);
        float zValue = Mathf.Pow(position2.z - position1.z, 2);

        return Mathf.Sqrt(xValue + zValue);
    }

    public void FireLasso(Transform playerTransform, Vector3 launchVector, float normalizedLaunchMagnitude)
    {
        float launchDistance = this.maxDistance * normalizedLaunchMagnitude;

        this.playerTransform = playerTransform;

        StartCoroutine(LaunchLassoTip(launchVector, normalizedLaunchMagnitude * this.maxDistance));
    }

    private IEnumerator LaunchLassoTip(Vector3 launchVector, float launchDistance)
    {
        float totalIterations = 0;

        //Launch lasso out
        while (Mathf.Abs(this.GetDistance(this.transform.position, this.lassoTip.transform.position)) < launchDistance)
        {
            this.lassoTip.transform.Translate(launchVector.normalized * this.maxSpeed * Time.fixedDeltaTime);
            totalIterations++;
            this.lassoLine.SetPosition(0, this.transform.position);
            this.lassoLine.SetPosition(1, this.lassoTip.transform.position);
            yield return null;
        }

        //Pull lasso in
        //This is dumb, don't judge me
        while (Mathf.Abs(this.GetDistance(this.transform.position, this.lassoTip.transform.position)) > 0.1f)
        {
            Vector3 returnVector = this.lassoTip.transform.position - this.playerTransform.position;

            this.lassoTip.transform.Translate(-returnVector.normalized * this.maxSpeed * Time.fixedDeltaTime);
            this.lassoLine.SetPosition(0, this.transform.position);
            this.lassoLine.SetPosition(1, this.lassoTip.transform.position);
            yield return null;
        }

        Destroy(this.gameObject);
    }

    private void InitiateLatch(GameObject target)
    {
        this.StopAllCoroutines();
        this.lassoLine.SetPosition(0, this.transform.position);
        this.lassoLine.SetPosition(1, target.transform.position);
        Destroy(this.lassoTip);

        this.latchedBoi = target.GetComponent<BaseBoi>();

        this.latchedBoi.OnCapture += this.DestroyLasso;
    }

    private void DestroyLasso()
    {
        this.latchedBoi.OnCapture -= this.DestroyLasso;
        Destroy(this.gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        this.lassoLine.SetPosition(0, this.playerTransform.position);

        if (this.latchedBoi != null)
        {
            this.lassoLine.SetPosition(1, this.latchedBoi.transform.position);
        }
    }
}
