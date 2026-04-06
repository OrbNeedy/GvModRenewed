

namespace GvMod.Common.Utils
{
    public static class Easing
    {
        public static float EaseInExponential(float x, float maxX)
        {
            if (maxX == 0) return 0;

            float realX = x / maxX;
            return realX * realX;
        }
    }
}
