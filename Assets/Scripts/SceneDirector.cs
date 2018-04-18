using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinectExercise
{
    public class SceneDirector : MonoBehaviour
    {
        public GoalManager goalManager;
        public BodyManager bodyManager;
        public float GoalDepth = 2.0f;
        public Vector2[] goalUVs = new Vector2[8];
        public Windows.Kinect.JointType[] goalJoints = new Windows.Kinect.JointType[8];

        private List<GameObject> generatedGoals = new List<GameObject>();
        
        public List<GameObject> GenerateGoals()
        {
            List<GameObject> goalGOs = new List<GameObject>();

            List<Vector2> goalUVList = new List<Vector2>();
            for(int i = 0; i < goalUVs.Length; i++)
            {
                if (!goalUVs[i].Equals(Vector2.zero))
                {
                    goalUVList.Add(goalUVs[i]);
                }
            }
            goalGOs = goalManager.GenerateGoalsAt(goalUVList, GoalDepth);

            return goalGOs;
        }

        public void Awake()
        {
            generatedGoals = GenerateGoals();
            goalManager.SetGoals(GetValidGoalJointsList(), generatedGoals);
            
            bodyManager.SetVisibleJoints(GetValidGoalJointsList());
        }

        public void Update()
        {
            DebugGoals();
        }

        public void DebugGoals()
        {
            //BodyManager bodyManager = new BodyManager();
            //foreach (var body in bodyManager.BodyMap.Values)
            //{
            //    body.transform.GetChild()
            //}

            List<Windows.Kinect.JointType> goalJointTypesList = GetValidGoalJointsList();
            List<Windows.Kinect.Joint> goalJointsList = new List<Windows.Kinect.Joint>();

            for(int i = 0; i < goalJointTypesList.Count; i++)
            {
                Windows.Kinect.JointType jt = goalJointTypesList[i];
                Windows.Kinect.Joint j = bodyManager.GetJoint(jt);
                goalJointsList.Add(j);

                bool goalMetForJoint = goalManager.GoalMet(j);
                if (goalMetForJoint)
                {
                    Debug.Log("Joint[" + jt.ToString() + "] Goal Met!");
                }
            }

            if (goalManager.AllGoalsMet(goalJointsList))
            {
                Debug.Log("SUCCESS: All joint goals met!");
            }
        }

        public int GetNumValidGoals()
        {
            int numValidGoals = 0;
            for(int i = 0; i < goalUVs.Length; i++)
            {
                if (goalUVs[i].Equals(Vector2.zero))
                {
                    break;
                }
                else
                {
                    numValidGoals++;
                }
            }

            return numValidGoals;
        }

        public List<Windows.Kinect.JointType> GetValidGoalJointsList()
        {
            List<Windows.Kinect.JointType> validGoalJointsList = new List<Windows.Kinect.JointType>();
            int numValidJointTypes = GetNumValidGoals();

            for(int i = 0; i < numValidJointTypes; i++)
            {
                validGoalJointsList.Add(goalJoints[i]);
            }

            return validGoalJointsList;
        }
    }
}