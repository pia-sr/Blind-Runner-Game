
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    //Scripts
    private SpawnManager _spawnManager;
    private Player _player;
    private GameValues _gameValues;
    
    //parent for the coins
    public Transform coinUnused;
    
    //values for current and goal position
    private float _xPos;
    private float _coinPos;
    private float _yPos;
    
    //bool to activate the movement of the coin
    public bool activate;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        _gameValues = _spawnManager.gameValues;
        
    }

    // Update is called once per frame
    void Update()
    {

        //once activated the position is set
        //The position depends on the coinPos from the spawnManager
        //and if the coin is on top of a flat stone
        if (this.gameObject.activeSelf && !activate)
        {
            activate = true;
            switch (_spawnManager.coinPos)
            {
                case "Left":
                {
                    _coinPos = -0.27f;
                    _xPos = -0.55f;
                    if (_spawnManager.lanes[0] == 3)
                    {
                        _yPos = 0.2f;
                    }
                    else
                    {
                        _yPos = 0;
                    }

                    break;
                }
                case "Right":
                {
                    _coinPos = 0.27f;
                    _xPos = 0.55f;
                    if (_spawnManager.lanes[2] == 3)
                    {
                        _yPos = 0.2f;
                    }
                    else
                    {
                        _yPos = 0;
                    }

                    break;
                }
                default:
                {
                    _coinPos = 0;
                    _xPos = 0f;
                    if (_spawnManager.lanes[1] == 3)
                    {
                        _yPos = 0.2f;
                    }
                    else
                    {
                        _yPos = 0;
                    }

                    break;
                }
            }
            this.transform.position = new Vector3(_xPos, 0.7f + _yPos, 0);
            
        }
        //once active it will start moving
        else if(activate)
        {
            this.transform.position =
                        Vector3.MoveTowards(this.transform.position, new Vector3(_coinPos, 0.2f + _yPos, -9f), _gameValues.speed * Time.deltaTime);
        }
        
        //deactivated and sorted to corresponding parent once it reaches the end of the "forest path"
        if (this.transform.position.z <= -8.9f)
        {
            this.transform.SetParent(coinUnused);
            this.transform.position = new Vector3(0, 0.7f, 0);
            activate = false;
            this.gameObject.SetActive(false);
        }

        
    }

    //Once it collides with the player, the coin will be deactivated and sorted to corresponding parent
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            this.transform.SetParent(coinUnused);
            this.transform.position = new Vector3(0, 0.7f, 0);
            this.gameObject.SetActive(false);
            activate = false;
            _gameValues.IncreaseCoin();
        }
    }


}