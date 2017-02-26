using PureMVC.Interfaces;
using PureMVC.Patterns;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 读表数据
/// </summary>
[System.Serializable]
public class NodeInfo {
    #region 编辑器使用
    [Space(10)]
    [Tooltip("中介器类名")]
    public string mediatorName;
    [Space(10)]
    [Tooltip("父节点类型")]
    public GlobalDefine.PanelType parentType;
    [Space(10)]
    [Tooltip("节点类型")]
    public GlobalDefine.PanelType type = GlobalDefine.PanelType.Default;
    /// <summary>
    /// 是否缓存（关闭的时候是隐藏还是销毁）
    /// </summary>
    [Space(10)]
    public bool isCache;
    /// <summary>
    /// 各个子节点之间是不是共存
    /// </summary>
    public bool isCoexist;
    /// <summary>
    /// 是不是控制节点
    /// </summary>
    public bool isControlNode;
    /// <summary>
    /// 自己与父节点是不是共存
    /// </summary>
    public bool isCover;
    /// <summary>
    /// 是否加入导航堆栈
    /// </summary>
    public bool isStack;
    /// <summary>
    /// PureMVC 中介器
    /// </summary>
    public IMediator mediator;
    /// <summary>
    /// 资源名称
    /// </summary>
    public string resource;
    /// <summary>
    /// 脚本名称
    /// </summary>
    public string script;
    /// <summary>
    /// 节点控制器
    /// </summary>
    public UIControllerBase uiControllerBase;
    #endregion 编辑器使用
    #region 树结构属性
    [NonSerialized]
    /// <summary>
    /// 子节点
    /// </summary>
    public List<NodeInfo> childNode = new List<NodeInfo>();
    /// <summary>
    /// 父节点
    /// </summary>
    [NonSerialized]
    public NodeInfo parentNode;
    #endregion 树结构属性

    public static bool operator !=(NodeInfo a, NodeInfo b) {
        return !(a == b);
    }

    public static bool operator ==(NodeInfo a, NodeInfo b) {
        // If both are null, or both are same instance, return true.
        if (System.Object.ReferenceEquals(a, b)) {
            return true;
        }

        // If one is null, but not both, return false.
        if (((object)a == null) || ((object)b == null)) {
            return false;
        }

        // Return true if the fields match:
        return a.type == b.type && a.parentType == b.parentType;
    }

    public override bool Equals(object obj) {
        return base.Equals(obj);
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }
}

public class Order {
    public bool state;
    /// <summary>
    /// 节点树的节点名
    /// </summary>
    public GlobalDefine.PanelType type;
    //true :open fasle: retrun
}

public class UIManagerView : MonoBehaviour {
    public static UIManagerView Instance;
    /// <summary>
    /// UI树关系数据
    /// </summary>
    private Dictionary<GlobalDefine.PanelType, NodeInfo> m_nodeInfoDic = new Dictionary<GlobalDefine.PanelType, NodeInfo>();
    /// <summary>
    /// 暂时通过Unity编辑器处理数据，后期通过读表处理
    /// </summary>
    [SerializeField]
    private List<NodeInfo> m_nodeInfoList = new List<NodeInfo>();
    /// <summary>
    /// 导航堆栈
    /// </summary>
    private Stack<GlobalDefine.PanelType> m_nodeStack = new Stack<GlobalDefine.PanelType>();

    public void Awake() {
        Instance = this;
        InitNodeInfo();
    }

    /// <summary>
    /// 创建新的节点
    /// </summary>
    /// <param name="type"></param>
    public void DoCreate(GlobalDefine.PanelType type) {
        if (m_nodeInfoDic.ContainsKey(type)) {
            NodeInfo nodeInfo = m_nodeInfoDic[type];
            if (nodeInfo.uiControllerBase == null) {
                UnityEngine.Object asset = Resources.Load(m_nodeInfoDic[type].resource);
                GameObject ui = Instantiate(asset) as GameObject;
                ui.SetActive(false);
                RectTransform rectTransform = ui.GetComponent<RectTransform>();
                if (rectTransform != null) {
                    rectTransform.SetParent(transform, false);
                }
                UIControllerBase controller = ui.GetComponent<UIControllerBase>();
                if (controller != null) {
                    controller.IsCache = nodeInfo.isCache;
                    //controller.MediatorName
                    controller.Create();
                }
                nodeInfo.uiControllerBase = controller;
                if (!string.IsNullOrEmpty(nodeInfo.mediatorName) && nodeInfo.mediatorName.Length > 8) {
                    Type classtype = Type.GetType(nodeInfo.mediatorName);
                    if (classtype != null) {
                        Debug.Log(classtype.ToString());
                        nodeInfo.mediator = System.Activator.CreateInstance(classtype, nodeInfo.uiControllerBase as object) as IMediator;
                    } else {

                        nodeInfo.mediator = null;
                    }
                } else {
                    nodeInfo.mediator = null;
                }
            }
        } else {
            Debug.Log("节点<color=red>" + type + "</color>尚未被包含在树结构中");
        }
    }

