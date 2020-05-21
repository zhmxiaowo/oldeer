using UnityEngine;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System;

namespace oldeer
{
    /// <summary>
    /// 日志工具，用于把日志记录保存在目录，移动设备在PersistentDataPath
    /// </summary>
    public class LogTool : Singleton<LogTool>
    {
        /// <summary>
        /// 日志等级，为不同输出配置用
        /// </summary>
        public enum LogLevel
        {
            LOG = 0,
            WARNING = 1,
            ASSERT = 2,
            ERROR = 3,
            MAX = 4,
        }

        /// <summary>
        /// 日志数据类
        /// </summary>
        public class LogData
        {
            public string Log { get; set; }
            public string Track { get; set; }
            public LogLevel Level { get; set; }
        }

        /// <summary>
        /// OnGUI回调
        /// </summary>
        public delegate void OnGUICallback();

        /// <summary>
        /// UI输出日志等级，只要大于等于这个级别的日志，都会输出到屏幕
        /// </summary>
        public LogLevel uiOutputLogLevel = LogLevel.LOG;
        /// <summary>
        /// 文本输出日志等级，只要大于等于这个级别的日志，都会输出到文本
        /// </summary>
        public LogLevel fileOutputLogLevel = LogLevel.MAX;
        /// <summary>
        /// unity日志和日志输出等级的映射
        /// </summary>
        private Dictionary<LogType, LogLevel> logTypeLevelDict = null;
        /// <summary>
        /// OnGUI回调
        /// </summary>
        public OnGUICallback onGUICallback = null;
        /// <summary>
        /// 日志输出列表
        /// </summary>
        private List<ILogOutput> logOutputList = null;
        private int mainThreadID = -1;

        private const int MaxKeepLogFiles = 15;

        /// <summary>
        /// Unity的Debug.Assert()在发布版本有问题
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="info">输出信息</param>
        public static void Assert(bool condition, string info)
        {
            if (condition)
                return;
            Debug.LogError(info);
        }

        private LogTool()
        {
            Application.logMessageReceived += LogCallback;
            Application.logMessageReceivedThreaded += LogMultiThreadCallback;

            this.logTypeLevelDict = new Dictionary<LogType, LogLevel>
            {
                { LogType.Log, LogLevel.LOG },
                { LogType.Warning, LogLevel.WARNING },
                { LogType.Assert, LogLevel.ASSERT },
                { LogType.Error, LogLevel.ERROR },
                { LogType.Exception, LogLevel.ERROR },
            };

            this.uiOutputLogLevel = LogLevel.LOG;
            this.fileOutputLogLevel = LogLevel.ERROR;
            this.mainThreadID = Thread.CurrentThread.ManagedThreadId;
            this.logOutputList = new List<ILogOutput>
            {
                new FileLogOutput(),
            };
            AppInit.Instance.appUpdate += OnUpdate;
            AppInit.Instance.appOnGUI += OnGUI;
            AppInit.Instance.appOnDestroy += OnDestroy;
            CheckLog();
        }

        void OnGUI()
        {
            if (this.onGUICallback != null)
                this.onGUICallback();
        }

        void OnDestroy()
        {
            Application.logMessageReceived -= LogCallback;
            Application.logMessageReceivedThreaded -= LogMultiThreadCallback;
        }

        void OnUpdate() {
//#if UNITY_EDITOR
//            if (Input.GetKeyDown("z")) {
//                LogOutputListStart();
//            }
//            if (Input.GetKeyDown("x")) {
//                LogOutputListStop();
//            }
//#endif
        }

        public void LogOutputListStart(string path = "") {
            for (int i = 0; i < logOutputList.Count; i++) {
                ((FileLogOutput)logOutputList[i]).Open(path);
            }
        }

        public void LogOutputListStop()
        {
            for (int i = 0; i < logOutputList.Count; i++)
            {
                ((FileLogOutput)logOutputList[i]).Close();
            }
        }

        /// <summary>
        /// 日志调用回调，主线程和其他线程都会回调这个函数，在其中根据配置输出日志
        /// </summary>
        /// <param name="log">日志</param>
        /// <param name="track">堆栈追踪</param>
        /// <param name="type">日志类型</param>
        void LogCallback(string log, string track, LogType type)
        {
            if (this.mainThreadID == Thread.CurrentThread.ManagedThreadId)
                Output(log, track, type);
        }

        void LogMultiThreadCallback(string log, string track, LogType type)
        {
            if (this.mainThreadID != Thread.CurrentThread.ManagedThreadId)
                Output(log, track, type);
        }

        void Output(string log, string track, LogType type)
        {
            LogLevel level = this.logTypeLevelDict[type];
            LogData logData = new LogData
            {
                Log = log,
                Track = track,
                Level = level,
            };
            for (int i = 0; i < this.logOutputList.Count; ++i)
                this.logOutputList[i].Log(logData);
        }

        //delete the too old log
        private void CheckLog()
        {
            var logFiles = System.IO.Directory.GetFiles(Application.persistentDataPath + FileLogOutput.LogPath);
            if (logFiles.Length > MaxKeepLogFiles)
            {
                System.Array.Sort(logFiles);
                for (int i = 0; i < logFiles.Length - MaxKeepLogFiles; i++)
                {
                    System.IO.File.Delete(logFiles[i]);
                }
            }
        }

        public Task<string> CompressLogFiles(string filePath,bool deleteAfterCompressed)
        {
            var tcs = new TaskCompletionSource<string>();
            var pathRoot = Application.persistentDataPath;
            try
            {
                Task.Run(() =>
                {
                    LogOutputListStop();
                    float progress = 0;
                    ZipUtility.ZipFileDirectory(pathRoot + $"{FileLogOutput.LogPath}", filePath, false, ref progress);
                    if (deleteAfterCompressed)
                    {
                        System.IO.Directory.Delete(pathRoot + $"{FileLogOutput.LogPath}", true);
                    }
                    LogOutputListStart();
                    tcs.TrySetResult(filePath);
                });
            }
            catch (Exception e)
            {
                Debug.LogError("compress log files error:" + e.Message);
            }
            return tcs.Task;
        }
    }
}