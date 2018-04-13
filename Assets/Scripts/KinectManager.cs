using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

namespace KinectExercise
{
    public class KinectManager : MonoBehaviour
    {
        public KinectSensor kinectSensor;
        public BodyFrameReader bodyFrameReader;
        public ColorFrameReader colorFrameReader;
        public DepthFrameReader depthFrameReader;
        public InfraredFrameReader infraredFrameReader;
        //public MultiSourceFrameReader multiSourceFrameReader;

        // Use this for initialization
        void Start()
        {
            OpenSensorAndReaders();
            Debug.Log("Finished initializing KinectManager");
        }

        public void OpenSensorAndReaders()
        {
            // https://docs.microsoft.com/en-us/previous-versions/windows/kinect/dn799273%28v%3dieb.10%29
            kinectSensor = KinectSensor.GetDefault();
            if (kinectSensor != null)
            {
                bodyFrameReader = kinectSensor.BodyFrameSource.OpenReader();
                colorFrameReader = kinectSensor.ColorFrameSource.OpenReader();
                depthFrameReader = kinectSensor.DepthFrameSource.OpenReader();
                infraredFrameReader = kinectSensor.InfraredFrameSource.OpenReader();
                //multiSourceFrameReader = kinectSensor.OpenMultiSourceFrameReader(
                //    FrameSourceTypes.Color | 
                //    FrameSourceTypes.Depth | 
                //    FrameSourceTypes.Audio);

                if (!kinectSensor.IsOpen)
                {
                    kinectSensor.Open();
                }
            }
        }

        public void CloseSensorsAndReaders()
        {
            if(bodyFrameReader != null)
            {
                bodyFrameReader.Dispose();
                bodyFrameReader = null;
            }

            if(colorFrameReader != null)
            {
                colorFrameReader.Dispose();
                colorFrameReader = null;
            }

            if(depthFrameReader != null)
            {
                depthFrameReader.Dispose();
                depthFrameReader = null;
            }

            if(infraredFrameReader != null)
            {
                infraredFrameReader.Dispose();
                infraredFrameReader = null;
            }

            //if(multiSourceFrameReader != null)
            //{
            //    multiSourceFrameReader.Dispose();
            //    multiSourceFrameReader = null;
            //}

            if(kinectSensor != null)
            {
                if (kinectSensor.IsOpen)
                {
                    kinectSensor.Close();
                }

                kinectSensor = null;
            }
        }

        public void OnApplicationQuit()
        {
            CloseSensorsAndReaders();
        }
    }
}