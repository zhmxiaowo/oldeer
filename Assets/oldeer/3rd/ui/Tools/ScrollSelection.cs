using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


/// <summary>
/// 重要:
/// item第一个元素必须是Text类型的UI
/// item的第二个元素必须是Image类型的UI
/// </summary>
public class ScrollSelection :MonoBehaviour, IBeginDragHandler,IDragHandler,IEndDragHandler {
    public ScrollDirection scrollDirection = ScrollDirection.horizontal;

    public bool Animation_Rotate = false;
    public bool Animation_Scale = false;
    public bool Animation_Color = false;

    public RectTransform item;
    public RectTransform scrollRect;
//    [Range(1,5)]
//    [SerializeField]
//    private int _itemCount;

    //所有item的列表
    private ScrollItem[] items;
    //是否在拖拽
    private bool _isDrag = false;

    private Color TexColor;
    private float TexSize = 100;

    private Color ImgColor = Color.black;
    private Vector3 ImgScale = Vector3.one;
    [Header("顶点")]
    public string[] itemValues;
    public string[] itemNames;
    public Sprite[] itemImage;

    //重要,成功修改状态的事件回调
    public delegate void ConfirmDelegate(string value);
    public ConfirmDelegate OnValueStateChangeDelegate =  null; 

    void Awake()
    {
        if(scrollRect == null)
        {
            scrollRect = this.GetComponent<RectTransform>();
        }
        Text text = item.GetComponentInChildren<Text>();
        if(text != null)
        {
            TexColor = item.GetComponentInChildren<Text>().color; 
            TexSize = text.fontSize;
        }
        if(item.childCount > 1)
        {
            Image img = item.GetChild(1).GetComponent<Image>();
            ImgColor = img.color;
            ImgScale = img.transform.localScale;
        }
            
    }

	void Start () {
        //先根据大小和数量创建出item
        CreateItems();
        //初始化一次状态
        Scrolling();
//        OnValueStateChangeDelegate = new ConfirmDelegate((string value) =>
//            {
//                Debug.Log("change value: "+value + "  进度 " +leapCount);
//            });
        Set(0);
	}
//	void Update ()
//    {
//        if(Input.GetKeyDown(KeyCode.Space))
//        {
//            int i = Random.Range(-10, 10);
//            Debug.Log("要设置" + i);
//            Settle(i);
//        }
//	}
    public string Value
    {
        get{
            if(itemValues.Length > 0 && mid != null)
            {
                return  itemValues[mid.index];
            }
            return "0";
        }
    }


    private Vector2 delta = Vector2.zero;
    private ScrollItem left,right, mid; 
    public void OnDrag(PointerEventData data)
    {
        //当前UI拖拽要移动的大小
        delta = data.delta*2;
        //移动UI
        Scrolling();

    }  
    //创建item
    public void CreateItems()
    {
        //初始化方向和大小
        int tempcount  = 0 ;
        Vector3 dir = Vector2.left;
        float itemlen = item.rect.width;
        float rectlen = scrollRect.rect.width;

        //如果是竖直方向
        if(scrollDirection == ScrollDirection.vertical)
        {
            dir = Vector2.up;
            itemlen = item.rect.height;
            rectlen = scrollRect.rect.height;
        }
        //计算实际需要的item大小   这里设定为覆盖满后+2个
        tempcount = Mathf.CeilToInt(rectlen/itemlen)+2;

        //初始化所有item的数组
        items = new ScrollItem[tempcount];
        //循环创建item
        for(int i = 0; i < tempcount; i++)
        {
            ScrollItem si = new ScrollItem();
            GameObject go = Instantiate(item.gameObject) as GameObject;
            go.transform.SetParent(scrollRect);
            go.transform.localScale = Vector3.one;

            si.rt = go.GetComponent<RectTransform>();
            si.index = i >= itemValues.Length ? i - itemValues.Length : i;
            si.rt.anchoredPosition = dir * (rectlen*0.5f-(i+0.5f)*itemlen);
            go.name = "item"+i.ToString();

            if(i < itemNames.Length)
                go.GetComponentInChildren<Text>().text = itemNames[i];
            if (i < itemImage.Length)
                go.GetComponentInChildren<Image>().sprite = itemImage[i]; 
            go.SetActive(true);
            items[i] = si;

        }
    }
    public void Settle(int index)
    {
        delta = Vector2.zero;
        StopAllCoroutines();

        if( index <0 || index > itemValues.Length)
        {
            index = itemValues.Length - 1;
        }
        if(index >= itemValues.Length)
        {
            index = itemValues.Length;
        }
        int len = items.Length;
        Vector3 dir = Vector2.left;
        float itemlen = item.rect.width;
        float rectlen = scrollRect.rect.width;

        //如果是竖直方向
        if(scrollDirection == ScrollDirection.vertical)
        {
            dir = Vector2.up;
            itemlen = item.rect.height;
            rectlen = scrollRect.rect.height;
        }
        //计算出 从头排列下来的第一个index
        int beginIndex = index - Mathf.CeilToInt(len / 2);
        if(beginIndex < 0)
        {
            beginIndex = itemValues.Length - beginIndex + 1;
        }
        for (int i = 0; i < len; i++)
        {
            items[i].rt.anchoredPosition = dir * (rectlen * 0.5f - (i - 0.5f) * itemlen);
            items[i].index = beginIndex;
            if(beginIndex < itemNames.Length)
                items[i].rt.GetComponentInChildren<Text>().text = itemNames[beginIndex];
            if (beginIndex < itemImage.Length)
                items[i].rt.GetComponentInChildren<Image>().sprite = itemImage[beginIndex]; 
            
            beginIndex++;
            if(beginIndex > itemValues.Length - 1)
            {
                beginIndex = 0;
            }
        }
        Scrolling();
        if(OnValueStateChangeDelegate != null && mid != null)
            OnValueStateChangeDelegate(itemValues[mid.index]);
    }

