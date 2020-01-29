namespace MaterialSkin.Animations
{
    using System;

    public static class AnimationCustomQuadratic
    {
        public static double CalculateProgress(double progress)
        {
            double num = 0.6;
            return (1.0 - Math.Cos(((Math.Max(progress, num) - num) * 3.1415926535897931) / (2.0 - (2.0 * num))));
        }
    }
}

