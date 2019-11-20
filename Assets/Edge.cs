using UnityEngine;
using QuickGraph;
using System;

class Edge : IEdge<Vertex>, IEquatable<Edge>
{
    public Vertex Source { get; set; }
    public Vertex Target { get; set; }

    public Vector3 MidPoint => (Source.Location + Target.Location) * 0.5f;
    public Vector3 Vector => Target.Location - Source.Location;
    public float Length => Vector.magnitude;

    public bool Equals(Edge other)
    {
        if (Source == other.Source && Target == other.Target) return true;
        if (Source == other.Target && Target == other.Source) return true;
        return false;
    }

    public override int GetHashCode()
    {
        return Source.GetHashCode() + Target.GetHashCode();
    }
}
