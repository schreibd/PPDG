using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;


public class RoomBuilder : MonoBehaviour{

    public static RoomBuilder Instance { get; private set; }

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

    int numOfRooms;

    RoomComponent currentRoom;


    // Use this for initialization 
    void Awake () {

         if (Instance == null)
        {
            Instance = this;
        }
            
        else if (Instance != gameObject)
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


        minimap = GameObject.Find("Canvas").transform.GetChild(1).GetComponent<Minimap>();
    }

    public void clearBuilderData()
    {
        builtRooms = new List<int>();
    }



    public void buildRoom(RoomComponent roomData, Enums.Direction direction, List<GameObject> activeMonsters)
    {
        clearRoom();
        currentRoom = roomData;
        drawFloor();
        drawWalls();


        // int cnt = 0; 
        //int lockedDoors = roomData.getDoors().Count;
        int cnt = 0;
        while (cnt < roomData.getDirections().Count)
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

        spawnMonster(activeMonsters);
        

    }

    public void spawnMonster(List<GameObject> activeMonsters)
    {
        foreach (GameObject actM in activeMonsters)
            Destroy(actM);

        foreach(Enums.Monster monster in currentRoom.getMonsters())
        {
            int x = MonsterSpawner.calcX(width);
            int y = MonsterSpawner.calcY(height);

            //Debug.Log("x: " + x + " y: " + y);
            float posX = currFloorMap.CellToWorld(new Vector3Int(x, 0, 0)).x + 0.5f;
            float posY = currFloorMap.CellToWorld(new Vector3Int(0, y, 0)).y;

            GameObject temp = Instantiate(Resources.Load(monster.ToString(), typeof(GameObject))) as GameObject;

            //Debug.Log("posx: " + posX + " posy: " + posY);
            //Debug.Log("WorldPosition: " + temp.transform.position);
            //Debug.Log("Root: " + temp.transform.root.gameObject.name);
            temp.transform.localPosition = new Vector3(posX, posY, 0);

            //Debug.Log(temp.transform.position);

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
                currWallMap.SetTile(new Vector3Int(width, height / 2, 0), null);
                currDoorMap.SetTile(new Vector3Int(width, height / 2, 0), door);
                break;
            case Enums.Direction.SOUTH:
                rotateDoor(door);
                currWallMap.SetTile(new Vector3Int((width / 2), -1, 0), null);
                currDoorMap.SetTile(new Vector3Int((width / 2), -1, 0), door);
                break;
            case Enums.Direction.WEST:
                rotateDoor(door);
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
