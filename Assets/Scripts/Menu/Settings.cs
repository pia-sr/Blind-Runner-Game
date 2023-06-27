/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using TextSpeech;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

public class Settings : MonoBehaviour
{
    //Game objects
    public GameObject menu;
    public GameObject ttsBoxes;

    //Slides for each option
    public Slider accessSlider;
    public Slider ttsSlider;
    public Slider hapticsSlider;
    public Slider naviBeltSlider;
    public Slider soundSlider;
    public Slider bSSlider;
    
    //Text to speech
    private TextToSpeech _tts;
    private bool _wait;    
    private int _index;
    private string _standardText = "\n wische für weitere Optionen.";
    
    //Values for touch input
    private Vector2 _startPos;
    private Vector2 _directionTouch;
    private float _startTouchTime;
    
    //Game data to save and load
    public Game gameData;

    
    // Start is called before the first frame update
    void Start()
    {
        _wait = true;
        _tts = TextToSpeech.Instance;
        
        //if all the accessibility options are inactive the general accessibility slider will inactive too
        if (!gameData.hapticsActive && !gameData.ttsActive && !gameData.beltActive)
        {
            accessSlider.value = 0;
        }
        
        //the activity of the slider depends on the saved settings
        hapticsSlider.value = gameData.hapticsActive ? 1 : 0;
        ttsSlider.value = gameData.ttsActive ? 1 : 0;
        naviBeltSlider.value = gameData.beltActive ? 1 : 0;
        soundSlider.value = gameData.soundActive ? 1 : 0;
        bSSlider.value = gameData.blackScreen ? 1 : 0;
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
        if (Input.touchCount > 0 && _index > -1 && gameData.ttsActive && !_wait)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
                
                //Select a button with double tap
                if (_startTouchTime != 0 && (Time.unscaledTime - _startTouchTime < 0.16f))
                {
                    
                    switch (_index)
                    {
                        case 0:
                            Back2Menu();
                            break;
                        case 2:
                            ChangeValue(accessSlider);
                            break;
                        case 3:
                            ChangeValue(ttsSlider);
                            break;
                        case 4:
                            ChangeValue(hapticsSlider);
                            break;
                        case 5:
                            ChangeValue(naviBeltSlider);
                            break;
                        case 6:
                            ChangeValue(soundSlider);
                            break;
                        case 7:
                            ChangeValue(bSSlider);
                            break;

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
                    ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
                    _index++;
                    if (_index > 7)
                    {
                        _index = 0;
                    }

                }
                
                //swiping to the left
                else if (_directionTouch.x < -150)
                {
                    _tts.StopSpeak();
                    ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
                    _index--;
                    if (_index < 0)
                    {
                        _index = 7;
                    }
                }
                else
                {
                    _startTouchTime = Time.unscaledTime;
                }

                //If a swipe happened a new box is active and a new text is read out
                if (startIndex != _index)
                {
                    ttsBoxes.transform.GetChild(_index).gameObject.SetActive(true);
                    string text1;
                    string text2;
                    switch (_index)
                    {
                        case 0:
                            _tts.StartSpeak("Zurück zum Hauptmenü. \nTippe zweimal kurz auf den Bildschirm zum Auswählen oder " + _standardText);
                            break;
                        case 1:
                            _tts.StartSpeak("Einstellungen. " +_standardText);
                            break;
                        case 2:
                            if (accessSlider.value > 0)
                            {
                                text1 = " aktiv";
                                text2 = " aus ";
                            }
                            else
                            {
                                text1 = " nicht aktiv ";
                                text2 = " an ";
                            }
                            _tts.StartSpeak("Bedienungshilfen.\n Die Bedienungshilfen sind zurzeit" + text1 + "\n Tippe zweimal kurz hintereinander auf den Bilschirm um alle Bedienungshilfen" + text2 + "zuschalten oder " + _standardText);
                            break;
                        case 3:
                            if (ttsSlider.value > 0)
                            {
                                text1 = " aktiv";
                                text2 = " aus ";
                            }
                            else
                            {
                                text1 = " nicht aktiv ";
                                text2 = " an ";
                            }
                            _tts.StartSpeak("Sprachwiedergabe.\n Die Sprachwiedergabe ist zurzeit" + text1 + "\n Tippe zweimal kurz hintereinander auf den Bilschirm um die Sprachwiedergabe" + text2 + "zuschalten oder " + _standardText);
                            break;
                        case 4:
                            if (hapticsSlider.value > 0)
                            {
                                text1 = " aktiv";
                                text2 = " aus ";
                            }
                            else
                            {
                                text1 = " nicht aktiv ";
                                text2 = " an ";
                            }
                            _tts.StartSpeak("Handy-Vibrationen.\n Die Handy-Vibrationen sind zurzeit" + text1 + "\n Tippe zweimal kurz hintereinander auf den Bilschirm um die Handy-Vibrationen" + text2 + "zuschalten oder " + _standardText);
                            break;
                        case 5:
                            if (naviBeltSlider.value > 0)
                            {
                                text1 = " aktiv";
                                text2 = " aus ";
                            }
                            else
                            {
                                text1 = " nicht aktiv ";
                                text2 = " an ";
                            }
                            _tts.StartSpeak("naviGürtel.\n Die Nutzung des naviGürtel ist zurzeit" + text1 + "\n Tippe zweimal kurz hintereinander auf den Bilschirm um die Nutzung des naviGürtels" + text2 + "zuschalten oder " + _standardText);
                            break;
                        case 6:
                            if (soundSlider.value > 0)
                            {
                                text1 = " aktiv";
                                text2 = " aus ";
                            }
                            else
                            {
                                text1 = " nicht aktiv ";
                                text2 = " an ";
                            }
                            _tts.StartSpeak("Ton.\n Der Ton ist zurzeit" + text1 + "\n Tippe zweimal kurz hintereinander auf den Bilschirm um den Ton" + text2 + "zuschalten oder " + _standardText);
                            break;
                        case 7:
                            if (bSSlider.value > 0)
                            {
                                text1 = " aktiv";
                                text2 = " aus ";
                            }
                            else
                            {
                                text1 = " nicht aktiv ";
                                text2 = " an ";
                            }
                            _tts.StartSpeak("Schwarzer Bildschirm.\n Der scharze Bildschirm ist zurzeit" + text1 + "\n Tippe zweimal kurz hintereinander auf den Bilschirm um den schwarzen Bildschirm" + text2 + "zuschalten oder " + _standardText);
                            break;

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
        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
        _index = 1;
        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(true);
        _tts.StartSpeak("Hauptmenü");
        gameData.SaveGame();
        this.gameObject.SetActive(false);
        menu.SetActive(true);
    }

    //Function for the general accessibility slider
    //When turn on all accessibility features are turned on too
    //When turn off all accessibility features are turned off too
    public void AccessiblitySlider()
    {
        if (accessSlider.value < 1)
        {
            TTSSlider();
            HapticsSlider();
            BeltSlider();
        }
        else
        {
            _tts.StartSpeak("Alle Bedienungshilfen wurden aktiviert.");
            ttsSlider.value = 1;
            hapticsSlider.value = 1;
            naviBeltSlider.value = 1;
        }

    }

    //Slider for TTS
    public void TTSSlider()
    {
        if (accessSlider.value > 0)
        {
            gameData.ttsActive = ttsSlider.value > 0;
            if (gameData.ttsActive)
            {
                ttsBoxes.SetActive(true);
                _tts.StartSpeak("Die Sprachwiedergabe wurde aktiviert.");
            }
        }
        else
        {
            ttsSlider.value = 0;
            gameData.ttsActive = false;
        }
    }
    
    //Slider for phone haptics
    public void HapticsSlider()
    {
        if (accessSlider.value > 0)
        {
            gameData.hapticsActive = hapticsSlider.value > 0;
            _tts.StartSpeak(gameData.hapticsActive
                ? "Die Handy-Vibration wurden aktiviert."
                : "Die Handy-Vibration wurden deaktiviert");
        }
        else
        {
            hapticsSlider.value = 0;
            gameData.hapticsActive = false;
        }
    }
    
    //Slider for naviBelt
    public void BeltSlider()
    {
        if (accessSlider.value > 0)
        {
            gameData.beltActive = naviBeltSlider.value > 0;
            _tts.StartSpeak(gameData.beltActive
                ? "Die Nutzung des naviGürtels wurden aktiviert."
                : "Die Nutzung des naviGürtels wurden deaktiviert");
        }
        else
        {
            naviBeltSlider.value = 0;
            gameData.beltActive = false;
        }
    }

    //Slider for sound
    public void SoundSlider()
    {
        gameData.soundActive = soundSlider.value > 0;
        _tts.StartSpeak(gameData.soundActive
            ? "Der Ton wurden aktiviert."
            : "Der Ton wurden deaktiviert");
    }
    
    //Slider for to activate a black screen
    public void BlackScreenSlider()
    {
        gameData.blackScreen = bSSlider.value > 0;
        _tts.StartSpeak(gameData.blackScreen
            ? "Der schwarze Bildschirm wurden aktiviert."
            : "Der schwarze Bildschirm wurden deaktiviert");
    }

    //Function to change the value of a given slider
    private void ChangeValue(Slider slider)
    {
        _tts.StopSpeak();
        slider.value = slider.value < 0.5f ? 1 : 0;
    }
    
    //Wait for two frames
    IEnumerator Wait()
    {
        yield return new WaitForEndOfFrame();
        yield return new WaitForEndOfFrame();
        _wait = false;
    }
}