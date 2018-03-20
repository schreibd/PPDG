using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DungeonMaster : MonoBehaviour {


    //We use the hash code of this string as seed in our RNGenerator
    [SerializeField]
    private string playerSeed;

    //Stores all RoomComponents
    public List<float> roomSequence;

    int numOfRooms;

    int pointerOnRoom = 0;

    //Collected keys by player
    int collectedKeys = 0;

    int level = 1;

    [SerializeField]
    public List<RoomComponent> rooms;

    Text roomNrLabel;

    List<GameObject> activeMonsters;


	// Use this for initialization
	void Start () {

        roomNrLabel = GameObject.Find("RoomNumber").GetComponent<Text>();

        activeMonsters = new List<GameObject>();

        //Set seed as state in our RNGenerator
        RNGenerator.Instance.init(playerSeed);

        //Gets the amount of rooms we want to generate
        numOfRooms = RNGenerator.Instance.getInitValues(level);

        rooms = new List<RoomComponent>();

        //Gets a sequence of random numbers we need to generate the rooms and their directions
        roomSequence = RNGenerator.Instance.getRoomSequence(numOfRooms);

        newBeginning();
        while (!isValidDungeon())
            newBeginning();
        
        ContentCreator.Instance.lockDoors(rooms);
        MonsterSpawner.calculateMonsters(rooms);

        pointerOnRoom = 0;
        RoomBuilder.Instance.buildRoom(rooms[pointerOnRoom], Enums.Direction.DEADEND, activeMonsters);
        roomNrLabel.text = 0.ToString();

    }

    public void nextLevel()
    {
        pointerOnRoom = 0;
        level++;

        RoomBuilder.Instance.clearBuilderData();

        foreach (GameObject monster in activeMonsters)
            Destroy(monster);

        Minimap.Instance.clearMinimap();
        RoomInterpolator.Instance.activate();
        activeMonsters = new List<GameObject>();
        numOfRooms = RNGenerator.Instance.getInitValues(level);
        rooms = new List<RoomComponent>();

        roomSequence = RNGenerator.Instance.getRoomSequence(numOfRooms);

        Minimap.Instance.clearScreen(false);

        newBeginning();
        while (!isValidDungeon())
            newBeginning();

        ContentCreator.Instance.lockDoors(rooms);
        MonsterSpawner.calculateMonsters(rooms);

        pointerOnRoom = 0;
        RoomBuilder.Instance.buildRoom(rooms[pointerOnRoom], Enums.Direction.DEADEND, activeMonsters);
        roomNrLabel.text = 0.ToString();

    }

    
    private bool isValidDungeon()
    {
        bool result = true;
        if (rooms[rooms.Count - 1].getRoomNumber() == 0)
            result = false;
        return result;

    } 

    void newBeginning()
    {
        rooms = new List<RoomComponent>();

        for (int i = 0; i < numOfRooms; i++)
        {
            rooms.Add(new RoomComponent());
        }


        for (int j = 0; j < numOfRooms; j++)
        {
            calculateDirections();
            pointerOnRoom++;
        }

        pointerOnRoom = 0;

        //Pointer which helps us to iterate over the roomSequence
        //pointerOnRoom = 0;

        preProcessRooms();
        
    }
    
    private void calculateDirections()
    {
        rooms[pointerOnRoom].Init(5, 7);
        rooms[pointerOnRoom].setDirections(new List<int>());
        int maxDoors = 4;
        //int maxDoors = calculateDoorDirection(RNGenerator.Instance.getNextNumber(0,100));
        int direction;
        for (int i = 0; i < maxDoors; i++)
        {
            direction = calculateDoorDirection(roomSequence[4 * pointerOnRoom + i]);
            rooms[pointerOnRoom].addDirection(direction);
        }
    } 
  
    //Sets neighbours for every room with help of the random number sequence we created on start
    //Calculates positions for every room on our minimap
    void preProcessRooms()
    {   
        int number = 0;

        //Iterates over all rooms
        for (int i = 0; i < numOfRooms; i++)
        {
            //Iterates over every possible direction (see enums)
            for (int j = 1; j <= 4; j++)
            {
                //Randomly chooses the direction of the next room
                int direction = calculateDoorDirection(RNGenerator.Instance.getNextNumber(0, 100));
                if (number < numOfRooms - 1 && rooms[i].getDirections().Contains(direction))
                {
                    
                    if (!rooms[i].hasNeighbour(direction))
                    {
                        if (i == 0)
                        {
                            rooms[number + 1].setRoomNumber(number + 1);
                            rooms[i].setPosition(0, 0);
                            setNeighbourhood(i, rooms[number + 1], (Enums.Direction)direction);
                            int tempY = Minimap.Instance.calcYPos(rooms[i], (Enums.Direction)direction);
                            int tempX = Minimap.Instance.calcXPos(rooms[i], (Enums.Direction)direction);
                            rooms[number + 1].setPosition(tempX, tempY);
                            number += 1;
                        }
                        else
                        {
                            int tempY = Minimap.Instance.calcYPos(rooms[i], (Enums.Direction)direction);
                            int tempX = Minimap.Instance.calcXPos(rooms[i], (Enums.Direction)direction);
                           
                            
                            if(!roomExists(tempX, tempY))
                            {
                                rooms[number + 1].setRoomNumber(number + 1);
                                rooms[number + 1].setPosition(tempX, tempY);
                                setNeighbourhood(i, rooms[number + 1], (Enums.Direction)direction);
                                number += 1; 
                            }
                           else
                            {
                                newBeginning();
                            }
                            
                        }
                        
                    }
                }
                
            }
        }
        cleanUp();
    } 

    //Gets rid of directions we don't need (for every room)
    private void cleanUp()
    {
       foreach(RoomComponent room in rooms)
        {
            List<int> newDirections = new List<int>();
            for(int i = 1; i <= 4; i++)
            {
                if (room.hasNeighbour(i))
                    newDirections.Add(i);
            }
            room.setDirections(newDirections);
        }
    }

    void setNeighbourhood(int i, RoomComponent neighbour, Enums.Direction direction)
    {
        switch(direction)
        {
            case Enums.Direction.NORTH:
                rooms[i].topNeighbour = neighbour;
                neighbour.bottomNeighbour = rooms[i];
                //neighbour.setPosition(rooms[i].getXPos(), rooms[i].getYPos() + 1);
                break;
            case Enums.Direction.EAST:
                rooms[i].rightNeighbour = neighbour;
                neighbour.leftNeihghbour= rooms[i];
                //neighbour.setPosition(rooms[i].getXPos()+1, rooms[i].getYPos());
                break;
            case Enums.Direction.SOUTH:
                rooms[i].bottomNeighbour = neighbour;
                neighbour.topNeighbour = rooms[i];
                //neighbour.setPosition(rooms[i].getXPos(), rooms[i].getYPos() -1);
                break;
            case Enums.Direction.WEST:
                rooms[i].leftNeihghbour = neighbour;
                neighbour.rightNeighbour = rooms[i];
                //neighbour.setPosition(rooms[i].getXPos()-1, rooms[i].getYPos());
                break;
        }
    }
  
    public void moveForward(Vector3Int pos, Enums.Direction direction)
    {
        RoomInterpolator.Instance.activate();
        
        List<int> directions = rooms[pointerOnRoom].getDirections();

        if(rooms[pointerOnRoom].getKey())
            rooms[pointerOnRoom].getKey().SetActive(false);
  
        switch(direction)
        {
            case Enums.Direction.NORTH:
                pointerOnRoom = rooms[pointerOnRoom].topNeighbour.getRoomNumber();
                pos -= new Vector3Int(-1, rooms[pointerOnRoom].getHeight()-2, 0);
                break;
            case Enums.Direction.EAST:
                pointerOnRoom = rooms[pointerOnRoom].rightNeighbour.getRoomNumber();
                pos -= new Vector3Int(rooms[pointerOnRoom].getWidth()+2, -1 , 0);
                break;
            case Enums.Direction.SOUTH:
                pointerOnRoom = rooms[pointerOnRoom].bottomNeighbour.getRoomNumber();
                pos += new Vector3Int(rooms[pointerOnRoom].getWidth()/2, rooms[pointerOnRoom].getHeight(), 0);
                break;
            case Enums.Direction.WEST:
                pointerOnRoom = rooms[pointerOnRoom].leftNeihghbour.getRoomNumber();
                pos += new Vector3Int(rooms[pointerOnRoom].getWidth()*2, 1, 0);
                break;  
        } 

        GameObject.Find("Player").transform.position = pos;
        
        RoomBuilder.Instance.buildRoom(rooms[pointerOnRoom], direction, activeMonsters);

        roomNrLabel.text = rooms[pointerOnRoom].getRoomNumber().ToString();
    } 

    private int calculateDoorDirection(float value)
    {
        int direction;
        if (value >= 0.0f && value <= 24.99f)
            direction = 1;
        else if (value >= 25.0f && value <= 49.99f)
            direction = 2;
        else if (value >= 50.0f && value <= 74.99f)
            direction = 3;
        else if (value >= 75.0f && value <= 100.00f)
            direction = 4;
        else
            direction = 0;

        return direction;
    }

    private bool roomExists(int x, int y)
    {
        
        bool result = false;
        foreach(RoomComponent room in rooms)
        {
                if (room.getXPos() == x && room.getYPos() == y)
                {    
                    result = true;
                    break;
                }         
        }  
        return result;
    }

    
}
