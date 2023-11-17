using System.IO;
using slocExporter.TriggerActions.Data;

namespace slocExporter.TriggerActions.Readers
{

    public interface ITriggerActionDataReader
    {

        BaseTriggerActionData Read(BinaryReader reader);

    }

}