    /// <summary>
    /// 销毁自己
    /// </summary>
    /// <param name="type"></param>
    public void DoDestroy(GlobalDefine.PanelType type) {
        if (m_nodeInfoDic.ContainsKey(type)) {
            NodeInfo nodeInfo = m_nodeInfoDic[type];
            nodeInfo.uiControllerBase.Destroy();
            nodeInfo.uiControllerBase = null;
        }
    }

    /// <summary>
    /// 删除缓存的所有界面
    /// </summary>
    public void DoDestroyAll() {
        foreach (var item in m_nodeInfoDic) {
            if (item.Value.uiControllerBase != null) {
                item.Value.uiControllerBase.Destroy();
                item.Value.uiControllerBase = null;
            }
        }
        m_nodeStack.Clear();
    }

    /// <summary>
    /// 打开自己
    /// </summary>
    /// <param name="type"></param>
    public void DoOpen(GlobalDefine.PanelType type) {
        if (m_nodeInfoDic.ContainsKey(type)) {
            NodeInfo nodeInfo = m_nodeInfoDic[type];
            Action openSelf = delegate () {
                nodeInfo.uiControllerBase.Open();
            };
            if (nodeInfo.uiControllerBase != null) {
                List<Order> orderList = new List<Order>();
                SearchNodeInfo(nodeInfo, orderList);
                Preclose(orderList, openSelf);
            } else {
                DoCreate(type);
                if (nodeInfo.uiControllerBase != null) {
                    List<Order> orderList = new List<Order>();
                    SearchNodeInfo(nodeInfo, orderList);
                    Preclose(orderList, openSelf);
                }
            }
            if (nodeInfo.mediator != null) {
                AppFacade.Instance.RegisterMediator(nodeInfo.mediator);
            }
        }
    }

    /// <summary>
    /// 退回到上一个堆栈里面
    /// </summary>
    /// <param name="type"></param>
    public void DoReturn(GlobalDefine.PanelType type) {
        if (m_nodeInfoDic.ContainsKey(type)) {
            NodeInfo nodeInfo = m_nodeInfoDic[type];
            if (nodeInfo.uiControllerBase != null) {
                nodeInfo.uiControllerBase.Return();
            }
            //Atheos 此处后面决定是用contians　还是用Peek检测当前堆栈的数据
            //堆栈处理
            if (m_nodeStack.Contains(type)) {
                m_nodeStack.Pop();
            }
            if (m_nodeStack.Count > 0) {
                DoOpen(m_nodeStack.Peek());
            }
            //if (nodeInfo.mediator != null) {
            //    AppFacade.Instance.RemoveMediator(nodeInfo.mediatorName);
            //}
        } else {
            Debug.Log("节点<color=red>" + type + "</color>尚未被包含在树结构中");
        }
    }

    /// <summary>
    /// 遍历对比并设置层级大小
    /// </summary>
    /// <param name="nodeInfo"></param>
    private int DepthChild(NodeInfo nodeInfo) {
        int tempDepth = 0;
        for (int i = 0; i < nodeInfo.childNode.Count; i++) {
            NodeInfo tempNodeInfo = nodeInfo.childNode[i];
            tempDepth = DepthChild(tempNodeInfo);
        }
        if (nodeInfo.uiControllerBase) {
            if (nodeInfo.uiControllerBase.IsActive) {
                int depth = nodeInfo.uiControllerBase.Depth;
                if (tempDepth <= depth) {
                    tempDepth = depth + 1;
                }
            }
        }
        return tempDepth;
    }

    /// <summary>
    /// 创建多叉树
    /// </summary>
    private void InitNodeInfo() {
        //Atheos，暂时通过Unity编辑器处理数据，后期通过读表处理
        for (int i = 0; i < m_nodeInfoList.Count; i++) {
            NodeInfo currentNode = m_nodeInfoList[i];
            if (!currentNode.isControlNode) {
                currentNode.resource = currentNode.type.ToString();
                //if (currentNode.resource == null) {
                //}
                currentNode.script = currentNode.type.ToString();
                //if (currentNode.script == null) {
                //}
            }
            m_nodeInfoDic.Add(currentNode.type, currentNode);
            if (currentNode.type != GlobalDefine.PanelType.RootNode) {
                if (m_nodeInfoDic.ContainsKey(currentNode.parentType)) {
                    currentNode.parentNode = m_nodeInfoDic[currentNode.parentType];
                    m_nodeInfoDic[currentNode.parentType].childNode.Add(currentNode);
                } else {
                    Debug.Log("节点<color=red>" + currentNode.type + "</color>的父节点<color=red>" + currentNode.parentType + "</color>尚未被包含在树结构中");
                }
            }
            //if (m_nodeInfoDic.ContainsKey(currentNode.parentNode.type)) {
            //    currentNode.parentNode = m_nodeInfoDic[currentNode.parentNode.type];
            //    m_nodeInfoDic[currentNode.parentNode.type].childNode.Add(currentNode);
            //} else {
            //    Debug.Log("节点<color=red>" + currentNode.type + "</color>的父节点<color=red>" + currentNode.parentNode + "</color>尚未被包含在树结构中");
            //}
        }
    }

