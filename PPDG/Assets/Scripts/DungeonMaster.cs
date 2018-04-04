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

    List<int> lostRoomIndeces;

    [SerializeField]
    public List<RoomComponent> rooms;

    Text roomNrLabel;

    List<GameObject> activeMonsters;


    // Use this for initialization
    void Start() {

        roomNrLabel = GameObject.Find("RoomNumber").GetComponent<Text>();

        activeMonsters = new List<GameObject>();

        //Set seed as state in our RNGenerator
        RNGenerator.Instance.init(playerSeed);

        //Gets the amount of rooms we want to generate
        numOfRooms = RNGenerator.Instance.getInitValues(level);

        rooms = new List<RoomComponent>();

        //Gets a sequence of random numbers we need to generate the rooms and their directions
        roomSequence = RNGenerator.Instance.getRoomSequence(numOfRooms);

        //Starts to
        startProcess();
        while (rooms[0].getDirections().Count == 0)
        {
            roomSequence = RNGenerator.Instance.getRoomSequence(numOfRooms);
            startProcess();
        }

        ContentCreator.Instance.lockDoors(rooms);
        MonsterRuleset.Instance.calculateMonsters(rooms);

        pointerOnRoom = 0;
        RoomBuilder.Instance.buildRoom(rooms[pointerOnRoom], Enums.Direction.DEADEND, activeMonsters);
        roomNrLabel.text = 0.ToString();

    }

    //Gets called if player enters level exit
    public void nextLevel()
    {
        pointerOnRoom = 0;
        
        level++;

        //Clears current level data in roombuilder 
        RoomBuilder.Instance.clearBuilderData();

        foreach (GameObject monster in activeMonsters)
            Destroy(monster);

        //Clears minimap
        Minimap.Instance.clearScreen(true);
        //Activate black screen
        RoomInterpolator.Instance.activate();
        //Creates a new list for active monsters
        activeMonsters = new List<GameObject>();

        numOfRooms = RNGenerator.Instance.getInitValues(level);

        roomSequence = RNGenerator.Instance.getRoomSequence(numOfRooms);

        //Start drawing new level on minimap
        Minimap.Instance.clearScreen(false);

        startProcess();
        //Edge case to catch if something goes wrong, process gets started again
        while (rooms[0].getDirections().Count == 0)
        {
            roomSequence = RNGenerator.Instance.getRoomSequence(numOfRooms);
            startProcess();
        } 

        //Start process to lock a door
        ContentCreator.Instance.lockDoors(rooms);
        //Start process to calculate monsters for every room
        MonsterRuleset.Instance.calculateMonsters(rooms);

        //Pointer which helps us to iterate over the roomSequence
        pointerOnRoom = 0;
        RoomBuilder.Instance.buildRoom(rooms[pointerOnRoom], Enums.Direction.DEADEND, activeMonsters);

        //Label needs to show the amount of keys player has
        //Testing amount is 99
        roomNrLabel.text = GameObject.Find("Player").GetComponent<PlayerComponent>().getKeys().ToString();
    }

    //Used to do initializing work and start rolling preprocessing
    void startProcess()
    {
        //List of rooms we want to create
        rooms = new List<RoomComponent>();

        //Creating rooms without data
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

        //Begin ordering the rooms
        preProcessRooms();
    }

    //Calculates the directions for every room depending on the rn we generated before
    private void calculateDirections()
    {
        rooms[pointerOnRoom].Init(5, 7);
        rooms[pointerOnRoom].setDirections(new List<int>());
        int maxDoors = 4;
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
                //int direction = j;
                if (number < numOfRooms - 1 && rooms[i].getDirections().Contains(direction))
                {
                    
                    if (!rooms[i].hasNeighbour(direction))
                    {
                        //Catches up starting room 
                        //Especially needed for MiniMapPositioning
                        if (i == 0)
                        {
                            rooms[number + 1].setRoomNumber(number + 1);
                            rooms[i].setPosition(0, 0);
                            setNeighbour(i, rooms[number + 1], (Enums.Direction)direction);
                            int tempY = Minimap.Instance.calcYPos(rooms[i], (Enums.Direction)direction);
                            int tempX = Minimap.Instance.calcXPos(rooms[i], (Enums.Direction)direction);
                            rooms[number + 1].setPosition(tempX, tempY);

                            number += 1;
                        }
                        else
                        {
                            int tempY = Minimap.Instance.calcYPos(rooms[i], (Enums.Direction)direction);
                            int tempX = Minimap.Instance.calcXPos(rooms[i], (Enums.Direction)direction);


                            if (!roomExists(tempX, tempY))
                            {
                                rooms[number + 1].setRoomNumber(number + 1);
                                rooms[number + 1].setPosition(tempX, tempY);
                                setNeighbour(i, rooms[number + 1], (Enums.Direction)direction);
                                number += 1;
                            }
                            else
                            {       //Finds us the room already existing on that position
                                    
                                    foreach (RoomComponent room in rooms)
                                    {
                                        if (room.getXPos() == tempX && room.getYPos() == tempY)
                                        {
                                            setNeighbour(i, room, (Enums.Direction)direction);
                                            rooms.Remove(rooms[number + 1]);    
                                            numOfRooms--;
                                            i--;
                                            break;
                                        }
                                    }
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

    //Used to set two rooms as neighbours
    void setNeighbour(int i, RoomComponent neighbour, Enums.Direction direction)
    {
        switch(direction)
        {
            case Enums.Direction.NORTH:
                rooms[i].topNeighbour = neighbour;
                neighbour.bottomNeighbour = rooms[i];
                break;
            case Enums.Direction.EAST:
                rooms[i].rightNeighbour = neighbour;
                neighbour.leftNeihghbour= rooms[i];
                break;
            case Enums.Direction.SOUTH:
                rooms[i].bottomNeighbour = neighbour;
                neighbour.topNeighbour = rooms[i];
                break;
            case Enums.Direction.WEST:
                rooms[i].leftNeihghbour = neighbour;
                neighbour.rightNeighbour = rooms[i];
                break;
        }
    }
    
    //Gets called if player enters a doortile
    //Used to set player to the next room
    public void moveForward(Vector3Int pos, Enums.Direction direction)
    {
        //Fade screen to black and back
        RoomInterpolator.Instance.activate();
        
        List<int> directions = rooms[pointerOnRoom].getDirections();

        //activates the key in the room if existing
        if(rooms[pointerOnRoom].getKey())
            rooms[pointerOnRoom].getKey().SetActive(false);
    
        //Switches the direction to calculate the playerPosition for the next room
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

        //New player position after leaving a room
        GameObject.Find("Player").transform.position = pos;
        
        RoomBuilder.Instance.buildRoom(rooms[pointerOnRoom], direction, activeMonsters);

        roomNrLabel.text = rooms[pointerOnRoom].getRoomNumber().ToString();
    }

    //Gets a rn as input and delivers depending on that rn value a direction
    private int calculateDoorDirection(float value)
    {
        int direction;
        //NORTH
        if (value >= 0.0f && value <= 24.99f)
            direction = 1;
        //EAST
        else if (value >= 25.0f && value <= 49.99f)
            direction = 2;
        //SOUTH
        else if (value >= 50.0f && value <= 74.99f)
            direction = 3;
        //WEST
        else if (value >= 75.0f && value <= 100.00f)
            direction = 4;
        //DEADEND
        else
            direction = 0;
        return direction;
    }

    //Check if room already exists at the given coordinates
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
