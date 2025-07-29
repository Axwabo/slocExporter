using slocExporter.Objects;
using UnityEngine;

namespace slocExporter.Extensions
{

    public static class GameObjectExtensions
    {

        public static void ApplyNameAndTag(this GameObject o, string name, string tag)
        {
            if (name != null)
                o.name = name;
            if (tag != null)
                o.tag = tag;
        }

        public static void ApplyNameAndTag(this GameObject o, slocGameObject from)
            => o.ApplyNameAndTag(from.Name, from.Tag);

    }

}
