using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinectExercise
{
    public class SceneDirector : MonoBehaviour
    {
        public GoalManager goalManager;
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
            goalManager.GenerateGoalsAt(goalUVList, 2.0f);
        }
    }
}