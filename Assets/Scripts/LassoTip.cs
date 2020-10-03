using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoTip : MonoBehaviour
{
    public delegate void LassoLatch(GameObject target);
    public static event LassoLatch OnLatched;

    void OnTriggerEnter(Collider other)
    {
        Debug.LogError("I'M TRIGGERED!!!!!!");

        if (other.gameObject.tag == "boi")
        {
            Debug.LogError("HIT!");
            LassoTip.OnLatched(other.gameObject);
        }
    }
}
