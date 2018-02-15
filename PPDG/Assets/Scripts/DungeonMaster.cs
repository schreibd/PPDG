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
        //preProcessRooms();
        
        /*
        Debug.Log("Room 0: " + " Richtungen: " +(Enums.Direction) rooms[0].getDirections()[0] + " " +(Enums.Direction) rooms[0].getDirections()[1] + " " + (Enums.Direction)rooms[0].getDirections()[2] + " " + (Enums.Direction)rooms[0].getDirections()[3]);
        Debug.Log("Room 1: " + " Richtungen: " +(Enums.Direction) rooms[1].getDirections()[0] + " " +(Enums.Direction) rooms[1].getDirections()[1] + " " + (Enums.Direction)rooms[1].getDirections()[2] + " " + (Enums.Direction)rooms[1].getDirections()[3]);
        Debug.Log("Room 2: " + " Richtungen: " +(Enums.Direction) rooms[2].getDirections()[0] + " " +(Enums.Direction) rooms[2].getDirections()[1] + " " + (Enums.Direction)rooms[2].getDirections()[2] + " " + (Enums.Direction)rooms[2].getDirections()[3]);
        Debug.Log("Room 3: " + " Richtungen: " +(Enums.Direction) rooms[3].getDirections()[0] + " " +(Enums.Direction) rooms[3].getDirections()[1] + " " + (Enums.Direction)rooms[3].getDirections()[2] + " " + (Enums.Direction)rooms[3].getDirections()[3]);
        Debug.Log("Room 4: " + " Richtungen: " +(Enums.Direction) rooms[4].getDirections()[0] + " " +(Enums.Direction) rooms[4].getDirections()[1] + " " + (Enums.Direction)rooms[4].getDirections()[2] + " " + (Enums.Direction)rooms[4].getDirections()[3]);
        Debug.Log("Room 5: " + " Richtungen: " +(Enums.Direction) rooms[5].getDirections()[0] + " " +(Enums.Direction) rooms[5].getDirections()[1] + " " + (Enums.Direction)rooms[5].getDirections()[2] + " " + (Enums.Direction)rooms[5].getDirections()[3]);
        Debug.Log("Room 6: " + " Richtungen: " +(Enums.Direction) rooms[6].getDirections()[0] + " " + (Enums.Direction)rooms[6].getDirections()[1] + " " + (Enums.Direction)rooms[6].getDirections()[2] + " " + (Enums.Direction)rooms[6].getDirections()[3]); 
        
        */

        //getCompleteSequence();
        //calculateDirections();

        roomBuilder.buildRoom(rooms[pointerOnRoom]);
       // Debug.Log("Anzahl der Türen in diesem Raum: " + rooms[pointerOnRoom].getNumOfDoors());

	}

    void preProcessRooms()
    {
        int number = 0;
        //Debug.Log(rooms.Length);
        for(int i = 0; i < rooms.Length; i++)
        {
            //Debug.Log("Room: " + rooms[i].getRoomNumber() + " Richtungen: " + (Enums.Direction)rooms[i].getDirections()[0] + " " + (Enums.Direction)rooms[i].getDirections()[1] + " " + (Enums.Direction)rooms[i].getDirections()[2] + " " + (Enums.Direction)rooms[i].getDirections()[3]);
             for(int j = 1; j < 4; j++)
            {
                if (rooms[i].getDirections().Contains(j) && number < rooms.Length-1)
                {
                    rooms[number+1].setRoomNumber(number+1);
                    checkNeighbourhood(i, rooms[number+1], (Enums.Direction)j);
                    number += 1;
                }
            } 
        }
        Debug.Log("Room: " + rooms[0].getRoomNumber() + " Richtungen: " + (Enums.Direction)rooms[0].getDirections()[0] + " " + (Enums.Direction)rooms[0].getDirections()[1] + " " + (Enums.Direction)rooms[0].getDirections()[2] + " " + (Enums.Direction)rooms[0].getDirections()[3]);
        Debug.Log("Room: " + rooms[1].getRoomNumber() + " Richtungen: " + (Enums.Direction)rooms[1].getDirections()[0] + " " + (Enums.Direction)rooms[1].getDirections()[1] + " " + (Enums.Direction)rooms[1].getDirections()[2] + " " + (Enums.Direction)rooms[1].getDirections()[3]);
        Debug.Log("Room: " + rooms[2].getRoomNumber() + " Richtungen: " + (Enums.Direction)rooms[2].getDirections()[0] + " " + (Enums.Direction)rooms[2].getDirections()[1] + " " + (Enums.Direction)rooms[2].getDirections()[2] + " " + (Enums.Direction)rooms[2].getDirections()[3]);
        Debug.Log("Room: " + rooms[3].getRoomNumber() + " Richtungen: " + (Enums.Direction)rooms[3].getDirections()[0] + " " + (Enums.Direction)rooms[3].getDirections()[1] + " " + (Enums.Direction)rooms[3].getDirections()[2] + " " + (Enums.Direction)rooms[3].getDirections()[3]);
        Debug.Log("Room: " + rooms[4].getRoomNumber() + " Richtungen: " + (Enums.Direction)rooms[4].getDirections()[0] + " " + (Enums.Direction)rooms[4].getDirections()[1] + " " + (Enums.Direction)rooms[4].getDirections()[2] + " " + (Enums.Direction)rooms[4].getDirections()[3]);
        Debug.Log("Room: " + rooms[5].getRoomNumber() + " Richtungen: " + (Enums.Direction)rooms[5].getDirections()[0] + " " + (Enums.Direction)rooms[5].getDirections()[1] + " " + (Enums.Direction)rooms[5].getDirections()[2] + " " + (Enums.Direction)rooms[5].getDirections()[3]);
        Debug.Log("Room: " + rooms[6].getRoomNumber() + " Richtungen: " + (Enums.Direction)rooms[6].getDirections()[0] + " " + (Enums.Direction)rooms[6].getDirections()[1] + " " + (Enums.Direction)rooms[6].getDirections()[2] + " " + (Enums.Direction)rooms[6].getDirections()[3]);
    }

    void checkNeighbourhood(int i, RoomComponent neighbour, Enums.Direction direction)
    {
        switch(direction)
        {
            case Enums.Direction.NORTH:
                rooms[i].topNeighbour = neighbour;
                break;
            case Enums.Direction.EAST:
                rooms[i].rightNeighbour = neighbour;
                break;
            case Enums.Direction.SOUTH:
                rooms[i].bottomNeighbour = neighbour;
                break;
            case Enums.Direction.WEST:
                rooms[i].leftNeihghbour = neighbour;
                break;
        }
        Debug.Log(rooms[0].rightNeighbour);
    }


    private void getCompleteSequence()
    {    
        for(int i = 0; i < rooms.Length * 4; i++)
        {
            generator.getNextNumber(0, 100);
        }
    }

   /* private void setDoorAmount(float value)
    {
        if(value <= 24.99f)
        {
            sequence[5 * pointerOnRoom] = 1.0f;
        }
        else if (value > 24.99f && value <= 49.99f)
        {
            sequence[5 * pointerOnRoom] = 2.0f;
        }
        else if (value > 49.99f && value <= 74.99f)
        {
            sequence[5 * pointerOnRoom] = 3.0f;
        }
        else if (value > 74.99f && value < 100.0f)
        {
            sequence[5 * pointerOnRoom] = 4.0f;
        }

        
        //Sets door amount on room pointer is directing
        rooms[pointerOnRoom].Init(5, 7, sequence[5 * pointerOnRoom]);
    } */

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


        /* for (int i = 4; i >= maxDoors - rooms[pointerOnRoom].getNumOfDoors(); i--)
         {
             rooms[pointerOnRoom].addDirection(calculateDoorDirection(sequence[5 * pointerOnRoom + i]));
         } */
    }
    
    public void moveForward(Vector3Int pos, Enums.Direction direction)
    {
        List<int> directions = rooms[pointerOnRoom].getDirections();

        
        switch(direction)
        {

            //Pointer muss korrekt hochgesetzt werden falls eine Richtung bspw. ein DeadEnd ist (Norden = 1, dann muss Osten +1 und nicht +2)
            case Enums.Direction.NORTH:
                pointerOnRoom = rooms[pointerOnRoom].topNeighbour.getRoomNumber();
                pos -= new Vector3Int(1, rooms[pointerOnRoom].getHeight()+2, 0);
                break;
            case Enums.Direction.EAST:
                pointerOnRoom = pointerOnRoom = rooms[pointerOnRoom].rightNeighbour.getRoomNumber();
                pos -= new Vector3Int(rooms[pointerOnRoom].getWidth()+2, -1 , 0);
                break;
            case Enums.Direction.SOUTH:
                pointerOnRoom = rooms[pointerOnRoom].bottomNeighbour.getRoomNumber();
                pos += new Vector3Int(rooms[pointerOnRoom].getWidth()/2, rooms[pointerOnRoom].getHeight(), 0);
                break;
            case Enums.Direction.WEST:
                //Something goes wrong here
                pointerOnRoom = rooms[pointerOnRoom].leftNeihghbour.getRoomNumber();
                break;  
        } 

        calculateDirections();
        GameObject.Find("Player").transform.position = pos;
        roomBuilder.buildRoom(rooms[pointerOnRoom]);
    }






    /*private void continueSequence()
    {
        isFinished = false;
        //RN-Loop for first room
        //Export the if-cases in seperated classes
        while (!isFinished)
        {

            generator.getNextNumber(0, 100);

            if (sequence.Count == 4)
            {
                if (sequence[sequence.Count - 1] <= 24.99f)
                {
                    generator.getNextNumber(0, 100);
                    sequence[3] = 1.0f;
                    isFinished = true;
                }
                else if (sequence[sequence.Count - 1] > 24.99f && sequence[sequence.Count - 1] <= 49.99f)
                {
                    generator.getNextNumber(0, 100);
                    generator.getNextNumber(0, 100);
                    sequence[3] = 2.0f;
                    isFinished = true;
                }
                else if (sequence[sequence.Count - 1] > 49.99f && sequence[sequence.Count - 1] <= 74.99f)
                {
                    generator.getNextNumber(0, 100);
                    generator.getNextNumber(0, 100);
                    generator.getNextNumber(0, 100);
                    sequence[3] = 3.0f;
                    isFinished = true;
                }
                else if (sequence[sequence.Count - 1] > 74.99f && sequence[sequence.Count - 1] <= 100.00f)
                {
                    generator.getNextNumber(0, 100);
                    generator.getNextNumber(0, 100);
                    generator.getNextNumber(0, 100);
                    generator.getNextNumber(0, 100);
                    sequence[3] = 4.0f;
                    isFinished = true;
                }
            }
        }

        rooms[numOfRooms-1] = new RoomComponent();
        int x = Mathf.RoundToInt(sequence[0]);
        int y = Mathf.RoundToInt(sequence[1]);

        //rooms[numOfRooms-1].Init(7, 5, new Vector3Int(x, y, 0), sequence[3]);


        int cnt = 0;
        while (cnt < Mathf.RoundToInt(sequence[3]))
        {
            rooms[0].addDirection(calculateDoorDirection(sequence[3 + cnt + 1]));
            cnt++;
        }

        roomBuilder.buildRoom(rooms[0]);
    } */



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

    /*private int calculateDoorDirection(float value)
    {
        int direction;
        if (value >= 0.0f && value <= 24.99f)
            direction = 0;
        else if (value >= 25.0f && value <= 49.99f)
            direction = 1;
        else if (value >= 50.0f && value <= 74.99f)
            direction = 2;
        else if (value >= 75.0f && value <= 100.00f)
            direction = 3;
        else
            direction = -1;

        return direction;
    } */
}
