namespace slocExporter.TriggerActions.Data {

    public readonly struct TriggerStore {

        public readonly int Count;

        public readonly BaseTriggerActionData[] Actions;

        public TriggerStore(int count) : this() {
            Count = count;
            Actions = new BaseTriggerActionData[count];
        }

    }

}
