using System;

namespace Coyote.Console.Common
{
    public static class GenericFunction
    {
        public static float GSTCalculation(float passValue, float gstPercent)
        {
            float gst = 0;
            if (passValue != 0 && gstPercent != 0)
            {
                float rateFactor = 1 + (gstPercent / 100);
                float exGstAmt = passValue / rateFactor;
                gst = Convert.ToSingle(Math.Round(passValue - exGstAmt, 2));
            }
            return gst;
        }
    }
}
