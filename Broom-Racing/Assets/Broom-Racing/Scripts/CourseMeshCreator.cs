using PathCreation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CourseMeshCreator : MonoBehaviour
{
    [Header("Road settings")]
    public float roadWidth = .4f;
    [Range(0, .5f)]
    public float thickness = .15f;
    public bool flattenSurface;

    [Header("Material settings")]
    //public Material roadMaterial;
    //public Material undersideMaterial;
    public float textureTiling = 1;

    [SerializeField, HideInInspector]
    GameObject pathHolder;

    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;

    GameObject colliderHolder;
    PolygonCollider2D polyCollider;

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
        colliderHolder.layer = LayerMask.NameToLayer("Course");

        if(!colliderHolder.gameObject.GetComponent<PolygonCollider2D>())
        {
            colliderHolder.gameObject.AddComponent<PolygonCollider2D>();
        }

        polyCollider = colliderHolder.gameObject.GetComponent<PolygonCollider2D>();

        Vector2[] vertsA = new Vector2[path.NumPoints];
        Vector2[] vertsB = new Vector2[path.NumPoints];

        for (int i = 0; i < path.NumPoints; i++)
        {
            Vector3 localUp = Vector3.Cross(path.GetTangent(i), path.GetNormal(i));
            Vector3 localRight = path.GetNormal(i);

            // Find position to left and right of current path vertex
            Vector3 vertSideA = path.GetPoint(i) - localRight * Mathf.Abs(roadWidth);
            Vector3 vertSideB = path.GetPoint(i) + localRight * Mathf.Abs(roadWidth);

            vertsA[i] = vertSideA;
            vertsB[i] = vertSideB;
        }
        polyCollider.pathCount = 2;
        polyCollider.SetPath(0, vertsA);
        polyCollider.SetPath(1, vertsB);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
