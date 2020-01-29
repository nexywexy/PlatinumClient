namespace MaterialSkin.Animations
{
    using System;

    public static class AnimationEaseOut
    {
        public static double CalculateProgress(double progress) => 
            ((-1.0 * progress) * (progress - 2.0));
    }
}

