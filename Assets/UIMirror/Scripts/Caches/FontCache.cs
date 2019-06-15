using UnityEngine;

public class FontCache
{
    public static Font GetFont(string fontName)
    {
        return Resources.GetBuiltinResource<Font>(fontName + ".ttf");
    }
}
