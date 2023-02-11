using System.IO;
using slocExporter.TriggerActions.Data;
using slocExporter.TriggerActions.Enums;

namespace slocExporter.TriggerActions.Readers {

    public sealed class Ver4ActionDataReader : ITriggerActionDataReader {

        public BaseTriggerActionData Read(BinaryReader reader) {
            ActionManager.ReadTypes(reader, out var actionType, out var targetType, out var eventType);
            BaseTriggerActionData data = actionType switch {
                TriggerActionType.TeleportToPosition => ReadTpToPos(reader),
                TriggerActionType.MoveRelativeToSelf => ReadMoveRelative(reader),
                TriggerActionType.TeleportToRoom => ReadTpToRoom(reader),
                TriggerActionType.KillPlayer => ReadKillPlayer(reader),
                TriggerActionType.TeleportToSpawnedObject => ReadTpToSpawnedObject(reader),
                _ => null
            };
            if (data is null)
                return null;
            data.SelectedTargets = targetType;
            data.SelectedEvents = eventType;
            return data;
        }

        public static TeleportToPositionData ReadTpToPos(BinaryReader reader) {
            var position = reader.ReadVector();
            return new TeleportToPositionData(position);
        }

        public static MoveRelativeToSelfData ReadMoveRelative(BinaryReader reader) {
            var position = reader.ReadVector();
            return new MoveRelativeToSelfData(position);
        }

        public static TeleportToRoomData ReadTpToRoom(BinaryReader reader) {
            var name = reader.ReadString();
            var position = reader.ReadVector();
            return new TeleportToRoomData(name, position);
        }

        public static KillPlayerData ReadKillPlayer(BinaryReader reader) {
            var cause = reader.ReadString();
            return new KillPlayerData(cause);
        }

        public static SerializableTeleportToSpawnedObjectData ReadTpToSpawnedObject(BinaryReader reader) {
            var virtualInstanceId = reader.ReadInt32();
            var offset = reader.ReadVector();
            return new SerializableTeleportToSpawnedObjectData(virtualInstanceId, offset);
        }

    }

}
