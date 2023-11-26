using UnityEditor;

namespace Editor
{

    [InitializeOnLoad]
    public sealed class RegisterTag
    {

        static RegisterTag() => AddTag(ObjectExporter.ExporterIgnoredTag);

        private static void AddTag(string tagName)
        {
            var tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            var tagsProp = tagManager.FindProperty("tags");
            if (PropertyExists(tagsProp, 0, tagsProp.arraySize, tagName))
                return;
            var index = tagsProp.arraySize;
            tagsProp.InsertArrayElementAtIndex(index);
            var sp = tagsProp.GetArrayElementAtIndex(index);
            sp.stringValue = tagName;
            tagManager.ApplyModifiedProperties();
        }

        private static bool PropertyExists(SerializedProperty property, int start, int end, string value)
        {
            for (var i = start; i < end; i++)
            {
                var t = property.GetArrayElementAtIndex(i);
                if (value.Equals(t.stringValue))
                    return true;
            }

            return false;
        }

    }

}
