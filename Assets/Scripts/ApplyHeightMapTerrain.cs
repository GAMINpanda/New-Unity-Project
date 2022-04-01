using System.IO;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSurgeon.HeightMapper;


public class ApplyHeightMapTerrain : MonoBehaviour
{

    public static string user = Environment.GetEnvironmentVariable("userprofile");

    // Start is called before the first frame update
    void Start()
    {

        string path = user + @"\New Unity Project\Assets\RawBmp\HeightMap.png";
        Texture2D HeightMap = new Texture2D(2, 2);

        HeightMap.LoadImage(File.ReadAllBytes(path));

        Terrain terrain = gameObject.GetComponent<Terrain>();

        HeightMapFromTexture.ApplyHeightmap(HeightMap, terrain.terrainData);
    }
}
