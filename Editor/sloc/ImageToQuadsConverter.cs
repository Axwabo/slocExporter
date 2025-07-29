using System;
using slocExporter;
using UnityEditor;
using UnityEngine;

namespace Editor.sloc
{

    public sealed class ImageToQuadsConverter : EditorWindow
    {

        [MenuItem("sloc/Image To Quads")]
        public static void ShowWindow() => GetWindow<ImageToQuadsConverter>(true, "Convert image to quads");

        private static Texture2D _image;

        private static bool _merge;

        private static float _mergeThreshold = 0.01f;

        private const string MergeQuadsDescription = "Should multiple neighboring quads (per row) with the same color be merged into a single one?";

        private const string MergeThresholdDescription = "The threshold for merging quads. A value of 0.01 means that the difference between two colors must be less than 1% to be considered the same. Alpha is not taken into account.";

        private void OnGUI()
        {
            GUILayout.Label("Image", EditorStyles.boldLabel);
            _image = (Texture2D) EditorGUI.ObjectField(new Rect(5, 20, 100, 100), _image, typeof(Texture2D), true);
            GUILayout.Space(105);
            _merge = EditorGUILayout.Toggle(new GUIContent("Merge Quads*", MergeQuadsDescription), _merge);
            if (_merge)
                _mergeThreshold = EditorGUILayout.Slider(new GUIContent("Merge Threshold*", MergeThresholdDescription), _mergeThreshold, 0f, 1f);
            EditorGUILayout.HelpBox("This operation may create an excessive amount of GameObjects. Make sure to use it on small images.", MessageType.Warning);
            if (GUILayout.Button("Generate"))
                Generate();
        }

        private static void Generate()
        {
            if (_image == null || !_image.isReadable)
            {
                EditorUtility.DisplayDialog("Conversion Error", "You must provide a readable image! Make sure to apply the proper import settings.", "OK");
                return;
            }

            MaterialHandler.ClearMaterialCache();
            MaterialHandler.SkipForAll = false;
            MaterialHandler.CreateForAll = false;
            try
            {
                AssetDatabase.DisallowAutoRefresh();
                var rootObject = new GameObject($"{_image.name}-container");
                if (_merge)
                    GenerateQuadsMerged(rootObject);
                else
                    GenerateQuadsNotMerged(rootObject);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                EditorUtility.DisplayDialog("Conversion failed", "Failed to convert the image. See the debug log for details.", "OK");
            }
            finally
            {
                EditorUtility.ClearProgressBar();
                AssetDatabase.AllowAutoRefresh();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
        }

        public static void GenerateQuadsMerged(GameObject parent)
        {
            var height = (float) _image.height;
            for (var y = 0; y < height; y++)
            {
                var progress = y / height;
                EditorUtility.DisplayProgressBar($"Generating quads (merged): {progress:P2}", $"Processing row {y}", progress);
                var x = 0;
                while (x < _image.width)
                {
                    var color = _image.GetPixel(x, y);
                    var width = CalculateWidth(x, y, color);
                    if (color.a > 0f)
                        CreatePrimitive(new Vector3(x + width * 0.5f, 1, y), color, width, parent);
                    x += width;
                }
            }
        }

        private static int CalculateWidth(int x, int y, Color color)
        {
            var width = 1;
            while (x + width < _image.width)
            {
                var nextColor = _image.GetPixel(x + width, y);
                if (CheckThreshold(nextColor.r, color.r) || CheckThreshold(nextColor.g, color.g) || CheckThreshold(nextColor.b, color.b))
                    break;
                width++;
            }

            return width;
        }

        private static bool CheckThreshold(float a, float b) => Mathf.Abs(a - b) > _mergeThreshold;

        private static void GenerateQuadsNotMerged(GameObject parent)
        {
            var height = (float) _image.height;
            for (var y = 0; y < height; y++)
            {
                var progress = y / height;
                EditorUtility.DisplayProgressBar($"Generating quads: {progress:P2}", $"Processing row {y}", progress);
                for (var x = 0; x < _image.width; x++)
                {
                    var color = _image.GetPixel(x, y);
                    if (color.a > 0f)
                        CreatePrimitive(new Vector3(x, 1, y), _image.GetPixel(x, y), 1, parent);
                }
            }
        }

        private static void CreatePrimitive(Vector3 position, Color color, int width, GameObject parent)
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var t = gameObject.transform;
            t.SetPositionAndRotation(position, Quaternion.Euler(90, 0, 0));
            t.localScale = new Vector3(width, 1, 1f);
            DestroyImmediate(gameObject.GetComponent<MeshCollider>());
            t.SetParent(parent.transform);
            if (!MaterialHandler.TryGetMaterial(color, out var mat, out var handle))
            {
                if (handle)
                    MaterialHandler.HandleNoMaterial(color, gameObject);
                return;
            }

            gameObject.GetComponent<MeshRenderer>().sharedMaterial = mat;
        }

    }

}
