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

namespace oldeer
{
    //只创建一次,并且永不销毁,用于流程梳理 写入一些只执行一次的操作
    public class AppInit : MonoBehaviour
    {

        //语言 0 中文 1 英文
        public int language = 0;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        // Use this for initialization
        void Start()
        {

        }

        //保存调用方法
        private static List<System.Type> addtionInitOnceList = new List<System.Type>();

        /// <summary>
        /// 只初始化一次的方法可以在这里附加调用,可以有效防止多次调用,程序运行到结束,多次调用,方法也只执行一次
        /// </summary>
        /// <param name="sender">调用的的主要类,通常为this</param>
        /// <param name="action">调用的方法</param>
        public static void AddInit(object sender, System.Action action)
        {
            System.Type type = sender.GetType();
            if (!addtionInitOnceList.Contains(type))
            {
                addtionInitOnceList.Add(type);
                //调用一次
                action();
            }
        }

    }
}

