using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationTest : MonoBehaviour
{
    private Animator anim;

	// Use this for initialization
	void Start ()
    {
        anim = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKey(KeyCode.A))
        {
            anim.SetTrigger("Dash");
        }

        if (Input.GetKeyDown(KeyCode.C))
        {
            anim.SetTrigger("Catch");
        }
	}
}
