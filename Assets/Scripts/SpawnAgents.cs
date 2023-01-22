using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAgents : MonoBehaviour
{
    CamControls controls;

    private float timer;
    public float timeLimit = 1;
    public GameObject agent; 


    private void Awake()
    {
        controls = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<CamControls>(); 
    }

    private void Update()
    {
        if (controls.play)
        {
            if (timer > timeLimit)
            {
                Instantiate(agent, new Vector3(transform.position.x, 1, transform.position.z), Quaternion.identity, null);
                timer = 0; 
            }
        }

        timer += Time.deltaTime; 
    }
}
