using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;


/// <summary>
/// 事件处理类。
/// </summary>
public class EventController
{
    private Dictionary<object, Delegate> m_theRouter = new Dictionary<object, Delegate>();

    public Dictionary<object, Delegate> TheRouter
    {
        get { return m_theRouter; }
    }

    /// <summary>
    /// 永久注册的事件列表
    /// </summary>
    private List<object> m_permanentEvents = new List<object>();

    /// <summary>
    /// 标记为永久注册事件
    /// </summary>
    /// <param name="eventType"></param>
    public void MarkAsPermanent(object eventType)
    {
        m_permanentEvents.Add(eventType);
    }

    /// <summary>
    /// 判断是否已经包含事件
    /// </summary>
    /// <param name="eventType"></param>
    /// <returns></returns>
    public bool ContainsEvent(object eventType)
    {
        return m_theRouter.ContainsKey(eventType);
    }

    /// <summary>
    /// 清除非永久性注册的事件
    /// </summary>
    public void Cleanup()
    {
        List<object> eventToRemove = new List<object>();

        foreach (KeyValuePair<object, Delegate> pair in m_theRouter)
        {
            bool wasFound = false;
            foreach (object Event in m_permanentEvents)
            {
                if (pair.Key == Event)
                {
                    wasFound = true;
                    break;
                }
            }

            if (!wasFound)
                eventToRemove.Add(pair.Key);
        }

        foreach (object Event in eventToRemove)
        {
            m_theRouter.Remove(Event);
        }
    }

    //检查，m_theRouter中是否有该eventType的Delegate，如果有判断，listenerBeingAdded类型是否跟该Delegate类型相同，比如都是Action<T>
    //如果不同，就报异常
    /// <summary>
    /// 处理增加监听器前的事项， 检查 参数等
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="listenerBeingAdded"></param>
    private void OnListenerAdding(object eventType, Delegate listenerBeingAdded)
    {
        if (!m_theRouter.ContainsKey(eventType))
        {
            m_theRouter.Add(eventType, null);
        }

        Delegate d = m_theRouter[eventType];

        //关键是listenerBeingAdded.GetType()，是Action、Action<T>、Action<T, U>等这些类型
        //用来判断
        if (d != null && d.GetType() != listenerBeingAdded.GetType())
        {

            //抛异常
            throw new EventException(string.Format(
                    "Try to add not correct event {0}. Current type is {1}, adding type is {2}.",
                    eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
        }
    }

    //判断m_theRouter是否有该eventType的delegate，如果有，就判断listenerBeingRemoved的类型跟该delegate的类型是否相同，相同返回true
    /// <summary>
    /// 移除监听器之前的检查
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="listenerBeingRemoved"></param>
    private bool OnListenerRemoving(object eventType, Delegate listenerBeingRemoved)
    {
        if (!m_theRouter.ContainsKey(eventType))
        {
            return false;
        }

        Delegate d = m_theRouter[eventType];
        if ((d != null) && (d.GetType() != listenerBeingRemoved.GetType()))
        {
            throw new EventException(string.Format(
                "Remove listener {0}\" failed, Current type is {1}, adding type is {2}.",
                eventType, d.GetType(), listenerBeingRemoved.GetType()));
        }
        else
            return true;
    }

    /// <summary>
    /// 移除监听器之后的处理。删掉事件
    /// </summary>
    /// <param name="eventType"></param>
    private void OnListenerRemoved(object eventType)
    {
        if (m_theRouter.ContainsKey(eventType) && m_theRouter[eventType] == null)
        {
            m_theRouter.Remove(eventType);
        }
    }

    #region 增加监听器

    //m_theRouter[eventType]对应该eventType的delegate，使用delegate + ，来添加同一种eventType的delegate
    /// <summary>
    ///  增加监听器， 不带参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void AddEventListener(object eventType, Action handler)
    {
        OnListenerAdding(eventType, handler);
        m_theRouter[eventType] = (Action)m_theRouter[eventType] + handler;
    }

    /// <summary>
    ///  增加监听器， 1个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void AddEventListener<T>(object eventType, Action<T> handler)
    {
        OnListenerAdding(eventType, handler);
        m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] + handler;
    }

    /// <summary>
    ///  增加监听器， 2个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void AddEventListener<T, U>(object eventType, Action<T, U> handler)
    {
        OnListenerAdding(eventType, handler);
        m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] + handler;
    }

