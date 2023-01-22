using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetDestinationPull : MonoBehaviour
{
    HeatmapInfo heatmap;
    public bool placed; 

    private void Awake()
    {
        heatmap = GameObject.FindGameObjectWithTag("GameController").GetComponent<HeatmapInfo>();
        placed = false; 
    }

    private void Update()
    {
        if (!placed)
        {
            GameObject nearest = null;
            float distance = Mathf.Infinity;

            foreach (GameObject go in heatmap.cells)
            {
                if (Vector3.Distance(go.transform.position, transform.position) < distance)
                {
                    nearest = go;
                    distance = Vector3.Distance(go.transform.position, transform.position);
                }
            }
            nearest.GetComponent<CellInfo>().destinationPullValue = 200;

            for (int l = 0; l < 200; l++)
            {
                foreach (GameObject go in heatmap.cells)
                {
                    CellInfo info = go.GetComponent<CellInfo>();
                    if (info.obstacleValue > 0)
                    {
                        for (int i = -1; i < 2; i++)
                        {
                            for (int j = -1; j < 2; j++)
                            {
                                if ((info.cellNumber + i) + (j * heatmap.gridSizeW) < heatmap.cells.Count && (info.cellNumber + i) + (j * heatmap.gridSizeW) > -1)
                                {
                                    if (heatmap.cells[(info.cellNumber + i) + (j * heatmap.gridSizeW)].GetComponent<CellInfo>().destinationPullValue > info.destinationPullValue)
                                    {
                                        info.destinationPullValue = heatmap.cells[(info.cellNumber + i) + (j * heatmap.gridSizeW)].GetComponent<CellInfo>().destinationPullValue - 1;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            placed = true; 

            /*foreach (GameObject go in heatmap.cells)
            {
                go.GetComponent<CellInfo>().destinationPullValue = Mathf.Clamp(200 - Mathf.RoundToInt(Vector3.Distance(transform.position, go.transform.position) * 2), 1, 200);
            }*/
        }
    }
}
