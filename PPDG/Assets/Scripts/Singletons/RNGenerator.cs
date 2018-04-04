using System.Collections;
using System.Collections.Generic;
using UnityEngine;



//Singleton concept to have just one instance of this class
//Won't get destroyed (including Gameobject attached on) when loading a new scene
public class RNGenerator : MonoBehaviour{

    public static RNGenerator Instance { get; private set; }

    private void Awake()
    {
        if (!Instance)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Use this for initialization
    //Sets seed as state for rn generation
    public void init(string input)
    {
        Random.InitState(input.GetHashCode());
    }


    //Is used to randomize the number of rooms per level stage
    public int getInitValues(int level)
    {
        float min = 5;
        float max = 7;
        float c = 1.5f;

        float minimum = min * (c * (level-1)) + min;
        float maximum = max * (c * (level-1)) + max;

        int result = Mathf.RoundToInt(Random.Range(minimum, maximum));

        return result;
    }

    //Is used to get 4 possible randomized directions per room
    public List<float> getRoomSequence(int numOfRooms)
    {
        List<float> result = new List<float>();

        for(int i = 0; i < numOfRooms * 4; i++)
        {
            result.Add(getNextNumber(0, 100));
        }

        return result;
    }

    //Returns the next pseudo-random number with our seed
    public float getNextNumber(float min, float max)
    {
        return Random.Range(min, max); 
    }
}
