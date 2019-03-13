/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2019/03/13 11:10:29
============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// name:事件管理
/// version:1.0
/// describe:
/// author:gaozhijie
/// time:2018.09.02
/// 
/// osmin
/// time 2018.10.9
/// 重大修改：使用方法不变，现在可以使用删除单个注册方法了
/// 自行维护：AddListener 一定要 RemoveListener
/// </summary>
static internal class Messenger
{
    static public Dictionary<string, LinkedList<Delegate>> mEventTable = new Dictionary<string, LinkedList<Delegate>>();

    static public List<string> mPermanentMessages = new List<string>();


    /// <summary>
    /// 将信息标记为永久信息
    /// </summary>
    /// <param name="eventType"></param>
    static public void MarkAsPermanent(string eventType)
    {
#if LOG_ALL_MESSAGES
        Debug.Log("Messenger MarkAsPermanent \t\"" + eventType + "\"");
#endif

        mPermanentMessages.Add(eventType);
    }

    /// <summary>
    /// 移除ALL信息
    /// </summary>
    static public void Cleanup()
    {
#if LOG_ALL_MESSAGES
        Debug.Log("MESSENGER Cleanup. Make sure that none of necessary listeners are removed.");
#endif

        List<string> messagesToRemove = new List<string>();

        foreach (KeyValuePair<string, LinkedList<Delegate>> pair in mEventTable)
        {
            bool wasFound = false;

            foreach (string message in mPermanentMessages)
            {
                if (pair.Key == message)
                {
                    wasFound = true;
                    break;
                }
            }

            if (!wasFound)
                messagesToRemove.Add(pair.Key);
        }

        foreach (string message in messagesToRemove)
        {
            mEventTable.Remove(message);
        }
    }

    static public void PrEGameEventEventTable()
    {
        Debug.Log("\t\t\t=== MESSENGER PrEGameEventEventTable ===");

        foreach (KeyValuePair<string, LinkedList<Delegate>> pair in mEventTable)
        {
            Debug.Log("\t\t\t" + pair.Key + "\t\t" + pair.Value);
        }

        Debug.Log("\n");
    }
    /// <summary>
    /// 添加监听
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="listenerBeingAdded"></param>
    static public bool OnListenerAdding(string eventType, Delegate listenerBeingAdded)
    {
#if LOG_ALL_MESSAGES || LOG_ADD_LISTENER
        Debug.Log("MESSENGER OnListenerAdding \t\"" + eventType + "\"\t{" + listenerBeingAdded.Target + " -> " + listenerBeingAdded.Method + "}");
#endif

        if (!mEventTable.ContainsKey(eventType))
        {
            mEventTable.Add(eventType, new LinkedList<Delegate>());
        }
        //frist可以代表当前整个list的类型
        var d = mEventTable[eventType].First;
        if (d != null && d.Value.GetType() != listenerBeingAdded.GetType())
        {
            throw new ListenerException(string.Format("Attempting to add listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being added has type {2}", eventType, d.GetType().Name, listenerBeingAdded.GetType().Name));
            //Debug.LogError(string.Format(">>> name:{0} type:{1} add type:{2}!", eventType, d, listenerBeingAdded.GetType()));
            return false;
        }
        return true;
    }
    /// <summary>
    /// 移除监听
    /// </summary>
    /// <param name="eventType"></param>
    /// <param name="listenerBeingRemoved"></param>
    static public bool OnListenerRemoving(string eventType, Delegate listenerBeingRemoved)
    {
#if LOG_ALL_MESSAGES
        Debug.Log("MESSENGER OnListenerRemoving \t\"" + eventType + "\"\t{" + listenerBeingRemoved.Target + " -> " + listenerBeingRemoved.Method + "}");
#endif

        if (mEventTable.ContainsKey(eventType))
        {
            var d = mEventTable[eventType].First;

            if (d == null)
            {
                Debug.LogError(string.Format(">>>name:{0} remove type is null:{1}", eventType, listenerBeingRemoved.GetType()));
                //throw new ListenerException(string.Format("Attempting to remove listener with for event type \"{0}\" but current listener is null.", eventType));
                return false;
            }
            else if (d.Value.GetType() != listenerBeingRemoved.GetType())
            {
                Debug.LogError(string.Format(">>>name{0} type:{1} remove type:{2}", eventType, d.Value.GetType(), listenerBeingRemoved.GetType()));
                //throw new ListenerException(string.Format("Attempting to remove listener with inconsistent signature for event type {0}. Current listeners have type {1} and listener being removed has type {2}", eventType, d.GetType().Name, listenerBeingRemoved.GetType().Name));
                return false;
            }
            return true;
        }
        else
        {
            Debug.Log(">>>no such event" + eventType);
            //throw new ListenerException(string.Format("Attempting to remove listener for type \"{0}\" but Messenger doesn't know about this event type.", eventType));
            return false;
        }
    }

    static public void OnListenerRemoved(string eventType)
    {
        if (mEventTable.ContainsKey(eventType) && mEventTable[eventType].First == null)
        {
            mEventTable.Remove(eventType);
        }
    }

    //移除该事件的所有监听
    public static void RemoveAllListener(string eventType)
    {
        if(mEventTable.ContainsKey(eventType))
        {
            mEventTable.Remove(eventType);
        }

    }
    /// <summary>
    /// 广播
    /// </summary>
    /// <param name="eventType"></param>
    static public void OnBroadcasting(string eventType)
    {
#if REQUIRE_LISTENER
        if (!mEventTable.ContainsKey(eventType)) {
        }
#endif
    }

