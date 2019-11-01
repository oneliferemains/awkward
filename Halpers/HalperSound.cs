using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HalperSound
{
  public static AudioClip InvertClip(AudioClip source)
  {
    //Debug.LogFormat("Inverting clip {0} with {1} samples and duration {2}", source.name, source.samples, source.length);

    AudioClip invertedClip = AudioClip.Create(source.name+"_inverted", source.samples, source.channels, source.frequency, false);

    float[] sourceData = new float[source.samples * source.channels];
    if(source.GetData(sourceData, 0))
    {
      int arrayHalfLength = Mathf.FloorToInt((float)sourceData.Length / (float)source.channels);
      for (int i = 0; i < arrayHalfLength ; i+=source.channels)
      {
        for (int k = 0; k < source.channels; k++)
        {
          float tmp = sourceData[i + k];
          int endIndex = sourceData.Length - source.channels - i + k;
          sourceData[i+k] = sourceData[endIndex];
          sourceData[endIndex] = tmp;
        }
      }
      invertedClip.SetData(sourceData, 0);
    }else
    { 
      Debug.LogWarningFormat("Couldn't read data from clip {0}", source.name);
      return null;
    }

    return invertedClip;
  }
}
