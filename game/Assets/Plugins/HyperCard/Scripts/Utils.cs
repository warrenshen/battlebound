/***************************************************************************\
Project:        HyperCard
Copyright (c)   Enixion
Developer       Bourgot Jean-Louis
\***************************************************************************/
namespace HyperCard
{
    public static class Utils 
    {
        public static float Remap (float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
}
