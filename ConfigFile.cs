﻿using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;

// Load a config file made of CATEGORIES containing key->value pairs
// and provides functions to retrieve those informations
// Usage example :
// void Start() {
//  ConfigFile gameTweak = new ConfigFile("game_tweak.txt");
//  int startLives = gameTweak.GetCategory("PLAYER").getInt("start_lives");
// }
public class ConfigFile
{
  public const string CONFIG_FILE_PREFIX = "config";
  protected const string CONFIG_FILE_CATEGORY_SYMBOL = "=";
  protected const string CONFIG_FILE_COMMENT = "//";
  protected const char CONFIG_FILE_PARAM_SEPARATOR = ':';
  protected const string CONFIG_DEFAULT_CATEGORY_NAME = "DEFAULT";

  string filePath = "";
  ConfigCategory[] categories;

  public ConfigFile(string filename) 
  {
    filePath = filename;
    parseFile(filePath);
  }

  // this method is created because the constructor leaves unclear if the filename given is just a filename, a relative path or an absolute path
  public static ConfigFile CreateFromExternalFolderPath(string relativeFilePath)
  {
    return new ConfigFile(relativeFilePath);
  }

  public void reload() {
    parseFile(filePath);
  }

  /* retourne l'index de la ligne de la cat */
  public int checkForCategory(string cat)
  {
    string path = HalperExternal.GetExternalFolderName() + filePath;
    if (!File.Exists(path)) Debug.LogWarning("{ConfigFile} didn't find config file at path " + path);
    else
    {
      string[] lines = File.ReadAllLines(path);

      for (int i = 0; i < lines.Length; i++)
      {
        if (lines[i] == "=" + cat) return i;
      }

      List<string> finalLines = new List<string>();
      for (int i = 0; i < lines.Length; i++)
      {
        finalLines.Add(lines[i]);
      }
      
      finalLines.Add("="+cat);

      Debug.Log("added category =" + cat + " to file "+path);

      File.WriteAllLines(path, finalLines.ToArray());
      
      return finalLines.Count-1;
    }

    return -1;
  }

  public void removeLines(string label) {
    string path = HalperExternal.GetExternalFolderName() + filePath;
    if (!File.Exists(path)) Debug.LogWarning("{ConfigFile} didn't find config file at path " + path);
    else {
      string[] lines = File.ReadAllLines(path);
      List<string> finalLines = new List<string>();

      int currentLine = 0;
      while (currentLine < lines.Length)
      {
        string line = lines[currentLine];
        //on dégage les lignes qui contiennent le label donné en param
        if(!line.Contains(label)) {
          finalLines.Add(line);
        }
        currentLine++;
      }

      File.WriteAllLines(path, finalLines.ToArray());
    }
  }

  public void writeRange(string categ, string name, Vector2 v)
  {
    string path = HalperExternal.GetExternalFolderName() + filePath;

    //Debug.Log("{ConfigFile} parsing config file : "+path);

    if (!File.Exists(path))
    {
      Debug.LogWarning("{ConfigFile} didn't find config file at path " + path);
    }else
    {
      string[] lines = File.ReadAllLines(path);
      List<string> finalLines = new List<string>();
      int currentLine = 0;
      bool incateg = false;
      bool lineInserted = false;

      while(currentLine < lines.Length)
      {
        string line = lines[currentLine];
        
        if (line.StartsWith("="))
        {
          if(line.StartsWith("="+categ)) incateg = true;
          else incateg = false;
        }else
        {
          if(incateg && !lineInserted)
          {
            string newline = name+":[" + v.x + ";" + v.y + "]";
            finalLines.Add(newline);
            lineInserted = true;
          }
        }

        if(!line.StartsWith(name))
        {
          finalLines.Add(line);
        }
        currentLine++;
      }

      // if we didn't find category, add it
      if(!lineInserted)
      {
        finalLines.Add("\n="+categ);
        string newline = name+":[" + v.x + ";" + v.y + "]";
        finalLines.Add(newline);
      }

      File.WriteAllLines(path, finalLines.ToArray());
    }
  }


  public void writeVector3Array(string categ, string name, Vector3[] v)
  {
    string path = HalperExternal.GetExternalFolderName() + filePath;

    //Debug.Log("{ConfigFile} parsing config file : "+path);

    if (!File.Exists(path))
    {
      Debug.LogWarning("{ConfigFile} didn't find config file at path " + path);
    }else
    {
      int catIndex = checkForCategory(categ);

      string[] lines = File.ReadAllLines(path);

      List<string> before = new List<string>();
      
      //add everything before insert
      for (int i = 0; i <= catIndex; i++)
      {
        before.Add(lines[i]);
      }

      List<string> after = new List<string>();
      
      //insert new info
      string newline = name + ":";
      for (int i = 0; i < v.Length; i++)
      {
        newline += "(" + v[i].x + "," + v[i].y + "," + v[i].z + ")" + ",";
      }
      if (v.Length > 0) newline = newline.Remove(newline.Length - 1);
      after.Add(newline);

      //add everything after
      for (int i = catIndex+1; i < lines.Length; i++)
      {
        after.Add(lines[i]);
      }

      //concat
      before.AddRange(after);

      //save
      string[] output = before.ToArray();
      File.WriteAllLines(path, output);
    }
  }

