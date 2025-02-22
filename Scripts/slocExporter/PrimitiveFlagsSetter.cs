using slocExporter.Objects;
using UnityEngine;

namespace slocExporter
{

    [DisallowMultipleComponent]
    public sealed class PrimitiveFlagsSetter : MonoBehaviour
    {

        private const string Visible = "Visible";
        private const string ClientCollidable = "Client Collidable";
        private const string ServerCollidable = "Server Collidable";
        private const string Trigger = "Trigger";
        private const string NotSpawned = "Not Spawned";

        public PrimitiveObjectFlags flags = PrimitiveObject.DefaultFlags;

        public static string ModeToString(PrimitiveObjectFlags mode) => mode switch
        {
            PrimitiveObjectFlags.Visible => Visible,
            PrimitiveObjectFlags.ClientCollidable => ClientCollidable,
            PrimitiveObjectFlags.ServerCollidable => ServerCollidable,
            PrimitiveObjectFlags.Trigger => Trigger,
            PrimitiveObjectFlags.NotSpawned => NotSpawned,
            _ => "None"
        };

        public static PrimitiveObjectFlags StringToMode(string mode) => mode switch
        {
            Visible => PrimitiveObjectFlags.Visible,
            ClientCollidable => PrimitiveObjectFlags.ClientCollidable,
            ServerCollidable => PrimitiveObjectFlags.ServerCollidable,
            Trigger => PrimitiveObjectFlags.Trigger,
            NotSpawned => PrimitiveObjectFlags.NotSpawned,
            _ => PrimitiveObjectFlags.None
        };

    }

}
