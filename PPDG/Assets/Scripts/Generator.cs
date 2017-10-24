using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour {
    private string[] rooms;
    private string[] monsters;
    private Random.State oldState;
    [SerializeField]
    private string playerSeed;
    // Use this for initialization
    void Start () {
        Random.InitState(playerSeed.GetHashCode());
        rooms = new string[10];
        monsters = new string[10];
        string roomsComplete = "";
        string monstersComplete = "";
        for(int i = 0; i < rooms.Length; i++)
        {
            rooms[i] = Random.Range(0, 100).ToString();
            roomsComplete = string.Concat(roomsComplete, rooms[i]);
            Debug.Log("Room " + i + ": " + rooms[i]);
        }

        for(int i = 0; i < monsters.Length; i++)
        {
            monsters[i] = Random.Range(0, 100).ToString();
            monstersComplete = string.Concat(monstersComplete, monsters[i]);
            Debug.Log("Monster " + i + ": " + monsters[i]);

        }
        Debug.Log("Complete Rooms: " + roomsComplete);
        Debug.Log("Complete Monsters: " + monstersComplete);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
