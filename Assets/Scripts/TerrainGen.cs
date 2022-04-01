using System.Collections;
using System.Collections.Generic;
using System;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using UnityEngine;
using Random = System.Random;
using Color = System.Drawing.Color;
using Graphics = System.Drawing.Graphics;

public class TerrainGen : MonoBehaviour
{
    public class BitmapClass
    {
        public static string user = Environment.GetEnvironmentVariable("userprofile");

        public static Bitmap Texture(Bitmap bitmap_peaks, Bitmap bitmap_grass, bool tf)
        {
            var target = new Bitmap(bitmap_peaks.Width, bitmap_peaks.Height, PixelFormat.Format32bppArgb);
            var graphics = Graphics.FromImage(target);
            //graphics.CompositingMode = CompositingMode.SourceOver;

            graphics.DrawImage(bitmap_peaks, 0, 0);
            graphics.DrawImage(bitmap_grass, 0, 0);

            string path = user + @"\New Unity Project\Assets\RawBmp\Texture_Terrain.png";

            if (tf)
            {
                if (File.Exists(path))
                    File.Delete(path); //Replaces file

                target.Save(path, ImageFormat.Png);
            }

            return target;
        }

        static Bitmap TreesAndFoilageGen(string seed, Bitmap bitmap_grass)
        {
            //Function to add trees and foilage

            string[] seedarray = seed.Split('.'); //Stop seperates sections e.g. resolution.peak_num.peak_points

            string[] resolution = seedarray[0].Split(','); //Comma seperates values in sections e.g. width,height under resolution
            int width = Convert.ToInt16(resolution[0]); //Res will stay low for now
            int height = Convert.ToInt16(resolution[1]);

            Bitmap bitmap_treesfoilage = new Bitmap(width, height);

            int peak_num = Convert.ToInt32(seedarray[1]);

            string generator_string = seedarray[2];
            //Console.WriteLine(generator_string);
            int generator_int = Convert.ToInt32(generator_string.Substring(0, 6));


            int rand;
            string randstring;
            int len;
            int x;
            int y;
            Color Pixel;

            for (int i = 0; i < (peak_num * peak_num * 30); i++)
            {
                rand = (i + 1) * (generator_int % (i + 1));
                rand = Convert.ToInt32(rand / ((i * i) + 1));
                randstring = Convert.ToString(rand);
                len = randstring.Length;

                if (len > 6)
                {
                    randstring = randstring.Substring(len - 4); //generating a random number each time
                }

                rand = Convert.ToInt32(randstring);

                x = Math.Abs((rand + 1) * (i + 1)) % width;
                y = Math.Abs((rand + 5) * ((i * i) + 1)) % height;

                Pixel = bitmap_grass.GetPixel(x, y);
                //Console.WriteLine("x: {0}, y: {1}",x,y);

                if (Pixel.G < 85 && Pixel.R == 0 && Pixel.G > 60)
                {
                    bitmap_treesfoilage.SetPixel(x, y, Color.FromArgb(255, 108, 59, 30)); //Nice brown
                }
            }

            return bitmap_treesfoilage;
        }

        public static Bitmap HeightMapOverlay(Bitmap bitmap_peaks1, Bitmap bitmap_peaks2)
        {
            int width = bitmap_peaks1.Width;
            int height = bitmap_peaks2.Height;
            int avg;

            for (int x = 0; x < width; x++) //Finds average of heights between Bitmaps and sets the pixel to that average
            {
                for (int y = 0; y < height; y++)
                {
                    avg = (bitmap_peaks1.GetPixel(x, y).R) + (bitmap_peaks2.GetPixel(x, y).R);
                    avg = Convert.ToInt32(avg / 2);
                    bitmap_peaks1.SetPixel(x, y, Color.FromArgb(255, avg, avg, avg));
                }
            }
            

            return bitmap_peaks1;
        }

        public static bool IsWater(Color Pixel)
        {
            if (Pixel.R == 35 && Pixel.G == 137 && Pixel.B == 218)
            {
                return true;
            }
            else { return false; }
        }

