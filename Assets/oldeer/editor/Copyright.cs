//using UnityEngine;
//using System.Collections;
//using System.IO;

//namespace oldeer
//{
//    public class Copyright : UnityEditor.AssetModificationProcessor
//    {
//        //作者
//        private static string author = "osmin";

//        //copyright
//        private static string str =
//@"/*===========================================================================
// * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
// * All rights reserved.
// * 
// * author:  #author#
// * time:    #time#
//============================================================================*/
//";
//        // 创建资源调用
//        public static void OnWillCreateAsset(string path)
//        {
//            // 只修改C#脚本
//            path = path.Replace(".meta", "");
//            if (path.EndsWith(".cs"))
//            {
//                string allText = str;
//                allText += File.ReadAllText(path);
//                allText = allText.Replace("#author#", author);
//                allText = allText.Replace("#time#", System.DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
//                File.WriteAllText(path, allText,System.Text.Encoding.UTF8);
//            }
//        }
//    }
//}
