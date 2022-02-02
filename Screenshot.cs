using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[ExecuteInEditMode]
public class Screenshot : EditorWindow
{
	int ssWidth = 2400; 
	int ssHeight = 1600;
	int ssScale = 1;

	bool setBGTransparent = false;

	string ssPath = "";

	public Camera renderCam;

	TextureFormat ssTexFormat;

	public string ScreenShotName(int width, int height)
	{
		string sceneName = SceneManager.GetActiveScene().name;
		string ssCompletePath = string.Format("{0}/" + sceneName + "_SS_{1}x{2}_{3}.png",
							ssPath,
							width, height,
							System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
		return ssCompletePath;
	}

	[MenuItem("Tools/Screenshot Tool")]
	public static void ShowWindow()
	{
		EditorWindow ssWindow = GetWindow(typeof(Screenshot));
		ssWindow.autoRepaintOnSceneChange = true;
		ssWindow.titleContent = new GUIContent("Screenshot Tool");
		ssWindow.maxSize = new Vector2(268f, 386f);
		ssWindow.minSize = ssWindow.maxSize;
		ssWindow.Show();
	}

	void TakeScreenshot()
    {
		int resWidthN = ssWidth * ssScale;
		int resHeightN = ssHeight * ssScale;
		RenderTexture renderTex = new RenderTexture(resWidthN, resHeightN, 24);
		renderCam.targetTexture = renderTex;

		if (setBGTransparent)
		{
			ssTexFormat = TextureFormat.ARGB32;
		}
		else
		{
			ssTexFormat = TextureFormat.RGB24;
		}

		Texture2D ssTex2D = new Texture2D(resWidthN, resHeightN, ssTexFormat, false);

		renderCam.Render();

		RenderTexture.active = renderTex;

		ssTex2D.ReadPixels(new Rect(0, 0, resWidthN, resHeightN), 0, 0);

		renderCam.targetTexture = null;
		RenderTexture.active = null;

		byte[] ssBytes = ssTex2D.EncodeToPNG();
		string ssFilename = ScreenShotName(resWidthN, resHeightN);

		System.IO.File.WriteAllBytes(ssFilename, ssBytes);
		Application.OpenURL(ssFilename);
	}

	void OnGUI()
	{
		EditorGUILayout.LabelField ("Resolution", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();
		if (GUILayout.Button("Default Res"))
		{
			ssWidth = 2400;
			ssHeight = 1600;
			ssScale = 1;
		}
		if (GUILayout.Button("Window Res"))
		{
			ssHeight = (int)Handles.GetMainGameViewSize().y;
			ssWidth = (int)Handles.GetMainGameViewSize().x;

		}
		EditorGUILayout.EndHorizontal();

		GUILayout.Label("Select Render Camera", EditorStyles.boldLabel);

		renderCam = EditorGUILayout.ObjectField(renderCam, typeof(Camera), true, null) as Camera;

		if (renderCam == null)
		{
			renderCam = Camera.main;
		}

		EditorGUILayout.HelpBox("Choose an appropriate width and height.\nThe image will be cropped.", MessageType.Warning);
		ssWidth = EditorGUILayout.IntField ("Width: ", ssWidth);
		ssHeight = EditorGUILayout.IntField ("Height: ", ssHeight);

		EditorGUILayout.Space();

		EditorGUILayout.HelpBox("Multiply res without quality loss (max 4x).", MessageType.Info);
		ssScale = EditorGUILayout.IntSlider ("Scale: ", ssScale, 1, 4);

		EditorGUILayout.Space();

		GUILayout.Label("Background", EditorStyles.boldLabel);
		setBGTransparent = EditorGUILayout.Toggle("Transparency Support?", setBGTransparent);

		EditorGUILayout.Space();
		
		GUILayout.Label ("Screenshot File Path:", EditorStyles.boldLabel);

		EditorGUILayout.BeginHorizontal();

		EditorGUILayout.TextField(ssPath,GUILayout.ExpandWidth(false));
		if(ssPath == null || ssPath == "")
        {
			ssPath = Application.dataPath;
		}
		if (GUILayout.Button("Browse", GUILayout.ExpandWidth(false)))
		{
			ssPath = EditorUtility.SaveFolderPanel("Path to Save Images", ssPath, Application.dataPath);
		}

		EditorGUILayout.EndHorizontal();

		if (GUILayout.Button("Open Folder...", GUILayout.MaxWidth(100), GUILayout.MinHeight(20)))
		{
			Application.OpenURL("file://" + ssPath);
		}

		EditorGUILayout.Space();

		if(GUILayout.Button("Take Screenshot!",GUILayout.MinHeight(40)))
		{
			TakeScreenshot();
		}
	}
}
