using UnityEngine;
using System.Collections;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class UIManagerProxy : Proxy,IProxy {
    public new const string NAME = "UIManagerProxy";

    public override void OnRegister() {
        base.OnRegister();
    }

    public override void OnRemove() {
        base.OnRemove();
    }
}
