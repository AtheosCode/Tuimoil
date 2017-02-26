using UnityEngine;
using System.Collections;
using DG.Tweening;

public class FPSView : UIControllerBase {
    public override void Create() {
        base.Create();
    }
    public override void Open() {
        base.Open();
        AppFacade.Instance.SendNotification(NotiConst.UIMANAGER_OPEN, GlobalDefine.PanelType.SphereView);
    }
    public override void Return() {
        transform.DOScale(new Vector3(0, 0, 0), 2.5f).OnComplete(new TweenCallback(base.Return));
    }
}
