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
using UnityEngine.UI;

public class Highscore : MonoBehaviour
{
    //Game objects 
    public GameObject menu; 
    public GameObject scoreElement;
    public GameObject scores;
    public GameObject ttsBoxes;
    
    //Game data to save and load
    public Game gameData;

    //Values for the touch input
    private Vector2 _startPos;
    private Vector2 _directionTouch;
    private float _startTouchTime;
    
    //Text to speech
    private TextToSpeech _tts;
    private bool _wait; //wait for text to finish
    private bool _up; //moving up in the navigation
    private int _index; //current active TTS box
    private string _standardText = "\n wische für weitere Optionen.";
    
    //Scrollbar
    public Scrollbar scrollbar;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _wait = true;
        _tts = TextToSpeech.Instance;
        HighscoreElements();
        _index = 2;
        
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
                            DeleteHighscore();
                            _tts.StartSpeak("Alle Einträge in der Highscore Tabelle wurden gelöscht.");
                            break;
                        case 1:
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
                    if (_index < 3)
                    {
                        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
                    }
                    else
                    {
                        scores.transform.GetChild(_index-3).GetChild(0).gameObject.SetActive(false);
                    }
                    _index--;
                    _up = true;
                    if (_index < 0)
                    {
                        _index = scores.transform.childCount + 2;
                        scrollbar.value = 0;
                    }
                    
                }
                
                //swiping to the right
                else if(_directionTouch.x > 150)
                {
                    if (_index < 3)
                    {
                        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
                    }
                    else
                    {
                        scores.transform.GetChild(_index-3).GetChild(0).gameObject.SetActive(false);
                    }
                    _index++;
                    _up = false;
                    if (_index > scores.transform.childCount + 2)
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
                    if (_index < 3)
                    {
                        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(true);
                        switch (_index)
                        {
                            case 0:
                                _tts.StartSpeak("Highscore löschen. \n Tippe zweimal kurz auf den Bildschirm um alle Einträge zu löschen oder " + _standardText);
                                break;
                            case 1:
                                _tts.StartSpeak("Zurück zum Hauptmenü. \nTippe zweimal kurz auf den Bildschirm zum Auswählen oder " + _standardText);
                                break;
                            case 2:
                                _tts.StartSpeak("Highscore Tabelle" + _standardText);
                                break;
                        }
                    }
                    
                    //TTS boxes and text of scores in the table
                    else
                    {
                        scores.transform.GetChild(_index-3).GetChild(0).gameObject.SetActive(true);
                        if (gameData.scores.Length == 0)
                        {
                            _tts.StartSpeak("Noch kein Highscore vorhanden " + _standardText);
                        }
                        else
                        {
                            _tts.StartSpeak("Platz " + (_index-2).ToString() + "\n " + gameData.scores[_index-3].ToString() + " Lichter " + _standardText);
                            if (_index > 9)
                            {
                                float value = (float)1 / (float) 15;
                                if (_up && scrollbar.value + value <= 1)
                                {
                                    scrollbar.value += value;
                                }
                                else if (!_up && scrollbar.value - value >= 0)
                                {
                                    scrollbar.value -= value;
                                }
                            }
                            else
                            {
                                scrollbar.value = 1;
                            }
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
        if (_index < 3)
        {
            ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
        }
        else
        {
            scores.transform.GetChild(_index-3).GetChild(0).gameObject.SetActive(false);
        }

        _index = 2;
        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(true);
        _tts.StartSpeak("Hauptmenü");
        this.gameObject.SetActive(false);
        scrollbar.value = 1;
        menu.SetActive(true);
    }

    //Retrieving the saved highscores
    private void HighscoreElements()
    {
        if (gameData.scores.Length == 0)
        {
            if (scores.transform.childCount == 0)
            {
                GameObject startText = Instantiate(scoreElement, Vector3.zero, Quaternion.identity, scores.transform);
                startText.GetComponent<TextMeshProUGUI>().text = "Kein Highscore vorhanden";
            }
            
        }
        else
        {
            if (scores.transform.childCount > 0)
            {
                Destroy(scores.transform.GetChild(0).gameObject);
            }
            foreach (int value in gameData.scores)
            {
                GameObject text = Instantiate(scoreElement, Vector3.zero, Quaternion.identity, scores.transform);
                int index = text.transform.GetSiblingIndex() + 1;
                text.GetComponent<TextMeshProUGUI>().text = index.ToString() + ". " + value.ToString();
            }
        }
    }

    //Delete all saved highscores
    public void DeleteHighscore()
    {
        gameData.scores = Array.Empty<int>();
        for (int i = 0; i < scores.transform.childCount; i++)
        {
            Destroy(scores.transform.GetChild(i).gameObject);
        }
        HighscoreElements();
    }
    
    //wait two frames
    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _wait = false;
    }
    
}