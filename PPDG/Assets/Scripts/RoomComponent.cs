using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComponent {

    int width;
    int height;
    List<DoorTile> doors = new List<DoorTile>();
    private GameObject key;
    int numOfDoors;
    int xPos;
    int yPos;

    List<int> directions = new List<int>();

    public RoomComponent leftNeihghbour;
    public RoomComponent topNeighbour;
    public RoomComponent rightNeighbour;
    public RoomComponent bottomNeighbour;

    private Rect minimapPosition;

    [SerializeField]
    private int number;

    private int monsterCount;

    private bool locked = false;

    private List<Enums.Monster> monsters = new List<Enums.Monster>();



    public int getMonsterCount()
    {
        return monsterCount;
    }

    public void setLock(bool locked)
    {
        this.locked = locked;
    }

    public bool getLock()
    {
        return locked;
    }

    public void setMonsterCount(int monsterCount)
    {
        this.monsterCount = monsterCount;
    }

    public void addMonster(Enums.Monster monster)
    {
        monsters.Add(monster);
    }

    public void removeDirection(int direction)
    {
        directions.Remove(direction);
    }

    public List<Enums.Monster> getMonsters()
    {
        return monsters;
    }


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

    public void setDirections(List<int> newDirections)
    {
        directions = newDirections;
    }

    public void setMinimapPosition(Rect position)
    {
        minimapPosition = position;
    }

    public Rect getMinimapPosition()
    {
        return minimapPosition;
    }

    /*public void killDoor(int direction)
    {
        foreach(DoorTile door in doors)
        {
            if((int)door.direction == direction)
            {
                doors.Remove(door);
                directions.Remove(direction);
            }
        }
    } */

    public void setPosition(int x, int y)
    {
        xPos = x;
        yPos = y;
    }

    public int getXPos()
    {
        return xPos;
    }

    public int getYPos()
    {
        return yPos;
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
       // foreach (DoorTile temp in doors)
            //if (temp.direction == door.direction)
                //doors.Remove(temp);
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

    public void addKey(GameObject key)
    {
        this.key = key;
    }

    public GameObject getKey()
    {
        return key;
    }

    public RoomComponent getNeighbour(int direction)
    {
        RoomComponent neighbour = null;
        switch ((Enums.Direction)direction)
        {
            case Enums.Direction.NORTH:
                neighbour = topNeighbour;
                break;
            case Enums.Direction.EAST:
                neighbour = rightNeighbour;
                break;
            case Enums.Direction.SOUTH:
                neighbour = bottomNeighbour;
                break;
            case Enums.Direction.WEST:
                neighbour = leftNeihghbour;
                break;
        }
        return neighbour;
    }

    public void clearDirections()
    {
        directions.Clear();
        System.GC.Collect();
    }

    public Enums.Direction findDirection(RoomComponent neighbour)
    {
        Enums.Direction result = Enums.Direction.DEADEND;

        if (neighbour == topNeighbour)
            result = Enums.Direction.NORTH;
        else if (neighbour == rightNeighbour)
            result = Enums.Direction.EAST;
        else if (neighbour == bottomNeighbour)
            result = Enums.Direction.SOUTH;
        else if (neighbour == leftNeihghbour)
            result = Enums.Direction.WEST;

        return result;
    }

    public bool isExisting(RoomComponent optionA, RoomComponent optionB)
    {
        bool result = false;

        if(optionA.hasNeighbour(2) && optionA.rightNeighbour == optionB.topNeighbour && optionB.hasNeighbour(4))
        {
            result = true;
        }

        return result;
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

    public bool hasDoor(DoorTile door)
    {
        bool result = false;
        foreach(DoorTile temp in doors)
        {
            if (temp.direction == door.direction)
            {
                result = true;
                break;
            }
        }
        return result;
    }

    public DoorTile findDoor(DoorTile door)
    {
        foreach(DoorTile temp in doors)
        {
            if (temp.direction == door.direction)
                return temp;
        }
        return null;
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
