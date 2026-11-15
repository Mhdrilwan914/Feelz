using UnityEngine;
public static class GameConfig
{
    public const int Capacity = 4, MaxLevel = 30;
    public static readonly string[] Names = { "cherry", "orange", "lemon", "lime", "lagoon", "blueberry", "grape", "guava", "coffee", "kiwi" };
    private static readonly string[] ColorHexValues =
    {
        "#ef476f", "#ff9f1c", "#ffd60a", "#2fd480", "#17c3e8",
        "#3a6bff", "#9b5de5", "#ff8fc7", "#8b5a3c", "#c9f31d"
    };

    public static Color[] Colors
    {
        get
        {
            var colors = new Color[ColorHexValues.Length];
            for (int colorIndex = 0; colorIndex < colors.Length; colorIndex++)
            {
                ColorUtility.TryParseHtmlString(ColorHexValues[colorIndex], out colors[colorIndex]);
            }

            return colors;
        }
    }

    public static Sprite Sprite(string spriteName)
    {
        return Resources.Load<Sprite>("Sprites/" + spriteName);
    }
}
