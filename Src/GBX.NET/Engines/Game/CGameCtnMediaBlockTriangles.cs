﻿namespace GBX.NET.Engines.Game;

/// <summary>
/// MediaTracker block - Triangles.
/// </summary>
/// <remarks>ID: 0x03029000</remarks>
[Node(0x03029000)]
[NodeExtension("GameCtnMediaBlockTriangles")]
public abstract partial class CGameCtnMediaBlockTriangles : CGameCtnMediaBlock, CGameCtnMediaBlock.IHasKeys
{
    #region Fields

    private IList<Key> keys = Array.Empty<Key>();
    private Vec4[] vertices = Array.Empty<Vec4>();
    private Int3[] triangles = Array.Empty<Int3>();

    #endregion

    #region Properties

    IEnumerable<CGameCtnMediaBlock.Key> IHasKeys.Keys
    {
        get => Keys.Cast<CGameCtnMediaBlock.Key>();
        set => Keys = value.Cast<Key>().ToList();
    }

    [NodeMember]
    [AppliedWithChunk<Chunk03029001>]
    public IList<Key> Keys { get => keys;  set => keys = value; }

    [NodeMember]
    [AppliedWithChunk<Chunk03029001>]
    public Vec4[] Vertices
    {
        get => vertices;
        set
        {
            if (vertices is null || value.Length != vertices.Length)
            {
                vertices ??= value;

                foreach (var key in keys)
                {
                    var positions = key.Positions;
                    Array.Resize(ref positions, value.Length);
                    key.Positions = positions;
                }

                RemoveTrianglesOutOfRange();
            }

            vertices = value;
        }
    }

    [NodeMember]
    [AppliedWithChunk<Chunk03029001>]
    public Int3[] Triangles
    {
        get => triangles;
        set
        {
            if (vertices is null)
            {
                return;
            }

            foreach (var int3 in value)
            {
                if (int3.X >= vertices.Length
                 || int3.Y >= vertices.Length
                 || int3.Z >= vertices.Length)
                    throw new Exception($"Index in {int3} is not available in vertices.");
            }

            triangles = value;
        }
    }

    #endregion

    #region Constructors

    internal CGameCtnMediaBlockTriangles()
    {
        
    }

    #endregion

    #region Methods

    private void RemoveTrianglesOutOfRange()
    {
        if (triangles == null) return;

        var trianglesToRemove = new List<Int3>();

        foreach (var triangle in triangles)
        {
            if (triangle.X >= vertices.Length
            || triangle.Y >= vertices.Length
            || triangle.Z >= vertices.Length)
            {
                trianglesToRemove.Add(triangle);
            }
        }

        triangles = triangles.Where(x => !trianglesToRemove.Contains(x)).ToArray();
    }

    #endregion

    #region Chunks

    #region 0x001 chunk

    /// <summary>
    /// CGameCtnMediaBlockTriangles 0x001 chunk
    /// </summary>
    [Chunk(0x03029001)]
    public class Chunk03029001 : Chunk<CGameCtnMediaBlockTriangles>
    {
        public int U01;
        public int U02;
        public int U03;
        public float U04;
        public int U05;
        public long U06;

        public override void Read(CGameCtnMediaBlockTriangles n, GameBoxReader r)
        {
            n.keys = r.ReadList(r1 => new Key(n)
            {
                Time = r1.ReadTimeSingle()
            });

            var numKeys = r.ReadInt32();
            var numVerts = r.ReadInt32();

            for (var i = 0; i < numKeys; i++)
            {
                n.keys[i].Positions = new Vec3[numVerts];

                for (var j = 0; j < numVerts; j++)
                {
                    n.keys[i].Positions[j] = r.ReadVec3();
                }
            }

            n.vertices = r.ReadArray<Vec4>();
            n.triangles = r.ReadArray<Int3>();

            U01 = r.ReadInt32();
            U02 = r.ReadInt32();
            U03 = r.ReadInt32();
            U04 = r.ReadSingle();
            U05 = r.ReadInt32();
            U06 = r.ReadInt64();
        }

        public override void Write(CGameCtnMediaBlockTriangles n, GameBoxWriter w)
        {
            w.WriteList(n.keys, (x, w) => w.WriteTimeSingle(x.Time));
            w.Write(n.keys.Count);
            w.Write(n.vertices.Length);

            foreach (var key in n.keys)
            {
                foreach (var pos in key.Positions)
                {
                    w.Write(pos);
                }
            }

            w.WriteArray(n.vertices);
            w.WriteArray(n.triangles);

            w.Write(U01);
            w.Write(U02);
            w.Write(U03);
            w.Write(U04);
            w.Write(U05);
            w.Write(U06);
        }
    }

    #region 0x002 chunk

    /// <summary>
    /// CGameCtnMediaBlockTriangles 0x002 skippable chunk
    /// </summary>
    [Chunk(0x03029002)]
    public class Chunk03029002 : SkippableChunk<CGameCtnMediaBlockTriangles>
    {
        public int U01;

        public override void ReadWrite(CGameCtnMediaBlockTriangles n, GameBoxReaderWriter rw)
        {
            rw.Int32(ref U01);
        }
    }

    #endregion

    #endregion

    #endregion
}