        public static void GrassWaterGen(Bitmap bitmap_peaks, string seed)
        {
            //Function to generate grass based off a given bitmap
            int width = bitmap_peaks.Width;
            int height = bitmap_peaks.Height;

            Bitmap bitmap_grass = new Bitmap(width, height);
            
            Color Pixel;
            
            Color PixelUp;
            Color PixelUpRight;
            Color PixelUpLeft;
            Color PixelDown;
            Color PixelDownRight;
            Color PixelDownLeft;
            Color PixelLeft;
            Color PixelRight;
            
            int score = 0;
            bool check;
            

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    score = 0;
                    Pixel = bitmap_peaks.GetPixel(x, y);

                    if (Pixel.R < 58)
                    {
                        bitmap_grass.SetPixel(x, y, Color.FromArgb(255, 35, 137, 218)); //supposed to be a nice shade of blue
                    }

                    else if (Pixel.R < 60)
                    {
                        bitmap_grass.SetPixel(x, y, Color.FromArgb(255, 194, 178, 128)); //supposed to be a nice sand colour
                    }
                    else
                    {
                        if (Pixel.R < 85)
                        {
                            bitmap_grass.SetPixel(x, y, Color.FromArgb(255, 0, Pixel.R, 0)); //Grass
                        }
                    }


                    /*
                    //Surrounding Pixels, doesn't bother with outside layer of pixels since they won't be visible anyway in unity project and take long to count for
                    //Apply water after so it isn't overwritten
                    //below mountain height water has to be connected and lower than existing water
                    if ((x > 1 && x < width - 1) && (y > 1 && y < height - 1))
                    {
                        PixelUp = bitmap_peaks.GetPixel(x, y + 1);
                        PixelUpRight = bitmap_peaks.GetPixel(x + 1, y + 1);
                        PixelUpLeft = bitmap_peaks.GetPixel(x - 1, y + 1);
                        PixelDown = bitmap_peaks.GetPixel(x, y - 1);
                        PixelDownRight = bitmap_peaks.GetPixel(x + 1, y - 1);
                        PixelDownLeft = bitmap_peaks.GetPixel(x - 1, y - 1);
                        PixelRight = bitmap_peaks.GetPixel(x + 1, y); ;
                        PixelLeft = bitmap_peaks.GetPixel(x - 1, y);

                        if (Pixel.G > 57 && Pixel.G < 100) { check = true; }
                        else { check = false; }

                        if (check)
                        {
                            if (PixelUp.G > Pixel.G || IsWater(PixelUp)) { score++; }

                            if (PixelDown.G > Pixel.G || IsWater(PixelDown)) { score++; }

                            if (PixelUpRight.G > Pixel.G || IsWater(PixelUpRight)) { score++; }

                            if (PixelDownRight.G > Pixel.G || IsWater(PixelDownRight)) { score++; }

                            if (PixelUpLeft.G > Pixel.G || IsWater(PixelUpLeft)) { score++; }

                            if (PixelDownLeft.G > Pixel.G || IsWater(PixelDownLeft)) { score++; }

                            if (PixelLeft.G > Pixel.G || IsWater(PixelLeft)) { score++; }

                            if (PixelRight.G > Pixel.G || IsWater(PixelRight)) { score++; }

                            if (score > 4)
                            { bitmap_grass.SetPixel(x, y, Color.FromArgb(255, 35, 137, 218)); } //sets blue colour
                        }
                    }
                    */
                }
            }

            bitmap_grass = Texture(bitmap_grass, TreesAndFoilageGen(seed, bitmap_grass), false);

