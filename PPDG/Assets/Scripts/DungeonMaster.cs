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
    public List<RoomComponent> rooms;

    //Class which generatesfloor/walls/doors and glues them together
    RoomBuilder roomBuilder;


	// Use this for initialization
	void Start () {

        roomBuilder = this.gameObject.GetComponent<RoomBuilder>();
        sequence = new List<float>();
        initValues = new List<float>();
        generator = this.GetComponent<RNGenerator>();
        
    
        generator.init(playerSeed, sequence);
        //numOfRooms = generator.getInitValues();
        numOfRooms = 50;
        Debug.Log("Anzahl der Räume in diesem Dungeon: " + numOfRooms);
        
        rooms = new List<RoomComponent>();
        getCompleteSequence();
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
        preProcessRooms();

        foreach(RoomComponent room in rooms)
        {
            //Debug.Log("Room " + room.getRoomNumber() + " Richtungen: " + (Enums.Direction)room.getDirections()[0] + " " + (Enums.Direction)room.getDirections()[1] + " " + (Enums.Direction)room.getDirections()[2] + " " + (Enums.Direction)room.getDirections()[3]);
            //Debug.Log("Room " + room.getRoomNumber() + "X/Y-Koordinaten: " + room.getXPos() + "/" + room.getYPos());
        }
        /*
        Debug.Log("Room 0: " + " Richtungen: " +(Enums.Direction) rooms[0].getDirections()[0] + " " +(Enums.Direction) rooms[0].getDirections()[1] + " " + (Enums.Direction)rooms[0].getDirections()[2] + " " + (Enums.Direction)rooms[0].getDirections()[3]);
        Debug.Log("Room 1: " + " Richtungen: " +(Enums.Direction) rooms[1].getDirections()[0] + " " +(Enums.Direction) rooms[1].getDirections()[1] + " " + (Enums.Direction)rooms[1].getDirections()[2] + " " + (Enums.Direction)rooms[1].getDirections()[3]);
        Debug.Log("Room 2: " + " Richtungen: " +(Enums.Direction) rooms[2].getDirections()[0] + " " +(Enums.Direction) rooms[2].getDirections()[1] + " " + (Enums.Direction)rooms[2].getDirections()[2] + " " + (Enums.Direction)rooms[2].getDirections()[3]);
        Debug.Log("Room 3: " + " Richtungen: " +(Enums.Direction) rooms[3].getDirections()[0] + " " +(Enums.Direction) rooms[3].getDirections()[1] + " " + (Enums.Direction)rooms[3].getDirections()[2] + " " + (Enums.Direction)rooms[3].getDirections()[3]);
        Debug.Log("Room 4: " + " Richtungen: " +(Enums.Direction) rooms[4].getDirections()[0] + " " +(Enums.Direction) rooms[4].getDirections()[1] + " " + (Enums.Direction)rooms[4].getDirections()[2] + " " + (Enums.Direction)rooms[4].getDirections()[3]);
        Debug.Log("Room 5: " + " Richtungen: " +(Enums.Direction) rooms[5].getDirections()[0] + " " +(Enums.Direction) rooms[5].getDirections()[1] + " " + (Enums.Direction)rooms[5].getDirections()[2] + " " + (Enums.Direction)rooms[5].getDirections()[3]);
        Debug.Log("Room 6: " + " Richtungen: " +(Enums.Direction) rooms[6].getDirections()[0] + " " + (Enums.Direction)rooms[6].getDirections()[1] + " " + (Enums.Direction)rooms[6].getDirections()[2] + " " + (Enums.Direction)rooms[6].getDirections()[3]);  
        
        */

        roomBuilder.buildRoom(rooms[pointerOnRoom], Enums.Direction.DEADEND);

        /*
        foreach(RoomComponent room in rooms)
        {
            Debug.Log("Room: " + room.getRoomNumber() + " Koordinaten: X " + room.getXPos() + " / Y " + room.getYPos());
        }

        */

       // Debug.Log(rooms[10].bottomNeighbour.getRoomNumber());
        





	}

    private void calculateDirections()
    {
        rooms[pointerOnRoom].Init(5, 7);
        int maxDoors = 4;
        int direction;
        for (int i = 0; i < maxDoors; i++)
        {
            direction = calculateDoorDirection(sequence[4 * pointerOnRoom + i]);
            rooms[pointerOnRoom].addDirection(direction);
        }
    }

    //Change method so room order is random and not N->E->S->W

    void preProcessRooms()
    {
        
        int number = 0;
        for (int i = 0; i < numOfRooms; i++)
        {
            for (int j = 1; j <= 4; j++)
            {
                if (number < numOfRooms - 1 && rooms[i].getDirections().Contains(j))
                {
                    
                    if (!rooms[i].hasNeighbour(j))
                    {

                        //
                        if (i == 0)
                        {
                            rooms[number + 1].setRoomNumber(number + 1);
                            rooms[i].setPosition(0, 0);
                            setNeighbourhood(i, rooms[number + 1], (Enums.Direction)j);
                            int tempY;
                            int tempX;
                            tempY = calcYPos(rooms[i], (Enums.Direction)j);
                            tempX = calcXPos(rooms[i], (Enums.Direction)j);
                            rooms[number + 1].setPosition(tempX, tempY);
                            number += 1;
                        }
                        else
                        {
                            int tempY;
                            int tempX;
                            tempY = calcYPos(rooms[i], (Enums.Direction)j);
                            tempX = calcXPos(rooms[i], (Enums.Direction)j);
                            //Debug.Log("X: " + tempX + " Y: " + tempY);


                            
                            if(!roomExists(tempX, tempY))
                            {
                                rooms[number + 1].setRoomNumber(number + 1);
                                rooms[number + 1].setPosition(tempX, tempY);
                                setNeighbourhood(i, rooms[number + 1], (Enums.Direction)j);
                                number += 1; 
                            }
                            else
                            {
                                
                            }
                            
                        }

                        //else-fall ohne i
                    }
                }
            }
        }
    }

    //Change method so room order is random and not N->E->S->W
    /*
    void preProcessRooms()
    {
        int number = 0;

        for(int i = 0; i < rooms.Length; i++)
        {
             for(int j = 1; j <= 4; j++)
            {
                if (rooms[i].getDirections().Contains(j) && number < rooms.Length-1)
                {                  
                    if (!rooms[i].hasNeighbour(j))
                    {

                        //
                        if(i == 0 || !rooms[i - 1].hasNeighbour(2) || !rooms[i - 1].hasNeighbour(4))
                        {
                            rooms[number + 1].setRoomNumber(number + 1);
                            setNeighbourhood(i, rooms[number + 1], (Enums.Direction)j);
                            number += 1;
                        }
                        
                        //else-fall ohne i
                    } 
                }
            } 
        }
    } */

    void findEmptyPlace()
    {

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


    private void getCompleteSequence()
    {    
        for(int i = 0; i < numOfRooms * 4; i++)
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
        
        roomBuilder.buildRoom(rooms[pointerOnRoom], direction);
        Debug.Log("Koordinaten für Raumnr.: " + rooms[pointerOnRoom].getRoomNumber() + " X|Y "+ rooms[pointerOnRoom].getXPos() + "|" + rooms[pointerOnRoom].getYPos());
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
                //Debug.Log("Raumnummer: " + room.getRoomNumber() + " " + room.getXPos() + "|" + room.getYPos() + " Targetnummer: " + target.getRoomNumber() + " " + target.getXPos() + "|" + target.getYPos());
                if (room.getXPos() == x && room.getYPos() == y)
                {    
                    result = true;
                    break;
                }         
        }  
        return result;
    }

    private int calcXPos(RoomComponent room, Enums.Direction direction)
    {
        int xPos = 0;
        switch (direction)
        {
            case Enums.Direction.NORTH:
                xPos = room.getXPos();
                break;
            case Enums.Direction.EAST:
                xPos = room.getXPos()+1;
                break;
            case Enums.Direction.SOUTH:
                xPos = room.getXPos();
                break;
            case Enums.Direction.WEST:
                xPos = room.getXPos()-1;
                break;
        }
        return xPos;
    }


    private int calcYPos(RoomComponent room, Enums.Direction direction)
    {
        int yPos = 0;
        switch (direction)
        {
            case Enums.Direction.NORTH:
                yPos = room.getYPos()+1;
                break;
            case Enums.Direction.EAST:
                yPos = room.getYPos();
                break;
            case Enums.Direction.SOUTH:
                yPos = room.getYPos()-1;
                break;
            case Enums.Direction.WEST:
                yPos = room.getYPos();
                break;
        }
        return yPos;
    }
}
