
/*
 * Copyright (c) 2023 Pia Schroeter. All rights reserved.
 *
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassManager : MonoBehaviour
{
    //parent for grass
    public Transform grassUnused;
    

    // Update is called once per frame
    void Update()
    {
        //The position is chosen randomly on both sides
        //each row of grass is different long and decided randomly
        
        float rand1 = Random.Range(-0.6f, -0.85f);
        float xPosLeft = -2.5f;
        float rand2 = Random.Range(0.6f, 0.85f);
        float xPosRight = 2.5f;
        
        //Grass on the left
        while (xPosLeft < rand1)
        {
            if (grassUnused.childCount > 0)
            {
                Transform grass = grassUnused.GetChild(0);
                grass.SetParent(this.transform);
                grass.position = new Vector3(xPosLeft, 0.6f, 0);
                grass.gameObject.SetActive(true);
                grass.GetComponent<Grass>().activate = true;
                
            }
            xPosLeft += 0.05f;
        }

        //Grass on the right
        while (xPosRight > rand2)
        {
            if (grassUnused.childCount > 0)
            {
                Transform grass = grassUnused.GetChild(0);
                grass.SetParent(this.transform);
                grass.position = new Vector3(xPosRight, 0.6f, 0);
                grass.gameObject.SetActive(true);
                grass.GetComponent<Grass>().activate = true;
                
            }
            xPosRight -= 0.05f;
        }
    }
}