
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : MonoBehaviour
{
    //game setting values
    private GameValues _gameValues;
    private SpawnManager _spawnManager;
    
    //Goal position
    private float _xAim;
    private float _yAim;
    
    //Bool for pos of the tree
    public bool bySide;
    public bool left;

    //parent for tree
    public Transform treesUnused;

    //bool to activate the movement of the coin
    public bool activate;

    //bool to signal the movement of the grass
    private bool _moving;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _spawnManager = GameObject.FindObjectOfType<SpawnManager>();
        _gameValues = _spawnManager.gameValues;
        
    }

    // Update is called once per frame
    void Update()
    {
        //once activated the aim position is set
        if (activate)
        {
            activate = false;
            _moving = true;
            if (left)
            {
                _xAim = -0.85f;
                if (!bySide) return;
                _xAim = -1.3f;
            }
            else
            {
                _xAim = 0.85f;
                if (!bySide) return;
                _xAim = 1.3f;
            }
            
        }
        //once active it will start moving
        else if (_moving)
        {
            this.transform.position =
                Vector3.MoveTowards(this.transform.position, new Vector3(_xAim, 0, -9f), _gameValues.speed * Time.deltaTime);
        }
        
        //deactivated and sorted to corresponding parent once it reaches the end of the "forest path"
        if (this.transform.position.z <= -8.9f)
        {
            _moving = false;
            
            this.transform.position = new Vector3(0, 0.55f, 0);
            this.gameObject.SetActive(false);
            this.transform.SetParent(treesUnused);
        }
    }
}
