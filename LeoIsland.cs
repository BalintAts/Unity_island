using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Linq;
using System.Runtime.CompilerServices;

public class LeoIsland : MonoBehaviour
{
    //public float epsilon;
    public Vector2[] boundaries;  //the vertices of the island polygon for read fron file
    public Vector3[] vertices3D;  //same with 3d vectors
    public Dictionary<string,float> boundingSquare;
    public static int numberOfTrees = 0;


    [SerializeField]
    string fileToRead;

    [SerializeField]
    float density;

    [SerializeField]
    GameObject objectToSpawn;

    public static int GetNumTrees(IEnumerable<Vector2> island, float epsilon)
    {
        return numberOfTrees;
    }



    public static Vector2[] LoadIsland(StreamReader sr)
    {
        List<Vector2> vectors = new List<Vector2>();
        while (sr.Peek() >= 0)
        {
            string line = sr.ReadLine();
            string[] coordinates = line.Split(',');
            vectors.Add(new Vector2(float.Parse(coordinates[0], CultureInfo.InvariantCulture.NumberFormat),
                                    float.Parse(coordinates[1], CultureInfo.InvariantCulture.NumberFormat)));

        }
        return vectors.ToArray();
    }

    void Awake()
    {
        string path = Path.Combine(Application.streamingAssetsPath, fileToRead);
        using (StreamReader sr = new StreamReader(path))
        {
            boundaries = LoadIsland(sr);
        }

        CreateIslandMesh();
    }

    void CreateIslandMesh()
    {
        // Create Vector2 vertices
        Vector2[] vertices2D = boundaries;

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(vertices2D);
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[vertices2D.Length];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(vertices2D[i].x, 0, vertices2D[i].y);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;

        vertices3D = vertices;
        CreateBoundingSquare();
        SpawnTrees();
    }

    void CreateBoundingSquare()
    {
        List<float> verticesX = new List<float>();
        List<float> verticesZ = new List<float>();
        foreach(Vector3 vertex in vertices3D)
        {
            verticesX.Add(vertex.x);
            verticesZ.Add(vertex.z);
        }
        float left = verticesX.Min();
        float right = verticesX.Max();
        float bottom = verticesZ.Min();
        float top = verticesZ.Max();

        boundingSquare = new Dictionary<string, float>(){ { "left" , left },
                                                          { "right" ,  right }, 
                                                            {"top" , top }, 
                                                        {"bottom" , bottom } };
    }

    void SpawnTrees()
    {
        float cordX = boundingSquare["left"];
        while (cordX < boundingSquare["right"])
        {
            float cordZ = boundingSquare["bottom"];
            while (cordZ <= boundingSquare["top"])
            {
                if (CanPutTreeToIsland(cordX, cordZ))
                {
                    Instantiate(objectToSpawn, new Vector3(cordX, 0, cordZ), new Quaternion(0, 0, 0, 0));
                    numberOfTrees++;
                }
                cordZ+= density;
            }
            cordX += density;
        }
    }

    public bool CanPutTreeToIsland(float cordX, float cordZ) {
        GFG.Point[] points = new GFG.Point[boundaries.Count()];
        for(int i=0; i <boundaries.Count()  ; i++)
        {   
            //Check for every side if can put close to it
            points[i] = new GFG.Point(boundaries[i][0], boundaries[i][1]);
            int k = (i+1) % boundaries.Length;
            Vector3 lineStart = new Vector3(boundaries[i][0], 0, boundaries[i][1]);
            Vector3 lineEnd = new Vector3(boundaries[k][0], 0, boundaries[k][1]);
            //if (!CanPutTreeByLine(lineStart, lineEnd, new Vector3(cordX, 0, cordZ)))  //this test happens int the Tree prefab's coastTest script
            //{
            //    return false;
            //}
        }
        return GFG.isInside(points, boundaries.Count(), new GFG.Point(cordX, cordZ));
    }

    //public bool CanPutTreeByLine(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    //{
    //    Vector3 closestOnLine = FindClosestPointOnLineSegment(lineStart, lineEnd, point);
    //    if ((closestOnLine - point).magnitude >= epsilon)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

    //public static Vector3 FindClosestPointOnLineSegment(Vector3 lineStart, Vector3 lineEnd, Vector3 point)
    //{
    //    Vector3 line = lineEnd - lineStart;
    //    Vector3 dir = point - lineStart;
    //    float d = Vector3.Dot(line, dir) / line.sqrMagnitude;
    //    d = Mathf.Clamp01(d);
    //    return Vector3.Lerp(lineStart, lineEnd, d);
    //}
}
