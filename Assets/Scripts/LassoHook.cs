using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoHook : MonoBehaviour
{
    public float maxDistance = 10f;
    public float maxSpeed = 5f;
    //public LineRenderer lassoLine;
    public GameObject lassoTip;

    [Range(0, 1)]
    public float testLaunchMagnitude;
    public Vector3 testLaunchVector;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    private float getDistance(Vector3 position1, Vector3 position2)
    {
        float xValue = Mathf.Pow(position2.x - position1.x, 2);
        float zValue = Mathf.Pow(position2.z - position1.z, 2);

        return Mathf.Sqrt(xValue + zValue);
    }

    public void TestLassoFire()
    {
        this.FireLasso(testLaunchVector, testLaunchMagnitude);
    }

    public void FireLasso(Vector3 launchVector, float normalizedLaunchMagnitude)
    {
        float launchDistance = this.maxDistance * normalizedLaunchMagnitude;

        StartCoroutine(LaunchLassoTip(launchVector, launchDistance));
    }

    private IEnumerator LaunchLassoTip(Vector3 launchVector, float launchDistance)
    {
        float totalIterations = 0;

        //Launch lasso out
        while (Mathf.Abs(this.getDistance(this.transform.position, this.lassoTip.transform.position)) < launchDistance)
        {
            this.lassoTip.transform.Translate(launchVector.normalized * this.maxSpeed * Time.fixedDeltaTime);
            totalIterations++;
            yield return null;
        }

        //Pull lasso in
        //This is dumb, don't judge me
        for (int i = 0; i < totalIterations; i++)
        {
            this.lassoTip.transform.Translate(-launchVector.normalized * this.maxSpeed * Time.fixedDeltaTime);
            yield return null;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            this.TestLassoFire();
        }
    }
}
