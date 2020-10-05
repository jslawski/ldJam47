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
    private int maxNumBois = 20;

    private int totalBoiCount = 0;

    private void Awake()
    {
        boiCount = 0;
        totalBoiCount = 0;
        this.minFieldX = generationField.transform.position.x - (generationField.size.x / 2);
        this.maxFieldX = generationField.transform.position.x + (generationField.size.x / 2);
        this.minFieldZ = generationField.transform.position.z - (generationField.size.z / 2);
        this.maxFieldZ = generationField.transform.position.z + (generationField.size.z / 2);

        this.GenerateInitialBois();
    }

    private void GenerateInitialBois()
    {
        for (int i = 0; i < this.maxNumBois; i++)
        {
            GenerateEasyPinkie();
        }
    }

    private void Update()
    {
        if (boiCount < this.maxNumBois)
        {
            this.GenerateRandomBoi();
        }
    }

    private void GenerateEasyPinkie()
    {
        float xValue = Random.Range(this.minFieldX, this.maxFieldX);
        float zValue = Random.Range(this.minFieldZ, this.maxFieldZ);
        
        this.baseBoiPrefab.GetComponent<BaseBoi>().boiStats = Resources.Load<BoiStats>("Bois/EasyPinkie");

        Vector3 worldSpaceGenerationPosition = new Vector3(xValue, 4.0f, zValue);
        GameObject objectInstance = Instantiate(this.baseBoiPrefab, worldSpaceGenerationPosition, new Quaternion()) as GameObject;
        boiCount++;
    }

    public void GenerateRandomBoi()
    {
        float xValue = Random.Range(this.minFieldX, this.maxFieldX);
        float zValue = Random.Range(this.minFieldZ, this.maxFieldZ);

        totalBoiCount++;

        //Spawn a golden boi 1 every 12
        if (totalBoiCount % 12 == 0)
        {
            this.baseBoiPrefab.GetComponent<BaseBoi>().boiStats = Resources.Load<BoiStats>("Bois/GoldenBoi");
        }
        //Spawn a beefcake boi 1 every 5
        else if (totalBoiCount % 5 == 0)
        {
            this.baseBoiPrefab.GetComponent<BaseBoi>().boiStats = Resources.Load<BoiStats>("Bois/Beefcake");
        }
        else
        {
            this.baseBoiPrefab.GetComponent<BaseBoi>().boiStats = Resources.Load<BoiStats>("Bois/EasyPinkie");
        }

        Debug.LogError("Boi #" + totalBoiCount + " is a " + this.baseBoiPrefab.GetComponent<BaseBoi>().boiStats.boiName);

        Vector3 worldSpaceGenerationPosition = new Vector3(xValue, 4.0f, zValue);
        GameObject objectInstance = Instantiate(this.baseBoiPrefab, worldSpaceGenerationPosition, new Quaternion()) as GameObject;
        boiCount++;
    }
}