namespace awkward
{
  public class Math
  {
    public static float map(float val, float sourceMin, float sourceMax, float targetMin, float targetMax)
    {
      return targetMin + (targetMax-targetMin) * ( (val-sourceMin) / (sourceMax-sourceMin) );
    }

    static public float loop(float val, float min, float max)
    {
      if (val > max) val = max - val;
      else if (val < min) val = val + max;
      return val;
    }
  }
}
