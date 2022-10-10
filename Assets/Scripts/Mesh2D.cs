using System;
using UnityEngine;

[CreateAssetMenu]
public class Mesh2D : ScriptableObject
{
    [Serializable]
    public struct Vertex
    {
        public Vector2 point;
        public Vector2 normal;
        public float u; // uvs but not v
    }

    public Vertex[] vertices;
    public int[] lineIndices;
}