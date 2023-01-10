using System.IO;

namespace slocExporter.TriggerActions.Data {

    public abstract class BaseTriggerActionData {

        public abstract TargetType TargetType { get; }

        public abstract TriggerActionType ActionType { get; }

        public void WriteTo(BinaryWriter writer) {
            writer.Write((byte) TargetType);
            writer.Write((byte) ActionType);
            WriteData(writer);
        }

        protected abstract void WriteData(BinaryWriter writer);

    }

}
