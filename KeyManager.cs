// using UnityEngine;
// using System.Collections.Generic;
// using System;

// public class Key{
  
//   public KeyCode kcode = KeyCode.Space;
//   public int val = 0;
//   public int valLimit = 1; // bool is 0,1
//   public Action<int> onChange;
//   public string info = "";

//   public Key(KeyCode code, int valIncLimit = 1, string info = "") {
//     kcode = code;
//     valLimit = valIncLimit;
//     this.info = info;
//     val = PlayerPrefs.GetInt("key_" + kcode.ToString(), 0);
//   }

//   public void subscribeCallback(Action<int> cb, bool throwCallbackOnSubscription) {
//     onChange += cb;
//     if(throwCallbackOnSubscription) {
//       onChange(val);
//     }
//   }

//   public void changeValue(int newVal) {
//     val = newVal;
//     PlayerPrefs.SetInt("key_" + kcode.ToString(), val);
//     DebugInfoDisplay.callDebug("{Keys} switched " + info + " to " + val, true);
//   }

//   public void update() {
//     if (Input.GetKeyUp(kcode))
//     {
//       val++;
//       if (val > valLimit) val = 0;

//       changeValue(val);
      
//       onChange(val);
//     }
//   }
// }

// public class KeyManager {

//   static public List<Key> keys;

//   static public Key getKeyByKeyCode(KeyCode code) {
//     for (int i = 0; i < keys.Count; i++)
//     {
//       if (keys[i].kcode == code) return keys[i];
//     }
//     return null;
//   }

//   static public int getKeyIndex(KeyCode code) {
//     for (int i = 0; i < keys.Count; i++)
//     {
//       if (keys[i].kcode == code) return i;
//     }
//     return -1;
//   }
//   static public void update() {
//     for (int i = 0; i < keys.Count; i++)
//     {
//       keys[i].update();
//     }
//   }

//   static public void subscribeKey(KeyCode code, string info = "", Action<int> cb = null, bool throwCallbackOnSubscription = true) { subscribeKey(code, 0, info, cb, throwCallbackOnSubscription); }
//   static public void subscribeKey(KeyCode code, int limit = 1, string info = "", Action<int> cb = null, bool throwCallbackOnSubscription = true) {
//     if (keys == null) keys = new List<Key>();

//     int idx = getKeyIndex(code);
//     Key k = null;
//     if(idx < 0) {
//       k = new Key(code, limit, info);
//       keys.Add(k);
//     }
//     else {
//       k = keys[idx];
//     }

//     if(cb != null) k.subscribeCallback(cb, throwCallbackOnSubscription);
//   }
// }
