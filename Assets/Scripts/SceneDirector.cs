using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinectExercise
{
    public class SceneDirector : MonoBehaviour
    {
        public GoalManager goalManager;
        public float GoalDepth = 2.0f;
        public Vector2[] goalUVs = new Vector2[8];

        
        public void GenerateGoals()
        {
            List<Vector2> goalUVList = new List<Vector2>();
            for(int i = 0; i < goalUVs.Length; i++)
            {
                if (!goalUVs[i].Equals(Vector2.zero))
                {
                    goalUVList.Add(goalUVs[i]);
                }
            }
            goalManager.GenerateGoalsAt(goalUVList, GoalDepth);
        }

        public void Awake()
        {
            GenerateGoals();
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

            GameObject WristRight = GameObject.Find("WristRight");

            GameObject goalManager = GameObject.Find("GoalManager");

            // Debug whether the goals are met
            for (int childIndex = 0; childIndex < goalManager.transform.childCount; childIndex++)
            {
                GameObject child = goalManager.transform.GetChild(childIndex).gameObject;
                GoalDebug g = child.GetComponent<GoalDebug>();
                if (g != null)
                {
                    if (WristRight != null)
                    {
                        if (g.GoalMet(WristRight.transform.position))
                        {
                            Debug.Log("Goal met!");
                        }
                    }
                }
                else
                {
                    Debug.Log("Child had no goalDebug script.");
                }
            }
        }
    }
}