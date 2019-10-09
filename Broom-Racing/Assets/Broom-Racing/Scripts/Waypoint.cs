using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroomRacing
{
    public class Waypoint : MonoBehaviour
    {
        public static Waypoint instance;
        //Used for paths
        protected int lastPoint { get { return followWaypoints.Count - 1; } }
        protected Vector2 nextPoint { get { return followWaypoints.Count > 0 ? followWaypoints[0].position : Vector2.zero; } }
        protected Vector2 finalPoint { get { return followWaypoints.Count > 0 ? followWaypoints[lastPoint].position : Vector2.zero; } }

        public class FollowData
        {
            public Vector2 position = Vector2.zero;
            public bool leap = false;

            public FollowData(Vector2 newPosition, bool leaping = false) { position = newPosition; leap = leaping; }
        }
        public List<FollowData> followWaypoints = new List<FollowData>();


        private void Awake()
        {
            instance = this;
        }
        // Start is called before the first frame update
        private void Start()
        {
            MoveToPoint(followWaypoints[0].position);
        }

        // Update is called once per frame
        void Update()
        {

        }

        public virtual void MoveToNearestPoint(List<Vector2> points)
        {
            int nearestIndex = 0;
            float nearestDistance = Mathf.Infinity;
            for (int i = 0; i < points.Count; i++)
            {
                if (Vector2.Distance(transform.position, points[i]) < nearestDistance)
                {
                    nearestDistance = Vector2.Distance(transform.position, points[i]);
                    nearestIndex = i;
                }
            }

            MoveToPoint(points[nearestIndex]);
        }

        public virtual void MoveToPoint(Vector2 point)
        {
            List<Vector2> destination = new List<Vector2>();
            destination.Add(point);
            FollowPath(destination, 0);
        }

        public virtual void FollowPath(List<Vector2> pathNodes, bool snap = false, bool resumeFollow = false, bool run = false)
        {
            float totalDistance = 0;
            if (pathNodes.Count > 1)
            {
                for (int i = 0; i < pathNodes.Count; i++)
                {
                    totalDistance += Vector2.Distance(pathNodes[i], pathNodes[i + 1]);
                }
            }
            FollowPath(pathNodes, totalDistance, snap, resumeFollow, run);
        }

        public virtual void FollowPath(List<Vector2> pathNodes, float distance, bool snap = false, bool resumeFollow = false, bool run = false)
        {
        }
    }
}