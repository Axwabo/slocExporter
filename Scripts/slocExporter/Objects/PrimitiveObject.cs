using System;
using System.IO;
using slocExporter.Readers;
using slocExporter.TriggerActions;
using slocExporter.TriggerActions.Data;
using UnityEngine;

namespace slocExporter.Objects {

    public sealed class PrimitiveObject : slocGameObject {

        public PrimitiveObject(int instanceId, ObjectType type) : base(instanceId) {
            if (type is ObjectType.None or ObjectType.Light)
                throw new ArgumentException("Invalid primitive type", nameof(type));
            Type = type;
        }

        public Color MaterialColor = Color.gray;

        public ColliderCreationMode ColliderMode = ColliderCreationMode.Both;

        public BaseTriggerActionData[] TriggerActions = Array.Empty<BaseTriggerActionData>();

        public ColliderCreationMode GetNonUnsetColliderMode() => ColliderMode is ColliderCreationMode.Unset ? ColliderCreationMode.Both : ColliderMode;

        protected override void WriteData(BinaryWriter writer, slocHeader header) {
            if (header.HasAttribute(slocAttributes.LossyColors)) {
                writer.Write(MaterialColor.ToLossyColor());
                writer.Write((byte) ColliderMode);
                return;
            }

            writer.Write(MaterialColor.r);
            writer.Write(MaterialColor.g);
            writer.Write(MaterialColor.b);
            writer.Write(MaterialColor.a);
            writer.Write((byte) ColliderMode);
            if (ColliderMode.IsTrigger())
                ActionManager.WriteActions(writer, TriggerActions);
        }

        public enum ColliderCreationMode : byte {

            Unset = 0,
            NoCollider = 1,
            ClientOnly = 2,
            ServerOnly = 3,
            Both = 4,
            Trigger = 5,
            NonSpawnedTrigger = 6,
            ServerOnlyNonSpawned = 7,
            NoColliderNonSpawned = 8

        }

    }

}
