
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeltController : MonoBehaviour
{
    //naviBelt
    private naviBelt _naviBelt;

    //Player
    private Player _player;

    //position of the player
    private int _playerPos;

    //List to save the different values for each of the three channels/lanes
    private float[] _distances = new float[] { 0, 0, 0 };
    private int[] _activePulse = new int[] { 0, 0, 0 };
    private int[] _activeObstacle = new int[] { 0, 0, 0 };
    private int[] _periodTime = new int[] { 0, 0, 0 };
    private int[] _duration = new int[] { 0, 0, 0 };

    private void Awake()
    {
        _naviBelt = GameObject.FindWithTag("naviBelt").GetComponent<naviBelt>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _playerPos = _player.playerXPos;
    }

    // Update is called once per frame
    void Update()
    {
        //If the player is on top of a flat stone the pattern changes
        //everywhere there is no active obstacle a pulsating pattern is started
        if (_player.highUp)
        {
            if (_activeObstacle[0] == 1 && _activePulse[0] == 1)
            {
                _naviBelt.StopVibration(0);
            }
            else if(_activeObstacle[0] == 0 && _activePulse[0] == 0)
            {
                _activePulse[0] = 1;
                _naviBelt.StartPulse(60, 500, 1000, 0, 0);
            }
            
            if (_activeObstacle[1] == 1 && _activePulse[1] == 1)
            {
                _naviBelt.StopVibration(1);
            }
            else if(_activeObstacle[1] == 0 && _activePulse[1] == 0)
            {
                _activePulse[1] = 1;
                _naviBelt.StartPulse(60, 500, 1000, 0, 1);
            }
            
            if (_activeObstacle[2] == 1 && _activePulse[2] == 1)
            {
                _naviBelt.StopVibration(2);
            }
            else if(_activeObstacle[2] == 0 && _activePulse[2] == 0)
            {
                _activePulse[2] = 1;
                _naviBelt.StartPulse(60, 500, 1000, 0, 2);
            }
            
            
        }
        
        //The pattern for an obstacle is activated/deactivated for each of the three lanes
        else
        {
            if (_activeObstacle[0] == 1)
            {
                DistanceCheck(0);
            }
            if (_activeObstacle[1] == 1)
            {
                DistanceCheck(1);
            }
            if (_activeObstacle[2] == 1)
            {
                DistanceCheck(2);
            }

            if (_activeObstacle[0] == 0 && _activePulse[0] == 1)
            {
                ObstacleGone(0);
            }
            if (_activeObstacle[1] == 0 && _activePulse[1] == 1)
            {
                ObstacleGone(1);
            }
            if (_activeObstacle[2] == 0 && _activePulse[2] == 1)
            {
                ObstacleGone(2);
            }
            
        }

        //The pattern for the armadillo's pos is activated after it is certain that the belt is connected
        if (_naviBelt.ConnectStatus() == "Connected" && _naviBelt.channelActivity()[3] == 0 && _naviBelt.channelActivity()[4] == 0 && _naviBelt.channelActivity()[5] == 0)
        {
            _naviBelt.StartVibration(25,_playerPos+3);
            _playerPos = 0;
        }
        //if the player switches a lane, the active vibro motor switches too
        else if (_playerPos != _player.playerXPos)
        {
            _naviBelt.StopVibration(_playerPos+3);
            _playerPos = _player.playerXPos;
            _naviBelt.StartVibration(25,_playerPos+3);
            
        }
        
    }

    //public function to set the distance for a given lane
    //only call when there is an obstacle on the given lane
    public void Distance2Player(float distance, int laneIndex)
    {
        _distances[laneIndex] = distance;
        if (_activeObstacle[laneIndex] == 0)
        {
            if (_playerPos == laneIndex)
            {
                _naviBelt.StopVibration(laneIndex);
            }

            _activeObstacle[laneIndex] = 1;
            _activePulse[laneIndex] = 1;
        }
    }

    //public function to communicate that an obstacle at the given lane is gone
    public void ObstacleGone(int channelIndex)
    {
        _activeObstacle[channelIndex] = 0;
        _naviBelt.StopVibration(channelIndex);
        _activePulse[channelIndex] = 0;
    }
    
  
    //function to set the pattern depending on the distance for the given channel/lane
    private void DistanceCheck(int channelIndex)
    {
        float distance = _distances[channelIndex];
        int periodTime = _periodTime[channelIndex];
        int duration = _duration[channelIndex];
        if (distance < 0.55f)
        {
            _periodTime[channelIndex] = 1000;
            _duration[channelIndex] = 500;
        }
        else if (distance < 3f)
        {
            _periodTime[channelIndex] = 400;
            _duration[channelIndex] = 200;
        }
        else if (distance < 5f)
        {
            _periodTime[channelIndex] = 600;
            _duration[channelIndex] = 200;
        }
        else if (distance < 7.5f)
        {
            _periodTime[channelIndex] = 800;
            _duration[channelIndex] = 300;
        }
        else if (distance < 10)
        {
            _periodTime[channelIndex] = 1000;
            _duration[channelIndex] = 300;
        }

        if (periodTime != _periodTime[channelIndex])
        {
            periodTime = _periodTime[channelIndex];
            duration = _duration[channelIndex];
            _naviBelt.StartPulse(60, duration, periodTime, 0, channelIndex);
            
        }
    }

    //Function to pause/stop all the vibrations
    public void Pause()
    {
        _naviBelt.StopAll();
    }

    //function to resume the vibration
    public void Resume()
    {
        
        _naviBelt.StartVibration(25,_playerPos+3);
        _periodTime = new int[] { 0, 0, 0 };
        _activePulse[0] = 0;
        _activePulse[1] = 0;
        _activePulse[2] = 0;
    }

}
