using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public bool NorthDoor;
    public bool SouthDoor;
    public bool EastDoor;
    public bool WestDoor;

    [Header("For StairCases")]
    public bool lowerNorthDoor;
    public bool lowerSouthDoor;
    public bool lowerEastDoor;
    public bool lowerWestDoor;

    public bool hasGoal;

}
