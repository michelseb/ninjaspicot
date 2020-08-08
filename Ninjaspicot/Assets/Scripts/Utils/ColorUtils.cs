using System.Collections.Generic;
using UnityEngine;

public enum CustomColor
{
    None,
    Red,
    Green,
    White,
    Blue,
    Yellow
}

public static class ColorUtils
{
    public static Color Red => new Color(0.87f, 0.23f, 0.23f, 1);
    public static Color Green => new Color(0, 1, 0.5f, 1);
    public static Color White => new Color(1, 1, 1, 1);
    public static Color Blue => new Color(0, 128, 255);
    public static Color Yellow => new Color(255, 200, 0);

    public static Dictionary<CustomColor, Color> CustomColors = new Dictionary<CustomColor, Color> 
    {
        { CustomColor.White, White },
        { CustomColor.Green, Green },
        { CustomColor.Red, Red },
        { CustomColor.Blue, Blue },
        { CustomColor.Yellow, Yellow },
    };

    public static Color GetColor(CustomColor color, float alpha = 1)
    {
        if (!CustomColors.ContainsKey(color))
            return White;

        var result = CustomColors[color];

        return new Color(result.r, result.g, result.b, alpha);
    }

}
