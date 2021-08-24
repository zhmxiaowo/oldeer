经过上线项目测试,非常稳定这套东西

# Json
1. 使用了NewtonsfotJson
2. 序列化和反序列化类(全平台)
3. 支持嵌套List,Array等复合类型class

```C++
    [System.Serializable]
    public class Temp
    {
        public string data = "data";
        [JsonIgnore]
        public string ignoreStr = "Ignore";

        public List<object> objs = new List<object>();


        public static void test()
        {
            Temp t = new Temp();
            t.objs.Add("string");
            t.objs.Add(10);
            t.objs.Add(100.00f);
            string json = JsonConvert.SerializeObject(t);
            Debug.Log(json);
            Temp test = JsonConvert.DeserializeObject<Temp>(json);
        }
    }
```

* 结果
```
{
	"data": "data",
	"objs": ["string", 10, 100.0]
}
```


---

# TinyTeam UI 框架
1. 支持链式编程
2. 去除异步加载方式
3. 修复部分bug
4. UIRoot 使用Canvas承载

```
    private void Awake()
    {
        base.Awake();
    
        var v = TinyTeam.UI.TTUIPage.ShowPage<UIPanel>();               //根据类型去创建ui
        TinyTeam.UI.TTUIPage.ShowPageByName<UIPanel>("name1");          //根据名字去创建相同ui
        var v1 =TinyTeam.UI.TTUIPage.ShowPageByName<UIPanel>("name2");  //根据名字去创建相同的ui
    
    }

```
UIPanel.cs

```
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
```


# Context简易处理框架

ContextServer为中控中心
1. 所有的Context互相耦合
2. 继承IContext可以有效防止单例影响程序顺序
3. 所有Context可以在ContextServer共同获取


```
graph LR
IContext-->BaseContext
IContext-->MonoContext
BaseContext-->ContextServer
MonoContext-->ContextServer
IContextStart-->Init
Init-->Class:IContext
```


```c++
    //context
    public class TempClass:BaseContext
    {
        public string str = "TempClass";
    }

    public class SceneInitStart : IContextStart
    {
        public override void Awake()
        {
            base.Awake();
            //初始化当前场景的具体context
            TempClass temp = new TempClass();
        }


        public void test()
        {
            //可以在任何地方获取该context
            string s = ContextServer.GetContext<TempClass>().str;

            Debug.Log(s);
        }
    }

```
* 结果
```
TempClass
```

# Copyright
1. 创建.cs文件时候会自动补齐版权头部
2. 可在Copyright.cs内修改作者或者添加内容


```
        //作者
        private static string author = "作者名字";
```

# I/O 文件
1.FileHelper 帮助保存和读取文件

```c++
//栗子(多重载)

//保存
FileHelper.SaveToFile(string path, string data)
//读取
FileHelper.ReadFile(string path)
```

