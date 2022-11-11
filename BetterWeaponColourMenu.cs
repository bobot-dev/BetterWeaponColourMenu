using HarmonyLib;
using System.Collections.Generic;
using System.Reflection;
using UMM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BetterWeaponColourMenu
{
	[HarmonyPatch]
    [UKPlugin("Better Weapon Colour Menu", "1.0.1", "A simple mod that improves the custom weapon colour system", true, true)]
    public class BetterWeaponColourMenu : UKMod
    {

		public const string GUID = "bot.betterweaponcolourmenu";

		private static Harmony harmony;

		public static AssetBundle assetBundle;

		public override void OnModLoaded()
        {

			assetBundle = AssetBundle.LoadFromMemory(Properties.Resources.colourmenu);

			SaveData.LoadData();

			harmony = new Harmony(GUID);
            harmony.PatchAll();          
        }

		private void Start()
		{
			SceneManager.sceneLoaded += SceneManagerOnsceneLoaded;
		}

		public override void OnModUnload()
        {
			SceneManager.sceneLoaded -= SceneManagerOnsceneLoaded;
			harmony.UnpatchSelf();
			SaveData.SaveDataToFile();
			assetBundle.Unload(true);
			base.OnModUnload();
        }


		private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode mode)
		{
			swapped = false;
			CreateSkinGUI();
		}


		//public static int currentPresetCollectionIndex = 0;


		string[] windowNames = new string[] { "Revolver", "Shotgun", "Nailgun", "Railcannon", "RocketLauncher" };

		public void CreateSkinGUI()
		{
			foreach (ShopGearChecker shopGearChecker in Resources.FindObjectsOfTypeAll<ShopGearChecker>())
			{
				foreach(var name in windowNames)
                {
					var pannel = shopGearChecker.gameObject.transform.Find($"{name}Window");

					if (pannel == null)
                    {
						Debug.LogError($"Could not find {name}Window!!!!");
						continue;
                    }

					var colourScreen = pannel.Find("Color Screen");

					if (colourScreen == null)
					{
						Debug.LogError($"Could not find {name}Window Color Screen!!!!");
						continue;
					}

					DoModificationsToPage(colourScreen.Find("Standard").gameObject, name, false);
					DoModificationsToPage(colourScreen.Find("Alternate").gameObject, name, true);

					LoadCustomColourPreset(colourScreen.Find("Standard").gameObject);
					LoadCustomColourPreset(colourScreen.Find("Alternate").gameObject);

					DoModificationsFromTonysCode(colourScreen.Find("Standard").gameObject, name, false);
					DoModificationsFromTonysCode(colourScreen.Find("Alternate").gameObject, name, true);
			
					typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler").GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

				}
			}
		}

		public void DoModificationsFromTonysCode(GameObject gameObject, string type, bool alt)
		{
			GunColorSetter[] Colors = gameObject.GetComponentsInChildren<GunColorSetter>(true);
			foreach (GunColorSetter GCS in Colors)
			{
				GameObject buttonBase = GCS.transform.parent.parent.Find("TemplateButton").gameObject;
				GameObject varButton = Instantiate(buttonBase, GCS.transform);
				varButton.name = "VarCol";
				Destroy(varButton.GetComponent<Button>());
				varButton.transform.localPosition = new Vector3(-35, -70, 0);
				varButton.transform.localScale = new Vector3(0.4f, 0.5f, 0.4f);
				ShopButton SB = varButton.GetComponent<ShopButton>();
				GameObject Activator = Instantiate(new GameObject(), varButton.transform);
				Activator.SetActive(false);
				SB.toActivate = new GameObject[] { Activator };
				SB.toDeactivate = new GameObject[0];
				var SVB = varButton.AddComponent<CopyVarientColourButton>();
				SVB.triggerMessage = false;
				SVB.activator = Activator;
				SVB.colorNum = GCS.colorNumber;
				SVB.weaponNum = GCS.transform.parent.parent.parent.GetComponent<GunColorTypeGetter>().weaponNumber;
				SVB.alt = alt;
				SVB.GunColSetter = GCS;
				varButton.GetComponentInChildren<Text>().text = "USE VAR COLOR";
				varButton.GetComponentInChildren<Text>().transform.localPosition = new Vector3(55, -20, 0);
				varButton.GetComponent<RectTransform>().sizeDelta = new Vector2(200, 30);

				GameObject CheckBox = Instantiate(buttonBase, varButton.transform);
				Destroy(CheckBox.GetComponent<ShopButton>());
				Destroy(CheckBox.GetComponentInChildren<Text>().transform.gameObject);
				CheckBox.transform.localScale = new Vector3(1, 1, 1);
				CheckBox.transform.localPosition = new Vector3(165, -2, 0);
				CheckBox.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
				SVB.CheckBox = CheckBox;
			}
		}

		public void DoModificationsToPage(GameObject gameObject, string type, bool alt)
        {

			if (gameObject.GetComponent<GunColorTypeGetter>() == null) return;

			var gunColorTypeGetter = gameObject.GetComponent<GunColorTypeGetter>();

			var sliders = gameObject.transform.Find("Custom").Find("Unlocked");

			
			foreach (Transform child in sliders.transform)
            {
				Transform copyButton = Instantiate(sliders.parent.parent.parent.Find("Done"), child);
				copyButton.gameObject.name = "Copy";
				copyButton.localPosition = new Vector3(150, -30, -0.0004f);
				copyButton.localScale = new Vector3(0.4f, 0.5f, 0.4f);
				copyButton.GetComponentInChildren<Text>().text = "COPY COLOR";

				GameObject cAGO = Instantiate(new GameObject(), copyButton.transform);
				cAGO.SetActive(false);

				var copyButtonController = copyButton.gameObject.AddComponent<CopyButton>();
				copyButtonController.activator = cAGO;
				copyButtonController.messagePopupText = "Copied Color!";

				
				copyButton.GetComponent<ShopButton>().toActivate = new GameObject[] { cAGO };
				copyButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];


				Transform pasteButton = Instantiate(sliders.parent.parent.parent.Find("Done"), child);
				pasteButton.gameObject.name = "Paste";
				pasteButton.localPosition = new Vector3(150, -50, -0.0004f);
				pasteButton.localScale = new Vector3(0.4f, 0.5f, 0.4f);
				pasteButton.GetComponentInChildren<Text>().text = "PASTE COLOR";


				GameObject pAGO = Instantiate(new GameObject(), pasteButton.transform);
				pAGO.SetActive(false);

				var pasteButtonController = pasteButton.gameObject.AddComponent<PasteButton>();
				pasteButtonController.activator = pAGO;
				pasteButtonController.messagePopupText = "Pasted Color!";


				pasteButton.GetComponent<ShopButton>().toActivate = new GameObject[] { pAGO };
				pasteButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];


				Transform randomButton = Instantiate(sliders.parent.parent.parent.Find("Done"), child);
				randomButton.gameObject.name = "Random";
				randomButton.localPosition = new Vector3(150, -70, -0.0004f);
				randomButton.localScale = new Vector3(0.4f, 0.5f, 0.4f);
				randomButton.GetComponentInChildren<Text>().text = "RANDOMISE COLOR";


				GameObject rAGO = Instantiate(new GameObject(), randomButton.transform);
				rAGO.SetActive(false);

				var randomButtonController = randomButton.gameObject.AddComponent<RandomButton>();
				randomButtonController.activator = rAGO;
				randomButtonController.triggerMessage = false;


				randomButton.GetComponent<ShopButton>().toActivate = new GameObject[] { rAGO };
				randomButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];
			}

			var templates = gameObject.transform.Find("Template");

			Transform nextPresetRButton = Instantiate(sliders.parent.parent.parent.Find("Done"), templates);

			nextPresetRButton.gameObject.name = "NextPresetRight";
			nextPresetRButton.localPosition = new Vector3(5, -185.0001f, -14.9917f);
			nextPresetRButton.localScale = new Vector3(0.2f, 1f, 1f);
			nextPresetRButton.GetComponentInChildren<Text>().text = ">";
			nextPresetRButton.GetComponentInChildren<Text>().transform.localScale = new Vector3(5f, 1f, 1f);

			GameObject nRAGO = Instantiate(new GameObject(), nextPresetRButton.transform);
			nRAGO.SetActive(false);

			var nextPresetRButtonController = nextPresetRButton.gameObject.AddComponent<NextPresetCollectionButton>();
			nextPresetRButtonController.activator = nRAGO;
			nextPresetRButtonController.triggerMessage = false;
			nextPresetRButtonController.side = NextPresetCollectionButton.Side.Right;
			nextPresetRButtonController.weaponType = gunColorTypeGetter.weaponNumber;
			nextPresetRButtonController.alt = alt;

			nextPresetRButton.GetComponent<ShopButton>().toActivate = new GameObject[] { nRAGO };
			nextPresetRButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];


			Transform nextPresetLButton = Instantiate(sliders.parent.parent.parent.Find("Done"), templates);

			nextPresetLButton.gameObject.name = "NextPresetLeft";
			nextPresetLButton.localPosition = new Vector3(-55, -185.0001f, -14.9917f);
			nextPresetLButton.localScale = new Vector3(0.2f, 1f, 1f);
			nextPresetLButton.GetComponentInChildren<Text>().text = "<";
			nextPresetLButton.GetComponentInChildren<Text>().transform.localScale = new Vector3(5f, 1f, 1f);


			GameObject nLAGO = Instantiate(new GameObject(), nextPresetLButton.transform);
			nLAGO.SetActive(false);

			var nextPresetLButtonController = nextPresetLButton.gameObject.AddComponent<NextPresetCollectionButton>();
			nextPresetLButtonController.activator = nLAGO;
			nextPresetLButtonController.triggerMessage = false;
			nextPresetLButtonController.side = NextPresetCollectionButton.Side.Left;
			nextPresetLButtonController.weaponType = gunColorTypeGetter.weaponNumber;
			nextPresetLButtonController.alt = alt;


			nextPresetLButton.GetComponent<ShopButton>().toActivate = new GameObject[] { nLAGO };
			nextPresetLButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];


			for (int i = 2; i <= 5; i++)
            {
				var template = templates.Find($"Template {i}");

				//template.transform.GetChild(0).localScale = new Vector3(0.75f, 1, 1);
				//template.transform.GetChild(0).GetChild(0).localScale = new Vector3(1.5f, 1, 1);

				Transform saveButton = Instantiate(sliders.parent.parent.parent.Find("Done"), template);

				saveButton.gameObject.SetActive(false);
				saveButton.gameObject.name = "Save";
				saveButton.localPosition = new Vector3(225, -40, -0.0005f);
				saveButton.localScale = new Vector3(0.2f, 1f, 1f);
				saveButton.GetComponentInChildren<Text>().text = "SAVE";
				saveButton.GetComponentInChildren<Text>().transform.localScale = new Vector3(5f, 1f, 1f);

				GameObject sAGO = Instantiate(new GameObject(), saveButton.transform);
				sAGO.SetActive(false);

				var saveButtonController = saveButton.gameObject.AddComponent<SaveButton>();
				saveButtonController.activator = sAGO;
				saveButtonController.messagePopupText = "Preset Saved!";
				saveButtonController.index = i - 1;
				saveButtonController.text = template.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>();
				saveButtonController.weaponType = gunColorTypeGetter.weaponNumber;
				saveButtonController.alt = alt;


				saveButton.GetComponent<ShopButton>().toActivate = new GameObject[] { sAGO };
				saveButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];

				Transform clearButton = Instantiate(sliders.parent.parent.parent.Find("Done"), template);

				clearButton.gameObject.SetActive(false);
				clearButton.gameObject.name = "Clear";
				clearButton.localPosition = new Vector3(285, -40, -0.0005f);
				clearButton.localScale = new Vector3(0.2f, 1f, 1f);
				clearButton.GetComponentInChildren<Text>().text = "RESET";
				clearButton.GetComponentInChildren<Text>().transform.localScale = new Vector3(5f, 1f, 1f);

				GameObject cAGO = Instantiate(new GameObject(), clearButton.transform);
				cAGO.SetActive(false);

				var clearButtonController = clearButton.gameObject.AddComponent<ClearButton>();
				clearButtonController.activator = cAGO;
				clearButtonController.messagePopupText = "Cleared!";
				clearButtonController.index = i - 1;
				clearButtonController.text = template.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>();
				clearButtonController.weaponType = gunColorTypeGetter.weaponNumber;
				clearButtonController.alt = alt;


				clearButton.GetComponent<ShopButton>().toActivate = new GameObject[] { cAGO };
				clearButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];
			}

			sliders.parent.parent.parent.Find("Done").localScale = new Vector3(0.75f, 1f, 1f);

			Transform importButton = Instantiate(sliders.parent.parent.parent.Find("Done"), sliders);
			importButton.gameObject.name = "Import";
			importButton.localPosition = new Vector3(20.0116f, 129f, -0.0004f);
			importButton.localScale = new Vector3(0.4f, 0.5f, 0.4f);
			importButton.GetComponentInChildren<Text>().text = "IMPORT PRESET";

			GameObject iAGO = Instantiate(new GameObject(), importButton.transform);
			iAGO.SetActive(false);

			var importButtonController = importButton.gameObject.AddComponent<ImportButton>();
			importButtonController.activator = iAGO;
			importButtonController.messagePopupText = "Imported!";
			importButtonController.weaponType = type;
			importButtonController.alt = alt;


			importButton.GetComponent<ShopButton>().toActivate = new GameObject[] { iAGO };
			importButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];


			//Transform toggleCustomEffectColoursButton = Instantiate(sliders.parent.parent.parent.Find("Done"), sliders);
			Transform toggleCustomEffectColoursButton = Instantiate(sliders.parent.parent.parent.Find("Done"), gameObject.transform);
			toggleCustomEffectColoursButton.gameObject.name = "ToggleCustomEffect";
			toggleCustomEffectColoursButton.localPosition = new Vector3(20.0116f, 110f, -0.0004f);
			toggleCustomEffectColoursButton.localScale = new Vector3(0.4f, 0.5f, 0.4f);
			toggleCustomEffectColoursButton.GetComponentInChildren<Text>().text = "TOGGLE EFFECT COLOR";

			GameObject tAGO = Instantiate(new GameObject(), toggleCustomEffectColoursButton.transform);
			tAGO.SetActive(false);

			var toggleCustomEffectColoursController = toggleCustomEffectColoursButton.gameObject.AddComponent<ToggleEffectButton>();
			toggleCustomEffectColoursController.activator = tAGO;
			toggleCustomEffectColoursController.triggerMessage = false;
			toggleCustomEffectColoursController.weaponNum = gameObject.GetComponent<GunColorTypeGetter>().weaponNumber;
			toggleCustomEffectColoursController.alt = alt;
			//toggleCustomEffectColoursController.isPressed = false;


			toggleCustomEffectColoursButton.GetComponent<ShopButton>().toActivate = new GameObject[] { tAGO };
			toggleCustomEffectColoursButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];

			GameObject buttonBase = gameObject.transform.Find("Custom").Find("TemplateButton").gameObject;
			GameObject CheckBox = Instantiate(buttonBase, toggleCustomEffectColoursButton.transform);
			Destroy(CheckBox.GetComponent<ShopButton>());
			Destroy(CheckBox.GetComponentInChildren<Text>().transform.gameObject);
			CheckBox.transform.localScale = new Vector3(1, 1, 1);
			CheckBox.transform.localPosition = new Vector3(7, 33, 0);
			CheckBox.GetComponent<RectTransform>().sizeDelta = new Vector2(25, 25);
			toggleCustomEffectColoursController.CheckBox = CheckBox;

		}

		public void LoadCustomColourPreset(GameObject gameObject)
        {

			if (gameObject.GetComponent<GunColorTypeGetter>() == null) return;

			var gunColorTypeGetter = gameObject.GetComponent<GunColorTypeGetter>();

			for (int i = 1; i < 5; i++)
			{
				var template = gameObject.transform.Find("Template").Find($"Template {i + 1}");

				Color[] colours = new Color[3];


				if (weaponNameFromNumber.ContainsKey(gunColorTypeGetter.weaponNumber))
				{
					var weaponType = weaponNameFromNumber[gunColorTypeGetter.weaponNumber];
					if (UKMod.PersistentModDataExists($"customPresetOverride.{weaponType}.{i}.isCustom", GUID))
                    {
						RemovePersistentModData($"customPresetOverride.{weaponType}.{i}.isCustom", GUID);
						RemovePersistentModData($"customPresetOverride.0.{weaponType}.{i}.isCustom", GUID);
					}

					for (int t = 1; t <= 4; t++)
					{
						for (int c = 0; c < 3; c++)
						{
							if (UKMod.PersistentModDataExists($"customPresetOverride.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}", GUID))
							{
								string value = UKMod.RetrieveStringPersistentModData($"customPresetOverride.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}", GUID);
								//UKMod.SetPersistentModData($"customPresetOverride.0.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}", value, GUID);
								RemovePersistentModData($"customPresetOverride.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}", GUID);
							}						
						}


						if (!bool.Parse(SaveData.RetriveSaveValue($"HasUpdatedCustomPreset", "NONE", "false")))
						{
							//Debug.Log("pre data transfer");
							for (int p = 0; p < 24; p++)
							{
								float[] coloursValues = new float[3];
								for (int c = 0; c < 3; c++)
								{
									if (UKMod.PersistentModDataExists($"customPresetOverride.{p}.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}", GUID))
									{
										string value = UKMod.RetrieveStringPersistentModData($"customPresetOverride.{p}.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}", GUID);

										float.TryParse(value, out coloursValues[c]);
										RemovePersistentModData($"customPresetOverride.{p}.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}", GUID);
										//Debug.LogError(coloursValues[c]);
									}
								}

								SaveData.SetSaveValue($"customPreset.{p}.{t}.{i}", $"{gunColorTypeGetter.weaponNumber}", ColorUtility.ToHtmlStringRGB(new Color(coloursValues[0], coloursValues[1], coloursValues[2])), true);
							}
							SaveData.SaveDataToFile();
							SaveData.SetSaveValue($"HasUpdatedCustomPreset", "NONE", true);
							typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler").GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
						}
					}
				}
				
				var currentPresetColI = int.Parse(SaveData.RetriveSaveValue("currentPresetCollectionIndex", $"{gunColorTypeGetter.weaponNumber}{(gunColorTypeGetter.altVersion ? ".a" : "")}", "-1"));

				if (currentPresetColI > -1) ColorUtility.TryParseHtmlString("#" + SaveData.RetriveSaveValue($"customPreset.{currentPresetColI}.{1}.{i}", $"{gunColorTypeGetter.weaponNumber}", "FFFFFF"), out colours[0]);
				if (currentPresetColI > -1) ColorUtility.TryParseHtmlString("#" + SaveData.RetriveSaveValue($"customPreset.{currentPresetColI}.{2}.{i}", $"{gunColorTypeGetter.weaponNumber}", "FFFFFF"), out colours[1]);
				if (currentPresetColI > -1) ColorUtility.TryParseHtmlString("#" + SaveData.RetriveSaveValue($"customPreset.{currentPresetColI}.{3}.{i}", $"{gunColorTypeGetter.weaponNumber}", "FFFFFF"), out colours[2]);



				GunColorPreset newPreset = new GunColorPreset(colours[0], colours[1], colours[2]);

				if (currentPresetColI >= 0)
                {
					template.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = $"Custom {(4 * currentPresetColI) + i}";

					switch (gunColorTypeGetter.weaponNumber)
					{
						//case "Revolver":
						case 1:
							MonoSingleton<GunColorController>.Instance.revolverColors[i] = newPreset;
							break;
						//case "Shotgun":
						case 2:
							MonoSingleton<GunColorController>.Instance.shotgunColors[i] = newPreset;
							break;
						//case "Nailgun":
						case 3:
							MonoSingleton<GunColorController>.Instance.nailgunColors[i] = newPreset;
							break;
						//case "Railcannon":
						case 4:
							MonoSingleton<GunColorController>.Instance.railcannonColors[i] = newPreset;
							break;
						//case "RocketLauncher":
						case 5:
							MonoSingleton<GunColorController>.Instance.rocketLauncherColors[i] = newPreset;
							break;
					}
				} 				
			}
		}

		/*public static Color[] GetColoursForPreset(string weaponType, int index, int collectionIndex)
        {
			Color[] colours = new Color[3];

			for (int t = 1; t <= 3; t++)
			{
				float[] rgbColourValues = new float[3];

				for (int c = 0; c < 3; c++)
				{
					if (UKMod.PersistentModDataExists($"customPresetOverride.{collectionIndex}.{weaponType}.{index}.{t}.{rgbValueFromInt[c]}", GUID))
					{
						rgbColourValues[c] = UKMod.RetrieveFloatPersistentModData(GUID, $"customPresetOverride.{collectionIndex}.{weaponType}.{index}.{t}.{rgbValueFromInt[c]}");
					}
					else
					{
						rgbColourValues[c] = 0;

						Debug.LogWarning($"could not find data \"{$"customPresetOverride.{collectionIndex}.{weaponType}.{index}.{t}.{rgbValueFromInt[c]}"}\"!!!!");
					}

				}

				colours[t - 1] = new Color(rgbColourValues[0], rgbColourValues[1], rgbColourValues[2]);
			}

			return colours;
		}*/

		public static GunColorPreset GetPreset(int weaponType, int index, int collectionIndex)
		{
			Color[] colours = new Color[3];

			/*for (int t = 1; t <= 3; t++)
			{
				float[] rgbColourValues = new float[3];

				for (int c = 0; c < 3; c++)
				{
					if (UKMod.PersistentModDataExists($"customPresetOverride.{collectionIndex}.{weaponType}.{index}.{t}.{rgbValueFromInt[c]}", GUID))
					{
						rgbColourValues[c] = UKMod.RetrieveFloatPersistentModData(GUID, $"customPresetOverride.{collectionIndex}.{weaponType}.{index}.{t}.{rgbValueFromInt[c]}");
					}
					else
					{
						rgbColourValues[c] = 0;

						//Debug.LogWarning($"could not find data \"{$"customPresetOverride.{currentPresetCollectionIndex}.{weaponType}.{index}.{t}.{rgbValueFromInt[c]}"}\"!!!!");
					}

				}

				
				//colours[t - 1] = new Color(rgbColourValues[0], rgbColourValues[1], rgbColourValues[2]);
			}*/
			Debug.Log(SaveData.RetriveSaveValue($"customPreset.{collectionIndex}.{1}.{index}", $"{weaponType}", "SHIT"));
			if (collectionIndex > -1) ColorUtility.TryParseHtmlString("#" + SaveData.RetriveSaveValue($"customPreset.{collectionIndex}.{1}.{index}", $"{weaponType}", "FFFFFF"), out colours[0]);
			if (collectionIndex > -1) ColorUtility.TryParseHtmlString("#" + SaveData.RetriveSaveValue($"customPreset.{collectionIndex}.{2}.{index}", $"{weaponType}", "FFFFFF"), out colours[1]);
			if (collectionIndex > -1) ColorUtility.TryParseHtmlString("#" + SaveData.RetriveSaveValue($"customPreset.{collectionIndex}.{3}.{index}", $"{weaponType}", "FFFFFF"), out colours[2]);

			

			return new GunColorPreset(colours[0], colours[1], colours[2]);
		}
		[HarmonyPatch(typeof(RevolverBeam), nameof(RevolverBeam.Start))]
		[HarmonyPostfix]
		public static void RevolverBeamColourChange(RevolverBeam __instance)
        {

			if (__instance.beamType != BeamType.Enemy && __instance.beamType != BeamType.MaliciousFace && __instance.lr && __instance.ultraRicocheter && __instance.gameObject.name != "ReflectedBeamPoint(Clone)")
			{
				if (!bool.Parse(SaveData.RetriveSaveValue("useColourEffects", $"{(__instance.beamType == BeamType.Revolver ? 1 : 4)}{(__instance.strongAlt ? ".a" : "")}", "false"))) return;

				float h = 0;
				float s = 0;
				float v = 0;

				Color.RGBToHSV(ColorBlindSettings.Instance.variationColors[__instance.gunVariation], out h, out s, out v);

				var colour = Color.HSVToRGB(h, s / 4, v * 1.25f);

				foreach (var lr in __instance.GetComponentsInChildren<LineRenderer>())
				{
					lr.endColor = ModifyColour(lr.endColor, h);
					lr.startColor = ModifyColour(lr.startColor, h);
				}

				foreach (var r in __instance.GetComponentsInChildren<SpriteRenderer>())
				{
					r.material = new Material(r.material);
					r.material.shader = assetBundle.LoadAsset<Shader>("assets/hueshift.shader");
					r.material.SetColor("_HueShift", colour);
				}
			}
		}

		[HarmonyPatch(typeof(Revolver), nameof(Revolver.Start))]
		[HarmonyPostfix]
		public static void RevolverChargeColourChange(Revolver __instance)
		{
			if (!bool.Parse(SaveData.RetriveSaveValue("useColourEffects", $"1{ (__instance.altVersion ? ".a" : "")}", "false"))) return;

			if (__instance.chargeEffect && __instance.GetComponentInParent<WeaponIcon>())
            {
				foreach (var r in __instance.chargeEffect.GetComponents<Renderer>())
				{
					r.material = new Material(r.material);
					r.material.shader = assetBundle.LoadAsset<Shader>("assets/hueshift.shader");
					r.material.SetColor("_HueShift", ColorBlindSettings.Instance.variationColors[__instance.gunVariation]);
				}			
			}
			if (__instance.revolverBeamSuper?.GetComponent<RevolverBeam>()?.hitParticle)
            {
				var r = __instance.revolverBeamSuper?.GetComponent<RevolverBeam>()?.hitParticle.GetComponent<ParticleSystemRenderer>();
				r.material = new Material(r.material);
				r.material.shader = assetBundle.LoadAsset<Shader>("assets/hueshift.shader");
				r.material.SetColor("_HueShift", ColorBlindSettings.Instance.variationColors[__instance.gunVariation]);
			}
		}

		[HarmonyPatch(typeof(Nail), nameof(Nail.Start))]
		[HarmonyPostfix]
		public static void NailTrailColourChange(Nail __instance)
		{
			if (__instance.GetComponent<TrailRenderer>() && __instance.sourceWeapon?.GetComponent<Nailgun>() && !__instance.enemy && __instance.fodderDamageBoost)
			{
				if (!bool.Parse(SaveData.RetriveSaveValue("useColourEffects", $"3{(__instance.sawblade ? ".a" : "")}", "false"))) return;

				__instance.GetComponent<TrailRenderer>().startColor = ColorBlindSettings.Instance.variationColors[__instance.sourceWeapon.GetComponent<Nailgun>().CorrectVariation()];
			}
		}

		/*[HarmonyPatch(typeof(Coin), nameof(Coin.Start))]
		[HarmonyPostfix]
		public static void CoinTrailColourChange(Coin __instance)
		{
			if (__instance.GetComponent<TrailRenderer>() && __instance.sourceWeapon?.GetComponent<Revolver>() && !__instance.wasShotByEnemy)
			{
				//var key = $"customPresetOverride.W{1}{(__instance.sourceWeapon.GetComponent<Revolver>().altVersion ? ".a" : "")}.useColouredEffects";
				//if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "false", BetterWeaponColourMenu.GUID);
				//if (!UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, key)) return;

				if (!SaveData.RetriveSaveValue("useColourEffects", $"1{(__instance.sourceWeapon.GetComponent<Revolver>().altVersion ? ".a" : "")}")) return;

				__instance.GetComponent<TrailRenderer>().startColor = ColorBlindSettings.Instance.variationColors[__instance.sourceWeapon.GetComponent<Revolver>().gunVariation];
			}
		}*/

		[HarmonyPatch(typeof(Magnet), nameof(Magnet.Start))]
		[HarmonyPostfix]
		public static void CoinTrailColourChange(Magnet __instance)
		{
			if (__instance.GetComponentInParent<TrailRenderer>())
			{
				if (!bool.Parse(SaveData.RetriveSaveValue("useColourEffects", "3", "false"))) return;

				__instance.GetComponentInParent<TrailRenderer>().startColor = ColorBlindSettings.Instance.variationColors[0];
			}
		}

		[HarmonyPatch(typeof(Harpoon), nameof(Harpoon.Start))]
		[HarmonyPostfix]
		public static void ScrewTrailColourChange(Harpoon __instance)
		{
			if (__instance.GetComponent<TrailRenderer>() && __instance.drill)
			{
				if (!bool.Parse(SaveData.RetriveSaveValue("useColourEffects", $"4", "false"))) return;

				__instance.GetComponent<TrailRenderer>().startColor = ColorBlindSettings.Instance.variationColors[1];
			}
		}

		static Color ModifyColour(Color colour, float hue)
        {
			float h = 0;
			float s = 0;
			float v = 0;

			Color.RGBToHSV(colour, out h, out s, out v);

			return Color.HSVToRGB(hue, s, v);
		}


		[HarmonyPatch(typeof(GunColorGetter), "UpdateColor")]
		[HarmonyPostfix]
		public static void WeaponSkinColorVariationPostFix(GunColorGetter __instance)
		{
			WeaponIcon wepico = __instance.GetComponentInParent<WeaponIcon>();

			if (__instance.gameObject.GetComponent<SkinnedMeshRenderer>() == null || __instance.gameObject.GetComponent<SkinnedMeshRenderer>().materials == null) return;
			
			Material[] materials = __instance.gameObject.GetComponent<SkinnedMeshRenderer>().materials;
			foreach (Material mat in materials)
			{
				if (!__instance.transform.gameObject.GetComponent<VariationColorHandler>())
				{
					__instance.transform.gameObject.AddComponent<VariationColorHandler>();
				}
				if (wepico && __instance.transform.gameObject.GetComponent<VariationColorHandler>())// && weaponNameFromNumber.ContainsKey(__instance.transform.gameObject.GetComponent<VariationColorHandler>().weaponnum))
				{
					Debug.Log(__instance.transform.gameObject.GetComponent<VariationColorHandler>().weaponnum);
					VariationColorHandler VCH = __instance.transform.gameObject.GetComponent<VariationColorHandler>();
					VCH.weaponnum = __instance.weaponNumber;
					VCH.alt = __instance.altVersion;

					var currentPresetColI = int.Parse(SaveData.RetriveSaveValue("currentPresetCollectionIndex", $"{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", "-1"));

					var presetNum = MonoSingleton<PrefsManager>.Instance.GetInt($"gunColorPreset.{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", 0);

					//for (int i = 1; i <= 3; i++)
					//{
					//if (!UKMod.PersistentModDataExists($"customPresetOverride.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{i}", BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData($"customPresetOverride.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{i}", "false", BetterWeaponColourMenu.GUID);
					//if (!UKMod.PersistentModDataExists($"customPresetOverride.{currentPresetColI}.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{i}.{presetNum}", BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{i}.{presetNum}", "false", BetterWeaponColourMenu.GUID);
					//}

					if (MonoSingleton<PrefsManager>.Instance.GetBool($"gunColorType.{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", false))
					{
						//VCH.swapColor1 = UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, $"customPresetOverride.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{1}");
						//VCH.swapColor2 = UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, $"customPresetOverride.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{2}");
						//VCH.swapColor3 = UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, $"customPresetOverride.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{3}");

						VCH.swapColor1 = bool.Parse(SaveData.RetriveSaveValue($"useVariationColour.{1}", $"{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", "false"));
						VCH.swapColor2 = bool.Parse(SaveData.RetriveSaveValue($"useVariationColour.{2}", $"{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", "false"));
						VCH.swapColor3 = bool.Parse(SaveData.RetriveSaveValue($"useVariationColour.{3}", $"{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", "false"));
					}
					else if (currentPresetColI >= 0)
					{
						//VCH.swapColor1 = UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, $"customPresetOverride.{currentPresetColI}.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{1}.{presetNum}");
						//VCH.swapColor2 = UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, $"customPresetOverride.{currentPresetColI}.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{2}.{presetNum}");
						//VCH.swapColor3 = UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, $"customPresetOverride.{currentPresetColI}.W{VCH.weaponnum}{(VCH.alt ? ".a" : "")}.{3}.{presetNum}");

						VCH.swapColor1 = bool.Parse(SaveData.RetriveSaveValue($"useVariationColour.{currentPresetColI}.{1}", $"{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", "false"));
						VCH.swapColor2 = bool.Parse(SaveData.RetriveSaveValue($"useVariationColour.{currentPresetColI}.{2}", $"{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", "false"));
						VCH.swapColor3 = bool.Parse(SaveData.RetriveSaveValue($"useVariationColour.{currentPresetColI}.{3}", $"{VCH.weaponnum}{(VCH.alt ? ".a" : "")}", "false"));
					}



					if (mat.HasProperty("_CustomColor1"))
					{
						if (VCH.swapColor1)
							mat.SetColor("_CustomColor1", ColorBlindSettings.Instance.variationColors[wepico.variationColor]);
						if (VCH.swapColor2)
							mat.SetColor("_CustomColor2", ColorBlindSettings.Instance.variationColors[wepico.variationColor]);
						if (VCH.swapColor3)
							mat.SetColor("_CustomColor3", ColorBlindSettings.Instance.variationColors[wepico.variationColor]);
					}
				}
			}
		}

		public static bool swapped;

		public static Dictionary<int, string> rgbValueFromInt = new Dictionary<int, string>()
		{
			{ 0, "r" },
			{ 1, "g" },
			{ 2, "b" },
		};

		public static Dictionary<string, int> weaponNumberFromName = new Dictionary<string, int>()
		{
			{ "Revolver", 1 },
			{ "Shotgun", 2 },
			{ "Nailgun", 3 },
			{ "Railcannon", 4 },
			{ "RocketLauncher", 5 },
		};

		public static Dictionary<int, string> weaponNameFromNumber = new Dictionary<int, string>()
		{
			{ 1, "Revolver" },
			{ 2, "Shotgun" },
			{ 3, "Nailgun" },
			{ 4, "Railcannon" },
			{ 5, "RocketLauncher" },
		};


		public static Dictionary<int, GunColorPreset[]> baseWeaponPresetColours = new Dictionary<int, GunColorPreset[]>()
		{
			{ 1, new GunColorPreset[] { new GunColorPreset(new Color(0, 0, 0), new Color(0, 0, 0), new Color(0.42f, 0.19f, 0)), new GunColorPreset(new Color(0, 0.6f, 1), new Color(0.4f, 0.5f, 0.54f), new Color(0.4f, 0.5f, 0.54f)), new GunColorPreset(new Color(1, 0.24f, 0), new Color(0.25f, 0.25f, 0.25f), new Color(0.25f, 0.25f, 0.25f)), new GunColorPreset(new Color(0.5f, 0f, 0f), new Color(0.25f, 0f, 0f), new Color(0.25f, 0f, 0f)) } },
			{ 2, new GunColorPreset[] { new GunColorPreset(new Color(0f, 0f, 0f), new Color(0f, 0f, 0f), new Color(0f, 0f, 0f)), new GunColorPreset(new Color(0.6f, 0.6f, 0.6f), new Color(1f, 0.66f, 0f), new Color(1f, 1f, 1f)), new GunColorPreset(new Color(1f, 0.8f, 0.6f), new Color(0.6f, 0.6f, 0.6f), new Color(1f, 0.8f, 0.6f)), new GunColorPreset(new Color(0f, 0f, 0f), new Color(1f, 0.65f, 0f), new Color(0f, 0f, 0f)), } },
			{ 3, new GunColorPreset[] { new GunColorPreset(new Color(0.6f, 0.6f, 0.6f), new Color(0.6f, 0.6f, 0.6f), new Color(0.33f, 1f, 0f)), new GunColorPreset(new Color(0.12f, 0.48f, 1f), new Color(0.12f, 0.48f, 1f), new Color(0f, 0f, 0f)), new GunColorPreset(new Color(1f, 1f, 1f), new Color(0f, 0f, 0f), new Color(0f, 0f, 0f)), new GunColorPreset(new Color(0f, 0f, 0f), new Color(0.5f, 0f, 0f), new Color(0.5f, 0f, 0f)), } },
			{ 4, new GunColorPreset[] { new GunColorPreset(new Color(0.88f, 0.88f, 0.88f), new Color(0.25f, 0.45f, 0.7f), new Color(0f, 0.35f, 0.7f)), new GunColorPreset(new Color(0.8f, 0.4f, 0.7f), new Color(0.6f, 0.6f, 0.6f), new Color(0f, 0f, 0f)), new GunColorPreset(new Color(0.6f, 0.6f, 0.6f), new Color(0.6f, 0.6f, 0.6f), new Color(1f, 0f, 0f)), new GunColorPreset(new Color(0.88f, 0.66f, 0f), new Color(0f, 0f, 0f), new Color(0f, 0f, 0f)), } },
			{ 5, new GunColorPreset[] { new GunColorPreset(new Color(0.5f, 0.25f, 0.25f), new Color(1f, 0.85f, 0.6f), new Color(0.65f, 0.65f, 0.65f)), new GunColorPreset(new Color(0.66f, 0.66f, 0.66f), new Color(1f, 0f, 0.42f), new Color(0.44f, 0.44f, 0.44f)), new GunColorPreset(new Color(0.7f, 1f, 0f), new Color(0.6f, 0f, 0.4f), new Color(0.64f, 0f, 0.48f)), new GunColorPreset(new Color(0f, 0f, 0f), new Color(0.32f, 0.2f, 0.6f), new Color(0f, 0f, 0f)), } },
		};

		public static Dictionary<int, string[]> baseWeaponPresetNames = new Dictionary<int, string[]>()
		{
			{ 1, new string[] { "Magnum", "Icebreaker", "Hot & Ready", "Sanguine" } },
			{ 2, new string[] { "Classic", "Palace", "Caramel", "Luxus" } },
			{ 3, new string[] { "Acidic", "Clear Sky", "Snow Leopard", "Vampire" } },
			{ 4, new string[] { "Inverse", "Love & Liquorice", "Statue Vein", "Industrial" } },
			{ 5, new string[] { "Rustic", "Lipstick", "Eggplant", "Night Amethyst" } },
		};

	}
}
