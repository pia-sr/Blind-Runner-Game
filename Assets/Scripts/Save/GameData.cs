
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Class to serialize the variables from the game
//source:https://www.youtube.com/watch?v=XOjd_qU2Ido
[System.Serializable]
public class GameData
{
    public int[] scores;
    public bool ttsActive;

    public bool hapticsActive;

    public bool beltActive;

    public bool soundActive;
    
    public bool blackScreen;

    public bool personalisedSettings;
    
    //Function to copy the data from game
    public GameData(Game game)
    {
        scores = game.GetScore();
        ttsActive = game.ttsActive;
        hapticsActive = game.hapticsActive;
        beltActive = game.beltActive;
        soundActive = game.soundActive;
        blackScreen = game.blackScreen;
        personalisedSettings = game.personalisedSettings;
    }
}