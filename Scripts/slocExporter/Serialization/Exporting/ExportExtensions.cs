using System;
using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using slocExporter.Serialization.Exporting.Identifiers;
using slocExporter.Serialization.Exporting.Processors;
using UnityEngine;

namespace slocExporter.Serialization.Exporting
{

    public static class ExportExtensions
    {

        private static readonly IObjectIdentifier<IExportable<slocGameObject>>[] Identifiers =
        {
            new OverriddenStructureIdentifier(),
            new PrefabStructureIdentifier(),
            new PrimitiveIdentifier()
        };

        public static bool TryIdentify(this GameObject o, out IExportable<slocGameObject> exportable)
        {
            foreach (var identifier in Identifiers)
            {
                exportable = identifier.Process(o);
                if (exportable != null)
                    return true;
            }

            exportable = null;
            return false;
        }

        public static bool TryProcess(this IExportable<slocGameObject> exportable, Component component) => (exportable, component) switch
        {
            (PrimitiveExportable primitive, Collider collider) => primitive.Process(collider, ColliderProcessor.Process),
            (PrimitiveExportable primitive, MeshRenderer renderer) => primitive.Process(renderer, MeshRendererProcessor.Process),
            (PrimitiveExportable primitive, PrimitiveFlagsSetter setter) => primitive.Process(setter, PrimitiveFlagsSetterProcessor.Process),
            _ => false
        };

        private static bool Process<TExportable, TComponent>(this TExportable exportable, TComponent component, Action<TExportable, TComponent> process)
            where TExportable : IExportable<slocGameObject>
            where TComponent : Component
        {
            process(exportable, component);
            return true;
        }

        public static void ApplyTransform(this slocGameObject exported, GameObject o)
        {
            var t = o.transform;
            exported.Transform = t;
            var parent = t.parent;
            if (parent)
                exported.ParentId = parent.gameObject.GetInstanceID();
        }

    }

}
