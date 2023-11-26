using System;

namespace Editor.Updater.Responses
{

    [Serializable]
    public struct TagResponse
    {

        public string name;

        public string message;

        public string zipball_url;

    }

}
