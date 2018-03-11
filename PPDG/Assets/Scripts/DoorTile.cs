using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DoorTile : Tile {


    Sprite texture;
    ColliderType collType;
    public Enums.Direction direction;
    public bool locked = false;


}
