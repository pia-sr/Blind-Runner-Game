
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TextSpeech;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    //Game objects
    public GameObject lostMessage;
    public GameObject pauseMessage;
    public GameValues gameValues;
    public GameObject ttsBoxes;
    public GameObject blackScreen;
    private Button _playAgainButton;
    
    //naviBelt
    private naviBelt _naviBelt;
    public BeltController beltController;
    
    //TTS
    private TextToSpeech _tts;
    private string _standardText = "\nWische für weitere Optionen.";
    private bool _wait;
    private int _index;
    
    //Parameters for touch input
    private Vector2 _startPos;
    private Vector2 _directionTouch;
    private float _startTouchTime;
    
    //Booleans for pausing and losing the game
    private bool _lost;
    private bool _pause;
    
    //Animation of the armadill0
    public Animator animator;
    
    //Game data to save and load
    private Game _gameData;
    
    
    //on awake everything is connected and the save data loaded
    private void Awake()
    { 
        DynamicGI.UpdateEnvironment();
        _gameData = GameObject.FindObjectOfType<Game>();
        //If a file already exists, the file is loaded
        string path = Application.persistentDataPath + "/gameData.game";
        if (File.Exists(path))
        {
            _gameData.loadGame();
        }

        GameObject[] navibelts = GameObject.FindGameObjectsWithTag("naviBelt");
        
        //if there are more naviBelt game objects, all but one are deleted
        if (navibelts.Length > 1)
        {
            for (int i = 1; i < navibelts.Length; i++)
            {
                Destroy(navibelts[i]);
            }
        }

        //if the sound is inactive, all the sounds in the game are muted
        if (!_gameData.soundActive)
        {
            AudioListener.volume = 0;
        }

        _wait = true;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Wait());
        _naviBelt = GameObject.FindObjectOfType<naviBelt>();
        _tts = TextToSpeech.Instance;
        _tts.Settings();
        _startTouchTime = 0;
        _lost = false;
        if (!_gameData.ttsActive)
        {
            ttsBoxes.SetActive(false);
        }

        //a black screen comes on top of the game if black screen is active
        if (_gameData.blackScreen)
        {
            blackScreen.SetActive(true);
        }
        _tts.StartSpeak("Spiel beginnt");
    }

    // Update is called once per frame
    void Update()
    {
        //Touch input when TTS is active
        if (Input.touchCount > 0 && _index > -1 && !_wait)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                _startPos = touch.position;
                
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                _directionTouch = touch.position - _startPos;
                int startIndex = _index;
                
                //detection of a finger swipe
                //swiping to the right
                if (_directionTouch.x > 150 && lostMessage.activeSelf)
                {
                    _tts.StopSpeak();
                    ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
                    _index++;
                    if (_index > 2)
                    {
                        _index = 0;
                    }
                    
                }
                //swiping to the left
                else if (_directionTouch.x < -150 && lostMessage.activeSelf)
                {
                    _tts.StopSpeak();
                    ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
                    _index--;
                    if (_index < 0)
                    {
                        _index = 2;
                    }
                }
                
                //Select a button with double tap or pause the game
                else if (_startTouchTime != 0 && (Time.unscaledTime - _startTouchTime < 0.3f))
                {
                    if (!_lost)
                    {
                        //resumes the game
                        if(_pause)
                        {
                            if (_gameData.blackScreen)
                            {
                                blackScreen.SetActive(true);
                            }
                            _tts.StopSpeak();
                            _tts.StartSpeak("Spiel wird fortgesetzt");
                            _pause = false;
                            Time.timeScale = 1;
                            pauseMessage.SetActive(false);
                            beltController.Resume();
                            animator.SetInteger("animation", 2);
                            
                                         
                        }
                        //pauses the game
                        else
                        {
                            if (_gameData.blackScreen)
                            {
                                blackScreen.SetActive(false);
                            }
                            _pause = true;
                            Time.timeScale = 0;
                            pauseMessage.SetActive(true);
                            string text = "Spiel pausiert! \nDu hast " + gameValues.GetCoinAmount() + " Lichter eingesammelt.";
                            pauseMessage.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
                            _tts.StartSpeak(text + "\nUm das Spiel fortzusetzen, tippe zweimal kurz hintereinander auf den Bildschirm");
                            beltController.Pause();
                            animator.SetInteger("animation", 0);
                                         
                        }
                    }
                    else if (_index == 1 && _gameData.ttsActive)
                    {
                        PlayAgain();
                    }
                    else if (_index == 2 && _gameData.ttsActive)
                    {
                        Back2Menu();
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
                    if (_index == 1) 
                    {
                        _tts.StartSpeak("Nochmal. \nUm nochmal zu Spielen, tippe zweimal kurz hintereinander auf den Bildschirm oder " + _standardText);
                    }
                    else if (_index == 0)
                    {
                        _tts.StartSpeak(lostMessage.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text + _standardText);
                    }
                    else
                    {
                        _tts.StartSpeak("Menü. \nUm zurück zum Hauptmenü zu gelangen, tippe zweimal kurz hintereinander auf den Bildschirm oder " + _standardText);
                    }
                }
                
            }
        }
    }
    
    //function to play the game again
    public void PlayAgain()
    {
        _tts.StopSpeak();
        ttsBoxes.transform.GetChild(_index).gameObject.SetActive(false);
        ttsBoxes.transform.GetChild(0).gameObject.SetActive(true);
        SceneManager.LoadScene("RunnerGame");
        lostMessage.SetActive(false);
        Time.timeScale = 1;

    }
    
    
    //Function to get back to the menu
    public void Back2Menu()
    {
        _tts.StopSpeak();
        _index = -1;
        SceneManager.LoadScene("Menu");
        _tts.StartSpeak("Hauptmenü");
        Time.timeScale = 1;

    }

    //Function when the player lost
    public void Lost()
    {
        animator.SetInteger("animation", 8);
        if (_gameData.blackScreen)
        {
            blackScreen.SetActive(false);
        }
        _index = 0;
        _naviBelt.StopAll();
        
        //game over panel
        string text = "Du bist gestorben! \nDu hast " + gameValues.GetCoinAmount() + " Lichter eingesammelt.";
        if (_gameData.scores.Length > 0)
        {
            int dif = _gameData.scores[0] - gameValues.GetCoinAmount();
            if (dif > 0)
            {
                string addText = "\nDir fehlen " + dif + " Lichter bis zu deinem Highscore.";
                text += addText;
            }
            else
            {
                text += "\nDu hast einen neuen Highscore erreicht!";
            }
        }
        else
        {
            text += "\nDu hast einen neuen Highscore erreicht!";
        }
        
        //score is added to the highscore list
        _gameData.AddScore(gameValues.GetCoinAmount());
        _gameData.SaveGame();
        _tts.StartSpeak(text + _standardText);
        _lost = true;  
        lostMessage.SetActive(true);
        lostMessage.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = text;
        Time.timeScale = 0;
    }

   


    //Stops the TTS function, the connection and the phone vibration to the belt when game stops
    void OnApplicationQuit()
    {
        if (_naviBelt.ConnectStatus() == "Connected")
        {
            _naviBelt.DisconnectBelt();
        }
        _tts.StopSpeak();
        Haptics.Instance.StopHaptics();
    }

    //Pauses the TTS function, the connection and the phone vibration to the belt when the app is in the background
    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _tts.StopSpeak();
            Haptics.Instance.StopHaptics();
            beltController.Pause();
        }
        else
        {
            beltController.Resume();
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
