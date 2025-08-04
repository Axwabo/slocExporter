using slocExporter.Objects;
using UnityEngine;

namespace slocExporter.Serialization.Exporting
{

    public interface IObjectIdentifier<out T> where T : IExportable<slocGameObject>
    {

        T Process(GameObject o);

    }

}
