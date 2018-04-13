using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

namespace KinectExercise
{
    public class BodyManager : MonoBehaviour
    {
        public KinectSensor sensor;
        public BodyFrameReader reader;
        //public KinectManager sourceManager;
        public Body[] FrameBodyData;

        //public BodyFrame bodyFrame;
        //public BodyFrameReader reader; // provides access to individual bodies through GetAndRefreshBodyData
        //public BodyFrameSource source; // bodycount (input vector for GetAndRefreshBodyData)

        private Dictionary<ulong, GameObject> _Bodies = new Dictionary<ulong, GameObject>();
        
        private Dictionary<JointType, JointType> _BoneMap = new Dictionary<JointType, JointType>()
    {
        { JointType.FootLeft, JointType.AnkleLeft },
        { JointType.AnkleLeft, JointType.KneeLeft },
        { JointType.KneeLeft, JointType.HipLeft },
        { JointType.HipLeft, JointType.SpineBase },

        { JointType.FootRight, JointType.AnkleRight },
        { JointType.AnkleRight, JointType.KneeRight },
        { JointType.KneeRight, JointType.HipRight },
        { JointType.HipRight, JointType.SpineBase },

        { JointType.HandTipLeft, JointType.HandLeft },
        { JointType.ThumbLeft, JointType.HandLeft },
        { JointType.HandLeft, JointType.WristLeft },
        { JointType.WristLeft, JointType.ElbowLeft },
        { JointType.ElbowLeft, JointType.ShoulderLeft },
        { JointType.ShoulderLeft, JointType.SpineShoulder },

        { JointType.HandTipRight, JointType.HandRight },
        { JointType.ThumbRight, JointType.HandRight },
        { JointType.HandRight, JointType.WristRight },
        { JointType.WristRight, JointType.ElbowRight },
        { JointType.ElbowRight, JointType.ShoulderRight },
        { JointType.ShoulderRight, JointType.SpineShoulder },

        { JointType.SpineBase, JointType.SpineMid },
        { JointType.SpineMid, JointType.SpineShoulder },
        { JointType.SpineShoulder, JointType.Neck },
        { JointType.Neck, JointType.Head },
    };


        // Use this for initialization
        void Awake()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateFrameBodyData();
            List<ulong> newBodyIDs = UpdateBodyDictionary();
            RefreshBodies(newBodyIDs);
        }
        
        public void Init()
        {
            sensor = KinectSensor.GetDefault();
            reader = sensor.BodyFrameSource.OpenReader();

            if (!sensor.IsOpen)
            {
                sensor.Open();
            }

            Debug.Log("Trying to get sensor stuff for bodyManager");
            FrameBodyData = new Body[sensor.BodyFrameSource.BodyCount];
        }

        public void UpdateFrameBodyData()
        {
            BodyFrame frame = reader.AcquireLatestFrame();
            if (frame != null)
            {
                frame.GetAndRefreshBodyData(FrameBodyData);

                frame.Dispose();
                frame = null;
            }
        }

        /// <summary>
        /// Cull offscreen body tracking for the body dictionary.
        /// </summary>
        public List<ulong> UpdateBodyDictionary()
        {
            List<ulong> newBodyIDs = new List<ulong>();

            List<ulong> onScreenIDs = GetOnScreenBodyIDs();
            CullOffscreenBodies(onScreenIDs);

            // Figure out which body IDs are new
            foreach(var onScreenID in onScreenIDs)
            {
                if (!_Bodies.ContainsKey(onScreenID))
                {
                    newBodyIDs.Add(onScreenID);
                }
            }

            return newBodyIDs;
        }

        public List<ulong> GetOnScreenBodyIDs()
        {
            List<ulong> onScreenIDs = new List<ulong>();
            foreach (Body body in FrameBodyData)
            {
                if (body == null)
                {
                    continue;
                }

                if (body.IsTracked)
                {
                    onScreenIDs.Add(body.TrackingId);
                }
            }

            return onScreenIDs;
        }

        public void CullOffscreenBodies(List<ulong> onScreenIDs)
        {
            List<ulong> knownIDs = new List<ulong>(_Bodies.Keys);

            // Cull the 
            foreach (ulong knownID in knownIDs)
            {
                if (!onScreenIDs.Contains(knownID))
                {
                    ulong offScreenID = knownID;
                    Destroy(_Bodies[offScreenID]);
                    _Bodies.Remove(offScreenID);
                }
            }
        }

        public void RefreshBodies(List<ulong> newBodyIDs)
        {
            // Refresh a body if it exists, or create a new one if needed
            foreach(ulong bodyID in _Bodies.Keys)
            {
                if (newBodyIDs.Contains(bodyID))
                {
                    // Create a new body character
                }
                else
                {
                    // Refresh an existing body character
                }
            }
        }

        public void CreateBody()
        {

        }

        public void RefreshBodyObject(Body body, GameObject bodyObject)
        {
            for(JointType jt = JointType.SpineBase; jt < JointType.ThumbRight; jt++)
            {
                Windows.Kinect.Joint sourceJoint = body.Joints[jt];
                Windows.Kinect.Joint? targetJoint = null;

                if (_BoneMap.ContainsKey(jt))
                {
                    targetJoint = body.Joints[_BoneMap[jt]];
                }

                Transform jointObj = bodyObject.transform.Find(jt.ToString());
                jointObj.localPosition = GetVector3FromJoint(sourceJoint);
                
                // Manipulate display of the body
                

                //LineRenderer lr = jointObj.GetComponent<LineRenderer>();
                //if (targetJoint.HasValue)
                //{
                //    lr.SetPosition(0, jointObj.localPosition);
                //    lr.SetPosition(1, GetVector3FromJoint(targetJoint.Value));
                //    lr.SetColors(GetColorForState(sourceJoint.TrackingState), GetColorForState(targetJoint.Value.TrackingState));
                //}
                //else
                //{
                //    lr.enabled = false;
                //}
            }
        }

        public static Color GetStateColor(TrackingState state)
        {
            switch (state)
            {
                case TrackingState.Tracked:
                    return Color.green;
                case TrackingState.Inferred:
                    return Color.red;
                default:
                    return Color.black;
            }
        }

        private static Vector3 GetVector3FromJoint(Windows.Kinect.Joint joint)
        {
            return new Vector3(joint.Position.X * 10, joint.Position.Y * 10, joint.Position.Z * 10);
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