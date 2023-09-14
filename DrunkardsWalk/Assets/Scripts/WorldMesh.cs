using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class WorldMesh : MonoBehaviour
{
    MeshFilter meshFilter;
    MeshCollider coll;
    Mesh mesh;

    List<Vector3> vertex;
    List<Color> colors;
    List<int> tris;

    private void Awake()
    {
        vertex = new List<Vector3>();
        colors = new List<Color>();
        tris = new List<int>();

        meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh = new Mesh();
        coll = gameObject.AddComponent<MeshCollider>();
        mesh.name = "WordlMesh";
    }

    private void GenerateTri(Vector3 v1, Vector3 v2, Vector3 v3, Color color)
    {
        int vertexAmount = vertex.Count;
        vertex.Add(v1);
        vertex.Add(v2);
        vertex.Add(v3);

        colors.Add(color);
        colors.Add(color);
        colors.Add(color);

        tris.Add(0 + vertexAmount);
        tris.Add(1 + vertexAmount);
        tris.Add(2 + vertexAmount);
    }

    private void GenerateQuad(Vector3 cornerPosition, Color color)
    {
        Vector3 v0, v1, v2 , v3;
        v0 = v1 = v2 = v3 = cornerPosition;
        v1.x += Metrics.cellSize.x;  
        v2.z += Metrics.cellSize.y;
        v3.x += Metrics.cellSize.x;
        v3.z += Metrics.cellSize.y;

        GenerateTri(v0, v2, v1, color);
        GenerateTri(v1, v2, v3, color);
    }

    public void GenerateMesh(WorldCell[] cells)
    {
        vertex.Clear();
        colors.Clear();
        tris.Clear();

        foreach (WorldCell cell in cells)
        {
            GenerateQuad(cell.transform.position, cell.GetColor());
        }
        mesh.vertices  = vertex.ToArray();
        mesh.colors    = colors.ToArray();
        mesh.triangles = tris.ToArray();
        mesh.RecalculateNormals();
    }

    /*
    private void GenerateQuad()
    {
        vertex.Add(new Vector3(0, 0, 0));
        vertex.Add(new Vector3(0, 0, 1));
        vertex.Add(new Vector3(1, 0, 0));
        vertex.Add(new Vector3(1, 0, 1));

        colors.Add(Color.black);
        colors.Add(Color.black);
        colors.Add(Color.black);
        colors.Add(Color.black);

        tris.Add(0);
        tris.Add(2);
        tris.Add(1);

        tris.Add(2);
        tris.Add(3);
        tris.Add(1);
    }
    */
    
}
