using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Singleton concept to have just one instance of this class
//Won't get destroyed (including Gameobject attached on) on loading new scene

public class RNGenerator : MonoBehaviour{

    public static RNGenerator Instance { get; private set; }
    // Use this for initialization
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

    private List<float> numberSequence;
    // Use this for initialization

    //Sets seed as state for rn generation
    public void init(string input, List<float> sequence)
    {
        Random.InitState(input.GetHashCode());
        numberSequence = sequence;
    }


    //Can be used to get some random numbers for level instantiation
    public int getInitValues(int level)
    {
        float min = 5;
        float max = 7;
        float c = 1.5f;
        float minimum = min * (c * (level-1)) + min;
        float maximum = max * (c * (level-1)) + max;
        Debug.Log("Minimum: " + minimum);
        Debug.Log("Maximum: " + maximum);
        int result = Mathf.RoundToInt(Random.Range(minimum, maximum));
        Debug.Log("Result: " + result);
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
