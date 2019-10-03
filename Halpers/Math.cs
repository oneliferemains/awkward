namespace awkward
{
  public class Math
  {
    public static float map(float val, float sourceMin, float sourceMax, float targetMin, float targetMax)
    {
      return targetMin + (targetMax-targetMin) * ( (val-sourceMin) / (sourceMax-sourceMin) );
    }
  }
}
