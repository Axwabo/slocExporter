using slocExporter.Extensions;
using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using UnityEngine;

namespace slocExporter.Serialization.Exporting.Identifiers
{

    public sealed class CameraIdentifier : IObjectIdentifier<CameraExportable>
    {

        public CameraExportable Process(GameObject o)
            => !o.TryGetComponent(out Scp079CameraProperties properties)
                ? null
                : properties.typeOverride != Scp079CameraType.None
                    ? new CameraExportable {Type = properties.typeOverride, Properties = properties}
                    : o.TryGetPrefabName(out var name) && Identify.CameraPrefabNames.TryGetValue(name, out var type)
                        ? new CameraExportable {Type = type, Properties = properties}
                        : null;

    }

}
