using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Minimap : MonoBehaviour {

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
    Texture miniDoor;

    [SerializeField]
    Texture inactiveRoom;

    [SerializeField]
    Texture pirateIcon;

    private bool newRoom = false;

    private RoomComponent currentRoom;

    private int pointerOnRoom = 0;

    public Enums.Direction fromDirection;


	// Use this for initialization
	void Awake () {
        canvas = GameObject.Find("Canvas");
        minimap = canvas.transform.GetChild(1);

        visitedRooms = new List<RoomComponent>();

        

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnGUI()
    {
        foreach(RoomComponent room in visitedRooms)
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
            //newRoom = false;
        }
        /*
        Rect doorPosition = new Rect(roomPosition.x, roomPosition.y, width / 2, height / 2);
        doorPosition.x += doorPosition.width / 2;
        doorPosition.y -= doorPosition.height / 2;
        Debug.Log("Rect-Position: " + roomPosition.position);
        
        
        GUI.DrawTexture(doorPosition, miniDoor);
        */

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
        foreach(Enums.Direction direction in currentRoom.getDirections())
        {

        }
    }


    private void drawDoor()
    {

    }
}
