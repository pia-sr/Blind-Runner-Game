
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;


public class GameValues : MonoBehaviour
{
    //Values to change during the gameplay 
    public float speed;
    public float transitionValue;//distance between the previous obstacle and the new obstacle on the same lane
    public int blockProp;//probability og randomly assigning "Nothing" in SpawnManager
    private int _threshold; //coin threshold to reach for value change
    public bool upperLevel; //active when flat stones are active
    private int _blockPropOld; //value to save the current blockProb

    
    private int _coinAmount;
    public TextMeshProUGUI coinScore;
    // Start is called before the first frame update
    void Start()
    {
        //start values
        _coinAmount = 0;
        coinScore.text = "0";
        speed = 1.5f;
        transitionValue = -8;
        blockProp = 94;
        _blockPropOld = blockProp;
        _threshold = 200;
    }

    // Update is called once per frame
    void Update()
    {
        if (_coinAmount == 25)
        {
            speed = 1.7f;
            transitionValue = -7.8f;
            blockProp = 86;
            _blockPropOld = blockProp;
        }
        else if (_coinAmount == 50)
        {
            speed = 1.9f;
            transitionValue = -7.6f;
            blockProp = 78;
            _blockPropOld = blockProp;
            //the coroutine becomes active to activate the flat stones
            StartCoroutine(ActivateJump());
        }
        else if (_coinAmount == 100)
        {
            speed = 2.1f;
            transitionValue = -7.4f;
            blockProp = 70;
            _blockPropOld = blockProp;
        }
        //every 200 coins a new threshold is reached
        else if (_coinAmount == _threshold)
        {
            speed += 0.2f;
            transitionValue += 0.2f;
            blockProp -= 8;
            _threshold += 200;
            _blockPropOld = blockProp;
        }
    }
    
    //function to increase the amount of collected coins
    public void IncreaseCoin()
    {
        _coinAmount++;
        coinScore.text = _coinAmount.ToString();
    }

    //getter for coins
    public int GetCoinAmount()
    {
        return _coinAmount;
    }

    //Coroutine to activate the jumping stones
    //once activate it will continuously loop
    IEnumerator ActivateJump()
    {
        while (Time.timeScale > 0.5f)
        {
            yield return new WaitForSeconds(50);
            upperLevel = true;
            blockProp = 85;
            yield return new WaitForSeconds(25);
            blockProp = _blockPropOld;
            upperLevel = false;
        }
    }
}