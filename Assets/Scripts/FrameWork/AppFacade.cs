using UnityEngine;
using System.Collections;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class AppFacade : Facade, IFacade {
    protected override void InitializeView() {
        base.InitializeView();
    }
    protected override void InitializeController() {
        base.InitializeController();
        RegisterCommand(NotiConst.STARTUP, new StartupCommand());
    }
    protected override void InitializeFacade() {
        base.InitializeFacade();
    }
    protected override void InitializeModel() {
        base.InitializeModel();
    }
    public override string ToString() {
        return base.ToString();
    }
    public void StartUp() {
        SendNotification(NotiConst.STARTUP);
        SendNotification(NotiConst.UIMANAGER_OPEN, GlobalDefine.PanelType.Login);
        Debug.Log("测试PureMVC启动" + Time.realtimeSinceStartup);
    }
}
