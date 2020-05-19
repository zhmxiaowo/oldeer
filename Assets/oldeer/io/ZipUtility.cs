using System.Collections;
using System.IO;
using System.Threading.Tasks;
using ICSharpCode.SharpZipLib.Zip;
using ICSharpCode.SharpZipLib.Checksum;
using UnityEngine;
public static class ZipUtility
{
    public class ZipClass
    {
        //public ArrayList alList = new ArrayList();
        public string rootDir;
        public long totalLen = 0;
        public long curLen = 0;
        //public float progress = 0;
        private Crc32 crc = new Crc32();
        byte[] buffer = new byte[4096];

        public long getAllFileLength(string strBaseDir)
        {
            string[] filenames = Directory.GetFileSystemEntries(strBaseDir);
            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    getAllFileLength(file);
                }
                else
                {
                    //Debug.Log("su su su 文件路径+" +file  +"------totalLen" + totalLen);
                    FileInfo info = new FileInfo(file);
                    totalLen += info.Length;
                }
            }
            return totalLen;
        }

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要压缩的文件</param>
        /// <param name="zipedFile">压缩后的文件</param>
        /// <param name="compressionLevel">压缩等级</param>
        /// <param name="blockSize">每次写入大小</param>
        public void ZipFile(string fileToZip, string zipedFile, int compressionLevel, int blockSize)
        {
            //如果文件没有找到，则报错
            if (!System.IO.File.Exists(fileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }

            using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
            {
                using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                {
                    using (System.IO.FileStream StreamToZip = new System.IO.FileStream(fileToZip, System.IO.FileMode.Open, System.IO.FileAccess.Read))
                    {
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);

                        ZipEntry ZipEntry = new ZipEntry(fileName);

                        ZipStream.PutNextEntry(ZipEntry);

                        ZipStream.SetLevel(compressionLevel);

                        byte[] buffer = new byte[blockSize];

                        int sizeRead = 0;

                        try
                        {
                            do
                            {
                                sizeRead = StreamToZip.Read(buffer, 0, buffer.Length);
                                ZipStream.Write(buffer, 0, sizeRead);
                            }
                            while (sizeRead > 0);
                        }
                        catch (System.Exception ex)
                        {
                            throw ex;
                        }

                        StreamToZip.Close();
                    }

                    ZipStream.Finish();
                    ZipStream.Close();
                }

                ZipFile.Close();
            }
        }

        /// <summary>
        /// 压缩单个文件
        /// </summary>
        /// <param name="fileToZip">要进行压缩的文件名</param>
        /// <param name="zipedFile">压缩后生成的压缩文件名</param>
        public void ZipFile(string fileToZip, string zipedFile)
        {
            //如果文件没有找到，则报错
            if (!File.Exists(fileToZip))
            {
                throw new System.IO.FileNotFoundException("指定要压缩的文件: " + fileToZip + " 不存在!");
            }

            using (FileStream fs = File.OpenRead(fileToZip))
            {
                byte[] buffer = new byte[fs.Length];
                fs.Read(buffer, 0, buffer.Length);
                fs.Close();

                using (FileStream ZipFile = File.Create(zipedFile))
                {
                    using (ZipOutputStream ZipStream = new ZipOutputStream(ZipFile))
                    {
                        string fileName = fileToZip.Substring(fileToZip.LastIndexOf("\\") + 1);
                        ZipEntry ZipEntry = new ZipEntry(fileName);
                        ZipStream.PutNextEntry(ZipEntry);
                        ZipStream.SetLevel(5);
                        ZipStream.Write(buffer, 0, buffer.Length);
                        ZipStream.Finish();
                        ZipStream.Close();
                    }
                }
            }
        }

        /// <summary>
        /// 压缩多层目录
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="zipedFile">The ziped file.</param>
        /// example: D:/creator/7ad236356ebd036f6c4e31178b5384b575cbeff8_201903061614291429/
        public bool ZipFileDirectory(string strDirectory, string zipedFile, bool includeDir, ref float progress)
        {
            totalLen = getAllFileLength(strDirectory);
            Debug.Log("总长度------>" + totalLen);
            string fdir = strDirectory.Replace("\\", "/");
            fdir = fdir.Substring(0, fdir.Length - 1);
            Debug.Log("fdir------>" + fdir);
            if (includeDir)
            {
                rootDir = fdir.Substring(fdir.LastIndexOf("/") + 1);
                rootDir += "/";
            }
            else
            {
                rootDir = "";
            }

            using (System.IO.FileStream ZipFile = System.IO.File.Create(zipedFile))
            {
                using (ZipOutputStream s = new ZipOutputStream(ZipFile))
                {
                    try
                    {
                        s.UseZip64 = UseZip64.Off;
                        s.SetLevel(8);
                        ZipSetp(strDirectory, s, rootDir, includeDir, ref progress, Path.GetFileName(zipedFile));
                        progress = 1;
                    }
                    catch (System.Exception e)
                    {
                        Debug.Log("压缩出错了" + e.ToString());
                        progress = 0;
                    }
                    finally
                    {
                        s.Finish();
                        s.Close();
                        s.Dispose();
                    }
                }
            }
            return progress >= 1 ? true : false;
        }

        /// <summary>
        /// 递归遍历目录
        /// </summary>
        /// <param name="strDirectory">The directory.</param>
        /// <param name="s">The ZipOutputStream Object.</param>
        /// <param name="parentPath">The parent path.</param>
        private void ZipSetp(string strDirectory, ZipOutputStream s, string parentPath, bool includeDir, ref float progress, string ignore = "")
        {
            if (strDirectory[strDirectory.Length - 1] != Path.DirectorySeparatorChar && strDirectory[strDirectory.Length - 1] != '/')
            {
                strDirectory += Path.DirectorySeparatorChar;
            }
            strDirectory.Replace("\\", "/");

            string[] filenames = Directory.GetFileSystemEntries(strDirectory);

            for (int i = 0; i < filenames.Length; i++)
            {
                filenames[i] = filenames[i].Replace("\\", "/");
            }

            foreach (string file in filenames)// 遍历所有的文件和目录
            {
                if (Directory.Exists(file))// 先当作目录处理如果存在这个目录就递归Copy该目录下面的文件
                {
                    string pPath = parentPath;
                    pPath += file.Substring(file.LastIndexOf("/") + 1);
                    pPath += "/";
                    ZipSetp(file, s, pPath, includeDir, ref progress, ignore);
                }
                else // 否则直接压缩文件
                {
                    if (!string.IsNullOrEmpty(ignore) && file.Contains(ignore))
                    {
                        continue;
                    }
                    //打开压缩文件
                    int sourceBytes = 0;
                    using (FileStream fs = File.OpenRead(file))
                    {
                        byte[] buffer = new byte[fs.Length];
                        sourceBytes = fs.Read(buffer, 0, buffer.Length);

                        string name = Path.GetFileName(file);
                        string fileName = parentPath + name;

                        Debug.Log("total:" + fileName);
                        ZipEntry entry = new ZipEntry(fileName);
                        entry.DateTime = System.DateTime.Now;
                        entry.Size = fs.Length;
                        fs.Close();

                        crc.Reset();
                        crc.Update(buffer);
                        entry.Crc = crc.Value;

                        s.PutNextEntry(entry);
                        s.Write(buffer, 0, buffer.Length);
                        curLen += sourceBytes;
                        //Debug.Log("当前压缩的长度----" +curLen);
                        progress = (float)(curLen / (double)totalLen);
                        Debug.Log("压缩的进度是------" + progress);
                    }
                }
            }
        }

        /// <summary>
        /// 解压缩一个 zip 文件。
        /// </summary>
        /// <param name="zipedFile">The ziped file.</param>
        /// <param name="strDirectory">The STR directory.</param>
        /// <param name="password">zip 文件的密码。</param>
        /// <param name="overWrite">是否覆盖已存在的文件。</param>
        public void UnZip(string zipedFile, string strDirectory, string password, bool overWrite)
        {

            if (strDirectory == "")
                strDirectory = Directory.GetCurrentDirectory();
            if (!strDirectory.EndsWith("\\"))
                strDirectory = strDirectory + "\\";

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipedFile)))
            {
                s.Password = password;
                ZipEntry theEntry;

                while ((theEntry = s.GetNextEntry()) != null)
                {
                    string directoryName = "";
                    string pathToZip = "";
                    pathToZip = theEntry.Name;

                    if (pathToZip != "")
                        directoryName = Path.GetDirectoryName(pathToZip) + "\\";

                    string fileName = Path.GetFileName(pathToZip);

                    Directory.CreateDirectory(strDirectory + directoryName);

                    if (fileName != "")
                    {
                        if ((File.Exists(strDirectory + directoryName + fileName) && overWrite) || (!File.Exists(strDirectory + directoryName + fileName)))
                        {
                            using (FileStream streamWriter = File.Create(strDirectory + directoryName + fileName))
                            {
                                int size = 2048;
                                byte[] data = new byte[2048];
                                while (true)
                                {
                                    size = s.Read(data, 0, data.Length);

                                    if (size > 0)
                                        streamWriter.Write(data, 0, size);
                                    else
                                        break;
                                }
                                streamWriter.Close();
                            }
                        }
                    }
                }

                s.Close();
            }
        }
    }

    public static bool ZipFileDirectory(string strDirectory, string zipedFile, bool includeDir, ref float progress)
    {
        ZipClass zc = new ZipClass();
        return zc.ZipFileDirectory(strDirectory, zipedFile, includeDir, ref progress);
    }

}
