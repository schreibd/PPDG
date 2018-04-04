using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomComponent {

    private int width;
    private int height;
    private List<DoorTile> doors = new List<DoorTile>();
    private GameObject key;
    private int numOfDoors;

    private int xPos;
    private int yPos;

    private List<int> directions = new List<int>();

    public RoomComponent leftNeihghbour;
    public RoomComponent topNeighbour;
    public RoomComponent rightNeighbour;
    public RoomComponent bottomNeighbour;

    private Rect minimapPosition;

    private int number;

    private int monsterCount;

    private bool locked = false;

    private List<Enums.Monster> monsters = new List<Enums.Monster>();

    //Initializes datastructure for room
    public void Init(int width, int height)
    {
        this.width = width;
        this.height = height;
    }

    //Checks if room contains parameterized direction
    public bool containsDirection(Enums.Direction direction)
    {
        return directions.Contains((int)direction);
    }

    //Sets a new list of directions
    public void setDirections(List<int> newDirections)
    {
        directions = newDirections;
    }

    //Sets new position for minimap
    public void setMinimapPosition(Rect position)
    {
        minimapPosition = position;
    }

    //Gets the minimap position
    public Rect getMinimapPosition()
    {
        return minimapPosition;
    }

    //Sets x and y coordinates
    public void setPosition(int x, int y)
    {
        xPos = x;
        yPos = y;
    }

    //Gets x position
    public int getXPos()
    {
        return xPos;
    }

    //Gets y position
    public int getYPos()
    {
        return yPos;
    }

    //Gets list of doors
    public List<DoorTile> getDoors()
    {
        return doors;
    }

    //Gets roomnumber
    public int getRoomNumber()
    {
        return number;
    }

    //Sets roomnumber
    public void setRoomNumber(int number)
    {
        this.number = number;
    }

    //Adds door to list of doors if it isn't already there
    public void addDoor(DoorTile door)
    {
        if(!doors.Contains(door))
            doors.Add(door);
    }

    //Gets list of all directions the room has
    public List<int> getDirections()
    {
        return directions;
    }

    public int findFreeDirection()
    {
        int c = 1; 
        while(c <= 4)
        {
            if (!directions.Contains(c))
                return c;
        }
        return 0;
    }

    //Adds a direction to room's direction list
    public void addDirection(int direction)
    {
        if (!directions.Contains(direction) && direction <= 4)
            directions.Add(direction);
        else
            directions.Add(0);
    }

    //Adds a pickable key to the room
    public void addKey(GameObject key)
    {
        this.key = key;
    }

    //Gets the placed key
    public GameObject getKey()
    {
        return key;
    }

    //Gets the neighbour in parameterized direction
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


    //Finds the direction from which the neighbour of the actual room is reachable
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

    //Checks if room has direction
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

    //Checks if room has door
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

    //Gets desired door 
    public DoorTile findDoor(DoorTile door)
    {
        foreach(DoorTile temp in doors)
        {
            if (temp.direction == door.direction)
                return temp;
        }
        return null;
    }

    //Checks if room has neighbour in direction
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

    //Gets width of room
    public int getWidth()
    {
        return width;
    }

    //Gets height of room
    public int getHeight()
    {
        return height;
    }

    //Gets the amount of door the room has
    public int getNumOfDoors()
    {
        return numOfDoors;
    }

    //Locks the rooms
    public void setLock(bool locked)
    {
        this.locked = locked;
    }

    //Returns true if room is locked
    public bool getLock()
    {
        return locked;
    }

    //Returns amount of monsters in room
    public int getMonsterCount()
    {
        return monsterCount;
    }

    //Sets amount of monsters in room
    public void setMonsterCount(int monsterCount)
    {
        this.monsterCount = monsterCount;
    }

    //Add a monster to rooms list of monsters
    public void addMonster(Enums.Monster monster)
    {
        monsters.Add(monster);
    }

    //Returns list of monsters
    public List<Enums.Monster> getMonsters()
    {
        return monsters;
    }

    //Removes parameterized direction
    public void removeDirection(int direction)
    {
        if(directions.Contains(direction))
            directions.Remove(direction);
    }
}
