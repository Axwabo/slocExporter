using System.Collections.Generic;
using slocExporter.Objects;

public sealed class InstanceList : List<KeyValuePair<int, slocGameObject>> {

    public InstanceList(IEnumerable<KeyValuePair<int, slocGameObject>> collection) : base(collection) {
    }

    public bool TryGet<T>(int index, out int id, out T item) where T : slocGameObject {
        var pair = this[index];
        id = pair.Key;
        item = pair.Value as T;
        return item != null;
    }

    public slocGameObject ObjectAt(int index) => this[index].Value;

}
