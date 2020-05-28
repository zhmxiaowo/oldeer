﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System;
using System.Globalization;

public class EmojiText : Text
{
    private static Dictionary<string, EmojiInfo> EmojiIndex = null;
    private static Dictionary<string, string> EmojiNameToIndex = null;
    struct EmojiInfo
    {
        public float x;
        public float y;
        public float size;
        public int len;
    }

    public override string text
    {
        get
        {
            return base.text;
        }
        set
        {
            initEmojiData();
            base.text = SetUITextThatHasEmoji(value);
        }
    }

    public static Dictionary<string, string> EmojiNameToText
    {
        get
        {
            initEmojiData();
            return EmojiNameToIndex;
        }
    }

    private static void initEmojiData()
    {
        if (EmojiIndex == null)
        {
            EmojiIndex = new Dictionary<string, EmojiInfo>();
            EmojiNameToIndex = new Dictionary<string, string>();
            TextAsset datafilestr = Resources.Load<TextAsset>("emoji");
            string[] allstrs;
            if (datafilestr.text.IndexOf('\r') != -1)
            {
                allstrs = datafilestr.text.Split('\r');
            }
            else
            {
                allstrs = datafilestr.text.Split('\n');
            }

            string line;
            bool isTitle = true;
            for (int i = 0; i < allstrs.Length; i++)
            {
                if (isTitle)
                {
                    isTitle = false;
                    continue;
                }
                line = allstrs[i];
                line = line.Replace("\n", "");
                if (line.IndexOf('\t') != -1)
                {
                    string[] strs = line.Split('\t');
                    EmojiInfo info;
                    info.x = float.Parse(strs[3]);
                    info.y = float.Parse(strs[4]);
                    info.size = float.Parse(strs[5]);
                    info.len = 0;
                    EmojiIndex.Add(strs[1], info);
                    EmojiNameToIndex.Add(GetConvertedString(strs[0]), strs[1]);
                }

            }
        }
    }


