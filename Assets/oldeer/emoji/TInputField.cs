using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using System.Text;

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
    }
    //emoji最多是4个字符
    LinkedList<char> charList = new LinkedList<char>();
    int count = 0;

    private char MyOnValidateInput(string text, int charIndex, char addedChar)
    {
        string utf16 = CharToUnicode(addedChar);
        if(Regex.IsMatch(utf16, @"[\u231a-\u1f9dd]"))
        {
            //是emoji,那么就匹配一下,如果失败就丢进linklist
            if(EmojiText.EmojiNameToText.ContainsKey(utf16))
            {
                //转码
            }
        }


        //some emoji is 4byte or 2byte or 1byte
        charList.AddLast(addedChar);
        if (charList.Count > 4)
        {
            charList.RemoveLast();
        }

        Debug.Log("text"+text);
        Debug.Log("charIndex"+charIndex);
        Debug.Log("addedChar"+addedChar.ToString());


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
    
