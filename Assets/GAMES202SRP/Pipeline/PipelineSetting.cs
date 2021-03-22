using System;
namespace MySRP
{
    public enum ColorSpace
    {
        linear,
        gamma
    }

    public static class PipelineSetting
    {
        public static ColorSpace activeColorSpace;


    }
}