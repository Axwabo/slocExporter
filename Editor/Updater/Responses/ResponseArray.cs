using System;
using UnityEngine;

namespace Editor.Updater.Responses
{

    [Serializable]
    public struct ResponseArray<T>
    {

        public T[] items;

        private static string Wrap(string json) => "{\"items\":" + json + "}";

        public static ResponseArray<T> Parse(string json) => JsonUtility.FromJson<ResponseArray<T>>(Wrap(json));

        public static implicit operator T[](ResponseArray<T> response) => response.items;

    }

}
