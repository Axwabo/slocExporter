using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class SpeakerExportable : IExportable<SpeakerObject>
    {

        public byte ControllerId;

        public bool Spatial = true;

        public float Volume = 1;

        public float MinDistance = 1f;

        public float MaxDistance = 1f;

        public SpeakerObject Export(int instanceId, ExportContext context) => new(instanceId)
        {
            ControllerId = ControllerId,
            Spatial = Spatial,
            Volume = Volume,
            MinDistance = MinDistance,
            MaxDistance = MaxDistance
        };

    }

}