    /// <summary>
    /// 顺序关闭
    /// </summary>
    /// <param name="orderList"></param>
    /// <param name="openSelf"></param>
    private void Preclose(List<Order> orderList, Action openSelf) {
        if (orderList.Count > 1) {
            for (int i = 1; i < orderList.Count; i++) {
                Order orderNow = orderList[i - 1];
                Order orderNext = orderList[i];
                if (m_nodeInfoDic.ContainsKey(orderNow.type)) {
                    if (m_nodeInfoDic[orderNow.type].uiControllerBase) {
                        m_nodeInfoDic[orderNow.type].uiControllerBase.CloseCallback = delegate () {
                            if (m_nodeInfoDic.ContainsKey(orderNext.type)) {
                                if (m_nodeInfoDic[orderNext.type].uiControllerBase) {
                                    m_nodeInfoDic[orderNext.type].uiControllerBase.Return();
                                }
                            }
                        };
                    }
                }
                if (i == orderList.Count - 1) {
                    m_nodeInfoDic[orderNext.type].uiControllerBase.CloseCallback = openSelf;
                }
            }
            m_nodeInfoDic[orderList[0].type].uiControllerBase.Return();
        } else if (orderList.Count == 1) {
            m_nodeInfoDic[orderList[0].type].uiControllerBase.CloseCallback = openSelf;
            m_nodeInfoDic[orderList[0].type].uiControllerBase.Return();
        } else {
            openSelf();
        }
    }

    /// <summary>
    ///
    /// 递归查找已经打开的子节点,返回需要关闭的链表
    /// </summary>
    /// <param name="type"></param>
    private List<Order> SearchChildNode(NodeInfo nodeInfo, List<Order> orderList) {
        for (int i = 0; i < nodeInfo.childNode.Count; i++) {
            NodeInfo tempNodeInfo = nodeInfo.childNode[i];
            SearchChildNode(tempNodeInfo, orderList);
        }
        if (nodeInfo.uiControllerBase) {
            if (nodeInfo.uiControllerBase.IsActive) {
                Order order = new Order();
                order.type = nodeInfo.type;
                order.state = false;
                orderList.Add(order);
            }
        }
        return orderList;
    }

    /// <summary>
    /// 节点显示前的一系列操作
    /// </summary>
    /// <param name="type"></param>
    private void SearchNodeInfo(NodeInfo currentNode, List<Order> orderList) {
        NodeInfo parentNode = currentNode.parentNode;

        #region 对属性 isStack 的操作（设置导航节点的操作）
        if (currentNode.isStack == true) {
            if (!m_nodeStack.Contains(currentNode.type)) {
                m_nodeStack.Push(currentNode.type);
            }
        }
        #endregion 对属性 isStack 的操作（设置导航节点的操作）
        #region 对属性 isCoexist 的操作（设置兄弟节点的开关）
        if (parentNode.isCoexist == true) {
            int finalDepth = currentNode.uiControllerBase.Depth;
            for (int i = 0; i < parentNode.childNode.Count; i++) {
                NodeInfo nodeInfo = parentNode.childNode[i];
                if (nodeInfo != currentNode) {
                    int tempDepth = DepthChild(nodeInfo);
                    finalDepth = finalDepth <= tempDepth ? tempDepth + 1 : finalDepth;
                }
            }

            currentNode.uiControllerBase.Depth = finalDepth;
        } else {
            for (int i = 0; i < parentNode.childNode.Count; i++) {
                NodeInfo nodeInfo = parentNode.childNode[i];
                if (nodeInfo != currentNode) {
                    SearchChildNode(nodeInfo, orderList);
                }
            }
        }

        #endregion 对属性 isCoexist 的操作（设置兄弟节点的开关）
        #region 对属性 isCover 的操作（设置父节点的开关）
        if (currentNode.isCover != true) {
            if (parentNode != null && parentNode.uiControllerBase != null && parentNode.uiControllerBase.IsActive) {
                Order order = new Order();
                order.type = parentNode.type;
                order.state = false;
                orderList.Add(order);
            }
        }
        #endregion 对属性 isCover 的操作（设置父节点的开关）
        #region 对属性 isControlNode 的操作（设置控制节点的操作）
        if (parentNode.isControlNode && parentNode.type != GlobalDefine.PanelType.RootNode) {
            SearchNodeInfo(parentNode, orderList);
        }
        #endregion 对属性 isControlNode 的操作（设置控制节点的操作）
    }
}