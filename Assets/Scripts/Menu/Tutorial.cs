
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    //Game objects
    public GameObject menu;
    public GameObject elements;
    public GameObject ttsBoxes;
    
    //Text to speech
    private TextToSpeech _tts;
    private string _standardText = "\n wische für weitere Optionen.";
    private int _index;
    private bool _wait;
    
    //Values for touch input
    private Vector2 _startPos;
    private Vector2 _directionTouch;
    private float _startTouchTime;
    
    //Game data to save and load
    public Game gameData;

    //Scrollbar
    public Scrollbar scrollbar;

    //Melody of the lights
    public AudioSource melody;
    
    // Start is called before the first frame update
    void Start()
    {
        _wait = true;
        _tts = TextToSpeech.Instance;
        _index = 1;
        
        //if TTS is inactive, the boxes need to inactive as well
        if (!gameData.ttsActive)
        {
            ttsBoxes.SetActive(false);
        }
        StartCoroutine(Wait());
    }

    // Update is called once per frame
    void Update()
    {
        //Touch input when TTS is active
        if (Input.touchCount > 0 && gameData.ttsActive && !_wait)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
                
                //Select a button with double tap
                if (_startTouchTime != 0 && (Time.unscaledTime - _startTouchTime < 0.16f))
                {
                    _tts.StopSpeak();
                    switch (_index)
                    {
                        case 0:
                            Back2Menu();
                            _tts.StartSpeak("Hauptmenü");
                            break;
                    }
                }
            }
            //detection of a finger swipe
            else if (touch.phase == TouchPhase.Ended)
            {
                int startIndex = _index;
                _directionTouch = touch.position - _startPos;
                
                //swiping to the left
                if (_directionTouch.x < -150)
                {
                    if (_index < 2)
                    {
                        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
                    }
                    else
                    {
                        elements.transform.GetChild(_index-2).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        elements.transform.GetChild(_index-2).GetChild(1).GetChild(0).gameObject.SetActive(false);
                    }
                    _index--;
                    if (_index < 0)
                    {
                        _index = elements.transform.childCount + 1;
                        scrollbar.value = 0;
                    }
                    
                }
                
                //swiping to the right
                else if(_directionTouch.x > 150)
                {
                    if (_index < 2)
                    {
                        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
                    }
                    else
                    {
                        elements.transform.GetChild(_index-2).GetChild(0).GetChild(0).gameObject.SetActive(false);
                        elements.transform.GetChild(_index-2).GetChild(1).GetChild(0).gameObject.SetActive(false);
                    }
                    _index++;
                    if (_index > elements.transform.childCount + 1)
                    {
                        _index = 0;
                        scrollbar.value = 1;
                    }
                }
                else
                {
                    _startTouchTime = Time.unscaledTime;
                }

                //If a swipe happened a new box is active and a new text is read out
                if (startIndex != _index)
                {
                    _tts.StopSpeak();
                    if (melody.isPlaying)
                    {
                        melody.Stop();
                    }
                    
                    if (_index < 2)
                    {
                        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(true);
                        switch (_index)
                        {
                            case 0:
                                _tts.StartSpeak("Zurück zum Hauptmenü. \nTippe zweimal kurz auf den Bildschirm zum Auswählen oder " + _standardText);
                                break;
                            case 1:
                                _tts.StartSpeak("Anleitung" + _standardText);
                                break;
                        }
                    }
                    else
                    {
                        //Text in between the scroll area
                        if (_index < 6)
                        {
                            
                            elements.transform.GetChild(_index-2).GetChild(0).GetChild(0).gameObject.SetActive(true);
                            elements.transform.GetChild(_index-2).GetChild(1).GetChild(0).gameObject.SetActive(true);
                        }
                        switch (_index)
                        {
                            case 2:
                                scrollbar.value = 1;
                                _tts.StartSpeak("Ziel des Spieles");
                                StartCoroutine(SpeakAgain(elements.transform.GetChild(0).GetChild(1)
                                    .GetComponent<TextMeshProUGUI>()
                                    .text + _standardText));
                                break;
                            case 3:
                                scrollbar.value = 0.815f;
                                _tts.StartSpeak("Die Bedienung");
                                StartCoroutine(SpeakAgain(elements.transform.GetChild(1).GetChild(1)
                                    .GetComponent<TextMeshProUGUI>()
                                    .text + _standardText));
                                break;
                            case 4:
                                scrollbar.value = 0.52f;
                                _tts.StartSpeak("Die Lichter");
                                StartCoroutine(SpeakAgain(elements.transform.GetChild(2).GetChild(1)
                                    .GetComponent<TextMeshProUGUI>()
                                    .text));
                                StartCoroutine(PlayMeldody());
                                break;
                            case 5:
                                scrollbar.value = 0.271f;
                                _tts.StartSpeak("Die Hindernisse");
                                StartCoroutine(SpeakAgain(elements.transform.GetChild(3).GetChild(1)
                                    .GetComponent<TextMeshProUGUI>()
                                    .text + _standardText));
                                break;
                            case 6:
                                elements.transform.GetChild(6).GetChild(0).gameObject.SetActive(true);
                                _tts.StartSpeak("Viel Glück!" + _standardText);
                                scrollbar.value = 0;
                                break;
                        }
                    }
                }
                
                
            }
        }
        if (!gameData.ttsActive)
        {
            ttsBoxes.SetActive(false);
        }
    }
    
    //Method to go back to the menu and to set the boxes back to default
    public void Back2Menu()
    {
        gameData.SaveGame();
        if (_index < 2)
        {
            ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
        }
        else
        {
            elements.transform.GetChild(_index-2).GetChild(0).GetChild(0).gameObject.SetActive(false);
            elements.transform.GetChild(_index-2).GetChild(1).GetChild(0).gameObject.SetActive(false);
        }

        _index = 1;
        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(true);
        _tts.StartSpeak("Hauptmenü");
        this.gameObject.SetActive(false);
        scrollbar.value = 1;
        menu.SetActive(true);
    }

    //Wait for two frames
    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _wait = false;
    }
    
    //Function to queue a given text
    IEnumerator SpeakAgain(string text)
    {
        yield return new WaitUntil(()=> _tts.isSpeaking() == false);
        _tts.StartSpeak("\n" + text);
    }

    //Function to play a sample of the melody
    IEnumerator PlayMeldody()
    {
        yield return new WaitUntil(()=> _tts.isSpeaking() == false);
        _tts.StartSpeak("Die Melodie der Lichter klingt so: \n");
        yield return new WaitUntil(()=> _tts.isSpeaking() == false);
        melody.Play();
        yield return new WaitForSeconds(15);
        melody.Stop();
        _tts.StartSpeak(_standardText);
    }
    
}