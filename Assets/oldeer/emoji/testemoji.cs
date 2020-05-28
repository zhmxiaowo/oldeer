using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;

public class testemoji : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        string s = "😀😃😄😁⚽🇭🇰";
        Debug.Log(StringToUnicode(s));
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
}
