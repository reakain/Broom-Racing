using System;
using System.Collections;
using System.Collections.Generic;
using CleverCrow.Fluid.BTs.Tasks;
using CleverCrow.Fluid.BTs.Trees;
using PathCreation;
using Pathfinding;
using UnityEngine;

namespace BroomRacing
{
    public class Racer : MonoBehaviour
    {
        
        
        [SerializeField] public bool isPlayer = false;
        [SerializeField] private bool _stun = false;
        [SerializeField] private bool _speedBoosted = false;
        //private bool _canBoost = false;
        //private float speedBoostTimer;
        //private float speedBoostWaitTime;

            //Gets these from RaceController
private float boostSpeedBonus = 5f;
private int boostActiveTime = 10;
private int stunTime = 1;
        private float speed = 5.0f;
        private LayerMask m_Obstacles;
        private float courseWidth = 3f;

        public PathCreator pathCreator;
        public EndOfPathInstruction endOfPathInstruction;
        float distanceTravelled;
        float xPos = 0f;
        float spotDistance = 2f;
        float spotRadius = 1f;
        float dodgeStride = 1f;

        private Rigidbody2D rigid2d;

        [SerializeField]
        private BehaviorTree _tree;

        private void Awake()
        {
            rigid2d = GetComponent<Rigidbody2D>();
            pathCreator = FindObjectOfType<PathCreator>();

            _tree = new BehaviorTreeBuilder(gameObject)
            .Sequence()
                .Condition("Is Racing", () => !RaceController.instance.raceOver)
                .Condition("Not Player", () => !isPlayer)
                .Condition("Stunned", () => !_stun)
                .Selector()
                    .Sequence("Speed Boost")
                        //.Condition("Can Speed Boost", () => _canBoost)
                        .Condition("Not Boosted", () => !_speedBoosted)
                        .Do("Use Speed Boost", () =>
                        {
                            StopCoroutine(SpeedBoostLoop());
                            StartCoroutine(SpeedBoostLoop());
                            return TaskStatus.Success;
                        })
                    .End()
                    .Sequence("Move Forward")
                        .Do("Get or move next waypoint", () =>
                        {
                            float xMove = 0f;
                            var hit = Physics2D.CircleCast(rigid2d.position, spotRadius, transform.up, spotDistance, m_Obstacles);
                            if(hit)
                            {
                                dodgeStride = courseWidth < dodgeStride + xPos ? -dodgeStride : dodgeStride;
                                xMove = dodgeStride;
                            }
                            MoveNext(xMove);
                            return TaskStatus.Success;
                        })
                    .End()
                .End()
            .End()
            .Build();
        }

        private void OnEnable()
        {
        }

        // Start is called before the first frame update
        void Start()
        {
            speed = RaceController.instance.raceSpeed;
            boostSpeedBonus = RaceController.instance.boostSpeedBonus;
            boostActiveTime = RaceController.instance.boostActiveTime;
            stunTime = RaceController.instance.stunTime;
            m_Obstacles = RaceController.instance.obstacleLayer;
            courseWidth = RaceController.instance.courseWidth;

        }

        // Update is called once per frame
        void Update()
        {
            
            if (!RaceController.instance.raceOver && isPlayer)
            {
                if(Input.GetButtonDown(InputContainer.instance.boostButton))
                {
                    StopCoroutine(SpeedBoostLoop());
                    StartCoroutine(SpeedBoostLoop());
                }
                MoveNext(0f);
            }
            _tree.Tick();
        }

        public void MoveNext(float xtemp)
        {
            if (pathCreator != null)
            {
                if(isPlayer)
                {
                    xtemp = Input.GetAxis(InputContainer.instance.movementAxis);
                }

                if(!_stun)
                {
                        xPos += (xtemp * speed*Time.deltaTime);
                        xPos = xPos > courseWidth ? courseWidth : xPos;
                        xPos = xPos < -courseWidth ? -courseWidth : xPos;

                        distanceTravelled += speed * Time.deltaTime;

                        Vector3 currentPos = pathCreator.path.GetPointAtDistance(distanceTravelled, endOfPathInstruction);
                        Vector2 localForward = pathCreator.path.GetDirectionAtDistance(distanceTravelled, endOfPathInstruction).normalized;

                        Vector2 localRight = Vector2.Perpendicular(localForward);

                    rigid2d.MoveRotation(Quaternion.FromToRotation(Vector3.up, localForward));

                        Vector2 postemp = currentPos;
                        Vector2 pos2D = currentPos;

                        float horizontalDist = Vector2.Dot(localRight, postemp - rigid2d.position);
                        pos2D += -xPos * localRight;

                        rigid2d.MovePosition(pos2D);

                }

            }
        }


        public void SetStartPosition()
        {
            rigid2d.position = pathCreator.path.GetPoint(0);
            Vector2 localForward = pathCreator.path.GetDirection(0,endOfPathInstruction);
            rigid2d.SetRotation(Quaternion.FromToRotation(Vector3.up, localForward));
        }



        private IEnumerator SpeedBoostLoop()
        {
            var _speed = speed;

            _speedBoosted = true;
            speed += boostSpeedBonus;

            yield return new WaitForSeconds(boostActiveTime);

            _speedBoosted = false;
            speed = _speed;
        }

        public void Stun()
        {
            StopCoroutine(StunLoop());
            StartCoroutine(StunLoop());
        }

        private IEnumerator StunLoop()
        {
            //ResetPath();
            _stun = true;
            yield return new WaitForSeconds(stunTime);
            _stun = false;
        }

        void OnCollisionEnter2D(Collision2D c)
        {
            if ((m_Obstacles.value & 1 << c.gameObject.layer) == 1 << c.gameObject.layer)
            {
                //...tell the Animator about it...
                //anim.SetTrigger("Hit");
                //...and then stun
                Stun();
                Debug.Log("Hit an obstacle!");
            }
        }
    }
}