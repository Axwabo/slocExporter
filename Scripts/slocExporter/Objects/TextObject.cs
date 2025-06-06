using System;
using System.IO;
using slocExporter.Readers;
using UnityEngine;

namespace slocExporter.Objects
{

    public sealed class TextObject : slocGameObject
    {

        public static Vector2 DefaultDisplaySize => new(200f, 50f);

        public TextObject(string format, int instanceId = 0) : base(instanceId) => Format = format;

        public string Format;

        public string[] Arguments = Array.Empty<string>();

        public Vector2 DisplaySize = DefaultDisplaySize;

        protected override void WriteData(BinaryWriter writer, slocHeader header)
        {
            writer.Write(Format);
            writer.Write(Arguments.Length);
            foreach (var argument in Arguments)
                writer.Write(argument);
            writer.Write(DisplaySize.x);
            writer.Write(DisplaySize.y);
        }

    }

}
