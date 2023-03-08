using System.Collections.Generic;
using slocExporter.Objects;
using UnityEditor;
using UnityEngine;

namespace slocExporter {

    internal static class MaterialHandler {

        private static readonly Dictionary<Color, Material> MaterialCache = new();

        public static void ClearMaterialCache() => MaterialCache.Clear();

        public static void HandleNoMaterial(PrimitiveObject primitive, GameObject created) {
            if (SkipForAll)
                return;
            var result = EditorUtility.DisplayDialogComplex("No Material", "No material found for color " + primitive.MaterialColor + ".\nCreate it now?", "Create", "Create for All", "Skip");
            switch (result) {
                case 0:
                case 1:
                    CreateMaterial(primitive.MaterialColor, out var mat);
                    created.GetComponent<MeshRenderer>().sharedMaterial = mat;
                    CreateForAll = true;
                    break;
                case 2:
                    if (!EditorUtility.DisplayDialog("Skip", "Do you want to skip creating materials for all objects?", "Skip only this", "Skip for All"))
                        SkipForAll = true;
                    break;
            }
        }

        public static bool TryGetMaterial(Color color, out Material material, out bool handle) {
            handle = true;
            material = null;
            if (!slocImporter.UseExistingMaterials)
                return ProcessAfterFind(color, ref material, ref handle);
            if (MaterialCache.TryGetValue(color, out material))
                return true;
            foreach (var e in AssetDatabase.FindAssets("t:material", slocImporter.SearchInColorsFolderOnly ? new[] {"Assets/Colors"} : null)) {
                var asset = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(e));
                if (!asset.mainTexture && asset.color == color) {
                    material = asset;
                    break;
                }

                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            return ProcessAfterFind(color, ref material, ref handle);
        }

        private static bool ProcessAfterFind(Color color, ref Material material, ref bool handle) {
            if (material != null) {
                MaterialCache.Add(color, material);
                return true;
            }

            if (SkipForAll || !CreateForAll) {
                handle = !CreateForAll;
                return false;
            }

            CreateMaterial(color, out material);
            return true;
        }

        private static void CreateMaterial(Color color, out Material material) {
            material = new Material(Shader.Find("Standard")) {
                color = color
            };
            AssetDatabase.CreateAsset(material, "Assets/Colors/" + $"Material-{color.ToString()}" + ".mat");
        }

        public static bool CreateForAll;

        public static bool SkipForAll;

    }

}
