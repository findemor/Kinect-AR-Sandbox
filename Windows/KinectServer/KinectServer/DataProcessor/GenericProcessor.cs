﻿using KinectServer.Kinect;
using KinectServer.TCP;
using Microsoft.Kinect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KinectServer.DataProcessor
{
    public class GenericProcessor : IDataProcessor
    {
        public const string KEY_DEPTH_ARRAY = "DepthArray";
        public const string KEY_DEPTH_WIDTH = "DepthWidth";
        public const string KEY_DEPTH_HEIGHT = "DepthHeight";
        public const string KEY_DEPTH_MIN = "MinDepth";
        public const string KEY_DEPTH_MAX = "MaxDepth";


        public TCPData GetProcessedData(KinectData kinectData)
        {
            TCPData pd = new TCPData()
            {
                Metadata = new Dictionary<string, string>()
            };

            int max = kinectData.MaxDepth;
            int min = kinectData.MinDepth;

            //Metemos los atributos que queremos enviar
            pd.Metadata.Add(KEY_DEPTH_WIDTH, kinectData.DepthWidth.ToString());
            pd.Metadata.Add(KEY_DEPTH_HEIGHT, kinectData.DepthHeight.ToString());

            pd.Metadata.Add(KEY_DEPTH_MIN, min.ToString());
            pd.Metadata.Add(KEY_DEPTH_MAX, max.ToString());

            //hay que serializar la lista de shorts (que son los milimetros de profundidad desde el sensor)
            //y enviarlo como texto

            /*
            char[] depthsChar = new char[kinectData.DepthArray.Length];
            for(int i = 0; i < kinectData.DepthArray.Length; i++)
            {
                depthsChar[i] = Convert.ToChar(kinectData.DepthArray[i].Depth);
            }
            */


            char[] depthsChar = kinectData.DepthArray.Select(pixel => pixel.Depth > max ? Convert.ToChar(max) : (pixel.Depth < min ? Convert.ToChar(min) : Convert.ToChar(pixel.Depth))).ToArray();
            string depthsArray = new string(depthsChar);
            /*

            //how to decode
            short[] depths = depthsArray.Select(character => Convert.ToInt16(character)).ToArray();

            //how to recode
            char[] depthsChar2 = depths.Select(pixel => Convert.ToChar(pixel)).ToArray();
            string depthsArray2 = new string(depthsChar2);


            bool igual = depthsArray == depthsArray2;
            */

            string b64 = Base64Encode(depthsArray);

            //lo añadimos a los datos que se enviaran
            pd.Metadata.Add(KEY_DEPTH_ARRAY, b64);


            return pd;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
    }
}
