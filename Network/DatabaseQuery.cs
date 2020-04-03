using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.Threading.Tasks;
using System;
using System.Net.Http;

/// <summary>
/// https://stackoverflow.com/questions/38645336/use-nets-own-httpclient-class-on-unity
/// 
/// child class must instantiate instance and setup in dbQuery static var
/// </summary>

abstract public class DatabaseQuery
{
  static protected DatabaseQuery dbQuery;

  static public DatabaseQuery fetch()
  {
    Debug.Assert(dbQuery != null, "context must have created a DatabaseQuery at this point");
    return dbQuery;
  }

  HttpClient http;

  abstract public string getDatabaseUrl();

  public void query(FormUrlEncodedContent form, Action<string> onComplete = null)
  {
    if(http == null) http = new HttpClient();
    
    http.PostAsync(getDatabaseUrl(), form).ContinueWith(delegate (Task<HttpResponseMessage> msg)
    {
      msg.Result.Content.ReadAsStringAsync().ContinueWith(delegate (Task<string> output)
      {
        //Debug.Log(output.Result);
        if (onComplete != null) onComplete(output.Result);
      });
    });
    
  }
}
