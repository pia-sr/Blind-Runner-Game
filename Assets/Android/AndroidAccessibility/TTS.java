package AndroidAccessibility;


/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

import android.speech.tts.TextToSpeech;

import com.unity3d.player.UnityPlayer;

import android.util.Log;

import java.util.Locale;

public class TTS{

    private static TextToSpeech textToSpeech;
    public boolean isSpeaking;

    //Function to set the settings for TTS
    public static void TTSSettings()
    {
        if (textToSpeech == null) {
            textToSpeech = new TextToSpeech(UnityPlayer.currentActivity, new TextToSpeech.OnInitListener() {
                @Override
                public void onInit(int status) {
                    if (status == TextToSpeech.SUCCESS) {
                        int result = textToSpeech.setLanguage(Locale.GERMAN);
                        if (result == TextToSpeech.LANG_MISSING_DATA || result == TextToSpeech.LANG_NOT_SUPPORTED) {
                            Log.e("Bridge", "Language is not supported");
                        }
                    } else {
                        Log.e("Bridge", "TextToSpeech initialization failed");
                    }
                }
            });
        }
    }
    
    //Function for tts to start speaking a given text
    public static void TTSSpeak(String textToSpeak)
    {
        if (textToSpeech == null)
        {
            TTSSettings();
        }
        textToSpeech.speak(textToSpeak, TextToSpeech.QUEUE_FLUSH, null, null);
    }

    //Function to stop tts
    public static void TTSStop()
    {
        if(textToSpeech.isSpeaking())
        {
            textToSpeech.stop();
        }

    }
    
    //Function to pause tts for a given amount of seconds
    public static void TTSPause(long seconds)
    {
        textToSpeech.playSilentUtterance(seconds,TextToSpeech.QUEUE_FLUSH, null);
    }
    
    //Function to check if tts is currently active
    public static boolean TTSisSpeaking()
    {
        return textToSpeech.isSpeaking();
    }
}


