using UnityEditor;
using System.IO;
public class CreateAssetBundles  {

	[MenuItem("Tool/AssetBundle5.0")]
	static void SetBundleName()
	{
		#region 设置资源的AssetBundle的名称和文件扩展名
		// 检查是否选中了一个合法的模型prefab
		if (Selection.activeGameObject == null || PrefabUtility.GetPrefabType(Selection.activeGameObject) != PrefabType.Prefab)
		{
			EditorUtility.DisplayDialog("警告！这不是演习！", "真替你着急，刚看你摸了半天，愣是错过了。咱们再确认一下：你确定选中了一个Prefab？", "点我死给你看");
			return;
		}
		UnityEngine. Object[] selects = Selection.objects;
		foreach (UnityEngine. Object selected in selects)
		{
			string path = AssetDatabase.GetAssetPath(selected);
			AssetImporter asset = AssetImporter .GetAtPath(path);
			asset.assetBundleName = selected.name + ".ab"; //设置Bundle文件的名称
			//asset.assetBundleVariant = "ab";//设置Bundle文件的扩展名
			asset.SaveAndReimport();

		}
       
        FileUtil.DeleteFileOrDirectory("Assets/AssetBundles");
		if (! Directory.Exists("Assets/AssetBundles"))
		{
			Directory.CreateDirectory("Assets/AssetBundles");
		}
        BuildPipeline.BuildAssetBundles("Assets/AssetBundles",BuildAssetBundleOptions.None,BuildTarget.Android);
        AssetDatabase .Refresh();
		#endregion
	}
}