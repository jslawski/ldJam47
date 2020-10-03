using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EasyPinkie : BaseBoi
{
    // Start is called before the first frame update
    void Start()
    {
        this.pointValue = 5;
        this.minDistancePulledPerClick = 0.5f;
        this.maxDistancePulledPerClick = 1.0f;
    }
}
