﻿using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections.Generic;

public class UIManagerMediator : Mediator, IMediator {
    public new const string NAME = "UIManagerMediator";
    public UIManagerMediator(object viewComponent = null) : base(NAME, viewComponent) {

    }
    /// <summary>
    /// 需要监听的消息号
    /// </summary>
    public override IEnumerable<string> ListNotificationInterests {
        get {
            List<string> list = new List<string>();
            list.Add(NotiConst.UIMANAGER_RETURN);
            list.Add(NotiConst.UIMANAGER_CREATE);
            list.Add(NotiConst.UIMANAGER_DESTORY);
            list.Add(NotiConst.UIMANAGER_OPEN);
            return list;
        }
    }

    /// <summary>
    /// 接收消息到消息之后处理
    /// </summary>
    /// <param name="notification"></param>
    public override void HandleNotification(INotification notification) {
        string name = notification.Name;
        object vo = notification.Body;
        UIManagerView view = ViewComponent as UIManagerView;
        switch (name) {
            case NotiConst.UIMANAGER_RETURN:
                view.DoReturn((GlobalDefine.PanelType)vo);
                break;
            case NotiConst.UIMANAGER_CREATE:
                view.DoCreate((GlobalDefine.PanelType)vo);
                break;

            case NotiConst.UIMANAGER_DESTORY:
                view.DoDestroy((GlobalDefine.PanelType)vo);
                break;
            case NotiConst.UIMANAGER_OPEN:
                view.DoOpen((GlobalDefine.PanelType)vo);
                break;
        }
    }
}