/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2019/03/13 14:37:17
============================================================================*/
/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018/08/07 20:25:25
============================================================================*/
using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System;
using System.Linq;

namespace oldeer
{
    /// <summary>
    /// 由于异步不能ref和out，但是需要拿的是缓存的图片还是刚下载的图片，因此封装了这个结构体
    /// </summary>
    public class TexturePackage
    {

        public Texture mTex;
        public bool mIsExist;
    }

    public enum NetworkEnvirment
    {
        WiFi,
        Local,
        Offline
    };

    /// <summary>
    /// 网络请求帮助类
    /// </summary>
    public static class NetUtility
    {


        /// <summary>
        /// 获取下载文件的长度
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static async Task<long> HttpGetSize(string url)
        {
            //Debug.Log(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.Timeout = 4000;
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)await request.GetResponseAsync();
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    return res.ContentLength;
                }
            }
            catch (Exception e)
            {
                Debug.Log("获取网页长度失败" + e.ToString());
                return 0;
            }

            return 0;
        }

        /// <summary>
        /// Http的Post方法
        /// </summary>
        /// <param name="url">请求地址</param>
        /// <param name="timeout">请求超时设置</param>
        /// <param name="parameters">请求数据合集</param>
        /// <returns></returns>
        public static async Task<string> CreatePostHttpResponse(string url, int timeout, IDictionary<string, string> parameters)
        {
            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            string result = string.Empty;
            request.ProtocolVersion = HttpVersion.Version10;
            request.Method = "POST";
            request.Timeout = timeout;
            request.ContentType = "application/x-www-form-urlencoded";
            request.UserAgent = null;
            //如果需要POST数据   
            if (!(parameters == null || parameters.Count == 0))
            {
                StringBuilder buffer = new StringBuilder();
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                byte[] data = Encoding.UTF8.GetBytes(buffer.ToString());
                try
                {
                    Stream stream = request.GetRequestStream();
                    stream.Write(data, 0, data.Length);
                }
                catch (Exception e)
                {
                    return "Network Error";
                }
            }
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                response = await request.GetResponseAsync();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();
            }
            catch (Exception e)
            {
                return "Network Error";
            }
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            if (response != null)
            {
                response.Close();
                response.Dispose();
                response = null;
            }
            if (request != null)
            {
                request.Abort();
                request = null;
            }
            return result;
        }

        //下载文件
        public static bool HttpDownload(string url, string path)
        {
            //FdageFileDownloader.DownloadEvent downloadEvent = new FdageFileDownloader.DownloadEvent();
            //downloadEvent.status = FdageFileDownloader.DownloadStatus.FAILED;
            //downloadEvent.savePath = path;
            //downloadEvent.fileURL = url;

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);    //存在则删除
            }
            try
            {
                FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                // 设置参数
                HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = 10000;
                //发送请求并获取相应回应数据
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                //直到request.GetResponse()程序才开始向目标网页发送Post请求
                Stream responseStream = response.GetResponseStream();

                byte[] bArr = new byte[4096];
                int size = responseStream.Read(bArr, 0, (int)bArr.Length);
                long totalBytes = size;
                while (size > 0)
                {
                    //stream.Write(bArr, 0, size);
                    fs.Write(bArr, 0, size);
                    size = responseStream.Read(bArr, 0, (int)bArr.Length);
                    totalBytes += size;
                }
                //stream.Close();
                fs.Close();
                responseStream.Close();

                //downloadEvent.status = FdageFileDownloader.DownloadStatus.COMPLETED;
                //downloadEvent.downloadedBytes = totalBytes;
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("下载文件失败" + ex.ToString());
                return false;
            }
        }


        public static async Task<string> RequestAndRespone(string url, string method = "GET", int timeout = 3000)
        {
            return await Task.Run(async () =>
            {
                string result = "";
                GC.Collect();
                HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);
                myReq.Proxy = null;
                myReq.Method = method;
                myReq.UserAgent = null;
                myReq.KeepAlive = false;
                myReq.Timeout = timeout;
                myReq.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
                WebResponse response = null;
                StreamReader reader = null;
                try
                {
                    response = myReq.GetResponse();
                    reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                    result = reader.ReadToEnd();
                    await AsyncTools.ToMainThread();
                    return result;
                }
                catch (Exception e)
                {
                    await AsyncTools.ToMainThread();
#if UNITY_EDITOR
                    Debug.LogError(e.Message);
#endif
                }
                if (reader != null)
                {
                    reader.Close();
                    reader = null;
                }
                if (response != null)
                {
                    response.Close();
                    response.Dispose();
                    response = null;
                }
                if (myReq != null)
                {
                    myReq.Abort();
                    myReq = null;
                }
                return "";
            });

        }

        public static string AsyncRequestAndRespone(string url, Dictionary<string, string> param = null, int timeout = 5000)
        {
            //return await Task.Run(async () =>
            //{
            string result = "";
            GC.Collect();
            HttpWebRequest myReq = (HttpWebRequest)WebRequest.Create(url);

            //myReq.ProtocolVersion = HttpVersion.Version11;
            //myReq.KeepAlive = false;
            myReq.Proxy = null;
            myReq.UserAgent = null;
            myReq.Timeout = timeout;
            myReq.Method = "POST";
            //myReq.Accept = "application/json";
            myReq.ContentType = "application/x-www-form-urlencoded:charset=UTF-8";
            myReq.AllowAutoRedirect = true;
            if (param != null && param.Count > 0)
            {
                try
                {
                    //循环加载参数
                    StringBuilder sb = new StringBuilder();
                    int i = 0;
                    foreach (var key in param.Keys)
                    {
                        if (i > 0)
                        {
                            sb.AppendFormat("&{0}={1}", key, param[key]);
                        }
                        else
                        {
                            sb.AppendFormat("{0}={1}", key, param[key]);
                        }
                        i++;
                    }

                    byte[] jsonData = Encoding.UTF8.GetBytes(sb.ToString());
                    myReq.ContentLength = jsonData.Length;
                    using (Stream reqStream = myReq.GetRequestStream())
                    {
                        reqStream.Write(jsonData, 0, jsonData.Length);
                    }
                }
                catch (System.Exception e)
                {
                    Debug.Log("异步http请求 request发送数据出错" + e.ToString());
                }
            }
            WebResponse response = null;
            StreamReader reader = null;
            try
            {
                response = myReq.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
                result = reader.ReadToEnd();
                return result;
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                Debug.LogError("异步http请求 request发送数据出错" + url + e.ToString());
#endif
            }
            if (reader != null)
            {
                reader.Close();
                reader = null;
            }
            if (response != null)
            {
                response.Close();
                response.Dispose();
                response = null;
            }
            if (myReq != null)
            {
                myReq.Abort();
                myReq = null;
            }
            return "";
            //});

        }

        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(2000, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
    }



}

