using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroomRacing
{
    public class RaceController : MonoBehaviour
    {
        public static RaceController instance;

        public bool raceOver = true;

        public Racer player;
        public Racer[] opponents;

        public Transform[] racePath;

        public int currentWayPoint = 0;
        Transform targetWayPoint;

        public float raceSpeed = 4f;
        public float raceStartTime = 0.0f;
        public float raceTime = 0.0f;

        public TMPro.TextMeshProUGUI raceTimeLabel;

        private void Awake()
        {
            instance = this;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
            if(!raceOver)
            {
                raceTime = Time.time - raceStartTime;
                if (currentWayPoint < this.racePath.Length-1)
                {
                    if (targetWayPoint == null)
                        targetWayPoint = racePath[currentWayPoint];
                    MoveAlongTrack();
                }
                else
                {
                    EndRace();
                }
            }
            if(Input.GetButtonDown(InputContainer.instance.startRaceButton))
            {
                StartRace();
            }
            if(Input.GetButtonDown(InputContainer.instance.endRaceButton))
            {
                EndRace();
            }
        }

        private void FixedUpdate()
        {
            if(raceTimeLabel != null)
            {
                raceTimeLabel.text = System.TimeSpan.FromSeconds(raceTime).ToString("g");
            }
        }

        void MoveAlongTrack()
        {
            // rotate towards the target
            
            var rotateUnClamp = Vector3.RotateTowards(transform.up, targetWayPoint.position - transform.position, raceSpeed/3f * Time.deltaTime, 0.0f);
            transform.up = new Vector3(rotateUnClamp.x,rotateUnClamp.y);

            // move towards the target
            transform.position = Vector3.MoveTowards(transform.position, targetWayPoint.position, raceSpeed * Time.deltaTime);

            if (transform.position == targetWayPoint.position)
            {
                currentWayPoint++;
                targetWayPoint = racePath[currentWayPoint];
            }
        }

        public void StartRace()
        {
            if(racePath != null && player != null)
            {
                raceStartTime = Time.time;
                raceTime = 0.0f;
                this.gameObject.transform.position = racePath[0].position;
                player.gameObject.SetActive(true);
                player.SetStartPosition();
                foreach(var opponent in opponents)
                {
                    opponent.SetStartPosition();
                }
                raceOver = false;
            }
        }

        public void SetRacePath(Transform[] racepath)
        {
            racePath = racepath;
        }

        public void EndRace()
        {
            raceOver = true;
            transform.position = Vector3.zero;
            player.gameObject.SetActive(false);
        }
    }
}