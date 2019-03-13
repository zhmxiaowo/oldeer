/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2019/03/13 11:00:30
============================================================================*/
/// <summary>
/// name:回调范型委托
/// version:1.0
/// describe:
/// author:gaozhijie
/// time:2018.09.02
/// </summary>
public delegate void Callback();
public delegate void Callback<T>(T arg1);
public delegate void Callback<T, U>(T arg1, U arg2);
public delegate void Callback<T, U, V>(T arg1, U arg2, V arg3);
public delegate void Callback<T, U, V, X>(T arg1, U arg2, V arg3, X arg4);
public delegate T ArgCallback<T>();