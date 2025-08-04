using System.IO;
using slocExporter.Readers;
using UnityEngine;

namespace slocExporter.Objects
{

    public sealed class CullingParentObject : slocGameObject
    {

        public CullingParentObject(int instanceId = 0)
            : base(instanceId)
            => Type = ObjectType.CullingParent;

        public Vector3 BoundsSize;

        protected override void WriteData(BinaryWriter writer, slocHeader header)
            => writer.WriteVector(BoundsSize);

    }

}
