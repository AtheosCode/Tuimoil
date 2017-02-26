using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
///贴图类 
/// </summary>
public class ImageInfo {
    public string name;
    public Rect rect;
    public Vector2 vec2;

    public ImageInfo(string[] str) {
        if (str != null && str.Length >= 7) {
            this.name = str[0];
            float x, y, w, h;
            x = float.Parse(str[1]);
            y = float.Parse(str[2]);
            w = float.Parse(str[3]);
            h = float.Parse(str[4]);

            this.rect = new Rect(x, y, w, h);

            float pivotX, pivotY;

            pivotX = float.Parse(str[5]);
            pivotY = float.Parse(str[6]);
            this.vec2 = new Vector2(pivotX, pivotY);
        } else {
            Debug.LogError("error parameter of the Length lesser than 7");
        }
    }
}

/// <summary>
/// 图集管理类
/// </summary>
public class Atlas {
    public string name;
    public Texture2D atlas;
    public List<ImageInfo> images = new List<ImageInfo>();

    //小图的名称——小图Sprite
    public Dictionary<string, Sprite> SpriteDic = new Dictionary<string, Sprite>();

    public Atlas(string name, Texture2D atlas) {
        this.name = name;
        this.atlas = atlas;
    }

    //根据图集txt，创建每个小图的ImageInfo信息
    public void LoadTxt(string[] lineArray) {
        images.Clear();

        for (int i = 0; i < lineArray.Length; i++) {
            lineArray[i] = lineArray[i].Replace("\r", "");
            string[] strArray = lineArray[i].Split(new char[] { ';' });
            ImageInfo imgInfo = new ImageInfo(strArray);

            images.Add(imgInfo);
        }
    }

    /// <summary>
    /// 获取该名字的小图
    /// </summary>
    /// <param name="spriteName">小图名称</param>
    /// <returns>小图Sprite</returns>
    public Sprite CreateSprite(string spriteName) {
        if (SpriteDic.ContainsKey(spriteName)) {
            return SpriteDic[spriteName];
        } else {
            ImageInfo info = GetImageInfo(spriteName);
            Sprite spr = Sprite.Create(atlas, info.rect, info.vec2);

            SpriteDic.Add(spriteName, spr);

            return spr;
        }
    }

    public ImageInfo GetImageInfo(string spriteName) {
        for (int i = 0; i < images.Count; i++) {
            if (images[i].name == spriteName) {
                return images[i];
            }
        }

        Debug.Log("no imageInfo " + spriteName);
        return null;
    }

}

public class AtlasMgr : Singleton<AtlasMgr> {
    //string url = "test";
    //public Texture2D mTex;
    //public GameObject UIRoot;
    //List<Image> images = new List<Image>();

    public AtlasMgr() {
        url = "Atlas/";
    }

    public string url;

    //已经加载的图集
    public Dictionary<string, Atlas> AtlasDic = new Dictionary<string, Atlas>();

    /// <summary>
    /// 加载图集的配置信息
    /// </summary>
    /// <param name="name"></param>
    public void LoadAtlas(string name) {
        if (AtlasDic.ContainsKey(name)) {
            Debug.Log("atlas already loaded" + name);
            return;
        }

        //加载图集图片、图集txt
        Texture2D texture = AssetMgr.Instance.LoadAsset<Texture2D>(name);
        TextAsset binAsset = AssetMgr.Instance.LoadAsset<TextAsset>(name + "Sheet");
        if (texture == null || binAsset == null) {
            Debug.LogError("load atlas failed : " + name);
        }

        Atlas atlas = new Atlas(name, texture);
        string[] lineArray = binAsset.text.Split(new char[] { '\n' });

        atlas.LoadTxt(lineArray);

        AtlasDic.Add(name, atlas);
    }

    //测试
    public void Create(Texture2D tx2d, TextAsset txt) {
        Atlas atlas = new Atlas("Item", tx2d);

        string[] lineArray = txt.text.Split(new char[] { '\n' });

        atlas.LoadTxt(lineArray);

        AtlasDic.Add("Item", atlas);
    }

    public Atlas GetAtlas(string name) {
        if (AtlasDic.ContainsKey(name)) {
            return AtlasDic[name];
        } else {
            LoadAtlas(name);
            return AtlasDic[name];
        }

    }

    /// <summary>
    /// 从图集中加载小图sprite
    /// </summary>
    /// <param name="atlasName">图集名称</param>
    /// <param name="spriteName">小图的名称</param>
    /// <returns></returns>
    public Sprite LoadSprite(string iconName) {
        string[] name = iconName.Split('|');

        if (name.Length == 2) {
            string atlasName = name[0];
            string spriteName = name[1];

            return GetAtlas(atlasName).CreateSprite(spriteName);
        } else {
            Debug.LogError("LoadSprite iconName not correct");
            return null;
        }


    }
}
