using System.Collections.Generic;
using UnityEngine;
using System.Net;
using System.Threading.Tasks;
using System.Text;
using System.IO;
using System;
using System.Linq;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace oldeer
{
    /// <summary>
    /// Net help
    /// </summary>
    public static class NetUtility
    {
        /// <summary>
        ///  Get the http length
        /// </summary>
        /// <param name="url"></param>
        /// <param name="timeout">millisecond</param>
        /// <returns>http url:html size ,file url:file bytes length </returns>
        public static long HttpGetSize(string url, int timeout = 3000)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.Method = "HEAD";
            request.Timeout = timeout;
            HttpWebResponse res;
            try
            {
                res = (HttpWebResponse)request.GetResponse();
                if (res.StatusCode == HttpStatusCode.OK)
                {
                    return res.ContentLength;
                }
            }
            catch (Exception e)
            {
                Debug.Log("Get http length failure:" + e.ToString());
                return 0;
            }
            return 0;
        }

        /// <summary>
        /// Post a request to server 
        /// *1.use "application/x-www-form-urlencoded" to upload(easy,sort),so server can read just like a=1&b=2
        /// *2 "multipart/form-data" to upload(one by one part,long),this one can upload a file.
        public static string Post(string url, IDictionary<string, string> parameters = null, int timeout = 3000, int readWriteTimeout = 3000)
        {
            HttpWebRequest request = null;
            if (url.StartsWith("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "POST";
            request.Proxy = null;
            request.UserAgent = null;
            request.Timeout = timeout;
            request.ReadWriteTimeout = readWriteTimeout;
            request.ContentType = "application/x-www-form-urlencoded";

            string result = string.Empty;
            //如果需要POST数据   
            if (parameters != null && parameters.Count >= 0)
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
                using (Stream stream = request.GetRequestStream())
                {
                    try
                    {
                        stream.Write(data, 0, data.Length);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Http Post failure:" + e.Message);
                        return "Network Error";
                    }
                }
            }
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
                using (StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = reader.ReadToEnd();
                }
            }
            catch (Exception e)
            {
                Debug.Log(e.Message);
                return "Network Error";
            }
            if (request != null)
            {
                request.Abort();
                request = null;
            }
            if (response != null)
            {
                response.Close();
                response.Dispose();
                response = null;
            }
            return result;
        }

        public static string Get(string url, IDictionary<string, string> parameters = null, int timeout = 3000, int readWriteTimeout = 3000)
        {
            string result = "";

            if (parameters != null && parameters.Count >= 0)
            {
                StringBuilder buffer = new StringBuilder();
                buffer.Append(url);
                int i = 0;
                foreach (string key in parameters.Keys)
                {
                    if (i > 0)
                    {
                        buffer.AppendFormat("&{0}={1}", key, parameters[key]);
                    }
                    else
                    {
                        buffer.AppendFormat("?{0}={1}", key, parameters[key]);
                    }
                    i++;
                }
                url = buffer.ToString();
            }
            HttpWebRequest request = null;
            if (url.StartsWith("https"))
            {
                ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                request = WebRequest.Create(url) as HttpWebRequest;
                request.ProtocolVersion = HttpVersion.Version10;
            }
            else
            {
                request = WebRequest.Create(url) as HttpWebRequest;
            }
            request.Method = "GET";
            request.Proxy = null;
            request.UserAgent = null;
            request.Timeout = timeout;
            request.ReadWriteTimeout = readWriteTimeout;
            request.ContentType = "application/x-www-form-urlencoded;charset=UTF-8";
            WebResponse response = null;
            try
            {
                response = request.GetResponse();
                using (StreamReader sr = new StreamReader(response.GetResponseStream(), Encoding.UTF8))
                {
                    result = sr.ReadToEnd();
                }
                return result;
            }
            catch (Exception e)
            {
                Debug.Log("Http Get failure:" + e.Message);
                return "Network Error";
            }
            finally
            {
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
            }

            return "";

        }
        //async post
        public static Task<string> PostAsync(string url, IDictionary<string, string> parameters = null, int timeout = 3000, int readWriteTimeout = 3000)
        {
            return Task.Run(() =>
            {
                return Post(url, parameters, timeout, readWriteTimeout);
            });
        }

        //async get
        public static Task<string> GetAsync(string url, IDictionary<string, string> parameters = null, int timeout = 3000, int readWriteTimeout = 3000)
        {
            return Task.Run(() =>
            {
                return Get(url, parameters, timeout, readWriteTimeout);
            });
        }


        //download and save file
        //not support big file more than MaxValue = 2147483647;
        public static byte[] HttpDownload(string url, string path, int timeout = 3000)
        {
            byte[] data = null;
            try
            {
                long fileSize = HttpGetSize(url, timeout);
                HttpWebRequest request = null;
                if (url.StartsWith("https"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = timeout;
                request.ReadWriteTimeout = timeout;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                data = new byte[fileSize];
                using (Stream responseStream = response.GetResponseStream())
                {
                    responseStream.Read(data, 0, (int)fileSize);
                }
                return data;
            }
            catch (Exception ex)
            {
                Debug.Log("HttpDownload Error:" + ex.ToString());
                return new byte[0];
            }
        }

        //download and save file
        //support download big file
        public static bool HttpDownload(string url, string path, int timeout = 3000, int readWriteTimeout = 3000)
        {
            try
            {
                HttpWebRequest request = null;
                if (url.StartsWith("https"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = timeout;
                request.ReadWriteTimeout = readWriteTimeout;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        byte[] bArr = new byte[8192];
                        int size = 0;
                        long totalBytes = size;
                        while ((size = responseStream.Read(bArr, 0, (int)bArr.Length)) > 0)
                        {
                            fs.Write(bArr, 0, size);
                            totalBytes += size;
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("HttpDownload Error:" + ex.ToString());
                return false;
            }
        }

        //下载文件
        public static bool HttpDownload(string url, string path, System.Action<float> onProgressHandler = null, int timeout = 3000, int readWriteTimeout = 3000)
        {
            try
            {
                long fileSize = HttpGetSize(url, timeout);
                HttpWebRequest request = null;
                if (url.StartsWith("https"))
                {
                    ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(CheckValidationResult);
                    request = WebRequest.Create(url) as HttpWebRequest;
                    request.ProtocolVersion = HttpVersion.Version10;
                }
                else
                {
                    request = WebRequest.Create(url) as HttpWebRequest;
                }
                request.Proxy = null;
                request.KeepAlive = false;
                request.Timeout = timeout;
                request.ReadWriteTimeout = readWriteTimeout;
                HttpWebResponse response = request.GetResponse() as HttpWebResponse;
                using (Stream responseStream = response.GetResponseStream())
                {
                    using (FileStream fs = new FileStream(path, FileMode.Create))
                    {
                        byte[] bArr = new byte[8192];
                        int size = 0;
                        long totalBytes = size;
                        while ((size = responseStream.Read(bArr, 0, (int)bArr.Length)) > 0)
                        {
                            fs.Write(bArr, 0, size);
                            totalBytes += size;
                            onProgressHandler((totalBytes / fileSize));
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Debug.Log("HttpDownload Error:" + ex.ToString());
                return false;
            }
        }

        /// <summary>
        /// 生成时间戳
        /// </summary>
        /// <returns></returns>
        public static string GetTimeStamp()
        {
            TimeSpan ts = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }


        //for https
        private static bool CheckValidationResult(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }
    }



}

