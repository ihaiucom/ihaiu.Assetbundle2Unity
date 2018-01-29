using UnityEngine;
using System.Collections;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEditorInternal;

namespace CC.Runtime
{
    public partial class SeeAssetBundleInfoWindow : EditorWindow
    {
		
		[MenuItem("Game/Analyze/移除没用的预设")]
		public static void RemoveEmptyPrefab()
		{
			string[] guids = AssetDatabase.FindAssets ("t:Prefab");
			foreach(string guid in guids)
			{
				string path = AssetDatabase.GUIDToAssetPath(guid);
				GameObject go = (GameObject) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(GameObject));
				
				var componts = go.GetComponentsInChildren<Component> (true);
				if (componts.Length == 2) 
				{
					SpriteRenderer sr = componts[1] as SpriteRenderer;
					if(sr != null && sr.sprite == null)
					{
						AssetDatabase.DeleteAsset(path);
					}
				}
			}
		}

		
		[MenuItem("Game/Analyze/Replace Material Shader")]
		public static void ReplaceMaterialShader()
		{
			string[] guids = AssetDatabase.FindAssets ("t:material");
			Dictionary<string, Shader> dict = new Dictionary<string, Shader> ();
			foreach(string guid in guids)
			{
				Material material = (Material) AssetDatabase.LoadAssetAtPath(AssetDatabase.GUIDToAssetPath(guid), typeof(Material));
				
				string shaderName = material.shader.name;
				Debug.Log(shaderName);
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

				if(shader != null && material.shader != shader)
				{
					material.shader = shader;
				}
			}
		}

		[MenuItem("Game/Analyze/Replace Material Shader CSV")]
		public static void ReplaceMaterialShaderCSV()
		{
			string[] lines = File.ReadAllLines ("mater_shader.csv");
			float count = lines.Length;
			Dictionary<string, Shader> dict = new Dictionary<string, Shader> ();
			for(int i = 0; i < count; i++)
			{
				try
				{
					string[] arr = lines[i].Split(';');
					string path = arr[0];
					string shaderName = arr[1];
					
					Material material = (Material) AssetDatabase.LoadAssetAtPath(path, typeof(Material));
					if(material == null) continue;

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
					
					if(shader != null && material.shader != shader)
					{
						material.shader = shader;
						AssetDatabase.SaveAssets();
					}
					EditorUtility.DisplayProgressBar("ReplaceMaterialShaderCSV", i +"/" + count + path, i / count);
				}
				catch(Exception e)
				{
					Debug.Log(e.ToString());
				}

			}

			EditorUtility.ClearProgressBar ();
		}
		
