using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Maze : MonoBehaviour
{
    [System.Serializable]
    public class Cell
    {
        public bool visited;
        public GameObject north;
        public GameObject east;
        public GameObject west;
        public GameObject south;
    }
    public GameObject finishPos;
    public GameObject startPos;
    private int startint = 0;
   
    public GameObject wall;
    //public GameObject Plane;
    public float wallLength = 1.0f;
    public int xSize = 5;
    public int ySize = 5;
    private Vector3 initialPos;
    private GameObject wallHolder;
    private Cell[] cells;
    private int currentCell = 0;
    private int totalCells;
    private int visitedCells = 0;
    private bool startedBuilding = false;

    private int currentNeighbor = 0;
    private List<int> lastCells;
    private int backingUp = 0;
    private int wallToBreak = 0;
    private float starttime = 0;
    public Button sbutton;
    public Joystick joysticks;
    public Slider xsli;
    public Slider ysli;
    public Text xText;
    public Text yText;
    public Text timeis;
    public Image nmsa;
    public Image nmsa2;
    string key = "Timertime";
    public Camera cam;



    // Start is called before the first frame update
    void Start()
    { 
        starttime = Time.time;
    }
    public void startbutton()
    {
        
        startint = 1;
        sbutton.gameObject.SetActive(false);
        joysticks.gameObject.SetActive(true);
    }
    float max(float a, float b)
    {
        if (a <= b)
            return b;
        else
            return a;
    }
    void Update()
    {
        Vector3 screenPos = new Vector3(0.25f, max(xSize, ySize)*1.0f, 0f);
        cam.transform.position = screenPos;
        xText.text = "x=" + xSize.ToString();
        yText.text = "y="+ySize.ToString();
        xSize = (int)xsli.value;
        ySize = (int)ysli.value;
        if (startint==1)
        {
            timeis.gameObject.SetActive(true);
            xsli.gameObject.SetActive(false);
            ysli.gameObject.SetActive(false);
            xText.gameObject.SetActive(false);
            yText.gameObject.SetActive(false);
            nmsa.gameObject.SetActive(false);
            nmsa2.gameObject.SetActive(false);
            // Plane.SetActive(true);
            Vector3 planepos = new Vector3(0, -0.5f, 0);
           // Plane.transform.localPosition = planepos;
           // Plane.transform.localScale *= xSize * 10;
            CreateWalls();
            startint++;
        }
        if(startint>0)
        {
            float t = Time.time - starttime;
            string minutes = ((int)t / 60).ToString();
            string seconds = ((int)t % 60).ToString();
            timeis.text = "TIME\n" + minutes + ":" + seconds;
            PlayerPrefs.SetFloat(key, t);
        }
    }
    void CreateWalls()
    {
        wallHolder = new GameObject();
        wallHolder.name = "Maze";

        


        initialPos = new Vector3((-xSize / 2) + wallLength/2, 0.0f, (-ySize / 2) + wallLength / 2);
        Vector3 myPos = initialPos;
        GameObject tempWall;

        for(int i=0;i<ySize;i++)
        {
            for(int j=0;j<=xSize;j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.0f, initialPos.z + (i * wallLength) - wallLength / 2);
                tempWall = Instantiate(wall, myPos, Quaternion.identity)as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }
        for (int i = 0; i <= ySize; i++)
        {
            for (int j = 0; j < xSize; j++)
            {
                myPos = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength);
                tempWall=Instantiate(wall, myPos, Quaternion.Euler(0.0f,90.0f,0.0f))as GameObject;
                tempWall.transform.parent = wallHolder.transform;
                if(i==ySize&&j==xSize-1)
                {
                    Vector3 finPose = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength-0.5f);
                    finishPos.SetActive(true);
                    finishPos.transform.localPosition = finPose;
                }
                if (i == 0 && j == 0)
                {
                    Vector3 startPose = new Vector3(initialPos.x + (j * wallLength), 0.0f, initialPos.z + (i * wallLength) - wallLength + 0.5f);
                    startPos.SetActive(true);
                    startPos.transform.localPosition = startPose;
                }
            }
        }

        CreateCells();
    }

    void CreateCells()
    {
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;
        GameObject[] allWalls;
        int children = wallHolder.transform.childCount;
        allWalls = new GameObject[children];
        cells = new Cell[xSize * ySize];
        int eastWestProcess = 0;
        int childProcess = 0;
        int termCount = 0;
        for(int i=0;i<children;i++)
        {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }    
        for(int cellprocess =0; cellprocess<cells.Length;cellprocess++)
        {
            if (termCount == xSize)
            {
                eastWestProcess++;
                termCount = 0;
            }

            cells[cellprocess] = new Cell();
            cells[cellprocess].east = allWalls[eastWestProcess];
            cells[cellprocess].south = allWalls[childProcess + (xSize + 1) * ySize];

                eastWestProcess++;

            termCount++;
            childProcess++;
            cells[cellprocess].west= allWalls[eastWestProcess];
            cells[cellprocess].north = allWalls[(childProcess + (xSize + 1) * ySize)+xSize-1];
        }
        CreateMaze();
    }

    void CreateMaze()
    {
        while(visitedCells<totalCells)
        {
            if (startedBuilding)
            {
                GiveMeNeighbor();
                if (cells[currentNeighbor].visited == false&& cells[currentCell].visited == true)
                {
                    BreakWall();
                    cells[currentNeighbor].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbor;
                    if(lastCells.Count>0)
                    {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else
            {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }
        }
    }
    void BreakWall()
    {
         switch(wallToBreak)
        {
            case 1:
                Destroy(cells[currentCell].north);
                break;
            case 2:
                Destroy(cells[currentCell].east);
                break;
            case 3:
                Destroy(cells[currentCell].west);
                break;
            case 4:
                Destroy(cells[currentCell].south);
                break;
        }
    }

    void GiveMeNeighbor()
    {
       
        int length = 0;
        int[] neighbors = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;
        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;

        //west
        if (currentCell + 1 < totalCells && (currentCell + 1) != check)
        {
            if (cells[currentCell + 1].visited == false)
            {
                neighbors[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }
        //east
        if (currentCell - 1 >= 0 && currentCell != check)
        {
            if (cells[currentCell - 1].visited == false)
            {
                neighbors[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }
        //north
        if (currentCell +xSize < totalCells)
        {
            if (cells[currentCell +xSize].visited == false)
            {
                neighbors[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }
        //south
        if (currentCell - xSize >= 0)
        {
            if (cells[currentCell - xSize].visited == false)
            {
                neighbors[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }
        if(length!=0)
        {
            int theChosenOne = Random.Range(0, length);
            currentNeighbor = neighbors[theChosenOne];
            wallToBreak = connectingWall[theChosenOne];
        }
        else
        {
            if(backingUp>0)
            {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }

    }
    
    // Update is called once per frame
    
}
