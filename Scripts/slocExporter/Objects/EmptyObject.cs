namespace slocExporter.Objects {

    public class EmptyObject : slocGameObject {

        public EmptyObject(int instanceId) : base(instanceId) {
        }

        public override bool IsValid => true;

    }

}
