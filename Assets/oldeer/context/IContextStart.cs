/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018/07/30 17:44:15
==========================================================================*/

/// <summary>
///  Context����ʼ�Ľӿ�,�³����ĳ�ʼ��������ʼ��Ҫ�̳и�����ܿ�ʼ
/// </summary>
using UnityEngine;
public abstract class IContextStart : MonoBehaviour
{
    //����index
    public int level = 0;

    public IContextStart()
    {
        //�³������Գ�ʼ��һ���ٽ��в���
        //ContextServer.Init();
    }
    public virtual void Awake()
    {
        ContextServer.Init();
    }

}
