using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class InvisibleInteractableExportable : IExportable<InvisibleInteractableObject>
    {

        public InvisibleInteractableObject.ColliderShape Shape;

        public float InteractionDuration;

        public InvisibleInteractableObject Export(int instanceId, ExportContext context)
            => new(Shape, instanceId) {InteractionDuration = instanceId};

    }

}
