using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class QueenSpawn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        QueenSpawnGrow();
    }

    void QueenSpawnGrow()
    {
        if (this.transform.localScale.x < 1.0f)
        {
            float growth_rate = Random.Range(0.1f, 0.5f);
            this.transform.localScale += new Vector3(growth_rate * Time.deltaTime, growth_rate * Time.deltaTime, growth_rate * Time.deltaTime);
        }
    } 
}
