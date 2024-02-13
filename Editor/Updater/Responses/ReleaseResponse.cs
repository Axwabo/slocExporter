using System;

namespace Editor.Updater.Responses
{

    [Serializable]
    public struct ReleaseResponse
    {

        public string tag_name;

        public bool prerelease;

        public string zipball_url;

    }

}
