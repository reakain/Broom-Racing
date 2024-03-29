﻿using PathCreation;
using Pathfinding;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace BroomRacing
{
    public class LevelGenerator : MonoBehaviour
    {
        public float randomScale = 0.1f;
        public Racer racerPrefab;
        public Obstacle obstaclePrefab;
        public Transform waypointPrefab;
        public SpriteRenderer background;

        public Sprite[] obstacleSprites;
        public Sprite[] backgroundSprites;

        private List<Obstacle> obstacles;
        private List<Racer> opponents;

        public Texture2D obstacleSpriteTexture;
        public Texture2D backgroundSpriteTexture;

        public Transform[] waypointList;
        public Vector2[] trackPath;

        PathCreator pathCreator;

        private void Awake()
        {
            obstacleSprites = Resources.LoadAll<Sprite>(obstacleSpriteTexture.name);
            obstacles = new List<Obstacle>();

            pathCreator = GetComponent<PathCreator>();

            //backgroundSprites = Resources.LoadAll<Sprite>(backgroundSpriteTexture.name);

            //GenerateLevelLayout();
            ////RaceController.instance.SetRacePath(waypointList);

            //if (waypointList.Length > 0)
            //{
            //    // Create a new bezier path from the waypoints.
            //    BezierPath bezierPath = new BezierPath(waypointList, true, PathSpace.xy);
            //    GetComponent<PathCreator>().bezierPath = bezierPath;
            //}
            //GetComponent<CourseMeshCreator>().GenerateCollider();
            
        }

        private void Start()
        {
            //GetComponent<Grid2D>().InstantiateGrid();
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

        public void CleanScene()
        {
            foreach (var obstacle in obstacles)
            {
                obstacle.gameObject.SetActive(false);
            }
            foreach (var opponent in opponents)
            {
                opponent.gameObject.SetActive(false);
            }
        }

        public void GenerateLevelLayout()
        {
            GenerateLevelPath();

            if (waypointList.Length > 0)
            {
                // Create a new bezier path from the waypoints.
                BezierPath bezierPath = new BezierPath(waypointList, true, PathSpace.xy);
                pathCreator.bezierPath = bezierPath;
                Debug.Log("Number of path points: " + pathCreator.path.NumPoints.ToString());
                for (int i = 10; i < pathCreator.path.NumPoints; i += 10)
                {
                    GenerateObstacles(pathCreator.path.GetPointAtDistance(i,EndOfPathInstruction.Loop));

                }
            }

            
            //GenerateObstacles();
        }


        // Load a background image for your level
        void GenerateBackground()
        {
            int numSprite = Random.Range(0, backgroundSprites.Length - 1);
            background.sprite = backgroundSprites[numSprite];
        }

        // Generate your objects with random sprites and positions from a prefab
        void GenerateObstacles(Vector3 position)
        {
            // Currently just dumps them all at 0,0,0 lol
            int numObstacles = Random.Range(1, 2);
            for (int i = 0; i < numObstacles; i++)
            {
                int numSprite = Random.Range(0, obstacleSprites.Length - 1);
                var obj = Instantiate(obstaclePrefab);
                obj.SetRandomObstacle(obstacleSprites[numSprite], GeneratePosition(position));
                obstacles.Add(obj);
                //
            }
        }

        Vector3 GeneratePosition(Vector3 position)
        {
            float xval = Random.Range(-3f,3f);
            float yval = Random.Range(-3f,3f);

            //// Based on Level layout, generate an obstacle position and return it
            //if (Random.Range(0, 1) == 0)
            //{
            //    xval = -xval;
            //}
            //if (Random.Range(0,1) == 0)
            //{
            //    yval = -yval;
            //}

            return new Vector3(xval + position.x, yval + position.y);
        }

        void GenerateLevelPath2()
        {
            // Create a list of empty game objects as waypoints
            int waypointNum = Random.Range(4, 20);
            waypointList = new Transform[waypointNum];

            waypointList[0] = Instantiate(waypointPrefab);
            var boundsize = background.bounds.extents;
            waypointList[0].position = new Vector3(boundsize.x - boundsize.x * 0.2f, boundsize.y - boundsize.y * 0.2f);
            Vector3 farpoint = new Vector3(-boundsize.x, -boundsize.y);
            float borderx = waypointList[0].position.x;
            float bordery = waypointList[0].position.y;
            //border = border - border * 0.3f;
            //Debug.Log("Border point is: " + border.ToString());
            //waypointList[0].position = new Vector3(Random.Range(border*0.7f,border),Random.Range(border*0.7f, border));
            //Vector3 farpoint = new Vector3(-border, -border);

            for (int i = 1; i < waypointNum; i++)
            {

                waypointList[i] = Instantiate(waypointPrefab,waypointList[i-1]);
                waypointList[i].position = Vector3.MoveTowards(waypointList[i].position, farpoint, Random.Range(7f* randomScale, 20f* randomScale));
                if( i%3 == 0)
                {
                    waypointList[i].position = Vector3.MoveTowards(waypointList[i].position, new Vector3(Random.Range(-borderx, borderx), Random.Range(-bordery, bordery)), Random.Range(7f* randomScale, 20f* randomScale));
                }
                GenerateObstacles(waypointList[i].position);
            }
            System.Array.Reverse(waypointList);

            
        }

        void GenerateLevelPath()
        {
            int waypointNum = Random.Range(4, 10);

            trackPath = ProceduralComplexLoop.GeneratePoints(50,50,waypointNum, 10f, 1f, 10f);// background.bounds.extents.x * randomScale*0.5f, background.bounds.extents.y * randomScale*0.5f, waypointNum);

            waypointList = new Transform[trackPath.Length];
            for(int i = 0; i < trackPath.Length; i ++)
            {
                waypointList[i] = Instantiate(waypointPrefab);
                waypointList[i].position = new Vector3(trackPath[i].x, trackPath[i].y);
                //GenerateObstacles(waypointList[i].position);
            }
        }

    }
}