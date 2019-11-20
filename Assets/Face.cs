using System.Collections.Generic;
using MIConvexHull;

class Face : TriangulationCell<Vertex, Face>
{
    public IEnumerable<Edge> GetEdges()
    {
        for (int i = 0; i < Vertices.Length; i++)
        {
            int j = (i < Vertices.Length - 1) ? i + 1 : 0;

            yield return new Edge()
            {
                Source = Vertices[i],
                Target = Vertices[j]
            };
        }
    }

}
