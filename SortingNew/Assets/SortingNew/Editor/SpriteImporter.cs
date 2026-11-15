using UnityEditor;
using UnityEngine;
// Auto-configures every PNG in Resources/Sprites as a 200 PPU sprite (9-sliced panels).
public class SpriteImporter : AssetPostprocessor
{
    void OnPreprocessTexture()
    {
        if (!assetPath.Contains("Resources/Sprites")) return;
        var textureImporter = (TextureImporter)assetImporter;
        textureImporter.textureType = TextureImporterType.Sprite;
        textureImporter.spriteImportMode = SpriteImportMode.Single;
        textureImporter.spritePixelsPerUnit = 200;
        textureImporter.alphaIsTransparency = true;
        textureImporter.mipmapEnabled = false;

        if (assetPath.Contains("glass_panel") || assetPath.Contains("order_panel"))
        {
            textureImporter.spriteBorder = new Vector4(120, 120, 120, 120);
            var textureSettings = new TextureImporterSettings();
            textureImporter.ReadTextureSettings(textureSettings);
            textureSettings.spriteMeshType = SpriteMeshType.FullRect;
            textureImporter.SetTextureSettings(textureSettings);
        }
    }
}
