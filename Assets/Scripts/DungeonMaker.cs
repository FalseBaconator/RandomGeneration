using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaker : MonoBehaviour
{

    public GameObject[] RoomPrefabs;

    Intersection[,] intersections = new Intersection[25,25];

    int currentRun;
    public int maxRuns;

    public float RoomSize;

    struct Intersection
    {
        public bool NorthDoor;
        public bool SouthDoor;
        public bool EastDoor;
        public bool WestDoor;
        public bool Start;
        public bool Empty;

        public Intersection(bool N, bool S, bool E, bool W, bool start)
        {
            NorthDoor = N;
            SouthDoor = S;
            EastDoor = E;
            WestDoor = W;
            Start = start;
            Empty = false;
        }

        public Intersection(bool empty)
        {
            NorthDoor = false;
            SouthDoor = false;
            EastDoor = false;
            WestDoor = false;
            Start = false;
            Empty = empty;
        }

    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < intersections.GetLength(0); i++)
        {
            for (int j = 0; j < intersections.GetLength(1); j++)
            {
                intersections[i, j] = new Intersection(true);
            }
        }

        intersections[12, 12] = new Intersection(true, true, true, true, true);
        currentRun = 0;

        while (currentRun <= maxRuns)
        {
            Debug.Log(intersections[12, 12].Start);
            currentRun++;
            List<int> toGenX = new List<int>();
            List<int> toGenY = new List<int>();
            for (int i = 0; i < intersections.GetLength(0); i++)
            {
                for (int j = 0; j < intersections.GetLength(1); j++)
                {
                    if (intersections[i, j].Empty && IsAdjacent(i, j))
                    {
                        toGenX.Add(i);
                        toGenY.Add(j);
                    }
                }
            }

            for (int i = 0; i < toGenX.Count; i++)
            {
                GenerateIntersection(toGenX[i], toGenY[i]);
            }

        }

        for (int i = 0; i < intersections.GetLength(0); i++)
        {
            for (int j = 0; j < intersections.GetLength(1); j++)
            {
                if (intersections[i,j].Empty == false)
                {
                    SpawnRoom(i, j);
                }
            }
        }

    }

    void GenerateIntersection(int x, int y)
    {
        bool N;
        bool S;
        bool E;
        bool W;

        //Determine North
        if (y < 24)
        {
            if (intersections[x, y + 1].Empty)
            {
                int temp = Random.Range(0, 2);
                if (currentRun < maxRuns)
                    N = (temp == 0) ? false : true;
                else
                    N = false;
            }
            else
            {
                N = intersections[x, y + 1].SouthDoor;
            }
        }
        else
        {
            N = false;
        }

        //Determine South
        if (y > 0)
        {
            if (intersections[x, y - 1].Empty)
            {
                int temp = Random.Range(0, 2);
                if (currentRun < maxRuns)
                    S = (temp == 0) ? false : true;
                else
                    S = false;
            }
            else
            {
                S = intersections[x, y - 1].NorthDoor;
            }
        }
        else
        {
            S = false;
        }

        //Determine East
        if (x < 24)
        {
            if (intersections[x + 1, y].Empty)
            {
                int temp = Random.Range(0, 2);
                if (currentRun < maxRuns)
                    E = (temp == 0) ? false : true;
                else
                    E = false;
            }
            else
            {
                E = intersections[x + 1, y].WestDoor;
            }
        }
        else
        {
            E = false;
        }

        //Determine West
        if (x > 0)
        {
            if (intersections[x - 1, y].Empty)
            {
                int temp = Random.Range(0, 2);
                if (currentRun < maxRuns)
                    W = (temp == 0) ? false : true;
                else
                    W = false;
            }
            else
            {
                W = intersections[x - 1, y].EastDoor;
            }
        }
        else
        {
            W = false;
        }

        if (N == false && S == false && E == false && W == false)
            intersections[x, y] = new Intersection(true);
        else
            intersections[x, y] = new Intersection(N, S, E, W, false);

    }

    bool IsAdjacent(int x, int y)
    {
        if (y > 0)
        {
            if (intersections[x, y - 1].NorthDoor) return true;
        }
        if (y < 24)
        {
            if (intersections[x, y + 1].SouthDoor) return true;
        }
        if (x < 24)
        {
            if (intersections[x + 1, y].WestDoor) return true;
        }
        if (x > 0)
        {
            if (intersections[x - 1, y].EastDoor) return true;
        }
        return false;
    }

    void SpawnRoom(int x, int y)
    {
        List<GameObject> tempRooms = new List<GameObject>();

        Intersection section = intersections[x, y];

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
                            tempRooms.Add(obj);
                        }
                    }
                }
            }
        }

        GameObject tempRoom = Instantiate(tempRooms[Random.Range(0, tempRooms.Count)]);
        tempRoom.transform.position = new Vector3(x * RoomSize, 0, y * RoomSize);

    }

}
