using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Drawing;

public class CivGen : MonoBehaviour
{
    public string seed = TerrainGen.BitmapClass.seed;

    public static string user = Environment.GetEnvironmentVariable("userprofile");
    public static (int[], int[], int[]) CivilisationOverlay(string seed)
    {
        string path = user + @"\New Unity Project\Assets\RawBmp\Texture_Terrain.png";

        Bitmap Texture = new Bitmap(path);

        static bool CoorCheck(int x, int y, Bitmap Texture)
        { //Checks coordinates to see if they are ok for civilisation

            int bitheight = Texture.GetPixel(x, y).R;

            if (bitheight > 65) //has to be above water
            {
                return true;
            }
            else { return false; }
        }

        static int[] AssignNew(int[] coord, int count)
        { //function to assign coordinate a nearby pixel that is valid
            coord[0] = coord[0] + count;
            coord[1] = coord[1] - count;

            count++;
            count = -1 * count;  //semi spiral pattern

            return coord;
        }

        //Function to find coords for civilisation

        string[] seedarray = seed.Split('.');
        string[] resolution = seedarray[0].Split(','); //Comma seperates values in sections e.g. width,height under resolution
        int width = Convert.ToInt16(resolution[0]); //Res will stay low for now
        int height = Convert.ToInt16(resolution[1]);

        int peak_num = Convert.ToInt32(seedarray[1]);

        string generator_string = seedarray[2];
        //Console.WriteLine(generator_string);
        long generator_int = Convert.ToInt64(generator_string);

        //created so can use generator in different ways
        int generator_int1 = Convert.ToInt32(generator_string.Substring(0, 3));
        int generator_int2 = Convert.ToInt32(generator_string.Substring(3, 3));
        int generator_int3 = Convert.ToInt32(generator_string.Substring(6, 3));
        int generator_int4 = Convert.ToInt32(generator_string.Substring(9, 3));

        int[] generator_list = new int[4] { generator_int1, generator_int2,
                                                generator_int3, generator_int4
            };

        int[] coor1 = { Math.Abs(generator_list[0] - generator_list[1]) % width, Math.Abs(generator_list[1] - generator_list[2]) % height };
        int[] coor2 = { Math.Abs(generator_list[2] - generator_list[3]) % width, Math.Abs(generator_list[3] - generator_list[0]) % height };
        int[] coor3 = { Math.Abs(generator_list[0] - generator_list[2]) % width, Math.Abs(generator_list[1] - generator_list[3]) % height };

        int count1 = 1;
        int count2 = 1;
        int count3 = 1;

        bool isValid = false; //coordinates need to be at a satisfactory height

        bool valid1;
        bool valid2;
        bool valid3;

        while (!isValid)
        {
            coor1[0] = coor1[0] % width; coor1[1] = coor1[1] % height;
            coor2[0] = coor2[0] % width; coor2[1] = coor2[1] % height;
            coor3[0] = coor3[0] % width; coor3[1] = coor3[1] % height;

            valid1 = CoorCheck(coor1[0], coor1[1], Texture);
            valid2 = CoorCheck(coor2[0], coor2[1], Texture);
            valid3 = CoorCheck(coor2[0], coor2[1], Texture);

            if (!valid1 || !valid2 || !valid3)
            {
                isValid = false;
            }

            if (!valid1)
            {
                if (count1 < 200)
                {
                    coor1 = AssignNew(coor1, count1);
                    count1++;
                }
                else
                {
                    valid1 = true; //if by count 200 a good coord hasn't been found, give up
                }
            }

            if (!valid2)
            {
                if (count2 < 200)
                {
                    coor2 = AssignNew(coor2, count2);
                    count2++;
                }
                else
                {
                    valid2 = true; //if by count 200 a good coord hasn't been found, give up
                }
            }

            if (!valid3)
            {
                if (count3 < 200)
                {
                    coor3 = AssignNew(coor3, count3);
                    count3++;
                }
                else
                {
                    valid3 = true; //if by count 200 a good coord hasn't been found, give up
                }
            }

            if (valid1 && valid2 && valid3)
            {
                isValid = true;
            }
        }
        //coordinates on where to place villages

        return (coor1, coor2, coor3);
    }

}
