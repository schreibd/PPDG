using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputSystem : MonoBehaviour {

    Rigidbody2D m_PlayerRigid;

    MovementComponent m_PlayerMoveComp;

    Transform m_PlayerTransform;

    Animator animator;

 
	// Use this for initialization
	void Start () {
        m_PlayerRigid = GameObject.Find("Player").GetComponent<Rigidbody2D>();
        m_PlayerMoveComp = GameObject.Find("Player").GetComponent<MovementComponent>();
        m_PlayerTransform = GameObject.Find("Player").GetComponent<Transform>();

        animator = GameObject.Find("Animations").GetComponent<Animator>();
        
	}
	
	// Update is called once per frame
	void FixedUpdate () {

        var move = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0);
        if (move.x < 0.0f)
        {
            //Debug.Log(Input.GetAxis("Horizontal"));
            animator.SetBool("right", false);
            animator.SetBool("idle", false);
            
            //m_PlayerTransform.localScale.Set(-1, m_PlayerTransform.localScale.y, m_PlayerTransform.localScale.z);
        }

        if(Input.GetButtonDown("Fire1"))
        {
            animator.SetBool("attacking", true);
        }

        if(animator.GetCurrentAnimatorStateInfo(0).IsName("Attack R") || animator.GetCurrentAnimatorStateInfo(0).IsName("Attack L"))
        {
            animator.SetBool("attacking", false);
        }

        if(move.x == 0.0f)
        {
            animator.SetBool("idle", true);
        }

        if(move.x > 0.0f)
        {
            animator.SetBool("right", true);
            animator.SetBool("idle", false);
        }
        transform.position += move * m_PlayerMoveComp.getVelocity() * Time.deltaTime;
    }
}