    readonly UIVertex[] m_TempVerts = new UIVertex[4];
    protected override void OnPopulateMesh(VertexHelper toFill)
    {
        if (font == null)
            return;
        initEmojiData();
        Dictionary<int, EmojiInfo> emojiDic = new Dictionary<int, EmojiInfo>();
        if (supportRichText)
        {
            MatchCollection matches = Regex.Matches(text, "\\[[a-z0-9A-Z]+\\]");
            for (int i = 0; i < matches.Count; i++)
            {
                EmojiInfo info;
                if (EmojiIndex.TryGetValue(matches[i].Value, out info))
                {
                    info.len = matches[i].Length;
                    emojiDic.Add(matches[i].Index, info);
                }
            }
        }

        // We don't care if we the font Texture changes while we are doing our Update.
        // The end result of cachedTextGenerator will be valid for this instance.
        // Otherwise we can get issues like Case 619238.
        m_DisableFontTextureRebuiltCallback = true;

        Vector2 extents = rectTransform.rect.size;

        var settings = GetGenerationSettings(extents);
        cachedTextGenerator.Populate(text, settings);

        Rect inputRect = rectTransform.rect;

        // get the text alignment anchor point for the text in local space
        Vector2 textAnchorPivot = GetTextAnchorPivot(alignment);
        Vector2 refPoint = Vector2.zero;
        refPoint.x = Mathf.Lerp(inputRect.xMin, inputRect.xMax, textAnchorPivot.x);
        refPoint.y = Mathf.Lerp(inputRect.yMin, inputRect.yMax, textAnchorPivot.y);

        // Determine fraction of pixel to offset text mesh.
        Vector2 roundingOffset = PixelAdjustPoint(refPoint) - refPoint;

        // Apply the offset to the vertices
        IList<UIVertex> verts = cachedTextGenerator.verts;
        float unitsPerPixel = 1 / pixelsPerUnit;
        //Last 4 verts are always a new line...
        int vertCount = verts.Count - 4;

        toFill.Clear();
        if (roundingOffset != Vector2.zero)
        {
            for (int i = 0; i < vertCount; ++i)
            {
                int tempVertsIndex = i & 3;
                m_TempVerts[tempVertsIndex] = verts[i];
                m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                m_TempVerts[tempVertsIndex].position.x += roundingOffset.x;
                m_TempVerts[tempVertsIndex].position.y += roundingOffset.y;
                if (tempVertsIndex == 3)
                    toFill.AddUIVertexQuad(m_TempVerts);
            }
        }
        else
        {
            float repairDistance = 0;
            float repairDistanceHalf = 0;
            float repairY = 0;
            if (vertCount > 0)
            {
                repairY = verts[3].position.y;
            }
            for (int i = 0; i < vertCount; ++i)
            {
                EmojiInfo info;
                int index = i / 4;
                if (emojiDic.TryGetValue(index, out info))
                {
                    //compute the distance of '[' and get the distance of emoji 
                    float charDis = (verts[i + 1].position.x - verts[i].position.x) * 3;
                    m_TempVerts[3] = verts[i];//1
                    m_TempVerts[2] = verts[i + 1];//2
                    m_TempVerts[1] = verts[i + 2];//3
                    m_TempVerts[0] = verts[i + 3];//4

                    //the real distance of an emoji
                    m_TempVerts[2].position += new Vector3(charDis, 0, 0);
                    m_TempVerts[1].position += new Vector3(charDis, 0, 0);

                    //make emoji has equal width and height
                    float fixValue = (m_TempVerts[2].position.x - m_TempVerts[3].position.x - (m_TempVerts[2].position.y - m_TempVerts[1].position.y));
                    m_TempVerts[2].position -= new Vector3(fixValue, 0, 0);
                    m_TempVerts[1].position -= new Vector3(fixValue, 0, 0);

                    float curRepairDis = 0;
                    if (verts[i].position.y < repairY)
                    {// to judge current char in the same line or not
                        repairDistance = repairDistanceHalf;
                        repairDistanceHalf = 0;
                        repairY = verts[i + 3].position.y;
                    }
                    curRepairDis = repairDistance;
                    int dot = 0;//repair next line distance
                    for (int j = info.len - 1; j > 0; j--)
                    {
                        if ((i + j * 4) < verts.Count)
                        {
                            if (verts[i + j * 4].position.y >= verts[i + 3].position.y)
                            {
                                repairDistance += verts[i + j * 4 + 1].position.x - m_TempVerts[2].position.x;
                                break;
                            }
                            else
                            {
                                dot = i + 4 * j;
                            }
                        }
                    }
                    if (dot > 0)
                    {
                        int nextChar = i + info.len * 4;
                        if (nextChar < verts.Count)
                        {
                            repairDistanceHalf = verts[nextChar].position.x - verts[dot].position.x;
                        }
                    }

                    //repair its distance
                    for (int j = 0; j < 4; j++)
                    {
                        m_TempVerts[j].position -= new Vector3(curRepairDis, 0, 0);
                    }

                    m_TempVerts[0].position *= unitsPerPixel;
                    m_TempVerts[1].position *= unitsPerPixel;
                    m_TempVerts[2].position *= unitsPerPixel;
                    m_TempVerts[3].position *= unitsPerPixel;

                    float pixelOffset = emojiDic[index].size / 32 / 2;
                    m_TempVerts[0].uv1 = new Vector2(emojiDic[index].x + pixelOffset, emojiDic[index].y + pixelOffset);
                    m_TempVerts[1].uv1 = new Vector2(emojiDic[index].x - pixelOffset + emojiDic[index].size, emojiDic[index].y + pixelOffset);
                    m_TempVerts[2].uv1 = new Vector2(emojiDic[index].x - pixelOffset + emojiDic[index].size, emojiDic[index].y - pixelOffset + emojiDic[index].size);
                    m_TempVerts[3].uv1 = new Vector2(emojiDic[index].x + pixelOffset, emojiDic[index].y - pixelOffset + emojiDic[index].size);

                    toFill.AddUIVertexQuad(m_TempVerts);

                    i += 4 * info.len - 1;
                }
                else
                {
                    int tempVertsIndex = i & 3;
                    if (tempVertsIndex == 0 && verts[i].position.y < repairY)
                    {
                        repairY = verts[i + 3].position.y;
                        repairDistance = repairDistanceHalf;
                        repairDistanceHalf = 0;
                    }
                    m_TempVerts[tempVertsIndex] = verts[i];
                    m_TempVerts[tempVertsIndex].position -= new Vector3(repairDistance, 0, 0);
                    m_TempVerts[tempVertsIndex].position *= unitsPerPixel;
                    if (tempVertsIndex == 3)
                        toFill.AddUIVertexQuad(m_TempVerts);
                }
            }

        }
        m_DisableFontTextureRebuiltCallback = false;
    }


    public string SetUITextThatHasEmoji(string inputString)
    {
        StringBuilder sb = new StringBuilder();
        int i = 0;
        while (i < inputString.Length)
        {
            string singleChar = inputString.Substring(i, 1);
            string doubleChar = "";
            string fourChar = "";

            if (i < (inputString.Length - 1))
            {
                doubleChar = inputString.Substring(i, 2);
            }

            if (i < (inputString.Length - 3))
            {
                fourChar = inputString.Substring(i, 4);
            }

            if (EmojiNameToIndex.ContainsKey(fourChar))
            {
                // Check 64 bit emojis first
                sb.Append(EmojiNameToIndex[fourChar]);
                i += 4;
            }
            else if (EmojiNameToIndex.ContainsKey(doubleChar))
            {
                // Then check 32 bit emojis
                sb.Append(EmojiNameToIndex[doubleChar]);
                i += 2;
            }
            else if (EmojiNameToIndex.ContainsKey(singleChar))
            {
                // Finally check 16 bit emojis
                sb.Append(EmojiNameToIndex[singleChar]);
                i++;
            }
            else
            {
                sb.Append(inputString[i]);
                i++;
            }
        }
        return sb.ToString();
    }

    private static string GetConvertedString(string inputString)
    {
        string[] converted = inputString.Split('-');
        for (int j = 0; j < converted.Length; j++)
        {
            converted[j] = char.ConvertFromUtf32(System.Convert.ToInt32(converted[j], 16));
        }
        return string.Join(string.Empty, converted);
    }

}
