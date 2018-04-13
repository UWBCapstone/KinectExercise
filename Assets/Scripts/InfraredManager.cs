using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

namespace KinectExercise
{
    public class InfraredManager : MonoBehaviour
    {
        public KinectSensor sensor;

        //public KinectManager sourceManager;
        public InfraredFrameReader reader;
        public ushort[] FrameData;
        public byte[] RawData;
        public Texture2D Texture;

        public void Awake()
        {
            Init();
        }

        public void Update()
        {
            if (reader != null)
            {
                InfraredFrame frame = reader.AcquireLatestFrame();
                if(frame != null)
                {
                    frame.CopyFrameDataToArray(FrameData);

                    int index = 0;
                    foreach(var ir in FrameData)
                    {
                        byte intensity = (byte)(ir >> 8);
                        for (int i = 0; i < 3; i++)
                        {
                            RawData[index++] = intensity;
                        }
                        RawData[index++] = 255; // Alpha
                    }

                    Texture.LoadRawTextureData(RawData);
                    Texture.Apply();

                    frame.Dispose();
                    frame = null;
                }
            }
        }

        public void Init()
        {
            sensor = KinectSensor.GetDefault();
            reader = sensor.InfraredFrameSource.OpenReader();
            if(reader != null)
            {
                FrameDescription desc = sensor.InfraredFrameSource.FrameDescription;
                FrameData = new ushort[desc.LengthInPixels];
                RawData = new byte[desc.LengthInPixels * 4];
                Texture = new Texture2D(desc.Width, desc.Height, TextureFormat.BGRA32, false);
            }

            if (!sensor.IsOpen)
            {
                sensor.Open();
            }
        }

        public InfraredFrameReader GetReader()
        {
            return reader;
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