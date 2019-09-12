using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Reflection;

// delayed text selection auto de tout le texte
// sauvegarde si on sort du texte

public class GUIEx
{
	public class TextFieldOnCompleteInfo
	{
		public string startString = "";
		public string currentString;
	}

	public static void DrawLine(Rect rect, Texture tex)
	{
		int controlID = GUIUtility.GetControlID (FocusType.Passive);
		GUI.Box(rect, tex);

		Mesh m = new Mesh();
		m.Clear();
		Vector3[] vertices = {new Vector3(0,0), new Vector3(200,200)};
		int[] indices = {0,1};
		m.vertices = vertices;
		m.SetIndices(indices, MeshTopology.Lines, 0);
		Graphics.DrawMeshNow(m, GUI.matrix);
	}

	// creates a text field that will return usr modification only when input has ended
	public static string DelayedTextField(Rect rect, string text)
	{
		int controlID = GUIUtility.GetControlID (FocusType.Keyboard);

		// Get (or create) the state object
		var state = (TextFieldOnCompleteInfo)GUIUtility.GetStateObject(
																					typeof(TextFieldOnCompleteInfo), 
																					controlID);

		if(state.startString == "") 
		{
			state.startString 	= text;
			state.currentString = text;
		}

		bool textValidated = false;
		if (Event.current.GetTypeForControl(controlID) == EventType.KeyDown) 
		{
			if(	Event.current.keyCode == KeyCode.KeypadEnter || 
					Event.current.keyCode == KeyCode.Return || 
					Event.current.keyCode == KeyCode.Tab
				)
			{
				textValidated = true;
			}
		}

		state.currentString = GUI.TextField(rect, state.currentString);

		if(textValidated)
		{
			state.startString = state.currentString;
			GUI.FocusControl(null);
		}

		return state.startString;
	}


}


// data for GUI Windows. we store some information to simplify usage, espacially not having to create a rect for the window
public class WindowData
{
	public int id;
	public Rect rect;
	public object data;
}

public class FilePickerData
{
	public DirectoryInfo currentDirectory = null;
	public string[] extensionsFilter;
	public string pickedUpFile = null;
	public bool opened = false;
	public Vector2 scrollPosition;
}

public class GUILayoutEx
{
	// storage for created windows. A window is then identified by its name
	static Dictionary<string, WindowData> windowDataList = new Dictionary<string, WindowData>();
	static int nextWindowID = 0;

	// for File Picker
	public static FilePickerData filePickerData = new FilePickerData();

	public static void Window(string name, GUI.WindowFunction func)
	{
    Window(name, func, new Vector2(20,20));
	}

  // screenPos : topleft corner
  public static void Window(string name, GUI.WindowFunction func, Vector2 screenPos, object winData = null)
	{
		if(!windowDataList.ContainsKey(name))
		{
			WindowData data = new WindowData();
			data.id = nextWindowID;
			data.rect = new Rect(screenPos.x, screenPos.y, 200, 50);
      data.data = winData;
			windowDataList[name] = data;	
			nextWindowID++;
		}
		windowDataList[name].rect = GUILayout.Window(windowDataList[name].id, windowDataList[name].rect, func, name);
	}

	public static WindowData GetDataForWindow(string name)
	{
		return windowDataList[name];
	}

	public static WindowData GetDataForWindow(int id)
	{
		foreach (var item in windowDataList)
		{
			if(item.Value.id == id) return item.Value;
		}
		return null;
	}

  public static string GetWindowNameFromID(int id)
	{
		foreach (var item in windowDataList)
		{
			if(item.Value.id == id) return item.Key;
		}
		return null;
	}



	public static string DelayedTextField(string text, params GUILayoutOption[] options)
	{
		GUIContent gUIContent = new GUIContent(text);
		Rect rect = GUILayoutUtility.GetRect(gUIContent, GUI.skin.textField, options);
		return GUIEx.DelayedTextField(rect, text);
	}

	// creates a Label + a delayed text field to display a variabla data
	public static object VarField(string label, object data, string tooltip = "")
	{
		if(!data.GetType().IsPrimitive) 
		{
			Debug.LogWarning("Can't make a var fiald for label " + label + " because associated data is not of a primitive type");
			return data;
		}
		GUILayout.BeginHorizontal();
		GUILayout.Label(new GUIContent(label, tooltip), GUILayout.ExpandHeight(false), GUILayout.ExpandWidth(true));
		string editedData = GUILayoutEx.DelayedTextField(data.ToString(), GUILayout.MinWidth(50));
		if(editedData != data.ToString())
		{
			data = TypeHelper.GetValueFromString(editedData, data.GetType());
		}
		GUILayout.EndHorizontal();
		return data;
	}




