﻿#if DEBUG
using System.Diagnostics;
#endif

namespace GBX.NET.Engines.Game;

/// <summary>
/// CGameCtnReplayRecord (0x03093000)
/// </summary>
/// <remarks>A replay.</remarks>
[Node(0x03093000), WritingNotSupported]
[NodeExtension("Replay")]
public partial class CGameCtnReplayRecord : CMwNod, CGameCtnReplayRecord.IHeader
{
    #region Fields

    private Ident? mapInfo;
    private TimeInt32? time;
    private string? playerNickname;
    private string? playerLogin;
    private string? titleID;
    private string? xml;
    private int? authorVersion;
    private string? authorLogin;
    private string? authorNickname;
    private string? authorZone;
    private string? authorExtraInfo;
    private byte[]? challengeData;
    private CGameCtnChallenge? challenge;
    private CGameCtnGhost[]? ghosts;
    private long[]? extras;
    private CGameCtnMediaClip? clip;
    private CPlugEntRecordData? recordData;
    private CCtnMediaBlockEventTrackMania? events;
    private int? eventsDuration;
    private ControlEntry[]? controlEntries;
    private string? game;
    private CCtnMediaBlockUiTMSimpleEvtsDisplay? simpleEventsDisplay;

    #endregion

    #region Properties

#if DEBUG
    /// <summary>
    /// Shows members that are available from the GBX header (members where reading the body is not required). This method is available only in DEBUG configuration.
    /// </summary>
    /// <remarks>This is just a helper method that just returns THIS object, just casted as <see cref="IHeader"/>. Avoid using this method in production.</remarks>
    public IHeader GetHeaderMembers() => this;
#endif

    /// <summary>
    /// Map UID, environment, and author login of the map the replay orients in.
    /// </summary>
    [NodeMember]
    public Ident? MapInfo => mapInfo;

    /// <summary>
    /// The record time.
    /// </summary>
    [NodeMember]
    public TimeInt32? Time => time;

    /// <summary>
    /// Nickname of the record owner.
    /// </summary>
    [NodeMember]
    public string? PlayerNickname => playerNickname;

    /// <summary>
    /// Login of the record owner.
    /// </summary>
    [NodeMember]
    public string? PlayerLogin => playerLogin;

    /// <summary>
    /// Title pack the replay orients in.
    /// </summary>
    [NodeMember]
    public string? TitleID
    {
        get
        {
            DiscoverChunk<Chunk03093018>();
            return titleID;
        }
    }

    /// <summary>
    /// XML replay information.
    /// </summary>
    [NodeMember]
    public string? XML => xml;

    [NodeMember]
    public int? AuthorVersion
    {
        get
        {
            DiscoverChunk<Chunk03093018>();
            return authorVersion;
        }
    }

    /// <summary>
    /// Login of the replay creator.
    /// </summary>
    [NodeMember]
    public string? AuthorLogin
    {
        get
        {
            DiscoverChunk<Chunk03093018>();
            return authorLogin;
        }
    }

    /// <summary>
    /// Nickname of the replay creator.
    /// </summary>
    [NodeMember]
    public string? AuthorNickname
    {
        get
        {
            DiscoverChunk<Chunk03093018>();
            return authorNickname;
        }
    }

    /// <summary>
    /// Zone of the replay creator.
    /// </summary>
    [NodeMember]
    public string? AuthorZone
    {
        get
        {
            DiscoverChunk<Chunk03093018>();
            return authorZone;
        }
    }

    [NodeMember]
    public string? AuthorExtraInfo
    {
        get
        {
            DiscoverChunk<Chunk03093018>();
            return authorExtraInfo;
        }
    }

    /// <summary>
    /// The map the replay orients in. Null if only the header was read.
    /// </summary>
    [NodeMember]
    public CGameCtnChallenge? Challenge
    {
        get
        {
            if (challengeData is null)
            {
                return null;
            }

            if (challenge is null)
            {
                using var ms = new MemoryStream(challengeData);
                challenge = GameBox.ParseNode<CGameCtnChallenge>(ms);
            }

            return challenge;
        }
    }

    /// <summary>
    /// Ghosts in the replay. Null if only the header was read.
    /// </summary>
    /// <remarks>Some ghosts can be considered as <see cref="CGameCtnMediaBlockGhost"/>. See <see cref="Clip"/>.</remarks>
    [NodeMember]
    public CGameCtnGhost[]? Ghosts => ghosts;

    [NodeMember]
    public long[]? Extras => extras;

    /// <summary>
    /// MediaTracker clip of the replay.
    /// </summary>
    [NodeMember]
    public CGameCtnMediaClip? Clip => clip;

