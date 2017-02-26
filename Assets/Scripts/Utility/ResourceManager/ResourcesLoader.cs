using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using UnityEngine;

/// <summary>
/// 加载Resources文件夹下的资源
/// </summary>
public class ResourcesLoader :  Singleton<ResourcesLoader>, IResource {
    public Dictionary<string, string> m_filesDic = new Dictionary<string, string>();//资源名称——资源路径
    public Dictionary<string, SingleResource> m_resoucesDic = new Dictionary<string, SingleResource>();

    public string m_resourcePath;

    public ResourcesLoader() {
        if (Application.platform != RuntimePlatform.WindowsEditor) {
            //获取Resources下面的所有资源，并且进行缓存，写入表格
            LoadPathFile();
        } else {
            //读取表格，进行缓存
            m_resourcePath = Application.dataPath + "/Resources/";

            GetFileInfo(new DirectoryInfo(m_resourcePath));
            SaveFileInfo();
        }
    }

    private void LoadPathFile() {
        var info = Resources.Load("resourceInfo") as TextAsset;
        var xml = LoadXML(info.text);
        for (int i = 0; i < xml.Children.Count; i += 2) {
            var key = xml.Children[i] as System.Security.SecurityElement;
            var value = xml.Children[i + 1] as System.Security.SecurityElement;
            m_filesDic.Add(key.Text, value.Text);
        }
    }

    //TODO
    private SecurityElement LoadXML(string xml) {
        try {
            SecurityParser securityParser = new SecurityParser();
            securityParser.LoadXml(xml);
            return securityParser.ToXml();
        } catch (System.Exception ex) {
            Debug.Log("error");
            return null;
        }
    }

    private void GetFileInfo(DirectoryInfo path) {
        var dirInfos = path.GetDirectories().Where(t => t.Name.StartsWith(".") == false);
        FileInfo[] fileInfos = path.GetFiles();

        foreach (DirectoryInfo item in dirInfos) {
            GetFileInfo(item);
        }

        foreach (FileInfo item in fileInfos) {
            string value = item.FullName.Replace("\\", "/").Replace(m_resourcePath, "");
            string key = GetFilePathWithoutExtention(item.Name);

            //string final = CombinePath(value, key);

            if (!IsResource(item.FullName.Replace("\\", "/")))
                continue;

            if (!m_filesDic.ContainsKey(key)) {
                value = GetFilePathWithoutExtention(value);
                m_filesDic.Add(key, value);
            } else {
                //Debug.Log("file already exist");
            }
        }
    }

    private string CombinePath(string path, string name) {
        //先计算出有几个/
        int count = 0;
        int startIndex = 0;
        while (true) {
            int y = path.IndexOf("/", startIndex);
            if (y != -1) {
                count++;
                startIndex = y + 1;
            } else {
                break;
            }
        }

        string final = "";
        for (int i = 0; i < count; i++) {
            final = final + '.';
        }

        return final + name;
    }

    private bool IsResource(string path) {
        string[] filter = new string[] { ".meta", ".xml", ".dds", ".unity" };

        for (int i = 0; i < filter.Length; i++) {
            if (path.EndsWith(filter[i], System.StringComparison.OrdinalIgnoreCase)) {
                return false;
            }
        }

        return true;
    }

    private void SaveFileInfo() {
        SecurityElement root = new SecurityElement("root");

        foreach (var item in m_filesDic) {
            root.AddChild(new SecurityElement("k", item.Key));
            root.AddChild(new SecurityElement("v", item.Value));
        }

        SaveText(m_resourcePath + "resourceInfo.xml", root.ToString());
    }

    //TODO
    private void SaveText(string fileName, string text) {
        if (!Directory.Exists(GetDirectoryName(fileName))) {
            Directory.CreateDirectory(GetDirectoryName(fileName));
        }

        if (File.Exists(fileName)) {
            File.Delete(fileName);
        }
        using (FileStream fs = new FileStream(fileName, FileMode.Create)) {
            using (StreamWriter sw = new StreamWriter(fs)) {
                //开始写入
                sw.Write(text);
                //清空缓冲区
                sw.Flush();
                //关闭流
                sw.Close();
            }
            fs.Close();
        }
    }

    public string GetDirectoryName(string fileName) {
        return fileName.Substring(0, fileName.LastIndexOf('/'));
    }

    public string GetFilePathWithoutExtention(string fileName) {
        return fileName.Substring(0, fileName.LastIndexOf('.'));
    }

    //资源加载API

    public Object LoadAsset(string name, bool isPath = false) {
        string path = null;

        if (isPath) {
            path = name;
        } else {
            if (m_filesDic.ContainsKey(name)) {
                path = m_filesDic[name];
            } else {
                Debug.LogError("No such File " + name);

                return null;
            }
        }

        return Load(path);
    }

    //释放资源
    public void ReleaseAsset(string name) {
        if (m_resoucesDic.ContainsKey(name)) {
            m_resoucesDic[name].referenceCount--;
        }
    }

    public void ReleaseInstance(GameObject obj) {
        GameObject.Destroy(obj);

        ReleaseAsset(obj.name);
    }

    public Object LoasAssetPath(string path) {
        return Load(path);
    }

    private Object Load(string path) {
        SingleResource resource;

        bool flag = m_resoucesDic.TryGetValue(path, out resource);
        if (!flag) {
            resource = new SingleResource(path);
            resource.Load();

            m_resoucesDic.Add(path, resource);
        }

        //引用加1
        resource.referenceCount++;

        return resource.obj;
    }

    private T Load<T>(string path) where T : Object {
        SingleResource resource;

        bool flag = m_resoucesDic.TryGetValue(path, out resource);
        if (!flag) {
            resource = new SingleResource(path);

            resource.Load<T>();

            m_resoucesDic.Add(path, resource);
        }

        //引用加1
        resource.referenceCount++;

        return (T)resource.obj;
    }

    public T LoadAsset<T>(string name, bool isPath = false) where T : Object {
        string path = null;

        if (isPath) {
            path = name;
        } else {
            if (m_filesDic.ContainsKey(name)) {
                path = m_filesDic[name];
            } else {
                Debug.LogError("No such File " + name);

                return null;
            }
        }

        return Load<T>(path);
    }

    public T LoasAssetPath<T>(string path) where T : Object {
        return Load<T>(path);
    }

    public GameObject LoadInstance(string name, Vector3 pos, Quaternion rotation, bool isPath = false) {
        Object asset = isPath ? LoadAsset(name, true) : LoadAsset(name, false);

        GameObject obj = null;

        if (asset != null) {
            obj = (GameObject)GameObject.Instantiate(asset, pos, rotation);
        }

        return obj;
    }
}

//资源的封装
public class SingleResource {
    public UnityEngine.Object obj;
    public string path;

    public int referenceCount;

    public SingleResource(string path) {
        this.path = path;
    }

    public void Load() {
        obj = Resources.Load(path);
    }

    public void Load<T>() where T : Object {
        T t = Resources.Load<T>(path);
        obj = (Object)t;
    }
}

//interface
public interface IResource {

    Object LoadAsset(string name, bool isPath = false);

    T LoadAsset<T>(string name, bool isPath = false) where T : Object;

    Object LoasAssetPath(string path);

    T LoasAssetPath<T>(string path) where T : Object;

    GameObject LoadInstance(string name, Vector3 pos, Quaternion rotation, bool isPath = false);

    //GameObject LoadInstance(string path, Vector3 pos, Quaternion rotation);

    void ReleaseAsset(string name);

    void ReleaseInstance(GameObject obj);
}