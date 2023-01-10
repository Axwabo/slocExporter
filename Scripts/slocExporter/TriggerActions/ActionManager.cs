using System.Collections.Generic;
using System.IO;
using slocExporter.TriggerActions.Data;
using slocExporter.TriggerActions.Readers;

namespace slocExporter.TriggerActions {

    public static class ActionManager {

        public const ushort MinVersion = 4;

        private static readonly ITriggerActionDataReader DefaultReader = new Ver4ActionDataReader();

        private static readonly Dictionary<ushort, ITriggerActionDataReader> Readers = new() {
            {4, new Ver4ActionDataReader()}
        };

        public static bool TryGetReader(ushort version, out ITriggerActionDataReader reader) {
            reader = null;
            if (version < MinVersion)
                return Readers.TryGetValue(version, out reader);
            reader = DefaultReader;
            return true;
        }

        public static ITriggerActionDataReader GetReader(ushort version) => TryGetReader(version, out var reader) ? reader : DefaultReader;

        public static void WriteActions(BinaryWriter writer, ICollection<BaseTriggerActionData> actions) {
            writer.Write(actions.Count);
            foreach (var data in actions)
                data.WriteTo(writer);
        }

        public static void ReadTypes(BinaryReader reader, out TargetType targetType, out TriggerActionType actionType) {
            targetType = (TargetType) reader.ReadByte();
            actionType = (TriggerActionType) reader.ReadByte();
        }

    }

}
