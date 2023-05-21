using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    public static class ColorHelpersUtilities 
    {
        public static Color GetVariantColor(Color color)
        {
            float h, s, v;
            Color variantColor;
            Color.RGBToHSV(color, out h, out s, out v);
            v = v > 0.80f ? 0.45f : v < 0.60f ? v + 0.35f : 1;
            variantColor = Color.HSVToRGB(h, s, v);
            return variantColor;
        }
    }
}