		[MenuItem("Game/Analyze/Test Mesh")]
		public static void TestAnimatorControl()
		{
//			GameObject go = Selection.activeGameObject;
			
//			Animator[] animations = go.GetComponentsInChildren<Animator> ();
//			foreach (Animator animation in animations) 
//			{
//				Debug.Log(animation.layerCount);
//				for(int i = 0; i < animation.layerCount; i ++)
//				{

//					AnimatorClipInfo[] infos = animation.GetCurrentAnimatorClipInfo (i);

//					foreach(AnimatorClipInfo info in infos)
//					{
//						Debug.Log(info);
//					}
//				}

//				if(animation.runtimeAnimatorController != null)
//				{
//					RuntimeAnimatorController controller = (RuntimeAnimatorController) RuntimeAnimatorController.Instantiate(animation.runtimeAnimatorController);
//					UnityEditor.Animations.AnimatorController ac = controller as UnityEditor.Animations.AnimatorController;
					
//					List<UnityEditor.Animations.AnimatorState> list = new List<UnityEditor.Animations.AnimatorState>();
//					for (int i = 0; i < ac.layerCount; i++)
//					{
//						UnityEditor.Animations.AnimatorControllerLayer layer = ac.GetLayer(i);
//						list.AddRange(AnimatorStateMachine_StatesRecursive(layer.stateMachine));
//					}
//					foreach (UnityEditor.Animations.AnimatorState state in list)
//					{
//						var clip = state.GetMotion() as AnimationClip;
//						Debug.Log(state + " " + clip);
//					}

////					SettingObjectReference2(controller);
////					AssetDatabase.CreateAsset(ac, "Assets/_Test/" + ac.name + ".controller");
//				}
//			}

//			MeshRenderer[] meshRenderers =  go.GetComponentsInChildren<MeshRenderer>();
//			foreach(MeshRenderer meshRenderer in meshRenderers)
//			{
//				Debug.Log("=======" + meshRenderer);
//				SettingObjectReference(meshRenderer);
//			}

			
//			var components = go.GetComponentsInChildren<Component>(true);
//			foreach (var component in components)
//			{
//				if (!component)
//				{
//					continue;
//				}
//				
//				Debug.Log("=======" + component);
//				SettingObjectReference(component);
//			}

//			SettingObjectReference(Selection.activeObject);
//			Material material = (Material)Selection.activeObject;
//			material.shader = Shader.Find ("Particles/Additive");
		}


			
//		[MenuItem("Game/Analyze/Test Anmator")]
//		public static void TestAnmator(Dictionary<string, AssetBundle> assetBundleDict)
//		{
//			string abname = "1892_GuiGuZi_LOD1_show.assetbundle";
//			string path = assetbundleRoot + "\\" + abname;
////			AssetBundle ab = AssetBundle.CreateFromMemoryImmediate(File.ReadAllBytes(assetbundleRoot + "/" + abname));
//			AssetBundle ab = assetBundleDict[path];
//			UnityEngine.Object[] objs = ab.LoadAll ();
//			foreach(UnityEngine.Object item in objs)
//			{
//				Debug.Log (item);
//				if(item.GetType() == typeof(GameObject))
//				{
//					GameObject go = (GameObject) item;
//					GameObject gg = PrefabUtility.CreatePrefab("Assets/_Test/AAA.prefab", go);
//					AssetDatabase.GetAssetPath(gg);
//					GameObject.Instantiate(go);
//					Debug.Log (go);
					
//					var components = go.GetComponentsInChildren<Component>(true);
//					foreach (var component in components)
//					{
//						if (!component)
//						{
//							continue;
//						}
//						Debug.Log(component);
						
//					}
//				}
//			}

////			ab.Unload (true);

//		}

		
		[MenuItem("Game/Analyze/Test Shader")]
		public static void ReplaceShader()
		{
			string shaderNameList = @"Mobile/Particles/Multiply
Particles/Additive
Particles/Alpha Blended
Particles/Alpha Blended Premultiply
Transparent/VertexLit
Unlit/Texture
S_Game_Effects/Addtive_Move
S_Game_Effects/Scroll2TexBend
S_Game_Effects/Scroll2TexBend.add
S_Game_Effects/Scroll2TexBend_0
S_Game_Effects/Scroll2TexBend_Shui
S_Game_Effects/Scroll2TexMask
S_Game_Hero/Hero_Battle
S_Game_Hero/Hero_Battle (Translucent) (Dissolve)
S_Game_Hero/Hero_Show
S_Game_Hero/Hero_Show_BlendVertex
S_Game_Hero/Hero_Show_Dissolve
S_Game_Hero/Hero_Show2
S_Game_Hero/Hero_Show3
S_Game_Particle/Additive
S_Game_Particle/AdditiveColorMask
S_Game_Particle/AlpahBlendColorMask
S_Game_Particle/AlphaAdd_BMWTemp
S_Game_Particle/AlphaBlend
S_Game_Particle/Cap_Blend
S_Game_Particle/Object
S_Game_Particle/Opaque
S_Game_Particle/Opaque (Fade)
S_Game_Scene/Alpha_Blend
S_Game_Scene/Alpha_MultiplyColor
S_Game_Scene/Texture
SGame/GoldenCard_000000
SGame/GoldenCard_000100
Sprites/Default2
TMPro/Mobile/Distance Field Overlay
UI/Default2
UI/ParticleMask
UI/UIStencilClear
Unlit/Texture2";

			string[] shaderNames = shaderNameList.Split('\n');
			foreach(string shaderName in shaderNames)
			{
				Shader shader = Shader.Find (shaderName);
				Debug.Log(shaderName + ",\t" + shader );
			}
		}

		
		/// <summary>
		/// 动画控制器比较特殊，不能通过序列化得到
		/// </summary>
		/// <param name="info"></param>
		/// <param name="o"></param>
		private static void SettingObjectReference2(UnityEngine.Object o)
		{
			
			AnimationClip cx = (AnimationClip) AssetDatabase.LoadAssetAtPath ("Assets/_Test/C/X.anim", typeof(AnimationClip));
			AnimationClip caa = (AnimationClip) AssetDatabase.LoadAssetAtPath ("Assets/_Test/aa.anim", typeof(AnimationClip));

			UnityEditor.Animations.AnimatorController ac = o as UnityEditor.Animations.AnimatorController;
			if (ac)
			{
				#if UNITY_5 || UNITY_5_3_OR_NEWER
				for(int i= 0;i < ac.animationClips.Length; i ++)
				{
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
					if (clip == cx)
					{
						state.SetAnimationClip(caa);
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

		
		private static PropertyInfo inspectorMode;
		public static void SettingObjectReference(UnityEngine.Object o)
		{
			Material r = (Material) AssetDatabase.LoadAssetAtPath ("Assets/_Test/R.mat", typeof(Material));
			Material g = (Material) AssetDatabase.LoadAssetAtPath ("Assets/_Test/G.mat", typeof(Material));
			
			
			Texture2D t1 = (Texture2D) AssetDatabase.LoadAssetAtPath ("Assets/_Test/111.png", typeof(Texture2D));
			Texture2D t2 = (Texture2D) AssetDatabase.LoadAssetAtPath ("Assets/_Test/222.png", typeof(Texture2D));




			var serializedObject = new SerializedObject(o);
			
			if (inspectorMode == null)
			{
				inspectorMode = typeof(SerializedObject).GetProperty("inspectorMode", BindingFlags.NonPublic | BindingFlags.Instance);
			}
			inspectorMode.SetValue(serializedObject, InspectorMode.Debug, null);
			
			var it = serializedObject.GetIterator();
			while (it.NextVisible(true))
			{
				Debug.Log(it.name + "  " + it.propertyType );
				if (it.propertyType == SerializedPropertyType.ObjectReference && it.objectReferenceValue != null)
				{
					Debug.Log("---" + it.name + "  " + it.objectReferenceValue );
					if(it.objectReferenceValue == r)
					{
						it.objectReferenceValue = g;
					}
					else if(it.objectReferenceValue == t1)
					{
						it.objectReferenceValue = t2;
					}
					else
					{
						SettingObjectReference(it.objectReferenceValue);
					}

				}
			}

			serializedObject.ApplyModifiedProperties ();
			
		}

		[SerializeField]
		public AssetBundle2Unity assetBundle2Unity;
			
		[MenuItem("Game/Analyze/Save Mesh")]
        public static void SaveMesh()
        {
            GameObject go = Selection.activeGameObject;
            SkinnedMeshRenderer[] skinnedMeshRenderers = go.GetComponentsInChildren<SkinnedMeshRenderer>();
            foreach(SkinnedMeshRenderer render in skinnedMeshRenderers)
            {
                if (render.sharedMesh == null) continue;
                Mesh mesh = render.sharedMesh;
				if(!File.Exists("Assets/_Test/" + mesh.name + ".asset"))
				{
					Mesh newmesh =(Mesh) Mesh.Instantiate(mesh);
	                AssetDatabase.CreateAsset(newmesh, "Assets/_Test/" + mesh.name + ".asset");
				}

				foreach(Material material in render.materials)
				{
					if(!File.Exists("Assets/_Test/" + material.name.Replace(" (Instance)", "") + ".mat"))
						AssetDatabase.CreateAsset(material, "Assets/_Test/" + material.name.Replace(" (Instance)", "") + ".mat");

					if(material.mainTexture != null)
					{
						Texture2D t = (Texture2D)material.mainTexture;
//						t = (Texture2D) Texture2D.Instantiate(t);
//						Texture2D newTexture2D = ne
//						newTexture2D.SetPixels32(t.GetPixels32());
//						newTexture2D.Apply();
						//						var bytes = newTexture2D.EncodeToPNG();
//						AssetDatabase.CreateAsset(t, "Assets/_Test/" + t.name + "png.asset");
//						File.WriteAllBytes("Assets/_Test/" + material.mainTexture.name + ".png", bytes);

						Shader shader = Shader.Find("Sprites/Default");
						SaveRenderTextureToPNG(t, shader, "Assets/_Test/", t.name.Replace(" (Instance)", "") );
					}
				}
            }


			
			Animation[] animations = go.GetComponentsInChildren<Animation>();
			foreach (Animation animation in animations) 
			{
				foreach (AnimationState state in animation) 
				{
					if(state != null && state.clip != null)
					{
						AnimationClip clip = (AnimationClip) AnimationClip.Instantiate(state.clip);
						if(!File.Exists("Assets/_Test/" + clip.name.Replace("(Clone)", "") + ".anim"))
							AssetDatabase.CreateAsset(clip, "Assets/_Test/" + clip.name.Replace("(Clone)", "") + ".anim");
					}
				}
			}


			
			SpriteRenderer[] spriteRenderers = go.GetComponentsInChildren<SpriteRenderer>();
			foreach (SpriteRenderer spriteRenderer in spriteRenderers) 
			{
				if(spriteRenderer.sprite != null)
				{
					Shader shader = Shader.Find("Sprites/Default");
					Texture2D t = (Texture2D)spriteRenderer.sprite.texture;
					SaveRenderTextureToPNG(t, shader, "Assets/_Test/", t.name.Replace(" (Instance)", "") );
					string path = "Assets/_Test/" + t.name.Replace(" (Instance)", "") + ".png";
					
					ImportAssetOptions options = ImportAssetOptions.Default;
					AssetDatabase.ImportAsset (path);
					TextureImporter importer = (TextureImporter) TextureImporter.GetAtPath(path);
					importer.textureType = TextureImporterType.Sprite;

					TextureImporterSettings tis = new TextureImporterSettings();  
					importer.ReadTextureSettings(tis);  
					tis.ApplyTextureType(TextureImporterType.Sprite, false);  
					importer.SetTextureSettings(tis);  
					AssetDatabase.ImportAsset (path);
				}
			}
        }

		
		public static bool SaveRenderTextureToPNG(Texture inputTex,Shader outputShader, string contents, string pngName)  
		{  
			RenderTexture temp = RenderTexture.GetTemporary(inputTex.width, inputTex.height, 0, RenderTextureFormat.ARGB32);  
			Material mat = new Material(outputShader);  
			Graphics.Blit(inputTex, temp, mat);  
			bool ret = SaveRenderTextureToPNG(temp, contents,pngName);  
			RenderTexture.ReleaseTemporary(temp);  
			return ret;  
			
		}   
		
		//将RenderTexture保存成一张png图片  
		public static bool SaveRenderTextureToPNG(RenderTexture rt,string contents, string pngName)  
		{  
			RenderTexture prev = RenderTexture.active;  
			RenderTexture.active = rt;  
			Texture2D png = new Texture2D(rt.width, rt.height, TextureFormat.ARGB32, false);  
			png.ReadPixels(new Rect(0, 0, rt.width, rt.height), 0, 0);  
			byte[] bytes = png.EncodeToPNG();  
			if (!Directory.Exists(contents))  
				Directory.CreateDirectory(contents);  
			FileStream file = File.Open(contents + "/" + pngName + ".png", FileMode.Create);  
			BinaryWriter writer = new BinaryWriter(file);  
			writer.Write(bytes);  
			file.Close();  
			Texture2D.DestroyImmediate(png);  
			png = null;  
			RenderTexture.active = prev;  
			return true;  
			
		}   

        public static SeeAssetBundleInfoWindow window;
        [MenuItem("Game/Analyze/查看AssetBundle")]

        public static void Open () 
        {
            window = EditorWindow.GetWindow <SeeAssetBundleInfoWindow>("查看AssetBundle");
            window.Show();
        }

		private List<string> ignoreExts = new List<string>(new string[] { ".meta", ".manifest", ".xlsx" });
		private List<string> ignoreFiles = new List<string>(new string[] { ".ds_store" });
		public static string assetbundleRoot = "C:/zengfeng/sdcard/com.tencent.tmgp.sgame/files/Resources/AssetBundle";
        public  string assetbundleMain = "C:/zengfeng/sdcard/com.gamesci.u1.hero.prod/files/patch/CommonAssets/CommonAssets";

        public string[] assetbundleFiles = new string[] { };
        public string[] assetbundleFilenames = new string[] { };
        public Dictionary<string, AssetBundle> assetBundleDict = new Dictionary<string, AssetBundle>(); 

        AssetBundle assetBundle;
        AssetBundle assetBundleMain;
//        AssetBundleManifest assetBundleManifest;
        string[] assetNames;
        UnityEngine.Object[] assetObjcts;
        Type[] assetTypes;
        Type typeGameObject = typeof(GameObject);
        Type typeTexture2D = typeof(Texture2D);
        Type typeSprite = typeof(Sprite);
        int assetNameHeight = 200;
        string assetNameStr = "";

        Vector2 scrollPos;
        Vector2 infoScrollPos;


        void OnGUI ()
        {

            EditorGUILayout.BeginVertical();

            assetbundleMain = EditorGUILayout.TextField("Assetbundle Main:", assetbundleMain, GUILayout.ExpandWidth(true));


            if (GUILayout.Button("加载Main"))
            {
//                assetBundleMain = AssetBundle.LoadFromFile(assetbundleMain);
//                assetBundleManifest = assetBundleMain.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
//                string[] assetBundles = assetBundleManifest.GetAllAssetBundles();
//                Debug.Log(String.Join("\n", assetBundles));
            }

            GUILayout.Space(20);


            assetbundleRoot = EditorGUILayout.TextField("Assetbundle Root:", assetbundleRoot, GUILayout.ExpandWidth(true));


            if (GUILayout.Button("生成AssetBundle列表"))
            {
                assetbundleFiles = Directory.GetFiles(assetbundleRoot, "*.*", SearchOption.AllDirectories)
                .Where(s => !ignoreExts.Contains(Path.GetExtension(s).ToLower()) && !ignoreFiles.Contains(Path.GetFileName(s).ToLower())).ToArray();

                assetbundleFilenames = new string[assetbundleFiles.Length];
                for (int i = 0; i < assetbundleFiles.Length; i++)
                {
                    assetbundleFilenames[i] = Path.GetFileName(assetbundleFiles[i]);
                }
            }


            if (GUILayout.Button("加载所有AssetBundle列表"))
            {
                assetBundleDict.Clear();
                for (int i = 0; i < assetbundleFiles.Length; i++)
                {
					assetBundleDict[assetbundleFiles[i]] = AssetBundle.LoadFromMemory(File.ReadAllBytes(assetbundleFiles[i]));
                }
            }

			
			
			if (GUILayout.Button("AssetBundle转Unity"))
			{
				assetBundle2Unity = new AssetBundle2Unity();
				assetBundle2Unity.Run(assetbundleRoot);
			}

			
			if (GUILayout.Button("替换Shader"))
			{
				assetBundle2Unity.ReplaceMaterialShader();
			}

			
			if (GUILayout.Button("保存预设"))
			{
				assetBundle2Unity.Save2UnityForGameObject();
			}

			if (assetBundle2Unity != null) 
			{
				assetBundle2Unity.Draw();
				return;
			}

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.ExpandWidth(true));
            for (int i = 0; i < assetbundleFiles.Length; i++)
            {
                if (GUILayout.Button(assetbundleFilenames[i], GUILayout.ExpandWidth(true)))
                {
                    //if(assetBundle != null)
                    //{
                    //    assetBundle.Unload(false);
                    //}

                    if(assetBundleDict.ContainsKey(assetbundleFiles[i]))
                        assetBundle = assetBundleDict[assetbundleFiles[i]];
                    else
					{

						assetBundle = AssetBundle.LoadFromMemory(File.ReadAllBytes(assetbundleFiles[i]));
//						assetBundle = AssetBundle.CreateFromFile(assetbundleFiles[i])
					}

					Debug.Log(assetBundle);
                    //                    string[] assetNames = assetBundle.GetAllAssetNames();

#if UNITY_5 || UNITY_5_3_OR_NEWER
                    assetObjcts = assetBundle.LoadAllAssets();
#else
                    assetObjcts = assetBundle.LoadAll();
#endif
                    assetTypes = new Type[assetObjcts.Length];

                    //assetNameHeight = Mathf.Max(200, assetNames.Length * 20 + 40);

                    //assetNameStr = "";
                    //for (int j = 0; j < assetNames.Length; j++)
                    //{
                    //    assetNameStr += i + "  " + assetNames[j] + "\n";
                    //}


					GameObject go = new GameObject(Path.GetFileName(assetbundleFiles[i]));
                    for (int j = 0; j < assetObjcts.Length; j++)
                    {
                        assetTypes[j] = assetObjcts[j].GetType();
                        
                        if(assetTypes[j] == typeGameObject)
                        {
							GameObject gameObject = (GameObject)GameObject.Instantiate((GameObject)assetObjcts[j]);
                            gameObject.transform.SetParent(go.transform);
                        }
                    }

                }
            }
            EditorGUILayout.EndScrollView();

            GUILayout.Space(20);


            infoScrollPos = EditorGUILayout.BeginScrollView(infoScrollPos, GUILayout.ExpandWidth(true), GUILayout.MinHeight(500));

            EditorGUILayout.LabelField("AssetNames:");
            //assetNameStr = GUILayout.TextArea(assetNameStr, GUILayout.Height(assetNameHeight));

            //GUILayout.Space(20);

            if (assetObjcts != null && assetTypes != null )
            {

                for (int i = 0; i < assetObjcts.Length; i++)
                {

                    EditorGUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField(assetObjcts[i].ToString(), GUILayout.Width(500));
                    if(assetTypes[i] == typeGameObject)
                    {

                    }
                    EditorGUILayout.EndHorizontal();
                }
            }

            EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical ();


        }
    }

}
