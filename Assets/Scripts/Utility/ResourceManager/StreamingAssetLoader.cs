using UnityEngine;
using System.Collections;

//资源热更新，TODO
public class StreamingAssetLoader : IResource
{

    public Object LoadAsset(string name, bool isPath = false)
    {
        throw new System.NotImplementedException();
    }

    public Object LoasAssetPath(string path)
    {
        throw new System.NotImplementedException();
    }

    public void ReleaseAsset(string name)
    {
        throw new System.NotImplementedException();
    }

    public void ReleaseInstance(GameObject obj)
    {
        throw new System.NotImplementedException();
    }

    public T LoadAsset<T>(string name, bool isPath = false) where T : Object
    {
        return null;
    }


    public T LoasAssetPath<T>(string path) where T : Object
    {
        throw new System.NotImplementedException();
    }


    public GameObject LoadInstance(string name, Vector3 pos, Quaternion rotation, bool isPath = false)
    {
        throw new System.NotImplementedException();
    }
}
