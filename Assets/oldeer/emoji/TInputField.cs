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
        onValidateInput = MyOnValidateInput;
    }

    private char MyOnValidateInput(string text, int charIndex, char addedChar)
    {
        Debug.Log("text"+text);
        Debug.Log("charIndex"+charIndex);
        Debug.Log("addedChar"+addedChar.ToString());
        byte[] b = new byte[2];
        b[0] = (byte)((addedChar & 0xFF00) >> 8);
        b[1] = (byte)(addedChar & 0xFF);
        StringBuilder builder = new StringBuilder();
        builder.Append(string.Format("\\u{0:x2}{1:x2}", b[0], b[1]));

        Debug.Log(builder.ToString());
        return addedChar;
    }

    private bool BEmoji(string s)
    {
        bool bEmoji = false;
        for (int i = 0; i < patterns.Count; ++i)
        {
            bEmoji = Regex.IsMatch(s, patterns[i]);
            if (bEmoji)
            {
                break;
            }
        }
        return bEmoji;
    }

    public void AddPatterns(string s)
    {
        patterns.Add(s);
    }

    public void ClearPatterns(string s)
    {
        patterns.Clear();
    }
}
    
