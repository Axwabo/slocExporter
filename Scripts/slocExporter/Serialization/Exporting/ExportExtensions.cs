using System;
using System.Collections.Generic;
using slocExporter.Extensions;
using slocExporter.Objects;
using slocExporter.Serialization.Exporting.Exportables;
using slocExporter.Serialization.Exporting.Identifiers;
using slocExporter.Serialization.Exporting.Processors;
using slocExporter.TriggerActions;
using UnityEngine;

namespace slocExporter.Serialization.Exporting
{

    public static class ExportExtensions
    {

        private static readonly IObjectIdentifier<IExportable<slocGameObject>>[] Identifiers =
        {
            new OverriddenStructureIdentifier(),
            PrefabStructureIdentifier.Instance,
            new SpeakerIdentifier(),
            new CameraIdentifier(),
            new CapybaraIdentifier(),
            new LightIdentifier(),
            new PrimitiveIdentifier(),
            new EmptyIdentifier()
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
            (PrimitiveExportable primitive, TriggerAction action) => primitive.Process(action, TriggerActionProcessor.Process),
            _ => false
        };

        private static bool Process<TExportable, TComponent>(this TExportable exportable, TComponent component, Action<TExportable, TComponent> process)
            where TExportable : IExportable<slocGameObject>
            where TComponent : Component
        {
            process(exportable, component);
            return true;
        }

        public static void ApplyTransformFrom(this slocGameObject exported, GameObject o)
        {
            var t = o.transform;
            exported.Transform = t;
            var parent = t.parent;
            if (parent)
                exported.ParentId = parent.gameObject.GetInstanceID();
        }

        public static void ApplyNameAndTagFrom(this slocGameObject exported, GameObject o)
        {
            var name = o.name;
            exported.Name = name == exported.GetDefaultName() ? null : name;
            exported.Tag = o.CompareTag("Untagged") ? null : o.tag;
        }

        public static string GetDefaultName(this slocGameObject o) => o switch
        {
            EmptyObject => "GameObject",
            LightObject light => light.LightType switch
            {
                LightType.Spot => "Spot Light",
                LightType.Directional => "Directional Light",
                LightType.Point => "Point Light",
                LightType.Rectangle => "Rectangle Light",
                LightType.Disc => "Disc Light",
                LightType.Pyramid => "Pyramid Light",
                LightType.Box => "Box Light",
                LightType.Tube => "Tube Light",
                _ => null
            },
            PrimitiveObject primitive => primitive.Type.ToString(),
            _ => null
        };

        public static List<slocGameObject> ProcessAndExportObjects(this Dictionary<GameObject, IExportable<slocGameObject>> exportables, ExportContext context, ProgressUpdater progress)
        {
            var slocObjects = new List<slocGameObject>();
            var i = 0;
            var exportablesCount = exportables.Count;
            foreach (var (o, exportable) in exportables)
            {
                progress.Count(++i, exportablesCount, "Processing objects {2:P2} ({0} of {1})");
                var skip = o.CompareTag(Identify.ExporterIgnoredTag);
                ProcessComponents(o, exportable, ref skip);
                if (skip || exportable.Export(o.GetInstanceID(), context) is not {IsValid: true} exported)
                    continue;
                exported.ApplyTransformFrom(o);
                exported.ApplyNameAndTagFrom(o);
                slocObjects.Add(exported);
            }

            return slocObjects;
        }

        private static void ProcessComponents(GameObject o, IExportable<slocGameObject> exportable, ref bool skip)
        {
            foreach (var component in o.GetComponents<Component>())
            {
                if (component is ExporterIgnored)
                {
                    skip = true;
                    break;
                }

                exportable.TryProcess(component);
            }
        }

    }

}
