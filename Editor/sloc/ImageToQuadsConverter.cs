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

        private const string MergeQuadsDescription = "Should multiple neighboring quads (per row) with the same color be merged into a single one?";

        private void OnGUI()
        {
            GUILayout.Label("Image", EditorStyles.boldLabel);
            _image = (Texture2D) EditorGUI.ObjectField(new Rect(5, 20, 100, 100), _image, typeof(Texture2D), true);
            GUILayout.Space(105);
            _merge = EditorGUILayout.Toggle(new GUIContent("Merge Quads*", MergeQuadsDescription), _merge);
            GUILayout.Space(5);
            if (GUILayout.Button("Generate"))
                Generate();
        }

        private static void Generate()
        {
            MaterialHandler.ClearMaterialCache();
            var rootObject = new GameObject($"{_image.name}-container");
            if (_image == null)
            {
                EditorUtility.DisplayDialog("Conversion Error", "You must provide an image!", "OK");
                return;
            }

            if (_merge)
                GenerateQuadsMerged(rootObject);
            else
                GenerateQuadsNotMerged(rootObject);
        }

        public static void GenerateQuadsMerged(GameObject parent)
        {
            for (var y = 0; y < _image.height; y++)
            {
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
            while (x + width < _image.width && _image.GetPixel(x + width, y) == color)
                width++;
            return width;
        }

        private static void GenerateQuadsNotMerged(GameObject parent)
        {
            for (var x = 0; x < _image.width; x++)
            for (var y = 0; y < _image.height; y++)
            {
                var color = _image.GetPixel(x, y);
                if (color.a > 0f)
                    CreatePrimitive(new Vector3(x, 1, y), _image.GetPixel(x, y), 1, parent);
            }
        }

        private static void CreatePrimitive(Vector3 position, Color color, int runLength, GameObject parent)
        {
            var gameObject = GameObject.CreatePrimitive(PrimitiveType.Quad);
            var t = gameObject.transform;
            t.position = position;
            t.localScale = new Vector3(runLength, 1, 1f);
            t.rotation = Quaternion.Euler(90f, 0f, 0f);
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
