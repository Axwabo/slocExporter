using System;
using System.Collections.Generic;

public sealed class InstanceDictionary<T> : Dictionary<int, T> {

    public T GetOrAdd(int id, Func<T> factory) {
        if (TryGetValue(id, out var value))
            return value;
        value = factory();
        Add(id, value);
        return value;
    }

    public bool TryGet<TGet>(int id, out TGet value) {
        if (TryGetValue(id, out var obj) && obj is TGet t) {
            value = t;
            return true;
        }

        value = default;
        return false;
    }

}
