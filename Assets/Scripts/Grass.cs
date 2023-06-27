
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass : MonoBehaviour
{
    //game setting values
    private GameValues _gameValues;
    
    //Goal position
    private float _xAim;
    
    //parent for grass
    public Transform grassUnused;

    //bool to activate the movement of the coin
    public bool activate;

    //bool to signal the movement of the grass
    private bool _moving;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _gameValues = GameObject.FindObjectOfType<GameValues>();
        
    }

    // Update is called once per frame
    void Update()
    {
        //once activated the aim position is set
        if (activate)
        {
            if (this.transform.position.x < 0)
            {
                _xAim = this.transform.position.x + 0.25f;
            }
            else
            {
                _xAim = this.transform.position.x - 0.25f;
            }

            activate = false;
            _moving = true;
        }
        //once active it will start moving
        else if (_moving)
        {
            
            this.transform.position =
                Vector3.MoveTowards(this.transform.position, new Vector3(_xAim, 0.05f, -9f), _gameValues.speed * Time.deltaTime);
        }
        
        //deactivated and sorted to corresponding parent once it reaches the end of the "forest path"
        if (this.transform.position.z <= -8.5f)
        {
            this.transform.position = new Vector3(0, 0.6f, 0);
            this.gameObject.SetActive(false);
            _moving = false;
            this.transform.parent = grassUnused;
        }

    }
}