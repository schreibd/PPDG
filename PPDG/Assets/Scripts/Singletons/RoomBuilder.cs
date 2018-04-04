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
    public Sprite exitTexture;

    [SerializeField]
    public int width;

    [SerializeField]
    public int height;

    List<int> builtRooms;

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

        //Gets the Tilemap we want to use for our walls and adds a TilemapCollider2D
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

    //Gets called when the next level is reached
    //Clears the list of already built rooms for usage in next level
    public void clearBuilderData()
    {
        builtRooms = new List<int>();
    }


    //Main method to build rooms
    //Gets called by the DungeonMaster
    public void buildRoom(RoomComponent roomData, Enums.Direction direction, List<GameObject> activeMonsters)
    {
        //Clear all tilemaps before drawing
        clearRoom();
        currentRoom = roomData;
        drawFloor();
        drawWalls();

        createDoorData(roomData.getDirections().Count);

        insertMissingDoors();

        buildDoors();

        if(!builtRooms.Contains(currentRoom.getRoomNumber()))
            builtRooms.Add(currentRoom.getRoomNumber());

        if (currentRoom.getKey())
            currentRoom.getKey().SetActive(true);

        spawnMonster(activeMonsters);

        minimap.drawRoom(currentRoom, direction);
    }

    public void spawnMonster(List<GameObject> activeMonsters)
    {
        //Clear all active enemies from the previous room
        foreach (GameObject actM in activeMonsters)
            Destroy(actM);

        //Loops over all stored monster enums in room
        foreach(Enums.Monster monster in currentRoom.getMonsters())
        {
            //x and y positioning of the monster within room boundaries
            int x = MonsterRuleset.Instance.calcX(width);
            int y = MonsterRuleset.Instance.calcY(height);

            float posX = currFloorMap.CellToWorld(new Vector3Int(x, 0, 0)).x + 0.5f;
            float posY = currFloorMap.CellToWorld(new Vector3Int(0, y, 0)).y;

            //Instantiate the enemy we want to spawn
            GameObject temp = Instantiate(Resources.Load(monster.ToString(), typeof(GameObject))) as GameObject;

            temp.transform.localPosition = new Vector3(posX, posY, 0);

            //Adds the monster to our list of active monsters
            activeMonsters.Add(temp);  
        }
    }

    public void createDoorData(int dirCnt)
    {
        int cnt = 0;
        while (cnt < dirCnt)
        {
            DoorTile door = DoorTile.CreateInstance<DoorTile>();
            door.sprite = doorTexture;
            door.colliderType = Tile.ColliderType.Sprite;
            door.direction = (Enums.Direction)currentRoom.getDirections()[cnt];
            if (!currentRoom.hasDoor(door))
                currentRoom.addDoor(door);
            else
            {
                door = currentRoom.findDoor(door);
                if (door.locked)
                    door.sprite = lockDoorTexture;
                else
                    door.sprite = doorTexture;
            }
            cnt++;
        }
    }

    //Used to make sure that there are doors where they need to be
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

    //Clears all tilemaps
    void clearRoom()
    {
        currWallMap.ClearAllTiles();
        currFloorMap.ClearAllTiles();
        currDoorMap.ClearAllTiles();
    }

    //Used for drawing floor tiles
    void drawFloor()
    {
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

    //Used for drawing wall tiles
    void drawWalls()
    {
        Tile tile;
        Vector3Int offset = new Vector3Int(-1, -1, 0);
        for(int i = 0; i < width + 2; i++)
        {
            tile = Tile.CreateInstance<Tile>();
            tile.sprite = wallTexture;
            currWallMap.SetTile(new Vector3Int(i, 0, 0)+offset, tile);
            currWallMap.SetTile(new Vector3Int(i, currentRoom.getHeight()-1, 0) + offset, tile);
        }
        
        for(int j = 0; j < height; j++)
        {
            tile = Tile.CreateInstance<Tile>();
            tile.sprite = wallTexture;
            currWallMap.SetTile(new Vector3Int(-1, j, 0),tile);
            currWallMap.SetTile(new Vector3Int(currentRoom.getWidth() + 2, j, 0), tile);
        }
    }


    //Used to draw door tiles on correct positions
    private void buildDoors()
    {
        foreach (DoorTile door in currentRoom.getDoors())
        {
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
                //Used for stairs to the next level
                case Enums.Direction.DOWN:
                    door.sprite = exitTexture;
                    currFloorMap.SetTile(new Vector3Int(width / 2, height / 2, 0), null);
                    currDoorMap.SetTile(new Vector3Int(width / 2, height / 2, 0), door);
                    break;
                default:
                    break;
            }
        }
    }


    void rotateDoor(DoorTile door)
    {
        //If the room has been built before, doors already got rotated in right direction
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
