using UnityEngine;

/// <summary>
///单利模板类(非mMonoBehaivour)
/// </summary>
/// <typeparam name="T"></typeparam>
public class Singleton<T> where T : Singleton<T>,new()
{
    private static T instance;
    public static T Instance
    {
        get {
            if (instance == null) {
                instance = new T();
            }
            return instance;
        }
    }
}

public class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    private static T instance;
    public static T Instance {
        get {
            if (instance = null)
            {
                Create();
            }
            return instance; }
    }

    public static T Create()
    {
        //get it
        T[] instances = GameObject.FindObjectsOfType<T>();
        if (instances.Length == 0)
        {
            try
            {
                //there is none, the create the instance
                GameObject go = new GameObject(typeof(T).Name, typeof(T));
                instance = go.GetComponent<T>();
                GameObject main = GameObject.Find("Main");
                instance.transform.parent = main.transform;
                //instance.tag = Tag.Manager;
            }
            catch (System.Exception)
            {
                return null;
            }
            
        }
        else if (instances.Length == 1)
        {
            //there is one, then get the instance
            instance = instances[0];
        }
        else
        {
            Debug.LogError("You have more than one " + typeof(T).Name + " in the scene. You only need 1, it's a singleton!");
            //more than one, use the first one, destory the left
            instance = instances[0];
            for (int i = 1; i < instances.Length; ++i)
            {
                Destroy(instances[i].gameObject);
            }
        }
        instance.Init();
        return instance;
    }

    public void Clear()
    {
        if (!instance) return;
        //destroy instance
        DestroyImmediate(instance.gameObject);
        instance = null;
    }

    /// <summary>
    /// 初始化动作，在创建改单例完成后会调用
    /// </summary>
    /// <returns></returns>
    protected virtual void Init()
    {

    }

}

