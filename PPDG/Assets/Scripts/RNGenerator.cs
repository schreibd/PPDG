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

    //Sets seed as state for rn generation
    public void init(string input, List<float> sequence)
    {
        Random.InitState(input.GetHashCode());
        numberSequence = sequence;
    }


    //Can be used to get some random numbers for level instantiation
    public int getInitValues()
    {
        int result = Mathf.RoundToInt(Random.Range(5, 9));
        return result;
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
