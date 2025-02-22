using slocExporter.Objects;
using UnityEngine;

namespace slocExporter.Serialization.Exporting
{

    public interface IComponentProcessor<out TComponent, out TExportable> where TComponent : MonoBehaviour where TExportable : IExportable<slocGameObject>
    {

        TComponent Component { get; }

        TExportable Exportable { get; }

    }

}
