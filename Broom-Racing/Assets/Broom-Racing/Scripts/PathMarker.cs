using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroomRacing
{
    public enum MarkerSide
    {
        Left,
        Right
    }

    public class PathMarker : MonoBehaviour
    {
        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        public float speed = 5;
        public float courseWidth = 3f;
        public MarkerSide pathSide = MarkerSide.Left;
        float distanceTravelled;

        void Start()
        {
            speed = RaceController.instance.raceSpeed;
            if (pathCreator != null)
            {
                // Subscribed to the pathUpdated event so that we're notified if the path changes during the game
                pathCreator.pathUpdated += OnPathChanged;
            }
        }

        void Update()
        {
            if (pathCreator != null && !RaceController.instance.raceOver)
            {
                distanceTravelled += speed * Time.deltaTime;
                Vector2 localRight = Vector2.Perpendicular(pathCreator.path.GetDirectionAtDistance(distanceTravelled, endOfPathInstruction).normalized);
                Vector2 newPos = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                newPos += (localRight * Mathf.Abs(courseWidth) * ((pathSide == MarkerSide.Left) ? 1 : -1));
                transform.position = newPos;
                transform.rotation = pathCreator.path.GetRotationAtDistance(distanceTravelled, endOfPathInstruction);
            }
        }

        // If the path changes during the game, update the distance travelled so that the follower's position on the new path
        // is as close as possible to its position on the old path
        void OnPathChanged()
        {
            distanceTravelled = pathCreator.path.GetClosestDistanceAlongPath(transform.position);
        }
    }
}