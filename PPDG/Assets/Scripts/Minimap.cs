using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {

    public static Minimap Instance { get; private set; }
    public GameObject canvas;
    public Transform minimap;
    public List<RoomComponent> visitedRooms;

    [SerializeField]
    public float width;
    [SerializeField]
    public float height;

    [SerializeField]
    Texture miniRoom;

    [SerializeField]
    Texture blankRoom;


    [SerializeField]
    Texture miniDoor;

    [SerializeField]
    Texture inactiveRoom;

    [SerializeField]
    Texture pirateIcon;

    private bool newRoom = false;

    private RoomComponent currentRoom;

    private int pointerOnRoom = 0;

    public Enums.Direction fromDirection;

    bool clear = false;


	// Use this for initialization
	void Awake () {

        if (Instance == null)
        {
            Instance = this;
        }


        canvas = this.gameObject.transform.parent.gameObject; 

        minimap = canvas.transform.GetChild(1);

        visitedRooms = new List<RoomComponent>();

	}

    public void clearMinimap()
    {
        clear = true;

        /*
        GameObject oldCanvas = canvas;
        canvas = Instantiate(Resources.Load("Canvas", typeof(GameObject))) as GameObject;
        
        canvas.name = "Canvas";
        float x = oldCanvas.transform.GetChild(1).position.x;
        float y = oldCanvas.transform.GetChild(1).position.y;
        float width = canvas.transform.GetChild(1).GetComponent<RectTransform>().rect.width;
        float height = canvas.transform.GetChild(1).GetComponent<RectTransform>().rect.height;
        Destroy(oldCanvas);
        Rect rect = new Rect(new Vector2(x, y), new Vector2(width, height));
        GUI.DrawTexture(rect, miniRoom);
        */
    }

    public void clearScreen(bool clear)
    {
        this.clear = clear;
        visitedRooms = new List<RoomComponent>();
        //GUI.DrawTexture
    }

    private void OnGUI()
    {
        /*
        if(clear)
        {
            foreach (RoomComponent room in visitedRooms)
            {
                Debug.Log("Ohjaaaaa");
                Rect position = room.getMinimapPosition();
                GUI.DrawTexture(room.getMinimapPosition(), blankRoom);
            }
            clear = false;
        }*/
        if(!clear)
        {
            foreach (RoomComponent room in visitedRooms)
            {
                if (room == currentRoom)
                {
                    float height = room.getMinimapPosition().height / 2;
                    float width = room.getMinimapPosition().width / 2;

                    Rect position = room.getMinimapPosition();
                    position.x += width / 2;
                    position.y += height / 2;

                    position.height = height;
                    position.width = width;
                    GUI.DrawTexture(room.getMinimapPosition(), miniRoom);
                    GUI.DrawTexture(position, pirateIcon);
                }


                else
                    GUI.DrawTexture(room.getMinimapPosition(), inactiveRoom);
            }
        }

        
    }


    public void drawRoom(RoomComponent roomData, Enums.Direction fromDirection)
    {
        float localX = 0.0f; 
        float localY = 0.0f;

        Rect roomPosition;

        if (visitedRooms.Count == 0)
        {
            localX = minimap.transform.position.x - width / 2;
            localY = Screen.height - minimap.transform.position.y - height / 2;
            roomPosition = new Rect(localX, localY, width, height);       
            roomPosition.width = width;
            roomPosition.height = height;
            roomData.setMinimapPosition(roomPosition);
            visitedRooms.Add(roomData);
        }
        else if(!visitedRooms.Contains(roomData))
        {
            localX += currentRoom.getMinimapPosition().x;
            localY += currentRoom.getMinimapPosition().y;
            
            switch(fromDirection)
            {
                case Enums.Direction.NORTH:
                    roomPosition = new Rect(localX, localY-height-5, width, height);
                    roomPosition.width = width;
                    roomPosition.height = height;
                    roomData.setMinimapPosition(roomPosition);
                    break;
                case Enums.Direction.EAST:
                    roomPosition = new Rect(localX+width + 5, localY, width, height);
                    roomPosition.width = width;
                    roomPosition.height = height;
                    roomData.setMinimapPosition(roomPosition);
                    break;
                case Enums.Direction.SOUTH:
                    roomPosition = new Rect(localX, localY+height+5, width, height);
                    roomPosition.width = width;
                    roomPosition.height = height;
                    roomData.setMinimapPosition(roomPosition);
                    break;
                case Enums.Direction.WEST:
                    roomPosition = new Rect(localX - width - 5, localY, width, height);
                    roomPosition.width = width;
                    roomPosition.height = height;
                    roomData.setMinimapPosition(roomPosition);
                    break;
            }
            visitedRooms.Add(roomData);
            currentRoom = roomData;
            
        }

        currentRoom = roomData;
    }

    public int calcXPos(RoomComponent room, Enums.Direction direction)
    {
        int xPos = 0;
        switch (direction)
        {
            case Enums.Direction.NORTH:
                xPos = room.getXPos();
                break;
            case Enums.Direction.EAST:
                xPos = room.getXPos() + 1;
                break;
            case Enums.Direction.SOUTH:
                xPos = room.getXPos();
                break;
            case Enums.Direction.WEST:
                xPos = room.getXPos() - 1;
                break;
        }
        return xPos;
    }


    public int calcYPos(RoomComponent room, Enums.Direction direction)
    {
        int yPos = 0;
        switch (direction)
        {
            case Enums.Direction.NORTH:
                yPos = room.getYPos() + 1;
                break;
            case Enums.Direction.EAST:
                yPos = room.getYPos();
                break;
            case Enums.Direction.SOUTH:
                yPos = room.getYPos() - 1;
                break;
            case Enums.Direction.WEST:
                yPos = room.getYPos();
                break;
        }
        return yPos;
    }
}
