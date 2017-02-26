using UnityEngine;
using System.Collections;

public class FinalView : UIControllerBase {
    public override void Create() {
        base.Create();
    }
    public override void Open() {
        base.Open();
        //AppFacade.Instance.SendNotification(NotiConst.UIMANAGER_OPEN, GlobalDefine.PanelType.SphereView);
    }
}
