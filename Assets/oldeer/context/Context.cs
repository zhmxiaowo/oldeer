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

//context 接口
public interface IContext
{

    //注册和调用事件的接口
    //protected virtual void Register()
    //{

    //}

    // 初始化后会注册到主事件中心
    //IContext();
}



//非挂载的context
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
    }
}

//挂载的context
public abstract class MonoContext : MonoBehaviour,IContext
{

    //注册和调用事件的接口
    protected virtual void Register()
    {

    }

    //初始化会注册到控制中心
    public MonoContext()
    {
        //注册
        ContextServer.OnRegister(this.GetType(), this);
    }

}

