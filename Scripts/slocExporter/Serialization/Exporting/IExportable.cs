using slocExporter.Objects;

namespace slocExporter.Serialization.Exporting
{

    public interface IExportable<out T> where T : slocGameObject
    {

        T Export(int instanceId, ExportContext context);

    }

}
