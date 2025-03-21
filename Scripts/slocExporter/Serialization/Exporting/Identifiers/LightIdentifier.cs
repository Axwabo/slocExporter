using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class LightIdentifier : IObjectIdentifier<LightExportable>
    {

        public LightExportable Process(GameObject o) => !o.TryGetComponent(out Light light)
            ? null
            : new LightExportable
            {
                Color = light.color,
                Range = light.range,
                Intensity = light.intensity,
                Shadows = light.shadows,
                ShadowStrength = light.shadowStrength,
                Type = light.type,
                Shape = light.shape,
                SpotAngle = light.spotAngle,
                InnerSpotAngle = light.innerSpotAngle
            };

    }

}
