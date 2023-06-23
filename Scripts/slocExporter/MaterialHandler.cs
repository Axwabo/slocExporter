using System.Collections.Generic;
using slocExporter.Objects;
using UnityEditor;
using UnityEngine;

namespace slocExporter
{

    internal static class MaterialHandler
    {

        private static readonly Dictionary<Color, Material> MaterialCache = new();

        public static void ClearMaterialCache() => MaterialCache.Clear();

        public static void HandleNoMaterial(PrimitiveObject primitive, GameObject created)
        {
            if (SkipForAll)
                return;
            var result = EditorUtility.DisplayDialogComplex("No Material", "No material found for color " + primitive.MaterialColor + ".\nCreate it now?", "Create", "Create for All", "Skip");
            switch (result)
            {
                case 0:
                case 1:
                    CreateMaterial(primitive.MaterialColor, out var mat);
                    created.GetComponent<MeshRenderer>().sharedMaterial = mat;
                    CreateForAll = result == 1;
                    break;
                case 2:
                    if (!EditorUtility.DisplayDialog("Skip", "Do you want to skip creating materials for all objects?", "Skip only this", "Skip for All"))
                        SkipForAll = true;
                    break;
            }
        }

        public static bool TryGetMaterial(Color color, out Material material, out bool handle)
        {
            handle = true;
            material = null;
            if (!slocImporter.UseExistingMaterials)
                return ProcessAfterFind(color, ref material, ref handle);
            if (MaterialCache.TryGetValue(color, out material))
                return true;
            BuildCache(color, ref material);
            return ProcessAfterFind(color, ref material, ref handle);
        }

        private static void BuildCache(Color color, ref Material material)
        {
            if (MaterialCache.Count > 0)
                return;
            EditorUtility.DisplayProgressBar("slocImporter - Building material cache", "Collecting assets", -1f);
            var assets = AssetDatabase.FindAssets("t:material", slocImporter.SearchInColorsFolderOnly ? new[] {"Assets/Colors"} : null);
            var count = assets.Length;
            var floatCount = (float) count;
            var loaded = 0;
            for (var i = 0; i < assets.Length; i++)
            {
                var path = AssetDatabase.GUIDToAssetPath(assets[i]);
                var asset = AssetDatabase.LoadAssetAtPath<Material>(path);
                if (!asset.mainTexture)
                {
                    if (asset.color == color)
                        material = asset;
                    MaterialCache[asset.color] = asset;
                }

                EditorUtility.DisplayProgressBar($"slocImporter - Building material cache {i / floatCount * 100:N2}%", $"{path} ({i + 1} of {count})", i / floatCount);
                if (++loaded < 100)
                    continue;
                EditorUtility.UnloadUnusedAssetsImmediate();
                loaded = 0;
            }
        }

        private static bool ProcessAfterFind(Color color, ref Material material, ref bool handle)
        {
            if (material != null)
            {
                MaterialCache[color] = material;
                return true;
            }

            if (SkipForAll || !CreateForAll)
            {
                handle = !CreateForAll;
                return false;
            }

            CreateMaterial(color, out material);
            return true;
        }

        private static void CreateMaterial(Color color, out Material material)
        {
            material = new Material(Shader.Find("Standard"))
            {
                color = color
            };
            AssetDatabase.CreateAsset(material, "Assets/Colors/" + $"Material-{color.ToString()}" + ".mat");
        }

        public static bool CreateForAll;

        public static bool SkipForAll;

    }

}
