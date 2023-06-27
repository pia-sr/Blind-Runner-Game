/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TextSpeech;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Menu : MonoBehaviour
{
    //Game objects
    public GameObject highscore;
    public GameObject settings;
    public GameObject menu;
    public GameObject tutorial;
    public GameObject ttsBoxes;
    public GameObject beltMessage;
    public GameObject beltConnectPanel;
    public GameObject gameBeginPanel;

    //Text to speech
    private TextToSpeech _tts;
    private int _index;
    private bool _wait;
    private string _standardText = " wische für weitere Optionen.";
    
    //naviBelt
    private naviBelt _naviBelt;
    public TextMeshProUGUI beltConnection;
    public TextMeshProUGUI naviBeltButtonText;
    
    //Parameters for touch input
    private Vector2 _startPos;
    private Vector2 _directionTouch;
    private float _startTouchTime;
    
    //Game data to save and load
    public Game gameData;
    
    private void Awake()
    {
        //If a file already exists, the file is loaded
        string path = Application.persistentDataPath + "/gameData.game";
        if (File.Exists(path))
        {
            gameData.loadGame();
        }
        GameObject[] navibelts = GameObject.FindGameObjectsWithTag("naviBelt");
        if (navibelts.Length > 1)
        {
            for (int i = 1; i < navibelts.Length; i++)
            {
                Destroy(navibelts[i]);
            }
        }
        _naviBelt = GameObject.FindObjectOfType<naviBelt>();
        _tts = TextToSpeech.Instance;
        _tts.Settings();
        _tts.StartSpeak("Hauptmenü");
        _wait = true;
        if (gameData.personalisedSettings) return;
        gameData.ttsActive = true;
        gameData.hapticsActive = true;
        gameData.beltActive = true;
        gameData.soundActive = true;
        gameData.blackScreen = false;
        gameData.personalisedSettings = true;
        gameData.SaveGame();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _index = 0;
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
        //Depending on the activity of the panels for the belt
        //an active TTS box is chosen
        GameObject boxes;
        if (beltMessage.activeSelf)
        {
            boxes = beltMessage.transform.GetChild(0).gameObject;
            beltConnectPanel.transform.GetChild(0).gameObject.SetActive(false);
            ttsBoxes.SetActive(false);
        }
        else if(beltConnectPanel.activeSelf)
        {
            boxes = beltConnectPanel.transform.GetChild(0).gameObject;
            beltMessage.transform.GetChild(0).gameObject.SetActive(false);
            ttsBoxes.SetActive(false);
        }
        else
        {
            boxes = ttsBoxes;
            beltMessage.transform.GetChild(0).gameObject.SetActive(false);
            beltConnectPanel.transform.GetChild(0).gameObject.SetActive(false);
        }

        if (gameData.ttsActive)
        {
            boxes.SetActive(true);
        }
        
        //Touch input when TTS is active
        if (Input.touchCount > 0 && _index > -1 && gameData.ttsActive && !_wait)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
                
                //Select a button with double tap
                if (_startTouchTime != 0 && (Time.unscaledTime - _startTouchTime < 0.16f) && gameData.ttsActive)
                {
                    if (beltMessage.activeSelf)
                    {
                        switch (_index)
                        {
                            case 1:
                                ButtonYes();
                                break;
                            case 2:
                                ButtonNo();
                                break;
                        }
                    }
                    else if (beltConnectPanel.activeSelf)
                    {
                        switch (_index)
                        {
                            case 0:
                                ConnectButton();
                                break;
                            case 1:
                                ButtonClose();
                                break;
                        }
                    }
                    else
                    {
                        switch (_index)
                        {
                            case 1:
                                StartGame();
                                break;
                            case 2:
                                OpenSettings();
                                break;
                            case 3:
                                OpenHighscore();
                                break;
                            case 4:
                                OpenTutorial();
                                break;
                            case 5:
                                OpenBelt();
                                break;

                        }
                    }
                    


                }
            }
            
            //detection of a finger swipe
            else if (touch.phase == TouchPhase.Ended)
            {
                _directionTouch = touch.position - _startPos;
                int startIndex = _index;
                
                //swiping to the right
                if (_directionTouch.x > 150)
                {
                    _tts.StopSpeak();
                    boxes.transform.GetChild(_index).gameObject.SetActive(false);
                    _index++;
                    if (_index > boxes.transform.childCount - 1)
                    {
                        _index = 0;
                    }
                }
                //swiping to the left
                else if (_directionTouch.x < -150)
                {
                    _tts.StopSpeak();
                    boxes.transform.GetChild(_index).gameObject.SetActive(false);
                    _index--;
                    if (_index < 0)
                    {
                        _index = boxes.transform.childCount - 1;
                    }
                }
                else
                {
                    _startTouchTime = Time.unscaledTime;
                }

                //If a swipe happened a new box is active and a new text is read out
                if (startIndex != _index)
                {
                    boxes.transform.GetChild(_index).gameObject.SetActive(true);
                    if (beltMessage.activeSelf)
                    {
                        switch (_index)
                        {
                            case 0:
                                _tts.StartSpeak("Es ist kein Gürtel verbunden. Möchtest du ohne Gürtel fortfahren? " +
                                                _standardText);
                                break;
                            case 1:
                                _tts.StartSpeak(
                                    "Ja. \n Tippe zweimal kurz hintereinander auf den Bildschirm um das Spiel ohne Gürtel zu starten oder " +
                                    _standardText);
                                break;
                            case 2:
                                _tts.StartSpeak(
                                    "Nein. \n Tippe zweimal kurz hintereinander auf den Bildschirm um einen Gürtel zu verbinden oder " +
                                    _standardText);
                                break;
                        }
                    }
                    else if (beltConnectPanel.activeSelf)
                    {
                        switch (_index)
                        {
                            case 0:
                                if (_naviBelt.ConnectStatus() == "Connected")
                                {
                                    _tts.StartSpeak(
                                        "Gürtel trennen. \n Tippe  zweimal kurz hintereinander auf den Bildschirm um den Gürtel zu trennen oder " +
                                        _standardText);
                                }
                                else
                                {
                                    _tts.StartSpeak(
                                        "Gürtel verbinden \n Tippe zweimal kurz hintereinander auf den Bildschirm um den Gürtel zu verbinden oder " +
                                        _standardText);
                                }

                                break;
                            case 1:
                                _tts.StartSpeak(
                                    "Schließen. Tippe zweimal kurz hintereinander auf den Bildschirm um zurück zum Hauptmenü zu gelangen oder " +
                                    _standardText);
                                break;
                        }
                    }
                    else
                    {
                        switch (_index)
                        {
                            case 0:
                                _tts.StartSpeak("Hauptmenü. " + _standardText);
                                break;
                            case 1:
                                _tts.StartSpeak(
                                    "Start. \n Tippe zweimal kurz hintereinander auf den Bildschirm um das Spiel zu starten oder " +
                                    _standardText);
                                break;
                            case 2:
                                _tts.StartSpeak(
                                    "Einstellungen. \n Tippe zweimal kurz hintereinander auf den Bildschirm um die Einstellungen zu öffnen oder " +
                                    _standardText);
                                break;
                            case 3:
                                _tts.StartSpeak(
                                    "Highscore Liste. \n Tippe zweimal kurz hintereinander auf den Bildschirm um die Highscore Liste zu öffnen oder " +
                                    _standardText);
                                break;
                            case 4:
                                _tts.StartSpeak(
                                    "Anleitung. \n Tippe zweimal kurz hintereinander auf den Bildschirm um die Anleitung zu öffnen oder " +
                                    _standardText);
                                break;
                            case 5:
                                _tts.StartSpeak(
                                    "naviGürtel. \n Tippe zweimal kurz hintereinander auf den Bildschirm um die Verbindung zum naviGürtel zu ändern oder " +
                                    _standardText);
                                break;
                        }
                    }
                }
                
            }
        }
        
        //the text for the belt connection status is updated based on the current status
        beltConnection.text = _naviBelt.ConnectStatus();
        if (_naviBelt.ConnectStatus() == "Disconnected")
        {
            naviBeltButtonText.text = "Gürtel verbinden";
        }
        else if (_naviBelt.ConnectStatus() == "Connected")
        {
            naviBeltButtonText.text = "Gürtel trennen";
        }

        if (!gameData.ttsActive)
        {
            boxes.SetActive(false);
        }
    }

    //Function to start the game
    //if not belt is connected and naviBelt active then a panel appears
    public void StartGame()
    {
        if (!gameData.beltActive || _naviBelt.ConnectStatus() == "Connected")
        {
            SceneManager.LoadScene("RunnerGame");
            _tts.StartSpeak("Spiel lädt");
            ttsBoxes.SetActive(false);
            for (int i = 1; i < 7; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }

            gameBeginPanel.SetActive(true);
            TTSSetBack();
        }
        else
        {
            beltMessage.SetActive(true);
            for (int i = 1; i < 7; i++)
            {
                this.transform.GetChild(i).gameObject.SetActive(false);
            }
            _tts.StartSpeak("Es ist kein Gürtel verbunden. Möchtest du ohne Gürtel fortfahren? " + _standardText);
            TTSSetBack();
        }
        
    }

    //opens the highscore
    public void OpenHighscore()
    {
        menu.SetActive(false);
        highscore.SetActive(true);
        _tts.StartSpeak("Highscore Liste");
        TTSSetBack();
    }

    //opens the settings
    public void OpenSettings()
    {
        menu.SetActive(false);
        settings.SetActive(true);
        _tts.StartSpeak("Einstellungen");
        TTSSetBack();
    }

    //opens the tutorial
    public void OpenTutorial()
    {
        menu.SetActive(false);
        tutorial.SetActive(true);
        _tts.StartSpeak("Anleitung");
        TTSSetBack();
    }
    
    //opens the belt pannel
    public void OpenBelt()
    {
        beltConnectPanel.SetActive(true);
        for (int i = 1; i < 7; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(false);
        }
        _tts.StartSpeak("naviGürtel verbinden");
        TTSSetBack();
    }

    //Function to set the TTS boxes back to default
    private void TTSSetBack()
    {
        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
        _index = 0;
        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(true);
    }

    //Function to start the connection to the belt
    public void ConnectButton()
    {
        if (_naviBelt.ConnectStatus() == "Disconnected")
        {
            _naviBelt.ConnectBelt();
        }
        else if (_naviBelt.ConnectStatus() == "Connected")
        {
            _naviBelt.DisconnectBelt();
        }
        
    }

    //Yes button to start the game without the belt
    public void ButtonYes()
    {
        SceneManager.LoadScene("RunnerGame");
        _tts.StartSpeak("Spiel beginnt");
        beltMessage.SetActive(false);
        gameBeginPanel.SetActive(true);
        TTSSetBack();
    }

    //No button to connect a belt before game begin
    public void ButtonNo()
    {
        _index = 0;
        beltMessage.SetActive(false);
        beltConnectPanel.SetActive(true);
    }

    //Function to close the belt panel
    public void ButtonClose()
    {
        beltConnectPanel.SetActive(false);
        _index = 0;
        for (int i = 1; i < 7; i++)
        {
            this.transform.GetChild(i).gameObject.SetActive(true);
        }
        _tts.StartSpeak("Hauptmenü");
    }

    //Stops the TTS function and the connection to the belt when game stops
    void OnApplicationQuit()
    {
        if (_naviBelt.ConnectStatus() == "Connected")
        {
            _naviBelt.DisconnectBelt();
        }
        _tts.StopSpeak();
    }

    //Stops TTS when game is paused
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _tts.StopSpeak();
        }
    }
    
    //waits for two frames
    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _wait = false;
    }
}