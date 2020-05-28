using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using System.Xml;
using System.IO;
namespace  test
{
    public class test : MonoBehaviour
    {

        public EmojiText txt;

        void Start()
        {
            txt.text = "\u6211\ud83d\udcaa\ud83d\udc45\ud83d\udeb4\ud83d\udeb4\ud83d\udeb4\ud83d\udeb4\ud83d\udeb4\ud83d\udeb4";
        }


        public static byte[] ToHex(string hexString)
        {
            byte[] datas = new byte[hexString.Length / 2];
            var j = 0;
            for (var i = 0; i < hexString.Length; i += 2)
                datas[j++] = Convert.ToByte(hexString.Substring(i, 2), 16);
            return datas;
        }

    }

}