  public ConfigCategory GetCategory(string categoryName)
  {
    if (categoryName == null) return null;
    if (categories == null) return null;

    for(int i = 0; i < categories.Length; i++)
    {
      if(categories[i].name == categoryName)
      {
        return categories[i];
      }
    }
    return null;
  }

  void parseFile(string filename)
  {
    string path = HalperExternal.GetExternalFolderName() + filename;

    //Debug.Log("{ConfigFile} parsing config file : "+path);

    if (!File.Exists(path))
    {
      Debug.LogWarning("{ConfigFile} didn't find config file at path " + path);
    }else
    {
      List<ConfigCategory> parametersList = new List<ConfigCategory>();
      string[] lines = File.ReadAllLines(path);
      int currentLine = 0;

      while(currentLine < lines.Length)
      {
        string line = lines[currentLine];
        
        if (line.StartsWith("="))
        {
          ConfigCategory p = parseCategory(lines, ref currentLine);
          parametersList.Add(p);
        }
        else
        {
          currentLine++;
        }
      }

      categories = parametersList.ToArray();
    }
  }


  ConfigCategory parseCategory(string[]data, ref int idx)
  {
    string groupName = data[idx].Substring(1);
    ConfigCategory c = new ConfigCategory();
    c.name = groupName;

    //on choppe toutes les lignes après le titre de la category =cat jusqu'à la prochaine cat ou la fin du fichier
    idx++;

    //c'est que c'était un titre avec rien du tout après
    if (idx >= data.Length) return c;
    
    string line = data[idx];
    while (!line.StartsWith(CONFIG_FILE_CATEGORY_SYMBOL) && idx < data.Length)
    {
      if (line.Length > 0 && !line.StartsWith(CONFIG_FILE_COMMENT))
      {
        string[] key_value = line.Split(CONFIG_FILE_PARAM_SEPARATOR);
        //Debug.Log(line + " , " + key_value.Length);
        c.parameters[key_value[0]] = key_value[1];
      }

      //next line
      idx++;
      if (idx < data.Length) {
        line = data[idx];
      }
    }
    
    return c;
  }
}


public class ConfigCategory
{
  public string name;
  public Dictionary<string, string> parameters;

  public ConfigCategory()
  {
    parameters = new Dictionary<string, string>();
  }

  public bool hasParameter(string paramName) {
    return parameters.ContainsKey(paramName);
  }

  public string getString(string paramName, string defaultValue = "")
  {
    if(!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    return val;
  }  

  public float getFloat(string paramName, float defaultValue = 0)
  {
    if(!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    return float.Parse(val, NumberStyles.Any, CultureInfo.InvariantCulture);
  }

  public int getInt(string paramName, int defaultValue = 0)
  {
    if(!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    return int.Parse(val);
  }


  public bool getBool(string paramName, bool defaultValue = false)
  {
    if(!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    return bool.Parse(val); 
  }


  public float getFloatFromRange(string paramName, float defaultValue = 0)
  {
    if(!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    val = val.Substring(1, val.Length-2);
    string[] min_max = val.Split(';');
    float min = float.Parse(min_max[0], NumberStyles.Any, CultureInfo.InvariantCulture);
    float max = float.Parse(min_max[1], NumberStyles.Any, CultureInfo.InvariantCulture);
    return Random.Range(min, max);
  }


  public int getIntFromRange(string paramName, int defaultValue = 0)
  {
    if(!parameters.ContainsKey(paramName))
    {
      return defaultValue;
    }
    string val = parameters[paramName];
    val = val.Substring(1, val.Length-2);
    string[] min_max = val.Split(';');
    int min = int.Parse(min_max[0]);
    int max = int.Parse(min_max[1]);
    return Random.Range(min, max);
  }

  public Vector2 getRange(string paramName)
  {
    if(!parameters.ContainsKey(paramName))
    {
      return Vector2.zero;
    }
    string val = parameters[paramName];
    val = val.Substring(1, val.Length-2);
    string[] min_max = val.Split(';');
    float min = float.Parse(min_max[0], NumberStyles.Any, CultureInfo.InvariantCulture);
    float max = float.Parse(min_max[1], NumberStyles.Any, CultureInfo.InvariantCulture);
    return new Vector2(min, max);
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

  public int[] getIntArray(string paramName)
  {
    if(!parameters.ContainsKey(paramName))
    {
      return null;
    }
    string val = parameters[paramName];
    string[] arrayVals = val.Split(',');
    int[] res = new int[arrayVals.Length];
    for(int i = 0; i < res.Length; i++)
    {
      res[i] = int.Parse(arrayVals[i]);
    }
    return res;
  }

  public Vector3[] getVector3Array(string paramName)
  {
    if(!parameters.ContainsKey(paramName))
    {
      return null;
    }

    string val = parameters[paramName];
    Vector3[] res = null;
    if(val.Length > 0)
    {
      string[] arrayVals = val.Split(')');
      res = new Vector3[arrayVals.Length-1];
      for(int i = 0; i < arrayVals.Length-1; i++)
      {
        int firstParen = arrayVals[i].IndexOf('(');
        string s = arrayVals[i].Substring(firstParen+1);
        string[] values = s.Split(',');
        res[i].x = float.Parse(values[0], NumberStyles.Any, CultureInfo.InvariantCulture);
        res[i].y = float.Parse(values[1], NumberStyles.Any, CultureInfo.InvariantCulture);
        res[i].z = float.Parse(values[2], NumberStyles.Any, CultureInfo.InvariantCulture);
      }
    }

    return res;
  }
}
