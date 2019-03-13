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
    public static GameObject mananger = null;
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
            OLog.Log(type.ToString() + " not initalization yet!");
        }
        return null;
    }

    public static T GetSimilarContext<T>() where T : class, IContext
    {
        System.Type type = typeof(T);
        foreach(var i in contexts.Values)
        {
            if((i is T))
            {
                return i as T;
            }
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

    //创建对象
    public static T CreateContext<T>() where T : class, IContext
    {
        //创建context
        var types = System.Reflection.Assembly.GetExecutingAssembly().GetTypes();
        System.Type type = typeof(T);
        System.Type baseType = typeof(BaseContext);
        System.Type monoType = typeof(MonoContext);
        foreach (System.Type t in types)
        {
            if (t == type)
            {
                if(t.BaseType == baseType)
                {
                    T newT = t.Assembly.CreateInstance(t.FullName) as T;
                    return newT;
                }
                else
                {
                    if(!mananger)
                    {
                        mananger = new GameObject("@Manager");
                    }
                    return mananger.gameObject.AddComponent(t) as T;
                }
            }
        }
        return null;

    }


    public static void Dispose()
    {
        contexts.Clear();
    }
}
