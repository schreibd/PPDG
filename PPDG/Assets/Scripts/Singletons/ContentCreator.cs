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

    public void lockDoors(List<RoomComponent> rooms)
    {

        int numOfRooms = rooms.Count - 1;
        int roomToLock = (int)RNGenerator.Instance.getNextNumber(0, numOfRooms);

        while (roomToLock == 0 || rooms[roomToLock].getDirections().Count != 1)
        {
            roomToLock = (int)RNGenerator.Instance.getNextNumber(0, numOfRooms);
        }

        int direction = rooms[roomToLock].getDirections()[0];

        RoomComponent room = rooms[roomToLock];

        RoomComponent lockedOut = room.getNeighbour(direction);

        DoorTile lockedDoor = DoorTile.CreateInstance<DoorTile>();

        lockedDoor.direction = lockedOut.findDirection(room);

        lockedDoor.locked = true;

        lockedOut.addDoor(lockedDoor);

        placeKeys(1, numOfRooms, roomToLock, direction, rooms, room);

        addExit(room);
    }

    private void addExit(RoomComponent room)
    {
        DoorTile exit = DoorTile.CreateInstance<DoorTile>();
        exit.direction = Enums.Direction.DOWN;
        room.addDoor(exit);
    }


    private void placeKeys(int keys, int numOfRooms,  int roomToLock, int direction, List<RoomComponent> rooms, RoomComponent room)
    {
        int keyRoom = (int)RNGenerator.Instance.getNextNumber(0, numOfRooms);
        while (keyRoom == roomToLock || keyRoom == 0 || keyRoom == room.getNeighbour(direction).getRoomNumber())
            keyRoom = (int)RNGenerator.Instance.getNextNumber(0, numOfRooms);

        GameObject key = Instantiate(Resources.Load("Key", typeof(GameObject))) as GameObject;
        key.SetActive(false);
        rooms[keyRoom].addKey(key);

    }
}
