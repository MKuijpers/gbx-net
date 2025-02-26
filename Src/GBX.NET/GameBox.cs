﻿using GBX.NET.Debugging;
using System.Diagnostics;
using System.Reflection;

namespace GBX.NET;

/// <summary>
/// An unknown serialized GameBox node with additional attributes. This class can represent deserialized .Gbx file.
/// </summary>
public partial class GameBox : IDisposable
{
    public const string Magic = "GBX";

    private readonly Header header;
    private readonly RefTable? refTable;

    /// <summary>
    /// If specialized actions should be executed that can help further with debugging but slow down the parse speed. Options can be then visible inside Debugger properties if available.
    /// </summary>
    public static bool Debug { get; set; }

    /// <summary>
    /// If to ignore exceptions on certain chunk versions that are unknown, but shouldn't crash the reading/writing, however, could return unexpected values.
    /// </summary>
    /// <remarks>Example where this could happen is <see cref="CGameCtnMediaBlockCameraCustom.Key.ReadWrite(GameBoxReaderWriter, int)"/>.</remarks>
    public static bool IgnoreUnseenVersions { get; set; }

    public Node? Node { get; private set; }
    public Body? RawBody { get; private set; }
    public GameBoxBodyDebugger? Debugger { get; private set; }

    /// <summary>
    /// File path of the GameBox.
    /// </summary>
    public string? FileName { get; }

    /// <summary>
    /// Creates a new GameBox object with the most common parameters without the node (object).
    /// </summary>
    /// <param name="id">ID of the expected node - should be provided remapped to latest.</param>
    public GameBox(uint id)
    {
        header = new Header(id);
    }

    /// <summary>
    /// Creates a new GameBox object with the most common parameters.
    /// </summary>
    /// <param name="node">Node to wrap in.</param>
    public GameBox(Node node)
    {
        header = new Header(node.Id);
        Node = node;
        Node.SetGbx(this);
    }

    public GameBox(Header header, RefTable? refTable, string? fileName = null)
    {
        this.header = header;
        this.refTable = refTable;

        FileName = fileName;
    }

    public Header GetHeader()
    {
        return header;
    }

    public RefTable? GetRefTable()
    {
        return refTable;
    }

    /// <summary>
    /// Tries to get the <see cref="Node"/> of this GBX.
    /// </summary>
    /// <typeparam name="T">Type of the <see cref="Node"/> to look for.</typeparam>
    /// <param name="node">A node that is being extracted from this <see cref="GameBox"/> object. Null if unsuccessful.</param>
    /// <returns>True if the type of this <see cref="GameBox"/> is <see cref="GameBox{T}"/> and <typeparamref name="T"/> matches. Otherwise false.</returns>
    public bool TryNode<T>(out T? node) where T : Node
    {
        var property = GetType().GetProperty(nameof(Node));

        if (property?.PropertyType == typeof(T))
        {
            node = property.GetValue(this) as T;
            return true;
        }

        node = null;
        return false;
    }

    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
    /// <exception cref="MissingLzoException"></exception>
    /// <exception cref="HeaderOnlyParseLimitationException">Writing is not supported in <see cref="GameBox"/> where only the header was parsed (without raw body being read).</exception>
    internal void Write(Stream stream, IDRemap remap, ILogger? logger)
    {
        var stateGuid = StateManager.Shared.CreateState(refTable);

        logger?.LogDebug("Writing the body...");

        using var ms = new MemoryStream();
        using var bodyW = new GameBoxWriter(ms, stateGuid, remap, logger: logger);

        (RawBody ?? new Body()).Write(this, bodyW, logger);

        logger?.LogDebug("Writing the header...");

        StateManager.Shared.ResetIdState(stateGuid);

        using var headerW = new GameBoxWriter(stream, stateGuid, remap, logger: logger);

        if (RawBody is null)
        {
            header.Write(Node!, headerW, StateManager.Shared.GetNodeCount(stateGuid) + 1, logger);
        }
        else
        {
            header.Write(Node!, headerW, header.NumNodes, logger);
        }

        logger?.LogDebug("Writing the reference table...");

        if (refTable is null)
        {
            headerW.Write(0);
        }
        else
        {
            refTable.Write(header, headerW);
        }

        headerW.WriteBytes(ms.ToArray());

        StateManager.Shared.RemoveState(stateGuid);
    }

    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
    /// <exception cref="MissingLzoException"></exception>
    /// <exception cref="HeaderOnlyParseLimitationException">Writing is not supported in <see cref="GameBox"/> where only the header was parsed (without raw body being read).</exception>
    internal async Task WriteAsync(Stream stream, IDRemap remap, ILogger? logger, GameBoxAsyncWriteAction? asyncAction, CancellationToken cancellationToken)
    {
        var stateGuid = StateManager.Shared.CreateState(refTable);

        logger?.LogDebug("Writing the body...");

        using var ms = new MemoryStream();
        using var bodyW = new GameBoxWriter(ms, stateGuid, remap, asyncAction, logger);

        await (RawBody ?? new Body()).WriteAsync(this, bodyW, logger, cancellationToken);

        logger?.LogDebug("Writing the header...");

        StateManager.Shared.ResetIdState(stateGuid);

        using var headerW = new GameBoxWriter(stream, stateGuid, remap, logger: logger);

        if (RawBody is null)
        {
            header.Write(Node!, headerW, StateManager.Shared.GetNodeCount(stateGuid) + 1, logger);
        }
        else
        {
            header.Write(Node!, headerW, header.NumNodes, logger);
        }

        logger?.LogDebug("Writing the reference table...");

        if (refTable is null)
            headerW.Write(0);
        else
            refTable.Write(header, headerW);

        headerW.WriteBytes(ms.ToArray());

        StateManager.Shared.RemoveState(stateGuid);
    }

