﻿using System.Collections;
using System.Text;

namespace GBX.NET.Engines.Game;

//
// Massive thanks to Mystixor and Shweetz for many of the findings!!!
// Without these guys, this would've stayed a mystery.
//

public partial class CGameCtnGhost
{
    /// <summary>
    /// Set of inputs.
    /// </summary>
    /// <remarks>Massive thanks to Mystixor and Shweetz for many of the findings!!!</remarks>
    public class PlayerInputData : IReadableWritable
    {
        public enum EVersion
        {
            _2017_07_07 = 7,
            _2017_09_12 = 8,
            _2020_04_08 = 11,
            _2020_07_20 = 12
        }

        private EVersion version; // 8 in shootmania, 12 in tm2020
        private int u02;
        private TimeInt32? startOffset;
        private int ticks;
        private byte[]? data;

        public EVersion Version { get => version; set => version = value; }
        public int U02 { get => u02; set => u02 = value; }
        public TimeInt32? StartOffset { get => startOffset; set => startOffset = value; }
        public int Ticks { get => ticks; set => ticks = value; }
        public byte[]? Data { get => data; set => data = value; }
        public IList<InputChange>? InputChanges { get; set; }

        public void ReadWrite(GameBoxReaderWriter rw, int version = 0)
        {
            rw.EnumInt32<EVersion>(ref this.version); // 8 in shootmania, 12 in tm2020
            rw.Int32(ref u02);

            if (version >= 4)
            {
                rw.TimeInt32Nullable(ref startOffset);
            }

            rw.Int32(ref ticks);
            rw.Bytes(ref data);

            if (rw.Reader is null || data is null)
            {
                return;
            }
            
            var bits = new BitArray(data);
            InputChanges = new List<InputChange>();

            /*var builder = new StringBuilder();
            for(var i = 0; i < bits.Length; i++)
            {
                if (i != 0 && i % 8 == 0)
                {
                    builder.Append(' ');
                }
                if (i != 0 && i % 32 == 0)
                {
                    builder.AppendLine();
                }
                builder.Append(bits[i] ? "1" : "0");
            }
            Console.WriteLine(builder.ToString());*/

            var position = 0;

            while (bits.Get(position))
            {
                position++;
            }

            var padding = position;

            for (var i = 0; i < ticks; i++)
            {
                var somethingHasChanged = true;
                var steer = default(sbyte);
                var gas = false;
                var brake = false;
                var zeroBitSet = false;

                for (var bit = 0; bit < 13; bit++)
                {
                    if (position >= bits.Length) // Happens often with padding > 0, dunno why
                    {
                        break;
                    }

                    var bitSet = bits.Get(position);
                    position++;

                    if (bit == 0)
                    {
                        zeroBitSet = bitSet;

                        if (!zeroBitSet)
                        {
                            position += 35;
                            break;
                        }
                    }
                    
                    if (bit == 1 && bitSet && zeroBitSet)
                    {
                        position++;
                        somethingHasChanged = false;
                        break; // nothing has changed
                    }

                    if (bit >= 2 && bit <= 9 && bitSet)
                    {
                        steer |= (sbyte)(1 << (bit - 2));
                    }

                    if (bit == 10)
                    {
                        gas = bitSet;
                    }
                    
                    if (bit == 11)
                    {
                        brake = bitSet;
                    }
                }

                if (somethingHasChanged)
                {
                    InputChanges.Add(new(TimeInt32.FromMilliseconds(i * 10), steer, gas, brake));
                }
            }
        }

        public record struct InputChange(TimeInt32 Timestamp, sbyte Steer, bool Gas, bool Brake);
    }
}
