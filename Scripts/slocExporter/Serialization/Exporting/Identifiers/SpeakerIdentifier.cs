using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class SpeakerIdentifier : IObjectIdentifier<SpeakerExportable>
    {

        public SpeakerExportable Process(GameObject o)
            => !o.TryGetComponent(out AudioSource source)
                ? null
                : new SpeakerExportable
                {
                    ControllerId = (byte) source.priority,
                    Spatial = source.spatialBlend > 0.5f,
                    Volume = source.volume,
                    MinDistance = source.minDistance,
                    MaxDistance = source.maxDistance
                };

    }

}