    /// <summary>
    /// Saves the serialized <see cref="GameBox{T}"/> to a stream.
    /// </summary>
    /// <param name="stream">Any kind of stream that supports writing.</param>
    /// <param name="remap">What to remap the newest node IDs to. Used for older games.</param>
    /// <param name="logger">Logger.</param>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
    /// <exception cref="MissingLzoException"></exception>
    /// <exception cref="HeaderOnlyParseLimitationException">Saving is not supported in <see cref="GameBox"/> where only the header was parsed (without raw body being read).</exception>
    public void Save(Stream stream, IDRemap remap = default, ILogger? logger = null)
    {
        Write(stream, remap, logger);
    }

    /// <summary>
    /// Saves the serialized <see cref="GameBox{T}"/> on a disk.
    /// </summary>
    /// <param name="fileName">Relative or absolute file path. Null will pick the <see cref="GameBox.FileName"/> value instead.</param>
    /// <param name="remap">What to remap the newest node IDs to. Used for older games.</param>
    /// <param name="logger">Logger.</param>
    /// <exception cref="PropertyNullException"><see cref="GameBox.FileName"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by System.IO.Path.InvalidPathChars.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="fileName"/> specified a file that is read-only. -or- <paramref name="fileName"/> specified a file that is hidden. -or- This operation is not supported on the current platform. -or- <paramref name="fileName"/> specified a directory. -or- The caller does not have the required permission.</exception>
    /// <exception cref="NotSupportedException"><paramref name="fileName"/> is in an invalid format.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
    /// <exception cref="MissingLzoException"></exception>
    /// <exception cref="HeaderOnlyParseLimitationException">Saving is not supported in <see cref="GameBox"/> where only the header was parsed (without raw body being read).</exception>
    public void Save(string? fileName = default, IDRemap remap = default, ILogger? logger = null)
    {
        fileName ??= (FileName ?? throw new PropertyNullException(nameof(FileName)));

        using var fs = File.Create(fileName);

        Save(fs, remap);

        logger?.LogDebug("GBX file {fileName} saved.", fileName);
    }

    /// <summary>
    /// Saves the serialized <see cref="GameBox{T}"/> to a stream.
    /// </summary>
    /// <param name="stream">Any kind of stream that supports writing.</param>
    /// <param name="remap">What to remap the newest node IDs to. Used for older games.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="asyncAction">Specialized executions during asynchronous writing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
    /// <exception cref="MissingLzoException"></exception>
    /// <exception cref="HeaderOnlyParseLimitationException">Saving is not supported in <see cref="GameBox"/> where only the header was parsed (without raw body being read).</exception>
    public async Task SaveAsync(Stream stream, IDRemap remap = default, ILogger? logger = null, GameBoxAsyncWriteAction? asyncAction = null, CancellationToken cancellationToken = default)
    {
        await WriteAsync(stream, remap, logger, asyncAction, cancellationToken);
    }

