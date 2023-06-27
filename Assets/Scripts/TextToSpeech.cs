
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using UnityEngine.UI;
using System;
using UnityEngine.Android;


namespace TextSpeech
{
    public class TextToSpeech : MonoBehaviour
    {
        //creates an instance/game object for instance if it does not already exist
        #region Init

        private static TextToSpeech _instance;
        private Game _gameData;

        public static TextToSpeech Instance
        {
            get
            {
                if (_instance == null)
                {
                    //Create if it doesn't exist
                    GameObject go = new GameObject("TextToSpeech");
                    _instance = go.AddComponent<TextToSpeech>();
                }

                return _instance;
            }
        }

        void Awake()
        {
            _gameData = GameObject.FindObjectOfType<Game>();
            _instance = this;
        }

        #endregion

        //Function to set the settings of tts implementation 
        public void Settings()
        {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidJavaClass javaUnityClass = new AndroidJavaClass("AndroidAccessibility.TTS");
        javaUnityClass.CallStatic("TTSSettings");
#endif
        }

        //Function to start speaking a given text
        public void StartSpeak(string _message)
        {
            if (_gameData.ttsActive)
            {
#if UNITY_EDITOR
#elif UNITY_IPHONE
        _TAG_StartSpeak(_message);
#elif UNITY_ANDROID
        AndroidJavaClass javaUnityClass = new AndroidJavaClass("AndroidAccessibility.TTS");
        javaUnityClass.CallStatic("TTSSpeak", _message);
#endif
            }

        }

        //Function to pause the reading of the text for given amount of seconds
        public void PauseTTS(long seconds)
        {
            if (_gameData.ttsActive)
            {
#if UNITY_EDITOR
#elif UNITY_ANDROID
        AndroidJavaClass javaUnityClass = new AndroidJavaClass("AndroidAccessibility.TTS");
        javaUnityClass.CallStatic("TTSPause", seconds);
#endif
            }
        }

        //Function to stop the tts from continuing to speak
        public void StopSpeak()
        {
            if (_gameData.ttsActive)
            {
#if UNITY_EDITOR
#elif UNITY_IPHONE
        _TAG_StopSpeak();
#elif UNITY_ANDROID
        AndroidJavaClass javaUnityClass = new AndroidJavaClass("AndroidAccessibility.TTS");
        javaUnityClass.CallStatic("TTSStop");
#endif
            }

        }

        //Function to check if tts is currently speaking/active
        public bool isSpeaking()
        {
#if UNITY_EDITOR
            return false;
#elif UNITY_ANDROID
        AndroidJavaClass javaUnityClass = new AndroidJavaClass("AndroidAccessibility.TTS");
        return javaUnityClass.CallStatic<bool>("TTSisSpeaking");
#endif
        }
    }
}