    static public BroadcastException CreateBroadcastSignatureException(string eventType)
    {
        return new BroadcastException(string.Format("Broadcasting message \"{0}\" but listeners have a different signature than the broadcaster.", eventType));
    }

    public class BroadcastException : Exception
    {
        public BroadcastException(string msg)
            : base(msg)
        {
        }
    }

    public class ListenerException : Exception
    {
        public ListenerException(string msg)
            : base(msg)
        {
        }
    }

    //无参
    static public void AddListener(string eventType, Callback handler)
    {
        if(OnListenerAdding(eventType, handler))
        {
            mEventTable[eventType].AddLast(handler);
        }
        //mEventTable[eventType] = (Callback)mEventTable[eventType] + handler;

    }

    //单个参数
    static public void AddListener<T>(string eventType, Callback<T> handler)
    {
        if (OnListenerAdding(eventType, handler))
        {
            mEventTable[eventType].AddLast(handler);
        }
    }

    //两个参数
    static public void AddListener<T, U>(string eventType, Callback<T, U> handler)
    {
        if (OnListenerAdding(eventType, handler))
        {
            mEventTable[eventType].AddLast(handler);
        }
    }

    //三个参数
    static public void AddListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        if (OnListenerAdding(eventType, handler))
        {
            mEventTable[eventType].AddLast(handler);
        }
    }

    //四个参数
    static public void AddListener<T, U, V, X>(string eventType, Callback<T, U, V, X> handler)
    {
        if (OnListenerAdding(eventType, handler))
        {
            mEventTable[eventType].AddLast(handler);
        }
    }



    static public void RemoveListener(string eventType, Callback handler)
    {
        if(OnListenerRemoving(eventType, handler))
        {
            mEventTable[eventType].Remove(handler);
        }
        OnListenerRemoved(eventType);
    }


    static public void RemoveListener<T>(string eventType, Callback<T> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            mEventTable[eventType].Remove(handler);
        }
        OnListenerRemoved(eventType);
    }


    static public void RemoveListener<T, U>(string eventType, Callback<T, U> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            mEventTable[eventType].Remove(handler);
        }
        OnListenerRemoved(eventType);
    }

    static public void RemoveListener<T, U, V>(string eventType, Callback<T, U, V> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            mEventTable[eventType].Remove(handler);
        }
        OnListenerRemoved(eventType);
    }


    static public void RemoveListener<T, U, V, X>(string eventType, Callback<T, U, V, X> handler)
    {
        if (OnListenerRemoving(eventType, handler))
        {
            mEventTable[eventType].Remove(handler);
        }
        OnListenerRemoved(eventType);
    }


    static public void Broadcast(string eventType)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        LinkedList<Delegate> d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            var node = d.First;
            while (node != null)
            {
                Callback callback = node.Value as Callback;

                if (callback != null)
                {
                    callback();
                }
                else
                {
                    //remove this action
                    d.Remove(node);
                }
                node = node.Next;
            }
        }
    }

    //static public void SendEvent(CEvent evt)
    //{
    //    Broadcast<CEvent>(evt.GetEventId(), evt);
    //}


    static public void Broadcast<T>(string eventType, T arg1)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        LinkedList<Delegate> d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            var node = d.First;
            while (node != null)
            {
                Callback<T> callback = node.Value as Callback<T>;

                if (callback != null)
                {
                    callback(arg1);
                }
                else
                {
                    //remove this action
                    d.Remove(node);
                }
                node = node.Next;
            }
        }
    }


    static public void Broadcast<T, U>(string eventType, T arg1, U arg2)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        LinkedList<Delegate> d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            var node = d.First;
            while (node != null)
            {
                Callback<T,U> callback = node.Value as Callback<T,U>;

                if (callback != null)
                {
                    callback(arg1,arg2);
                }
                else
                {
                    //remove this action
                    d.Remove(node);
                }
                node = node.Next;
            }
        }
    }


    static public void Broadcast<T, U, V>(string eventType, T arg1, U arg2, V arg3)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        LinkedList<Delegate> d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            var node = d.First;
            while (node != null)
            {
                Callback<T,U,V> callback = node.Value as Callback<T,U,V>;

                if (callback != null)
                {
                    callback(arg1,arg2,arg3);
                }
                else
                {
                    //remove this action
                    d.Remove(node);
                }
                node = node.Next;
            }
        }
    }


    static public void Broadcast<T, U, V, X>(string eventType, T arg1, U arg2, V arg3, X arg4)
    {
#if LOG_ALL_MESSAGES || LOG_BROADCAST_MESSAGE
        Debug.Log("MESSENGER\t" + System.DateTime.Now.ToString("hh:mm:ss.fff") + "\t\t\tInvoking \t\"" + eventType + "\"");
#endif
        OnBroadcasting(eventType);

        LinkedList<Delegate> d;
        if (mEventTable.TryGetValue(eventType, out d))
        {
            var node = d.First;
            while (node != null)
            {
                Callback<T,U,V,X> callback = node.Value as Callback<T,U,V,X>;

                if (callback != null)
                {
                    callback(arg1,arg2,arg3,arg4);
                }
                else
                {
                    //remove this action
                    d.Remove(node);
                }
                node = node.Next;
            }
        }
    }


    //#region 拓展方法
    //public static System.Type GetListType<T>(this LinkedList<T> _)
    //{
    //    return typeof(T);
    //}
    //#endregion

}