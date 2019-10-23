using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseMeshCreator : MonoBehaviour
{
    [Header("Course settings")]
    public float courseWidth = 3f;
    public float wallThickness = .5f;
    public LayerMask wallLayer;
    public LayerMask courseLayer;
    public GameObject wallPiecePrefab;

    [SerializeField, HideInInspector]
    GameObject pathHolder;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;

    GameObject colliderHolder;
    PolygonCollider2D polyCollider;

    GameObject wallHolder;
    CompositeCollider2D wallCollider;

    public PathCreator pathCreator;
    protected VertexPath path
    {
        get
        {
            return pathCreator.path;
        }
    }

    private void Awake()
    {
        pathCreator = GetComponent<PathCreator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void GenerateCollider()
    {
        if(colliderHolder == null)
        {
            colliderHolder = new GameObject("Path Collider Holder");
        }

        colliderHolder.transform.position = Vector3.zero;
        colliderHolder.transform.rotation = Quaternion.identity;
        colliderHolder.transform.localScale = Vector3.one;
        colliderHolder.layer = courseLayer.value;

        if(!colliderHolder.gameObject.GetComponent<PolygonCollider2D>())
        {
            colliderHolder.gameObject.AddComponent<PolygonCollider2D>();
        }

        polyCollider = colliderHolder.gameObject.GetComponent<PolygonCollider2D>();

        

        Vector2[] vertsA1 = new Vector2[path.NumPoints];
        Vector2[] vertsA2 = new Vector2[path.NumPoints];
        Vector2[] vertsB1 = new Vector2[path.NumPoints];
        Vector2[] vertsB2 = new Vector2[path.NumPoints];

        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 localUp = Vector3.Cross(path.GetTangent(i), path.GetNormal(i));
            Vector3 localRight = path.GetNormal(i);

            // Find position to left and right of current path vertex
            Vector3 vertSideA1 = path.GetPoint(i) - localRight * Mathf.Abs(courseWidth);
            Vector3 vertSideA2 = path.GetPoint(i) - localRight * Mathf.Abs(courseWidth + wallThickness);
            Vector3 vertSideB1 = path.GetPoint(i) + localRight * Mathf.Abs(courseWidth);
            Vector3 vertSideB2 = path.GetPoint(i) + localRight * Mathf.Abs(courseWidth + wallThickness);

            vertsA1[i] = vertSideA1;
            vertsA2[i] = vertSideA2;
            vertsB1[i] = vertSideB1;
            vertsB2[i] = vertSideB2;
        }
        polyCollider.pathCount = 4;
        polyCollider.SetPath(0, vertsA1);
        polyCollider.SetPath(1, vertsA2);
        polyCollider.SetPath(2, vertsB1);
        polyCollider.SetPath(3, vertsB2);
    }

    public void GenerateAll()
    {
        GenerateCollider();
        GenerateWalls();
    }

    public void GenerateWalls()
    {
        if (wallHolder == null)
        {
            wallHolder = new GameObject("Wall Collider Holder");
        }

        wallHolder.transform.position = Vector3.zero;
        wallHolder.transform.rotation = Quaternion.identity;
        wallHolder.transform.localScale = Vector3.one;
        wallHolder.layer = wallLayer;

        if (!wallHolder.gameObject.GetComponent<CompositeCollider2D>())
        {
            wallHolder.gameObject.AddComponent<CompositeCollider2D>();
        }

        var rigid = wallHolder.GetComponent<Rigidbody2D>();
        //rigid.freezeRotation = true;
        rigid.gravityScale = 0;
        rigid.constraints = RigidbodyConstraints2D.FreezeAll;
        wallCollider = wallHolder.gameObject.GetComponent<CompositeCollider2D>();

        Vector3 firstRight = path.GetNormal(path.NumPoints - 1);

        // Find position to left and right of current path vertex
        Vector2 vertSideA = path.GetPoint(path.NumPoints - 1) - firstRight * Mathf.Abs(courseWidth);
        Vector2 vertSideB = path.GetPoint(path.NumPoints - 1) + firstRight * Mathf.Abs(courseWidth);


        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 localUp = Vector3.Cross(path.GetTangent(i), path.GetNormal(i));
            Vector3 localRight = path.GetNormal(i);

            Vector2 nextA = path.GetPoint(i) - localRight * Mathf.Abs(courseWidth);
            Vector2 nextB = path.GetPoint(i) + localRight * Mathf.Abs(courseWidth);

            var newWallA = Instantiate(wallPiecePrefab,wallHolder.transform);
            float distanceA = Vector2.Distance(vertSideA, nextA);
            Vector2 posA = Vector2.Lerp(vertSideA, nextA, distanceA);
            Vector2 tangentA = vertSideA.RotateTowards(nextA);
            newWallA.transform.position = posA;
            newWallA.transform.rotation = Quaternion.AngleAxis(posA.AngleBetweenDeg(nextA),Vector3.forward);
            newWallA.transform.localScale = tangentA.normalized * distanceA;

            var newWallB = Instantiate(wallPiecePrefab, wallHolder.transform);
            float distanceB = Vector2.Distance(vertSideB, nextB);
            Vector2 posB = Vector2.Lerp(vertSideB, nextB, distanceB);
            Vector2 tangentB = vertSideB.RotateTowards(nextB);
            newWallB.transform.position = posB;
            newWallB.transform.rotation = Quaternion.AngleAxis(posB.AngleBetweenDeg(nextB), Vector3.forward);
            newWallB.transform.localScale = tangentB.normalized * distanceB;


            // Find position to left and right of current path vertex
            vertSideA = new Vector3(nextA.x, nextA.y);
            vertSideB = new Vector3(nextB.x, nextB.y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void GetCrossing()
    {
        for (int i = 0; i < path.NumPoints; i++)
        {

        }
    }
}
