using slocExporter.Extensions;
using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class PrimitiveIdentifier : IObjectIdentifier<PrimitiveExportable>
    {

        public PrimitiveExportable Process(GameObject o)
        {
            if (!o.TryGetComponent(out MeshFilter filter))
                return null;
            var mesh = filter.sharedMesh;
            if (!mesh)
                return null;
            var type = Identify.PrimitiveObjectType(mesh.name);
            return type == ObjectType.None ? null : new PrimitiveExportable {PrimitiveType = type};
        }

    }

}
