using UnityEngine;
using System.Collections;
using PureMVC.Patterns;

public class Main:MonoBehaviour{
    public AppFacade appFacade;
    public void Awake() {
        DontDestroyOnLoad(this);
        appFacade = new AppFacade();
        appFacade.StartUp();//(Facade.GetInstance() as AppFacade).StartUp();
    }
}
