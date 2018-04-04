using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class DoorController : MonoBehaviour {

    //Tilemap this script is attached to
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
            //playerposition in grid-cell coordinates used for retrieving the right door tile
            Vector3Int tilePos = doormap.WorldToCell(GameObject.Find("Player").transform.position);
            DoorTile door = doormap.GetTile<DoorTile>(tilePos);

            if (door)
            {
                //If door is locked and player has key the door gets opened and player gets ported to next room
                if(door.locked && collision.gameObject.GetComponent<PlayerComponent>().getKeys() > 0)
                {
                    door.locked = false;
                    collision.gameObject.GetComponent<PlayerComponent>().decrKeys();
                    //DungeonMaster needs to move player to the next room
                    master.moveForward(tilePos, doormap.GetTile<DoorTile>(tilePos).direction);
                    GameObject.Find("KeyCounter").GetComponent<Text>().text = collision.gameObject.GetComponent<PlayerComponent>().getKeys().ToString();
                }
                //If door is levelexit, dungeonmaster needs to create the next level
                else if(door.direction == Enums.Direction.DOWN)
                {
                    master.nextLevel();
                }
                else if(!door.locked)
                    master.moveForward(tilePos, doormap.GetTile<DoorTile>(tilePos).direction);
            }
        }
    }
}
