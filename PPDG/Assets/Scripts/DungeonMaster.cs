using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaster : MonoBehaviour {

    [SerializeField]
    private string playerSeed;

    private RNGenerator generator;

    public List<float> sequence;

    public List<float> initValues;

    bool isFinished;

    int numOfRooms;

    int pointerOnRoom = 0;

    [SerializeField]
    RoomComponent[] rooms;

    //Class which generatesfloor/walls/doors and glues them together
    RoomBuilder roomBuilder;

	// Use this for initialization
	void Start () {

        roomBuilder = this.gameObject.GetComponent<RoomBuilder>();
        sequence = new List<float>();
        initValues = new List<float>();
        generator = this.GetComponent<RNGenerator>();
    
        generator.init(playerSeed, sequence);
        numOfRooms = generator.getInitValues();
        
        rooms = new RoomComponent[numOfRooms];
        getCompleteSequence();
        for (int i = 0; i < rooms.Length; i++)
        {
            rooms[i] = new RoomComponent();
        }

        for (int j = 0; j < rooms.Length; j++)
        {
            calculateDirections();
            pointerOnRoom++;
        }

        pointerOnRoom = 0;
        preProcessRooms();
        
        /*
        Debug.Log("Room 0: " + " Richtungen: " +(Enums.Direction) rooms[0].getDirections()[0] + " " +(Enums.Direction) rooms[0].getDirections()[1] + " " + (Enums.Direction)rooms[0].getDirections()[2] + " " + (Enums.Direction)rooms[0].getDirections()[3]);
        Debug.Log("Room 1: " + " Richtungen: " +(Enums.Direction) rooms[1].getDirections()[0] + " " +(Enums.Direction) rooms[1].getDirections()[1] + " " + (Enums.Direction)rooms[1].getDirections()[2] + " " + (Enums.Direction)rooms[1].getDirections()[3]);
        Debug.Log("Room 2: " + " Richtungen: " +(Enums.Direction) rooms[2].getDirections()[0] + " " +(Enums.Direction) rooms[2].getDirections()[1] + " " + (Enums.Direction)rooms[2].getDirections()[2] + " " + (Enums.Direction)rooms[2].getDirections()[3]);
        Debug.Log("Room 3: " + " Richtungen: " +(Enums.Direction) rooms[3].getDirections()[0] + " " +(Enums.Direction) rooms[3].getDirections()[1] + " " + (Enums.Direction)rooms[3].getDirections()[2] + " " + (Enums.Direction)rooms[3].getDirections()[3]);
        Debug.Log("Room 4: " + " Richtungen: " +(Enums.Direction) rooms[4].getDirections()[0] + " " +(Enums.Direction) rooms[4].getDirections()[1] + " " + (Enums.Direction)rooms[4].getDirections()[2] + " " + (Enums.Direction)rooms[4].getDirections()[3]);
        Debug.Log("Room 5: " + " Richtungen: " +(Enums.Direction) rooms[5].getDirections()[0] + " " +(Enums.Direction) rooms[5].getDirections()[1] + " " + (Enums.Direction)rooms[5].getDirections()[2] + " " + (Enums.Direction)rooms[5].getDirections()[3]);
        Debug.Log("Room 6: " + " Richtungen: " +(Enums.Direction) rooms[6].getDirections()[0] + " " + (Enums.Direction)rooms[6].getDirections()[1] + " " + (Enums.Direction)rooms[6].getDirections()[2] + " " + (Enums.Direction)rooms[6].getDirections()[3]);  
        */

        roomBuilder.buildRoom(rooms[pointerOnRoom]);
	}

    private void calculateDirections()
    {
        rooms[pointerOnRoom].Init(5, 7);
        int maxDoors = 4;
        int direction;
        for (int i = 0; i < maxDoors; i++)
        {
            direction = calculateDoorDirection(sequence[4 * pointerOnRoom + i]);
            //Debug.Log((Enums.Direction)direction);
            rooms[pointerOnRoom].addDirection(direction);
        }
    }

    void preProcessRooms()
    {
        int number = 0;

        for(int i = 0; i < rooms.Length; i++)
        {
             for(int j = 1; j <= 4; j++)
            {
                if (rooms[i].getDirections().Contains(j) && number < rooms.Length-1)
                {                  
                    if (rooms[i].hasNeighbour(j) == false)
                    {
                        
                        rooms[number+1].setRoomNumber(number+1);   
                        setNeighbourhood(i, rooms[number+1], (Enums.Direction)j);
                        number += 1;
                    } 
                }
            } 
        }
    }

    void setNeighbourhood(int i, RoomComponent neighbour, Enums.Direction direction)
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


    private void getCompleteSequence()
    {    
        for(int i = 0; i < rooms.Length * 4; i++)
        {
            generator.getNextNumber(0, 100);
        }
    }
  
    public void moveForward(Vector3Int pos, Enums.Direction direction)
    {
        List<int> directions = rooms[pointerOnRoom].getDirections();
  
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
                //Something goes wrong here
                pointerOnRoom = rooms[pointerOnRoom].leftNeihghbour.getRoomNumber();
                pos += new Vector3Int(rooms[pointerOnRoom].getWidth()*2, 1, 0);
                break;  
        } 

        calculateDirections();
        GameObject.Find("Player").transform.position = pos;
        //roomBuilder.saveRoom();
        roomBuilder.buildRoom(rooms[pointerOnRoom]);
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
}
