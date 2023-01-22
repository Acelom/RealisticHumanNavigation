using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateHeatmapFloor : MonoBehaviour
{
    private HeatmapInfo heatmap; 

    private int cellCountW;
    private int cellCountH;

    private float floorSizeW;
    private float floorSizeH;
    private float cellSizeW;
    private float cellSizeH;

    public List<GameObject> cells;

    public GameObject cell;

    private void Awake()
    {
        heatmap = GameObject.FindGameObjectWithTag("GameController").GetComponent<HeatmapInfo>();
        cellCountH = heatmap.gridSizeH;
        cellCountW = heatmap.gridSizeW;
        cells = new List<GameObject>();
        floorSizeW = transform.parent.localScale.x;
        floorSizeH = transform.parent.localScale.z;

        cellSizeW = floorSizeW / cellCountW;
        cellSizeH = floorSizeH / cellCountH;

        for (int i = 0; i < (cellCountH * cellCountW); i++)
        {
            cells.Add(Instantiate(cell, new Vector3(((floorSizeH / 2) - (cellSizeH / 2)) - (i / cellCountH) * cellSizeH,
                                                      transform.position.y,
                                                    ((floorSizeW / 2) - (cellSizeW / 2)) - (i % cellCountW) * cellSizeW),
                                                        Quaternion.identity, transform));
            cells[i].transform.localScale = new Vector3(cellSizeH, cellSizeW, 1);
            cells[i].transform.rotation = Quaternion.Euler(90, 0, 0);
            cells[i].GetComponent<CellInfo>().cellNumber = i; 
        }
        heatmap.cells = new List<GameObject>(cells); 
    }

}
