using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Drawing;
using System;
using Color = UnityEngine.Color;

public class AddGrassAndTrees : MonoBehaviour
{
    public static string user = Environment.GetEnvironmentVariable("userprofile");

    public Terrain terrain;

    // Use this for initialization
    void Start()
    {
        string path =  user + @"\New Unity Project\Assets\RawBmp\Texture_Terrain.png";

        TerrainData terrainData = terrain.terrainData;

        Texture2D texture = new Texture2D(2, 2);

        texture.LoadImage(File.ReadAllBytes(path));
        //uses ground texture to see if to add grass or not -- could be mimicked for assets

        int width = terrainData.detailWidth;
        int height = terrainData.detailHeight;
        float elevation;

        int treescale = terrainData.heightmapResolution;

        int[,] map = terrainData.GetDetailLayer(0, 0, width, height, 0);
        
        int i = 0;

        int ximage;
        int yimage;

        double widthdouble = Convert.ToDouble(width);
        double heightdouble = Convert.ToDouble(height); //double division needs to use doubles

        double xconv = 256.0 / widthdouble;
        double yconv = 256.0 / heightdouble; //need to convert from the image res to the detail res

        int red;
        int green;
        int blue;

        float xtemp;
        float ytemp;


        int count = 0;
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {

                xtemp = x;
                ytemp = y;

                xtemp = x / terrainData.size.x;
                ytemp = y / terrainData.size.z;

                elevation = terrainData.GetHeight(x,y);
                elevation = elevation / terrainData.size.y;

                ximage = Convert.ToInt32(x * xconv); //need to then convert
                yimage = Convert.ToInt32(y * yconv);

                Color Pixel = texture.GetPixel(ximage, yimage);

                red = Convert.ToInt32(Pixel.r * 256);
                green = Convert.ToInt32(Pixel.g * 256);
                blue = Convert.ToInt32(Pixel.b * 256);

                if (green <= 85 && red == 0 && blue == 0){

                    if (green < 62 || green > 83) {
                        map[y, x] = 10; //lower density if border colour
                    }
                    else
                    {
                        map[y, x] = 20; //need to flip to get equivalent image coords to the detail coords
                    }
                }

                else if (red == 108 && green == 59 && blue == 30) //accounts for trees colour as well
                {
                    map[y, x] = 20; //need to flip to get equivalent image coords to the detail coords
                    count++;
                }

                else {
                    map[y, x] = 0;
                }
            }
        }

        double conv = 256.0 / 3000.0;

        for (int x = 0; x < terrainData.size.x; x++)
        {
            for (int y = 0; y < terrainData.size.z; y++)
            {
                Color Pixel1 = texture.GetPixel(Convert.ToInt32(x * conv), Convert.ToInt32(y * conv));
                red = Convert.ToInt32(Pixel1.r * 256);
                green = Convert.ToInt32(Pixel1.g * 256);
                blue = Convert.ToInt32(Pixel1.b * 256);

                if (red == 108 && green == 59 && blue == 30) //accounts for trees colour as well
                {
                    if (x % 5 == 0 && y % 7 == 0) //So not every pixel on brown has a tree
                    {
                        count++; //most accurate count
                    }
                }
            }
        }


        TreeInstance treeorig = terrainData.GetTreeInstance(0); //Needs to use a tree to create an image from

        TreeInstance[] treereset = new TreeInstance[0]; //so all previous trees are removed
        terrainData.SetTreeInstances(treereset, true);

        TreeInstance[] trees = new TreeInstance[count];

        for (int a = 0; a < count; a++)
        {
            trees[a] = treeorig;
        }

        for (int x = 0; x < terrainData.size.x; x++)
        {
            for (int y = 0; y < terrainData.size.z; y++)
            {

                xtemp = x;
                ytemp = y;

                xtemp = x / terrainData.size.x;
                ytemp = y / terrainData.size.z;

                elevation = terrainData.GetHeight(x, y);
                elevation = elevation / terrainData.size.y;

                ximage = Convert.ToInt32(x * conv); //need to then convert
                yimage = Convert.ToInt32(y * conv);

                Color Pixel2 = texture.GetPixel(ximage, yimage);

                red = Convert.ToInt32(Pixel2.r * 256);
                green = Convert.ToInt32(Pixel2.g * 256);
                blue = Convert.ToInt32(Pixel2.b * 256);

                if (red == 108 && green == 59 && blue == 30) //accounts for trees colour as well
                {
                    if (x % 5 == 0 && y % 7 == 0)
                    {
                        trees[i].position = new Vector3(xtemp, elevation, ytemp);
                        i++;
                        //Debug.Log(Vector3.Scale(trees[i].position, terrainData.size) + terrain.transform.position);
                    }
                }
            }
        }

        terrainData.SetTreeInstances(trees, true);
        terrainData.SetDetailLayer(0, 0, 0, map);
    }
}
