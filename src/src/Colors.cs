using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace src
{
    public static class Colors
    {
        public static float Clamp(float value, float min, float max)
        {
            if (value < min)
                return min;
            if (value > max)
                return max;
            return value;
        }
        public static Color Mix(Color color1, Color color2, float ratio)
        {
            // Ensure ratio is between 0 and 1
            ratio = Clamp(ratio, 0f, 1f);

            // Calculate the mixed color components
            int red = (int)(color1.R * (1 - ratio) + color2.R * ratio);
            int green = (int)(color1.G * (1 - ratio) + color2.G * ratio);
            int blue = (int)(color1.B * (1 - ratio) + color2.B * ratio);

            // Clamp color values to the range 0-255
            red = (int)Clamp(red, 0, 255);
            green = (int)Clamp(green, 0, 255);
            blue = (int)Clamp(blue, 0, 255);

            // Return the resulting color
            return Color.FromArgb(red, green, blue);
        }
        private static Color GetColorFromLighting(Vector3 normal, Color baseColor, LightSource light)
        {
            double cos = Vector3.Dot(light.direction, normal) / (light.direction.Length() * normal.Length());

            // If the angle between the light and the normal is negative, the surface is in shadow
            if (cos <= 0)
                return Colors.Mix(baseColor, Color.Black, 0.2f);

            // Mix the base color with black, using the cosine of the angle to modulate brightness
            return Colors.Mix(baseColor, Color.Black, (float)cos);
        }
    }
}
