using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BroomRacing
{
    public class LevelGenerator : MonoBehaviour
    {
        public Racer racerPrefab;
        public Obstacle obstaclePrefab;
        public Transform waypointPrefab;

        public Sprite[] obstacleSprites;
        private Image[] _images;

        private List<Obstacle> obstacles;

        public Texture2D obstacleSpriteTexture;

        public Transform[] waypointList;

        private void Awake()
        {
            obstacleSprites = Resources.LoadAll<Sprite>(obstacleSpriteTexture.name);
            obstacles = new List<Obstacle>();
            GenerateLevelLayout();
            RaceController.instance.SetRacePath(waypointList);

        }

        /*
        void Start()
        {
            _images = gameObject.GetComponentsInChildren<Image>();
            StartCoroutine(Count());
        }

        IEnumerator Count()
        {
            for (int i = 0; i < _images.Length; i++)
            {
                var spriteNumber = Random.Range(0, _myOtherSprites.Length - 1);
                _images[i].sprite = _myOtherSprites[spriteNumber];
            }
            yield return new WaitForSeconds(2);
            Application.LoadLevel(0);
        }*/

        void GenerateLevelLayout()
        {
            GenerateLevelPath();
        }

        void GenerateObstacles()
        {
            // Currently just dumps them all at 0,0,0 lol
            int numObstacles = Random.Range(10, 30);
            for (int i = 0; i < numObstacles; i++)
            {
                int numSprite = Random.Range(0, obstacleSprites.Length - 1);
                var obj = Instantiate(obstaclePrefab);
                obj.SetRandomObstacle(obstacleSprites[numSprite], GeneratePosition());
                obstacles.Add(obj);
                //
            }
        }

        Vector3 GeneratePosition()
        {
            // Based on Level layout, generate an obstacle position and return it

            return new Vector3(Random.Range(-10.0f,10.0f), Random.Range(-6.0f,6.0f));
        }

        void GenerateLevelPath()
        {
            // Create a list of empty game objects as waypoints
            int waypointNum = Random.Range(4, 20);
            waypointList = new Transform[waypointNum];

            waypointList[0] = Instantiate(waypointPrefab);
            waypointList[0].position = new Vector3(Random.Range(100f,200f),Random.Range(100f,200f));

            for (int i = 1; i < waypointNum; i++)
            {

                waypointList[i] = Instantiate(waypointPrefab,waypointList[i-1]);
                waypointList[i].position = Vector3.MoveTowards(waypointList[i].position, Vector3.zero, Random.Range(7f,20f));
                if( i%3 == 0)
                {
                    waypointList[i].position = Vector3.MoveTowards(waypointList[i].position, new Vector3(Random.Range(-300f, 300f), Random.Range(-50f, 50f)), Random.Range(7f, 20f));
                }
            }
            System.Array.Reverse(waypointList);

            //Generate2DLevelPath();
        }

        void Generate2DLevelPath()
        {
            foreach (var waypoint in waypointList)
            {
                Waypoint.instance.followWaypoints.Add(new Waypoint.FollowData(waypoint.position));
            }
        }

    }
}