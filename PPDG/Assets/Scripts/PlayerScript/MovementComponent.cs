using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour {

    [SerializeField]
    private float m_playerVelocity;
    
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public float getVelocity()
    {
        return m_playerVelocity;
    }

}