    [NodeMember]
    public CPlugEntRecordData? RecordData => recordData;

    /// <summary>
    /// Events occuring during the replay. Available in TMS and older games.
    /// </summary>
    [NodeMember]
    public CCtnMediaBlockEventTrackMania? Events => events;

    /// <summary>
    /// Events occuring during the replay. Available in TMS and older games.
    /// </summary>
    [NodeMember]
    public CCtnMediaBlockUiTMSimpleEvtsDisplay? SimpleEventsDisplay => simpleEventsDisplay;

    /// <summary>
    /// Duration of events in the replay (range of detected inputs). This can be 0 if the replay was driven in editor and null if driven in TMU, TMUF, TMTurbo, TM2 and TM2020.
    /// </summary>
    [NodeMember]
    public int? EventsDuration => eventsDuration;

    /// <summary>
    /// Inputs (keyboard, pad, wheel) of the replay from TM1.0, TMO, Sunrise and ESWC. For inputs stored in TMU, TMUF, TMTurbo and TM2: see <see cref="CGameCtnGhost.ControlEntries"/> in <see cref="Ghosts"/>. TM2020 and Shootmania inputs aren't available in replays and ghosts. Can be null if <see cref="EventsDuration"/> is 0, which can happen when you save the replay in editor.
    /// </summary>
    [NodeMember]
    public ControlEntry[]? ControlEntries => controlEntries;

    [NodeMember]
    public string? Game
    {
        get
        {
            DiscoverChunks<Chunk03093008, Chunk0309300F>();
            return game;
        }
    }

    #endregion

    #region Constructors

    protected CGameCtnReplayRecord()
    {
        
    }

    #endregion

    #region Methods

    public IEnumerable<CGameCtnGhost> GetGhosts()
    {
        if (ghosts is not null)
        {
            foreach (var ghost in ghosts)
            {
                yield return ghost;
            }
        }

        if (clip is not null)
        {
            foreach (var track in clip.Tracks)
            {
                foreach (var block in track.Blocks)
                {
                    if (block is CGameCtnMediaBlockGhost ghostBlock)
                    {
                        yield return ghostBlock.GhostModel;
                    }
                }
            }
        }
    }

    #endregion

    #region Chunks

    #region 0x000 header chunk (basic)

    [Chunk(0x03093000, "basic")]
    public class Chunk03093000 : HeaderChunk<CGameCtnReplayRecord>, IVersionable
    {
        public byte U01;

        public int Version { get; set; }

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            Version = r.ReadInt32();

