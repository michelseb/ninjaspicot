using System.Collections.Generic;
using UnityEngine;

public enum CustomColor
{
    None,
    Red,
    DarkRed,
    Green,
    White,
    Blue,
    Yellow,
    Black,
    NightBlue,
    MidnightBlue,
    Grey
}

public static class ColorUtils
{
    public static Color Red => new Color(0.87f, 0.23f, 0.23f, 1);
    public static Color Green => new Color(0, 1, 0.5f, 1);
    public static Color White => new Color(1, 1, 1, 1);
    public static Color Blue => new Color(0, 0.8f, 1);
    public static Color NightBlue => new Color(0, .1f, .2f);
    public static Color MidnightBlue => new Color(0, 0, .05f);
    public static Color Yellow => new Color(1, .9f, 0);
    public static Color Black => new Color(0, 0, 0);
    public static Color DarkRed => new Color(.55f, 0, 0);
    public static Color Grey => new Color(.45f, .5f, .55f);

    public static Dictionary<CustomColor, Color> CustomColors = new Dictionary<CustomColor, Color>
    {
        { CustomColor.White, White },
        { CustomColor.Green, Green },
        { CustomColor.Red, Red },
        { CustomColor.Blue, Blue },
        { CustomColor.Yellow, Yellow },
        { CustomColor.Black, Black },
        { CustomColor.NightBlue, NightBlue },
        { CustomColor.MidnightBlue, MidnightBlue },
        { CustomColor.DarkRed, DarkRed },
        { CustomColor.Grey, Grey }
    };

    public static Color GetColor(CustomColor color, float alpha = 1)
    {
        if (!CustomColors.ContainsKey(color))
            return White;

        var result = CustomColors[color];

        return new Color(result.r, result.g, result.b, alpha);
    }

}
