using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComponent {

    int width;
    int height;
    List<DoorTile> doors = new List<DoorTile>();
    int numOfDoors;

    List<int> directions = new List<int>();

    public RoomComponent leftNeihghbour;
    public RoomComponent topNeighbour;
    public RoomComponent rightNeighbour;
    public RoomComponent bottomNeighbour;

    [SerializeField]
    private int number;





    // Use this for initialization
    void Start() {

    }

    public bool containsDirection(Enums.Direction direction)
    {
        return directions.Contains((int)direction);
    }

    // Update is called once per frame
    void Update() {

    }



    //Initialzes datastructure for room
    public void Init(int width, int height)
    {
        this.width = width;
        this.height = height;

    }

    public List<DoorTile> getDoors()
    {
        return doors;
    }

    public int getRoomNumber()
    {
        return number;
    }

    public void setRoomNumber(int number)
    {
        this.number = number;
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
        if (!directions.Contains(direction) && direction <= 4)
            directions.Add(direction);
        else
            directions.Add(0);
    }

    public bool hasDirection(int direction)
    {
        bool result = false;
        foreach (int value in directions)
        {
            if (direction == value)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    public bool hasNeighbour(int direction)
    {
        Enums.Direction dir = (Enums.Direction)direction;
        bool result = true;
        if (dir == Enums.Direction.NORTH)
        {
            if (topNeighbour == null)
            {
                result = false;
            }
        }
        else if (dir == Enums.Direction.EAST)
        {
            if (rightNeighbour == null)
            {
                result = false;
            }
        }
        else if (dir == Enums.Direction.SOUTH)
        {
            if (bottomNeighbour == null)
            {
                result = false;
            }
        }
        else if (dir == Enums.Direction.WEST)
        {
            if (leftNeihghbour == null)
            {
                result = false;
            }
        }
        return result;
       
             
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
