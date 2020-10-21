using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;

public class LeoIsland : MonoBehaviour
{
    public float epsilon;
    public Vector2[] boundaries;  //the vertices of the island polygon

    [SerializeField]
    string fileToRead;
    
    public static int GetNumTrees(IEnumerable<Vector2> island, float epsilon)
    {
        throw new NotImplementedException();
    }

    public static Vector2[] LoadIsland(StreamReader sr)
    {
        List<Vector2> vectors = new List<Vector2>() ;
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
        foreach(Vector2 vector in boundaries)
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
            vertices[i] = new Vector3(vertices2D[i].x, vertices2D[i].y, 0);
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
    }
}
