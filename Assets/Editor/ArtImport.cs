using UnityEditor;
using UnityEngine;
namespace VerdantTrail.Editor {
 public sealed class ArtImport:AssetPostprocessor {
  public override uint GetVersion()=>2;
  void OnPreprocessTexture(){
   if(!assetPath.StartsWith("Assets/Resources/Art/"))return;
   var importer=(TextureImporter)assetImporter;importer.textureType=TextureImporterType.Default;
   importer.isReadable=true;importer.alphaIsTransparency=true;importer.mipmapEnabled=false;
   importer.npotScale=TextureImporterNPOTScale.None;
   importer.maxTextureSize=2048;importer.textureCompression=TextureImporterCompression.Uncompressed;
   importer.filterMode=FilterMode.Bilinear;importer.wrapMode=TextureWrapMode.Clamp;
  }
 }
}
