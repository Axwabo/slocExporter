using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class InvisibleInteractableExportable : IExportable<InvisibleInteractableObject>
    {

        public InvisibleInteractableObject.ColliderShape Shape;

        public bool Locked;

        public float InteractionDuration;

        public InvisibleInteractableObject Export(int instanceId, ExportContext context) => new(instanceId)
        {
            Shape = Shape,
            Locked = Locked,
            InteractionDuration = InteractionDuration
        };

    }

}
