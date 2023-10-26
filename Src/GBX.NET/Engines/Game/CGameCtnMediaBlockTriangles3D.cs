﻿using GBX.NET.Builders.Engines.Game;

namespace GBX.NET.Engines.Game;

/// <summary>
/// MediaTracker block - 3D triangles.
/// </summary>
/// <remarks>ID: 0x0304C000</remarks>
[Node(0x0304C000)]
[NodeExtension("GameCtnMediaBlockTriangles3D")]
public class CGameCtnMediaBlockTriangles3D : CGameCtnMediaBlockTriangles
{
    internal CGameCtnMediaBlockTriangles3D()
    {

    }
    
    /// <param name="vertices">Array of vertex colors.</param>
    public static CGameCtnMediaBlockTriangles3DBuilder Create(Vec4[] vertices) => new(vertices);
}
