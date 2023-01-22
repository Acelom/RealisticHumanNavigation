using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RespawnAgent : MonoBehaviour
{
    private List<GameObject> agents;
    public Transform spawn; 

    private void Awake()
    {
        agents = new List<GameObject>(GameObject.FindGameObjectsWithTag("Agent"));
    }

    private void Update()
    {
        foreach (GameObject go in agents)
        {
            if (Vector3.Distance(go.transform.position, transform.position) < 3)
            {
                AIAgent agent = go.GetComponent<AIAgent>();
                for (int i = 0; i < GameObject.FindGameObjectWithTag("GameController").GetComponent<HeatmapInfo>().gridSizeH; i++)
                {
                    for (int j = 0; j < GameObject.FindGameObjectWithTag("GameController").GetComponent<HeatmapInfo>().gridSizeW; j++)
                    {
                        agent.alreadyVisitedHeatmap[i, j] = 1;
                        agent.newDest = false;
                        agent.directRouteCalculated = false;
                        agent.arrived = true; 
                    }
                }

                agent.transform.position = spawn.position;
            }
        }
    }
}
