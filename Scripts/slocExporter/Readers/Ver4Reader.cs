using System.IO;
using slocExporter.Objects;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using UnityEngine.Pool;

namespace slocExporter.Readers {

    public sealed class Ver4Reader : IObjectReader {

        public slocHeader ReadHeader(BinaryReader stream) => Ver3Reader.ReadHeaderStatic(stream, 4);

        public slocGameObject Read(BinaryReader stream, slocHeader header) {
            var objectType = (ObjectType) stream.ReadByte();
            return objectType switch {
                ObjectType.Cube
                    or ObjectType.Sphere
                    or ObjectType.Cylinder
                    or ObjectType.Plane
                    or ObjectType.Capsule
                    or ObjectType.Quad => ReadPrimitive(stream, objectType, header),
                ObjectType.Light => Ver3Reader.ReadLight(stream, header),
                ObjectType.Empty => Ver2Reader.ReadEmpty(stream),
                _ => null
            };
        }

        public static slocGameObject ReadPrimitive(BinaryReader stream, ObjectType type, slocHeader header) {
            var primitive = Ver3Reader.ReadPrimitive(stream, type, header);
            var reader = ActionManager.GetReader(header.Version);
            var actionCount = stream.ReadInt32();
            var actions = ListPool<BaseTriggerActionData>.Get();
            for (var i = 0; i < actionCount; i++) {
                var action = reader.Read(stream);
                if (action != null)
                    actions.Add(action);
            }

            primitive.TriggerActions = actions.ToArray();
            ListPool<BaseTriggerActionData>.Release(actions);
            return primitive;
        }

    }

}
