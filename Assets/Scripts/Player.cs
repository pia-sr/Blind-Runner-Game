
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using TextSpeech;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.UI;


using UnityEditor;
using UnityEngine.Android;

public class Player : MonoBehaviour
{
    //values for the movement changes
    private Vector3 _aimPos;
    private float _xMove;
    private float _yMove;
    public int[] blockPos = new int[] { 0, 0, 0 };
    public int playerXPos;
    private float _speed;
    private bool _move;
    
    //Animation and sound
    public Animator animator;
    public AudioSource sound;
    
    //parameter for the flat rocks
    private bool _jumpUp;
    public bool highUp;
    
    //Vectors for the touch input
    private Vector2 _startPos;
    private Vector2 _directionTouch;
    
    //Scripts
    public GameValues gameValues;
    public UI uI;
    
    //Bool for game over
    private bool _lost;
    
    // Start is called before the first frame update
    void Start()
    {
        TextToSpeech.Instance.Settings();
        playerXPos = 1;
        highUp = false;
        animator.SetInteger("animation", 2);
    }

    // Update is called once per frame
    void Update()
    {
        //when the player is on top of a flat stone and needs to move down
        if (blockPos[playerXPos] == 2 && this.transform.position.y > 0.2f && highUp)
        {
            _yMove = -0.2f;
            _move = true;
            _speed = gameValues.speed/2;
            _aimPos = new Vector3(this.transform.position.x, 0.12f, this.transform.position.z);
            highUp = false;
            this.transform.rotation = new Quaternion(-0.0729237199f, 0, 0, 0.99733758f);
        }
        
        //Touch input for the movements
        if (Input.touchCount > 0 && Time.timeScale != 0 && !_move) 
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _directionTouch = touch.position - _startPos;
                
                //Jumping / swiping up
                if (_directionTouch.y > 100)
                {
                    
                    if(this.transform.position.y < 0.25f)
                    {
                        animator.SetInteger("animation", 3);
                        _yMove = 0.2f;
                        _jumpUp = true;
                        _move = true;
                        _speed = gameValues.speed/3;
                        sound.Pause();
                    }
                    else
                    {
                        _yMove = 0;
                    }
                    _aimPos = this.transform.position + new Vector3(0, _yMove, 0);
                }
                                            
                //switching lane / swiping left and right
                else if (Math.Abs(_directionTouch.x) > 150)
                {
                    //swiping to the right
                    if (_directionTouch.x > 0 && this.transform.position.x <= 0)
                    {
                        playerXPos++;
                        _xMove = 0.31f;
                        _aimPos = this.transform.position + new Vector3(_xMove, 0, 0);
                        _move = true;
                        _speed = gameValues.speed / 2;
                        this.transform.rotation =
                            new Quaternion(-0.184428468f, 0.447648585f, -0.0224523991f, 0.874695837f);
                    }
                    //Swiping to the left
                    else if (_directionTouch.x < 0 && this.transform.position.x >= 0)
                    {
                        playerXPos--;
                        _xMove = -0.31f;
                        _aimPos = this.transform.position + new Vector3(_xMove, 0, 0);
                        _move = true;
                        _speed = gameValues.speed / 2;
                        this.transform.rotation =
                            new Quaternion(-0.184427634f, -0.447647065f, 0.0224493705f, 0.87469691f);


                    }
                }


            }
        }

        //when the player falls down from a flat stone
        if (this.transform.position.y >= 0.32f && blockPos[playerXPos] == 0 && !_move && !_jumpUp) 
        {
            _aimPos = this.transform.position + new Vector3(0, -0.2f, 0);
            highUp = false;
            _lost = true;
            _move = true;
        }
        //armadillo now moves on top of the flat stones
        else if (this.transform.position.y >= 0.32f && blockPos[playerXPos] != 0 && !_move && !_jumpUp && !highUp)
        {
            highUp = true;
        }
        //armadillo moves towards the newly set goal position
        else if (_move)
        {
            
            this.transform.position = Vector3.MoveTowards(this.transform.position, _aimPos, _speed * Time.deltaTime);
            if (this.transform.position == _aimPos)
            {
                this.transform.rotation = new Quaternion(-0.264374405f, 0, 0, 0.986168683f);
                //the player only loses after the movement is carried out
                if (_lost)
                {
                    _lost = false;
                    uI.Lost();
                    
                }
                //the armadillo moves down after a jump if there is not stone to jump on top to
                if ((!gameValues.upperLevel || (blockPos[playerXPos] == 0 && this.transform.position.y > 0.25f)) && _jumpUp)
                {
                    _aimPos = this.transform.position + new Vector3(0, -0.2f, 0);
                }
                //after the jump the armadillo continues to run
                else
                {
                    _move = false;
                    animator.SetInteger("animation", 2);
                    sound.UnPause();
                }

                _jumpUp = false;

            }
        }

        //sound is paused when game is paused
        if (Time.timeScale == 0)
        {
            sound.Pause();
        }
        else if (!sound.isPlaying && !_move)
        {
            sound.UnPause();
        }
    } 

}
