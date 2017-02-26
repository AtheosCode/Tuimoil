using System;
using System.Collections.Generic;
using UnityEngine;

//++ 本地时间原点为 1970.0.0； 所有时间为当地时间而不是格林威治标准时间

/// <summary>
/// 时间数据类
/// </summary>
public class ScheduleData {
    ///// <summary>
    ///// 这个时间事件触发次数
    ///// </summary>
    //public int schedulingTimes;
    /// <summary>
    /// 这个时间事件间隔多少时间后触发回调函数
    /// </summary>
    public Action callback;
    /// <summary>
    /// 开始时间
    /// </summary>
    public double sartTime;
    /// <summary>
    /// 这个时间事件唯一id
    /// </summary>
    public int schedulerID;
    /// <summary>
    /// 计时刷新数据
    /// </summary>
    public List<SchedulerRefresh> schedulerRefreshList = new List<SchedulerRefresh>();
    ///// <summary>
    ///// 这个时间事件间隔多少时间后触发
    ///// </summary>
    //public double continuedTime;
    /// <summary>
    /// 目标时间
    /// </summary>
    public double targetTime;
}

//    public CDownTimeData(double fromTime) {
//        resetTime = fromTime;
//    }
//}
/// <summary>
/// 计时刷新数据
/// </summary>
public class SchedulerRefresh {
    /// <summary>
    /// 显示模式
    /// </summary>
    public int showModel;
    /// <summary>
    /// 需要刷新的label
    /// </summary>
    public Transform target;
}

public class TimeScheduler : MonoSingleton<TimeScheduler> {

    /// <summary>
    /// 倒计时显示枚举类型
    /// </summary>
    public enum EActiveState {
        /// <summary>
        /// 还未到显示时间或已经结束
        /// </summary>
        ActiveNoShow = 0,

        /// <summary>
        /// 已经到显示时间但是还未到开始时间
        /// </summary>
        ActiveNoStart = 1,

        /// <summary>
        /// 活动进行中
        /// </summary>
        ActiveStarting = 2
    }

    /// <summary>
    /// 重置周期
    /// </summary>
    public enum EResetType {
        /// <summary>
        /// 一次性任务
        /// </summary>
        NoReset = 0,

        /// <summary>
        /// 每日重置
        /// </summary>
        ResetDaily = 1,

        /// <summary>
        /// 7日重置
        /// </summary>
        ResetWeekly = 2,

        /// <summary>
        /// 1月重置
        /// </summary>
        ResetMonthly = 3,
    }

    /// <summary>
    /// 原点日期（1970,1,1）用于转换C#时间和通用时间起点不同
    /// </summary>
    public readonly static System.DateTime ORIGINTIME = new System.DateTime(1970, 1, 1);
    /// <summary>
    /// 一天的秒数
    /// </summary>
    private readonly int SECONDDAY = 24 * 3600;
    /// <summary>
    /// 游戏上线时的客户端时间
    /// </summary>
    private double launchClientTime;
    /// <summary>
    /// 游戏上线时的服务器时间
    /// </summary>
    private double launchServerTime = 0;
    /// <summary>
    /// 需要刷新的Label字典
    /// </summary>
    private Dictionary<Transform, int> m_labelUpdateList = new Dictionary<Transform, int>();
    /// <summary>
    /// 计时操作List字典
    /// </summary>
    private List<ScheduleData> m_timeSchedulerList = new List<ScheduleData>();
    /// <summary>
    /// 计时操作的唯一ID标识
    /// </summary>
    private int schedulerID = 0;
    /// <summary>
    /// 计算的服务器当前日期
    /// </summary>
    private System.DateTime serverCurDate;
    /// <summary>
    /// 计算的服务器当前时间
    /// </summary>
    private double serverCurTime;
    /// <summary>
    /// 游戏上线时的客户端时间(1.1.1)
    /// </summary>
    public double LaunchClientTime {
        get {
            return launchClientTime;
        }

        set {
            this.launchClientTime = value;
        }
    }
    /// <summary>
    /// 游戏上线时的服务器时间(1.1.1)
    /// </summary>
    public double LaunchServerTime {
        get {
            return launchServerTime;
        }

        private set {
            this.launchServerTime = value;
        }
    }
    /// <summary>
    /// 计算的服务器当前日期(1.1.1)
    /// </summary>
    public System.DateTime ServerCurDate {
        get {
            //serverCurDate = ORIGINTIME.AddSeconds(LaunchServerTime + Time.realtimeSinceStartup);
            serverCurDate = SecondsToDateTime(ServerCurTime);
            return serverCurDate;
        }
    }
    /// <summary>
    /// 计算的服务器当前时间(1.1.1)
    /// </summary>
    public double ServerCurTime {
        get {
            //serverCurTime = (ServerCurDate - ORIGINTIME).TotalSeconds;
            serverCurTime = LaunchServerTime + Time.realtimeSinceStartup;
            return serverCurTime;
        }
    }

