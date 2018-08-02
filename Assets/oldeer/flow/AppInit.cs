/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018/07/30 20:50:10
============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//只创建一次,并且永不销毁,用于流程梳理 写入一些只执行一次的操作
public class AppInit : MonoBehaviour {

    //语言 0 中文 1 英文
    public int language = 0;
    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }
    // Use this for initialization
    void Start ()
    {
		
	}

}
