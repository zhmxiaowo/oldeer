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

/// <summary>
/// context 接口
/// </summary>
public interface IContext
{
    void Init();
}

/// <summary>
/// 非挂载的context
/// </summary>
public abstract class BaseContext : IContext
{
    //注册和调用事件的接口
    protected virtual void Register()
    {

    }
    //初始化会注册到控制中心
    public BaseContext()
    {
        //注册
        ContextServer.OnRegister(this.GetType(), this);
        Init();
    }
    public virtual void Init()
    {

    }
}

/// <summary>
/// 挂载Unity的context
/// </summary>
public abstract class MonoContext : MonoBehaviour,IContext
{

    /// <summary>
    /// 注册和调用事件的接口
    /// </summary>
    protected virtual void Register()
    {

    }

    /// <summary>
    /// 初始化会注册到控制中心
    /// </summary>
    public MonoContext()
    {
        //注册
        ContextServer.OnRegister(this.GetType(), this);
        Init();
    }
    /// <summary>
    /// 初始化接口,在创建后自动调用
    /// </summary>
    public virtual void Init()
    {

    }
}

