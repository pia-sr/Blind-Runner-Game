
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class naviBelt : MonoBehaviour
{
    private AndroidJavaObject _naviBelt;

    //creates a java object to access the java class for the belt
    void Start() {
        AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        AndroidJavaObject activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        AndroidJavaObject context = activity.Call<AndroidJavaObject>("getApplicationContext");
        _naviBelt = new AndroidJavaObject("AndroidAccessibility.naviBelt.naviBelt", context);
        DontDestroyOnLoad(this.gameObject);
    }

    //Function to connect the belt via the corresponding java class
    public void ConnectBelt() {
        _naviBelt.Call("connectBelt");
    }

    //Function to disconnect the belt via the corresponding java class
    public void DisconnectBelt() {
        _naviBelt.Call("disconnectBelt");
    }

    //Function to check the belt status via the corresponding java class
    public string ConnectStatus()
    {
        return _naviBelt.Call<string>("connectionStatus");
    }

    //Function to start a vibration with given intensity at given channel via the corresponding java class
    public void StartVibration(int intensity, int channelIndex)
    {
        _naviBelt.Call("startVibration", intensity, channelIndex);
    }

    //Function to stop a vibration at given channel via the corresponding java class
    public void StopVibration(int channelIndex)
    {
        _naviBelt.Call("stopVibration", channelIndex);
    }

    //Function to start all vibration via the corresponding java class
    public void StopAll()
    {
        _naviBelt.Call("stopAll");
    }

    //Function to resume all vibrations as before stop via the corresponding java class
    public void ResumeVibration()
    {
        _naviBelt.Call("resumeVibration");
    }

    //Function to start a pulse pattern with specific parameters via the corresponding java class
    public void StartPulse(int intensity, int duration, int period, int continuous, int channelIndex)
    {
        _naviBelt.Call("startPulse", intensity, duration, period, continuous, channelIndex);
    }

    //List of the channel activity to check which channel is active
    public int[] channelActivity()
    {
        return _naviBelt.Call<int[]>("channelActivity");
    }
    
}
