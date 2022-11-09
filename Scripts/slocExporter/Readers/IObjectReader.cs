using System.IO;
using slocExporter.Objects;

namespace slocExporter.Readers {

    public interface IObjectReader {

        slocHeader ReadHeader(BinaryReader stream);
        
        slocGameObject Read(BinaryReader stream, slocHeader header);

    }

}