    /// <summary>
    /// Saves the serialized <see cref="GameBox{T}"/> on a disk.
    /// </summary>
    /// <param name="fileName">Relative or absolute file path. Null will pick the <see cref="FileName"/> value instead.</param>
    /// <param name="remap">What to remap the newest node IDs to. Used for older games.</param>
    /// <param name="logger">Logger.</param>
    /// <param name="asyncAction">Specialized executions during asynchronous writing.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <exception cref="PropertyNullException"><see cref="FileName"/> is null.</exception>
    /// <exception cref="ArgumentException"><paramref name="fileName"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by System.IO.Path.InvalidPathChars.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="fileName"/> is null.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid (for example, it is on an unmapped drive).</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="fileName"/> specified a file that is read-only. -or- <paramref name="fileName"/> specified a file that is hidden. -or- This operation is not supported on the current platform. -or- <paramref name="fileName"/> specified a directory. -or- The caller does not have the required permission.</exception>
    /// <exception cref="NotSupportedException"><paramref name="fileName"/> is in an invalid format.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="ObjectDisposedException">The stream is closed.</exception>
    /// <exception cref="MissingLzoException"></exception>
    /// <exception cref="HeaderOnlyParseLimitationException">Saving is not supported in <see cref="GameBox"/> where only the header was parsed (without raw body being read).</exception>
    public async Task SaveAsync(string? fileName = default, IDRemap remap = default, ILogger? logger = null, GameBoxAsyncWriteAction? asyncAction = null, CancellationToken cancellationToken = default)
    {
        fileName ??= (FileName ?? throw new PropertyNullException(nameof(FileName)));

        using var fs = File.Create(fileName);

        await SaveAsync(fs, remap, logger, asyncAction, cancellationToken);

        logger?.LogDebug("GBX file {fileName} saved.", fileName);
    }

    /// <summary>
    /// Implicitly casts <see cref="GameBox"/> to its <see cref="Node"/>.
    /// </summary>
    /// <param name="gbx"></param>
    public static implicit operator Node?(GameBox gbx) => gbx.Node;

    private static uint? ReadNodeID(GameBoxReader reader)
    {
        if (!reader.HasMagic(Magic)) // If the file doesn't have GBX magic
            return null;

        var version = reader.ReadInt16(); // Version

        if (version < 3)
            return null;

        reader.ReadBytes(3);

        if (version >= 4)
            reader.ReadByte();

        return reader.ReadUInt32();
    }

    /// <summary>
    /// Reads the GBX node ID the quickest possible way.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <returns>ID of the main node presented in GBX. Null if not a GBX stream.</returns>
    public static uint? ReadNodeID(Stream stream)
    {
        using var r = new GameBoxReader(stream);
        return ReadNodeID(r);
    }

    /// <summary>
    /// Reads the GBX node ID the quickest possible way.
    /// </summary>
    /// <param name="fileName">File to read from.</param>
    /// <returns>ID of the main node presented in GBX. Null if not a GBX stream.</returns>
    public static uint? ReadNodeID(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return ReadNodeID(fs);
    }

    /// <summary>
    /// Reads the type of the main node from GBX file.
    /// </summary>
    /// <param name="fileName">File to read from.</param>
    /// <returns>Type of the main node.</returns>
    public static Type? ReadNodeType(string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return ReadNodeType(fs);
    }

    /// <summary>
    /// Reads the type of the main node from GBX stream.
    /// </summary>
    /// <param name="stream">Stream to read from.</param>
    /// <returns>Type of the main node.</returns>
    public static Type? ReadNodeType(Stream stream)
    {
        using var r = new GameBoxReader(stream);
        return ReadNodeType(r);
    }

