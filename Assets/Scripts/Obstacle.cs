
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class Obstacle : MonoBehaviour
{

    //Scripts
    private SpawnManager _spawnManager;
    private GameValues _gameValues;
    private Player _player;
    
    //naviBelt
    private BeltController _beltController;
    private bool _beltActive;
    
    //Parents for rocks
    private Transform _rocksUnused;
    private Transform _rocksSmallUnused;
    
    //Pos for rocks and player
    private float _xPos;
    private float _blockPos;
    private string _obstaclePos;
    private int _playerPos;
    private int _laneIndex;
    private bool _movedPast;
    
    //haptics
    private bool _pulseActive;
    private int _amplitude;
    private float _timeSec;
    private GameObject _rock;
    
    //values to change the speed of the game
    private bool _speedCheck;
    private float _startTime;


    //Once awake everything is set and connected
    private void Awake()
    {
        _beltController = GameObject.FindObjectOfType<BeltController>();
        _spawnManager = this.transform.parent.GetComponent<SpawnManager>();
        _rocksUnused = GameObject.Find("RocksUnused").transform;
        _rocksSmallUnused = GameObject.Find("RocksSmallUnused").transform;
        _gameValues = _spawnManager.gameValues;
        _obstaclePos = _spawnManager.obstaclePos;
        _speedCheck = false;
        _startTime = 0;
        _movedPast = false;
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _playerPos = -1;
        _rock = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        //The position of the rock chain dependent on the obstaclePos from spawnManager
        if (_obstaclePos == "Left")
        {
            _blockPos = -0.3f;
            _xPos = -0.55f;
            _laneIndex = 0;
        }
        else if (_obstaclePos  == "Right")
        {
            _blockPos = 0.3f;
            _xPos = 0.55f;
            _laneIndex = 2;
        }
        else 
        {
            _blockPos = 0;
            _xPos = 0f;
            _laneIndex = 1;
        }

        this.gameObject.tag = _laneIndex.ToString();
        _spawnManager.lanes[_laneIndex] = 1;
        
        //The length of the rock chain is random
        int rand = Random.Range(2, 30);
        for (int i = 0; i <= rand; i++)
        {
            //flat stones
            if (_gameValues.upperLevel)
            {
                _spawnManager.lanes[_laneIndex] = 3;
                Transform block;
                
                //End rock stand move the player to the ground
                if (i == rand)
                {
                    block = _rocksSmallUnused.GetChild(0).GetChild(0);
                    block.SetParent(this.transform);
                    block.rotation = Quaternion.Euler(10.735f, -90, 0);
                    block.position = new Vector3(_xPos, 0.65f, 0 + 0.15f);
                    block.gameObject.SetActive(true);
                }
                //normal flat rocks
                else
                {
                    float randRot = Random.Range(-65f, -115);
                    block = _rocksSmallUnused.GetChild(1);
                    block.SetParent(this.transform);
                    block.position = new Vector3(_xPos, 0.65f, 0);
                    block.gameObject.SetActive(true);
                    block.transform.rotation = Quaternion.Euler(-3.836f, randRot,0);
                }
                block.GetComponent<Stone>().aimPos = _blockPos;
            }
            //Normal rocks
            else
            {
                _spawnManager.smallBlocks = false;
                int rand1 = Random.Range(0, _rocksUnused.childCount);
                Transform block = _rocksUnused.GetChild(rand1);
                block.SetParent(this.transform);
                block.position = new Vector3(_xPos, 0.7f, 0);
                block.rotation = Quaternion.Euler(-3.826f, 0, 0);
                block.gameObject.SetActive(true);
                block.GetComponent<Stone>().aimPos = _blockPos;
            }
            
            //Time is set to 0 to check how fast a rock moves
            if (_startTime == 0)
            {
                _startTime = Time.time;

            }
        }

        this.transform.GetChild(0).tag = "FirstBlock";

    }

    // Update is called once per frame
    private void Update()
    {
        //if the game object has no children, it will be destroyed 
        if (this.transform.childCount == 0)
        {
            Destroy(this.gameObject);
        }
        else
        {
            float distance = this.transform.GetChild(0).position.z - _player.gameObject.transform.position.z;
            
            //the distance to the player from the first child of this game object is send to the BeltConrtoller
            if (this.transform.childCount < 2)
            {
                _beltController.ObstacleGone(_laneIndex);
            }
            else
            {
                _beltController.Distance2Player(distance, _laneIndex);
            }
            
            //Section for the phone haptics
            //if the name of the rock is Small_Block then the children are all flat stones
            if (this.CompareTag(_player.playerXPos.ToString()) &&
                this.transform.GetChild(0).name.Contains("Small_Block") &&
                this.transform.GetChild(0).CompareTag("FirstBlock"))
            {
                if (_rock == null)
                {
                    _rock = transform.GetChild(0).gameObject;
                }

                //if the player moves the phone vibrations are stopped
                if (_playerPos != -1 && _player.playerXPos != _playerPos)
                {
                    _pulseActive = false;
                    StopCoroutine(Pulse());
                    Haptics.Instance.StopHaptics();
                    _playerPos = -1;
                    _rock = null;
                }
                //depending on the distance to the player the pattern and intensity of the phone vibration is set
                else if (_rock != null && _rock.name.Contains("Small_Block"))
                {
                    if (!_pulseActive)
                    {
                        _timeSec = 0.5f;
                        _amplitude = 50;
                        _pulseActive = true;
                        StartCoroutine(Pulse());
                    }

                    _playerPos = _player.playerXPos;
                    if (distance < 0.1f)
                    {
                        _pulseActive = false;
                        StopCoroutine(Pulse());
                        Haptics.Instance.StopHaptics();
                        _playerPos = -1;
                        _rock = null;
                    }
                    else if (distance < 1.3f)
                    {
                        _timeSec = 1f;
                        _amplitude = 255;
                    }
                    else if (distance < 4)
                    {
                        _timeSec = 0.1f;
                        _amplitude = 110;
                    }
                    else if (distance < 6)
                    {
                        _timeSec = 0.2f;
                        _amplitude = 80;
                    }
                    else if (distance < 8)
                    {
                        _timeSec = 0.3f;
                        _amplitude = 50;
                    }
                }
            }
            
            //to adjust the speed change
            if (this.transform.GetChild(0).transform.position.z < -0.3f && !_speedCheck)
            {
                _speedCheck = true;
                _spawnManager.waitFor = (Time.time - _startTime) * 2;
            }  
            //the rocks moved past a specific point and new rocks an be spawned in the same lane
            else if (this.transform.GetChild(this.transform.childCount - 1).transform.position.z <= _gameValues.transitionValue  && _spawnManager.lanes[_laneIndex] == 2 && !_movedPast)
            {
                _movedPast = true;
                _spawnManager.lanes[_laneIndex] = 0;
            }
            //The rock chain started to move towards the player
            else if (this.transform.GetChild(this.transform.childCount - 1).transform.position.z <= -0.3f && (_spawnManager.lanes[_laneIndex] == 1 || _spawnManager.lanes[_laneIndex] == 3) && _spawnManager)
            {
                _spawnManager.lanes[_laneIndex] = 2;
            }
        }
        
        
        
        
        
        
    }
    IEnumerator Pulse()
    {
        while (_pulseActive)
        {
            Haptics.Instance.StartHaptics((int)(_timeSec * 1000), _amplitude);
            yield return new WaitForSeconds(_timeSec*2);
        }
        
    }
    

}
