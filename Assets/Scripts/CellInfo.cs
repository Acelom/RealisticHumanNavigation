using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellInfo : MonoBehaviour
{
    public int obstacleValue;
    public int destinationPullValue;
    public int cellNumber; 

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Obstacle")
        {
            obstacleValue = 0;
        }
    }

    private void Update()
    {
        obstacleValue = 1;
    }
}
