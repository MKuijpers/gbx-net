﻿using GBX.NET;
using System.Collections.Immutable;

namespace GbxExplorer.Client.Models;

public class GbxModel : GbxModelBase
{
    public GameBox Gbx { get; }
    public string FileNameWithoutExtension { get; set; }
    public string? OfficialExtension { get; set; }

    public ImmutableArray<TypeModel>? Inheritance { get; }

    public GbxModel(string fileName, DateTimeOffset lastModified, byte[] pureData, string sha256, GameBox gbx)
        : base(fileName, lastModified, pureData, sha256)
    {
        Gbx = gbx;
        FileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);

        if (NodeManager.GetGbxExtensions(gbx.Header.Id).Any())
        {
            FileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileNameWithoutExtension);
            OfficialExtension = GetNodeExtension(fileName);
        }

        if (gbx.Node is not null)
        {
            Inheritance = GetInheritance(gbx.Node).Select(x => new TypeModel(x)).ToImmutableArray();
        }
    }

    private static string? GetNodeExtension(string fileName)
    {
        var extensionSplit = fileName.Split('.');

        if (extensionSplit.Length < 3)
        {
            return null;
        }

        var extensionWithSpaces = extensionSplit[extensionSplit.Length - 2];
        var indexOfFirstSpace = extensionWithSpaces.IndexOf(' ');

        if (indexOfFirstSpace == -1)
        {
            return extensionWithSpaces;
        }

        return extensionWithSpaces.Substring(0, indexOfFirstSpace);
    }

    private static IEnumerable<Type> GetInheritance(Node node)
    {
        var type = node.GetType();

        while (type is not null && type != typeof(Node))
        {
            yield return type;
            type = type.BaseType;
        }
    }
}