    /// <summary>
    ///  增加监听器， 3个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void AddEventListener<T, U, V>(object eventType, Action<T, U, V> handler)
    {
        OnListenerAdding(eventType, handler);
        m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] + handler;
    }

    /// <summary>
    ///  增加监听器， 4个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void AddEventListener<T, U, V, W>(object eventType, Action<T, U, V, W> handler)
    {
        OnListenerAdding(eventType, handler);
        m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] + handler;
    }
    #endregion

    #region 移除监听器

    //先判断m_theRouter是否有该eventType的delegate，有就移出之
    /// <summary>
    ///  移除监听器， 不带参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void RemoveEventListener(object eventType, Action handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            m_theRouter[eventType] = (Action)m_theRouter[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    /// <summary>
    ///  移除监听器， 1个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void RemoveEventListener<T>(object eventType, Action<T> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            m_theRouter[eventType] = (Action<T>)m_theRouter[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    /// <summary>
    ///  移除监听器， 2个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void RemoveEventListener<T, U>(object eventType, Action<T, U> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            m_theRouter[eventType] = (Action<T, U>)m_theRouter[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    /// <summary>
    ///  移除监听器， 3个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void RemoveEventListener<T, U, V>(object eventType, Action<T, U, V> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            m_theRouter[eventType] = (Action<T, U, V>)m_theRouter[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }

    /// <summary>
    ///  移除监听器， 4个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void RemoveEventListener<T, U, V, W>(object eventType, Action<T, U, V, W> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            m_theRouter[eventType] = (Action<T, U, V, W>)m_theRouter[eventType] - handler;
            OnListenerRemoved(eventType);
        }
    }
    #endregion

    #region 触发事件

    //获取该eventType的所有delegate，调用它们
    /// <summary>
    ///  触发事件， 不带参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void TriggerEvent(object eventType)
    {
        Delegate d;
        if (!m_theRouter.TryGetValue(eventType, out d))
        {
            return;
        }

        var callbacks = d.GetInvocationList();
        for (int i = 0; i < callbacks.Length; i++)
        {
            Action callback = callbacks[i] as Action;

            if (callback == null)
            {
                throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
            }

            try
            {

                callback();
            }
            catch (Exception ex)
            {
                //LoggerHelper.Except(ex);
            }
        }
    }

    /// <summary>
    ///  触发事件， 带1个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void TriggerEvent<T>(object eventType, T arg1)
    {
        Delegate d;
        if (!m_theRouter.TryGetValue(eventType, out d))
        {
            return;
        }

        var callbacks = d.GetInvocationList();
        for (int i = 0; i < callbacks.Length; i++)
        {
            Action<T> callback = callbacks[i] as Action<T>;

            if (callback == null)
            {
                throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
            }

            try
            {
                callback(arg1);
            }
            catch (Exception ex)
            {
                //LoggerHelper.Except(ex);
            }
        }
    }

    /// <summary>
    ///  触发事件， 带2个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void TriggerEvent<T, U>(object eventType, T arg1, U arg2)
    {
        Delegate d;
        if (!m_theRouter.TryGetValue(eventType, out d))
        {
            return;
        }
        var callbacks = d.GetInvocationList();
        for (int i = 0; i < callbacks.Length; i++)
        {
            Action<T, U> callback = callbacks[i] as Action<T, U>;

            if (callback == null)
            {
                throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
            }

            try
            {
                callback(arg1, arg2);
            }
            catch (Exception ex)
            {
                //LoggerHelper.Except(ex);
            }
        }
    }

    /// <summary>
    ///  触发事件， 带3个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void TriggerEvent<T, U, V>(object eventType, T arg1, U arg2, V arg3)
    {
        Delegate d;
        if (!m_theRouter.TryGetValue(eventType, out d))
        {
            return;
        }
        var callbacks = d.GetInvocationList();
        for (int i = 0; i < callbacks.Length; i++)
        {
            Action<T, U, V> callback = callbacks[i] as Action<T, U, V>;

            if (callback == null)
            {
                throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
            }
            try
            {
                callback(arg1, arg2, arg3);
            }
            catch (Exception ex)
            {
                //LoggerHelper.Except(ex);
            }
        }
    }

    /// <summary>
    ///  触发事件， 带4个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    public void TriggerEvent<T, U, V, W>(object eventType, T arg1, U arg2, V arg3, W arg4)
    {
        Delegate d;
        if (!m_theRouter.TryGetValue(eventType, out d))
        {
            return;
        }
        var callbacks = d.GetInvocationList();
        for (int i = 0; i < callbacks.Length; i++)
        {
            Action<T, U, V, W> callback = callbacks[i] as Action<T, U, V, W>;

            if (callback == null)
            {
                throw new EventException(string.Format("TriggerEvent {0} error: types of parameters are not match.", eventType));
            }
            try
            {
                callback(arg1, arg2, arg3, arg4);
            }
            catch (Exception ex)
            {
                //LoggerHelper.Except(ex);
            }
        }
    }

    #endregion
}