    private static Type? ReadNodeType(GameBoxReader reader)
    {
        var classID = ReadNodeID(reader);

        if (!classID.HasValue)
            return null;

        var modernID = classID.GetValueOrDefault();
        if (NodeCacheManager.Mappings.TryGetValue(classID.GetValueOrDefault(), out uint newerClassID))
            modernID = newerClassID;

        System.Diagnostics.Debug.WriteLine("GetGameBoxType: " + modernID.ToString("x8"));

        // This should be optimized
        var availableClass = Assembly.GetExecutingAssembly().GetTypes().Where(x => x.IsClass
                && x.Namespace?.StartsWith("GBX.NET.Engines") == true && x.IsSubclassOf(typeof(CMwNod))
                && x.GetCustomAttribute<NodeAttribute>()?.ID == modernID).FirstOrDefault();

        if (availableClass is null)
            return null;

        return typeof(GameBox<>).MakeGenericType(availableClass);
    }

    /// <summary>
    /// Decompresses the body part of the GBX file, also setting the header parameter so that the outputted GBX file is compatible with the game. If the file is already detected decompressed, the input is just copied over to the output.
    /// </summary>
    /// <param name="input">GBX stream to decompress.</param>
    /// <param name="output">Output GBX stream in the decompressed form.</param>
    /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
    /// <exception cref="ObjectDisposedException">One of the streams is closed.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="VersionNotSupportedException">GBX files below version 3 are not supported.</exception>
    /// <exception cref="TextFormatNotSupportedException">Text-formatted GBX files are not supported.</exception>
    public static void Decompress(Stream input, Stream output)
    {
        using var r = new GameBoxReader(input);
        using var w = new GameBoxWriter(output);

        var version = CopyBasicInformation(r, w);

        // Body compression type
        var compressedBody = r.ReadByte();

        if (compressedBody != 'C')
        {
            w.Write(compressedBody);
            input.CopyTo(output);
            return;
        }

        w.Write('U');

        // Unknown byte
        if (version >= 4)
            w.Write(r.ReadByte());

        // Id
        w.Write(r.ReadInt32());

        // User data
        if (version >= 6)
        {
            var bytes = r.ReadBytes();
            w.Write(bytes.Length);
            w.WriteBytes(bytes);
        }

        // Num nodes
        w.Write(r.ReadInt32());

        var numExternalNodes = r.ReadInt32();

        if (numExternalNodes > 0)
            throw new Exception(); // Ref table, TODO: full read

        w.Write(numExternalNodes);

        var uncompressedSize = r.ReadInt32();
        var compressedData = r.ReadBytes();

        var buffer = new byte[uncompressedSize];
        Lzo.Decompress(compressedData, buffer);
        w.WriteBytes(buffer);
    }

    /// <summary>
    /// Decompresses the body part of the GBX file, also setting the header parameter so that the outputted GBX file is compatible with the game. If the file is already detected decompressed, the input is just copied over to the output.
    /// </summary>
    /// <param name="inputFileName">GBX file to decompress.</param>
    /// <param name="output">Output GBX stream in the decompressed form.</param>
    /// <exception cref="ArgumentException"><paramref name="inputFileName"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.InvalidPathChars"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="inputFileName"/> is null.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="inputFileName"/> specified a directory. -or- The caller does not have the required permission.</exception>
    /// <exception cref="FileNotFoundException">The file specified in path was not found.</exception>
    /// <exception cref="NotSupportedException"><paramref name="inputFileName"/> is in an invalid format.</exception>
    /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
    /// <exception cref="ObjectDisposedException">One of the streams is closed.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="VersionNotSupportedException">GBX files below version 3 are not supported.</exception>
    /// <exception cref="TextFormatNotSupportedException">Text-formatted GBX files are not supported.</exception>
    public static void Decompress(string inputFileName, Stream output)
    {
        using var fs = File.OpenRead(inputFileName);
        Decompress(fs, output);
    }

    /// <summary>
    /// Decompresses the body part of the GBX file, also setting the header parameter so that the outputted GBX file is compatible with the game. If the file is already detected decompressed, the input is just copied over to the output.
    /// </summary>
    /// <param name="input">GBX stream to decompress.</param>
    /// <param name="outputFileName">Output GBX file in the decompressed form.</param>
    /// <exception cref="ArgumentException"><paramref name="outputFileName"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.InvalidPathChars"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="outputFileName"/> is null.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
    /// <exception cref="UnauthorizedAccessException">The caller does not have the required permission. -or- path specified a file that is read-only.</exception>
    /// <exception cref="NotSupportedException"><paramref name="outputFileName"/> is in an invalid format.</exception>
    /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
    /// <exception cref="ObjectDisposedException">One of the streams is closed.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="VersionNotSupportedException">GBX files below version 3 are not supported.</exception>
    /// <exception cref="TextFormatNotSupportedException">Text-formatted GBX files are not supported.</exception>
    public static void Decompress(Stream input, string outputFileName)
    {
        using var fs = File.Create(outputFileName);
        Decompress(input, fs);
    }

