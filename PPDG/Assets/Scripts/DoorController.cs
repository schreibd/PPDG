using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

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
            DoorTile door = doormap.GetTile<DoorTile>(tilePos);

            //Check if door
            if (door)
            {
                if(door.locked && collision.gameObject.GetComponent<PlayerComponent>().getKeys() > 0)
                {
                    door.locked = false;
                    collision.gameObject.GetComponent<PlayerComponent>().decrKeys();
                    master.moveForward(tilePos, doormap.GetTile<DoorTile>(tilePos).direction);
                    GameObject.Find("KeyCounter").GetComponent<Text>().text = collision.gameObject.GetComponent<PlayerComponent>().getKeys().ToString();
                }
                else if(!door.locked)
                    master.moveForward(tilePos, doormap.GetTile<DoorTile>(tilePos).direction);
                //Check direction player exits room
                //Debug.Log(doormap.GetTile<DoorTile>(tilePos).direction);

                //TODO: Teleport player to next room
            }

        }


        
    }
}
