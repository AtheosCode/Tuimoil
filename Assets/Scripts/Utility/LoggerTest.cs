using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class MyLogHandler : ILogHandler
{
    public void LogFormat(LogType logType, UnityEngine.Object context, string format, params object[] args)
    {
        Debug.logger.logHandler.LogFormat(logType, context, format, args);
    }

    public void LogException(Exception exception, UnityEngine.Object context)
    {
        Debug.logger.LogException(exception, context);
    }
}

public class LoggerTest : MonoBehaviour
{
    private static string kTAG = "MyGameTag";
    private Logger myLogger;

    void Start()
    {
        myLogger = new Logger(new MyLogHandler());
        myLogger.Log(kTAG, "MyGameClass Start.");
        myLogger.logEnabled = false;
        myLogger.Log(kTAG, "MyGameClass Start.");
    }
}