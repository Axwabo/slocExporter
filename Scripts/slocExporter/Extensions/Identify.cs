using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using slocExporter.Objects;

namespace slocExporter.Extensions
{

    public static class Identify
    {

        public const string ExporterIgnoredTag = "slocExporter Ignored";
        public const string RoomTag = "Room";

        public static readonly Dictionary<Regex, ObjectType> PrimitiveTypes = new Dictionary<string, ObjectType>
        {
            {"Cube", ObjectType.Cube},
            {"Cylinder", ObjectType.Cylinder},
            {"Sphere", ObjectType.Sphere},
            {"Capsule", ObjectType.Capsule},
            {"Plane", ObjectType.Plane},
            {"Quad", ObjectType.Quad}
        }.ToDictionary(k => new Regex($"{k.Key}(?:(?: Instance)+)?"), v => v.Value);

        public static readonly Dictionary<string, StructureObject.StructureType> StructureGuids = new()
        {
            {"c9027d87e276243499d0855914516728", StructureObject.StructureType.Adrenaline},
            {"c89913dc758501e4d88bbd018f72c543", StructureObject.StructureType.BinaryTarget},
            {"db20325cd26c8c84eb3ad5b333807b21", StructureObject.StructureType.DboyTarget},
            {"1b1ee647e05e4bf4491ce470b3982de3", StructureObject.StructureType.EzBreakableDoor},
            {"cecdb86e000e08445895728aa8890bfb", StructureObject.StructureType.Generator},
            {"1a97804ccde6c3f4f835c0dcd09c6f85", StructureObject.StructureType.HczBreakableDoor},
            {"99d103e1d107c434283bcf72ae8b3c76", StructureObject.StructureType.LargeGunLocker},
            {"bbb9c95a6d1307d4e9c1218f4532072b", StructureObject.StructureType.LczBreakableDoor},
            {"54c07ebe424ee83429e95a25a10534c6", StructureObject.StructureType.Medkit},
            {"da8fd4315d23e984fa861e05c4e0f3cc", StructureObject.StructureType.MiscellaneousLocker},
            {"581e190a7fcffe14b8ed1f3b72d4719d", StructureObject.StructureType.RifleRack},
            {"4a1fa0d57462cb34db76e55e9de544fc", StructureObject.StructureType.Scp018Pedestal},
            {"cb1f8708ad190734ba785b3cdafafb66", StructureObject.StructureType.Scp207Pedestal},
            {"c960034df4c76fe4f8a3cd08a58d883c", StructureObject.StructureType.Scp244Pedestal},
            {"17a2b65d1966ef041aac04e61ec852f6", StructureObject.StructureType.Scp268Pedestal},
            {"d38194e63b3da2b4a80979c17f887d0b", StructureObject.StructureType.Scp500Pedestal},
            {"41460fe253b9dd2438beb331ddc0ca25", StructureObject.StructureType.Scp1576Pedestal},
            {"8e1c86dc26ed42e4eb519aedd0e9fcd1", StructureObject.StructureType.Scp1853Pedestal},
            {"6ad060242329d2d46ab64c47fd417146", StructureObject.StructureType.Scp2176Pedestal},
            {"b39d8037aa87d5348af5c3ad54251890", StructureObject.StructureType.SportTarget},
            {"67777259bd9055040bc1be50789f9624", StructureObject.StructureType.Workstation}
        };

        public static ObjectType PrimitiveObjectType(string meshName) => PrimitiveTypes.FirstOrDefault(e => e.Key.IsMatch(meshName)).Value;

    }

}
