/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018.7.30
============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// 后续需要添加持久化log日志工具
/// </summary>
//辅助log的工具
public class OLog {
    //是否使用log
    public static bool use = true;

    //输出基本数据
    static public void Log(object message)
    {
        Log(message, null);
    }
    static public void Log(object message, Object context)
    {
        if (use)
        {
            Debug.Log(message, context);
        }
    }
    //输出错误
    static public void LogError(object message)
    {
        LogError(message, null);
    }
    static public void LogError(object message, Object context)
    {
        if (use)
        {
            Debug.LogError(message, context);
        }
    }
    //输出警告
    static public void LogWarning(object message)
    {
        LogWarning(message, null);
    }
    static public void LogWarning(object message, Object context)
    {
        if (use)
        {
            Debug.LogWarning(message, context);
        }
    }
}
