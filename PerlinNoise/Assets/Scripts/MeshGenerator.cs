// Miguel Rodriguez Gallego
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    Mesh mesh;

    Vector3[] vertices;
    int[] triangles;

    // Mesh terrain size values
    [SerializeField] int xSize = 20;
    [SerializeField] int zSize = 20;

    /// <summary>
    ///     Get mesh references and initialize procedural world generation
    /// </summary>
    void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        CreateShape();        
        UpdateMesh();
    }

    /// <summary>
    ///     Create mesh triangle values
    /// </summary>
    void CreateShape()
    {
        vertices = new Vector3[(xSize + 1) * (zSize + 1)];

        // Triangle vertices positions
        for (int i = 0, z = 0; z <= zSize; z++)
        {
            for (int x = 0; x <= xSize; x++)
            {
                float y = Mathf.PerlinNoise(x * .3f, z * .3f) * 2f;
                vertices[i] = new Vector3(x, y, z);
                i++;
            }
        }

        triangles = new int[xSize * zSize * 6];

        int vert = 0;
        int tris = 0;

        // Set triangle length => Update triangle points
        for (int z = 0; z < zSize; z++)
        {
            for (int x = 0; x < xSize; x++)
            {
                triangles[tris + 0] = vert + 0;
                triangles[tris + 1] = vert + xSize + 1;
                triangles[tris + 2] = vert + 1;
                triangles[tris + 3] = vert + 1;
                triangles[tris + 4] = vert + xSize + 1;
                triangles[tris + 5] = vert + xSize + 2;

                vert++;
                tris += 6;
            }
            vert++;
        }
    }

    /// <summary>
    ///     Calculate mesh
    /// </summary>
    void UpdateMesh()
    {
        mesh.Clear();

        mesh.vertices = vertices;
        mesh.triangles = triangles;

        mesh.RecalculateNormals();
    }

    /// <summary>
    ///     Show terrain vertex
    /// </summary>
    //void OnDrawGizmos()
    //{
    //    for (int i = 0; i < vertices.Length; i++)
    //    {
    //        Gizmos.DrawSphere(vertices[i], .1f);
    //    }
    //}
}