    #region 工具方法

    /// <summary>
    /// 服务器时间（1970.1.1 0时区）转换成本地程序可用时间（1.1.1 当地时区）
    /// </summary>
    /// <param name="serverTime"></param>
    /// <returns></returns>
    public DateTime ConvertUtcToLocalTime(double serverTime) {
        return ORIGINTIME.AddSeconds(serverTime).ToLocalTime();
    }

    /// <summary>
    /// 日期转秒
    /// </summary>
    /// <param name="dateTime">日期（1.1.1）</param>
    /// <returns></returns>
    public double DateTimeToSeconds(DateTime dateTime) {
        return (dateTime - DateTime.MinValue).TotalSeconds;
    }

    /// <summary>
    /// 秒转日期
    /// </summary>
    /// <param name="seconds">秒（1.1.1）</param>
    /// <returns></returns>
    public System.DateTime SecondsToDateTime(double seconds) {
        return DateTime.MinValue.AddSeconds(seconds);
    }
    #endregion 工具方法

    public void FixedUpdate() {
        ScheduleData scheduleData;
        SchedulerRefresh schedulerRefresh;
        double dif;
        for (int i = 0; i < m_timeSchedulerList.Count; i++) {
            scheduleData = m_timeSchedulerList[i];
            dif = Instance.GetTimeScheduler(scheduleData.schedulerID);
            for (int j = 0; j < scheduleData.schedulerRefreshList.Count; j++) {
                schedulerRefresh = scheduleData.schedulerRefreshList[j];
                if (schedulerRefresh.target != null && m_labelUpdateList.ContainsKey(schedulerRefresh.target) && m_labelUpdateList[schedulerRefresh.target] == scheduleData.schedulerID) {
                    UILabel label = schedulerRefresh.target.GetComponent<UILabel>();
                    if (label != null) {
                        string stringFormat;
                        if (schedulerRefresh.showModel == 0) {
                            stringFormat = "{1:00}:{2:00}:{3:00}";
                        } else if (schedulerRefresh.showModel == 1) {
                            stringFormat = "[8B6A4F]折扣活动剩余时间：[-][32A227]{0}天{1:00}时{2:00}分{3:00}秒[-]";
                        } else if (schedulerRefresh.showModel == 2) {
                            stringFormat = "[8B6A4F]全服限购剩余时间：[-][32A227]{0}天{1:00}时{2:00}分{3:00}秒[-]";
                        } else {
                            stringFormat = "{1:00}:{2:00}:{3:00}";
                        }
                        DateTime tempDate = new DateTime();
                        if (dif > 0.5) {
                            tempDate = DateTime.MinValue.AddSeconds(dif);
                            label.text = string.Format(stringFormat, tempDate.DayOfYear - 1, tempDate.Hour, tempDate.Minute, tempDate.Second);
                        } else if (dif >= 0) {
                            label.text = string.Format(stringFormat, 0, 0, 0, 0);
                        }
                    } else {
                        Debug.LogError("传入刷新的transform不包含UILabel组件，请检查");
                    }
                } else {
                    scheduleData.schedulerRefreshList.Remove(schedulerRefresh);
                    j--;
                }
            }
        }
    }

