using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Text;

namespace KinectExercise
{
    public class GoalDebug : MonoBehaviour
    {
        public List<Vector3> LineRenderPositions;
        public Color color = Color.red;
        public float radius = 1.0f;
        private Vector3 lastPosition;

        // Use this for initialization
        void Start()
        {
            LineRenderPositions = new List<Vector3>();
            lastPosition = gameObject.transform.position;
        }

        public void Update()
        {
            if (gameObject.transform.position != lastPosition)
            {
                lastPosition = gameObject.transform.position;
                SetPositions(Goal.CalculateCirclePoints(gameObject.transform.position, radius));
            }
        }

        public void SetPositions(List<Vector3> positions)
        {
            if (positions == null)
            {
                LineRenderPositions = new List<Vector3>();
            }
            else
            {
                LineRenderPositions = new List<Vector3>(positions);
            }
            UpdateLineRenderer();
        }

        public void AddPosition(Vector3 position)
        {
            LineRenderPositions.Add(position);
        }

        public void RemovePosition(int index)
        {
            if (index < NumPositions)
            {
                LineRenderPositions.Remove(LineRenderPositions[index]);
            }
        }

        public void RemovePosition(Vector3 position)
        {
            LineRenderPositions.Remove(position);
        }

        public int NumPositions
        {
            get
            {
                return LineRenderPositions.Count;
            }
        }

        public bool GoalMet(Vector3 position)
        {
            //// if the position passed in is within the radius of the lastPosition recorded, the goal is met
            //// Ignore alignment on the Z axis
            //Vector2 pos = new Vector2(position.x, position.y);
            //Vector2 cent = new Vector2(lastPosition.x, lastPosition.y);

            Vector3 posViewPort = Camera.main.WorldToViewportPoint(position);
            Vector3 centViewPort = Camera.main.WorldToViewportPoint(lastPosition);

            Vector2 pos = new Vector2(posViewPort.x, posViewPort.y);
            Vector2 cent = new Vector2(centViewPort.x, centViewPort.y);

            Debug.Log("Pos = " + pos);
            Debug.Log("Cent = " + cent);

            float magnitude = (cent - pos).magnitude;

            Vector3 comparativeCirclePos = lastPosition + Vector3.left * radius;
            Vector3 comparativeCirclePosViewPort = Camera.main.WorldToViewportPoint(comparativeCirclePos);
            Vector2 comp = new Vector2(comparativeCirclePosViewPort.x, comparativeCirclePosViewPort.y);

            float comparisonMagnitude = (cent - comp).magnitude;

            Debug.Log("Magnitude = " + magnitude + "; Radius = " + comparisonMagnitude);

            //return magnitude <= radius;
            return magnitude <= comparisonMagnitude;
        }

        public bool GoalMet(Windows.Kinect.CameraSpacePoint point)
        {
            Vector2 pos = new Vector2(point.X, point.Y);
            Vector2 cent = new Vector2(lastPosition.x, lastPosition.y);

            float magnitude = (cent - pos).magnitude;

            return magnitude <= radius;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            string goalNameStr = gameObject.name + " (name);";
            string numPositionsStr = "# of Points = " + NumPositions.ToString();

            sb.AppendLine(goalNameStr);
            sb.AppendLine(numPositionsStr);

            for (int i = 0; i < LineRenderPositions.Count; i++)
            {
                string posStr = "\tPosition[" + i + "] = " + LineRenderPositions[i].ToString();
                sb.AppendLine(posStr);
            }

            return sb.ToString();
        }

        public void UpdateLineRenderer()
        {
            var lr = gameObject.GetComponent<LineRenderer>();
            if (lr != null)
            {
                if (LineRenderPositions != null)
                {
                    lr.positionCount = LineRenderPositions.Count;
                    //lr.SetPositions(LineRenderPositions.ToArray());
                    for(int i = 0; i < LineRenderPositions.Count; i++)
                    {
                        lr.SetPosition(i, LineRenderPositions[i]);
                        //Debug.Log("Adding pos " + LineRenderPositions[i] + " to linerenderer");
                    }
                }
                if (color != null)
                {
                    lr.startColor = color;
                    lr.endColor = color;
                }
            }
        }
    }
}