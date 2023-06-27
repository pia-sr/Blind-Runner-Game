
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class SpawnManager : MonoBehaviour
{
    //Positions of objects on the lane
    private List<string> _posList; 
    public string obstaclePos;
    public string coinPos;
    public int[] lanes;
    
    //Counters
    private int _coinCount;
    private int _coinStringAim; 
    private int _nothingCount;
    
    //Game setting values
    public GameValues gameValues;
    
    //booleans
    private bool _go;
    public bool smallBlocks;
    
    //Float for waiting time in coroutine
    public float waitFor;

    //Prefab for obstacles
    [SerializeField] private GameObject obstaclePrefab;

    //Parents for Trees and coins
    public Transform treesUnused;
    public Transform coinsUnused;

    
    // Start is called before the first frame update
    void Start()
    {
        //lanes are set empty at the beginning
        lanes = new int[] { 0, 0, 0 };
        StartCoroutine(CreateObstacle());
        _coinCount = 0;
        waitFor = 0.75f;
        _nothingCount = 0;
    }

    // Update is called once per frame
    void Update()
    {
        //if go is true, an obstacle/coin/trees are placed at 0
        //Unless the pos value for coin/obstacle says "Nothing", then nothing is placed
        if (_go)
        {
            _go = false;
            //Obstacle
            if (obstaclePos != "Nothing")
            {
                Instantiate(obstaclePrefab, this.transform.position, Quaternion.Euler(0,0,0), this.transform);
                
            }

            //Coin
            if (coinPos != "Nothing")
            {
                Transform coin = coinsUnused.GetChild(0);
                coin.SetParent(this.transform.GetChild(0));
                coin.gameObject.SetActive(true);
            }

            
            //Placement of trees
            //The prefab for trees is decided randomly as is their pos
            int randL = Random.Range(0, 2);
            int randR = Random.Range(0, 2);
            int randTree = Random.Range(0, 5);

            //Placement of a tree on the left side of the path
            if (randTree != 1)
            {
                int rand1 = Random.Range(0, treesUnused.childCount);
                Transform tree = treesUnused.GetChild(rand1);
                tree.position = randL == 0 ? new Vector3(-1.75f, 0.6f, 0) : new Vector3(-1.15f, 0.6f, 0);
                tree.gameObject.SetActive(true);
                Tree treeScript = tree.GetComponent<Tree>();
                treeScript.bySide = randL == 0;
                treeScript.left = true;
                treeScript.activate = true;
                tree.SetParent(this.transform.GetChild(1));
            }

            //Placement of a tree on the right side of the path
            if (randTree != 0)
            {
                int rand3 = Random.Range(0, treesUnused.childCount);
                Transform tree = treesUnused.GetChild(rand3);
                tree.position = randR == 0 ? new Vector3(1.75f, 0.6f, 0) : new Vector3(1.15f, 0.6f, 0);
                tree.gameObject.SetActive(true);
                Tree treeScript = tree.GetComponent<Tree>();
                treeScript.bySide = randR == 0;
                treeScript.left = false;
                treeScript.activate = true;
                tree.SetParent(this.transform.GetChild(1));
            }
            
            StartCoroutine(CreateObstacle());

        }

    }

    //Method to randomly decide the placement of coin and obstacle
    IEnumerator CreateObstacle()
    {
        yield return new WaitForSeconds(waitFor);
        
        //Placement of obstacle
        //the placement depends on if there are already obstacles on a lane
        //if the middle lane or the both side lanes how obstacles then no obstacle can be placed
        if (lanes[1] != 0 || (lanes[0] != 0 && lanes[2] != 0))
        {
            obstaclePos = "Nothing";
        }
        else 
        {
            //Depending on what lane is free, an obstacle can be placed on one of the empty lanes
            if (lanes[0] != 0)
            {
                _posList = new List<string>() { "Nothing", "Right", "Middle", "Nothing"};
            }
            else if (lanes[2] != 0)
            {
                _posList = new List<string>() { "Nothing", "Left", "Middle", "Nothing" };
            }
            else if (lanes[1] == 0)
            {
                _posList = new List<string>() {"Nothing", "Left",  "Middle", "Right" };
            }

            //The lane is chosen randomly
            //The higher the blockProp from gameValue, the higher the probability that "Nothing" iis choosen
            int rand = Random.Range(0, 100);
            int restIndex = (int)(100 - gameValues.blockProp) / 3;
            if (rand < gameValues.blockProp)
            {
                obstaclePos = _posList[0];
            }
            else if (rand < gameValues.blockProp + restIndex)
            {
                obstaclePos = _posList[1];
            }
            else if (rand < gameValues.blockProp + (restIndex*2))
            {
                obstaclePos = _posList[2];
            }
            else
            {
                obstaclePos = _posList[3];
            }
            
        }

        //Placement of Coins
        //The pos of coin depends on the position of obstacles
        //A coin cannot be placed on the same lane as an obstacle
        //Unless the obstacle is an flat stone than the coin must be on the same lane
        if (gameValues.upperLevel && obstaclePos != "Nothing")
        {
            coinPos = obstaclePos;
            _coinCount = 0;
        }
        //A coin continues to be in the same lane as the previous coin
        //if the lane is empty and the coin chain is not too long
        else if (_coinCount < _coinStringAim && coinPos != "Nothing" &&  ((coinPos != obstaclePos) || gameValues.upperLevel))
        { 
            _coinCount++;
        }
        //The coin continues to be on the same lane even when the coin chain is too long
        //when the middle lane is blocked and the current coin lane is still empty
        else if (lanes[1] > 0 && !gameValues.upperLevel && _nothingCount < 10)
        {
            if (coinPos == "Left" && lanes[0] != 1 && coinPos != obstaclePos)
            {
                _coinCount = 0;
            }
            else if (coinPos == "Right" && lanes[2] != 1 && coinPos != obstaclePos)
            {
                _coinCount = 0;
            }
            else
            {
                coinPos = "Nothing";
                _nothingCount++;
            }
        }
        //A new position is chosen randomly
        else
        {
            _coinCount = 0;
            List<string> posCoin = new List<string>() {"Left",  "Middle", "Right", "Nothing"};
            
            //If nothing was chosen the last 8 times, a coin needs to appear on a lane
            if (_nothingCount >= 8)
            {
                posCoin = new List<string>() {"Left",  "Middle", "Right"};
            }
            int rand = Random.Range(0, posCoin.Count);
            coinPos = posCoin[rand];
            while (coinPos != "Nothing" && !smallBlocks && (lanes[rand] != 0 || (coinPos == obstaclePos)))
            {
                
                rand = Random.Range(0, posCoin.Count);
                coinPos = posCoin[rand];
                
            }

            //if "Nothing" was chosen the _nothingCount increases
            if (coinPos == "Nothing")
            {
                _nothingCount++;
            }
            else
            {
                _nothingCount = 0;
            }
            //the coin chain length is also chosen randomly
            _coinStringAim = Random.Range(5, 15);
        }
        _go = true;
    }
}