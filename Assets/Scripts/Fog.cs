using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for the fog
//source: https://www.youtube.com/watch?v=EFt_lLVDeRo
//Slight alterations to accommodate the right image of the fog 
public class Fog : MonoBehaviour {
    [Header("Fog")]
    public Shader fogShader;
    public Color fogColor;
    
    [Range(0.0f, 1000.0f)]
    public float fogDensity;
    
    [Range(0.0f, 100.0f)]
    public float fogOffset;

    public float fogHeight;
    
    private Material fogMat;

    void Start() {
        if (fogMat == null) {
            fogMat = new Material(fogShader);
            fogMat.hideFlags = HideFlags.HideAndDontSave;
        }

        Camera cam = GetComponent<Camera>();
        cam.depthTextureMode = cam.depthTextureMode | DepthTextureMode.Depth;
    }

    [ImageEffectOpaque]
    void OnRenderImage(RenderTexture source, RenderTexture destination) {
        fogMat.SetVector("_FogColor", fogColor);
        fogMat.SetFloat("_FogDensity", fogDensity);
        fogMat.SetFloat("_FogOffset", fogOffset);
        fogMat.SetFloat("_FogHeight", fogHeight);
        Graphics.Blit(source, destination, fogMat);
    }
}