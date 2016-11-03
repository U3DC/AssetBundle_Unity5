using UnityEngine;
using System.Collections;

public class AssetBundleLoad : MonoBehaviour {

    public string abURL;
    AssetBundle myAssetBundle;

    void Start()
    {
        StartCoroutine(LoadAssetBundleAsset());

    }

    IEnumerator LoadAssetBundleAsset()
    {
        //unity 推荐的方式，缓存机制，如果纯粹www机制就必须每次都是在线下载，使用loadfromecaheordownload可以自动判断是否本地有缓存。ios和安卓4g，web 50m
        WWW www = WWW.LoadFromCacheOrDownload(abURL,5);
        //等待下载完成
        yield return www;

        //指定我定义的bunle为从网上拉下来的bundle，以便进一步操作。
        myAssetBundle = www.assetBundle;

        //异步加载对象
        AssetBundleRequest request = myAssetBundle.LoadAssetAsync("Holiday",typeof(GameObject));
        //等待加载完成
        yield return request;

        //获取请求到的对象资源引用
        GameObject cube = request.asset as GameObject;
        //动态获取shader，防止丢失
        cube.GetComponentInChildren<Renderer>().sharedMaterial.shader = Shader.Find("Unlit/Texture");


        //从内存中卸载assebundle,false参数也是官方建议的，不直接删除，直到引用为零。
        myAssetBundle.Unload(false);

        GameObject.Instantiate(cube);

    }
}
