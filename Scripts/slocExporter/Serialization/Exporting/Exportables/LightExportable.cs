using slocExporter.Objects;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Exportables
{

    public sealed class LightExportable : IExportable<LightObject>
    {

        public Color Color;

        public float Range;

        public float Intensity;

        public LightShadows Shadows;

        public float ShadowStrength;

        public LightType Type;

        public LightShape Shape;

        public float SpotAngle;

        public float InnerSpotAngle;

        public LightObject Export(int instanceId, ExportContext context) => new(instanceId)
        {
            LightColor = Color,
            Range = Range,
            Intensity = Intensity,
            ShadowType = Shadows,
            ShadowStrength = ShadowStrength,
            LightType = Type,
            Shape = Shape,
            SpotAngle = SpotAngle,
            InnerSpotAngle = InnerSpotAngle
        };

    }

}
