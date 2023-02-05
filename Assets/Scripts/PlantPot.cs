using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantPot : MonoBehaviour
{
    public Transform startPos;
    public Transform endPos;
    public GameObject stem;
    public float rateOfGrowth = 1f;
    public float totalWatered = 0f;
    [HideInInspector]
    public float watered = 0f;

    // Start is called before the first frame update
    void Start()
    {
        totalWatered = 0f;
        watered = 0f;
        stem.transform.position = startPos.position;
    }

    // Update is called once per frame
    void Update()
    {
        totalWatered += watered * rateOfGrowth * Time.deltaTime;
        stem.transform.position = Vector3.Lerp(startPos.position, endPos.position, totalWatered);
        watered = 0.0f;
    }
}
