using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using UnityEditorInternal;
using System.IO;
using System.Linq;
using System.Reflection;

public class AssetBundle2Unity 
{
	public string outRoot = "Assets/AB2Unity/";

	string[] assetbundleFiles;
	string[] assetbundleFilenames;
	AssetBundle[] assetBundles;
	Dictionary<AssetBundle,  int > assetBundleDict;
	
	public Dictionary<long, Object> guidAssetDict = new Dictionary<long, Object>();
	public Dictionary<Object, long> assetGuidDict = new Dictionary<Object, long>();
	public Dictionary<Object, AssetBundle> asset_ab_Dict = new Dictionary<Object, AssetBundle>();

	public Dictionary<Object, string> new_asset_path_Dict = new Dictionary<Object, string>();
	public Dictionary<Object, Object> new_asset_obj_Dict = new Dictionary<Object, Object>();
	public Dictionary<Object, bool> new_asset_Setting_Dict = new Dictionary<Object, bool>();

	[SerializeField]
	public List<Mesh> 	meshList = new List<Mesh> ();
	
	[SerializeField]
	public List<Texture> 	textureList = new List<Texture> ();
	
	[SerializeField]
	public List<Sprite> 	spriteList = new List<Sprite> ();
	
	[SerializeField]
	public List<AnimationClip> 	animationClipList = new List<AnimationClip> ();
	
	[SerializeField]
	public List<UnityEditor.Animations.AnimatorController> 	animatorControllerList = new List<UnityEditor.Animations.AnimatorController> ();
	
	[SerializeField]
	public List<Material> 	materialList = new List<Material> ();
	
	[SerializeField]
	public List<GameObject> 	gameObjectList = new List<GameObject> ();
	
	[SerializeField]
	public List<Shader> 	shaderList = new List<Shader> ();

	public string GetAssetBundleName(Object obj)
	{
		if (asset_ab_Dict.ContainsKey (obj)) 
		{
			return assetbundleFilenames[assetBundleDict[asset_ab_Dict[obj]]];
		}
		return "DontKnowAssetBundleName";
	}
	
	public string GetAssetBundleName(AssetBundle ab)
	{
		return assetbundleFilenames[assetBundleDict[ab]];
	}


	public void Run(string root)
	{
		try
		{
			EditorUtility.DisplayProgressBar("AssetBundle2Unity", "正在加载 AssetBundle 文件...", 0f);
			LoadAllAssetBundles (root);
			
			EditorUtility.DisplayProgressBar("AssetBundle2Unity", "正在 LoadAllAssets...", 0.2f);
			LoadAllAssets ();
			
//			EditorUtility.DisplayProgressBar("AssetBundle2Unity", "正在 Save2Unity...", 0.2f);
//			Save2Unity ();
//			Save2UnityForGameObject();
		}
		catch(System.Exception e)
		{
			EditorUtility.ClearProgressBar();
			Debug.Log(e.ToString());
		}
		finally
		{
			EditorUtility.ClearProgressBar();
		}
	}

	public void LoadAllAssetBundles(string root)
	{
		
		List<string> ignoreExts = new List<string>(new string[] { ".meta", ".manifest", ".xlsx" });
		List<string> ignoreFiles = new List<string>(new string[] { ".ds_store" });

		assetbundleFiles = Directory.GetFiles(root, "*.*", SearchOption.AllDirectories)
			.Where(s => !ignoreExts.Contains(Path.GetExtension(s).ToLower()) && !ignoreFiles.Contains(Path.GetFileName(s).ToLower())).ToArray();
		
		 assetbundleFilenames = new string[assetbundleFiles.Length];
		for (int i = 0; i < assetbundleFiles.Length; i++)
		{
			assetbundleFilenames[i] = Path.GetFileName(assetbundleFiles[i]);
			assetbundleFilenames[i] = assetbundleFilenames[i].Replace(Path.GetExtension(assetbundleFilenames[i]), "");
		}

		
		float pCount = assetbundleFiles.Length;
		int pIndext = 0;

		assetBundles = new AssetBundle[assetbundleFiles.Length];
		assetBundleDict = new Dictionary<AssetBundle, int> ();
		for (int i = 0; i < assetbundleFiles.Length; i++)
		{
			pIndext ++;
			EditorUtility.DisplayProgressBar("LoadAllAssetBundles", pIndext +"/" + pCount  + "   "+ assetbundleFilenames[i], pIndext / pCount);

			AssetBundle ab = AssetBundle.LoadFromMemory(File.ReadAllBytes(assetbundleFiles[i]));
			assetBundles[i] = ab;
			if(ab != null)
			{
				assetBundleDict.Add(ab, i);
			}
		}
	}
	
