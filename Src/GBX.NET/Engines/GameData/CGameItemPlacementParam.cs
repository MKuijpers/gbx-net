﻿namespace GBX.NET.Engines.GameData;

/// <summary>
/// Item placement parameters (0x2E020000)
/// </summary>
[Node(0x2E020000)]
public class CGameItemPlacementParam : CMwNod
{
    #region Constants

    const int yawOnlyBit = 1;
    const int notOnObjectBit = 2;
    const int autoRotationBit = 3;
    const int switchPivotManuallyBit = 4;

    #endregion

    #region Fields

    private short flags;
    private Vec3 cube_Center;
    private float cube_Size;
    private float gridSnap_HStep;
    private float gridSnap_VStep;
    private float gridSnap_HOffset;
    private float gridSnap_VOffset;
    private float flyStep;
    private float flyOffset;
    private float pivotSnap_Distance;
    private Vec3[]? pivotPositions;

    #endregion

    #region Properties

    public short Flags
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return flags;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            flags = value;
        }
    }

    [NodeMember]
    public bool YawOnly
    {
        get => (Flags & (1 << yawOnlyBit)) != 0;
        set
        {
            if (value) Flags |= 1 << yawOnlyBit;
            else Flags &= ~(1 << yawOnlyBit);
        }
    }

    [NodeMember]
    public bool NotOnObject
    {
        get => (Flags & (1 << notOnObjectBit)) != 0;
        set
        {
            if (value) Flags |= 1 << notOnObjectBit;
            else Flags &= ~(1 << notOnObjectBit);
        }
    }

    [NodeMember]
    public bool AutoRotation
    {
        get => (Flags & (1 << autoRotationBit)) != 0;
        set
        {
            if (value) Flags |= 1 << autoRotationBit;
            else Flags &= ~(1 << autoRotationBit);
        }
    }

    [NodeMember]
    public bool SwitchPivotManually
    {
        get => (Flags & (1 << switchPivotManuallyBit)) != 0;
        set
        {
            if (value) Flags |= 1 << switchPivotManuallyBit;
            else Flags &= ~(1 << switchPivotManuallyBit);
        }
    }

    [NodeMember]
    public Vec3 Cube_Center
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return cube_Center;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            cube_Center = value;
        }
    }

    [NodeMember]
    public float Cube_Size
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return cube_Size;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            cube_Size = value;
        }
    }

    [NodeMember]
    public float GridSnap_HStep
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return gridSnap_HStep;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            gridSnap_HStep = value;
        }
    }

    [NodeMember]
    public float GridSnap_VStep
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return gridSnap_VStep;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            gridSnap_VStep = value;
        }
    }

    [NodeMember]
    public float GridSnap_HOffset
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return gridSnap_HOffset;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            gridSnap_HOffset = value;
        }
    }

    [NodeMember]
    public float GridSnap_VOffset
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return gridSnap_VOffset;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            gridSnap_VOffset = value;
        }
    }

    [NodeMember]
    public float FlyStep
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return flyStep;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            flyStep = value;
        }
    }

    [NodeMember]
    public float FlyOffset
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return flyOffset;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            flyOffset = value;
        }
    }

    [NodeMember]
    public float PivotSnap_Distance
    {
        get
        {
            DiscoverChunk<Chunk2E020000>();
            return pivotSnap_Distance;
        }
        set
        {
            DiscoverChunk<Chunk2E020000>();
            pivotSnap_Distance = value;
        }
    }

    [NodeMember]
    public Vec3[]? PivotPositions
    {
        get
        {
            DiscoverChunk<Chunk2E020001>();
            return pivotPositions;
        }
        set
        {
            DiscoverChunk<Chunk2E020001>();
            pivotPositions = value;
        }
    }

    #endregion

    #region Constructors

    protected CGameItemPlacementParam()
    {

    }

    #endregion

    #region Chunks

    #region 0x000 skippable chunk

    /// <summary>
    /// CGameItemPlacementParam 0x000 skippable chunk
    /// </summary>
    [Chunk(0x2E020000)]
    public class Chunk2E020000 : SkippableChunk<CGameItemPlacementParam>, IVersionable
    {
        private int version;

        public int Version
        {
            get => version;
            set => version = value;
        }

        public override void ReadWrite(CGameItemPlacementParam n, GameBoxReaderWriter rw)
        {
            rw.Int32(ref version);
            rw.Int16(ref n.flags);
            rw.Vec3(ref n.cube_Center);
            rw.Single(ref n.cube_Size);
            rw.Single(ref n.gridSnap_HStep);
            rw.Single(ref n.gridSnap_VStep);
            rw.Single(ref n.gridSnap_HOffset);
            rw.Single(ref n.gridSnap_VOffset);
            rw.Single(ref n.flyStep);
            rw.Single(ref n.flyOffset);
            rw.Single(ref n.pivotSnap_Distance);
        }
    }

    #endregion

    #region 0x001 skippable chunk (pivot positions)

    /// <summary>
    /// CGameItemPlacementParam 0x001 skippable chunk (pivot positions)
    /// </summary>
    [Chunk(0x2E020001, "pivot positions")]
    public class Chunk2E020001 : SkippableChunk<CGameItemPlacementParam>
    {
        public int U01;

        public override void ReadWrite(CGameItemPlacementParam n, GameBoxReaderWriter rw)
        {
            rw.Array(ref n.pivotPositions,
                (i, r) => r.ReadVec3(),
                (x, w) => w.Write(x));
            rw.Int32(ref U01);
        }
    }

    #endregion

    #endregion
}
