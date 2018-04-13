using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

namespace KinectExercise
{
    public class GoalManager : MonoBehaviour
    {
        public Camera MainCamera;
        public float GoalRadius = 1.0f;
        public HashSet<GameObject> goalGOs;
        private Dictionary<JointType, GameObject> jointTypeToGoal;

        public void GenerateGoalsAt(List<Vector2> cameraUVPoints, float depth)
        {
            foreach(Vector2 uvPoint in cameraUVPoints)
            {
                Vector2 screenPoint = CameraUVPointToScreenSpace(uvPoint, MainCamera);
                Debug.Log("Generating point for screen point " + screenPoint);
                Vector3 pos = new Vector3(screenPoint.x, screenPoint.y, depth);
                pos = MainCamera.ScreenToWorldPoint(pos);

                Goal.GenerateGoal(pos, GoalRadius);
            }
        }

        public void GenerateGoalsAt(List<Vector2> cameraUVPoints, List<float> depths)
        {
            if (cameraUVPoints != null
                && depths != null)
            {
                if (depths.Count != cameraUVPoints.Count)
                {
                    if (depths.Count > 0)
                    {
                        GenerateGoalsAt(cameraUVPoints, depths[0]);
                        Debug.LogError("Depths list does not have the same size as cameraUVPoints list.");
                    }
                }
            }
            else
            {
                Debug.LogError("Depths list or cameraUVPointsList are null");
            }
        }

        private static Vector2 CameraUVPointToScreenSpace(Vector2 cameraUVPoint, Camera cam)
        {
            return new Vector2(cameraUVPoint.x * cam.pixelWidth, cameraUVPoint.y * cam.pixelHeight);
        }

        public void SetGoals(Dictionary<JointType, GameObject> jointTypeToGoalDictionary)
        {
            if (jointTypeToGoalDictionary != null)
            {
                ClearGoals();
                jointTypeToGoal = jointTypeToGoalDictionary;
                foreach(var go in jointTypeToGoalDictionary.Values)
                {
                    goalGOs.Add(go);
                }
            }
        }

        public void ClearGoals()
        {
            goalGOs.Clear();
            goalGOs = new HashSet<GameObject>();
            jointTypeToGoal.Clear();
            jointTypeToGoal = new Dictionary<JointType, GameObject>();
        }

        public GameObject GetGoal(JointType jointType)
        {
            if (jointTypeToGoal.ContainsKey(jointType))
            {
                return jointTypeToGoal[jointType];
            }
            else
            {
                return null;
            }
        }

        public bool GoalMet(Windows.Kinect.Joint joint)
        {
            return GoalMet(jointTypeToGoal[joint.JointType], joint);
        }
        
        public bool GoalMet(GameObject goalGO, Windows.Kinect.Joint joint)
        {
            bool goalMet = false;

            if(goalGO != null)
            {
                GoalDebug goal = goalGO.GetComponent<GoalDebug>();
                if(goal != null)
                {
                    goalMet = goal.GoalMet(joint.Position);
                }
            }

            return goalMet;
        }

        // Use this for initialization
        void Start()
        {
            goalGOs = new HashSet<GameObject>();
            jointTypeToGoal = new Dictionary<JointType, GameObject>();
        }

        public bool AllGoalsMet(List<Windows.Kinect.Joint> jointList)
        {
            List<JointType> keys = new List<JointType>(jointTypeToGoal.Keys);

            bool allGoalsMetForJointList = true;
            for(int i = 0; i < jointList.Count; i++)
            {
                if (jointTypeToGoal.ContainsKey(jointList[i].JointType))
                {
                    keys.Remove(jointList[i].JointType);

                    if (!GoalMet(jointList[i]))
                    {
                        allGoalsMetForJointList = false;
                        break;
                    }
                }
            }

            bool allKeysAccountedFor = keys.Count == 0;

            return allKeysAccountedFor && allGoalsMetForJointList;
        }

        public int NumGoals
        {
            get
            {
                return goalGOs.Count;
            }
        }

        public List<JointType> JointsOfInterest
        {
            get
            {
                return new List<JointType>(jointTypeToGoal.Keys);
            }
        }
    }
}