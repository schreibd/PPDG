﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerComponent : MonoBehaviour {

    int keys;
	// Use this for initialization
	void Start () {
        keys = 1;
      //keys = 0;
	}

    public int getKeys()
    {
        return keys;
    }

    public void decrKeys()
    {
        keys--;
    }

    public void incrKeys()
    {
        keys +=1;
    }

    private void OnTriggerStay2D(Collider2D collision)
    { 
        if(collision.CompareTag("Key"))
        {
            incrKeys();
            GameObject.Find("KeyCounter").GetComponent<Text>().text = keys.ToString();
            Destroy(collision.gameObject);

        } 
    }
}
