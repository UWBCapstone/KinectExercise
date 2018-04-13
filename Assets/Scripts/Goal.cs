using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KinectExercise
{
    public static class Goal
    {
        public static GameObject GenerateGoal()
        {
            return GenerateGoal(Vector3.zero, 1.0f);
        }

        public static GameObject GenerateGoal(Vector3 center, float radius)
        {
            int numCirclePoints = 16;
            return GenerateGoal(center, radius, numCirclePoints);
        }

        public static GameObject GenerateGoal(Vector3 center, float radius, int numCirclePoints)
        {
            GameObject goalGO = new GameObject();
            goalGO.SetActive(false);
            goalGO.transform.position = center;
            
            // Add a LineRenderer to display the goal
            LineRenderer lr = goalGO.AddComponent<LineRenderer>();
            SetLineRendererSettings(lr);
            // Add a debugScript that lets you see into the goal's settings
            // and that also managers setting the LineRenderer
            GoalDebug debugScript = goalGO.AddComponent<GoalDebug>();
            // Add points for the circle
            List<Vector3> lrPositions = CalculateCirclePoints(center, radius, numCirclePoints);
            debugScript.SetPositions(lrPositions);

            goalGO.SetActive(true);

            return goalGO;
        }

        private static void SetLineRendererSettings(LineRenderer lr)
        {
            lr.useWorldSpace = true;
            lr.startWidth = .1f;
            lr.endWidth = .1f;
            lr.loop = true;
        }

        public static List<Vector3> CalculateCirclePoints(Vector3 center, float radius)
        {
            int numPoints = 16;
            return CalculateCirclePoints(center, radius, numPoints);
        }

        public static List<Vector3> CalculateCirclePoints(Vector3 center, float radius, int numPoints)
        {
            List<Vector3> positions = new List<Vector3>();

            List<Vector3> topPassPositions = new List<Vector3>();
            List<Vector3> lastPassPositions = new List<Vector3>();

            if(numPoints <= 0)
            {
                return positions;
            }

            // Assumes it'll be on the x/y plane
            int degreesPerPoint = 360;
            
            int numDivisions = 0;
            while(numPoints >> numDivisions > 1)
            {
                degreesPerPoint = degreesPerPoint >> 1;
                numDivisions++;
            }

            //Debug.Log("NumDivisions = " + numDivisions);
            //Debug.Log("DegreesPerPoint after binary split = " + degreesPerPoint);

            // Handle points where you can easily divide numPoints by 2;
            for(int i = 0; i < (1 << numDivisions); i++)
            {
                int degreeForPoint = degreesPerPoint * (i);
                Vector3 point = CalculateCirclePoint(center, radius, degreeForPoint);
                topPassPositions.Add(point);
            }

            // Handle points where you can't easily divide numPoints by 2;
            for(int i = 0; i < (numPoints >> numDivisions); i++)
            {
                degreesPerPoint /= 2;
                int degreeForPoint = degreesPerPoint * (i + 1);
                Vector3 point = CalculateCirclePoint(center, radius, degreesPerPoint);
                lastPassPositions.Add(point);
            }
            
            // Merge lists
            for(int i = 0; i < topPassPositions.Count; i++)
            {
                positions.Add(topPassPositions[i]);
                if(i < lastPassPositions.Count)
                {
                    positions.Add(lastPassPositions[i]);
                }
            }
            
            return positions;
        }

        private static Vector3 CalculateCirclePoint(Vector3 center, float radius, int degrees)
        {
            // Starts at top and rotates counter-clockwise

            //Debug.Log("Calculating circle point...");
            //Debug.Log("Center = " + center + "; radius = " + radius + "; degrees = " + degrees);

            Vector3 adj = Vector3.up;
            Vector3 opp = (adj * Mathf.Tan(Mathf.Deg2Rad * degrees)).magnitude * Vector3.left;
            //Debug.Log("Tan = " + Mathf.Tan(Mathf.Deg2Rad * degrees));
            if(degrees > 180)
            {
                // Flip the x value once you get past 180 degrees
                opp.x = -opp.x;

                //Debug.Log("Flipping opp.x");
            }
            if(degrees > 90
                && degrees < 270)
            {
                // Flip the y value once you get to the bottom half
                adj.y = -adj.y;
            }
            //Debug.Log("Opposite = " + opp);
            Vector3 pointUnNormalized = center + adj + opp;
            Vector3 normalizedDir = (pointUnNormalized - center).normalized * radius;
            //Debug.Log("Normalized Direction = " + normalizedDir);
            Vector3 point = center + normalizedDir;
            
            return point;
        }

        private static Material GenerateGoalMaterial()
        {
            Material goalMat = new Material(Shader.Find("Custom/GoalShader"));
            goalMat.name = "GoalMaterial";

            return goalMat;
        }


    }
}