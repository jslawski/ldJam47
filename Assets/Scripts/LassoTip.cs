using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LassoTip : MonoBehaviour
{
    public delegate void LassoLatch(GameObject target);
    public static event LassoLatch OnLatched;

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "boi")
        {
            LassoTip.OnLatched(other.gameObject);
        }
    }
}
