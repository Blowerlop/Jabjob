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

        public static Gradient GetGradient(Color color)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKey;
            GradientAlphaKey[] alphaKey;
            colorKey = new GradientColorKey[2];
            colorKey[0].color = color;
            colorKey[0].time = 0.0f;
            colorKey[1].color = GetVariantColor(color);
            colorKey[1].time = 1.0f;
            alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;

            gradient.SetKeys(colorKey, alphaKey);
            return gradient; 
        }
        public static Gradient GetGradient(Color startColor, Color endColor)
        {
            Gradient gradient = new Gradient();
            GradientColorKey[] colorKey;
            GradientAlphaKey[] alphaKey;
            colorKey = new GradientColorKey[2];
            colorKey[0].color = startColor;
            colorKey[0].time = 0.0f;
            colorKey[1].color = endColor;
            colorKey[1].time = 1.0f;
            alphaKey = new GradientAlphaKey[2];
            alphaKey[0].alpha = 1.0f;
            alphaKey[0].time = 0.0f;
            alphaKey[1].alpha = 1.0f;
            alphaKey[1].time = 1.0f;

            gradient.SetKeys(colorKey, alphaKey);
            return gradient;
        }
    }
}