    /// <summary>
    /// 登录时本地时间
    /// private float gameStartFixedTime_ = 0
    /// 刚登入游戏时同步初始化本地时间
    /// </summary>
    /// <param name="serverTime"></param>
    public void InitDateTime(int serverTime) {
        LaunchServerTime = DateTimeToSeconds(ORIGINTIME.AddSeconds(serverTime).ToLocalTime());
        LaunchClientTime = DateTimeToSeconds(System.DateTime.Now);//Time.realtimeSinceStartup;
        Debug.Log("LaunchServerTime：" + SecondsToDateTime(LaunchServerTime).ToString() + "   LaunchClientTime:" + SecondsToDateTime(LaunchClientTime).ToString());
        //gameStartPassLength_ = gameStartLength_;
        //gameStartFixedTime_ = Time.fixedTime;
        UpdateAllZeroTime();
    }

    #region 临时缓存数据
    public double gameCurDayZeroTime;
    public double gameCurMonthZeroTime;
    public double gameCurWeekZeroTime;
    private System.DateTime gameCurDayZeroDateTime;
    #endregion 临时缓存数据
    #region 计时操作

    /// <summary>
    /// 注册计时操作
    /// </summary>
    /// <param name="continuedTime">多少秒以后</param>
    /// <param name="schedulerHandler">计时到了的操作</param>
    /// <returns></returns>
    public int AddTimeScheduler(double continuedTime, Action schedulerHandler = null) {
        ScheduleData shedulerData = new ScheduleData();
        shedulerData.schedulerID = CreateStaticID();
        shedulerData.sartTime = ServerCurTime;
        shedulerData.targetTime = ServerCurTime + continuedTime;
        shedulerData.callback = schedulerHandler;
        int index = m_timeSchedulerList.FindIndex(x => x.schedulerID == shedulerData.schedulerID);
        if (index >= 0) {
            m_timeSchedulerList[schedulerID] = shedulerData;
        } else {
            m_timeSchedulerList.Add(shedulerData);
        }
        return shedulerData.schedulerID;
    }

    /// <summary>
    /// 注册计时操作
    /// </summary>
    /// <param name="targetDateTime">目标结束日期(当地日期)</param>
    /// <param name="schedulerHandler"></param>
    /// <returns></returns>
    public int AddTimeScheduler(DateTime targetDateTime, Action schedulerHandler = null) {
        ScheduleData shedulerData = new ScheduleData();
        shedulerData.schedulerID = CreateStaticID();
        shedulerData.sartTime = ServerCurTime;
        shedulerData.targetTime = (targetDateTime - DateTime.MinValue).TotalSeconds;
        shedulerData.callback = schedulerHandler;
        int index = m_timeSchedulerList.FindIndex(x => x.schedulerID == shedulerData.schedulerID);
        if (index >= 0) {
            m_timeSchedulerList[schedulerID] = shedulerData;
        } else {
            m_timeSchedulerList.Add(shedulerData);
        }
        return shedulerData.schedulerID;
    }

    /// <summary>
    /// 获取计时任务剩余时间
    /// 返回结果0 表示结束 -1 表示错误 大于0 正常
    /// </summary>
    /// <param name="schedulerID">计时ID</param>
    /// <returns>0 表示结束 -1 表示错误 大于0 正常</returns>
    public double GetTimeScheduler(int schedulerID) {
        ScheduleData schedulerData;
        int index = m_timeSchedulerList.FindIndex(x => x.schedulerID == schedulerID);
        if (index >= 0) {
            schedulerData = m_timeSchedulerList[index];
            double dif = schedulerData.targetTime - ServerCurTime;
            if (dif > 0) {
                DateTime dateTime = DateTime.MinValue;
                dateTime = dateTime.AddSeconds(dif);
                return dif;
            } else {
                if (schedulerData.callback != null) {
                    schedulerData.callback();
                    RemoveTimeScheduler(schedulerID);
                    Debug.Log("mission complete");
                }
                return 0;
            }
        } else {
            Debug.Log("计时任务已完成或未注册");
            return -1;
        }
    }

    /// <summary>
    /// 从计划时间List 中移除
    /// </summary>
    /// <param name="iSchedulerID"></param>
    /// <returns></returns>
    public bool RemoveTimeScheduler(int schedulerID) {
        bool bRet = false;
        int index = m_timeSchedulerList.FindIndex(x => x.schedulerID == schedulerID);
        if (index >= 0) {
            m_timeSchedulerList.Remove(m_timeSchedulerList[index]);
            bRet = true;
        }
        return bRet;
    }

