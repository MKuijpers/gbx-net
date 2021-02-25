﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices;
using System.Text;

namespace GBX.NET
{
    public class GameBoxWriter : BinaryWriter
    {
        public ILookbackable Lookbackable { get; }
        public Chunk Chunk { get; internal set; }

        public GameBoxWriter(Stream output) : base(output, Encoding.UTF8, true)
        {

        }

        public GameBoxWriter(Stream input, ILookbackable lookbackable) : this(input)
        {
            Lookbackable = lookbackable;
        }

        public GameBoxWriter(Stream input, GameBoxWriter reference) : this(input)
        {
            Lookbackable = reference.Lookbackable;
            Chunk = reference.Chunk;
        }

        public void Write(string value, StringLengthPrefix lengthPrefix)
        {
            if (value == null)
            {
                if (lengthPrefix == StringLengthPrefix.Byte)
                    Write((byte)0);
                else if (lengthPrefix == StringLengthPrefix.Int32)
                    Write(0);
            }
            else
            {
                if (lengthPrefix == StringLengthPrefix.Byte)
                    Write((byte)Encoding.UTF8.GetByteCount(value));
                else if (lengthPrefix == StringLengthPrefix.Int32)
                    Write(Encoding.UTF8.GetByteCount(value));
                var bytes = Encoding.UTF8.GetBytes(value);
                Write(bytes, 0, bytes.Length);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Write(string value)
        {
            Write(value, StringLengthPrefix.Int32);
        }

        public void Write(bool value, bool asByte)
        {
            if (asByte)
                base.Write(value);
            else
                Write(Convert.ToInt32(value));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public override void Write(bool value)
        {
            Write(value, false);
        }

        public void Write<T>(T[] array)
        {
            var bytes = new byte[array.Length * Marshal.SizeOf(default(T))];
            Buffer.BlockCopy(array, 0, bytes, 0, bytes.Length);
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// First writes an <see cref="int"/> representing the length, then does a for loop with this length, each yield having an option to write something from <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of the array.</typeparam>
        /// <param name="array">An array.</param>
        /// <param name="forLoop">Each element.</param>
        public void Write<T>(T[] array, Action<T> forLoop)
        {
            if (array == null)
            {
                Write(0);
            }
            else
            {
                Write(array.Length);

                for (var i = 0; i < array.Length; i++)
                    forLoop.Invoke(array[i]);
            }
        }

        internal void Write<T>(T[] array, Action<T, GameBoxWriter> forLoop)
        {
            if (array == null)
            {
                Write(0);
            }
            else
            {
                Write(array.Length);

                for (var i = 0; i < array.Length; i++)
                    forLoop.Invoke(array[i], this);
            }
        }

        public void Write<T>(List<T> list, Action<T> forLoop)
        {
            if (list == null)
            {
                Write(0);
            }
            else
            {
                Write(list.Count);

                for (var i = 0; i < list.Count; i++)
                    forLoop.Invoke(list[i]);
            }
        }

        public void Write<T>(IEnumerable<T> enumerable, Action<T> forLoop)
        {
            if (enumerable == null)
            {
                Write(0);
            }
            else
            {
                var count = enumerable.Count();

                Write(count);

                IEnumerator<T> enumerator = enumerable.GetEnumerator();

                while (enumerator.MoveNext())
                    forLoop.Invoke(enumerator.Current);
            }
        }

        public void Write<TValue>(Dictionary<int, TValue> dictionary) where TValue : Node
        {
            Write(dictionary.Count);

            foreach(var pair in dictionary)
            {
                Write(pair.Key);
                Write(pair.Value);
            }
        }

        public void Write(Vec2 value)
        {
            Write(new float[] { value.X, value.Y });
        }

        public void Write(Vec3 value)
        {
            Write(new float[] { value.X, value.Y, value.Z });
        }

        public void Write(Vec4 value)
        {
            Write(new float[] { value.X, value.Y, value.Z, value.W });
        }

        public void Write(Int3 value)
        {
            Write(new int[] { value.X, value.Y, value.Z });
        }

        public void Write(Int2 value)
        {
            Write(new int[] { value.X, value.Y });
        }

        public void Write(Byte3 value)
        {
            var bytes = new byte[] { value.X, value.Y, value.Z };
            Write(bytes, 0, bytes.Length);
        }

        public void Write(FileRef fileRef)
        {
            Write(fileRef.Version);

            if (fileRef.Version >= 3)
                Write(fileRef.Checksum, 0, fileRef.Checksum.Length);

            Write(fileRef.FilePath);

            if ((fileRef.FilePath.Length > 0 && fileRef.Version >= 1) || fileRef.Version >= 3)
                Write(fileRef.LocatorUrl.ToString());
        }

        public void Write(Id value)
        {
            var l = value.Owner;

            if (!l.IdWritten)
            {
                if (l.IdVersion.HasValue)
                    Write(l.IdVersion.Value);
                else Write(3);
                l.IdWritten = true;
            }

            if (value == "Unassigned")
                Write(0xBFFFFFFF);
            else if (value == "")
                Write(0xFFFFFFFF);
            else if (l.IdStrings.Contains(value))
                Write(value.Index + 1 + 0x40000000);
            else if (int.TryParse(value, out int cID) && Id.CollectionIDs.ContainsKey(cID))
                Write(cID);
            else
            {
                Write(0x40000000);
                Write(value.ToString());
                l.IdStrings.Add(value);
            }
        }

        public void WriteId(string value)
        {
            Write(new Id(value, Lookbackable));
        }

        public void Write(Ident ident, ILookbackable lookbackable)
        {
            Write(new Id(ident.ID, lookbackable));
            Write(ident.Collection.ToId(lookbackable));
            Write(new Id(ident.Author, lookbackable));
        }

        public void Write(Ident ident)
        {
            Write(ident, Lookbackable);
        }

        public void Write(Node node, IGameBoxBody body)
        {
            if (node == null)
                Write(-1);
            else
            {
                if (body.AuxilaryNodes.ContainsValue(node))
                {
                    Write(body.AuxilaryNodes.FirstOrDefault(x => x.Equals(node)).Key);
                }
                else
                {
                    body.AuxilaryNodes[body.AuxilaryNodes.Count] = node;
                    Write(body.AuxilaryNodes.Count);
                    Write(node.ID);
                    node.Write(this, body.GBX.Game);
                }
            }
        }

        public void Write(Node node)
        {
            Write(node, (IGameBoxBody)Lookbackable);
        }

        public void Write(TimeSpan? timeSpan)
        {
            if (timeSpan.HasValue)
                Write(Convert.ToInt32(timeSpan.Value.TotalMilliseconds));
            else Write(-1);
        }

        public void WriteBytes(byte[] bytes)
        {
            Write(bytes, 0, bytes.Length);
        }

        /// <summary>
        /// Writes the node array that are presented directly and not as a node reference.
        /// </summary>
        /// <typeparam name="T">Type of the node.</typeparam>
        /// <param name="nodes">Node array.</param>
        public void WriteNodes<T>(IEnumerable<T> nodes) where T : Node
        {
            var watch = Stopwatch.StartNew();

            var count = nodes.Count();

            var nodeType = typeof(T);

            Write(count);

            var counter = 0;

            foreach (var node in nodes)
            {
                Write(node.ID);
                node.Write(this);

                string logProgress = $"[{nodeType.FullName.Substring("GBX.NET.Engines".Length + 1).Replace(".", "::")}] {counter + 1}/{count} ({watch.Elapsed.TotalMilliseconds}ms)";
                if (Chunk.Part == null || !Chunk.Part.GBX.ClassID.HasValue || Node.Remap(Chunk.Part.GBX.ClassID.Value) != node.ID)
                    logProgress = "~ " + logProgress;

                Log.Write(logProgress, ConsoleColor.Magenta);

                if (counter != count - 1)
                    Log.Push(node.Chunks.Count + 2);

                counter += 1;
            }
        }
    }
}
