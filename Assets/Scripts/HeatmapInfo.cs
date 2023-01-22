using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeatmapInfo : MonoBehaviour
{
    public int gridSizeW;
    public int gridSizeH;

    public int[,] obstacleMap;
    public int[,] destinationPullMap;
    public int[,] obstacleAndPullMap;
    public int[,] firstAgentMemoryHeatmap; 
    public int[,] combinedMap; 
    public List<GameObject> cells;

    private void Awake()
    {
        obstacleMap = new int[gridSizeH, gridSizeW];
        destinationPullMap = new int[gridSizeH, gridSizeW];
        obstacleAndPullMap = new int[gridSizeH, gridSizeW];
        firstAgentMemoryHeatmap = new int[gridSizeH, gridSizeW];
        combinedMap = new int[gridSizeH, gridSizeW]; 

        for (int i = 0; i < gridSizeH; i++)
        {
            for (int j = 0; j < gridSizeW; j++)
            {
                obstacleMap[i, j] = 1;
                destinationPullMap[i, j] = 1;
            }
        }
    }

    public int GridToList(int x, int y)
    {
        int z = (y * gridSizeW) + x;
        return z;
    }

    private void Update()
    {
        UpdateHeatMaps();
        if (GameObject.FindGameObjectWithTag("Agent"))
        {
            ColourHeatMap(GameObject.FindGameObjectsWithTag("Agent")[0].GetComponent<AIAgent>().combinedMap);
        }

    }

    private void UpdateHeatMaps()
    {
        for (int i = 0; i < gridSizeH; i++)
        {
            for (int j = 0; j < gridSizeW; j++)
            {
                CellInfo temp = cells[GridToList(i, j)].GetComponent<CellInfo>();
               
                obstacleMap[i, j] = temp.obstacleValue;
                destinationPullMap[i, j] = temp.destinationPullValue;
                obstacleAndPullMap[i, j] = Mathf.Clamp((temp.obstacleValue * temp.destinationPullValue), 0, 200);
                if (GameObject.FindGameObjectWithTag("Agent"))
                {
                    firstAgentMemoryHeatmap = GameObject.FindGameObjectsWithTag("Agent")[0].GetComponent<AIAgent>().memoryHeatmap;
                    int memoryTemp = firstAgentMemoryHeatmap[i, j];

                    combinedMap[i, j] = Mathf.CeilToInt(Mathf.InverseLerp(0, 200, memoryTemp) * obstacleAndPullMap[i,j]);
                }
            }
        }
    }

    private void ColourHeatMap(int[,] map)
    {
        for (int i = 0; i < gridSizeH; i++)
        {
            for (int j = 0; j < gridSizeW; j++)
            {
                Color colour = new Color(0,0,0); 
                if (map[i, j] != 0)
                {
                    float newColour = Mathf.InverseLerp(0, 200, map[i, j]);
                    colour = new Color(newColour, 0, 1 - newColour); 
                }
                else
                {
                    colour = Color.green; 
                }
                cells[GridToList(i, j)].GetComponent<Renderer>().material.color = colour;
            }
        }
    }
}