            if (Version >= 2)
            {
                n.mapInfo = r.ReadIdent();
                n.time = r.ReadTimeInt32Nullable();
                n.playerNickname = r.ReadString();

                if (Version >= 6)
                {
                    n.playerLogin = r.ReadString();

                    if (Version >= 8)
                    {
                        U01 = r.ReadByte();
                        n.titleID = r.ReadId();
                    }
                }
            }
        }
    }

    #endregion

    #region 0x001 header chunk (xml)

    [Chunk(0x03093001, "xml")]
    public class Chunk03093001 : HeaderChunk<CGameCtnReplayRecord>
    {
        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.xml = r.ReadString();
        }
    }

    #endregion

    #region 0x002 header chunk (author)

    [Chunk(0x03093002, "author")]
    public class Chunk03093002H : HeaderChunk<CGameCtnReplayRecord>, IVersionable
    {
        public int Version { get; set; }

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            Version = r.ReadInt32();
            n.authorVersion = r.ReadInt32();
            n.authorLogin = r.ReadString();
            n.authorNickname = r.ReadString();
            n.authorZone = r.ReadString();
            n.authorExtraInfo = r.ReadString();
        }
    }

    #endregion

    #region 0x002 chunk (track)

    [Chunk(0x03093002, "track")]
    public class Chunk03093002B : Chunk<CGameCtnReplayRecord>
    {
        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.challengeData = r.ReadBytes();
        }
    }

    #endregion

    #region 0x003 chunk (validation TM1.0)

    [Chunk(0x03093003, "validation TM1.0")]
    public class Chunk03093003 : Chunk<CGameCtnReplayRecord>
    {
        public int U01;
        public int U02;

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.eventsDuration = r.ReadInt32();

            if (n.eventsDuration == 0)
                return;

            U01 = r.ReadInt32();

            // All control names available in the game
            var controlNames = r.ReadArray(r1 =>
            {
                // Maybe bindings
                r1.ReadInt32();
                r1.ReadInt32();

                return r1.ReadString(); // Input name
            });

            var numEntries = r.ReadInt32() - 1;

            n.controlEntries = new ControlEntry[numEntries];

            for (var i = 0; i < numEntries; i++)
            {
                var time = TimeInt32.FromMilliseconds(r.ReadInt32() - 10000);
                var controlNameIndex = r.ReadInt32();
                var data = r.ReadUInt32();

                var name = controlNames[controlNameIndex];

                n.controlEntries[i] = name switch
                {
                    "Steer (analog)" => new ControlEntryAnalog(name, time, data), // Data is bugged
                    _ => new ControlEntry(name, time, data),
                };
            }

            Array.Reverse(n.controlEntries); // Inputs are originally reversed

            U02 = r.ReadInt32();
        }
    }

    #endregion

    #region 0x004 chunk (ghosts)

    [Chunk(0x03093004, "ghosts")]
    public class Chunk03093004 : Chunk<CGameCtnReplayRecord>, IVersionable
    {
        public int Version { get; set; }

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            Version = r.ReadInt32();
            var u02 = r.ReadInt32();

            n.ghosts = r.ReadArray(r => r.ReadNodeRef<CGameCtnGhost>()!);

            var u03 = r.ReadInt32(); // millisecond length of something (usually record time + 0.5s)
            var u04 = r.ReadInt32();
        }

        public override async Task ReadAsync(CGameCtnReplayRecord n, GameBoxReader r, ILogger? logger, CancellationToken cancellationToken = default)
        {
            Version = r.ReadInt32();
            var u02 = r.ReadInt32();

            n.ghosts = (await r.ReadArrayAsync(r => r.ReadNodeRefAsync<CGameCtnGhost>()!))!;

            var u03 = r.ReadInt32(); // millisecond length of something (usually record time + 0.5s)
            var u04 = r.ReadInt32();
        }
    }

    #endregion

    #region 0x005 chunk

    [Chunk(0x03093005)]
    public class Chunk03093005 : Chunk<CGameCtnReplayRecord>
    {
        public int U01;

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            U01 = r.ReadInt32();
        }
    }

    #endregion

    #region 0x007 skippable chunk

    [Chunk(0x03093007)]
    public class Chunk03093007 : SkippableChunk<CGameCtnReplayRecord>
    {
        public int U01;

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            U01 = r.ReadInt32();
        }
    }

    #endregion

    #region 0x008 skippable chunk (game)

    [Chunk(0x03093008, "game")]
    public class Chunk03093008 : SkippableChunk<CGameCtnReplayRecord>
    {
        public int U01;

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.game = r.ReadString();
            U01 = r.ReadInt32();
        }
    }

    #endregion

    #region 0x00C chunk (clip)

    [Chunk(0x0309300C, "clip")]
    public class Chunk0309300C : Chunk<CGameCtnReplayRecord>
    {
        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.clip = r.ReadNodeRef<CGameCtnMediaClip>();
        }

        public override async Task ReadAsync(CGameCtnReplayRecord n, GameBoxReader r, ILogger? logger, CancellationToken cancellationToken = default)
        {
            n.clip = await r.ReadNodeRefAsync<CGameCtnMediaClip>(cancellationToken);
        }
    }

    #endregion

    #region 0x00D chunk (validation)

    [Chunk(0x0309300D, "validation")]
    public class Chunk0309300D : Chunk<CGameCtnReplayRecord>
    {
        public int U01;
        public int U02;

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.eventsDuration = r.ReadInt32();

            if (n.eventsDuration == 0)
                return;

            U01 = r.ReadInt32();

            var controlNames = r.ReadArray(r1 => r1.ReadId());

            var numEntries = r.ReadInt32();
            U02 = r.ReadInt32();

            n.controlEntries = new ControlEntry[numEntries];

            for (var i = 0; i < numEntries; i++)
            {
                var time = TimeInt32.FromMilliseconds(r.ReadInt32() - 100000);
                var controlNameIndex = r.ReadByte();
                var data = r.ReadUInt32();

                var name = controlNames[controlNameIndex];

                n.controlEntries[i] = (string)name switch
                {
                    "Steer" or "Gas" or "AccelerateReal" or "BrakeReal"
                      => new ControlEntryAnalog(name, time, data),
                    _ => new ControlEntry(name, time, data),
                };
            }
        }
    }

    #endregion

    #region 0x00E chunk (events)

    [Chunk(0x0309300E, "events")]
    public class Chunk0309300E : Chunk<CGameCtnReplayRecord>
    {
        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.events = r.ReadNodeRef<CCtnMediaBlockEventTrackMania>();
        }
    }

    #endregion

    #region 0x00F chunk

    [Chunk(0x0309300F)]
    public class Chunk0309300F : SkippableChunk<CGameCtnReplayRecord>
    {
        public int U01;
        public int U02;
        public int U03;
        public string U04 = string.Empty;

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.game = r.ReadString();
            U01 = r.ReadInt32();
            U02 = r.ReadInt32();
            U03 = r.ReadInt32();
            U04 = r.ReadString();
        }
    }

    #endregion

    #region 0x010 chunk

    [Chunk(0x03093010)]
    public class Chunk03093010 : Chunk<CGameCtnReplayRecord>
    {
        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.simpleEventsDisplay = r.ReadNodeRef<CCtnMediaBlockUiTMSimpleEvtsDisplay>();
        }

        public override async Task ReadAsync(CGameCtnReplayRecord n, GameBoxReader r, ILogger? logger, CancellationToken cancellationToken = default)
        {
            n.simpleEventsDisplay = await r.ReadNodeRefAsync<CCtnMediaBlockUiTMSimpleEvtsDisplay>(cancellationToken);
        }
    }

    #endregion

    #region 0x011 chunk

    [Chunk(0x03093011)]
    public class Chunk03093011 : Chunk<CGameCtnReplayRecord>
    {
        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {

        }
    }

    #endregion

    #region 0x013 skippable chunk

    [Chunk(0x03093013), IgnoreChunk]
    public class Chunk03093013 : SkippableChunk<CGameCtnReplayRecord>
    {

    }

    #endregion

    #region 0x014 chunk (ghosts)

    [Chunk(0x03093014, "ghosts")]
    public class Chunk03093014 : Chunk<CGameCtnReplayRecord>, IVersionable
    {
        public int U01;

        public int Version { get; set; }

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            Version = r.ReadInt32();
            n.ghosts = r.ReadArray(r => r.ReadNodeRef<CGameCtnGhost>()!);
            U01 = r.ReadInt32();
            n.extras = r.ReadArray(r => r.ReadInt64());
        }

        public override async Task ReadAsync(CGameCtnReplayRecord n, GameBoxReader r, ILogger? logger, CancellationToken cancellationToken = default)
        {
            Version = r.ReadInt32();
            n.ghosts = (await r.ReadArrayAsync(r => r.ReadNodeRefAsync<CGameCtnGhost>()!))!;
            U01 = r.ReadInt32();
            n.extras = r.ReadArray(r => r.ReadInt64());
        }
    }

    #endregion

    #region 0x015 chunk (clip)

    [Chunk(0x03093015, "clip")]
    public class Chunk03093015 : Chunk<CGameCtnReplayRecord>
    {
        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.clip = r.ReadNodeRef<CGameCtnMediaClip>();
        }

        public override async Task ReadAsync(CGameCtnReplayRecord n, GameBoxReader r, ILogger? logger, CancellationToken cancellationToken = default)
        {
            n.clip = await r.ReadNodeRefAsync<CGameCtnMediaClip>(cancellationToken);
        }
    }

    #endregion

    #region 0x018 skippable chunk

    [Chunk(0x03093018, "author")]
    public class Chunk03093018 : SkippableChunk<CGameCtnReplayRecord>
    {
        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            n.titleID = r.ReadId();
            n.authorVersion = r.ReadInt32();
            n.authorLogin = r.ReadString();
            n.authorNickname = r.ReadString();
            n.authorZone = r.ReadString();
            n.authorExtraInfo = r.ReadString();
        }
    }

    #endregion

    #region 0x01C skippable chunk

    [Chunk(0x0309301C)]
    public class Chunk0309301C : SkippableChunk<CGameCtnReplayRecord>, IVersionable
    {
        public int Version { get; set; }

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            Version = r.ReadInt32();
            r.ReadString(); // CampaignSolo
        }
    }

    #endregion

    #region 0x024 chunk

    [Chunk(0x03093024)]
    public class Chunk03093024 : Chunk<CGameCtnReplayRecord>
    {
        public int U01;
        public int U02;

        public override void Read(CGameCtnReplayRecord n, GameBoxReader r)
        {
            U01 = r.ReadInt32();
            U02 = r.ReadInt32();
            n.recordData = r.ReadNodeRef<CPlugEntRecordData>();
        }

        public override async Task ReadAsync(CGameCtnReplayRecord n, GameBoxReader r, ILogger? logger, CancellationToken cancellationToken = default)
        {
            U01 = r.ReadInt32();
            U02 = r.ReadInt32();
            n.recordData = await r.ReadNodeRefAsync<CPlugEntRecordData>(cancellationToken);
        }
    }

    #endregion

    #endregion
}