	System.Type typeGameObject = typeof(GameObject);
	System.Type typeMesh = typeof(Mesh);
	System.Type typeMaterial = typeof(Material);
	System.Type typeTextre = typeof(Texture2D);
	System.Type typeSprite = typeof(Sprite);
	System.Type typeAnimationClip = typeof(AnimationClip);
	System.Type typeAnimatorController = typeof(UnityEditor.Animations.AnimatorController);
	
	public void LoadAllAssets()
	{
		float pCount = assetBundles.Length;
		int pIndext = 0;
		foreach (AssetBundle ab in assetBundles) 
		{
			pIndext ++;
			if(ab == null) continue;
			string abName = GetAssetBundleName(ab);

			if(pIndext % 30 == 0)
			EditorUtility.DisplayProgressBar("LoadAllAssets", pIndext +"/" + pCount  + "   "+ abName, pIndext / pCount);

#if UNITY_5 || UNITY_5_3_OR_NEWER 
            Object[] objs = ab.LoadAllAssets();
#else
            Object[] objs = ab.LoadAll();
#endif
            foreach (Object obj in objs)
			{
				
				AnalyzeObjectReference(obj, ab);
				AnalyzeObjectComponent(obj, ab);

			}
		}



		System.Type type;

		foreach(var kvp in assetGuidDict)
		{

			type = kvp.Key.GetType();
			if(type == typeGameObject)
			{
				gameObjectList.Add((GameObject)kvp.Key);
			}
			else if(type == typeMesh)
			{
				meshList.Add((Mesh)kvp.Key);
			}
			else if(type == typeMaterial)
			{
				materialList.Add((Material)kvp.Key);
			}
			else if(type == typeTextre)
			{
				textureList.Add((Texture2D)kvp.Key);
			}
			else if(type == typeSprite)
			{
				spriteList.Add((Sprite)kvp.Key);
			}
			else if(type == typeAnimationClip)
			{
				animationClipList.Add((AnimationClip)kvp.Key);
			}
			else if(type == typeAnimatorController)
			{
				animatorControllerList.Add((UnityEditor.Animations.AnimatorController)kvp.Key);
			}
		}


		

		Debug.Log ("GameObject Count:" + gameObjectList.Count);
		Debug.Log ("Mesh Count:" + meshList.Count);
		Debug.Log ("Material Count:" + materialList.Count);
		Debug.Log ("Texture Count:" + textureList.Count);
		Debug.Log ("Sprite Count:" + spriteList.Count);
		Debug.Log ("AnimationClip Count:" + animationClipList.Count);
		Debug.Log ("AnimatorController Count:" + animatorControllerList.Count);
	}

