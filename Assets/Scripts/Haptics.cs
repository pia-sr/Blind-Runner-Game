
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class Haptics : MonoBehaviour
{
    //creates an instance/game object for instance if it does not already exist
    #region Init
    private static Haptics _instance;
    public bool isActive;
    private Game _gameData;
    public static Haptics Instance
    {
        get
        {
            if (_instance == null)
            {
                //Create if it doesn't exist
                GameObject go = new GameObject("Haptics");
                _instance = go.AddComponent<Haptics>();
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
    
    
    //Function to start phone vibration for a given amount of time and for a given amplitude
   public void StartHaptics(int milliSec, int amplitude)
   {
       //only does something when haptics are activated in the game settings
       if (_gameData.hapticsActive)
       {
           isActive = true;
           AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
           StartCoroutine(VibrationCheck(milliSec));
#if UNITY_EDITOR
#elif UNITY_ANDROID
//accesses the vibrate function from the Java script
        AndroidJavaClass javaUnityClass = new AndroidJavaClass("AndroidAccessibility.Haptics");
        javaUnityClass.CallStatic("vibrate", context, milliSec, amplitude);

#endif
       }
       
   }
   
   //Function to start phone vibrations for a given amount of time for the highest amplitude (255)
   public void StartHaptics(int milliSec)
   {
       //only does something when haptics are activated in the game settings
       if (_gameData.hapticsActive)
       {
           isActive = true;
           AndroidJavaObject context = new AndroidJavaClass("com.unity3d.player.UnityPlayer").GetStatic<AndroidJavaObject>("currentActivity");
           StartCoroutine(VibrationCheck(milliSec));
#if UNITY_EDITOR
#elif UNITY_ANDROID
//accesses the vibrateIntense function from the Java script
        AndroidJavaClass javaUnityClass = new AndroidJavaClass("AndroidAccessibility.Haptics");
        javaUnityClass.CallStatic("vibrateIntense", context, milliSec);

#endif
       }
       
   }

   //Function to stop any current phone vibration
   public void StopHaptics()
   {
       //only does something when haptics are activated in the game settings and if there is a current phone vibration
       if (isActive && _gameData.hapticsActive)
       {
#if UNITY_EDITOR
#elif UNITY_ANDROID
//accesses the stopPulsing function from the Java script
        AndroidJavaClass javaUnityClass = new AndroidJavaClass("AndroidAccessibility.Haptics");
        javaUnityClass.CallStatic("stopPulsing");
#endif
           StopCoroutine(VibrationCheck(1));
           isActive = false;
       }

   }

   //Function to have a boolean active when a vibration is active
   IEnumerator VibrationCheck(float time)
   {
       yield return new WaitForSeconds(time / 1000);
       isActive = false;
   }
}
