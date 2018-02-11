﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

[Prefab("Prefabs/Singletons/DungeonManager", true)]
public class RoomBuilder : Singleton<RoomBuilder> {

    public static RoomBuilder instance = null;

    public Tilemap currFloorMap;

    public Tilemap currWallMap;

    public Tilemap currObjectMap;

    public Tilemap currDoorMap;

    public GameObject grid;

    [SerializeField]
    public Sprite wallTexture;

    [SerializeField]
    public Sprite floorTexture;

    [SerializeField]
    public Sprite doorTexture;

    [SerializeField]
    public int width;

    [SerializeField]
    public int height;

    GameObject[] rooms = new GameObject[10];

    int numOfRooms = 1;

    RoomComponent currentRoom;

    // Use this for initialization 
    void Start () {

         if (instance == null)
        {
            instance = this;
        }
            
        else if (instance != gameObject)
        {
            Destroy(gameObject);
        }

        

        //createTilemaps();
         /*

        //Set the number of rooms in this level
        rooms = new GameObject[10];
        
        

        createTilemaps();

        currentRoom = this.gameObject.AddComponent<RoomComponent>();

       // currentRoom.Init(width, height);

        DoorTile door = DoorTile.CreateInstance<DoorTile>();
        door.direction = Enums.Direction.NORTH;
        door.sprite = doorTexture;
        door.colliderType = Tile.ColliderType.Sprite;

        currentRoom.addDoor(door);

        door = DoorTile.CreateInstance<DoorTile>();
        door.direction = Enums.Direction.EAST;
        door.sprite = doorTexture;
        door.colliderType = Tile.ColliderType.Sprite;
        currentRoom.addDoor(door);


        door = DoorTile.CreateInstance<DoorTile>();
        door.direction = Enums.Direction.SOUTH;
        door.sprite = doorTexture;
        door.colliderType = Tile.ColliderType.Sprite;
        currentRoom.addDoor(door);

        door = DoorTile.CreateInstance<DoorTile>();
        door.direction = Enums.Direction.WEST;
        door.sprite = doorTexture;
        door.colliderType = Tile.ColliderType.Sprite;
        currentRoom.addDoor(door); */

    }


    public void saveRoom()
    {
        rooms[numOfRooms-1] = Instantiate<GameObject>(grid);
        rooms[numOfRooms-1].SetActive(false);
        numOfRooms++;
    }

    public void createTilemaps()
    {
        grid = GameObject.Find("Grid");
        
        currFloorMap = grid.transform.GetChild(0).GetComponent<Tilemap>();

        //Create a ColliderTilemap for walls and obstacles
        currWallMap = grid.transform.GetChild(1).GetComponent<Tilemap>();
        currWallMap.gameObject.AddComponent<TilemapCollider2D>();

        //Create a ColliderTilemap for interactable Tiles like doors/chests
        currDoorMap = grid.transform.GetChild(2).GetComponent<Tilemap>();

        currDoorMap.gameObject.AddComponent<TilemapCollider2D>().isTrigger = true;
        currDoorMap.gameObject.AddComponent<Rigidbody2D>();
        currDoorMap.GetComponent<Rigidbody2D>().gravityScale = 0;
        currDoorMap.GetComponent<Rigidbody2D>().isKinematic = true;
        currDoorMap.gameObject.AddComponent<DoorController>(); 
    }

    public void buildRoom(RoomComponent roomData)
    {

        createTilemaps();
        currentRoom = roomData;
        drawFloor();
        drawWalls();

        int cnt = 0; 
        while(cnt < currentRoom.getNumOfDoors())
        {
            DoorTile door = DoorTile.CreateInstance<DoorTile>();
            door.sprite = doorTexture;
            door.colliderType = Tile.ColliderType.Sprite;
            door.direction = (Enums.Direction)currentRoom.getDirections()[cnt];
            currentRoom.addDoor(door);
            cnt++;
        }
        foreach (DoorTile door in currentRoom.getDoors())
        {
            buildDoors(door);
        }

        saveRoom();

    }

    void clearRoom()
    {
        currWallMap.ClearAllTiles();
        currFloorMap.ClearAllTiles();
        currDoorMap.ClearAllTiles();
    }

    void drawFloor()
    {
       
        Tile[] floorTiles = new Tile[currentRoom.getWidth() * currentRoom.getHeight()];
        for(int i = 0; i < height; i++)
        {
            for(int j = 0; j < width; j++)
            {
                Tile tile = Tile.CreateInstance<Tile>();
                tile.sprite = floorTexture;
                currFloorMap.SetTile(new Vector3Int(j, i, 0), tile);
            }
        }
    }


    void drawWalls()
    {
        Tile tile;
        Vector3Int hOffset = new Vector3Int(-1, -1, 0);
        Tile[] wallTilesHorizontal = new Tile[(currentRoom.getWidth() * 2) + 4];
        Tile[] wallTilesVertical = new Tile[currentRoom.getHeight() * 2];
        for(int i = 0; i < width + 2; i++)
        {
            tile = Tile.CreateInstance<Tile>();
            //Needs variation for texture
            tile.sprite = wallTexture;
            currWallMap.SetTile(new Vector3Int(i, 0, 0)+hOffset, tile);
            currWallMap.SetTile(new Vector3Int(i, currentRoom.getHeight() + 1, 0) + hOffset, tile);
        }
        
        for(int j = 0; j < height; j++)
        {
            tile = Tile.CreateInstance<Tile>();
            tile.sprite = wallTexture;
            currWallMap.SetTile(new Vector3Int(-1, j, 0),tile);
            currWallMap.SetTile(new Vector3Int(currentRoom.getWidth(), j, 0), tile);
        }
    }

    void buildDoors(DoorTile door)
    {
        /*
        //Create new instance of a door
        DoorTile door = DoorTile.CreateInstance<DoorTile>();

        //Needs variation for texture
        door.sprite = doorTexture;
        door.colliderType = Tile.ColliderType.Sprite;
        door.direction = (Enums.Direction)direction; */

        //Stores the rotation we want to use in degree 
        Quaternion rotation;
        //Matrix we need to use to rotate our tile to right direction
        Matrix4x4 rotationM;

        switch (door.direction)
        {
            case Enums.Direction.NORTH:
                currWallMap.SetTile(new Vector3Int((width / 2), height, 0), null);
                currDoorMap.SetTile(new Vector3Int((width / 2), height, 0), door);
                break;
            case Enums.Direction.EAST:
                rotation = Quaternion.Euler(0, 0, -90.0f);
                rotationM = Matrix4x4.Rotate(rotation);
                door.transform *= rotationM;
                currWallMap.SetTile(new Vector3Int(width, height / 2, 0), null);
                currDoorMap.SetTile(new Vector3Int(width, height / 2, 0), door);
                break;
            case Enums.Direction.SOUTH:
                rotation = Quaternion.Euler(0, 0, 180.0f);
                rotationM = Matrix4x4.Rotate(rotation);
                door.transform *= rotationM;
                currWallMap.SetTile(new Vector3Int((width / 2), -1, 0), null);
                currDoorMap.SetTile(new Vector3Int((width / 2), -1, 0), door);
                break;
            case Enums.Direction.WEST:
                rotation = Quaternion.Euler(0, 0, 90.0f);
                rotationM = Matrix4x4.Rotate(rotation);
                door.transform *= rotationM;
                currWallMap.SetTile(new Vector3Int(-1, height / 2, 0), null);
                currDoorMap.SetTile(new Vector3Int(-1, height / 2, 0), door);
                break;
            default:
                break;
        }
    }


}