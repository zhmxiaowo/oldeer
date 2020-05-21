using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ItemInterface : MonoBehaviour {
    //当前数据的index
    public int index = 0;
    public RectTransform rt = null;
    public virtual void Init()
    {

    }
    public virtual void Awake()
    {
        rt = transform as RectTransform;
        Init();
    }
    //设定数据
    public virtual void SetData(int i)
    {
        index = i;
    }

}
