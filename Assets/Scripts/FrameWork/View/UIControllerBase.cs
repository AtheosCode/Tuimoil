using PureMVC.Core;
using PureMVC.Interfaces;
using PureMVC.Patterns;
using System;
using UnityEngine;

public class UIControllerBase : MonoBehaviour {
    /// <summary>
    /// 打开状态
    /// </summary>
    private bool isActive;
    /// <summary>
    /// 是否在关闭的时候只是隐藏而不删除
    /// </summary>
    [SerializeField]
    private bool isCache;
    /// <summary>
    /// 层级
    /// </summary>
    private int depth;
    /// <summary>
    /// pureMVC 中  mediator　NAME
    /// </summary>
    [SerializeField]
    private string mediatorName;
    /// <summary>
    /// 界面类型标记
    /// </summary>
    private GlobalDefine.PanelType type;
    /// <summary>
    /// 是否激活
    /// </summary>
    public bool IsActive {
        get {
            return isActive;
        }
        set {
            isActive = value;
        }
    }
    /// <summary>
    /// 是否在关闭的时候只是隐藏而不删除 
    /// </summary>
    public bool IsCache {
        get {
            return isCache;
        }

        set {
            this.isCache = value;
        }
    }
    /// <summary>
    /// 层级
    /// </summary>
    public int Depth {
        get {
            if (transform != null) {
                depth = transform.GetSiblingIndex();
            }
            return depth;
        }

        set {
            this.depth = value;
            if (transform != null) {
                transform.SetSiblingIndex(depth);
            }
        }
    }
    /// <summary>
    /// 当前panel的IMediator
    /// </summary>
    public string MediatorName {
        get {
            return mediatorName;
        }

        set {
            this.mediatorName = value;
        }
    }
    /// <summary>
    /// 界面类型标记
    /// </summary>
    public GlobalDefine.PanelType Type {
        get {
            return type;
        }

        set {
            this.type = value;
        }
    }
    /// <summary>
    /// 关闭动画🙆
    /// </summary>
    public Action CloseCallback {
        get;set;
    }
    /// <summary>
    /// 打开动画🛀
    /// </summary>
    public Action OpenCallback {
        get; set;
    }
    #region 继承接口

    /// <summary>
    /// 有DOTween 缓动动作 就在这里重写
    /// </summary>
    public virtual void Return() {
        IsActive = false;
        if (CloseCallback != null) {
            CloseCallback();
            CloseCallback = null;
        }
        this.enabled = false;
        if (IsCache) {
            gameObject.SetActive(false);
        } else {
            //删除UI，回收资源
            //AssetMgr.Inst().ReleaseInstance(gameObject);
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 在Awake()之前调用，类似构造函数
    /// </summary>
    public virtual void Create() {
        Depth = transform.GetSiblingIndex();
    }

    //Atheos 销毁界面
    /// <summary>
    /// 删除自己
    /// </summary>
    public virtual void Destroy() {
    }

    /// <summary>
    /// 显示panel，一些动画处理
    /// </summary>
    public virtual void Open() {
        IsActive = true;
        if (OpenCallback!=null) {
            OpenCallback();
            OpenCallback = null;
        }
        this.enabled = true;
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 给子类调用的Update()函数
    /// </summary>
    public virtual void UpdateChild() {
    }
    #endregion 继承接口

    private void Update() {
        UpdateChild();
    }
}