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

        public Sprite[] _myOtherSprites;
        private Image[] _images;

        private List<Obstacle> obstacles;

        private void Awake()
        {
            obstacles = new List<Obstacle>();
            GenerateObstacles();
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
            
        }

        void GenerateObstacles()
        {
            // Currently just dumps them all at 0,0,0 lol
            int numObstacles = Random.Range(10, 30);
            for (int i = 0; i < numObstacles; i++)
            {
                //int numSprite = Random.Range(0, _myOtherSprites.Length - 1);
                var obj = Instantiate(obstaclePrefab);
                // obj.SetRandomObstacle(_myOtherSprites[numSprite], GeneratePosition());
                obstacles.Add(obj);
                //
            }
        }

        Vector3 GeneratePosition()
        {
            // Based on Level layout, generate an obstacle position and return it
            return Vector3.zero;
        }


    }
}