    /// <summary>
    /// 计时操作唯一标识ID 增加
    /// </summary>
    /// <returns></returns>
    private int CreateStaticID() {
        return ++schedulerID;
    }
    #endregion 计时操作
    #region 计时刷新Label

    /// <summary>
    /// 增加新的label到刷新列表
    /// </summary>
    /// <param name="target"></param>
    /// <param name="schedulerID"></param>
    public void AddSchedulerLabel(Transform target, int schedulerID, int showModel = 0) {
        //int index = m_timeSchedulerList.FindIndex(x => x.schedulerRefreshList.FindIndex(y => y.target == target) >= 0);
        int index = m_timeSchedulerList.FindIndex(x => x.schedulerID == schedulerID);
        if (index >= 0) {
            SchedulerRefresh data = new SchedulerRefresh();
            data.showModel = showModel;
            data.target = target;
            ScheduleData scheduleData = m_timeSchedulerList[index];
            index = scheduleData.schedulerRefreshList.FindIndex(x => x.target == target);
            if (index >= 0) {
                scheduleData.schedulerRefreshList[index] = data;
            } else {
                scheduleData.schedulerRefreshList.Add(data);
            }
        } else {
            target.GetComponent<UILabel>().text = "";
            Debug.Log("该计时事件ID：" + schedulerID + "没有注册");
        }
        if (m_labelUpdateList.ContainsKey(target)) {
            m_labelUpdateList[target] = schedulerID;
        } else {
            m_labelUpdateList.Add(target,schedulerID);
        }
    }

    /// <summary>
    /// 移除不需要刷新的label
    /// </summary>
    /// <param name="target"></param>
    public void RemoveSchedulerLabel(Transform target) {
        int index = -1;
        ScheduleData scheduleData;
        for (int i = 0; i < m_timeSchedulerList.Count; i++) {
            scheduleData = m_timeSchedulerList[i];
            index = scheduleData.schedulerRefreshList.FindIndex(x => x.target == target);
            if (index >= 0) {
                scheduleData.schedulerRefreshList.Remove(scheduleData.schedulerRefreshList[index]);
                i--;
            }
        }

        if (m_labelUpdateList.ContainsKey(target)) {
            m_labelUpdateList.Remove(target);
        }
    }
    #endregion 计时刷新Label
    #region 工具方法

    /// <summary>
    /// 每天零点时间
    /// </summary>
    /// <param name="today"></param>
    /// <returns></returns>
    private DateTime GetDayZeroTime(DateTime today) {
        return new System.DateTime(today.Year, today.Month, today.Day, 0, 0, 0);
    }

    /// <summary>
    /// 获取当前周的周一时间
    /// </summary>
    /// <param name="today"></param>
    /// <returns></returns>
    private DateTime GetEveryWeekMonday(DateTime today) {
        if (today.DayOfWeek == DayOfWeek.Sunday) {
            return today.AddDays(-6);
        } else {
            return today.AddDays(1 - (int)today.DayOfWeek);
        }
    }

    /// <summary>
    /// 每月一号时间
    /// </summary>
    /// <param name="today"></param>
    /// <returns></returns>
    private DateTime GetMonthZeroTime(DateTime today) {
        return new System.DateTime(today.Year, today.Month, 1, 0, 0, 0);
    }

    /// <summary>
    /// 更新初始时间 每天零点，每周周一，每月一号
    /// </summary>
    private void UpdateAllZeroTime() {
        System.DateTime gameTime = ServerCurDate;
        gameCurDayZeroDateTime = GetDayZeroTime(gameTime);
        gameCurDayZeroTime = (gameCurDayZeroDateTime - ORIGINTIME).TotalSeconds;
        gameCurWeekZeroTime = (GetDayZeroTime(GetEveryWeekMonday(gameTime)) - ORIGINTIME).TotalSeconds;
        gameCurMonthZeroTime = (GetMonthZeroTime(gameTime) - ORIGINTIME).TotalSeconds;
    }
    #endregion 工具方法
}