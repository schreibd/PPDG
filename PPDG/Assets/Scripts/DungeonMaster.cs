using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonMaster : MonoBehaviour {

    [SerializeField]
    private string playerSeed;

    private RNGenerator generator;

    public List<float> sequence;

    bool isFinished;

    int numOfRooms = 1;

    RoomComponent[] rooms = new RoomComponent[10];

    //Class who's generating floor/walls/doors and glues it together
    RoomBuilder roomBuilder;

	// Use this for initialization
	void Start () {

        roomBuilder = this.gameObject.GetComponent<RoomBuilder>();
        sequence = new List<float>();
        generator = this.GetComponent<RNGenerator>();
    
        generator.init(playerSeed, sequence);

        continueSequence(); 
	}

    

    private void continueSequence()
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
        /* Debug.Log("Num of elements: " + sequence.Count);
        foreach (float num in sequence)
            Debug.Log(num); */

        rooms[numOfRooms-1] = new RoomComponent();
        int x = Mathf.RoundToInt(sequence[0]);
        int y = Mathf.RoundToInt(sequence[1]);

        rooms[numOfRooms-1].Init(7, 5, new Vector3Int(x, y, 0), sequence[3]);


        int cnt = 0;
        while (cnt < Mathf.RoundToInt(sequence[3]))
        {
            rooms[0].addDirection(calculateDoorDirection(sequence[3 + cnt + 1]));
            cnt++;
        }

        roomBuilder.buildRoom(rooms[0]);
    }



    private int calculateDoorDirection(float value)
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
    }
}
