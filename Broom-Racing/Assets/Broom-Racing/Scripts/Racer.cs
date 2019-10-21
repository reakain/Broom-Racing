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
        [SerializeField] private float m_BoostForce = 400f;                          // Amount of force added when the player Boosts.
        [Range(0, .3f)] [SerializeField] private float m_MovementSmoothing = .3f;  // How much to smooth out the movement
        [SerializeField] private LayerMask m_Walls;                          // A mask determining what are walls
        [SerializeField] private LayerMask m_Obstacles;
        [SerializeField] private float m_Speed = 5.0f;
        [SerializeField] private float analogDeadZone = 0.5f;
        [SerializeField] private bool isPlayer = false;
        [SerializeField] private bool _stun = false;
        [SerializeField] private bool _speedBoosted = false;
        //private bool _canBoost = false;
        //private float speedBoostTimer;
        //private float speedBoostWaitTime;

        [SerializeField] private float boostSpeedBonus = 5f;
        [SerializeField] private int boostActiveTime = 10;
        [SerializeField] private int stunTime = 1;

        public PathCreator pathCreator;

        private float _speed;
        private float _turning;

        private Rigidbody2D rigid2d;
        private Vector3 m_Velocity = Vector3.zero;
        private Collider2D m_HitBox;

        [SerializeField]
        private BehaviorTree _tree;

        // For Pathing
        public GameObject target;
        public Vector2 targetBounds = Vector2.zero;
        public float speed = 20;
        public float turnSpeed = 3;
        public float turnDst = 5;
        public float stoppingDst = 10;
        protected Path2D path;
        const float minPathUpdateTime = .2f;
        const float pathUpdateMoveThreshold = .5f;

        private void Awake()
        {
            rigid2d = GetComponent<Rigidbody2D>();
            m_HitBox = GetComponent<Collider2D>();
            rigid2d.freezeRotation = false;

            _tree = new BehaviorTreeBuilder(gameObject)
            .Sequence()
                .Condition("Not Player", () => !isPlayer)
                .Condition("Stunned", () => _stun)
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
            StartCoroutine(UpdatePath());
        }

        // Update is called once per frame
        void Update()
        {
            _tree.Tick();
            Vector2 targetPos;
            if(isPlayer)
            {
                targetPos = GetPlayerMovement();
            }
            else
            {
                // AI goes here
                targetPos = Vector2.zero;
            }
            Move(targetPos);
            LockPosition();
        }

        public void Move(Vector2 movement)
        {
            // Set new velocity
            Vector3 targetVelocity = movement * m_Speed;
            //transform.position = RaceController.instance.transform.position;
            // And then smoothing it out and applying it 
            rigid2d.velocity = Vector3.SmoothDamp(rigid2d.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
        }

        public void LockPosition()
        {
            transform.up = RaceController.instance.transform.up;
            if (isPlayer)
            {
                transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);
            }
        }

        public void SetStartPosition()
        {
            transform.up = RaceController.instance.transform.up;

            transform.localPosition = new Vector3(transform.localPosition.x, 0, 0);
        }

        public Vector2 GetAIPoint()
        {
            return Vector2.zero;
        }

        Vector2 GetPlayerMovement()
        {
            float xtemp = Input.GetAxis(InputContainer.instance.movementAxis);
            float ytemp = Input.GetAxis(InputContainer.instance.raceAxis);


            xtemp = (Math.Abs(xtemp) > analogDeadZone) ? xtemp : 0.0f;
            ytemp = (Math.Abs(ytemp) > analogDeadZone) ? ytemp : 0.0f;

            return (new Vector2(xtemp, ytemp));
        }

        void SetDestination(GameObject pos, Vector2 newbounds)
        {
            if (target == pos)
            {
                return;
            }
            target = pos;
            targetBounds = newbounds;
            StopCoroutine("FollowPath");
        }

        private IEnumerator SpeedBoostLoop()
        {
            var speed = _speed;
            var turning = _turning;

            _speedBoosted = true;
            _speed += boostSpeedBonus;
            _turning += 50;

            yield return new WaitForSeconds(boostActiveTime);

            _speedBoosted = false;
            _speed = speed;
            _turning = turning;
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
            yield return new WaitForSeconds(1);
            _stun = false;
        }

        void OnCollisionEnter2D(Collision2D c)
        {
            if ((m_Obstacles.value & 1 << c.gameObject.layer) == 1 << c.gameObject.layer)
            {
                // Zero out the velocity
                rigid2d.velocity = Vector2.zero;

                // Force position kickback
                rigid2d.AddForce((rigid2d.transform.position - c.transform.position).normalized * -500f);

                //...tell the Animator about it...
                //anim.SetTrigger("Hit");
                //...and tell the game control about it.
                //GameControl.instance.
                Debug.Log("Hit an obstacle!");
            }
        }

        protected IEnumerator FollowPath()
        {

            bool followingPath = true;
            int pathIndex = 0;
            //transform.LookAt (path.lookPoints [0],Vector3.up);

            float speedPercent = 1;

            while (followingPath)
            {
                Vector2 pos2D;

                if (rigid2d)
                {
                    pos2D = new Vector2(rigid2d.position.x, rigid2d.position.y);
                }
                else
                {
                    pos2D = new Vector2(transform.position.x, transform.position.y);
                }

                while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
                {
                    if (pathIndex == path.finishLineIndex)
                    {
                        followingPath = false;
                        break;
                    }
                    else
                    {
                        pathIndex++;
                    }
                }

                if (followingPath)
                {

                    if (pathIndex >= path.slowDownIndex && stoppingDst > 0)
                    {
                        speedPercent = Mathf.Clamp01(path.turnBoundaries[path.finishLineIndex].DistanceFromPoint(pos2D) / stoppingDst);
                        if (speedPercent < 0.01f)
                        {
                            followingPath = false;
                        }
                    }

                    //Quaternion targetRotation = Quaternion.LookRotation (Vector3.zero,path.lookPoints [pathIndex] - transform.position);
                    //transform.rotation = Quaternion.Lerp (transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                    //transform.Translate (Vector3.up * Time.deltaTime * speed * speedPercent, Space.Self);
                    Vector2 move;
                    if (rigid2d)
                    {
                        move = Vector2.MoveTowards(rigid2d.position, path.lookPoints[pathIndex], Time.deltaTime * speed * speedPercent);
                        rigid2d.MovePosition(move);
                    }
                    else
                    {
                        move = Vector2.MoveTowards(transform.position, path.lookPoints[pathIndex], Time.deltaTime * speed * speedPercent);
                        transform.position = move;
                    }
                }

                yield return null;

            }
        }

        protected IEnumerator UpdatePath()
        {

            if (Time.timeSinceLevelLoad < 0.5f)
            {
                yield return new WaitForSeconds(0.5f);
            }
            if (rigid2d)
            {
                PathRequestManager2D.RequestPath(new PathRequest(rigid2d.position, target.transform.position, targetBounds, OnPathFound));
            }
            else
            {
                PathRequestManager2D.RequestPath(new PathRequest(transform.position, target.transform.position, targetBounds, OnPathFound));
            }

            float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
            Vector3 targetPosOld = target.transform.position;

            while (true)
            {
                yield return new WaitForSeconds(minPathUpdateTime);
                //print (((target.position - targetPosOld).sqrMagnitude) + "    " + sqrMoveThreshold);
                if ((target.transform.position - targetPosOld).sqrMagnitude > sqrMoveThreshold)
                {
                    if (rigid2d)
                    {
                        PathRequestManager2D.RequestPath(new PathRequest(rigid2d.position, target.transform.position, targetBounds, OnPathFound));
                    }
                    else
                    {
                        PathRequestManager2D.RequestPath(new PathRequest(transform.position, target.transform.position, targetBounds, OnPathFound));
                    }
                    targetPosOld = target.transform.position;
                }
            }
        }

        public virtual void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
        {
            if (pathSuccessful)
            {
                path = new Path2D(waypoints, transform.position, turnDst, stoppingDst);

                StopCoroutine("FollowPath");
                StartCoroutine("FollowPath");
            }
        }
        public void OnDrawGizmos()
        {
            if (path != null)
            {
                path.DrawWithGizmos();
            }
        }
    }
}