	public void Save2Unity()
	{
		// Shader
		foreach(Shader shader in shaderList)
		{
			Shader newObj = Shader.Find(shader.name);
			if(newObj != null)
				new_asset_obj_Dict.Add(shader, newObj);
		}


		int pIndext = 0;
		float pCount = textureList.Count;
//		// Texture2D
		foreach(Texture2D texture in textureList)
		{
			string path = ExportTexture2D(texture, outRoot + GetAssetBundleName(texture));
			new_asset_path_Dict.Add(texture, path);

			AssetDatabase.ImportAsset(path);
			
			Texture2D newObj = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
			new_asset_obj_Dict.Add(texture, newObj);

			pIndext ++;
			if(pIndext % 30 == 0)
			EditorUtility.DisplayProgressBar("Save2Unity Texture2D", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
		}

		pIndext = 0;
		pCount = spriteList.Count;
		// Sprite
		foreach(Sprite sprite in spriteList)
		{
			string path = new_asset_path_Dict[sprite.texture];

			TextureImporter importer = (TextureImporter) TextureImporter.GetAtPath(path);
			importer.textureType = TextureImporterType.Sprite;
			
			TextureImporterSettings tis = new TextureImporterSettings();  
			importer.ReadTextureSettings(tis);  
			tis.ApplyTextureType(TextureImporterType.Sprite, false);  
			importer.SetTextureSettings(tis);  
			AssetDatabase.ImportAsset (path);
			
			Sprite newObj = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
			if(!new_asset_obj_Dict.ContainsKey(sprite))new_asset_obj_Dict.Add(sprite, newObj);
			if(!new_asset_path_Dict.ContainsKey(newObj))new_asset_path_Dict.Add(newObj, path);
			
			pIndext ++;
			if(pIndext % 30 == 0)
			EditorUtility.DisplayProgressBar("Save2Unity Sprite", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
		}
		
		pIndext = 0;
		pCount = materialList.Count;
		// Material
		foreach(Material material in materialList)
		{
			if(new_asset_obj_Dict.ContainsKey(material))
				continue;

			string path = outRoot + GetAssetBundleName(material) + "/" + material.name.Replace(" (Instance)", "") + ".mat";
			CheckPath(path);
			
			Material newMaterial =(Material) Material.Instantiate(material);
			if(newMaterial.shader != null && new_asset_obj_Dict.ContainsKey(newMaterial.shader))
				newMaterial.shader = (Shader) new_asset_obj_Dict[newMaterial.shader];
			SettingObjectReference(newMaterial);
			AssetDatabase.CreateAsset(newMaterial, path);
			AssetDatabase.ImportAsset (path);
			
			Material newObj = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));
			new_asset_obj_Dict.Add(material, newObj);
			new_asset_path_Dict.Add(material, path);
			
			pIndext ++;
			EditorUtility.DisplayProgressBar("Save2Unity Material", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
		}
		
		pIndext = 0;
		pCount = meshList.Count;
		// Mesh
		for(int i = 0; i < pCount; i ++)
		//foreach(Mesh mesh in meshList)
		{
			Mesh mesh = meshList[i];
			if(mesh == null)
			{
				Debug.Log("Mesh =null" + i);
				continue;
			}
			string path = outRoot + GetAssetBundleName(mesh) + "/" + mesh.name.Replace(" (Instance)", "") + ".asset";
			if(File.Exists(path))
				continue;
			CheckPath(path);
			try
			{
			Mesh newmesh =(Mesh) Mesh.Instantiate(mesh);
			//newmesh.RecalculateNormals();
			AssetDatabase.CreateAsset(newmesh, path);
			AssetDatabase.ImportAsset (path);
			
			Mesh newObj = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
			new_asset_obj_Dict.Add(mesh, newObj);
			new_asset_path_Dict.Add(mesh, path);
			}catch(System.Exception e)
			{
				Debug.LogError(e.ToString());
			}

			
					pIndext ++;
		if(pIndext % 30 == 0)
			EditorUtility.DisplayProgressBar("Save2Unity Mesh", pIndext +"/" + pCount + "   "+ path, pIndext / pCount);
		}
		
		pIndext = 0;
		pCount = animationClipList.Count;
		// AnimationClip
		for(int i = 1000; i < pCount; i ++)
//		foreach (AnimationClip animationClip in animationClipList) 
		{
			AnimationClip animationClip = animationClipList[i];
			if(animationClip == null)
			{
				
				Debug.Log("animationClip =null " + i);
				continue;
			}
			string path = outRoot + GetAssetBundleName(animationClip) + "/" + animationClip.name.Replace("(Clone)", "") + ".anim";
			if(File.Exists(path))
				continue;
			CheckPath(path);
			AnimationClip clip = (AnimationClip) AnimationClip.Instantiate(animationClip);
			AssetDatabase.CreateAsset(clip, path);
			AssetDatabase.ImportAsset (path);
			
			AnimationClip newObj = (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
			new_asset_obj_Dict.Add(animationClip, newObj);
			new_asset_path_Dict.Add(animationClip, path);

			
			pIndext ++;
			if(pIndext % 30 == 0)
			EditorUtility.DisplayProgressBar("Save2Unity AnimationClip", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
		}
		pIndext = 0;
		pCount = animatorControllerList.Count;
		// AnimatorController
		foreach(UnityEditor.Animations.AnimatorController animatorController in animatorControllerList)
		{
			
			string path = outRoot + GetAssetBundleName(animatorController) + "/" + animatorController.name.Replace("(Clone)", "") + ".controller";
			CheckPath(path);
			UnityEditor.Animations.AnimatorController controller = (UnityEditor.Animations.AnimatorController) UnityEditor.Animations.AnimatorController.Instantiate(animatorController);
			
			SettingObjectReference2(controller);
			AssetDatabase.CreateAsset(controller, path);
			AssetDatabase.ImportAsset (path);
			
			UnityEditor.Animations.AnimatorController newObj = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(UnityEditor.Animations.AnimatorController));
			new_asset_obj_Dict.Add(animatorController, newObj);
			new_asset_path_Dict.Add(animatorController, path);

			
			pIndext ++;
			EditorUtility.DisplayProgressBar("Save2Unity AnimatorController", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
		}


		pIndext = 0;
		pCount = gameObjectList.Count;

		// GameObject
		List<GameObject> newGOList = new List<GameObject> ();
		foreach(GameObject gameObject in gameObjectList)
		{
			GameObject go = gameObject;
			go.name = go.name.Replace("(Clone)", "");
			
			string path = outRoot + GetAssetBundleName(gameObject) + "/" + go.name + ".prefab";
			CheckPath(path);
			go = PrefabUtility.CreatePrefab(path, go);
			new_asset_obj_Dict.Add(gameObject, go);
			new_asset_path_Dict.Add(gameObject, path);
			newGOList.Add(go);

			
			
			pIndext ++;
			EditorUtility.DisplayProgressBar("Save2Unity GameObject Prefab", pIndext +"/" + pCount + path, pIndext / pCount);
		}

		
		pIndext = 0;
		pCount = newGOList.Count;
		foreach(GameObject gameObject in newGOList)
		{
			SettingObjectReference(gameObject);
			SettingObjectComponent(gameObject);
			
			
			pIndext ++;
			EditorUtility.DisplayProgressBar("Save2Unity GameObject SettingObjectReference", pIndext +"/" + pCount + gameObject.name, pIndext / pCount);
		}

	}

	
	public void Save2UnityForAnimatorController()
	{
		
		int pIndext = 0;
		float pCount = animationClipList.Count;
		// AnimationClip
		for(int i = 1000; i < pCount; i ++)
			//		foreach (AnimationClip animationClip in animationClipList) 
		{
			AnimationClip animationClip = animationClipList[i];
			if(animationClip == null)
			{
				
				Debug.Log("animationClip =null " + i);
				continue;
			}
			string path = outRoot + GetAssetBundleName(animationClip) + "/" + animationClip.name.Replace("(Clone)", "") + ".anim";
			if(!File.Exists(path))
				continue;
			
			AnimationClip newObj = (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
			new_asset_obj_Dict.Add(animationClip, newObj);
			new_asset_path_Dict.Add(animationClip, path);
		}

		pIndext = 0;
		pCount = animatorControllerList.Count;
		// AnimatorController
		foreach(UnityEditor.Animations.AnimatorController animatorController in animatorControllerList)
		{
			
			string path = outRoot + GetAssetBundleName(animatorController) + "/" + animatorController.name.Replace("(Clone)", "") + ".controller";
			if(File.Exists(path))
				continue;
			CheckPath(path);
			UnityEditor.Animations.AnimatorController controller = (UnityEditor.Animations.AnimatorController) UnityEditor.Animations.AnimatorController.Instantiate(animatorController);
			
			SettingObjectReference2(controller);
			AssetDatabase.CreateAsset(controller, path);
			AssetDatabase.ImportAsset (path);
			
			UnityEditor.Animations.AnimatorController newObj = (UnityEditor.Animations.AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(UnityEditor.Animations.AnimatorController));
			new_asset_obj_Dict.Add(animatorController, newObj);
			new_asset_path_Dict.Add(animatorController, path);
			
			
			pIndext ++;
			EditorUtility.DisplayProgressBar("Save2Unity AnimatorController", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
		}
	}

	
	public void Save2UnityForGameObject()
	{
		
		
		int pIndext = 0;
		float pCount = textureList.Count;
//		
//		foreach(Texture2D texture in textureList)
//		{
//			
//			string path = Path.Combine (outRoot + GetAssetBundleName(texture), texture.name + ".png");
//			if(!File.Exists(path))
//				continue;
//			new_asset_path_Dict.Add(texture, path);
//			Texture2D newObj = (Texture2D)AssetDatabase.LoadAssetAtPath(path, typeof(Texture2D));
//			new_asset_obj_Dict.Add(texture, newObj);
//			
//			pIndext ++;
//			if(pIndext % 30 == 0)
//				EditorUtility.DisplayProgressBar("Save2Unity Texture2D", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
//		}
//		
//		pIndext = 0;
//		pCount = spriteList.Count;
//		// Sprite
//		foreach(Sprite sprite in spriteList)
//		{
//			string path = new_asset_path_Dict[sprite.texture];
//			if(!File.Exists(path))
//				continue;
//			new_asset_path_Dict.Add(sprite, path);
//			
//			Sprite newObj = (Sprite)AssetDatabase.LoadAssetAtPath(path, typeof(Sprite));
//			if(!new_asset_obj_Dict.ContainsKey(sprite))new_asset_obj_Dict.Add(sprite, newObj);
//			if(!new_asset_path_Dict.ContainsKey(newObj))new_asset_path_Dict.Add(newObj, path);
//			
//			pIndext ++;
//			if(pIndext % 30 == 0)
//				EditorUtility.DisplayProgressBar("Save2Unity Sprite", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
//		}
//		
//		pIndext = 0;
//		pCount = materialList.Count;
//		// Material
//		foreach(Material material in materialList)
//		{
//			if(new_asset_obj_Dict.ContainsKey(material))
//				continue;
//			
//			string path = outRoot + GetAssetBundleName(material) + "/" + material.name.Replace(" (Instance)", "") + ".mat";
//			
//			if(!File.Exists(path))
//				continue;
//
//			
//			Material newObj = (Material)AssetDatabase.LoadAssetAtPath(path, typeof(Material));
//			new_asset_obj_Dict.Add(material, newObj);
//			new_asset_path_Dict.Add(material, path);
//			
//			pIndext ++;
//			EditorUtility.DisplayProgressBar("Save2Unity Material", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
//		}
//		
//		pIndext = 0;
//		pCount = meshList.Count;
//		// Mesh
//		for(int i = 0; i < pCount; i ++)
//			//foreach(Mesh mesh in meshList)
//		{
//			Mesh mesh = meshList[i];
//			if(mesh == null)
//			{
//				Debug.Log("Mesh =null" + i);
//				continue;
//			}
//			string path = outRoot + GetAssetBundleName(mesh) + "/" + mesh.name.Replace(" (Instance)", "") + ".asset";
//			if(!File.Exists(path))
//				continue;
//			CheckPath(path);
//			try
//			{
//				
//				Mesh newObj = (Mesh)AssetDatabase.LoadAssetAtPath(path, typeof(Mesh));
//				new_asset_obj_Dict.Add(mesh, newObj);
//				new_asset_path_Dict.Add(mesh, path);
//			}catch(System.Exception e)
//			{
//				Debug.LogError(e.ToString());
//			}
//			
//			
//			pIndext ++;
//			if(pIndext % 30 == 0)
//				EditorUtility.DisplayProgressBar("Save2Unity Mesh", pIndext +"/" + pCount + "   "+ path, pIndext / pCount);
//		}
//		
//		pIndext = 0;
//		pCount = animationClipList.Count;
//		// AnimationClip
//		for(int i = 1000; i < pCount; i ++)
//			//		foreach (AnimationClip animationClip in animationClipList) 
//		{
//			AnimationClip animationClip = animationClipList[i];
//			if(animationClip == null)
//			{
//				
//				Debug.Log("animationClip =null " + i);
//				continue;
//			}
//			string path = outRoot + GetAssetBundleName(animationClip) + "/" + animationClip.name.Replace("(Clone)", "") + ".anim";
//			if(!File.Exists(path))
//				continue;
//			
//			AnimationClip newObj = (AnimationClip)AssetDatabase.LoadAssetAtPath(path, typeof(AnimationClip));
//			new_asset_obj_Dict.Add(animationClip, newObj);
//			new_asset_path_Dict.Add(animationClip, path);
//			
//			
//			pIndext ++;
//			if(pIndext % 30 == 0)
//				EditorUtility.DisplayProgressBar("Save2Unity AnimationClip", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
//		}
//		pIndext = 0;
//		pCount = animatorControllerList.Count;
//		// AnimatorController
//		foreach(AnimatorController animatorController in animatorControllerList)
//		{
//			
//			string path = outRoot + GetAssetBundleName(animatorController) + "/" + animatorController.name.Replace("(Clone)", "") + ".controller";
//			if(!File.Exists(path))
//				continue;
//			
//			AnimatorController newObj = (AnimatorController)AssetDatabase.LoadAssetAtPath(path, typeof(AnimatorController));
//			new_asset_obj_Dict.Add(animatorController, newObj);
//			new_asset_path_Dict.Add(animatorController, path);
//			
//			
//			pIndext ++;
//			EditorUtility.DisplayProgressBar("Save2Unity AnimatorController", pIndext +"/" + pCount  + "   "+ path, pIndext / pCount);
//		}
		
		
		pIndext = 0;
		pCount = gameObjectList.Count;
		
		// GameObject
		List<GameObject> newGOList = new List<GameObject> ();
		for(int i = 6650; i < pCount; i ++)
		{
			GameObject gameObject = gameObjectList[i];
			if(gameObject == null)
			{
				Debug.Log( i + " gameObjcet = null");
				continue;
			}
			GameObject go = gameObject;
			string path = outRoot + GetAssetBundleName(gameObject) + "/" + go.name + ".prefab";
			if(File.Exists(path))
				continue;
			go.name = go.name.Replace("(Clone)", "");
			
			SettingObjectReference(go);
			SettingObjectComponent(go);

			CheckPath(path);
			go = PrefabUtility.CreatePrefab(path, go);
//			new_asset_obj_Dict.Add(gameObject, go);
//			new_asset_path_Dict.Add(gameObject, path);
//			newGOList.Add(go);
			
			
			
			pIndext ++;
			EditorUtility.DisplayProgressBar("Save2Unity GameObject Prefab", pIndext +"/" + pCount + path, pIndext / pCount);
		}
		
		
//		pIndext = 0;
//		pCount = newGOList.Count;
//		foreach(GameObject gameObject in newGOList)
//		{
//			SettingObjectReference(gameObject);
//			SettingObjectComponent(gameObject);
//			
//			pIndext ++;
//			EditorUtility.DisplayProgressBar("Save2Unity GameObject SettingObjectReference", pIndext +"/" + pCount + gameObject.name, pIndext / pCount);
//		}

	}

	public void SaveGameObject(GameObject gameObject, string path)
	{
		GameObject go = gameObject;
		go.name = go.name.Replace("(Clone)", "");
		
		SettingObjectReference(go);
		SettingObjectComponent(go);
		CheckPath(path);
		go = PrefabUtility.CreatePrefab(path, go);

	}

	
	public Object GetUnityAssset(Object abObj)
	{
		string path = GetUnityAsssetPath (abObj);
		if (string.IsNullOrEmpty (path))
			return null;

		if (!File.Exists (path))
			return null;

		return AssetDatabase.LoadAssetAtPath (path, abObj.GetType());

	}

	public string GetUnityAsssetPath(Object abObj)
	{
		System.Type type = abObj.GetType ();

		if(type == typeGameObject)
		{
			return outRoot + GetAssetBundleName(abObj) + abObj.name.Replace("(Clone)", "") + ".prefab";
		}
		else if(type == typeMesh)
		{
			return outRoot + GetAssetBundleName(abObj) + "/" + abObj.name.Replace(" (Instance)", "") + ".asset";
		}
		else if(type == typeMaterial)
		{
			return outRoot + GetAssetBundleName(abObj) + "/" + abObj.name.Replace(" (Instance)", "") + ".mat";
		}
		else if(type == typeTextre)
		{
			return  outRoot + GetAssetBundleName(abObj) + abObj.name + ".png";
		}
		else if(type == typeSprite)
		{
			Sprite sprite = (Sprite)abObj;
			if(sprite != null && sprite.texture != null)
				return  GetUnityAsssetPath(sprite.texture);
		}
		else if(type == typeAnimationClip)
		{
			return outRoot + GetAssetBundleName(abObj) + "/" + abObj.name.Replace("(Clone)", "") + ".anim";
		}
		else if(type == typeAnimatorController)
		{
			return outRoot + GetAssetBundleName(abObj) + "/" + abObj.name.Replace("(Clone)", "") + ".controller";
		}
		return null;
	}
	
	private PropertyInfo inspectorMode;

	/// <summary>
	/// 分析对象的引用
	/// </summary>
	/// <param name="info"></param>
	/// <param name="o"></param>
	public void AnalyzeObjectReference(Object o, AssetBundle ab)
	{
		if (o == null || asset_ab_Dict.ContainsKey(o))
		{
			return;
		}
		asset_ab_Dict.Add (o, ab);
		
		var serializedObject = new SerializedObject(o);

		bool isAsset = IsAsset (o);

		if (isAsset) 
		{
			long guid;
			SerializedProperty pathIdProp = serializedObject.FindProperty ("m_LocalIdentfierInFile");
			if(pathIdProp == null) pathIdProp = serializedObject.FindProperty ("LocalIdentfierInFile");
#if UNITY_5 || UNITY_5_3_OR_NEWER
			guid = pathIdProp.longValue;
#else
			if(pathIdProp != null)
				guid = pathIdProp.intValue;
			else
				guid = o.GetInstanceID();
#endif

			assetGuidDict.Add(o, guid);
			guidAssetDict.Add(guid, o);
		}




		if (inspectorMode == null)
		{
			inspectorMode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
		}
		inspectorMode.SetValue(serializedObject, InspectorMode.Debug, null);
		
		var it = serializedObject.GetIterator();
		while (it.NextVisible(true))
		{
			if (it.propertyType == SerializedPropertyType.ObjectReference && it.objectReferenceValue != null)
			{
				AnalyzeObjectReference(it.objectReferenceValue, ab);
			}
		}
		
		// 只能用另一种方式获取的引用
		AnalyzeObjectReference2(o, ab);
		serializedObject.Dispose ();
	}
	
	/// <summary>
	/// 动画控制器比较特殊，不能通过序列化得到
	/// </summary>
	/// <param name="info"></param>
	/// <param name="o"></param>
	private void AnalyzeObjectReference2(Object o, AssetBundle ab)
	{
		UnityEditor.Animations.AnimatorController ac = o as UnityEditor.Animations.AnimatorController;
		if (ac)
		{
#if UNITY_5 || UNITY_5_3_OR_NEWER
			foreach (var clip in ac.animationClips)
			{
				AnalyzeObjectReference(o, ab);
			}
#else
			List<State> list = new List<State>();
			for (int i = 0; i < ac.layerCount; i++)
			{
				AnimatorControllerLayer layer = ac.GetLayer(i);
				list.AddRange(AnimatorStateMachine_StatesRecursive(layer.stateMachine));
			}
			foreach (State state in list)
			{
				var clip = state.GetMotion() as AnimationClip;
				if (clip)
				{
					AnalyzeObjectReference(clip, ab);
				}
			}
#endif
		}
	}
	
#if !(UNITY_5 || UNITY_5_3_OR_NEWER)
	private static List<State> AnimatorStateMachine_StatesRecursive(StateMachine stateMachine)
	{
		List<State> list = new List<State>();
		for (int i = 0; i < stateMachine.stateCount; i++)
		{
			list.Add(stateMachine.GetState(i));
		}
		for (int i = 0; i < stateMachine.stateMachineCount; i++)
		{
			list.AddRange(AnimatorStateMachine_StatesRecursive(stateMachine.GetStateMachine(i)));
		}
		return list;
	}
#endif
	
	/// <summary>
	/// 分析脚本的引用（这只在脚本在工程里时才有效）
	/// </summary>
	/// <param name="info"></param>
	/// <param name="o"></param>
	public void AnalyzeObjectComponent(Object o, AssetBundle ab)
	{
		var go = o as GameObject;
		if (!go)
		{
			return;
		}
		
		var components = go.GetComponentsInChildren<Component>(true);
		foreach (var component in components)
		{
			if (!component)
			{
				continue;
			}
			
			AnalyzeObjectReference(component, ab);
		}
	}

	public static bool IsAsset(Object o)
	{
		if (!o)
		{
			return false;
		}

		string name2 = o.name;
		string type = o.GetType().ToString();
		if (type.StartsWith("UnityEngine."))
		{
			type = type.Substring(12);
			
			// 如果是内置的组件，就不当作是资源
			if (o as Component)
			{
				return false;
			}
		}
		else if (type == "UnityEditor.Animations.AnimatorController")
		{
		}
		else if (type == "UnityEditorInternal.AnimatorController")
		{
		}
		else if (type == "UnityEditor.MonoScript")
		{
			return false;
		}
		else
		{
			// 外部的组件脚本，走上面的MonoScript
			if (o as Component)
			{
				return false;
			}
			// 外部的序列化对象，已经被脚本给分析完毕了，不需要再添加进来
			if (o as ScriptableObject)
			{
				return false;
			}

		}
		
		// 内建的资源排除掉
		string assetPath = AssetDatabase.GetAssetPath(o);
		if (!string.IsNullOrEmpty(assetPath))
		{
			return false;
		}
		return true;
	}
	
	
	/// <summary>
	/// 分析对象的引用
	/// </summary>
	/// <param name="info"></param>
	/// <param name="o"></param>
	public void SettingObjectReference(Object o)
	{
		if (new_asset_Setting_Dict.ContainsKey (o))
			return;

		new_asset_Setting_Dict.Add (o, true);

		var serializedObject = new SerializedObject(o);
		
		if (inspectorMode == null)
		{
			inspectorMode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
		}
		inspectorMode.SetValue(serializedObject, InspectorMode.Debug, null);

		bool hasSet = false;
		var it = serializedObject.GetIterator();
		while (it.NextVisible(true))
		{
			if (it.propertyType == SerializedPropertyType.ObjectReference && it.objectReferenceValue != null)
			{
				if(new_asset_obj_Dict.ContainsKey(it.objectReferenceValue))
				{
					hasSet = true;
					it.objectReferenceValue = new_asset_obj_Dict[it.objectReferenceValue];
				}
				else
				{
					Object uo = GetUnityAssset(it.objectReferenceValue);
					if(uo != null)
					{
						hasSet = true;
						it.objectReferenceValue = uo;
					}
				}
			}
		}

		if (hasSet)
			serializedObject.ApplyModifiedProperties ();

		SettingObjectReference2 (o);
		serializedObject.Dispose ();
	}

	/// <summary>
	/// 动画控制器比较特殊，不能通过序列化得到
	/// </summary>
	/// <param name="info"></param>
	/// <param name="o"></param>
	private void SettingObjectReference2(UnityEngine.Object o)
	{
		
		UnityEditor.Animations.AnimatorController ac = o as UnityEditor.Animations.AnimatorController;
		if (ac)
		{
#if UNITY_5 || UNITY_5_3_OR_NEWER
			for(int i= 0;i < ac.animationClips.Length; i ++)
			{
				var  clip = ac.animationClips[i];
				if(clip )
				{
					if(new_asset_obj_Dict.ContainsKey(clip))
					{
						ac.animationClips[i] = (AnimationClip) new_asset_obj_Dict[clip];
					}
					else
					{
						Object uo = GetUnityAssset(clip);
						if(uo != null)
						{
							ac.animationClips[i] = (AnimationClip) uo;
						}
					}
				}
			}
#else
			List<State> list = new List<State>();
			for (int i = 0; i < ac.layerCount; i++)
			{
				AnimatorControllerLayer layer = ac.GetLayer(i);
				list.AddRange(AnimatorStateMachine_StatesRecursive(layer.stateMachine));
			}
			foreach (State state in list)
			{
				var clip = state.GetMotion() as AnimationClip;

				if(clip )
				{
					if(new_asset_obj_Dict.ContainsKey(clip))
					{
						state.SetAnimationClip((AnimationClip) new_asset_obj_Dict[clip]);
					}
					
					else
					{
						Object uo = GetUnityAssset(clip);
						if(uo != null)
						{
							state.SetAnimationClip((AnimationClip) uo);
						}
					}
				}
			}
#endif
		}
	}
	
	
	/// <summary>
	/// 分析脚本的引用（这只在脚本在工程里时才有效）
	/// </summary>
	/// <param name="info"></param>
	/// <param name="o"></param>
	public void SettingObjectComponent(Object o)
	{
		var go = o as GameObject;
		if (!go)
		{
			return;
		}
		
		var components = go.GetComponentsInChildren<Component>(true);
		foreach (var component in components)
		{
			if (!component)
			{
				continue;
			}
			
			SettingObjectReference(component);
		}
	}
	
	
	private static string ExportTexture2D(Texture2D tex, string rootPath)
	{
		string path = Path.Combine (rootPath, tex.name + ".png");
		if (File.Exists (path)) 
		{
			return path;
		}

		if (!Directory.Exists(rootPath))
		{
			Directory.CreateDirectory(rootPath);
		}
		
		RenderTexture rt = RenderTexture.GetTemporary(tex.width, tex.height, 0);
		Graphics.Blit(tex, rt);
		
		RenderTexture active = RenderTexture.active;
		RenderTexture.active = rt;
		Texture2D cont = new Texture2D(tex.width, tex.height);
		cont.hideFlags = HideFlags.HideAndDontSave;
		cont.ReadPixels(new Rect(0, 0, tex.width, tex.height), 0, 0);
		cont.Apply();
		RenderTexture.active = active;
		RenderTexture.ReleaseTemporary(rt);
		
		File.WriteAllBytes(path, cont.EncodeToPNG());
		return path;
	}
	
	public static void CheckPath(string path, bool isFile = true)
	{
		if(isFile) path = path.Substring(0, path.LastIndexOf('/'));
		string[] dirs = path.Split('/');
		string target = "";
		
		bool first = true;
		foreach(string dir in dirs)
		{
			if(first)
			{
				first = false;
				target += dir;
				continue;
			}
			
			if(string.IsNullOrEmpty(dir)) continue;
			target +="/"+ dir;
			if(!Directory.Exists(target))
			{
				Directory.CreateDirectory(target);
			}
		}
	}
	
	Vector2 scrollPos;
	public void Draw()
	{
		return;
		scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));

		for (int i = 0; i < gameObjectList.Count; i++) 
		{
			if(gameObjectList[i] != null)
			{
				if (GUILayout.Button(gameObjectList[i].name, GUILayout.ExpandWidth(true)))
				{
					SaveGameObject(gameObjectList[i], "Assets/_Test/" + gameObjectList[i].name + ".prefab");
				}
			}
			else
			{
				GUILayout.Label(i + " null");
			}
		}
		EditorGUILayout.EndScrollView ();
	}

	public void ReplaceMaterialShader()
	{
		StringWriter sw = new StringWriter ();
		int count = materialList.Count;
		for (int i = 0; i < count; i++)
		{
			Material material = materialList[i];
			string path = null;
			if(material == null)
			{
				sw.WriteLine(i.ToString());
				continue;
			}

			if(material.shader == null)
			{
				path = GetUnityAsssetPath(material);
				sw.WriteLine(path );
				continue;
			}
			path = GetUnityAsssetPath(material);
			string shaderName = material.shader.name;
			sw.WriteLine(path + ";" + shaderName);
		}
		File.WriteAllText ("mater_shader.csv", sw.ToString());
		return;

		Dictionary<string, Shader> dict = new Dictionary<string, Shader> ();
		for (int i = 0; i < count; i++)
		{
			Material material = materialList[i];
			if(material == null || material.shader == null)
				continue;

			Material asset = (Material) GetUnityAssset(material);
			if(asset != null)
			{
				string shaderName = material.shader.name;
				Shader shader = null;
				if(dict.ContainsKey(shaderName))
				{
					shader = dict[shaderName];
				}
				else
				{
					shader = Shader.Find(shaderName);
					dict.Add(shaderName, shader);
				}
				
				if(shader != null && asset.shader != shader)
				{
					asset.shader = shader;
				}
			}
		}
	}
}
