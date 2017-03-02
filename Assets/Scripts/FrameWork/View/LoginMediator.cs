using PureMVC.Interfaces;
using PureMVC.Patterns;
using System.Collections.Generic;

public class LoginMediator : Mediator, IMediator {
    public new const string NAME = "LoginMediator";

    //需要监听的消息号
    public override IEnumerable<string> ListNotificationInterests {
        get {
            List<string> list = new List<string>();
            //list.Add(NotiConst.UIMANAGER_RETURN);
            //list.Add(NotiConst.UIMANAGER_CREATE);
            //list.Add(NotiConst.UIMANAGER_DESTORY);
            //list.Add(NotiConst.UIMANAGER_OPEN);
            return list;
        }
    }

    public LoginMediator(object viewComponent = null) : base(NAME, viewComponent) {
    }

    //接收消息到消息之后处理
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