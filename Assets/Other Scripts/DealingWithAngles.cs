using UnityEngine;

namespace MusicPlayer
{
    internal static class DealingWithAngles
    {
        public static float Editor2Runtime(float value)
        {
            value %= 360f;
            if (value < 0)
                value += 360;

            return value;
        }

        public static float Runtime2Editor(float value)
        {
            value %= 360f;
            if (value > 180f)
                value -= 360;

            return value;
        }

        public static float CalcAngularSize(float size, float distance)
        {
            return Mathf.Rad2Deg * 2f * Mathf.Atan2(size, 2 * distance); // See https://en.wikipedia.org/wiki/Angular_diameter
        }
    }
}
