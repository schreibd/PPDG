using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorController : MonoBehaviour {

    Tilemap doormap;
    DungeonMaster master;
    
    // Use this for initialization
    void Start () {
        doormap = GameObject.Find("Grid").transform.GetChild(2).GetComponent<Tilemap>();
        master = GameObject.Find("DungeonManager").GetComponent<DungeonMaster>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    
    //Used for retrieving right exit so player can leave room to the next one
    void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            
            Vector3Int tilePos = doormap.WorldToCell(GameObject.Find("Player").transform.position);

            //Check if door
            if (doormap.GetTile<DoorTile>(tilePos))
            {
                //Check direction player exits room
                //Debug.Log(doormap.GetTile<DoorTile>(tilePos).direction);
                master.moveForward(tilePos, doormap.GetTile<DoorTile>(tilePos).direction);
                //TODO: Teleport player to next room
            }

        }


        
    }
}
