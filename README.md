一，AssetBundle是什么？

明显，我们从字面上就能理解：资源包。

 

二，为什么要使用AssetBundle？

至少，我们不希望我们的安装包大到让客户没有安装的动力。当然，这是玩笑话，AssetBundle主要是要解决开发中对资源动态下载与动态加载的痛点。是不是很痛，就要看项目的需求了。

 

三，AssetBundle的工作流程是怎么样的？

很简单，就跟现在我们下载网络上的资源一样样的。开发者将资源放在某个服务器地址上，客户端通过一个地址（URL）来访问服务器，下载对应的资源，客户端下载到资源再根据需求进行加载展现。

 

四，在Unity5.x中要如何打一个AssetBundle包？

从Unity4.x到5.x，我们发现，unity简化了AssetBundle的一些操作，最大好处是操作更加简单。Unity5中提供了一个简单的AssetBundle的UI。透过这个，我们就可以快速标记一个资源，并且使用BuildPipeline.BuildAssetBundles(Path),这么一条简洁易懂的代码实现快速打包。

具体案例步骤：

1.在场景中创建一个Cube。

2.为了使得它有点灵性，我们再给它(Cube)附上一个脚本（CubeRote.cs），脚本内容如下，就一条代码：

transform.Rotate(Vector3.up * Time.deltaTime *50);

3.再次创建一个新脚本：CreateAssetBundle.cs，新建一个叫Editor的文件夹（此文件夹的内容最终程序打包时不会被包含进去）。将新建脚本CreateAssetBundle.cs放进Editor文件夹内。打开这脚本并编写：

先加入Editor的引用： using UnityEditor;

然后定义个静态方法（Editor中调用的方法必须是静态Static的）。比如方法名：

static voie CreateBundles（）{}

在方法体中，我们可以填入一下内容，其中BuildTarget.iOS是资源构建平台，不同平台打包出来的ab包（assetbundle包）是不能完全兼容通用的。这里我以iOS为例。注意：这里填写的路径为当前项目路径，需要手动创建添加目录，当然我们也可以使用代码来智能的判断添加具体可以看我github上的源码实现，本文末会附地址，我们先用手动的形式创建目录／AssetBundles／iOS，然后在CreateBundles方法中添加实现：
BuildPipeline.BuildAssetBundles(“Assets/AssetBundles/iOS”, BuildAssetBundleOptions.None, BuildTarget.iOS）

这样就差不多了，是的，我们需要写东西并不多，最后，我们还可以在方法前加入这么一句：

[MenuItem(“Tool/AssetBundle5.0”)]

这句话的作用，是在unity的菜单栏中新增了Tool菜单，菜单下有AssetBundle5.0的下拉菜单触发选项。

最后贴出完整的代码：

using UnityEditor;
public class CreateAssetBundles  
{
[MenuItem("Tool/AssetBundle5.0")]
static void CreateBundles()
{
  BuildPipeline.BuildAssetBundles("Assets/AssetBundles/iOS",BuildAssetBundleOptions.None, BuildTarget.iOS);
}
}
将这个脚本，放到editor文件夹中备用。

4.将创建的cube，拉到project中，使之成为一个prefab预置体。

5.选中cube预置体，在inspetor面板的最下方，可以看到assetbundle的UI，选中第一栏，new，新建一个标记，这里我们比如取名『cube.ab』，可以直接在名字中指定任意后缀名。

6.Assebundle UI的第二栏是Variant，主要用于运行时动态替换。这里先不讨论。

7.打上标记后，接着就可以打包了，运行菜单栏的Tool的下拉菜单AssetBundle5.0。这时会自动完成打包。并且我们发现资源目录里多了AssetBundles目录和其子目录iOS，子目录中就已经生成了我们需要的ab包。正常情况下是4个文件，IOS和iOS.manifest主要是记录了所有包与包之间的依赖关系，如果我们的包和包之间相互依赖，那么就需要先调用manifest来获取依赖。这里我们并没有什么依赖。cube.ab和cube.mainfest是我们需要的。其中，cube.ab是包含我们打包的资源内容的，而cube.manifest是记录了资源的依赖关系（依赖的贴图，音频等）。这里，要特别指出的是，如果我们的预置体中有包含了脚本，打包时，会被编译成二进制文件。

 

五，我们要怎么使用这个AssetBundle呢？

1.打包完毕，我们接着要实现的是，如何下载这个资源，并且实现加载。我们新建一个脚本：Test.cs（随便取一个名字，测试啦。。）。编辑这个脚本内容：

//定义ab包的存放地址
    public string abURL;
//定义一个ab，用来接收下载来的ab包的引用。
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
 
        //异步加载对象，我们打包的对象就是那个Cube（这是它的name），类型是gameobject
        AssetBundleRequest request = myAssetBundle.LoadAssetAsync("Cube",typeof(GameObject));
        //等待加载完成
        yield return request;
 
        //获取请求到的对象资源引用
        GameObject cube = request.asset as GameObject;
 
 
        //从内存中卸载assebundle,false参数也是官方建议的，不直接删除，直到引用为零。
        myAssetBundle.Unload(false);
//最终表现形式是实例化出来
        GameObject.Instantiate(cube);
 
    }
}
2.将这个脚本挂在场景的camera上，点击play，效果实现。这时我们就看到了一个转动的cube，但是挂载在它的上面脚本已经被编译成无法编辑的格式。

3.今天我们的案例教程到这里啦。本文U3DC原创，更多教程请持续关注U3DC.COM

4.对于以上内容有疑问可以加群：139457522（Unity黑带⑤段）

GitHub完整源码链接：https://github.com/GeeScan/AssetBundle_Unity5

我的公众号：优三帝
