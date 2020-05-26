/*vvvvvvvvvvv
 * @Author: chiuan wei 
 * @Date: 2017-05-27 18:14:53 
 * @Last Modified by: chiuan wei
 * @Last Modified time: 2017-05-27 18:33:48
 */
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using System.Threading.Tasks;
using System.Linq;
namespace TinyTeam.UI {


   /// <summary>
   /// Init The UI Root
   /// 
   /// UIRoot
   /// -Canvas
   /// --FixedRoot
   /// --NormalRoot
   /// --PopupRoot
   /// -Camera
   /// </summary>
   public class TTUIRoot : MonoBehaviour {
        private static TTUIRoot m_Instance = null;
        public static TTUIRoot Instance {
            get {
                if (m_Instance == null) {
                    InitRoot();
                }
                return m_Instance;
            }
        }

        public Transform root;
        public RectTransform fixedRoot;
        public RectTransform normalRoot;
        public RectTransform popupRoot;
        public Camera uiCamera;

        static void InitRoot() {
            GameObject go = new GameObject("@UIRoot");
            go.layer = LayerMask.NameToLayer("UI");
            m_Instance = go.AddComponent<TTUIRoot>();
            go.AddComponent<RectTransform>();

            Canvas can = go.AddComponent<Canvas>();
            can.renderMode = RenderMode.ScreenSpaceCamera;
            //can.renderMode = RenderMode.ScreenSpaceOverlay;
            can.pixelPerfect = false;
            can.planeDistance = 1;
            go.AddComponent<GraphicRaycaster>();

            m_Instance.root = go.transform;

            GameObject camObj = new GameObject("@UICamera");
            camObj.layer = LayerMask.NameToLayer("UI");
            //测试出现的可能情况
            //camObj.transform.parent = go.transform;
            go.transform.SetParent(camObj.transform,false);
            camObj.transform.localPosition = new Vector3(0, 100, -100f);
            Camera cam = camObj.AddComponent<Camera>();
            cam.clearFlags = CameraClearFlags.Depth;
            cam.orthographic = true;
            cam.farClipPlane = 1000f;
            can.worldCamera = cam;
            cam.cullingMask = 1 << 5;
            cam.nearClipPlane = -2000f;
            cam.farClipPlane = 200f;
            cam.useOcclusionCulling = false;
            cam.depth = 50f;
            m_Instance.uiCamera = cam;

            //add audio listener
            //camObj.AddComponent<AudioListener>();
            //camObj.AddComponent<GUILayer>();

            CanvasScaler cs = go.AddComponent<CanvasScaler>();
            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            Vector2 resolotion = new Vector2(1080f, 1920f);//new Vector2(Screen.width, Screen.height);//
            // iphonex 适配 
            //if (DeviceScreenRect.iPhoneX)
            //{
            //    //resolotion = new Vector2(1125f, 2436f);
            //    //去除掉安全区域的px
            //    //812 - 44 -44 ppi        2436 px
            //    //---------------- = -------------
            //    //     812     ppi        2202 px   
            //    resolotion = new Vector2(1125f, 2202f);
            //}
            cs.referenceResolution = resolotion;
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;

            //2018.8.18 osmin 使用width适配,天然优势
            // cs.matchWidthOrHeight = 0.5f;
            cs.matchWidthOrHeight = 0f;
            //set the raycaster
            //GraphicRaycaster gr = go.AddComponent<GraphicRaycaster>();

            GameObject subRoot;
//            subRoot = CreateSubCanvasForRoot(go.transform, 0);
            subRoot = CreateSubCanvas(go.transform, 0);
            subRoot.name = "NormalRoot";
            m_Instance.normalRoot = subRoot.transform as RectTransform;
            m_Instance.normalRoot.transform.localScale = Vector3.one;

            subRoot = CreateSubCanvas(go.transform, 2);
            subRoot.name = "FixedRoot";
            m_Instance.fixedRoot = subRoot.transform as RectTransform;
            m_Instance.fixedRoot.transform.localScale = Vector3.one;

            subRoot = CreateSubCanvas(go.transform, 4);
            subRoot.name = "PopupRoot";
            m_Instance.popupRoot = subRoot.transform as RectTransform;
            m_Instance.popupRoot.transform.localScale = Vector3.one;

            //add Event System
            GameObject esObj = GameObject.Find("EventSystem");
            if (esObj != null) {
                GameObject.DestroyImmediate(esObj);
            }

            GameObject eventObj = new GameObject("EventSystem");
            eventObj.layer = LayerMask.NameToLayer("UI");
            eventObj.transform.SetParent(go.transform);
            eventObj.AddComponent<EventSystem>();
            eventObj.AddComponent<UnityEngine.EventSystems.StandaloneInputModule>();
//            eventObj.AddComponent<UnityEngine.EventSystems.TouchInputModule>();
        }