    public void Scrolling()
    {

        //初始化方向和大小
        int tempcount  = 0 ;
        Vector2 dir = Vector2.left;
        float itemlen = item.rect.width;
        float rectlen = scrollRect.rect.width;
        
        //判断是垂直的列表还是横向的列表
        if(scrollDirection == ScrollDirection.horizontal)
        {
            delta.y = 0;
        }else
        {
            delta.x = 0;

            dir = Vector2.down;
            itemlen = item.rect.height;
            rectlen = scrollRect.rect.height;
        }
        if(items.Length > 0 )
        {
            float scale = 1;
            left = items[0];
            right = items[0];
            mid = items[0];
//            float horv = scrollDirection == ScrollDirection.horizontal ? it.rt.anchoredPosition.x : it.rt.anchoredPosition.y;
            foreach(ScrollItem it in items)
            {
                //滚动
                it.rt.anchoredPosition += delta;
                //当前和中心点的阈值

                Text tex = it.rt.GetChild(0).GetComponent<Text>();
                Image img = null;

                float tango = it.rt.anchoredPosition.x;
                if(scrollDirection == ScrollDirection.vertical)
                {
                    tango = it.rt.anchoredPosition.y;
                }
                scale = (tango)/rectlen*0.8f;

                if(it.rt.childCount > 1)
                {
                    img = it.rt.GetChild(1).GetComponent<Image>();
                }

                if(Animation_Scale)
                {
                    if(tex != null)
                    {
                        tex.fontSize = (int)( TexSize * (1 - Mathf.Abs(scale)));
                    }
                    if(img != null)
                    {
                        img.rectTransform.localScale = (1 - Mathf.Abs(tango / rectlen)) * ImgScale;
                    }
                }
                if(Animation_Color)
                {
                    if(tex != null)
                    {
                        tex.color = new Color(TexColor.r,TexColor.g,TexColor.b,(1 - Mathf.Abs(scale) * 2));
                    }
                    if(img != null)
                    {
                        img.color = new Color(ImgColor.r,ImgColor.g,ImgColor.b,(1 - Mathf.Abs(scale) * 2));
                    }

                }
                if(Animation_Rotate)
                {
                    if (tex != null)
                    {
                        if(scrollDirection == ScrollDirection.vertical)
                        {
                            tex.rectTransform.eulerAngles = new Vector3(90 * (scale), 0, 0);
                        }else
                        {
                            tex.rectTransform.eulerAngles = new Vector3(0, 90 * (scale), 0);
                        }
                    }
                    if(img != null)
                    {
                        if(scrollDirection == ScrollDirection.vertical)
                        {
                            img.rectTransform.eulerAngles = new Vector3(90 * (scale), 0, 0);
                        }else
                        {
                            img.rectTransform.eulerAngles = new Vector3(0, 90 * (scale), 0);
                        }
                    }
                }


                if(Animation_Scale)
                {
                    //获取文字信息


                    Color newcol = TexColor*(1- Mathf.Abs(scale)*2);
                    tex.color = newcol;



                    

//                    for(int i = 0 ; i < it.rt.childCount;i++)
//                    {
//                        tex.fontSize = (int)(100 * (1 - Mathf.Abs(scale)));
//                    }
                }
//                scale = (it.rt.anchoredPosition.x)/rectlen*0.5f;
//                //获取文字信息
//                Text tex = it.rt.GetChild(0).GetComponent<Text>();
//                //旋转角度
//                tex.rectTransform.eulerAngles = new Vector3(0, 270*((it.rt.anchoredPosition.x)/rectlen*0.5f), 0);
//                //设置字体颜色
//                Color newcol = TexColor*(1- Mathf.Abs(scale));
////                newcol.a = 1;
//                tex.color = newcol;
                //字体大小
                //                tex.fontSize = (int)(100 * (1 - Mathf.Abs(scale)));

                if(scrollDirection == ScrollDirection.horizontal)
                {
                    //判定最左和最右的item
                    if(it.rt.anchoredPosition.x < left.rt.anchoredPosition.x)
                    {
                        left = it;
                    }
                    if(it.rt.anchoredPosition.x > right.rt.anchoredPosition.x)
                    {
                        right = it;
                    }
                    if(Mathf.Abs(mid.rt.anchoredPosition.x) > Mathf.Abs(it.rt.anchoredPosition.x))
                    {
                        mid = it;
                    }
                }else
                {
                    //判定最上和最下的item
                    if(it.rt.anchoredPosition.y < left.rt.anchoredPosition.y)
                    {
                        left = it;
                    }
                    if(it.rt.anchoredPosition.y > right.rt.anchoredPosition.y)
                    {
                        right = it;
                    }
                    if(Mathf.Abs(mid.rt.anchoredPosition.y) > Mathf.Abs(it.rt.anchoredPosition.y))
                    {
                        mid = it;
                    }
                }


            }
            if (scrollDirection == ScrollDirection.horizontal)
            {
                //右边滑动的情况下
                if (delta.x > 0)
                {
                    if (right.rt.anchoredPosition.x > (rectlen + itemlen)* 0.5f )
                    {
                        right.rt.anchoredPosition = left.rt.anchoredPosition + dir * itemlen;
                        right.index = left.index - 1;
                        if (right.index < 0)
                        {
                            if (itemNames.Length > 0)
                                right.index = itemNames.Length - 1;
                            else if (itemImage.Length > 0)
                                right.index = itemImage.Length - 1;
                            else
                                right.index = 0;

                        }

                        //修改item的值
                        if (right.index < itemNames.Length)
                            right.rt.GetComponentInChildren<Text>().text = itemNames[right.index];
                        if (right.index < itemImage.Length)
                            right.rt.GetComponentInChildren<Image>().sprite = itemImage[right.index]; 
                    }
                }
                //左边滑动的情况下
                if (delta.x < 0)
                {
                    if (left.rt.anchoredPosition.x < -(rectlen  + itemlen)* 0.5f)
                    {
                        left.rt.anchoredPosition = right.rt.anchoredPosition - dir * itemlen;
                        left.index = right.index + 1;

                        if (left.index >= itemValues.Length)
                        {
                            left.index = 0;
                        }

                        //修改item的值
                        if (left.index < itemNames.Length)
                            left.rt.GetComponentInChildren<Text>().text = itemNames[left.index];
                        if (left.index < itemImage.Length)
                            left.rt.GetComponentInChildren<Image>().sprite = itemImage[left.index]; 
                    }
                }
            }
            else
            {
                //上边滑动的情况下
                if (delta.y > 0)
                {
                    if (right.rt.anchoredPosition.y > (rectlen + itemlen)* 0.5f)
                    {
                        right.rt.anchoredPosition = left.rt.anchoredPosition + dir * itemlen;

                        right.index = left.index + 1;
                        if (right.index >= itemNames.Length && right.index >= itemImage.Length)
                        {
                            right.index = 0;
                        }

                        //修改item的值
                        if (right.index < itemNames.Length)
                            right.rt.GetComponentInChildren<Text>().text = itemNames[right.index];
                        if (right.index < itemImage.Length)
                            right.rt.GetComponentInChildren<Image>().sprite = itemImage[right.index]; 
                    }
                }
                //下边滑动的情况下
                if (delta.y < 0)
                {
                    if (left.rt.anchoredPosition.y < -(rectlen + itemlen) * 0.5f)
                    {
                        left.rt.anchoredPosition = right.rt.anchoredPosition - dir * itemlen;
                        left.index = right.index - 1;

                        if (left.index < 0)
                        {
                            if (itemNames.Length > 0)
                                left.index = itemNames.Length - 1;
                            else if (itemImage.Length > 0)
                                left.index = itemImage.Length - 1;
                            else
                                left.index = 0;
                        }

                        //修改item的值
                        if (left.index < itemNames.Length)
                            left.rt.GetComponentInChildren<Text>().text = itemNames[left.index];
                        if (left.index < itemImage.Length)
                            left.rt.GetComponentInChildren<Image>().sprite = itemImage[left.index]; 
                    }
                }
            }
      
        }
    }

