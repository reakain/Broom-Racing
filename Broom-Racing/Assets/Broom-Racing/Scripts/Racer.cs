using System;
using System.Collections;
using System.Collections.Generic;
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

        private Rigidbody2D m_Rigidbody2D;
        private Vector3 m_Velocity = Vector3.zero;
        private Collider2D m_HitBox;

        

        private void Awake()
        {
            m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_HitBox = GetComponent<Collider2D>();
            m_Rigidbody2D.freezeRotation = true;
        }

        private void OnEnable()
        {
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
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
            m_Rigidbody2D.velocity = Vector3.SmoothDamp(m_Rigidbody2D.velocity, targetVelocity, ref m_Velocity, m_MovementSmoothing);
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

        void OnCollisionEnter2D(Collision2D c)
        {
            if ((m_Obstacles.value & 1 << c.gameObject.layer) == 1 << c.gameObject.layer)
            {
                // Zero out the velocity
                m_Rigidbody2D.velocity = Vector2.zero;

                // Force position kickback
                m_Rigidbody2D.AddForce((m_Rigidbody2D.transform.position - c.transform.position).normalized * -500f);

                //...tell the Animator about it...
                //anim.SetTrigger("Hit");
                //...and tell the game control about it.
                //GameControl.instance.
                Debug.Log("Hit an obstacle!");
            }
        }
    }
}