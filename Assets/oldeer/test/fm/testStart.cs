using UnityEngine;

namespace test
{
    public class testStart : IContextStart
    {

        private void Awake()
        {
            base.Awake();

            var v = TinyTeam.UI.TTUIPage.ShowPage<UIPanel>();               //��������ȥ����ui
            TinyTeam.UI.TTUIPage.ShowPageByName<UIPanel>("name1");          //��������ȥ������ͬui
            var v1 =TinyTeam.UI.TTUIPage.ShowPageByName<UIPanel>("name2");  //��������ȥ������ͬ��ui
            Debug.Log(v.gameObject.name);
            Debug.Log(TinyTeam.UI.TTUIPage.GetPage("name2") == null);       //��������ȥ��ȡ��ͬ��ui


        }
    }
}

