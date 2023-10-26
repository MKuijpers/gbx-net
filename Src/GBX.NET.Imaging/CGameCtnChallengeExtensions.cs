﻿using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.Versioning;
using GBX.NET.Engines.Game;

namespace GBX.NET.Imaging;

/// <summary>
/// Imaging extensions for <see cref="CGameCtnChallenge"/>.
/// </summary>
#if NET6_0_OR_GREATER
[SupportedOSPlatform("windows")]
#endif
public static class CGameCtnChallengeExtensions
{
    /// <summary>
    /// Gets the map thumbnail as <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="node">CGameCtnChallenge</param>
    /// <returns>Thumbnail as <see cref="Bitmap"/>.</returns>
    public static Bitmap? GetThumbnailBitmap(this CGameCtnChallenge node)
    {
        if (node.Thumbnail is null)
        {
            return null;
        }

        using var ms = new MemoryStream(node.Thumbnail);
        var bitmap = (Bitmap)Image.FromStream(ms);
        bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);
        return bitmap;
    }

    /// <summary>
    /// Exports the map's thumbnail.
    /// </summary>
    /// <param name="node">CGameCtnChallenge</param>
    /// <param name="stream">Stream to export to.</param>
    /// <param name="format">Image format to use.</param>
    public static void ExportThumbnail(this CGameCtnChallenge node, Stream stream, ImageFormat format)
    {
        var thumbnail = GetThumbnailBitmap(node);

        if (thumbnail is null)
        {
            return;
        }

        if (format == ImageFormat.Jpeg)
        {
            SaveAsJpeg(stream, thumbnail);
        }
        else
        {
            thumbnail.Save(stream, format);
        }
    }

    private static void SaveAsJpeg(Stream stream, Bitmap thumbnail)
    {
        var encoding = new EncoderParameters(1);
        encoding.Param[0] = new EncoderParameter(Encoder.Quality, 90L);
        var encoder = ImageCodecInfo.GetImageDecoders().Where(x => x.FormatID == ImageFormat.Jpeg.Guid).First();

        thumbnail.Save(stream, encoder, encoding);
    }

    /// <summary>
    /// Exports the map's thumbnail.
    /// </summary>
    /// <param name="node">CGameCtnChallenge</param>
    /// <param name="fileName">File to export to.</param>
    /// <param name="format">Image format to use.</param>
    public static void ExportThumbnail(this CGameCtnChallenge node, string fileName, ImageFormat format)
    {
        using var fs = File.Create(fileName);
        ExportThumbnail(node, fs, format);
    }

    /// <summary>
    /// Replaces a thumbnail (any popular image format) to use for the map.
    /// </summary>
    /// <param name="node">CGameCtnChallenge</param>
    /// <param name="stream">Stream to import from.</param>
    public static Bitmap ImportThumbnail(this CGameCtnChallenge node, Stream stream)
    {
        var bitmap = new Bitmap(stream);
        bitmap.RotateFlip(RotateFlipType.Rotate180FlipX);

        using var ms = new MemoryStream();
        
        SaveAsJpeg(ms, bitmap);
        
        node.Thumbnail = ms.ToArray();

        return bitmap;
    }

    /// <summary>
    /// Replaces a thumbnail (any popular image format) to use for the map.
    /// </summary>
    /// <param name="node">CGameCtnChallenge</param>
    /// <param name="fileName">File to import from.</param>
    public static Bitmap ImportThumbnail(this CGameCtnChallenge node, string fileName)
    {
        using var fs = File.OpenRead(fileName);
        return ImportThumbnail(node, fs);
    }
}
