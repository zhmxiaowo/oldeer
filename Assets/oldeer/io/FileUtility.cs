/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018/08/02 10:50:11
============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
/// <summary>
/// 文件读写帮助类
/// 统一字符串使用UTF-8编码格式
/// </summary>
public static class FileUtility
{
    /// <summary>
    /// 主文件路径
    /// </summary>
    public static string local_data_path = Application.persistentDataPath + "/creator/";

    /// <summary>
    /// 临时文件路径
    /// </summary>
    public static string local_temp_path = Application.temporaryCachePath + "/";

    /// <summary>
    /// unity主线程调用一次 路径信息,防止出错
    /// </summary>
    public static void init()
    {
        local_data_path = Application.persistentDataPath + "/creator/";
        local_temp_path = Application.temporaryCachePath + "/";
    }

    /// <summary>
    /// 保存字符串string 到文件
    /// </summary>
    /// <param name="path">路径 如 "C:/temp/Savepath/"</param>
    /// <param name="name">文件名 如 "filename.txt"</param>
    /// <param name="data">数据源(string)</param>
    public static void SaveToFile(string path, string name, string data)
    {
        SaveToFile(path + name, data);
    }

    /// <summary>
    /// 保存字符串string 到文件
    /// </summary>
    /// <param name="path">路径 如 "C:/temp/Savepath/filename.txt"</param>
    /// <param name="data">数据源(string)</param>
    public static void SaveToFile(string path, string data)
    {
        try
        {
            //文件流信息
            //如果文件夹不存在,则创建路径
            if (!Directory.Exists(path.Substring(0,path.LastIndexOf("/"))))
            {
                Directory.CreateDirectory(path);
            }

            //打开文件信息
            FileInfo t = new FileInfo(path);
            if (t.Exists)
            {
                //如果此文件存在则删除
                t.Delete();
            }
            //保存文件
            FileStream st = new FileStream(path, FileMode.Create);
            byte[] dataByte = System.Text.Encoding.UTF8.GetBytes(data);
            st.Flush();
            st.Write(dataByte, 0, dataByte.Length);
            st.Close();
        }
        catch(System.Exception e)
        {
            Debug.Log("save file error" + e.ToString());
        }

    }

    /// <summary>
    /// 读取文件
    /// </summary>
    /// <param name="path">文件完整路径</param>
    /// <returns>string</returns>
    public static string ReadFile(string path)
    {
        FileInfo fi = new FileInfo(path);
        if (!fi.Exists)
        {
            UnityEngine.Debug.Log("读取的文件不存在" + path);
            return string.Empty;
        }
        StreamReader sr = null;
        string data = "";
        try
        {
            sr = new StreamReader(path);
            data = sr.ReadToEnd();
            sr.Close();
            sr.Dispose();
        }
        catch(System.Exception e)
        {
            Debug.Log("读取文件出错" + e.ToString());
            if(sr != null)
            {
                sr.Close();
                sr.Dispose();
            }
        }
        return data;
    }

    /// <summary>
    /// 读取文件的byte字节
    /// </summary>
    /// <param name="path">路径</param>
    /// <returns></returns>
    public static byte[] ReadFileByte(string path)
    {
        FileStream fs = null;
        byte[] bytes = new byte[0];
        try
        {

            fs = new FileStream(path, FileMode.Open, FileAccess.Read);
            fs.Seek(0, SeekOrigin.Begin);
            bytes = new byte[fs.Length];
            fs.Read(bytes, 0, (int)fs.Length);

        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.Log(e);
        }
        finally
        {
            if (fs != null)
            {
                fs.Close();
                fs.Dispose();
                fs = null;
            }

        }

        return bytes;
    }

    /// <summary>
    /// 判断文件是否存在
    /// </summary>
    /// <param name="path">文件完整路径</param>
    /// <returns></returns>
    public static bool FileExist(string path)
    {
        FileInfo fi = new FileInfo(path);
        return fi.Exists;
    }

    #region alongside texture
    /// <summary>
    /// 从本地加载图片
    /// </summary>
    /// <param name="texpath">图片路径</param>
    /// <param name="w">宽,不知道可以不填</param>
    /// <param name="h">高,不知道可以不填</param>
    /// <returns>图片类型</returns>
    public static Texture2D LoadTexutreFromDisk(string texpath, int w = 1, int h = 1)
    {
        byte[] datas = ReadFileByte(texpath);

        Texture2D tex = new Texture2D(w, h, TextureFormat.ARGB32, false);

        if (datas.Length > 0)
        {
            tex.LoadImage(datas);
        }
        else
        {
            Debug.Log("获取图片失败 " + texpath);
        }
        return tex;
    }
    /// <summary>
    /// 获取sprite
    /// </summary>
    /// <param name="tex"></param>
    /// <returns></returns>
    public static Sprite GetSprite(Texture2D tex)
    {
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        return sp;
    }
    /// <summary>
    /// 获取spirte
    /// </summary>
    /// <param name="path"></param>
    /// <param name="w"></param>
    /// <param name="h"></param>
    /// <returns></returns>
    public static Sprite GetSprite(string path, int w = 1, int h = 1)
    {
        Texture2D tex = LoadTexutreFromDisk(path, w, h);
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        return sp;
    }
    #endregion
}
