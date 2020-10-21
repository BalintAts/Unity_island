using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using System.Linq;

public class LeoIsland : MonoBehaviour
{
    public float epsilon;
    public Vector2[] boundaries;  //the vertices of the island polygon
    public Vector3[] vertices3D;
    public Dictionary<string,float> boundingSquare;


    [SerializeField]
    string fileToRead;

    [SerializeField]
    string objectToSpawn;

    public static int GetNumTrees(IEnumerable<Vector2> island, float epsilon)
    {
        throw new NotImplementedException();
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
        foreach (Vector2 vector in boundaries)
        {
            Debug.Log(vector.x);
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
        //float cordY
        //while 
    }
}
