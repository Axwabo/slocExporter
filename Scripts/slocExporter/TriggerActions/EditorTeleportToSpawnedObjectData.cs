using System;
using slocExporter.TriggerActions.Data;
using UnityEngine;

namespace slocExporter.TriggerActions {

    [Serializable]
    public sealed class EditorTeleportToSpawnedObjectData {

        public GameObject go;

        public Vector3 offset;

        public TeleportToSpawnedObjectData ExporterSuitableEquivalent => go == null ? null : new TeleportToSpawnedObjectData(go.GetInstanceID(), offset);

    }

}
