using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class SpeakerExportable : IExportable<SpeakerObject>
    {

        public byte ControllerId;

        public bool Spatial;

        public float Volume;

        public float MinDistance;

        public float MaxDistance;

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
