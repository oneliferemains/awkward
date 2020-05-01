using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;

public class ConfigCategory
{

  public string name;
  public Dictionary<string, string> parameters;

  public ConfigCategory(string name)
  {
    this.name = name;
    parameters = new Dictionary<string, string>();
  }
  public ConfigCategory()
  {
    parameters = new Dictionary<string, string>();
  }

  public bool hasParameter(string paramName)
  {
    //Debug.Log(parameters.ContainsKey(paramName)+" , "+parameters.Count);

    //foreach(KeyValuePair<string,string> kv in parameters) Debug.Log(kv.Key + " , " + kv.Value);

    return parameters.ContainsKey(paramName);
  }

  /// <summary>
  /// Pour récup tt les params de la catégorie en paramètre de la fonction dans la catégorie
  /// </summary>
  public void merge(ConfigCategory cat)
  {
    //Debug.Log("  merging " + name + " with " + cat.name);

    //on peut pas modif un dico dans un foreach
    List<string> otherKeys = new List<string>(cat.parameters.Keys);

    foreach (string otherKey in otherKeys)
    {
      if (!parameters.ContainsKey(otherKey))
      {
        parameters.Add(otherKey, cat.parameters[otherKey]);
        //Debug.Log("    added " + otherKey + " -> " + cat.parameters[otherKey]);
      }
      else
      {
        //Debug.Log("    applied " + otherKey + " -> " + cat.parameters[otherKey]);
        parameters[otherKey] = cat.parameters[otherKey];
      }

    }
  }






  // string


  public string getString(string paramName, string defaultValue = "")
  {
    if (!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    return val;
  }

  // floats

  public float getFloat(string paramName, float defaultValue = 0)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    return float.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture);
  }


  public float getFloatFromRange(string paramName, float defaultValue = 0)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    val = val.Substring(1, val.Length - 2);
    string[] min_max = val.Split(';');
    float min = float.Parse(min_max[0], NumberStyles.Any, CultureInfo.InvariantCulture);
    float max = float.Parse(min_max[1], NumberStyles.Any, CultureInfo.InvariantCulture);
    return Random.Range(min, max);
  }


  public float[] getFloatArray(string paramName)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return null;
    }
    string val = parameters[paramName];
    string[] arrayVals = val.Split(',');
    float[] res = new float[arrayVals.Length];
    for (int i = 0; i < res.Length; i++)
    {
      res[i] = float.Parse(arrayVals[i], NumberStyles.Any, CultureInfo.InvariantCulture);
    }
    return res;
  }

  // ints

  public int getInt(string paramName, int defaultValue = 0)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    return int.Parse(val);
  }

  public int getIntFromRange(string paramName, int defaultValue = 0)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    val = val.Substring(1, val.Length - 2);
    string[] min_max = val.Split(';');
    int min = int.Parse(min_max[0]);
    int max = int.Parse(min_max[1]);
    return Random.Range(min, max);
  }

  public int[] getIntArray(string paramName)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return null;
    }
    string val = parameters[paramName];
    string[] arrayVals = val.Split(',');
    int[] res = new int[arrayVals.Length];
    for (int i = 0; i < res.Length; i++)
    {
      res[i] = int.Parse(arrayVals[i]);
    }
    return res;
  }

  // bools

  public bool getBool(string paramName, bool defaultValue = false)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    return bool.Parse(val);
  }

  // vectors

  public Vector2 getRange(string paramName)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return Vector2.zero;
    }
    string val = parameters[paramName];
    // On avait peut-être mis des des crochets pour entourer le range ?
    // val = val.Substring(1, val.Length - 2);
    string[] min_max = val.Split(';');
    float min = float.Parse(min_max[0], NumberStyles.Any, CultureInfo.InvariantCulture);
    float max = float.Parse(min_max[1], NumberStyles.Any, CultureInfo.InvariantCulture);
    return new Vector2(min, max);
  }


  public Vector3[] getVector3Array(string paramName)
  {
    if (!parameters.ContainsKey(paramName))
    {
      return null;
    }

    string val = parameters[paramName];
    Vector3[] res = null;
    if (val.Length > 0)
    {
      string[] arrayVals = val.Split(')');
      res = new Vector3[arrayVals.Length - 1];
      for (int i = 0; i < arrayVals.Length - 1; i++)
      {
        int firstParen = arrayVals[i].IndexOf('(');
        string s = arrayVals[i].Substring(firstParen + 1);
        string[] values = s.Split(',');
        res[i].x = float.Parse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture);
        res[i].y = float.Parse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture);
        res[i].z = float.Parse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture);
      }
    }

    return res;
  }


  public Vector4 getVector4(string paramName, Vector4 defaultValue)
  {
    Vector4 res = Vector4.zero;
    if (!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    string[] channels = val.Split(',');
    res.x = float.Parse(channels[0], NumberStyles.Any, CultureInfo.InvariantCulture);
    res.y = float.Parse(channels[1], NumberStyles.Any, CultureInfo.InvariantCulture);
    res.z = float.Parse(channels[2], NumberStyles.Any, CultureInfo.InvariantCulture);
    res.w = float.Parse(channels[3], NumberStyles.Any, CultureInfo.InvariantCulture);

    return res;
  }

  // colors

  public Color getColor(string paramName)
  {
    Color c = Color.white;
    if (!parameters.ContainsKey(paramName))
    {
      return c;
    }
    string val = parameters[paramName];
    string[] channels = val.Split(',');
    c.r = float.Parse(channels[0], NumberStyles.Any, CultureInfo.InvariantCulture);
    c.g = float.Parse(channels[1], NumberStyles.Any, CultureInfo.InvariantCulture);
    c.b = float.Parse(channels[2], NumberStyles.Any, CultureInfo.InvariantCulture);
    c.a = float.Parse(channels[3], NumberStyles.Any, CultureInfo.InvariantCulture);
    return c;
  }

}
