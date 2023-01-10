namespace slocExporter.TriggerActions.Data {

    public sealed class TeleportToSpawnedObjectData : BaseTriggerActionData {

        public override TargetType TargetType => TargetType.Toy;
        public override TriggerActionType ActionType => TriggerActionType.TeleportToSpawnedObject;

        public readonly int ID;

        public TeleportToSpawnedObjectData(int id) => ID = id;

    }

}
