/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018.7.27
==========================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ContextServer {

    //context中心
    static Dictionary<System.Type, IContext> contexts = new Dictionary<System.Type, IContext>();
    
    //注册
    public static void OnRegister(System.Type t, IContext c)
    {
        //Debug.Log(t.Name);
        if (contexts.ContainsKey(t))
        {
            OLog.Log(t + " Context already exist!");
        }
        else
        {
            contexts.Add(t, c);
        }
    }
    //获取对象
    public static T GetContext<T>() where T : class,IContext
    {
        System.Type type = typeof(T);
        IContext c = null;
        if (contexts.TryGetValue(type, out c))
        {
            return c as T;
        }
        else
        {
            OLog.Log(type.ToString() + "not initalization yet!");
        }
        return null;
    }
    //初始化
    public static void Init()
    {
        if(contexts != null)
        {
            contexts.Clear();
        }
        else
        {
            contexts = new Dictionary<System.Type, IContext>();
        }
        OLog.Log("=====>context init<======");
    }
}
