using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField]
    private float moveForce;
    [SerializeField]
    private float jumpForce;

    private Rigidbody2D _rigidbody2D;

    private int _horizontalInput;

    //============================================================================================================//
    // Start is called before the first frame update
    private void Start()
    {
        _rigidbody2D = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKey(KeyCode.A) == false && Input.GetKey(KeyCode.D) == false)
            _horizontalInput = 0;
        else
        {
            if(Input.GetKeyDown(KeyCode.A))
                _horizontalInput = -1;
            else if(Input.GetKeyDown(KeyCode.D))
                _horizontalInput = 1;
        }
        
        if(Input.GetKeyDown(KeyCode.Space))
            _rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    private void FixedUpdate()
    {
        var vel = _rigidbody2D.velocity;
        vel.x = (moveForce * _horizontalInput);
        _rigidbody2D.velocity = vel;
    }
    //============================================================================================================//
}
