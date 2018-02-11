using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RNGenerator : MonoBehaviour{


   /* private string[] rooms;
    public int numOfRooms = 2;

    public bool isFinished = false;
    private string[] monsters;
    private Random.State oldState; */


    private List<float> numberSequence;
    // Use this for initialization

    /*void Start()
    {
        Random.InitState(playerSeed.GetHashCode());
        numberSequence = new List<float>();
        while(!isFinished)
        {
            numberSequence.Add(Random.Range(0, 100));

            if(numberSequence.Count == 2)
            {
                float temp = numberSequence[1];
                if(temp >= 0.0f && temp <= 24.99f)
                {

                }


            }
        }  
        
    } */

    public void init(string input, List<float> sequence)
    {
        Random.InitState(input.GetHashCode());
        numberSequence = sequence;
    }

    public float getNextNumber(float min, float max)
    {
        float value = Random.Range(min, max);
        numberSequence.Add(value);
        return value;
    }

    public List<float> getSequence()
    {
        return numberSequence;
    }
}
