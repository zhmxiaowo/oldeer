#define UNITY_ANDROID
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Text;
using System;

public class TInputField : InputField
{

    List<string> patterns = new List<string>();
    protected override void Awake()
    {
        base.Awake();
        patterns.Add(@"\p{Cs}");
        patterns.Add(@"[\u2702-\u27B0]");
        patterns.Add(@"[\u231a-\u1f9dd]");
        onValidateInput = MyOnValidateInput;
        Application.runInBackground = true;
    }
    //some emoji is 4byte or 2byte or 1byte
    List<char> charList = new List<char>(5);
    private char MyOnValidateInput(string text, int charIndex, char addedChar)
    {
        string utf16 = CharToUnicode(addedChar);
        Debug.Log("addedChar" + utf16);

        this.text = EmojiText.SetUITextThatHasEmoji(text);
        return '\0';

        //if (Regex.IsMatch(new string(addedChar,1), @"[\u1f600-\u1f9dd]"))
        //{
        //    charList.Add(addedChar);
        //    //是emoji,那么就匹配一下,如果失败就丢进linklist
        //    string str = new string(charList.ToArray());
        //    if (EmojiText.EmojiNameToText.ContainsKey(str))
        //    {
        //        m_Text = text.Substring(0, text.Length - 1) + EmojiText.EmojiNameToText[str];
        //        charList.Clear();
        //        //转码
        //        Debug.Log("emoji!");
        //    }else
        //    {
        //        Debug.Log("not emoji!");
        //        if(charList.Count > 4)
        //        {
        //            Debug.Log("Emoji check failure.clean all chars");
        //            charList.Clear();
        //        }
        //    }
        //    return '\0';
        //}
        //byte[] uncoide_16 = System.Text.Encoding.Default.GetBytes(text);
        //Debug.Log("text"+Encoding.UTF8.GetString(uncoide_16));
        //Debug.Log("charIndex"+charIndex);
        //Debug.Log("addedChar" + utf16);


        //Debug.Log(builder.ToString());
        return addedChar;
    }

    private bool isEmoji(char s)
    {
        return Regex.IsMatch(CharToUnicode(s), @"[\u231a-\u1f9dd]");
    }
    //private bool BEmoji(string s)
    //{
    //    Regex.IsMatch(s, patterns[i]);
    //    bool bEmoji = false;
    //    for (int i = 0; i < patterns.Count; ++i)
    //    {
    //        bEmoji = Regex.IsMatch(s, patterns[i]);
    //        if (bEmoji)
    //        {
    //            break;
    //        }
    //    }
    //    return bEmoji;
    //}

    //public void AddPatterns(string s)
    //{
    //    patterns.Add(s);
    //}

    //public void ClearPatterns(string s)
    //{
    //    patterns.Clear();
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
    public static string CharToUnicode(char str)
    {
        return string.Format("\\u{0:x2}{1:x2}", (str & 0xFF00) >> 8, (str & 0xFF));
    }
}
    
