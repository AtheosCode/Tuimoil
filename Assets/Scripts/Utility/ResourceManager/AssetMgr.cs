using UnityEngine;
using System.Collections;

//资源加载总控制器
public class AssetMgr : Singleton<AssetMgr>
{
    //主要加载器
    IResource m_resourceLoader;

    public void Init()
    {
        //根据表判断
        //热更新版本———StreamingAssetLoader
        //不热更新——ResourcesLoader

        //现在没有做表格，暂时用ResourcesLoader不热更新
        m_resourceLoader = new ResourcesLoader();
    }

    public Object LoadAsset(string name)
    {
        return m_resourceLoader.LoadAsset(name);
    }

    public T LoadAsset<T>(string name) where T : Object
    {
        return m_resourceLoader.LoadAsset<T>(name);
    }

    public Object LoasAssetPath(string path)
    {
        return m_resourceLoader.LoasAssetPath(path);
    }

    public T LoasAssetPath<T>(string path) where T:Object
    {
        return m_resourceLoader.LoasAssetPath<T>(path);
    }

    public GameObject LoadInstance(string name, Vector3 pos, Quaternion rotation, bool isPath = true)
    {
        return m_resourceLoader.LoadInstance(name, pos, rotation, isPath);
    }

    public GameObject LoadInstance(string name)
    {
        return m_resourceLoader.LoadInstance(name, Vector3.zero, Quaternion.identity);
    }

    public void ReleaseAsset(string name)
    {
        m_resourceLoader.ReleaseAsset(name);
    }

    public void ReleaseInstance(GameObject obj)
    {
        m_resourceLoader.ReleaseInstance(obj);
    }


}