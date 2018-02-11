using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComponent : MonoBehaviour {

    int width;
    int height;
    List<DoorTile> doors = new List<DoorTile>();
    Vector3Int position;
    int numOfDoors;

    List<int> directions = new List<int>();


	// Use this for initialization
	void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    //Initialzes datastructure for room
    public void Init(int width, int height, Vector3Int position, float numOfDoors)
    {
        this.width = width;
        this.height = height;
        this.position = position;
        this.numOfDoors = Mathf.RoundToInt(numOfDoors);
    }

    public List<DoorTile> getDoors()
    {
        return doors;
    }

    public void addDoor(DoorTile door)
    {
        doors.Add(door);
    }

    public List<int> getDirections()
    {
        return directions;
    }

    public void addDirection(int direction)
    {
        directions.Add(direction);
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }

    public int getNumOfDoors()
    {
        return numOfDoors;
    }
}