/// <summary>
/// 事件分发函数。
/// 提供事件注册， 反注册， 事件触发
/// 采用 delegate, dictionary 实现
/// 支持自定义事件。 事件采用字符串方式标识
/// 支持 0，1，2，3 等4种不同参数个数的回调函数
/// </summary>
public class EventDispatcher
{
    //都是调用EventController来触发事件，delegate都保存在EventController中

    private static EventController m_eventController = new EventController();

    public static Dictionary<object, Delegate> TheRouter
    {
        get
        {
            return m_eventController.TheRouter;
        }
    }

    /// <summary>
    /// 标记为永久注册事件
    /// </summary>
    /// <param name="eventType"></param>
    static public void MarkAsPermanent(object eventType)
    {
        m_eventController.MarkAsPermanent(eventType);
    }

    /// <summary>
    /// 清除非永久性注册的事件
    /// </summary>
    static public void Cleanup()
    {
        m_eventController.Cleanup();
    }

    #region 增加监听器
    /// <summary>
    ///  增加监听器， 不带参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener(object eventType, Action handler)
    {
        m_eventController.AddEventListener(eventType, handler);
    }

    /// <summary>
    ///  增加监听器， 1个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener<T>(object eventType, Action<T> handler)
    {
        m_eventController.AddEventListener(eventType, handler);
    }

    /// <summary>
    ///  增加监听器， 2个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener<T, U>(object eventType, Action<T, U> handler)
    {
        m_eventController.AddEventListener(eventType, handler);
    }

    /// <summary>
    ///  增加监听器， 3个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener<T, U, V>(object eventType, Action<T, U, V> handler)
    {
        m_eventController.AddEventListener(eventType, handler);
    }

    /// <summary>
    ///  增加监听器， 4个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void AddEventListener<T, U, V, W>(object eventType, Action<T, U, V, W> handler)
    {
        m_eventController.AddEventListener(eventType, handler);
    }
    #endregion

    #region 移除监听器
    /// <summary>
    ///  移除监听器， 不带参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener(object eventType, Action handler)
    {
        m_eventController.RemoveEventListener(eventType, handler);
    }

    /// <summary>
    ///  移除监听器， 1个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener<T>(object eventType, Action<T> handler)
    {
        m_eventController.RemoveEventListener(eventType, handler);
    }

    /// <summary>
    ///  移除监听器， 2个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener<T, U>(object eventType, Action<T, U> handler)
    {
        m_eventController.RemoveEventListener(eventType, handler);
    }

    /// <summary>
    ///  移除监听器， 3个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener<T, U, V>(object eventType, Action<T, U, V> handler)
    {
        m_eventController.RemoveEventListener(eventType, handler);
    }

    /// <summary>
    ///  移除监听器， 4个参数
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void RemoveEventListener<T, U, V, W>(object eventType, Action<T, U, V, W> handler)
    {
        m_eventController.RemoveEventListener(eventType, handler);
    }
    #endregion

    #region 触发事件
    /// <summary>
    ///  触发事件， 不带参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent(object eventType)
    {
        m_eventController.TriggerEvent(eventType);
    }

    /// <summary>
    ///  触发事件， 带1个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent<T>(object eventType, T arg1)
    {
        m_eventController.TriggerEvent(eventType, arg1);
    }

    /// <summary>
    ///  触发事件， 带2个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent<T, U>(object eventType, T arg1, U arg2)
    {
        m_eventController.TriggerEvent(eventType, arg1, arg2);
    }

    /// <summary>
    ///  触发事件， 带3个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent<T, U, V>(object eventType, T arg1, U arg2, V arg3)
    {
        m_eventController.TriggerEvent(eventType, arg1, arg2, arg3);
    }

    /// <summary>
    ///  触发事件， 带4个参数触发
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="handler"></param>
    static public void TriggerEvent<T, U, V, W>(object eventType, T arg1, U arg2, V arg3, W arg4)
    {
        m_eventController.TriggerEvent(eventType, arg1, arg2, arg3, arg4);
    }

    internal static void TriggerEvent (GlobalDefine.GlobalEvent uI_RoomSwapScene, object leaveGroup) {
        throw new NotImplementedException ();
    }

    internal static void AddEventListener<T>( GlobalDefine.GlobalEvent uI_FriendList ) {
        throw new NotImplementedException ();
    }

    #endregion
}