            Texture(bitmap_peaks, bitmap_grass, true);
        }

        public static Bitmap BitmapPeaksShort(string seed)
        { //A procedure that attempts to improve on the long string method, don't need to input exact coordinates
            //seed in form resw,resh.num_peaks.generatorstring(12 chars long)

            string[] seedarray = seed.Split('.'); //Stop seperates sections e.g. resolution.peak_num.peak_points

            string[] resolution = seedarray[0].Split(','); //Comma seperates values in sections e.g. width,height under resolution
            int width = Convert.ToInt16(resolution[0]); //Res will stay low for now
            int height = Convert.ToInt16(resolution[1]);

            Bitmap bitmap = new Bitmap(width, height);

            Bitmap bitmap_grass = new Bitmap(width, height); //more efficient to create grass and height map at the same time

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

            int radius; //self explanatory
            int peak_calc1; //"random" number between 1 and 4
            int peak_calc2;//same as above
            int cur_genw; //act as x and y values
            int cur_genh;

            int[] xrange = new int[2];
            int[] yrange = new int[2];
            double range_prox;
            int x_prox;
            int y_prox;
            Color Pixel;
            int colour_gen;
            int Location_cur;

            for (int peak_num_cur = 0; peak_num_cur < peak_num; peak_num_cur++)
            {
                radius = Convert.ToInt32(((generator_int1 * generator_int2) + 1) % ((peak_num_cur + 1) * 5));//Trying to get a pseudo random value for the radius
                radius = radius % width; //within range of resolution ish

                if (peak_num_cur % 2 == 0) //Means the sequences aren't exactly the same
                {
                    peak_calc1 = (peak_num_cur + 1) % 4;
                    peak_calc2 = ((peak_num_cur * peak_calc1) + 2) % 4;
                }
                else
                {
                    if (peak_num_cur % 3 == 0)
                    {
                        peak_calc1 = (peak_num_cur + 2) % 4;
                        peak_calc2 = ((peak_num_cur * peak_calc1) + 3) % 4;
                    }

                    else
                    {
                        peak_calc1 = (peak_num_cur + 3) % 4;
                        peak_calc2 = ((peak_num_cur * peak_calc1) + 1) % 4;
                    }
                }

                peak_calc2 = peak_num_cur % 4; //Keeps inside generator substrings

                cur_genw = (generator_list[peak_calc1] * radius * (peak_num_cur + 1)) + 1;
                cur_genh = (generator_list[peak_calc2] * radius * (peak_num_cur + 1)) + 1;

                cur_genw = cur_genw % width; //so values stay inside the resolution
                cur_genh = cur_genh % height;

                //Console.WriteLine("width: {0}, height: {1}, radius: {2}",cur_genw, cur_genh, radius);
                //Now need to generate values into bitmap (can use previous algorithm)

                xrange[0] = cur_genw - radius; //Finding Upper and Lower limits for x and y
                if (xrange[0] < 0) //Making sure coordinates outside of the resolution aren't accessed
                { xrange[0] = 0; }

                xrange[1] = cur_genw + radius; ;
                if (xrange[1] > width)
                { xrange[1] = width; }

                yrange[0] = cur_genh - radius; ;
                if (yrange[0] < 0)
                { yrange[0] = 0; }

                yrange[1] = cur_genh + radius; ;
                if (yrange[1] > height)
                { yrange[1] = height; }

                //Console.WriteLine("x: {0}, y: {1}, rad_cur: {2}",peak_w,peak_h,rad_cur);
                //Console.WriteLine("xrangemin: {0}, xrangemax: {1}", xrange[0], xrange[1]);
                //Console.WriteLine("yrangemin: {0}, yrangemax: {1}", yrange[0], yrange[1]);

                for (int x = xrange[0]; x < xrange[1]; x++)
                {
                    for (int y = yrange[0]; y < yrange[1]; y++)
                    {
                        x_prox = Math.Abs(cur_genw - x);
                        y_prox = Math.Abs(cur_genh - y);

                        x_prox = Convert.ToInt32(Math.Pow(x_prox, 2));
                        y_prox = Convert.ToInt32(Math.Pow(y_prox, 2));

                        range_prox = Math.Sqrt(x_prox + y_prox) * 10; //Pythagoras to grasp proximity to point

                        range_prox = (range_prox / radius);

                        Pixel = bitmap.GetPixel(x, y);
                        Location_cur = Pixel.R; //Gets red value of current pixel

                        if (radius > 255) { radius = 255; }//changing radius just for height generation

                        if (range_prox == 0) { colour_gen = radius; } //max height depends on the max radius

                        else
                        {
                            colour_gen = Convert.ToInt16(radius / (range_prox + 0.9)); //don't want decimals

                            if (colour_gen > radius) { colour_gen = radius; }
                        }

                        Pixel = Color.FromArgb(255, colour_gen, colour_gen, colour_gen); //so the gradient is smooth

                        if (Location_cur < colour_gen) { bitmap.SetPixel(x, y, Pixel); } //Sort of weighted so Higher Colours won't be overwitten

                        //Console.WriteLine("Pixel: {0}, Location_cur: {1}, colour_gen: {2}",Pixel,Location_cur, colour_gen);
                    }
                }
            }

            return bitmap;
        }

        public static string BitmapSeedGen(string res)
        {
            //Procedure to generate a (random) string that can be utilised by the Image generator

            Random random = new Random();

            string seed = "";
            int three_dig;

            for (int i = 0; i < 4; i++)
            { //generates four sets of three digit numbers
                three_dig = random.Next(100, 1000);
                seed = seed + Convert.ToString(three_dig);
            }

            //Console.WriteLine(seed);

            seed = res + "." + Convert.ToString(random.Next(45,91)) + "." + seed; //The peak number also means lower values have more water

            Debug.Log(seed);
            return seed;
        }

        public static string seed = BitmapSeedGen("256,256"); //Higher res is a time sacrifice
    }

    // Start is called before the first frame update
    void Start()
    {
        //General rule is the higher the res, the more iterations you want and the lower the terrain height

        string seed = BitmapClass.seed;

        //string seed = "256,256.52.123456789123"; //If you need to test on the same seed -- produces nice islands when combined with the other seed
        Bitmap bitmap_peaks1 = BitmapClass.BitmapPeaksShort(seed);
        Bitmap bitmap_peaks2;

        for (int i = 0; i < 2; i++) //more iterations makes rougher and generally flatter terrain, but the difference is hard to see on low res
        {
            bitmap_peaks2 = BitmapClass.BitmapPeaksShort(BitmapClass.BitmapSeedGen("256,256")); //Generates two different Bitmaps

            //bitmap_peaks2 = BitmapClass.BitmapPeaksShort("256,256.52.321987654321");

            bitmap_peaks1 = BitmapClass.HeightMapOverlay(bitmap_peaks2, bitmap_peaks1); //Overlays for further variety
        }

        BitmapClass.GrassWaterGen(bitmap_peaks1, seed);

        string path = BitmapClass.user + @"\New Unity Project\Assets\RawBmp\HeightMap.png";
        if (File.Exists(path))
            File.Delete(path); //Replaces file

        bitmap_peaks1.Save(path, ImageFormat.Png);

        (int[] coor1, int[] coor2, int[] coor3) = CivGen.CivilisationOverlay(seed);
        string string1 = "coor1: " + coor1[0] + "," + coor1[1];
        string string2 = "coor2: " + coor2[0] + "," + coor2[1];
        string string3 = "coor3: " + coor3[0] + "," + coor3[1];
        Debug.Log(string1);
        Debug.Log(string2);
        Debug.Log(string3);
    }
}
