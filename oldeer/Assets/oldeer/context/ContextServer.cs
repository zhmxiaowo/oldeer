/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018.7.27
============================================================================*/
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
            Debug.Log(t + " Context已经存在!");
        }
        else
        {
            contexts.Add(t, c);
        }

    }
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
            //            contexts.Remove(type);
            //            contexts.Add(type,c);
            Debug.Log(type.ToString() + "尚未初始化");
        }
        return null;
    }
}
