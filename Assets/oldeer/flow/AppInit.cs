/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018/07/30 20:50:10
============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace oldeer
{
    //ֻ����һ��,������������,������������ д��һЩִֻ��һ�εĲ���
    public class AppInit : MonoBehaviour
    {

        //���� 0 ���� 1 Ӣ��
        public int language = 0;

        private void Awake()
        {
            DontDestroyOnLoad(this.gameObject);
        }
        // Use this for initialization
        void Start()
        {

        }

        //������÷���
        private static List<System.Type> addtionInitOnceList = new List<System.Type>();

        /// <summary>
        /// ֻ��ʼ��һ�εķ������������︽�ӵ���,������Ч��ֹ��ε���,�������е�����,��ε���,����Ҳִֻ��һ��
        /// </summary>
        /// <param name="sender">���õĵ���Ҫ��,ͨ��Ϊthis</param>
        /// <param name="action">���õķ���</param>
        public static void AddInit(object sender, System.Action action)
        {
            System.Type type = sender.GetType();
            if (!addtionInitOnceList.Contains(type))
            {
                addtionInitOnceList.Add(type);
                //����һ��
                action();
            }
        }

    }
}

