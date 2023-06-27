
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

//Class to read or write a file to save the game data
//source: https://www.youtube.com/watch?v=XOjd_qU2Ido
public static class SaveSystem 
{
    //Function to save the game in binary code in a file on the played device
    public static void saveGame(Game game)
    {
        FileStream stream = null;
        BinaryFormatter formatter = new BinaryFormatter();
        string path = Application.persistentDataPath + "/gameData.game";
        if (!System.IO.File.Exists("gameData.game"))
        {

            stream = new FileStream(path, FileMode.Create);
        }

        GameData data = new GameData(game);

        formatter.Serialize(stream, data);
        stream.Close();
    }
    
    //Function to load the data from an existing file
    public static GameData loadGame()
    {
        string path = Application.persistentDataPath + "/gameData.game";
        FileStream stream = new FileStream(path, FileMode.Open);
        if (File.Exists(path) && stream.Length > 0)
        {
            BinaryFormatter formatter = new BinaryFormatter();

            GameData data = (GameData)formatter.Deserialize(stream);
            stream.Close();

            return data;
        }
        else
        {
            return null;
        }
    }
}