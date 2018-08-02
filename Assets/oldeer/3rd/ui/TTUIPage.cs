namespace TinyTeam.UI
{
    using System;
    using UnityEngine;
    using System.Collections;
    using System.Collections.Generic;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Each Page Mean one UI 'window'
    /// 3 steps:
    /// instance ui > refresh ui by data > show
    /// 
    /// by chiuan
    /// 2015-09
    /// </summary>

    #region define

    public enum UIType
    {
        Normal,
        Fixed,
        PopUp,
        None,      //独立的窗口
    }

    public enum UIMode
    {
        DoNothing,
        HideOther,     // 闭其他界面
        NeedBack,      // 点击返回按钮关闭当前,不关闭其他界面(需要调整好层级关系)
        NoNeedBack,    // 关闭TopBar,关闭其他界面,不加入backSequence队列
    }

    public enum UICollider
    {
        None,      // 显示该界面不包含碰撞背景
        Normal,    // 碰撞透明背景
        WithBg,    // 碰撞非透明背景
    }
    #endregion

    public abstract class TTUIPage
    {
        public string name = string.Empty;

        //this page's id
        public int id = -1;

        //this page's type
        public UIType type = UIType.Normal;

        //how to show this page.
        public UIMode mode = UIMode.DoNothing;

        //the background collider mode
        public UICollider collider = UICollider.None;

        //path to load ui
        public string uiPath = string.Empty;

        //this ui's gameobject
        public GameObject gameObject;
        public Transform transform;

        //all pages with the union type
        private static Dictionary<string, TTUIPage> m_allPages;
        public static Dictionary<string, TTUIPage> allPages
        { get { return m_allPages; } }

        //control 1>2>3>4>5 each page close will back show the previus page.
        private static List<TTUIPage> m_currentPageNodes;
        public static List<TTUIPage> currentPageNodes
        { get { return m_currentPageNodes; } }

        //record this ui load mode.async or sync.
        private bool isAsyncUI = false;

        //this page active flag
        protected bool isActived = false;

        //refresh page 's data.
        private object m_data = null;
        protected object data { get { return m_data; } }

        //delegate load ui function.
        public static Func<string, Object> delegateSyncLoadUI = null;
        //public static Action<string, Action<Object>> delegateAsyncLoadUI = null;

        #region virtual api

        ///When Instance UI 1 Once.
        public virtual void Awake(GameObject go) { }

        ///Show UI Refresh Eachtime.
        public virtual void Refresh() { }

        ///Active this UI
        public virtual void Active()
        {
            this.gameObject.SetActive(true);
            isActived = true;
        }

        /// <summary>
        /// Only Deactive UI wont clear Data.
        /// </summary>
        public virtual void Hide()
        {
            this.gameObject.SetActive(false);
            isActived = false;
            //set this page's data null when hide.
            this.m_data = null;
        }

        #endregion

        #region internal api

        private TTUIPage() { }
        public TTUIPage(UIType type, UIMode mod, UICollider col)
        {
            this.type = type;
            this.mode = mod;
            this.collider = col;
            this.name = this.GetType().ToString();

            //这部是将Resource的所有预制放在字典里
            //when create one page.
            //bind special delegate .
            TTUIBind.Bind();
            //Debug.LogWarning("[UI] create page:" + ToString());
        }

        /// <summary>
        /// Sync Show UI Logic
        /// </summary>
        protected void Show()
        {
            //1:instance UI
            if (this.gameObject == null && string.IsNullOrEmpty(uiPath) == false)
            {
                GameObject go = null;

                if (delegateSyncLoadUI != null)
                {
                    Object o = delegateSyncLoadUI(uiPath);
                    go = o != null ? GameObject.Instantiate(o) as GameObject : null;
                }
                else
                {
                    go = GameObject.Instantiate(Resources.Load(uiPath)) as GameObject;
                }

                //protected.
                if (go == null)
                {
                    Debug.LogError(uiPath+"  [UI] Cant sync load your ui prefab.");
                    return;
                }
                //放置,标准化UI
                AnchorUIGameObject(go);

                //after instance should awake init.
                Awake(go);

                //mark this ui sync ui
                isAsyncUI = false;
            }

            //:animation or init when active.
            Active();

            //:refresh ui component.
            Refresh();

            //:popup this node to top if need back.
            PopNode(this);
        }

        /// <summary>
        /// Async Show UI Logic
        /// 异步显示UI
        /// </summary>
        //protected void Show(Action callback)
        //{
        //    //TTUIRoot.Instance.StartCoroutine(AsyncShow(callback));
        //}

        //IEnumerator AsyncShow(Action callback)
        //{
        //    //1:Instance UI
        //    //FIX:support this is manager multi gameObject,instance by your self.
        //    if (this.gameObject == null && string.IsNullOrEmpty(uiPath) == false)
        //    {
        //        GameObject go = null;
        //        bool _loading = true;
        //        delegateAsyncLoadUI(uiPath, (o) =>
        //        {
        //            go = o != null ? GameObject.Instantiate(o) as GameObject : null;
        //            AnchorUIGameObject(go);
        //            Awake(go);
        //            isAsyncUI = true;
        //            _loading = false;

        //            //:animation active.
        //            Active();

        //            //:refresh ui component.
        //            Refresh();

        //            //:popup this node to top if need back.
        //            PopNode(this);

        //            if (callback != null) callback();
        //        });

        //        float _t0 = Time.realtimeSinceStartup;
        //        while (_loading)
        //        {
        //            if (Time.realtimeSinceStartup - _t0 >= 10.0f)
        //            {
        //                Debug.LogError("[UI] WTF async load your ui prefab timeout!");
        //                yield break;
        //            }
        //            yield return null;
        //        }
        //    }
        //    else
        //    {
        //        //:animation active.
        //        Active();

        //        //:refresh ui component.
        //        Refresh();

        //        //:popup this node to top if need back.
        //        PopNode(this);

        //        if (callback != null) callback();
        //    }
        //}
        //change by osmin,去掉了对不动UI的pop支持
        internal bool CheckIfNeedBack()
        {
//            if (type == UIType.Fixed || type == UIType.PopUp || type == UIType.None) return false;
//            else if (mode == UIMode.NoNeedBack || mode == UIMode.DoNothing) return false;
            return true;
        }

        protected void AnchorUIGameObject(GameObject ui)
        {
            if (TTUIRoot.Instance == null || ui == null) return;

            this.gameObject = ui;
            this.transform = ui.transform;

            //check if this is ugui or (ngui)?
            Vector3 anchorPos = Vector3.zero;
            Vector2 sizeDel = Vector2.zero;
            Vector3 scale = Vector3.one;
            if (ui.GetComponent<RectTransform>() != null)
            {
                anchorPos = ui.GetComponent<RectTransform>().anchoredPosition;
                sizeDel = ui.GetComponent<RectTransform>().sizeDelta;
                scale = ui.GetComponent<RectTransform>().localScale;
            }
            else
            {
                anchorPos = ui.transform.localPosition;
                scale = ui.transform.localScale;
            }

            //Debug.Log("anchorPos:" + anchorPos + "|sizeDel:" + sizeDel);

            if (type == UIType.Fixed)
            {
                ui.transform.SetParent(TTUIRoot.Instance.fixedRoot);
            }
            else if (type == UIType.Normal)
            {
                ui.transform.SetParent(TTUIRoot.Instance.normalRoot);
            }
            else if (type == UIType.PopUp)
            {
                ui.transform.SetParent(TTUIRoot.Instance.popupRoot);
            }

            if (ui.GetComponent<RectTransform>() != null)
            {
                ui.GetComponent<RectTransform>().anchoredPosition = anchorPos;
                ui.GetComponent<RectTransform>().sizeDelta = sizeDel;
                ui.GetComponent<RectTransform>().localScale = scale;
            }
            else
            {
                ui.transform.localPosition = anchorPos;
                ui.transform.localScale = scale;
            }
        }

        public override string ToString()
        {
            return ">Name:" + name + ",ID:" + id + ",Type:" + type.ToString() + ",ShowMode:" + mode.ToString() + ",Collider:" + collider.ToString();
        }

        public bool isActive()
        {
            //fix,if this page is not only one gameObject
            //so,should check isActived too.
            bool ret = (gameObject != null && gameObject.activeSelf);
            return ret || isActived;
        }

        #endregion

        #region static api

        private static bool CheckIfNeedBack(TTUIPage page)
        {
            return page != null && page.CheckIfNeedBack();
        }

        /// <summary>
        /// 仅仅是将隐藏的层级顺序调整为最高
        /// </summary>
        private static void PopNode(TTUIPage page)
        {
            if (m_currentPageNodes == null)
            {
                m_currentPageNodes = new List<TTUIPage>();
            }

            if (page == null)
            {
                Debug.LogError("[UI] page popup is null.");
                return;
            }

            //sub pages should not need back.
            if (CheckIfNeedBack(page) == false)
            {
                return;
            }

            bool _isFound = false;
            for (int i = 0; i < m_currentPageNodes.Count; i++)
            {
                if (m_currentPageNodes[i].Equals(page))
                {
                    m_currentPageNodes.RemoveAt(i);
                    m_currentPageNodes.Add(page);
                    _isFound = true;
                    break;
                }
            }

            //if dont found in old nodes
            //should add in nodelist.
            if (!_isFound)
            {
                m_currentPageNodes.Add(page);
            }

            //after pop should hide the old node if need.
            HideOldNodes();
        }
        /// <summary>
        /// 当模式为HideOther的时候,隐藏处了他之外的全部界面
        /// </summary>
        private static void HideOldNodes()
        {
            if (m_currentPageNodes.Count < 1) return;
            TTUIPage topPage = m_currentPageNodes[m_currentPageNodes.Count - 1];
            if (topPage.mode == UIMode.HideOther)
            {
                //form bottm to top.
                for (int i = m_currentPageNodes.Count - 2; i >= 0; i--)
                {
                    if(m_currentPageNodes[i].isActive())
                        m_currentPageNodes[i].Hide();
                }
            }
        }

        public static void ClearNodes()
        {
            if(m_currentPageNodes != null)
                m_currentPageNodes.Clear();
        }
        public static void ClearAllPage()
        {
            if(m_allPages != null)
                m_allPages.Clear();
        }

        //step 2 search type
        private static T ShowPage<T>(object pageData) where T : TTUIPage, new()
        {
            Type t = typeof(T);
            string pageName = t.ToString();

            if (m_allPages != null && m_allPages.ContainsKey(pageName))
            {
               return ShowPage(pageName, m_allPages[pageName], pageData) as T;
            }
            else
            {
                T instance = new T();
                return ShowPage(pageName, instance, pageData) as T;
            }
        }

        //final step   3    instance
        private static TTUIPage ShowPage(string pageName, TTUIPage pageInstance, object pageData)
        {
            if (string.IsNullOrEmpty(pageName) || pageInstance == null)
            {
                Debug.LogError("[UI] show page error with :" + pageName + " maybe null instance.");
                return null;
            }

            if (m_allPages == null)
            {
                m_allPages = new Dictionary<string, TTUIPage>();
            }

            TTUIPage page = null;
            if (m_allPages.ContainsKey(pageName))
            {
                page = m_allPages[pageName];
            }
            else
            {
                m_allPages.Add(pageName, pageInstance);
                page = pageInstance;
            }

            //if active before,wont active again.
            //if (page.isActive() == false)
            {
                //before show should set this data if need. maybe.!!
                if(pageData != null)
                {
                    page.m_data = pageData; 
                }



                page.Show();
                return page;
            }

        }
        /// <summary>
        /// Sync Show Page      1
        /// </summary>
        public static T ShowPage<T>() where T : TTUIPage, new()
        {
            return ShowPage<T>(null);
        }

        /// <summary>
        /// Sync Show Page With Page Data Input.
        /// </summary>
        public static void ShowPage(string pageName, TTUIPage pageInstance)
        {
            ShowPage(pageName, pageInstance);
        }

        #region change by osmin
        //用不同的id创建T类型的page
        //step 3 根据名称创建名字
        public static T ShowPageByName<T>(string pageName, object pageData) where T : TTUIPage, new()
        {
            if (string.IsNullOrEmpty(pageName))
            {
                Debug.LogError("[UI] show page error with :" + pageName + " is empty.");
                return null;
            }

            if (m_allPages == null)
            {
                m_allPages = new Dictionary<string, TTUIPage>();
            }

            TTUIPage page = null;
            if (m_allPages.ContainsKey(pageName))
            {
                page = m_allPages[pageName];
            }
            else
            {
                T instance = new T();
                m_allPages.Add(pageName, instance);
                page = instance;
            }

            //if active before,wont active again.
            //if (page.isActive() == false)
            {
                //before show should set this data if need. maybe.!!
                page.m_data = pageData;

                page.Show();
                return page as T;
            }
            

        }
        //by name step  2   
        public static T ShowPageByName<T>(string pageName) where T : TTUIPage, new()
        {
            return ShowPageByName<T>(pageName, null);
        }

        //根据名字获取Page
        public static TTUIPage GetPage(string pageName)
        {
            TTUIPage instance = null;
            try
            {
                m_allPages.TryGetValue(pageName, out instance);

            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }

            if (instance == null)
            {
                Debug.Log(pageName + "尚未创建!");
            }
            return instance;
        }
        //获取特定的page
        public static T GetPage<T>() where T : TTUIPage
        {
            String pageName = typeof(T).ToString();
            return GetPage(pageName) as T;
        }
        //获取特定类型的,别名为 pageName的ui
        public static T GetPage<T>(string pageName) where T:TTUIPage
        {
            TTUIPage instance = null;
            try
            {
                m_allPages.TryGetValue(pageName, out instance);

            }
            catch (System.Exception e)
            {
                Debug.Log(e.Message);
            }

            if (instance == null)
            {
                Debug.Log(pageName + "尚未创建!");
            }
            return instance as T;
        }
        //返回当前的类型对象
        //public static T ShowPageAndReturn<T>() where T : TTUIPage
        //{

        //}

        #endregion
        /// <summary>
        /// close current page in the "top" node.
        /// </summary>
        public static void ClosePage()
        {
            //Debug.Log("Back&Close PageNodes Count:" + m_currentPageNodes.Count);

            if (m_currentPageNodes == null || m_currentPageNodes.Count <= 1) return;

            TTUIPage closePage = m_currentPageNodes[m_currentPageNodes.Count - 1];
            m_currentPageNodes.RemoveAt(m_currentPageNodes.Count - 1);

            //show older page.
            //TODO:Sub pages.belong to root node.
            if (m_currentPageNodes.Count > 0)
            {
                TTUIPage page = m_currentPageNodes[m_currentPageNodes.Count - 1];
                if (page.isAsyncUI)
//                    ShowPage(page.name, page, () =>
//                    {
                        closePage.Hide();
//                    });
                else
                {
//                    ShowPage(page.name, page);

                    //after show to hide().
                    closePage.Hide();
                }
            }
        }
        /// <summary>
        /// 关闭目标界面,并且刷新当前界面的上一级界面 refresh   active
        /// </summary>
        public static void ClosePage(TTUIPage target)
        {
            //如果当前页处于隐藏状态,则在一个列表中移除它,然而实际并没有做别的操作
            if (target == null) return;
            if (target.isActive() == false)
            {
                if (m_currentPageNodes != null)
                {
                    for (int i = 0; i < m_currentPageNodes.Count; i++)
                    {
                        if (m_currentPageNodes[i] == target)
                        {
                            m_currentPageNodes.RemoveAt(i);
                            break;
                        }
                    }
                    return;
                }
            }
            //如果target在顶部,则移除它
            if (m_currentPageNodes != null && m_currentPageNodes.Count >= 1 && m_currentPageNodes[m_currentPageNodes.Count - 1] == target)
            {
                m_currentPageNodes.RemoveAt(m_currentPageNodes.Count - 1);

                //show older page.
                //TODO:Sub pages.belong to root node.
                //如果不是只剩target,则把最上层的ui刷新一遍
                if (m_currentPageNodes.Count > 0)
                {
                    TTUIPage page = m_currentPageNodes[m_currentPageNodes.Count - 1];
                    if (page.isAsyncUI)
//                        ShowPage(page.name, page, () =>
//                        {
                            target.Hide();
//                        });
                    else
                    {
//                        ShowPage(page.name, page);
                        target.Hide();
                    }

                    return;
                }
            }
            else if (target.CheckIfNeedBack())
            {
                for (int i = 0; i < m_currentPageNodes.Count; i++)
                {
                    if (m_currentPageNodes[i] == target)
                    {
                        m_currentPageNodes.RemoveAt(i);
                        target.Hide();
                        break;
                    }
                }
            }

            target.Hide();
        }

        public static void ClosePage<T>() where T : TTUIPage
        {
            try
            {
                Type t = typeof(T);
                string pageName = t.ToString();

                if (m_allPages != null && m_allPages.ContainsKey(pageName))
                {
                    ClosePage(m_allPages[pageName]);
                }
                else
                {
                    Debug.Log(pageName + "havnt show yet!");
                }
            }catch(System.Exception e)
            {
                Debug.Log(typeof(T).ToString()+"关闭失败");
                Debug.Log(e.Message);
            }

        }

        public static void ClosePage(string pageName)
        {
            try
            {
                if (m_allPages != null && m_allPages.ContainsKey(pageName))
                {
                    ClosePage(m_allPages[pageName]);
                }
                else
                {
                    Debug.LogError(pageName + " havnt show yet!");
                }

            }catch(System.Exception e)
            {
                Debug.Log(e.Message);
            }

        }

        #endregion

    }//TTUIPage
}//namespace