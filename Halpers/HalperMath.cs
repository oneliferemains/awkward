using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperMath
{
  public static float map(float val, float sourceMin, float sourceMax, float targetMin, float targetMax)
  {
    return targetMin + (targetMax - targetMin) * ((val - sourceMin) / (sourceMax - sourceMin));
  }

  static public float loop(float val, float min, float max)
  {
    if (val > max) val = max - val;
    else if (val < min) val = val + max;
    return val;
  }

  static public bool closeTo(float val, float target, float range)
  {
    return val > target - range && val < target + range;
  }

  static public bool closeToZero(float val)
  {
    return Mathf.Abs(val) < 0.001f;
  }

  static public Vector2 solvePointProjection(Vector2 p, Vector2 a, Vector2 b)
  {
    Vector2 ap = p - a;
    Vector2 ab = b - a;
    return a + Vector2.Dot(ap, ab) / Vector2.Dot(ab, ab) * ab;
  }

  /// <summary>
  /// clamped to [a,b] ?
  /// </summary>
  static public Vector3 solvePointProjectionSegment(Vector3 p, Vector3 a, Vector3 b)
  {
    Vector3 ab = b - a;
    float absq = Vector3.Dot(ab, ab);

    if (absq == 0) return a;

    Vector3 ap = p - a;
    float t = Vector3.Dot(ap, ab) / absq;
    if (t < 0f) return a;
    else if (t > 1f) return b;

    return a + (t * ab);
  }

  //https://forum.unity.com/threads/line-intersection.17384/
  public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
  {
    float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num, offset;
    float x1lo, x1hi, y1lo, y1hi;

    Ax = p2.x - p1.x;
    Bx = p3.x - p4.x;

    // X bound box test/
    if (Ax < 0)
    {
      x1lo = p2.x; x1hi = p1.x;
    }
    else
    {
      x1hi = p2.x; x1lo = p1.x;
    }

    if (Bx > 0)
    {
      if (x1hi < p4.x || p3.x < x1lo) return false;
    }
    else
    {
      if (x1hi < p3.x || p4.x < x1lo) return false;
    }

    Ay = p2.y - p1.y;
    By = p3.y - p4.y;

    // Y bound box test//
    if (Ay < 0)
    {
      y1lo = p2.y; y1hi = p1.y;
    }
    else
    {
      y1hi = p2.y; y1lo = p1.y;
    }

    if (By > 0)
    {
      if (y1hi < p4.y || p3.y < y1lo) return false;
    }
    else
    {
      if (y1hi < p3.y || p4.y < y1lo) return false;
    }

    Cx = p1.x - p3.x;
    Cy = p1.y - p3.y;
    d = By * Cx - Bx * Cy;  // alpha numerator//
    f = Ay * Bx - Ax * By;  // both denominator//

    // alpha tests//
    if (f > 0)
    {
      if (d < 0 || d > f) return false;
    }
    else
    {
      if (d > 0 || d < f) return false;
    }

    e = Ax * Cy - Ay * Cx;  // beta numerator//

    // beta tests //
    if (f > 0)
    {
      if (e < 0 || e > f) return false;
    }
    else
    {
      if (e > 0 || e < f) return false;
    }

    // check if they are parallel
    if (f == 0) return false;

    // compute intersection coordinates //
    num = d * Ax; // numerator //
    offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;   // round direction //
    intersection.x = p1.x + (num + offset) / f;

    num = d * Ay;
    offset = same_sign(num, f) ? f * 0.5f : -f * 0.5f;
    intersection.y = p1.y + (num + offset) / f;

    return true;
  }

  private static bool same_sign(float a, float b)
  {
    return ((a * b) >= 0f);
  }


  public static bool LineLineIntersection(out Vector3 intersection, Vector3 linePoint1, Vector3 lineVec1, Vector3 linePoint2, Vector3 lineVec2)
  {
    Vector3 lineVec3 = linePoint2 - linePoint1;
    Vector3 crossVec1and2 = Vector3.Cross(lineVec1, lineVec2);
    Vector3 crossVec3and2 = Vector3.Cross(lineVec3, lineVec2);

    float planarFactor = Vector3.Dot(lineVec3, crossVec1and2);

    //is coplanar, and not parrallel
    if (Mathf.Abs(planarFactor) < 0.0001f && crossVec1and2.sqrMagnitude > 0.0001f)
    {
      float s = Vector3.Dot(crossVec3and2, crossVec1and2) / crossVec1and2.sqrMagnitude;
      intersection = linePoint1 + (lineVec1 * s);
      return true;
    }
    else
    {
      intersection = Vector3.zero;
      return false;
    }
  }

  /// <summary>
  /// https://blog.dakwamine.fr/?p=1943
  /// Gets the coordinates of the intersection point of two lines.
  /// </summary>
  /// <param name="A1">A point on the first line.</param>
  /// <param name="A2">Another point on the first line.</param>
  /// <param name="B1">A point on the second line.</param>
  /// <param name="B2">Another point on the second line.</param>
  /// <param name="found">Is set to false of there are no solution. true otherwise.</param>
  /// <returns>The intersection point coordinates. Returns Vector2.zero if there is no solution.</returns>
  static public Vector2 GetIntersectionPointCoordinates(Vector2 A1, Vector2 A2, Vector2 B1, Vector2 B2)
  {
    float tmp = (B2.x - B1.x) * (A2.y - A1.y) - (B2.y - B1.y) * (A2.x - A1.x);

    if (tmp == 0)
    {
      // No solution!
      //found = false;
      return Vector2.zero;
    }

    float mu = ((A1.x - B1.x) * (A2.y - A1.y) - (A1.y - B1.y) * (A2.x - A1.x)) / tmp;

    //found = true;

    return new Vector2(
        B1.x + (B2.x - B1.x) * mu,
        B1.y + (B2.y - B1.y) * mu
    );
  }

  static public bool compareVector2(Vector2 a, Vector2 b)
  {
    float epsilon = 0.1f;
    if (b.x < a.x - epsilon || b.x > a.x + epsilon) return false;
    if (b.y < a.y - epsilon || b.y > a.y + epsilon) return false;
    return true;
  }

  static public bool compareVector3(Vector3 a, Vector3 b)
  {
    float epsilon = 0.1f;
    if (b.x < a.x - epsilon || b.x > a.x + epsilon) return false;
    if (b.y < a.y - epsilon || b.y > a.y + epsilon) return false;
    if (b.z < a.z - epsilon || b.z > a.z + epsilon) return false;
    return true;
  }

}
