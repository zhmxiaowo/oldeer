using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System;

public class testemoji : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string s = "😀😃😄😁⚽🇭🇰";
        Debug.Log(StringToUnicode(s));
        Debug.Log(ConvertEmoji2UnicodeHex("🇨🇦"));
    }

    // Update is called once per frame
    //void Update()
    //{
        
    //}
    public static string StringToUnicode(string str)
    {
        char[] chars = str.ToCharArray();
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < chars.Length; i++)
        {
            builder.Append(string.Format("\\u{0:x2}{1:x2}", (chars[i] & 0xFF00) >> 8, (chars[i] & 0xFF)));
        }
        return builder.ToString();
    }

    //byte[] b = new byte[2];
    //b[0] = (byte)((addedChar & 0xFF00) >> 8);
    //b[1] = (byte)(addedChar & 0xFF);
    //StringBuilder builder = new StringBuilder();
    //builder.Append(string.Format("\\u{0:x2}{1:x2}", b[0], b[1]));
    public static string ConvertEmoji2UnicodeHex(string emoji)
    {
        if (string.IsNullOrWhiteSpace(emoji))
            return emoji;
        byte[] bytes = Encoding.UTF8.GetBytes(emoji);
        string firstItem = Convert.ToString(bytes[0], 2); //获取首字节二进制
        int iv;
        if (bytes.Length == 1)
        {
            //单字节字符
            iv = Convert.ToInt32(firstItem, 2);
        }
        else
        {
            //多字节字符
            StringBuilder sbBinary = new StringBuilder();
            sbBinary.Append(firstItem.Substring(bytes.Length + 1).TrimStart('0'));
            for (int i = 1; i < bytes.Length; i++)
            {
                string item = Convert.ToString(bytes[i], 2);
                item = item.Substring(2);
                sbBinary.Append(item);
            }
            iv = Convert.ToInt32(sbBinary.ToString(), 2);
        }
        return Convert.ToString(iv, 16).PadLeft(4, '0');
    }
}
