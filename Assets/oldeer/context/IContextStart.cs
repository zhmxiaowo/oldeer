/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018/07/30 17:44:15
==========================================================================*/

/// <summary>
///  Context程序开始的接口,新场景的初始化部件开始需要继承该类才能开始
/// </summary>
using UnityEngine;
public abstract class IContextStart : MonoBehaviour
{
    //场景index
    public int level = 0;

    public IContextStart()
    {
        //新场景可以初始化一遍再进行操作
        //ContextServer.Init();
    }
    public virtual void Awake()
    {
        ContextServer.Init();
    }

}
