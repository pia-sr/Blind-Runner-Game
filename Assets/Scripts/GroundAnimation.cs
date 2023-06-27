using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//script make it look like the armadillo is moving
//Slight changes to accommodate the speed of the game
//Source: https://answers.unity.com/questions/19848/making-textures-scroll-animate-textures.html
public class GroundAnimation : MonoBehaviour
{
    private int materialIndex = 0;

    private Vector2 uvAnimationRate = new Vector2( 0.0f, -1.0f );

    private Vector2 uvOffset = Vector2.zero;
    public string textureName;
    private GameValues _gameValues;

    public Renderer renderer;
    
    
    // Start is called before the first frame update
    void Start()
    {
        _gameValues = GameObject.FindObjectOfType<GameValues>();
        
    }

    void LateUpdate()
    {
        uvOffset += (uvAnimationRate * ((_gameValues.speed/10) * Time.deltaTime));
        if( renderer.enabled )
        {
            renderer.materials[ materialIndex ].SetTextureOffset( textureName, uvOffset );
        }
    }
}