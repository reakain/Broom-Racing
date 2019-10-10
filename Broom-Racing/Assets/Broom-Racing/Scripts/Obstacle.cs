using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace BroomRacing
{
    public class Obstacle : MonoBehaviour
    {
        private PolygonCollider2D m_Collider2D;
        //private Rigidbody2D m_Rigidbody2D;
        private SpriteRenderer m_SpriteRenderer;

        private void Awake()
        {
            m_Collider2D = GetComponent<PolygonCollider2D>();
            //m_Rigidbody2D = GetComponent<Rigidbody2D>();
            m_SpriteRenderer = GetComponent<SpriteRenderer>();
        }

        public void SetRandomObstacle(Sprite newSprite, Vector3 position)
        {
            // Set sprite gravity and mass
            //m_Rigidbody2D.mass = Random.Range(0.01f, 3.0f);
            //m_Rigidbody2D.gravityScale = Random.Range(0.0f, 2.0f);

            /*for (int i = 0; i < m_Collider2D.pathCount; i++)
            {
                m_Collider2D.SetPath(i, null);
            }*/

            // Set sprite and collider
            m_Collider2D.pathCount = newSprite.GetPhysicsShapeCount();
            List<Vector2> path = new List<Vector2>();
            for (int i = 0; i < m_Collider2D.pathCount; i++)
            {
                path.Clear();
                newSprite.GetPhysicsShape(i, path);
                m_Collider2D.SetPath(i, path.ToArray());
            }
            m_SpriteRenderer.sprite = newSprite;

            // Set Sprite position
            this.gameObject.transform.position = position;
        }

        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnCollisionEnter(Collision collision)
        {
            //ContactPoint contact = collision.contacts[0];
            //Quaternion rot = Quaternion.FromToRotation(Vector3.up, contact.normal);
            //Vector3 pos = contact.point;
            //Instantiate(explosionPrefab, pos, rot);
            //Destroy(gameObject);
            Debug.Log("Hit an obstacle!");
        }
    }
}