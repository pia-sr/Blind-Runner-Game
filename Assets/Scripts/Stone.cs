
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stone : MonoBehaviour
{
    // Start is called before the first frame update
    public float aimPos;
    private UI _ui;
    private SpawnManager _spawnManager;
    private GameValues _gameValues;
    private float _xPos;
    private int _startTime;
    private bool _speedCheck;
    private Player _player;
    private int _blockPos;
    private bool _activate;
    public Transform rocksUnused;
    public Transform rocksSmallUnused;

    void Start()
    {
        _ui = GameObject.FindWithTag("UI").GetComponent<UI>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        _gameValues = _spawnManager.gameValues;

    }

    void Update()
    {
        if (this.gameObject.activeSelf && !_activate)
        {
            _activate = true;
            if (aimPos == 0)
            {
                _blockPos = 1;
            }
            else if (aimPos < -0.25f)
            {
                _blockPos = 0;
            }
            else
            {
                _blockPos = 2;
            }
        }
        else if (_activate)
        {
            if (this.transform.position.z <= -8.9f)
            {
                if (this.gameObject.name.Contains("Small"))
                {
                    if (this.gameObject.name.Contains("End"))
                    {
                        this.transform.SetParent(rocksSmallUnused.GetChild(0));
                    }
                    else
                    {
                        this.transform.SetParent(rocksSmallUnused);
                    }
                }
                else
                {
                    this.transform.SetParent(rocksUnused);
                }
                this.transform.position = new Vector3(0, 0.65f, 0);
                _activate = false;
                this.gameObject.SetActive(false);
            }
            else if (this.transform.GetSiblingIndex() == 0 || this.transform.parent.GetChild(this.transform.GetSiblingIndex()-1).transform.position.z < -0.3f)
            {
                this.transform.position = Vector3.MoveTowards(this.transform.position, new Vector3(aimPos, 0.1f, -9f), _gameValues.speed * Time.deltaTime);
            }
        
            if (this.transform.GetSiblingIndex() == this.transform.parent.childCount-1 && this.transform.position.z <= (_player.transform.position.z +0.05f) && this.name.Contains("End"))
            {
                _player.blockPos[_blockPos] = 2;
            }
            else if (this.transform.GetSiblingIndex() == this.transform.parent.childCount-1 && this.transform.position.z < -8.3f)
            {
                _player.blockPos[_blockPos] = 0;
            }
            else if (this.transform.GetSiblingIndex() == 0 && this.transform.position.z < -7.6f)
            {
                _player.blockPos[_blockPos] = 1;
            }
        }
        
    }
        
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            _ui.Lost();

        }
    }
}
