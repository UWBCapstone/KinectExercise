﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Windows.Kinect;

namespace KinectExercise
{
    public class GoalManager : MonoBehaviour
    {
        public Camera MainCamera;
        public float GoalRadius = 1.0f;
        public HashSet<GameObject> goalGOs = new HashSet<GameObject>();
        private Dictionary<JointType, GameObject> jointTypeToGoal = new Dictionary<JointType, GameObject>();
        public BodyManager bodyManager;

        public List<GameObject> GenerateGoalsAt(List<Vector2> cameraUVPoints, float depth)
        {
            List<GameObject> goalGOs = new List<GameObject>();
            foreach(Vector2 uvPoint in cameraUVPoints)
            {
                Vector2 screenPoint = CameraUVPointToScreenSpace(uvPoint, MainCamera);
                Debug.Log("Generating point for screen point " + screenPoint);
                Vector3 pos = new Vector3(screenPoint.x, screenPoint.y, depth);
                pos = MainCamera.ScreenToWorldPoint(pos);
                Debug.Log("Screen point translated to " + pos);

                GameObject goal = Goal.GenerateGoal(pos, GoalRadius);
                goal.transform.parent = gameObject.transform;
                goalGOs.Add(goal);
            }

            return goalGOs;
        }

        public List<GameObject> GenerateGoalsAt(List<Vector2> cameraUVPoints, List<float> depths)
        {
            if (cameraUVPoints != null
                && depths != null)
            {
                if (depths.Count != cameraUVPoints.Count)
                {
                    if (depths.Count > 0)
                    {
                        Debug.LogError("Depths list does not have the same size as cameraUVPoints list.");
                        return GenerateGoalsAt(cameraUVPoints, depths[0]);
                    }
                }
            }
            else
            {
                Debug.LogError("Depths list or cameraUVPointsList are null");
            }

            return null;
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

        public void SetGoals(List<JointType> jointList, List<GameObject> goList)
        {
            Dictionary<JointType, GameObject> jointTypeToGoalDictionary = new Dictionary<JointType, GameObject>();
            for (int i = 0; i < jointList.Count; i++)
            {
                Debug.Log("Set Goals: i = " + i);
                if (goList[i] != null)
                {
                    Debug.Log("jointList i = " + jointList[i]);
                    Debug.Log("goList i = " + goList[i]);

                    jointTypeToGoalDictionary.Add(jointList[i], goList[i]);
                }
            }

            SetGoals(jointTypeToGoalDictionary);
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
            if (jointTypeToGoal.ContainsKey(joint.JointType))
            {
                return GoalMet(jointTypeToGoal[joint.JointType], bodyManager.GetJointObject(joint.JointType));
            }
            else
            {
                return false;
            }
        }
        
        public bool GoalMet(GameObject goalGO, GameObject jointObj)
        {
            bool goalMet = false;

            if(goalGO != null)
            {
                GoalDebug goal = goalGO.GetComponent<GoalDebug>();
                if(goal != null)
                {
                    //goalMet = goal.GoalMet(joint.Position);
                    goalMet = goal.GoalMet(jointObj.transform.position);
                }
            }

            return goalMet;
        }

        // Use this for initialization
        void Start()
        {
            goalGOs = new HashSet<GameObject>();
            if (jointTypeToGoal == null)
            {
                jointTypeToGoal = new Dictionary<JointType, GameObject>();
            }
        }

        public bool AllGoalsMet(List<Windows.Kinect.Joint> jointList)
        {
            List<JointType> keys = new List<JointType>(jointTypeToGoal.Keys);
            if(keys.Count == 0)
            {
                return false;
            }

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