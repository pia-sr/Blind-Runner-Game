
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class for all the variables that need to be saved
//source:https://www.youtube.com/watch?v=XOjd_qU2Ido
public class Game : MonoBehaviour
{
    public int[] scores;

    public bool ttsActive;

    public bool hapticsActive;

    public bool beltActive;

    public bool soundActive;

    public bool personalisedSettings;

    public bool blackScreen;
    
    // Start is called before the first frame update
    private void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    //Function to get the read variables
    public void loadGame()
    {
        GameData data = SaveSystem.loadGame();
        scores = data.scores;
        ttsActive = data.ttsActive;
        hapticsActive = data.hapticsActive;
        beltActive = data.beltActive;
        soundActive = data.soundActive;
        blackScreen = data.blackScreen;
        personalisedSettings = data.personalisedSettings;
    }

    //Function to add and sort the given score in the highscore list
    public void AddScore(int score)
    {
        int length = scores.Length;
        int[] newScores = new int[length + 1];
        Array.Copy(scores,newScores,length);
        newScores[length] = score;
        if (length > 1)
        {
            for (int i = 0; i < length; i++)
            {
                for (int j = 0; j < length - i; j++)
                {
                    if (newScores[j+1] > newScores[j])
                    {
                        (newScores[j], newScores[j + 1]) = (newScores[j + 1], newScores[j]);
                    }
                }
            }
        }

        scores = new int[length + 1];
        Array.Copy(newScores,scores, length+1);

    }

    //getter for score
    public int[] GetScore()
    {
        return scores;
    }
    
    //function to save the game
    public void SaveGame()
    {
        SaveSystem.saveGame(this);
    }
}