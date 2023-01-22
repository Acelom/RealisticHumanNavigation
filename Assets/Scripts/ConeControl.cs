using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeControl : MonoBehaviour
{
    public List<GameObject> currColliders;

    private void Awake()
    {
        currColliders = new List<GameObject>(); 
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Cell" && !currColliders.Contains(other.gameObject))
        {
            currColliders.Add(other.gameObject); 
        }
    }

    private void FixedUpdate()
    {
        currColliders = new List<GameObject>(); 
    }
}
