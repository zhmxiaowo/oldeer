/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018.7.27
============================================================================*/
using System;
using System.Collections.Generic;
using UnityEngine;

namespace oldeer
{
    /// <summary>
    /// 模板类 
    /// </summary>
    class oldeerTemplate
    {
        /// 临时变量
        public int template;

        /// 无参数的方法
        public void Add()
        {

        }

        /// <summary>
        /// 有参数方法
        /// </summary>
        /// <param name="a">加数</param>
        /// <param name="b">被加数</param>
        public int Add(int a ,int b)
        {
            //返回
            return a + b;
        }


    }
}
