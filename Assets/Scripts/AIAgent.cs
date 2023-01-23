using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class AIAgent : MonoBehaviour
{
    private HeatmapInfo heatmap;
    private GameObject visionCone;
    private List<GameObject> currColliders;

    public int countdownLoss = 1;
    public int retentionLoss = 1;
    public int memoryLoss = 1;
    public int retentionStart = 10;

    public float viewDistance = 20;
    public float viewWidth = 10;
    public int[,] memoryHeatmap;
    public float[,] memoryRetentionHeatmap;
    public int[,] memoryCountdownHeatmap;
    public float[,] alreadyVisitedHeatmap;
    public float visitDistance = 5;
    public int[,] combinedMap;
    private List<GameObject> weightedRandomChoice;
    private List<int> weightsForRandomChoice;

    public bool newDest;
    public bool directRouteCalculated;
    public bool pathCalculated; 
    public bool arrived;
    private Vector3 dest;
    private int totalWeighting;
    private int rotationCounter; 




    private void Awake()
    {
        newDest = true;
        heatmap = GameObject.FindGameObjectWithTag("GameController").GetComponent<HeatmapInfo>();
        memoryHeatmap = new int[heatmap.gridSizeH, heatmap.gridSizeW];
        memoryRetentionHeatmap = new float[heatmap.gridSizeH, heatmap.gridSizeW];
        memoryCountdownHeatmap = new int[heatmap.gridSizeH, heatmap.gridSizeW];
        alreadyVisitedHeatmap = new float[heatmap.gridSizeH, heatmap.gridSizeW];
        combinedMap = new int[heatmap.gridSizeH, heatmap.gridSizeW];

        for (int i = 0; i < memoryRetentionHeatmap.GetLength(0); i++)
        {
            for (int j = 0; j < memoryRetentionHeatmap.GetLength(0); j++)
            {
                memoryHeatmap[i, j] = 0;
                memoryRetentionHeatmap[i, j] = retentionStart;
                memoryCountdownHeatmap[i, j] = retentionStart;
                alreadyVisitedHeatmap[i, j] = 1;
            }
        }
    }

    private void Start()
    {
        visionCone = transform.GetChild(0).gameObject;
        visionCone.transform.localScale = new Vector3(1, viewDistance, viewWidth);
        visionCone.transform.localPosition = new Vector3(viewWidth / 2, -1, viewDistance);

        List<Collider> startingNearbyColliders = new List<Collider>(Physics.OverlapSphere(transform.position, (viewDistance * 2) / 3));
        foreach (Collider col in startingNearbyColliders)
        {
            if (col.tag == "Cell")
            {
                Vector2Int temp = ListToGrid(col.gameObject.GetComponent<CellInfo>().cellNumber);
                memoryHeatmap[temp.x, temp.y] = 200;
            }
        }
    }

    private void Update()
    {
        currColliders = visionCone.GetComponent<ConeControl>().currColliders;
        UpdateMemoryHeatmap();
        CalculateDestination();
        CheckPathToDestination();
        MoveToDestination();
        LookAround(); 
    }

    private void CalculateDestination()
    {
        if (newDest)
        {
            weightedRandomChoice = new List<GameObject>();
            weightsForRandomChoice = new List<int>();
            totalWeighting = 0;
            for (int i = 0; i < heatmap.cells.Count; i++)
            {
                Vector2Int temp = ListToGrid(i);
                if (memoryHeatmap[temp.x, temp.y] > 0 && combinedMap[temp.x, temp.y] > 0)
                {

                    //calculates the weighting of each position using the combined map and the distance from the agent, so that further cells are prioritized. 
                    int weighting = combinedMap[temp.x, temp.y] -
                        Mathf.RoundToInt(combinedMap[temp.x, temp.y] * Mathf.Clamp(Mathf.InverseLerp(0, combinedMap[temp.x, temp.y], Mathf.RoundToInt(Vector3.Distance(transform.position, heatmap.cells[i].transform.position)) * 5),
                        0, combinedMap[temp.x, temp.y]));
                    weighting = (int)(weighting * (1 + (Vector3.Angle(-transform.forward, heatmap.cells[i].transform.position - transform.position)) / 720));
                    weighting = Mathf.RoundToInt(Mathf.Pow(Mathf.Ceil(weighting / 10), 2));

                    if (heatmap.cells[i].GetComponent<CellInfo>().destinationPullValue == 200)
                    {
                        weighting = weighting * 100; 
                    }

                    if (weighting > 0)
                    {
                        weightedRandomChoice.Add(heatmap.cells[i]);
                        weightsForRandomChoice.Add(weighting);
                        totalWeighting += weighting;
                    }

                }

            }

            int random = Random.Range(0, totalWeighting);
            if (weightsForRandomChoice.Count > 0)
            {
                do
                {
                    if (random >= weightsForRandomChoice[0])
                    {
                        random = random - weightsForRandomChoice[0];
                        weightsForRandomChoice.Remove(weightsForRandomChoice[0]);
                        weightedRandomChoice.Remove(weightedRandomChoice[0]);
                    }
                }
                while (random >= weightsForRandomChoice[0]);
                dest = weightedRandomChoice[0].transform.position;

                newDest = false;
            }
        }
    }

    private void CheckPathToDestination()
    {
        if (!arrived)
        {
            RaycastHit hit;
            Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            Vector3 direction = new Vector3(dest.x, transform.position.y + 3, dest.z) - rayOrigin;
            float rayLength = Vector3.Distance(new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), new Vector3(dest.x, transform.position.y + 3, dest.z));
            if (!Physics.SphereCast(rayOrigin, 0.5f, direction, out hit, rayLength))
            {
                transform.LookAt(dest + Vector3.up);
                directRouteCalculated = true;
            }
            else
            {
                if (IsConnected())
                {
                    CalculatePathToDestination();
                }
                else
                {
                    newDest = true; 
                }
            }
        }
    }

    private void CalculatePathToDestination()
    {

    }

    private void MoveToDestination()
    {
        if (directRouteCalculated)
        {
            transform.position += (transform.forward);
        }

        if (Vector3.Distance(transform.position - Vector3.up, dest) < 1)
        {
            arrived = true;
            directRouteCalculated = false; 
        }

    }

    private void LookAround()
    {
        if (arrived)
        {
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y + viewWidth, transform.eulerAngles.z);
            rotationCounter += 1;
            if (rotationCounter == 360/viewWidth)
            {
                arrived = false; 
                newDest = true;
                rotationCounter = 0; 
            }
        }
    }

    private bool IsConnected()
    {
        return false; 
    }

    private Vector2Int ListToGrid(int z)
    {
        int x = z / memoryHeatmap.GetLength(0);
        int y = z % memoryHeatmap.GetLength(1);

        return new Vector2Int(y, x);
    }

    // todo reset stuff not on the list
    private void UpdateMemoryHeatmap()
    {
        for (int i = 0; i < currColliders.Count; i++)
        {
            Vector2Int temp = ListToGrid(currColliders[i].GetComponent<CellInfo>().cellNumber);
            GameObject cellTemp = heatmap.cells[currColliders[i].GetComponent<CellInfo>().cellNumber];
            RaycastHit hit;
            Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 3, transform.position.z);
            Vector3 direction = new Vector3(cellTemp.transform.position.x, transform.position.y + 3, cellTemp.transform.position.z) - rayOrigin;
            float rayLength = Vector3.Distance(new Vector3(transform.position.x, transform.position.y + 3, transform.position.z), new Vector3(cellTemp.transform.position.x, transform.position.y + 3, cellTemp.transform.position.z));
            if (!Physics.SphereCast(rayOrigin, 0.5f, direction, out hit, rayLength))
            {
                memoryHeatmap[temp.x, temp.y] = 200;
                memoryRetentionHeatmap[temp.x, temp.y] = retentionStart;
                memoryCountdownHeatmap[temp.x, temp.y] = retentionStart;
            }
        }

        for (int i = 0; i < heatmap.cells.Count - 1; i++)
        {
            Vector2Int temp = ListToGrid(heatmap.cells[i].GetComponent<CellInfo>().cellNumber);
            if (memoryHeatmap[temp.x, temp.y] > 0)
            {
                if (memoryRetentionHeatmap[temp.x, temp.y] < 1)
                {
                    memoryHeatmap[temp.x, temp.y] -= memoryLoss;
                    memoryRetentionHeatmap[temp.x, temp.y] = memoryCountdownHeatmap[temp.x, temp.y];
                    memoryCountdownHeatmap[temp.x, temp.y] -= countdownLoss;
                }
                else
                {
                    memoryRetentionHeatmap[temp.x, temp.y] -= retentionLoss;
                }
            }
            else
            {
                alreadyVisitedHeatmap[temp.x, temp.y] = 1; 
            }

            CellInfo cellTemp = heatmap.cells[i].GetComponent<CellInfo>();
            if (Vector3.Distance(transform.position, heatmap.cells[i].transform.position) < visitDistance)
            {
                alreadyVisitedHeatmap[temp.x, temp.y] = 0.5f;
            }


            combinedMap[temp.x, temp.y] = Mathf.RoundToInt(Mathf.CeilToInt(Mathf.InverseLerp(0, 200, memoryHeatmap[temp.x, temp.y]) * (heatmap.obstacleAndPullMap[temp.x, temp.y])) * alreadyVisitedHeatmap[temp.x, temp.y]);
        }


    }
}
