/*===========================================================================
 * Copyright (C) 2018-2021, 4DAGE Technology Co., Ltd. and/or its affiliates.
 * All rights reserved.
 * 
 * author:  osmin
 * time:    2018/07/31 17:15:18
============================================================================*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TinyTeam.UI;
namespace test
{
    public class UIPanel : TTUIPage
    {
        
        public UIPanel() : base(UIType.PopUp, UIMode.DoNothing, UICollider.Normal)
        {
            uiPath = "UIPanel";
        }
        public override void Awake(GameObject go)
        {
            base.Awake(go);
        }
    }
}

