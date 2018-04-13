using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

namespace KinectExercise
{
    public class DepthManager : MonoBehaviour
    {
        public KinectSensor sensor;
        public DepthFrameReader reader;

        //public KinectManager sourceManager;
        public ushort[] Data;
        public Texture2D Texture;

        public void Awake()
        {
            sensor = KinectSensor.GetDefault();
            reader = sensor.DepthFrameSource.OpenReader();
            FrameDescription desc = sensor.DepthFrameSource.FrameDescription;
            Data = new ushort[desc.LengthInPixels];
            Texture = new Texture2D(desc.Width, desc.Height);

            if (!sensor.IsOpen)
            {
                sensor.Open();
            }
        }

        public void Update()
        {
            DepthFrame frame = reader.AcquireLatestFrame();
                
            if(frame != null)
            {
                frame.CopyFrameDataToArray(Data);
                frame.Dispose();
                frame = null;
            }
        }

        public void FixedUpdate()
        {
            RefreshTexture();
        }

        public void RefreshTexture()
        {
            Color[] colors = new Color[Data.Length];
            for (int i = 0; i < Data.Length; i++)
            {
                colors[i] = new Color(Data[i], Data[i], Data[i], 1);
            }
            Texture.SetPixels(colors);
            Texture.Apply();
        }
        
        public void CloseSensorAndReader()
        {
            if (reader != null)
            {
                reader.Dispose();
                reader = null;
            }

            if (sensor != null)
            {
                if (sensor.IsOpen)
                {
                    sensor.Close();
                }

                sensor = null;
            }
        }

        public void OnApplicationQuit()
        {
            CloseSensorAndReader();
        }
    }
}