using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

namespace KinectExercise
{
    public class ColorImageManager : MonoBehaviour
    {
        public KinectSensor sensor;
        public ColorFrameReader reader;
        //public KinectManager sourceManager;
        public byte[] Data;
        public Texture2D Texture;

        public void Awake()
        {
            //Data = new byte[sourceManager.kinectSensor.ColorFrameSource.FrameDescription.LengthInPixels];
            sensor = KinectSensor.GetDefault();
            reader = sensor.ColorFrameSource.OpenReader();

            //FrameDescription desc = sensor.ColorFrameSource.FrameDescription;
            FrameDescription desc = sensor.ColorFrameSource.CreateFrameDescription(ColorImageFormat.Rgba);
            Texture = new Texture2D(desc.Width, desc.Height, TextureFormat.RGBA32, false);
            Data = new byte[desc.BytesPerPixel * desc.LengthInPixels];

            if (!sensor.IsOpen)
            {
                sensor.Open();
            }
        }

        public void Update()
        {
            if (reader != null)
            {
                ColorFrame frame = reader.AcquireLatestFrame();

                if (frame != null)
                {
                    frame.CopyConvertedFrameDataToArray(Data, ColorImageFormat.Rgba);
                    Texture.LoadRawTextureData(Data);
                    Texture.Apply();

                    frame.Dispose();
                    frame = null;
                }
            }
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