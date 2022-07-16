using System.IO;
using slocExporter.Objects;

namespace slocExporter.Readers {

    public interface IObjectReader {

        slocGameObject Read(BinaryReader stream);

    }

}
