using UnityEngine;

namespace test
{
    public class testStart : IContextStart
    {

        private void Awake()
        {
            base.Awake();

            var v = TinyTeam.UI.TTUIPage.ShowPage<UIPanel>();               //根据类型去创建ui
            TinyTeam.UI.TTUIPage.ShowPageByName<UIPanel>("name1");          //根据名字去创建相同ui
            var v1 =TinyTeam.UI.TTUIPage.ShowPageByName<UIPanel>("name2");  //根据名字去创建相同的ui
            Debug.Log(v.gameObject.name);
            Debug.Log(TinyTeam.UI.TTUIPage.GetPage("name2") == null);       //根据名字去获取相同的ui


        }
    }
}

