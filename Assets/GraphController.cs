using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GraphController : MonoBehaviour
{
    [SerializeField] Mesh _mesh;
    [SerializeField] Material _material;
    [SerializeField] Texture2D _texture;

    Mesh _renderMesh;

    void Start()
    {
        var points = RandomPoints(1_000);

        var matrices = points.Select(p =>
        {
            var rotation = Quaternion.identity;
            var scale = Vector3.one * 0.01f;
            return Matrix4x4.TRS(p, rotation, scale);
        });
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

    void SetMesh(List<Matrix4x4> matrices)
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
