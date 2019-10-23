using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BroomRacing
{
    public class RaceController : MonoBehaviour
    {
        public static RaceController instance;

        public Racer[] racers;
        private Racer player;

        private int raceLoops = 0;

        public LevelGenerator levelGenerator;

        // Used by rest of scene
        public bool raceOver = true;
        public float raceSpeed = 4f;
        [SerializeField] public float boostSpeedBonus = 5f;
        [SerializeField] public int boostActiveTime = 10;
        [SerializeField] public int stunTime = 1;
        [SerializeField] public LayerMask obstacleLayer;
        public float courseWidth = 3f;

        public float raceStartTime = 0.0f;
        public float raceTime = 0.0f;

        public Camera StartScreenCamera;
        public TMPro.TextMeshProUGUI raceTimeLabel;

        private void Awake()
        {
            instance = this;
            levelGenerator = FindObjectOfType<LevelGenerator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            racers = FindObjectsOfType<Racer>();
            foreach(var racer in racers)
            {
                racer.gameObject.SetActive(false);
                if(racer.isPlayer)
                {
                    player = racer;
                }
            }
            raceOver = true;
        }

        // Update is called once per frame
        void Update()
        {
            if(!raceOver)
            {
                raceTime = Time.time - raceStartTime;

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

        public void StartRace()
        {
                raceStartTime = Time.time;
                raceTime = 0.0f;
                levelGenerator.GenerateLevelLayout();

                foreach(var racer in racers)
                {
                racer.gameObject.SetActive(true);
                    racer.SetStartPosition();
                }
            raceLoops = 0;
                raceOver = false;
            StartScreenCamera.gameObject.SetActive(false);
        }


        public void EndRace()
        {
            raceOver = true;
            transform.position = Vector3.zero;
            player.gameObject.SetActive(false);
            StartScreenCamera.gameObject.SetActive(true);
        }
    }
}