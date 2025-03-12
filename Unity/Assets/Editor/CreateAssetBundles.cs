using UnityEditor;
using System.IO;
using System.Linq;

public sealed class CreateAssetBundles
{
    [MenuItem("AssetBundle/Build")]
    static void BuildAllAssetBundlesWin()
    {
        string assetBundleDirectory = "Assets/StreamingAssets";
        if (!Directory.Exists(assetBundleDirectory))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        string and = "Assets/StreamingAssets/And";
        if (!Directory.Exists(and))
        {
            Directory.CreateDirectory(and);
        }
        else
        {
            Directory.EnumerateFiles(and, "*.*", SearchOption.AllDirectories).ToList().ForEach(File.Delete);
        }
        string win = "Assets/StreamingAssets/Win";
        if (!Directory.Exists(win))
        {
            Directory.CreateDirectory(win);
        }
        else
        {
            Directory.EnumerateFiles(win, "*.*", SearchOption.AllDirectories).ToList().ForEach(File.Delete);
        }
        BuildPipeline.BuildAssetBundles(win, BuildAssetBundleOptions.None, BuildTarget.StandaloneWindows);
        BuildPipeline.BuildAssetBundles(and, BuildAssetBundleOptions.None, BuildTarget.Android);
    }
}