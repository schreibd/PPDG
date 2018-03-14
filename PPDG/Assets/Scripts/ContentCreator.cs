using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContentCreator : MonoBehaviour {

    public static ContentCreator Instance { get; private set; }
	// Use this for initialization
	void Awake () {

        if (!Instance)
        {
            Instance = this;
        }
        else
            Destroy(gameObject);
	}
	
	// Update is called once per frame
	void Update () {
		
	}



    public void lockDoors(List<RoomComponent> rooms)
    {

        int numOfRooms = rooms.Count - 1;
        int roomToLock = (int)RNGenerator.Instance.getNextNumber(0, numOfRooms);

        //Debug.Log(rooms[roomToLock].getDoors().Count);
        //Debug.Log("RoomToLock: " + roomToLock);
        // Debug.Log("Raum hat soviele Türen: " + rooms[roomToLock].getDoors().Count);



        while (roomToLock == 0 || rooms[roomToLock].getDirections().Count != 1)
        {
            //Debug.Log("Neuer Versuch");
            roomToLock = (int)RNGenerator.Instance.getNextNumber(0, numOfRooms);
        }

        //Debug.Log("Raum der abgeschlossen werden soll: " + roomToLock);


        int direction = rooms[roomToLock].getDirections()[0];


        //Debug.Log("Richtung die abgeschlossen werden soll: " + (Enums.Direction)direction);


        RoomComponent room = rooms[roomToLock];
        //RoomComponent room = rooms[roomToLock];

        RoomComponent lockedOut = room.getNeighbour(direction);

        DoorTile lockedDoor = DoorTile.CreateInstance<DoorTile>();
        //lockedDoor.direction = room.findDirection(rooms[roomToLock]);
        lockedDoor.direction = lockedOut.findDirection(room);
        //Debug.Log("Direktion : " + lockedDoor.direction);
        lockedDoor.locked = true;

        lockedOut.addDoor(lockedDoor);

        placeKeys(1, numOfRooms, roomToLock, direction, rooms, room);

    }

    private void placeKeys(int keys, int numOfRooms,  int roomToLock, int direction, List<RoomComponent> rooms, RoomComponent room)
    {
        //Schlüssel darf nicht in einem der Räume hinter der Tür sein! DOETT FAGGOT
        int keyRoom = (int)RNGenerator.Instance.getNextNumber(0, numOfRooms);
        while (keyRoom == roomToLock || keyRoom == 0 || keyRoom == room.getNeighbour(direction).getRoomNumber())
            keyRoom = (int)RNGenerator.Instance.getNextNumber(0, numOfRooms);


        //Debug.Log("Schlüssel ist in Raum: " + keyRoom);



        GameObject key = Instantiate(Resources.Load("Key", typeof(GameObject))) as GameObject;
        key.SetActive(false);
        rooms[keyRoom].addKey(key);

        //Debug.Log(rooms[roomToLock].getDoors().Count);
    }
}