        //采用面板的模式
        static GameObject CreateSubCanvasForRoot(Transform root, int sort) {
            GameObject go = new GameObject("canvas");
            go.transform.parent = root;
            go.layer = LayerMask.NameToLayer("UI");

            RectTransform rect = go.AddComponent<RectTransform>();
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;

            //  Canvas can = go.AddComponent<Canvas>();
            //  can.overrideSorting = true;
            //  can.sortingOrder = sort;
            //  go.AddComponent<GraphicRaycaster>();

            return go;
        }

        //采用画布的模式
        static GameObject CreateSubCanvas(Transform root,int sort)
        {
            GameObject go = new GameObject("canvas");
            go.transform.parent = root;
            go.layer = LayerMask.NameToLayer("UI");
            RectTransform rect = go.AddComponent<RectTransform>();

            Canvas can = go.AddComponent<Canvas>();
            can.overrideSorting = true;
            can.sortingOrder = sort;
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Left, 0, 0);
            rect.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, 0, 0);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.anchoredPosition3D = Vector3.zero;
            go.AddComponent<GraphicRaycaster>();

            //子canvas不需要以下东西
//            can.worldCamera = worldcam;
//            CanvasScaler cs = go.AddComponent<CanvasScaler>();
//            cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
//            cs.referenceResolution = new Vector2(1080f, 1920f);
//            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;
            return go;
        }
        //ui update
        private void Update()
        {
            if(TTUIPage.allPages != null && TTUIPage.allPages.Count > 0)
            {
                var values = TTUIPage.allPages.Values.ToList();
                for (var i = 0; i < values.Count; i++)
                {
                    if (values[i].isActive())
                    {
                        values[i].Update();
                    }   
                }
            }
        }
        //当被销毁时,ui需要做什么
        void OnDestroy()
        {
            m_Instance = null;
            TTUIPage.ClearNodes();
            TTUIPage.ClearAllPage();
        }

        //当暂停程序时,ui要做什么
        private void OnApplicationFocus(bool pause)
        {
            if (TTUIPage.allPages != null && TTUIPage.allPages.Count > 0)
            {
                foreach (var i in TTUIPage.allPages.Values)
                {
                    if (i.isActive())
                    {
                        i.OnApplicationPause(pause);
                    }
                }
            }
        }

        //新增 添加ui不可控的方法
        public static void EnableRayCaster(bool ignore)
        {
            if(m_Instance != null)
            {
                //m_Instance.root.GetComponent<GraphicRaycaster>().enabled = ignore;
                var uiRaycasters = m_Instance.root.GetComponentsInChildren<BaseRaycaster>(true);
                for(int i = 0; i < uiRaycasters.Length; i ++)
                {
                    uiRaycasters[i].enabled = ignore;
                }
                //m_Instance.root.GetComponent<GraphicRaycaster>().enabled = ignore;
                //m_Instance.fixedRoot.GetComponent<GraphicRaycaster>().enabled = ignore;
                //m_Instance.popupRoot.GetComponent<GraphicRaycaster>().enabled = ignore;
                //m_Instance.normalRoot.GetComponent<GraphicRaycaster>().enabled = ignore;
            }
        }
        public static async void ShortDisable(float time = 1f)
        {
            if (m_Instance != null)
            {
                EnableRayCaster(false);
                await Task.Delay((int)(time * 1000));
                EnableRayCaster(true);
            }
        }
        static IEnumerator _ShortDisable(float time)
        {
            EnableRayCaster(false);
            yield return new WaitForSeconds(time);
            EnableRayCaster(true);
        }
    }
}