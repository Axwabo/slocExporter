using slocExporter.Serialization.Exporting.Exportables;
using slocExporter.TriggerActions;

namespace slocExporter.Serialization.Exporting.Processors
{

    public static class TriggerActionProcessor
    {

        public static void Process(PrimitiveExportable primitive, TriggerAction action)
        {
            var data = action.SelectedData;
            if (data != null)
                primitive.TriggerActions.Add(data);
        }

    }

}
