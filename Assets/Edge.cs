using UnityEngine;

class Edge
{
    public Vertex Source;
    public Vertex Target;

    public Vector3 MidPoint => (Source.Location + Target.Location) * 0.5f;
    public Vector3 Vector => Target.Location - Source.Location;
    public float Length => Vector.magnitude;

}