    /// <summary>
    /// Decompresses the body part of the GBX file, also setting the header parameter so that the outputted GBX file is compatible with the game. If the file is already detected decompressed, the input is just copied over to the output.
    /// </summary>
    /// <param name="inputFileName">GBX file to decompress.</param>
    /// <param name="outputFileName">Output GBX file in the decompressed form.</param>
    /// <exception cref="ArgumentException"><paramref name="inputFileName"/> or <paramref name="outputFileName"/> is a zero-length string, contains only white space, or contains one or more invalid characters as defined by <see cref="Path.InvalidPathChars"/>.</exception>
    /// <exception cref="ArgumentNullException"><paramref name="inputFileName"/> or <paramref name="outputFileName"/> is null.</exception>
    /// <exception cref="PathTooLongException">The specified path, file name, or both exceed the system-defined maximum length.</exception>
    /// <exception cref="DirectoryNotFoundException">The specified path is invalid, (for example, it is on an unmapped drive).</exception>
    /// <exception cref="UnauthorizedAccessException"><paramref name="inputFileName"/> or <paramref name="outputFileName"/> specified a directory. -or- The caller does not have the required permission.  -or- path specified a file that is read-only.</exception>
    /// <exception cref="FileNotFoundException">The file specified in path was not found.</exception>
    /// <exception cref="NotSupportedException"><paramref name="inputFileName"/> is in an invalid format.</exception>
    /// <exception cref="EndOfStreamException">The end of the stream is reached.</exception>
    /// <exception cref="ObjectDisposedException">One of the streams is closed.</exception>
    /// <exception cref="IOException">An I/O error occurs.</exception>
    /// <exception cref="VersionNotSupportedException">GBX files below version 3 are not supported.</exception>
    /// <exception cref="TextFormatNotSupportedException">Text-formatted GBX files are not supported.</exception>
    public static void Decompress(string inputFileName, string outputFileName)
    {
        using var fsInput = File.OpenRead(inputFileName);
        using var fsOutput = File.Create(outputFileName);
        Decompress(fsInput, fsOutput);
    }

    public static void Compress(Stream input, Stream output)
    {
        using var r = new GameBoxReader(input);
        using var w = new GameBoxWriter(output);

        var version = CopyBasicInformation(r, w);

        // Body compression type
        var compressedBody = r.ReadByte();

        if (compressedBody != 'U')
        {
            input.CopyTo(output);
            return;
        }

        w.Write('C');

        // Unknown byte
        if (version >= 4)
        {
            w.Write(r.ReadByte());
        }

        // Id
        w.Write(r.ReadInt32());

        // User data
        if (version >= 6)
        {
            var bytes = r.ReadBytes();
            w.Write(bytes.Length);
            w.WriteBytes(bytes);
        }

        // Num nodes
        w.Write(r.ReadInt32());

        var numExternalNodes = r.ReadInt32();

        if (numExternalNodes > 0)
        {
            throw new Exception(); // Ref table, TODO: full read
        }

        w.Write(numExternalNodes);

        var uncompressedData = r.ReadToEnd();
        var compressedData = Lzo.Compress(uncompressedData);

        w.Write(uncompressedData.Length);
        w.Write(compressedData.Length);
        w.WriteBytes(compressedData);
    }

    private static short CopyBasicInformation(GameBoxReader r, GameBoxWriter w)
    {
        // Magic
        if (!r.HasMagic(Magic))
            throw new Exception();

        w.Write(Magic, StringLengthPrefix.None);

        // Version
        var version = r.ReadInt16();

        if (version < 3)
        {
            throw new VersionNotSupportedException(version);
        }

        w.Write(version);

        // Format
        var format = r.ReadByte();

        if (format != 'B')
        {
            throw new TextFormatNotSupportedException();
        }

        w.Write(format);

        // Ref table compression
        w.Write(r.ReadByte());

        return version;
    }

    public void Dispose()
    {
        Node?.Dispose();
    }
}
