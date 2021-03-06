﻿/********************************************************************
	created:	2016/08/18
	created:	18:8:2016   22:20
	filename: 	f:\users\administrator\projects\miaobox\miaobox\miaoboxmvc\assets\scripts\framework\controller\macrocommands\startupcommand.cs
	file path:	f:\users\administrator\projects\miaobox\miaobox\miaoboxmvc\assets\scripts\framework\controller\macrocommands
	file base:	startupcommand
	file ext:	cs
	author:		Zhou Jingren
	
	purpose:	启动Command
*********************************************************************/
using UnityEngine;
using System.Collections;
using PureMVC.Patterns;
using PureMVC.Interfaces;

public class StartupCommand : SimpleCommand {
    public override void Execute(INotification notification) {
        base.Execute(notification);
        Debug.Log("StartUpCommand 操作被调用" + Time.realtimeSinceStartup);
        Facade.RegisterMediator(new UIManagerMediator(UIManagerView.Instance));
    }
}