	public static void StructInspector<T>(ref T inspectorData)
	{
		// reflection set value make a copy of a struct, even if this one is passed by ref
		// so we copy the struct into an object that can be set as reference
		object boxedData = inspectorData;

		GUI.contentColor = Color.yellow;
		GUILayout.Label(inspectorData.GetType().Name);

		GUI.contentColor = Color.white;
		FieldInfo[] fields = inspectorData.GetType().GetFields();
		foreach (var field in fields)
		{
			GUILayout.BeginHorizontal();
			GUILayout.Space(10);

			object[] attr = field.GetCustomAttributes(false);
			TooltipAttribute tooltip = (TooltipAttribute)System.Array.Find(attr, (x) => x.GetType() == typeof(TooltipAttribute));
			
			object editedData = VarField(field.Name, field.GetValue(inspectorData), tooltip == null ? "" : tooltip.tooltip);
			field.SetValue(boxedData, editedData);

			GUILayout.EndHorizontal();
		}
		inspectorData = (T)boxedData;
	}


	public static void StructInspectorWindow<T>(T inspectorData)
	{
		GUILayoutEx.Window("Inspector-" + inspectorData.GetType(), DoStructInspector);
		WindowData data = GetDataForWindow("Inspector-" + inspectorData.GetType());
		data.data = inspectorData;
	}

	static void DoStructInspector(int windowID)
	{
		WindowData windata = GetDataForWindow(windowID);
		FieldInfo[] fields = windata.data.GetType().GetFields();

		foreach (var field in fields)
		{
			object[] attr = field.GetCustomAttributes(false);
			TooltipAttribute tooltip = (TooltipAttribute)System.Array.Find(attr, (x) => x.GetType() == typeof(TooltipAttribute));
			VarField(field.Name, field.GetValue(windata.data), tooltip == null ? "" : tooltip.tooltip);
		}

		GUI.DragWindow(new Rect(0, 0, 10000, 20));
	}


	public static void OpenFilePicker(string path, params string[] extensions)
	{
		filePickerData.extensionsFilter = extensions;
		filePickerData.pickedUpFile 		= null;
		filePickerData.currentDirectory = null;
		filePickerData.opened						= true;

		DirectoryInfo info = Directory.GetParent(path);
		filePickerData.currentDirectory = System.Array.Find(info.GetDirectories(), (x) => path.Contains(x.Name));
	}

	public static bool FilePicker()
	{
		GUILayoutEx.Window("File Picker", DoFilePicker);
		GUI.FocusWindow(windowDataList["File Picker"].id);
		return filePickerData.opened;
	}

	public static string GetLastFilePickerResult()
	{
		return filePickerData.pickedUpFile;
	}

	static void DoFilePicker(int windowID)
	{
		DirectoryInfo[] dirs 	= filePickerData.currentDirectory.GetDirectories();
		FileInfo[] files 			= filePickerData.currentDirectory.GetFiles();

		GUI.backgroundColor = Color.black;
		GUILayout.Label(filePickerData.currentDirectory.Name);


		DirectoryInfo info = filePickerData.currentDirectory.Parent;
		GUI.backgroundColor = Color.grey;

		filePickerData.scrollPosition = GUILayout.BeginScrollView(filePickerData.scrollPosition, GUILayout.MinHeight(200), GUILayout.ExpandWidth(true));

		if(info != null && GUILayout.Button(".. (parent folder)", GUILayout.ExpandWidth(true)))
		{
			filePickerData.currentDirectory = info;
			return;
		}

		foreach (var item in dirs)
		{
			if(GUILayout.Button(item.Name, GUILayout.ExpandWidth(true)))
			{
				filePickerData.currentDirectory = item;
				return;
			}
		}
		GUI.backgroundColor = Color.green;
		foreach (var item in files)
		{
			// filter out files with wrong extensions
			if(	filePickerData.extensionsFilter.Length == 0 || 
					System.Array.Exists(filePickerData.extensionsFilter, (x) => x == item.Extension )
				)
			{
				if(GUILayout.Button(item.Name, GUILayout.ExpandWidth(true)))
				{
					filePickerData.pickedUpFile = item.FullName;
					filePickerData.opened = false;
					return;
				}
			}
		}

		GUILayout.EndScrollView();

		GUI.DragWindow(new Rect(0, 0, 10000, 20));
	}


}


public class TypeHelper
{

	public static object GetValueFromString( string strData, System.Type type )
	{
			// Test for Nullable<T> and return the base type instead:
			//System.Type undertype = Nullable.GetUnderlyingType(_t);
			//System.Type basetype = undertype == null ? _t : undertype;
			return System.Convert.ChangeType(strData, type);
	}
}


// manages calls to OnGUI for certain types of GUI elements, for instance
// generic windows that can't be called from other like file picker
public class GUIExManager : MonoBehaviour
{
	public static GUISkin skin;

	[RuntimeInitializeOnLoadMethod]
	static void Init()
	{
		GameObject o = new GameObject("GUIExManager");
		o.AddComponent<GUIExManager>();
		skin = Resources.Load("olr-runtime-gui-skin") as GUISkin;
	}

	void OnGUI()
	{
		GUI.skin = skin;
		if(GUILayoutEx.filePickerData.opened)
		{
			GUILayoutEx.FilePicker();
			// close with escape
			if(Event.current.isKey && Event.current.keyCode == KeyCode.Escape)
			{
				GUILayoutEx.filePickerData.opened = false;
			}
		}
	}
}
