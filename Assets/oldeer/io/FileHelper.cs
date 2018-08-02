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
/// �ļ���д������
/// ͳһ�ַ���ʹ��UTF-8�����ʽ
/// </summary>
public static class FileHelper

{
    //���ļ�·��
    public static string local_data_path = Application.persistentDataPath + "/creator/";

    //��ʱ�ļ�·��
    public static string local_temp_path = Application.temporaryCachePath + "/";

    //unity���̵߳���һ�� ·����Ϣ,��ֹ����
    public static void init()
    {
        local_data_path = Application.persistentDataPath + "/creator/";
        local_temp_path = Application.temporaryCachePath + "/";
    }

    /// <summary>
    /// �����ַ���string ���ļ�
    /// </summary>
    /// <param name="path">·�� �� "C:/temp/Savepath/"</param>
    /// <param name="name">�ļ��� �� "filename.txt"</param>
    /// <param name="data">����Դ(string)</param>
    public static void SaveToFile(string path, string name, string data)
    {
        SaveToFile(path + name, data);
    }

    /// <summary>
    /// �����ַ���string ���ļ�
    /// </summary>
    /// <param name="path">·�� �� "C:/temp/Savepath/filename.txt"</param>
    /// <param name="data">����Դ(string)</param>
    public static void SaveToFile(string path, string data)
    {
        try
        {
            //�ļ�����Ϣ
            //����ļ��в�����,�򴴽�·��
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }

            //���ļ���Ϣ
            FileInfo t = new FileInfo(path);
            if (t.Exists)
            {
                //������ļ�������ɾ��
                t.Delete();
            }
            //�����ļ�
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
    /// ��ȡ�ļ�
    /// </summary>
    /// <param name="path">�ļ�����·��</param>
    /// <returns>string</returns>
    public static string ReadFile(string path)
    {
        FileInfo fi = new FileInfo(path);
        if (!fi.Exists)
        {
            UnityEngine.Debug.Log("��ȡ���ļ�������" + path);
            return string.Empty;
        }
        StreamReader sr = new StreamReader(path);
        string data = sr.ReadToEnd();
        sr.Close();
        sr.Dispose();
        return data;
    }

    /// <summary>
    /// ��ȡ�ļ���byte�ֽ�
    /// </summary>
    /// <param name="path">·��</param>
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
    /// �ж��ļ��Ƿ����
    /// </summary>
    /// <param name="path">�ļ�����·��</param>
    /// <returns></returns>
    public static bool FileExist(string path)
    {
        FileInfo fi = new FileInfo(path);
        return fi.Exists;
    }

    #region alongside texture
    //�ӱ��ؼ���ͼƬ
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
            Debug.Log("��ȡͼƬʧ�� " + texpath);
        }
        return tex;
    }
    //��ȡsprite
    public static Sprite GetSprite(Texture2D tex)
    {
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        return sp;
    }
    //��ȡspirte
    public static Sprite GetSprite(string path, int w = 1, int h = 1)
    {
        Texture2D tex = LoadTexutreFromDisk(path, w, h);
        Sprite sp = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), Vector2.one * 0.5f);
        return sp;
    }
    #endregion
}
