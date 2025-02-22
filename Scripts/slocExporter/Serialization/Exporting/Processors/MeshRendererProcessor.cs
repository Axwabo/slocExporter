using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Processors
{

    public static class MeshRendererProcessor
    {

        public static void Process(PrimitiveExportable primitive, MeshRenderer renderer)
        {
            if (renderer.enabled)
                primitive.ResolvedFlags |= PrimitiveObjectFlags.Visible;
            else
                primitive.VetoedFlags |= PrimitiveObjectFlags.Visible;
            var material = renderer.sharedMaterial;
            if (material)
                primitive.MaterialColor = material.color;
        }

    }

}
