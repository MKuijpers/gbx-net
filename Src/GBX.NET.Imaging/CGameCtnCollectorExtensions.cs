﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using GBX.NET.Engines.GameData;

namespace GBX.NET.Imaging;

/// <summary>
/// Imaging extensions for <see cref="CGameCtnCollector"/>.
/// </summary>
public static class CGameCtnCollectorExtensions
{
    /// <summary>
    /// Gets the collector's icon as <see cref="Bitmap"/>.
    /// </summary>
    /// <param name="node">CGameCtnCollector</param>
    /// <returns>Thumbnail as <see cref="Bitmap"/>. Null if <see cref="CGameCtnCollector.Icon"/> is null.</returns>
    public static Bitmap GetIconBitmap(this CGameCtnCollector node)
    {
        if (node.Icon == null) return null;

        var width = node.Icon.GetLength(0);
        var height = node.Icon.GetLength(1);

        var bitmap = new Bitmap(width, height);
        var bitmapData = bitmap.LockBits(
            new Rectangle(0, 0, width, height),
            ImageLockMode.WriteOnly,
            PixelFormat.Format32bppPArgb);

        var data = new int[width * height];

        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                data[y * width + x] = node.Icon[x, y].ToArgb();
            }
        }

        Marshal.Copy(data, 0, bitmapData.Scan0, data.Length);

        bitmap.UnlockBits(bitmapData);

        return bitmap;
    }

    /// <summary>
    /// Exports the collector's icon.
    /// </summary>
    /// <param name="node">CGameCtnCollector</param>
    /// <param name="stream">Stream to export to.</param>
    /// <param name="format">Image format to use.</param>
    public static void ExportIcon(this CGameCtnCollector node, Stream stream, ImageFormat format)
    {
        var icon = GetIconBitmap(node);
        if (icon != null)
            icon.Save(stream, format);
    }

    /// <summary>
    /// Exports the collector's icon as PNG.
    /// </summary>
    /// <param name="node">CGameCtnCollector</param>
    /// <param name="stream">Stream to export to.</param>
    public static void ExportIcon(this CGameCtnCollector node, Stream stream)
    {
        ExportIcon(node, stream, ImageFormat.Png);
    }

    /// <summary>
    /// Exports the collector's icon.
    /// </summary>
    /// <param name="node">CGameCtnCollector</param>
    /// <param name="fileName">File to export to.</param>
    /// <param name="format">Image format to use.</param>
    public static void ExportIcon(this CGameCtnCollector node, string fileName, ImageFormat format)
    {
        using var fs = File.Create(fileName);
        ExportIcon(node, fs, format);
    }

    /// <summary>
    /// Exports the collector's icon as PNG.
    /// </summary>
    /// <param name="node">CGameCtnCollector</param>
    /// <param name="fileName">File to export to.</param>
    public static void ExportIcon(this CGameCtnCollector node, string fileName)
    {
        ExportIcon(node, fileName, ImageFormat.Png);
    }
}
