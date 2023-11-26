using System;

namespace Editor.Updater.Responses
{

    [Serializable]
    public struct ChangesResponse
    {

        public ChangedFile[] files;

        public string message;

    }

    [Serializable]
    public struct ChangedFile
    {

        public string filename;

        public string status;

    }

}
