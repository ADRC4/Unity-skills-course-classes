using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MIConvexHull;
using QuickGraph;
using QuickGraph.Algorithms;


public class GraphController : MonoBehaviour
{
    [SerializeField] Mesh _mesh;
    [SerializeField] Material _material;
    [SerializeField] Texture2D _texture;

    Mesh _renderMesh;

    void Start()
    {
        Random.InitState(42);

        var points = RandomPoints(400);

        var vertices = points
            .Select(p => new Vertex() { Location = p })
            .ToList();

        var triangulation = Triangulation.CreateDelaunay<Vertex, Face>(vertices);
        var edges = triangulation.Cells.SelectMany(f => f.GetEdges())
            .Distinct()
            .Where(e => e.Length < 0.2f);

        var edgeList = edges.ToList();
        var graph = edgeList.ToUndirectedGraph<Vertex, Edge>();
        var tree = graph.MinimumSpanningTreePrim(GetWeight).ToList();

        SetMeshFromEdges(tree);
        PaintEdges(tree);
    }

    double GetWeight(Edge edge)
    {
        return (edge.Target.Location - edge.Source.Location).magnitude;
    }

    void BunnyEdges()
    {
        var points = RandomPoints(400);

        var vertices = points
            .Select(p => new Vertex() { Location = p })
            .ToList();

        var triangulation = Triangulation.CreateDelaunay<Vertex, Face>(vertices);
        var edges = triangulation.Cells.SelectMany(f => f.GetEdges());
        edges = edges.Where(e => e.Length < 0.2f);
        var edgeList = edges.ToList();

        SetMeshFromEdges(edgeList);
        PaintEdges(edgeList);
    }

    void PaintEdges(IList<Edge> edges)
    {
        var colors = new Color[edges.Count * 4];

        for (int i = 0; i < edges.Count; i++)
        {
            var t = 1 - (edges[i].Length / 0.2f);
            t = Mathf.Pow(t, 2.2f);

            var color = Color.white * t;

            for (int j = 0; j < 4; j++)
                colors[i * 4 + j] = color;
        }

        _renderMesh.colors = colors;
    }

    void Update()
    {
        Graphics.DrawMesh(_renderMesh, Matrix4x4.identity, _material, 0);
    }

    void DiscreteBunny()
    {
        var points = RandomPoints(1_000_000);
        var matrices = points.Select(p =>
        {
            var rotation = Quaternion.identity;
            var scale = Vector3.one * 0.0004f;
            return Matrix4x4.TRS(p, rotation, scale);
        });

    }

    void SetMeshFromEdges(IEnumerable<Edge> edges)
    {
        var matrices = edges.Select(e =>
        {
            var rotation = Quaternion.LookRotation(Vector3.forward, e.Vector);
            var scale = new Vector3(0.002f, e.Length, 1f);
            return Matrix4x4.TRS(e.MidPoint, rotation, scale);
        });

        SetMesh(matrices);
    }

    void SetMeshFromPoints(IEnumerable<Vector3> points)
    {
        var matrices = points.Select(p =>
        {
            var rotation = Quaternion.identity;
            var scale = Vector3.one * 0.01f;
            return Matrix4x4.TRS(p, rotation, scale);
        });

        SetMesh(matrices);
    }

    void SetMesh(IEnumerable<Matrix4x4> matrices)
    {
        var instances = matrices
            .Select(m => new CombineInstance() { mesh = _mesh, transform = m });

        var mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        mesh.CombineMeshes(instances.ToArray());
        _renderMesh = mesh;
    }

    IEnumerable<Vector3> RandomPoints(int count)
    {
        while (count > 0)
        {
            float x = Random.value;
            float y = Random.value;
            var color = _texture.GetPixelBilinear(x, y);
            var t = color.r;
            t = Mathf.Pow(t, 2.2f);

            if (t > Random.value)
            {
                var position = new Vector3(x, y, 0);
                count--;
                yield return position;
            }
        }
    }
}
