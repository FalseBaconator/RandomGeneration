using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DungeonMaker : MonoBehaviour
{

    public GameObject[] RoomPrefabs;

    Intersection[,] mainFloor = new Intersection[25,25];
    Intersection[,] lowerFloor = new Intersection[25,25];

    int currentRun;
    public int maxRuns;

    public float RoomSize;

    List<Intersection> possibleGoals;

    public void QuitGame()
    {
        Application.Quit();
    }

    public void ResetGame()
    {
        Debug.Log("RESET");
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    struct Intersection
    {
        public bool NorthDoor;
        public bool LNorthDoor;
        public bool SouthDoor;
        public bool LSouthDoor;
        public bool EastDoor;
        public bool LEastDoor;
        public bool WestDoor;
        public bool LWestDoor;
        public bool Start;
        public bool Empty;
        public bool isLower;
        public bool isStairs;
        public bool isGoal;

        public Intersection(bool N, bool S, bool E, bool W, bool LN, bool LS, bool LE, bool LW, bool start, bool lower)
        {
            NorthDoor = N;
            LNorthDoor = LN;
            SouthDoor = S;
            LSouthDoor = LS;
            EastDoor = E;
            LEastDoor = LE;
            WestDoor = W;
            LWestDoor = LW;
            Start = start;
            Empty = false;
            isLower = lower;
            if (LNorthDoor || LSouthDoor || LWestDoor || LEastDoor)
            {
                isStairs = true;
            }
            else isStairs = false;
            isGoal = false;
        }

        public Intersection(bool empty)
        {
            NorthDoor = false;
            LNorthDoor = false;
            SouthDoor = false;
            LSouthDoor = false;
            EastDoor = false;
            LEastDoor= false;
            WestDoor = false;
            LWestDoor = false;
            Start = false;
            Empty = empty;
            isLower = false;
            isStairs = false;
            isGoal = false;
        }

        public int GetDoorCount()
        {
            int count = 0;
            if (NorthDoor) count++;
            if (SouthDoor) count++;
            if (EastDoor) count++;
            if (WestDoor) count++;
            if (LNorthDoor) count++;
            if (LSouthDoor) count++;
            if (LEastDoor) count++;
            if (LWestDoor) count++;
            return count;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        possibleGoals = new List<Intersection>();
        Time.timeScale = 1;
        for (int i = 0; i < mainFloor.GetLength(0); i++)
        {
            for (int j = 0; j < mainFloor.GetLength(1); j++)
            {
                mainFloor[i, j] = new Intersection(true);
            }
        }

        for (int i = 0; i < lowerFloor.GetLength(0); i++)
        {
            for (int j = 0; j < lowerFloor.GetLength(1); j++)
            {
                lowerFloor[i, j] = new Intersection(true);
            }
        }

        //Determine Intersections
        mainFloor[12, 12] = new Intersection(true, true, true, true, false, false, false, false, true, false);
        currentRun = 0;
        
        while (currentRun <= maxRuns)
        {
            currentRun++;
            List<int> toGenXMain = new List<int>();
            List<int> toGenXLower = new List<int>();
            List<int> toGenYMain = new List<int>();
            List<int> toGenYLower = new List<int>();

            for (int i = 0; i < mainFloor.GetLength(0); i++)
            {
                for (int j = 0; j < mainFloor.GetLength(1); j++)
                {
                    if (mainFloor[i, j].Empty && IsAdjacent(i, j, false))
                    {
                        toGenXMain.Add(i);
                        toGenYMain.Add(j);
                    }
                }
            }

            for (int i = 0; i < lowerFloor.GetLength(0); i++)
            {
                for (int j = 0; j < lowerFloor.GetLength(1); j++)
                {
                    if (lowerFloor[i, j].Empty && mainFloor[i, j].isStairs == false && IsAdjacent(i, j, true))
                    {
                        //Debug.Log("TEST");
                        toGenXLower.Add(i);
                        toGenYLower.Add(j);
                    }
                }
            }

            for (int i = 0; i < toGenXMain.Count; i++)
            {
                GenerateIntersection(toGenXMain[i], toGenYMain[i], false);
            }
            for (int i = 0; i < toGenXLower.Count; i++)
            {
                GenerateIntersection(toGenXLower[i], toGenYLower[i], true);
            }

        }

        int lowerRooms = 0;
        for (int i = 0; i < lowerFloor.GetLength(0); i++)
        {
            for (int j = 0; j < lowerFloor.GetLength(1); j++)
            {
                if (!lowerFloor[i, j].Empty) lowerRooms++;
            }
        }
        //Debug.Log(lowerRooms);

        //Place Goal
        //How Many Viable Rooms
        int count = 0;
        for (int i = 0; i < mainFloor.GetLength(0); i++)
        {
            for (int j = 0; j < mainFloor.GetLength(1); j++)
            {
                if (mainFloor[i, j].GetDoorCount() == 1) count++;
                if (lowerFloor[i, j].GetDoorCount() == 1) count++;
            }
        }
        //Pick One of Said Rooms if there are enough
        Debug.Log("Count: " + count);
        if (count == 0) ResetGame();
        int Index = Random.Range(0, count-1);
        //Assign the chosen room as goal
        for (int i = 0; i < mainFloor.GetLength(0); i++)
        {
            for (int j = 0; j < mainFloor.GetLength(1); j++)
            {
                if (mainFloor[i, j].GetDoorCount() == 1)
                {
                    if (Index == 0)
                    {
                        mainFloor[i, j].isGoal = true;
                        Debug.Log(mainFloor[i, j].isGoal);
                    }
                    Index--;
                }
            }
        }
        if(Index > 0)
        {
            for (int i = 0; i < lowerFloor.GetLength(0); i++)
            {
                for (int j = 0; j < lowerFloor.GetLength(1); j++)
                {
                    if (lowerFloor[i, j].GetDoorCount() == 1)
                    {
                        if (Index == 0)
                        {
                            lowerFloor[i, j].isGoal = true;
                            Debug.Log(lowerFloor[i, j].isGoal);
                        }
                        Index--;
                    }
                }
            }
        }

        //Generate Rooms
        for (int i = 0; i < mainFloor.GetLength(0); i++)
        {
            for (int j = 0; j < mainFloor.GetLength(1); j++)
            {
                if (mainFloor[i,j].Empty == false)
                {
                    SpawnRoom(i, j, false);
                }
            }
        }
        for (int i = 0; i < lowerFloor.GetLength(0); i++)
        {
            for (int j = 0; j < lowerFloor.GetLength(1); j++)
            {
                if (lowerFloor[i, j].Empty == false)
                {
                    SpawnRoom(i, j, true);
                }
            }
        }

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.N)) ResetGame();
    }

    void GenerateIntersection(int x, int y, bool isLower)
    {
        bool N = false;
        bool LN = false;
        bool S = false;
        bool LS = false;
        bool E = false;
        bool LE = false;
        bool W = false;
        bool LW = false;

        if (isLower == false) //Main Floor
        {
            //Determine North
            if (y < 24)
            {
                if (mainFloor[x, y + 1].Empty)
                {
                    int temp = Random.Range(0, 2);
                    if (currentRun < maxRuns)
                        N = (temp == 0) ? false : true;
                    else
                        N = false;
                }
                else
                {
                    N = mainFloor[x, y + 1].SouthDoor;
                }
            }
            else
            {
                N = false;
            }

            //Determine South
            if (y > 0)
            {
                if (mainFloor[x, y - 1].Empty)
                {
                    int temp = Random.Range(0, 2);
                    if (currentRun < maxRuns)
                        S = (temp == 0) ? false : true;
                    else
                        S = false;
                }
                else
                {
                    S = mainFloor[x, y - 1].NorthDoor;
                }
            }
            else
            {
                S = false;
            }

            //Determine East
            if (x < 24)
            {
                if (mainFloor[x + 1, y].Empty)
                {
                    int temp = Random.Range(0, 2);
                    if (currentRun < maxRuns)
                        E = (temp == 0) ? false : true;
                    else
                        E = false;
                }
                else
                {
                    E = mainFloor[x + 1, y].WestDoor;
                }
            }
            else
            {
                E = false;
            }

            //Determine West
            if (x > 0)
            {
                if (mainFloor[x - 1, y].Empty)
                {
                    int temp = Random.Range(0, 2);
                    if (currentRun < maxRuns)
                        W = (temp == 0) ? false : true;
                    else
                        W = false;
                }
                else
                {
                    W = mainFloor[x - 1, y].EastDoor;
                }
            }
            else
            {
                W = false;
            }

            int amountOfDoorsOpen = 0;
            if (N) amountOfDoorsOpen++;
            if (S) amountOfDoorsOpen++;
            if (E) amountOfDoorsOpen++;
            if (W) amountOfDoorsOpen++;

            //Determine Stairs
            if (currentRun < maxRuns && lowerFloor[x, y].Empty)
            {
                //stairs
                if (amountOfDoorsOpen == 1)
                {
                    int temp = Random.Range(0, 2);
                    if (temp == 0)
                    {
                        if (N && isSpaceOccupiedLower(x, y - 1) == false) LS = true;
                        if (S && isSpaceOccupiedLower(x, y + 1) == false) LN = true;
                        if (E && isSpaceOccupiedLower(x + 1, y) == false) LW = true;
                        if (W && isSpaceOccupiedLower(x - 1, y) == false) LE = true;
                    }
                }
            }


            if (!N && !S && !E && !W && !LN && !LS && !LE && !LW)
            {
                mainFloor[x, y] = new Intersection(true);
            }
            else
            {
                mainFloor[x, y] = new Intersection(N, S, E, W, LN, LS, LE, LW, false, false);
            }
        }
        else //Lower Floor
        {
            //Determine North
            if (y < 24)
            {
                if (isSpaceOccupiedLower(x, y + 1) == false)
                {
                    int temp = Random.Range(0, 2);
                    if (currentRun < maxRuns)
                        N = (temp == 0) ? false : true;
                    else
                        N = false;
                }
                else if (lowerFloor[x, y+1].Empty == false)
                {
                    N = lowerFloor[x, y + 1].SouthDoor;
                }
                else
                {
                    N = mainFloor[x, y + 1].LSouthDoor;
                }
            }
            else
            {
                N = false;
            }

            //Determine South
            if (y > 0)
            {
                if (isSpaceOccupiedLower(x, y - 1) == false)
                {
                    int temp = Random.Range(0, 2);
                    if (currentRun < maxRuns)
                        S = (temp == 0) ? false : true;
                    else
                        S = false;
                }
                else if (lowerFloor[x, y - 1].Empty == false)
                {
                    S = lowerFloor[x, y - 1].NorthDoor;
                }
                else
                {
                    S = mainFloor[x, y - 1].LNorthDoor;
                }
            }
            else
            {
                S = false;
            }

            //Determine East
            if (x < 24)
            {
                if (isSpaceOccupiedLower(x + 1, y) == false)
                {
                    int temp = Random.Range(0, 2);
                    if (currentRun < maxRuns)
                        E = (temp == 0) ? false : true;
                    else
                        E = false;
                }
                else if (lowerFloor[x + 1, y].Empty == false)
                {
                    E = lowerFloor[x + 1, y].WestDoor;
                }
                else if (mainFloor[x + 1, y].isStairs)
                {
                    E = mainFloor[x + 1, y].LWestDoor;
                }
            }
            else
            {
                E = false;
            }

            //Determine West
            if (x > 0)
            {
                if (isSpaceOccupiedLower(x - 1, y) == false)
                {
                    int temp = Random.Range(0, 2);
                    if (currentRun < maxRuns)
                        W = (temp == 0) ? false : true;
                    else
                        W = false;
                }
                else if (lowerFloor[x - 1, y].Empty == false)
                {
                    W = lowerFloor[x - 1, y].EastDoor;
                }
                else
                {
                    W = mainFloor[x - 1, y].LEastDoor;
                }
            }
            else
            {
                W = false;
            }

            if (N == false && S == false && E == false && W == false)
            {
                lowerFloor[x, y] = new Intersection(true);
            }
            else
            {
                lowerFloor[x, y] = new Intersection(N, S, E, W, LN, LS, LE, LW, false, true);
            }
        }

    }

    bool isSpaceOccupiedLower(int x, int y)
    {
        if (!lowerFloor[x, y].Empty) return true;
        if (mainFloor[x, y].isStairs) return true;
        return false;
    }

    bool IsAdjacent(int x, int y, bool isLower)
    {
        if (isLower == false)
        {
            if (y > 0)
            {
                if (mainFloor[x, y - 1].NorthDoor) return true;
            }
            if (y < 24)
            {
                if (mainFloor[x, y + 1].SouthDoor) return true;
            }
            if (x < 24)
            {
                if (mainFloor[x + 1, y].WestDoor) return true;
            }
            if (x > 0)
            {
                if (mainFloor[x - 1, y].EastDoor) return true;
            }
        }
        else
        {
            if (y > 0)
            {
                if (lowerFloor[x, y - 1].NorthDoor || mainFloor[x, y-1].LNorthDoor) return true;
            }
            if (y < 24)
            {
                if (lowerFloor[x, y + 1].SouthDoor || mainFloor[x, y + 1].LSouthDoor) return true;
            }
            if (x < 24)
            {
                if (lowerFloor[x + 1, y].WestDoor || mainFloor[x + 1, y].LWestDoor) return true;
            }
            if (x > 0)
            {
                if (lowerFloor[x - 1, y].EastDoor || mainFloor[x - 1, y].LEastDoor) return true;
            }
        }
        return false;
    }

    void SpawnRoom(int x, int y, bool isLower)
    {
        List<GameObject> tempRooms = new List<GameObject>();

        Intersection section;
        if(isLower)
            section = lowerFloor[x, y];
        else
            section = mainFloor[x, y];

        foreach (GameObject obj in RoomPrefabs)
        {
            Room room = obj.GetComponent<Room>();
            if(room.NorthDoor == section.NorthDoor)
            {
                if(room.SouthDoor == section.SouthDoor)
                {
                    if(room.EastDoor == section.EastDoor)
                    {
                        if(room.WestDoor == section.WestDoor)
                        {
                            //tempRooms.Add(obj);
                            if(room.lowerNorthDoor == section.LNorthDoor)
                            {
                                if(room.lowerSouthDoor == section.LSouthDoor)
                                {
                                    if(room.lowerEastDoor == section.LEastDoor)
                                    {
                                        if(room.lowerWestDoor == section.LWestDoor)
                                        {
                                            if(room.hasGoal == section.isGoal)
                                                tempRooms.Add(obj);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        //if (section.Start) Debug.Log(x + "," + y);
        //if (section.Start) Debug.Log(x * RoomSize + "," + y * RoomSize);
        if (tempRooms.Count > 0)
        {
            GameObject tempRoom = Instantiate(tempRooms[Random.Range(0, tempRooms.Count)]);
            if (section.isLower)
                tempRoom.transform.position = new Vector3(x * RoomSize, -11, y * RoomSize);
            else
                tempRoom.transform.position = new Vector3(x * RoomSize, 0, y * RoomSize);
        }
        else
        {
            Debug.Log("FAIL");
        }

    }

}
