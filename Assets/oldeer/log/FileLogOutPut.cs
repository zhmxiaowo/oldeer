using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.IO;
using UnityEngine;

namespace oldeer
{
    /// <summary>
    /// 文本日志输出
    /// </summary>
    public class FileLogOutput : ILogOutput
    {

#if UNITY_EDITOR
        string mDevicePersistentPath = Application.persistentDataPath;
#elif UNITY_STANDALONE_WIN
        string mDevicePersistentPath = Application.persistentDataPath;
#elif UNITY_STANDALONE_OSX
        string mDevicePersistentPath = Application.persistentDataPath;
#else
        string mDevicePersistentPath = Application.persistentDataPath;
#endif


        public const string LogPath = "/Log/";

        private Queue<LogTool.LogData> mWritingLogQueue = null;
        private Queue<LogTool.LogData> mWaitingLogQueue = null;
        private object mLogLock = null;
        private Thread mFileLogThread = null;
        private bool mIsRunning = false;
        private StreamWriter mLogWriter = null;

        public FileLogOutput()
        {
            AppInit.Instance.appOnApplicationQuit += Close;
            this.mWritingLogQueue = new Queue<LogTool.LogData>();
            this.mWaitingLogQueue = new Queue<LogTool.LogData>();
            this.mLogLock = new object();
            DateTime now = DateTime.Now;
            //string logName = string.Format("Q{0}{1}{2}{3}{4}{5}",
            //    now.Year, now.Month, now.Day, now.Hour, now.Minute, now.Second);
            string logPath = string.Format("{0}{1}{2}.txt", mDevicePersistentPath, LogPath, System.DateTime.Now.ToString("yyyy-MM-dd"));
            //if (File.Exists(logPath))
            //    File.Delete(logPath);
            string logDir = Path.GetDirectoryName(logPath);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir);
            }
            this.mLogWriter = new StreamWriter(logPath,true);
            this.mLogWriter.AutoFlush = true;
            this.mIsRunning = true;
            this.mFileLogThread = new Thread(new ThreadStart(WriteLog));
            this.mFileLogThread.Start();
        }

        void WriteLog()
        {
            while (this.mIsRunning)
            {
                if (this.mWritingLogQueue.Count == 0)
                {
                    lock (this.mLogLock)
                    {
                        while (this.mWaitingLogQueue.Count == 0)
                            Monitor.Wait(this.mLogLock);
                        Queue<LogTool.LogData> tmpQueue = this.mWritingLogQueue;
                        this.mWritingLogQueue = this.mWaitingLogQueue;
                        this.mWaitingLogQueue = tmpQueue;
                    }
                }
                else
                {
                    while (this.mWritingLogQueue.Count > 0)
                    {
                        LogTool.LogData log = this.mWritingLogQueue.Dequeue();
                        if (log.Level == LogTool.LogLevel.ERROR)
                        {
                            this.mLogWriter.WriteLine("---------------------------------------------------------------------------------------------------------------------");
                            this.mLogWriter.WriteLine(System.DateTime.Now.ToString() + "\t" + log.Log + "\n");
                            this.mLogWriter.WriteLine(log.Track);
                            this.mLogWriter.WriteLine("---------------------------------------------------------------------------------------------------------------------");
                        }
                        else
                        {
                            this.mLogWriter.WriteLine(System.DateTime.Now.ToString() + "\t" + log.Log);
                        }
                    }
                }
            }
        }

        public void Log(LogTool.LogData logData)
        {
            lock (this.mLogLock)
            {
                this.mWaitingLogQueue.Enqueue(logData);
                Monitor.Pulse(this.mLogLock);
            }
        }

        public void Close()
        {
            this.mIsRunning = false;
            this.mLogWriter.Close();
        }

        public void Open(string path) {
            if (mIsRunning)
                return;

            string logPath = string.Format("{0}{1}{2}.txt", mDevicePersistentPath, LogPath, System.DateTime.Now.ToString("yyyy-MM-dd"));
            if (!string.IsNullOrEmpty(path))
                logPath = path;
            //if (File.Exists(logPath))
            //    File.Delete(logPath);
            string logDir = Path.GetDirectoryName(logPath);
            if (!Directory.Exists(logDir))
            {
                Directory.CreateDirectory(logDir); 
            }
            this.mIsRunning = true;
            this.mLogWriter = new StreamWriter(logPath, true);
            this.mLogWriter.AutoFlush = true;
            this.mFileLogThread = new Thread(new ThreadStart(WriteLog));
            this.mFileLogThread.Start();
        }
    }
}