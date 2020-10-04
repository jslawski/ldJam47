using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomBoiGenerator : MonoBehaviour
{
    //BOX COLLIDER SPACE
    private float minFieldX;
    private float maxFieldX;
    private float minFieldZ;
    private float maxFieldZ;

    [SerializeField]
    private BoxCollider generationField;

    public GameObject baseBoiPrefab;

    public static int boiCount;
    private int maxNumBois = 5;

    private void Awake()
    {
        boiCount = 0;
        this.minFieldX = generationField.transform.position.x - (generationField.size.x / 2);
        this.maxFieldX = generationField.transform.position.x + (generationField.size.x / 2);
        this.minFieldZ = generationField.transform.position.z - (generationField.size.z / 2);
        this.maxFieldZ = generationField.transform.position.z + (generationField.size.z / 2);

    }

    private void Update()
    {
        if (boiCount < this.maxNumBois)
        {
            this.GenerateRandomBoi();
        }
    }

    public void GenerateRandomBoi()
    {
        float xValue = Random.Range(this.minFieldX, this.maxFieldX);
        float zValue = Random.Range(this.minFieldZ, this.maxFieldZ);

        Vector3 worldSpaceGenerationPosition = new Vector3(xValue, 0.0f, zValue);
        GameObject objectInstance = Instantiate(this.baseBoiPrefab, worldSpaceGenerationPosition, new Quaternion()) as GameObject;
        boiCount++;
    }
}