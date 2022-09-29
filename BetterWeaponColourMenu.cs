using HarmonyLib;
using System.Collections.Generic;
using UMM;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace BetterWeaponColourMenu
{
    [UKPlugin("Better Weapon Colour Menu", "1.0.0", "IDFK WHAT TO PUT HERE :)", true, true)]
    public class BetterWeaponColourMenu : UKMod
    {

		public const string GUID = "bot.betterweaponcolourmenu";

		private static Harmony harmony;
        public override void OnModLoaded()
        {
            //Debug.Log("Starting custom arms");
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
            base.OnModUnload();
        }


		private void SceneManagerOnsceneLoaded(Scene scene, LoadSceneMode mode)
		{
			swapped = false;
			CreateSkinGUI();
		}


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

					LoadCustomColourPreset(colourScreen.Find("Standard").gameObject, name, false);
					LoadCustomColourPreset(colourScreen.Find("Alternate").gameObject, name, true);

				}
			}
		}

		public void DoModificationsToPage(GameObject gameObject, string type, bool alt)
        {

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

			for(int i = 2; i <= 5; i++)
            {
				var template = templates.Find($"Template {i}");

				template.transform.GetChild(0).localScale = new Vector3(0.75f, 1, 1);
				template.transform.GetChild(0).GetChild(0).localScale = new Vector3(1.5f, 1, 1);

				Transform saveButton = Instantiate(sliders.parent.parent.parent.Find("Done"), template);

				saveButton.gameObject.name = "Save";
				saveButton.localPosition = new Vector3(225, -40, -0.0005f);
				saveButton.localScale = new Vector3(0.2f, 1f, 1f);
				saveButton.GetComponentInChildren<Text>().text = "SAVE";
				saveButton.GetComponentInChildren<Text>().transform.localScale = new Vector3(5f, 1f, 1f);

				GameObject sAGO = Instantiate(new GameObject(), saveButton.transform);
				sAGO.SetActive(false);

				var saveButtonController = saveButton.gameObject.AddComponent<SaveButton>();
				saveButtonController.activator = sAGO;
				saveButtonController.messagePopupText = "Overrode Preset!";
				saveButtonController.index = i - 1;
				saveButtonController.text = template.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>();
				saveButtonController.weaponType = type;
				saveButtonController.alt = alt;


				saveButton.GetComponent<ShopButton>().toActivate = new GameObject[] { sAGO };
				saveButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];

				Transform clearButton = Instantiate(sliders.parent.parent.parent.Find("Done"), template);

				clearButton.gameObject.name = "Clear";
				clearButton.localPosition = new Vector3(285, -40, -0.0005f);
				clearButton.localScale = new Vector3(0.2f, 1f, 1f);
				clearButton.GetComponentInChildren<Text>().text = "RESET";
				clearButton.GetComponentInChildren<Text>().transform.localScale = new Vector3(5f, 1f, 1f);

				GameObject cAGO = Instantiate(new GameObject(), clearButton.transform);
				cAGO.SetActive(false);

				var clearButtonController = clearButton.gameObject.AddComponent<ClearButton>();
				clearButtonController.activator = cAGO;
				clearButtonController.messagePopupText = "Reset Preset!";
				clearButtonController.index = i - 1;
				clearButtonController.text = template.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>();
				clearButtonController.weaponType = type;


				clearButton.GetComponent<ShopButton>().toActivate = new GameObject[] { cAGO };
				clearButton.GetComponent<ShopButton>().toDeactivate = new GameObject[0];
			}

			sliders.parent.parent.parent.Find("Done").localScale = new Vector3(0.75f, 1f, 1f);

			Transform importButton = Instantiate(sliders.parent.parent.parent.Find("Done"), sliders);
			importButton.gameObject.name = "Import";
			importButton.localPosition = new Vector3(20.0116f, 110f, -0.0004f);
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


		}

		public void LoadCustomColourPreset(GameObject gameObject, string weaponType, bool alt)
        {
			for(int i = 1; i < 5; i++)
            {
				
				var template = gameObject.transform.Find("Template").Find($"Template {i + 1}");

				Color[] colours = new Color[3];

				//fucking die
				//if (!(EnsurePersistentModDataExists($"customPresetOverride.{weaponType}.{i}.isCustom", GUID) && UKMod.RetrieveBooleanPersistentModData($"customPresetOverride.{weaponType}.{i}.isCustom", GUID))) continue;
				if (!(EnsurePersistentModDataExists($"customPresetOverride.{weaponType}.{i}.isCustom", GUID) && UKMod.RetrieveBooleanPersistentModData(GUID, $"customPresetOverride.{weaponType}.{i}.isCustom"))) continue;

				for (int t = 1; t <= 3; t++)
				{
					float[] rgbColourValues = new float[3];

					for (int c = 0; c < 3; c++)
					{
						EnsurePersistentModDataExists($"customPresetOverride.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}", GUID);
						rgbColourValues[c] = UKMod.RetrieveFloatPersistentModData(GUID, $"customPresetOverride.{weaponType}.{i}.{t}.{rgbValueFromInt[c]}");											
					}

					colours[t - 1] = new Color(rgbColourValues[0], rgbColourValues[1], rgbColourValues[2]);
				}



				GunColorPreset newPreset = new GunColorPreset(colours[0], colours[1], colours[2]);

				template.transform.GetChild(0).GetChild(0).GetComponent<UnityEngine.UI.Text>().text = "Custom " + i;

				switch (weaponType)
				{
					case "Revolver":
						MonoSingleton<GunColorController>.Instance.revolverColors[i] = newPreset;
						break;
					case "Shotgun":
						MonoSingleton<GunColorController>.Instance.shotgunColors[i] = newPreset;
						break;
					case "Nailgun":
						MonoSingleton<GunColorController>.Instance.nailgunColors[i] = newPreset;
						break;
					case "Railcannon":
						MonoSingleton<GunColorController>.Instance.railcannonColors[i] = newPreset;
						break;
					case "RocketLauncher":
						MonoSingleton<GunColorController>.Instance.rocketLauncherColors[i] = newPreset;
						break;
				}
			}
		}

		public bool EnsurePersistentModDataExists(string key, string modName)
        {
			//Debug.LogWarning($"{key}: \"{UKMod.RetrieveStringPersistentModData(key, modName)}\"");
			return !string.IsNullOrEmpty(UKMod.RetrieveStringPersistentModData(key, modName));
        }

		public static bool swapped;

		Dictionary<int, string> rgbValueFromInt = new Dictionary<int, string>()
		{
			{ 0, "r" },
			{ 1, "g" },
			{ 2, "b" },
		};

		public static Dictionary<string, GunColorPreset[]> baseWeaponPresetColours = new Dictionary<string, GunColorPreset[]>()
		{
			{ "Revolver", new GunColorPreset[] { new GunColorPreset(new Color(0, 0, 0), new Color(0, 0, 0), new Color(0.42f, 0.19f, 0)), new GunColorPreset(new Color(0, 0.6f, 1), new Color(0.4f, 0.5f, 0.54f), new Color(0.4f, 0.5f, 0.54f)), new GunColorPreset(new Color(1, 0.24f, 0), new Color(0.25f, 0.25f, 0.25f), new Color(0.25f, 0.25f, 0.25f)), new GunColorPreset(new Color(0.5f, 0f, 0f), new Color(0.25f, 0f, 0f), new Color(0.25f, 0f, 0f)) } },
			{ "Shotgun", new GunColorPreset[] { new GunColorPreset(new Color(0f, 0f, 0f), new Color(0f, 0f, 0f), new Color(0f, 0f, 0f)), new GunColorPreset(new Color(0.6f, 0.6f, 0.6f), new Color(1f, 0.66f, 0f), new Color(1f, 1f, 1f)), new GunColorPreset(new Color(1f, 0.8f, 0.6f), new Color(0.6f, 0.6f, 0.6f), new Color(1f, 0.8f, 0.6f)), new GunColorPreset(new Color(0f, 0f, 0f), new Color(1f, 0.65f, 0f), new Color(0f, 0f, 0f)), } },
			{ "Nailgun", new GunColorPreset[] { new GunColorPreset(new Color(0.6f, 0.6f, 0.6f), new Color(0.6f, 0.6f, 0.6f), new Color(0.33f, 1f, 0f)), new GunColorPreset(new Color(0.12f, 0.48f, 1f), new Color(0.12f, 0.48f, 1f), new Color(0f, 0f, 0f)), new GunColorPreset(new Color(1f, 1f, 1f), new Color(0f, 0f, 0f), new Color(0f, 0f, 0f)), new GunColorPreset(new Color(0f, 0f, 0f), new Color(0.5f, 0f, 0f), new Color(0.5f, 0f, 0f)), } },
			{ "Railcannon", new GunColorPreset[] { new GunColorPreset(new Color(0.88f, 0.88f, 0.88f), new Color(0.25f, 0.45f, 0.7f), new Color(0f, 0.35f, 0.7f)), new GunColorPreset(new Color(0.8f, 0.4f, 0.7f), new Color(0.6f, 0.6f, 0.6f), new Color(0f, 0f, 0f)), new GunColorPreset(new Color(0.6f, 0.6f, 0.6f), new Color(0.6f, 0.6f, 0.6f), new Color(1f, 0f, 0f)), new GunColorPreset(new Color(0.88f, 0.66f, 0f), new Color(0f, 0f, 0f), new Color(0f, 0f, 0f)), } },
			{ "RocketLauncher", new GunColorPreset[] { new GunColorPreset(new Color(0.5f, 0.25f, 0.25f), new Color(1f, 0.85f, 0.6f), new Color(0.65f, 0.65f, 0.65f)), new GunColorPreset(new Color(0.66f, 0.66f, 0.66f), new Color(1f, 0f, 0.42f), new Color(0.44f, 0.44f, 0.44f)), new GunColorPreset(new Color(0.7f, 1f, 0f), new Color(0.6f, 0f, 0.4f), new Color(0.64f, 0f, 0.48f)), new GunColorPreset(new Color(0f, 0f, 0f), new Color(0.32f, 0.2f, 0.6f), new Color(0f, 0f, 0f)), } },
		};

	}
}
