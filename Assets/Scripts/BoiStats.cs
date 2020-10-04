using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Boi Stats", menuName = "BoiStats")]
public class BoiStats : ScriptableObject
{
    public int pointValue;
    public float minDistancePulledPerClick;
    public float maxDistancePulledPerClick;
    public string boiName;

    //Movement Stuff
    public float moveSpeed;
    public float minTimeSpentMoving;
    public float maxTimeSpentMoving;
    public float minTimeSpentIdling;
    public float maxTimeSpentIdling;
    public float minTimeBeforeDirectionChange;
    public float maxTimeBeforeDirectionChange;

    //SpriteStuff
    public string animatorName;

    public float GetRandomDistancePulled()
    {
        return Random.Range(minDistancePulledPerClick, maxDistancePulledPerClick);
    }

    public float GetRandomTimeSpentMoving()
    {
        return Random.Range(minTimeSpentMoving, maxTimeSpentMoving);
    }

    public float GetRandomTimeSpentIdling()
    {
        return Random.Range(minTimeSpentIdling, maxTimeSpentIdling);
    }

    public float GetRandomTimeBeforeDirectionChange()
    {
        return Random.Range(minTimeBeforeDirectionChange, maxTimeBeforeDirectionChange);
    }
}
