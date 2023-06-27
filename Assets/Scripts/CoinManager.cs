
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    //Scripts
    private Player _player;
    private Game _gameData;
    
    //the melody for the lights
    private AudioSource _sound;
    
    //boolean to determine binaural sound
    private bool _accessibilityActive;

    
    // Start is called before the first frame update
    void Start()
    {
        _gameData = GameObject.FindObjectOfType<Game>();
        _player = GameObject.FindWithTag("Player").GetComponent<Player>();
        _sound = this.GetComponent<AudioSource>();

        //if all the other accessibility features are inactive, the binaural sound will be inactive too
        if (!_gameData.ttsActive && !_gameData.beltActive && !_gameData.hapticsActive)
        {
            _accessibilityActive = false;
        }
        else
        {
            _accessibilityActive = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        //the music is paused when the game is paused
        if (Time.timeScale == 0)
        {
            _sound.Pause();
        }
        else if (this.transform.childCount > 0 && _accessibilityActive)
        {
            //when player is on a flat stone the pitch will change
            if (_player.highUp)
            {
                _sound.pitch = 1.2f;
            }
            else
            {
                _sound.pitch = 1;
            }
            
            //The distance to the player is calculated and the volume of the melody is set based on the result
            int childIndex = this.transform.childCount > 1 ? 1 : 0;
            float distance = Math.Abs(Vector3.Distance(this.transform.GetChild(0).transform.position,
                _player.transform.position));
            _sound.volume = (1.15f - (distance/8));
            if (!_sound.isPlaying )
            {
                _sound.Play();

            }
            
            //binaural sound depending on the position of the coin and the armadillo
            if (_player.transform.position.x > this.transform.GetChild(childIndex).transform.position.x && _player.transform.position.x > -0.3f)
            {
                _sound.panStereo = -1f; //right side muted
                
            }
            else if (_player.transform.position.x < this.transform.GetChild(childIndex).transform.position.x && _player.transform.position.x < 0.3f)
            {
                _sound.panStereo = 1f; //left side muted
            }
            else
            {
                _sound.panStereo = 0; //sound from both sides
            }
        }
        else if (!_accessibilityActive)
        {
            if (!_sound.isPlaying )
            {
                _sound.Play();

            }
            _sound.volume = 0.75f; //fixed volume when binaural sound inactive
        }
        else
        {
            //when no coins are currently in the game the sound is still active at a low volume
            _sound.volume = 0.1f; 
        }
    }

}