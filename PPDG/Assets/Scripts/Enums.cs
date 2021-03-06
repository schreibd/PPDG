﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour {

    public enum Direction { DEADEND,  NORTH, EAST, SOUTH, WEST, DOWN};
    public enum Difficulty { VERY_EASY, EASY, NORMAL, HARD, NUTS };
    public enum Monster { NONE, SKELETON, JOE, WILLIAM, JACK, AVERELL, LUCKY, LUKE };
    public enum DoorType { NORMAL, BOSSDOOR, EXIT};

}
