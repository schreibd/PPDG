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

    public Minimap minimap;

    [SerializeField]
    public Sprite wallTexture;

    [SerializeField]
    public Sprite floorTexture;

    [SerializeField]
    public Sprite doorTexture;

    [SerializeField]
    public Sprite lockDoorTexture;

    [SerializeField]
    public int width;

    [SerializeField]
    public int height;

    List<int> builtRooms;

    int numOfRooms = 1;

    RoomComponent currentRoom;


    // Use this for initialization 
    void Awake () {

         if (instance == null)
        {
            instance = this;
        }
            
        else if (instance != gameObject)
        {
            Destroy(gameObject);
        }

        builtRooms = new List<int>();
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


        minimap = GameObject.Find("Canvas").transform.GetChild(0).GetComponent<Minimap>();
    }



    public void buildRoom(RoomComponent roomData, Enums.Direction direction, RNGenerator generator, List<GameObject> activeMonsters)
    {
        clearRoom();
        currentRoom = roomData;
        drawFloor();
        drawWalls();


        // int cnt = 0; 
        int lockedDoors = roomData.getDoors().Count;
        int cnt = 0;
        while (cnt < 4)
        {
            DoorTile door = DoorTile.CreateInstance<DoorTile>();
            door.sprite = doorTexture;
            door.colliderType = Tile.ColliderType.Sprite;
            door.direction = (Enums.Direction)currentRoom.getDirections()[cnt];
            if(!currentRoom.hasDoor(door))
                currentRoom.addDoor(door);
            else
            {
                door = currentRoom.findDoor(door);
                if (door.locked)
                    door.sprite = lockDoorTexture;
                else
                    door.sprite = doorTexture;
                door.colliderType = Tile.ColliderType.Sprite;
            }
            cnt++;
        }



        insertMissingDoors();

        
        foreach (DoorTile door in currentRoom.getDoors())
        {
            buildDoors(door);
        }

        if(!builtRooms.Contains(currentRoom.getRoomNumber()))
            builtRooms.Add(currentRoom.getRoomNumber());

        if (currentRoom.getKey())
            currentRoom.getKey().SetActive(true);
        

        minimap.drawRoom(currentRoom, direction);
        //saveRoom();

        spawnMonster(generator, activeMonsters);
        

    }

    public void spawnMonster(RNGenerator generator, List<GameObject> activeMonsters)
    {
        foreach (GameObject actM in activeMonsters)
            Destroy(actM);
        //GameObject key = Instantiate(Resources.Load("Key", typeof(GameObject))) as GameObject;
        foreach(Enums.Monster monster in currentRoom.getMonsters())
        {
            int x = MonsterSpawner.calcX(generator, width);
            int y = MonsterSpawner.calcY(generator, height);

            float posX = currFloorMap.CellToWorld(new Vector3Int(x, y, 0)).x;
            float posY = currFloorMap.CellToWorld(new Vector3Int(x, y, 0)).y;

            GameObject temp = Instantiate(Resources.Load(monster.ToString(), typeof(GameObject))) as GameObject;
            temp.transform.position.Set(posX, posY, 0);

            activeMonsters.Add(temp);

            
        }
    }


    public void insertMissingDoors()
    {
        int cnt = 1;
        while (cnt <= 4)
        {
            if (currentRoom.hasNeighbour(cnt) && !currentRoom.hasDirection(cnt))
            {
                
                DoorTile door = DoorTile.CreateInstance<DoorTile>();
                door.sprite = doorTexture;
                door.colliderType = Tile.ColliderType.Sprite;
                door.direction = (Enums.Direction)cnt;
                currentRoom.addDoor(door);

            }
            cnt++;
        }
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
            currWallMap.SetTile(new Vector3Int(i, currentRoom.getHeight()-1, 0) + hOffset, tile);
        }
        
        for(int j = 0; j < height; j++)
        {
            tile = Tile.CreateInstance<Tile>();
            tile.sprite = wallTexture;
            currWallMap.SetTile(new Vector3Int(-1, j, 0),tile);
            currWallMap.SetTile(new Vector3Int(currentRoom.getWidth() + 2, j, 0), tile);
        }
    }



    void buildDoors(DoorTile door)
    {
        /* Matrix4x4 identity = Matrix4x4.identity;
         Quaternion identityQ = Quaternion.identity;
         identity = Matrix4x4.Rotate(identityQ);
         door.transform *= identity;
         */
        if (!currentRoom.hasNeighbour((int)door.direction))
            return;


        switch (door.direction)
        {
            case Enums.Direction.NORTH:
                currWallMap.SetTile(new Vector3Int((width / 2), height, 0), null);
                currDoorMap.SetTile(new Vector3Int((width / 2), height, 0), door);
                break;
            case Enums.Direction.EAST:
                rotateDoor(door);
                //rotation = Quaternion.Euler(0, 0, -90.0f);
                //rotationM = Matrix4x4.Rotate(rotation);
                //door.transform *= rotationM;
                currWallMap.SetTile(new Vector3Int(width, height / 2, 0), null);
                currDoorMap.SetTile(new Vector3Int(width, height / 2, 0), door);
                break;
            case Enums.Direction.SOUTH:
                rotateDoor(door);
                //rotation = Quaternion.Euler(0, 0, 180.0f);
                //rotationM = Matrix4x4.Rotate(rotation);
                //door.transform *= rotationM;
                currWallMap.SetTile(new Vector3Int((width / 2), -1, 0), null);
                currDoorMap.SetTile(new Vector3Int((width / 2), -1, 0), door);
                break;
            case Enums.Direction.WEST:
                rotateDoor(door);
                //rotation = Quaternion.Euler(0, 0, 90.0f);
                //rotationM = Matrix4x4.Rotate(rotation);
                //door.transform *= rotationM;
                currWallMap.SetTile(new Vector3Int(-1, height / 2, 0), null);
                currDoorMap.SetTile(new Vector3Int(-1, height / 2, 0), door);
                break;
            default:
                break;
        }
    }


    void rotateDoor(DoorTile door)
    {
        if (builtRooms.Contains(currentRoom.getRoomNumber()))
        {
            return;
        }
            
        else
        {
            //Stores the rotation we want to use in degree 
            Quaternion rotation;
            //Matrix we need to use to rotate our tile to right direction
            Matrix4x4 rotationM;

            switch (door.direction)
            {
                case Enums.Direction.EAST:
                    rotation = Quaternion.Euler(0, 0, -90.0f);
                    rotationM = Matrix4x4.Rotate(rotation);
                    door.transform *= rotationM;
                    break;
                case Enums.Direction.SOUTH:
                    rotation = Quaternion.Euler(0, 0, 180.0f);
                    rotationM = Matrix4x4.Rotate(rotation);
                    door.transform *= rotationM;
                    break;
                case Enums.Direction.WEST:
                    rotation = Quaternion.Euler(0, 0, 90.0f);
                    rotationM = Matrix4x4.Rotate(rotation);
                    door.transform *= rotationM;
                    break;
            }
        }

    }
}
