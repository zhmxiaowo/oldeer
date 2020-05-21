/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin wartheking
 * time:    2018/08/13 11:09:22
============================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace oldeer
{
    /// <summary>
    /// 日志输出接口
    /// </summary>
    public interface ILogOutput
    {
        /// <summary>
        /// 输出日志数据
        /// </summary>
        /// <param name="logData">日志数据</param>
        void Log(LogTool.LogData logData);
        /// <summary>
        /// 关闭
        /// </summary>
        void Close();
    }
}