    public void OnBeginDrag(PointerEventData data) 
    {
        _isDrag = true;
    }
    public void OnEndDrag(PointerEventData data)
    {
        delta = Vector2.zero;
        _isDrag = false;

        StartCoroutine(Leap(0.5f,(float times)=>{
            delta =  Vector2.Lerp(mid.rt.anchoredPosition,Vector2.zero,times) - mid.rt.anchoredPosition;
            Scrolling();
        }));
        

    }
    int leapCount = 0;
    IEnumerator Leap(float time = 1f,System.Action<float> ac = null,System.Action OnFinish = null)
    {
        if(ac != null)
        {
            leapCount++;
            float total = Time.time;
            float curr = Time.time - total;
            while(curr <= time && !_isDrag)
            {
                ac(curr);
                curr = Time.time - total;
                yield return null;
            }
            ac(1);
            if(OnFinish != null && !_isDrag)
            {
                OnFinish();
            }
//            yield return new WaitForSeconds(0.3f);
            // 执行回调
            if(leapCount < 2 && !_isDrag && OnValueStateChangeDelegate != null && mid != null && mid.index < itemValues.Length)
            {
                OnValueStateChangeDelegate(itemValues[mid.index]);
            }
            leapCount--; 
        }
    }

    public void Set(int idx)
    {
        if (items == null || items.Length == 0)
            return;
        if(idx < 0)
        {
            idx = itemValues.Length - 1;
        }
        if(idx > itemValues.Length - 1)
        {
            idx = 0;
        }

        if(mid != null && left != null && right != null)
        {
            if (mid.index == idx)
                return;
            int p = idx - mid.index;
            int q = 0;
            //先找到当前item的位置  item的位置是不变的,只是left的
            int start = 0;
            ScrollItem si = scrollDirection == ScrollDirection.horizontal ? left : right;
            for(int i = 0 ; i < scrollRect.childCount;i++)
            {
                if(items[i] == si)
                {
                    start = i;
                    break;
                }
            }
//                p = idx;
            q = si.index +p;
            if(q >= itemValues.Length)
            {
//                q = itemValues.Length - q;
                q =  q - itemValues.Length;
            }
            if(q < 0)
            {
                q = itemValues.Length + q ;
            }
//            for(int i = 0 ; i < items.Length;i++)
//            {
//                Debug.Log(start +"  "+ items[start].index);
//                start++;
//                if (start >= items.Length)
//                    start = 0;
//            }
            for (int i = 0; i < items.Length; i++)
            {
                items[start].index = q;
                if(q < itemNames.Length)
                    items[start].rt.GetComponentInChildren<Text>().text = itemNames[q];
                if(q < itemImage.Length)
                    items[start].rt.GetComponentInChildren<Image>().sprite = itemImage[q]; 
                q++;
                if(q >= itemValues.Length)
                    q = 0;
                
                start++;
                if(start >= items.Length)
                {
                    start = 0;
                }
            }

            Scrolling();
        }

    }
    public void Set(string v)
    {
        if (items == null || items.Length == 0)
            return;
        if(v.Length > 0)
        {
            for(int i = 0 ; i < itemValues.Length;i++)
            {
                if(v == itemValues[i])
                {
                    Set(i);
                    break;
                }
            }
        }
    }


    public enum ScrollDirection
    {
        horizontal,
        vertical
    }

    public class ScrollItem
    {
        public RectTransform rt;
        public int index;
//        public System.Action onClick;

    }

}