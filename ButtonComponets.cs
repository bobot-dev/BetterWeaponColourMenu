using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UMM;
using System.Reflection;
using UnityEngine.UI;

namespace BetterWeaponColourMenu
{

    class ButtonBase : MonoBehaviour
    {

        public GameObject activator;

        void Awake()
        {
            if (activator != null && activator.activeSelf) activator.SetActive(false);
        }

        private void Update()
        {
            if (activator != null && activator.activeSelf)
            {              
                activator.SetActive(false);
                if (isEnabled) isPressed = !isPressed;
                if (isEnabled) OnButtonTriggered();
            }
        }

        public virtual void OnButtonTriggered()
        {
            if (triggerMessage) MonoSingleton<HudMessageReceiver>.Instance.SendHudMessage(messagePopupText, "", "", 0, false);
        }

        public string messagePopupText;

        public static Color copiedColor;

        public bool triggerMessage = true;

        public bool isEnabled = true;

        public bool isPressed = false;

    }

    class ToggleEffectButton : ButtonBase
    {
        void Start()
        {
            isPressed = bool.Parse(SaveData.RetriveSaveValue("useColourEffects", $"{weaponNum}{(alt ? ".a" : "")}", "false"));

            if (CheckBox && CheckBox.GetComponent<Image>()) CheckBox.GetComponent<Image>().fillCenter = isPressed;
        }

        public override void OnButtonTriggered()
        {
            if (CheckBox && CheckBox.GetComponent<Image>()) CheckBox.GetComponent<Image>().fillCenter = isPressed;

            SaveData.SetSaveValue("useColourEffects", $"{weaponNum}{(alt ? ".a" : "")}", isPressed);

            base.OnButtonTriggered();
        }

        public GameObject CheckBox;
        public int weaponNum;
        public bool alt;
    }

    class CopyVarientColourButton : ButtonBase
    {
        void Start()
        {

            

            //var key = $"customPresetOverride.W{weaponNum}{(alt ? ".a" : "")}.{colorNum}";
            //if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "false", BetterWeaponColourMenu.GUID);

            //isPressed = UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, key);
            isPressed = bool.Parse(SaveData.RetriveSaveValue($"useVariationColour.{colorNum}", $"{weaponNum}{(alt ? ".a" : "")}", "false")); 

            

            OnButtonTriggered();
        }

        public override void OnButtonTriggered()
        {
            
            if (CheckBox && CheckBox.GetComponent<Image>()) CheckBox.GetComponent<Image>().fillCenter = isPressed;


            //GameObject GC = GameObject.FindGameObjectWithTag("GunControl");
            GameObject GC = GunControl.instance.gameObject;

            foreach (Renderer renderer in GC.GetComponentsInChildren<Renderer>(true))
            {
                if (renderer.gameObject.GetComponent<VariationColorHandler>())
                {
                    if (renderer.gameObject.GetComponent<VariationColorHandler>().weaponnum == weaponNum && renderer.gameObject.GetComponent<VariationColorHandler>().alt == alt)
                    {
                        VariationColorHandler VCH = renderer.gameObject.GetComponent<VariationColorHandler>();
                        switch (colorNum)
                        {
                            case 1:
                                VCH.swapColor1 = isPressed;
                                break;
                            case 2:
                                VCH.swapColor2 = isPressed;
                                break;
                            case 3:
                                VCH.swapColor3 = isPressed;
                                break;
                            default:
                                break;
                        }

                        var colourSetter = GetComponentInParent<GunColorSetter>();

                        colourSetter.transform.Find("Red").GetComponentInChildren<ControllerPointer>().enabled = !isPressed;
                        colourSetter.transform.Find("Red").GetComponentInChildren<Slider>().interactable = !isPressed;

                        colourSetter.transform.Find("Green").GetComponentInChildren<ControllerPointer>().enabled = !isPressed;
                        colourSetter.transform.Find("Green").GetComponentInChildren<Slider>().interactable = !isPressed;

                        colourSetter.transform.Find("Blue").GetComponentInChildren<ControllerPointer>().enabled = !isPressed;
                        colourSetter.transform.Find("Blue").GetComponentInChildren<Slider>().interactable = !isPressed;

                        //Debug.Log("CopyVarientColourButton.isPressed = " + isPressed);

                        SaveData.SetSaveValue($"useVariationColour.{colorNum}", $"{weaponNum}{(alt ? ".a" : "")}", isPressed);

                        //UKMod.SetPersistentModData($"customPresetOverride.W{weaponNum}{(alt ? ".a" : "")}.{colorNum}", isPressed.ToString(), BetterWeaponColourMenu.GUID);

                        //typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler").GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

                        if (renderer.GetComponent<GunColorSetter>()) renderer.GetComponent<GunColorSetter>().UpdateColor();
                    }
                }
            }

            base.OnButtonTriggered();
        }

        public GameObject CheckBox;
        public GunColorSetter GunColSetter;
        public int colorNum;
        public int weaponNum;
        public bool alt;
    }

    class CopyButton : ButtonBase
    {

        public override void OnButtonTriggered()
        {
            var colourSetter = GetComponentInParent<GunColorSetter>();

            copiedColor = new Color(colourSetter.redAmount, colourSetter.greenAmount, colourSetter.blueAmount);

            base.OnButtonTriggered();
        }

    }

    class ImportButton : ButtonBase
    {

        public override void OnButtonTriggered()
        {
            GunColorPreset[] presets = new GunColorPreset[3];
            for (int i = 1; i <= 3; i++)
            {
                var presetNum = MonoSingleton<PrefsManager>.Instance.GetInt("gunColorPreset." + weaponNumThing[weaponType] + (alt ? ".a" : ""), 0);


                var colourSetter = transform.parent.Find("Color " + i).gameObject.GetComponent<GunColorSetter>();

                switch (weaponType)
                {
                    case "Revolver":
                        presets[i - 1] = MonoSingleton<GunColorController>.Instance.revolverColors[presetNum];
                        break;
                    case "Shotgun":
                        presets[i - 1] = MonoSingleton<GunColorController>.Instance.shotgunColors[presetNum];
                        break;
                    case "Nailgun":
                        presets[i - 1] = MonoSingleton<GunColorController>.Instance.nailgunColors[presetNum];
                        break;
                    case "Railcannon":
                        presets[i - 1] = MonoSingleton<GunColorController>.Instance.railcannonColors[presetNum];
                        break;
                    case "RocketLauncher":
                        presets[i - 1] = MonoSingleton<GunColorController>.Instance.rocketLauncherColors[presetNum];
                        break;
                }

                switch (i)
                {
                    case 1:
                        colourSetter.transform.Find("Red").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color1.r;
                        colourSetter.SetRed(presets[i - 1].color1.r);

                        colourSetter.transform.Find("Green").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color1.g;
                        colourSetter.SetGreen(presets[i - 1].color1.g);

                        colourSetter.transform.Find("Blue").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color1.b;
                        colourSetter.SetBlue(presets[i - 1].color1.b);
                        break;
                    case 2:
                        colourSetter.transform.Find("Red").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color2.r;
                        colourSetter.SetRed(presets[i - 1].color2.r);

                        colourSetter.transform.Find("Green").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color2.g;
                        colourSetter.SetGreen(presets[i - 1].color2.g);

                        colourSetter.transform.Find("Blue").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color2.b;
                        colourSetter.SetBlue(presets[i - 1].color2.b);
                        break;
                    case 3:
                        colourSetter.transform.Find("Red").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color3.r;
                        colourSetter.SetRed(presets[i - 1].color3.r);

                        colourSetter.transform.Find("Green").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color3.g;
                        colourSetter.SetGreen(presets[i - 1].color3.g);

                        colourSetter.transform.Find("Blue").GetComponentInChildren<UnityEngine.UI.Slider>().value = presets[i - 1].color3.b;
                        colourSetter.SetBlue(presets[i - 1].color3.b);
                        break;
                }
            }

            base.OnButtonTriggered();
        }

        public bool alt;
        public string weaponType;

        Dictionary<string, int> weaponNumThing = new Dictionary<string, int>()
        {
            { "Revolver", 1 },
            { "Shotgun", 2 },
            { "Nailgun", 3 },
            { "Railcannon", 4 },
            { "RocketLauncher", 5 },
        };

    }


    class PasteButton : ButtonBase
    {
        public override void OnButtonTriggered()
        {
            var colourSetter = GetComponentInParent<GunColorSetter>();

            if (copiedColor == null) copiedColor = Color.white;

            colourSetter.transform.Find("Red").GetComponentInChildren<UnityEngine.UI.Slider>().value = copiedColor.r;
            colourSetter.SetRed(copiedColor.r);

            colourSetter.transform.Find("Green").GetComponentInChildren<UnityEngine.UI.Slider>().value = copiedColor.g;
            colourSetter.SetGreen(copiedColor.g);

            colourSetter.transform.Find("Blue").GetComponentInChildren<UnityEngine.UI.Slider>().value = copiedColor.b;
            colourSetter.SetBlue(copiedColor.b);

            base.OnButtonTriggered();
        }
    }

    class RandomButton : ButtonBase
    {
        public override void OnButtonTriggered()
        {
            var colourSetter = GetComponentInParent<GunColorSetter>();

            var r = UnityEngine.Random.value;
            colourSetter.transform.Find("Red").GetComponentInChildren<UnityEngine.UI.Slider>().value = r;
            colourSetter.SetRed(r);

            var g = UnityEngine.Random.value;
            colourSetter.transform.Find("Green").GetComponentInChildren<UnityEngine.UI.Slider>().value = g;
            colourSetter.SetGreen(g);

            var b = UnityEngine.Random.value;
            colourSetter.transform.Find("Blue").GetComponentInChildren<UnityEngine.UI.Slider>().value = b;
            colourSetter.SetBlue(b);

            base.OnButtonTriggered();
        }
    }

    class ClearButton : ButtonBase
    {
        public override void OnButtonTriggered()
        {

            //var key = $"customPresetOverride.{weaponType}{(alt ? ".a" : "")}.CurrentPresetCollectionIndex";
            //if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "-1", BetterWeaponColourMenu.GUID);

            //var currentPresetColI = UKMod.RetrieveFloatPersistentModData(BetterWeaponColourMenu.GUID, key);

            switch (weaponType)
            {
                case 1:
                    MonoSingleton<GunColorController>.Instance.revolverColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
                case 2:
                    MonoSingleton<GunColorController>.Instance.shotgunColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
                case 3:
                    MonoSingleton<GunColorController>.Instance.nailgunColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
                case 4:
                    MonoSingleton<GunColorController>.Instance.railcannonColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
                case 5:
                    MonoSingleton<GunColorController>.Instance.rocketLauncherColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
            }


            //UKMod.SetPersistentModData($"customPresetOverride.{BetterWeaponColourMenu.currentPresetCollectionIndex}.{weaponType}.{index}.isCustom", "false", BetterWeaponColourMenu.GUID);

            //var key = $"customPresetOverride.{weaponType}{(alt ? ".a" : "")}.CurrentPresetCollectionIndex";
            //if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "-1", BetterWeaponColourMenu.GUID);

            //var currentPresetColI = UKMod.RetrieveFloatPersistentModData(BetterWeaponColourMenu.GUID, key);
            var currentPresetColI = int.Parse(SaveData.RetriveSaveValue("currentPresetCollectionIndex", $"{weaponType}{(alt ? ".a" : "")}", "-1"));

            for (int i = 1; i <= 3; i++)
            {
                //UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.W{weaponType}{(alt ? ".a" : "")}.{i}.{index}", false.ToString(), BetterWeaponColourMenu.GUID);

                SaveData.SetSaveValue($"useVariationColour.{currentPresetColI}.{i}", $"{weaponType}{(alt ? ".a" : "")}", false);
            }


            //typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler").GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

            transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().SetPreset(index);
            //if (transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts != null) transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts[index] = text.text;

            base.OnButtonTriggered();
        }

        public int weaponType;
        public int index = 1;
        public UnityEngine.UI.Text text;
        public bool alt;
    }

    class SaveButton : ButtonBase
    {
        public override void OnButtonTriggered()
        {
            //text.text = "Custom " + index;

            switch(weaponType)
            {
                case 1:
                    MonoSingleton<GunColorController>.Instance.revolverColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(1, alt);
                    break;
                case 2:
                    MonoSingleton<GunColorController>.Instance.shotgunColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(2, false);                   
                    break;
                case 3:
                    MonoSingleton<GunColorController>.Instance.nailgunColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(3, alt);
                    break;
                case 4:
                    MonoSingleton<GunColorController>.Instance.railcannonColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(4, false);
                    break;
                case 5:
                    MonoSingleton<GunColorController>.Instance.rocketLauncherColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(5, false);
                    break;
            }


            var currentPresetColI = int.Parse(SaveData.RetriveSaveValue("currentPresetCollectionIndex", $"{weaponType}{(alt ? ".a" : "")}", "-1"));


            SaveData.SetSaveValue($"customPreset.{currentPresetColI}.{1}.{index}", $"{weaponType}", ColorUtility.ToHtmlStringRGB(MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponType, alt).color1));
            SaveData.SetSaveValue($"customPreset.{currentPresetColI}.{2}.{index}", $"{weaponType}", ColorUtility.ToHtmlStringRGB(MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponType, alt).color2));
            SaveData.SetSaveValue($"customPreset.{currentPresetColI}.{3}.{index}", $"{weaponType}", ColorUtility.ToHtmlStringRGB(MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponType, alt).color3));

            /*
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.1.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.1.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.1.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.b.ToString(), BetterWeaponColourMenu.GUID);

            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.2.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.2.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.2.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.b.ToString(), BetterWeaponColourMenu.GUID);

            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.3.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.3.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.3.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.b.ToString(), BetterWeaponColourMenu.GUID);
            */


            for(int i = 1; i <= 3; i++)
            {
                //string key2 = $"customPresetOverride.W{weaponNumThing[weaponType]}{(alt ? ".a" : "")}.{i}";

                //if (!UKMod.PersistentModDataExists(key2, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key2, "false", BetterWeaponColourMenu.GUID);

                //var b = UKMod.RetrieveBooleanPersistentModData(BetterWeaponColourMenu.GUID, key2);
                var b = SaveData.RetriveSaveValue($"useVariationColour.{i}", $"{weaponType}{(alt ? ".a" : "")}", "false");
                //UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.W{weaponNumThing[weaponType]}{(alt ? ".a" : "")}.{i}.{index}", b.ToString(), BetterWeaponColourMenu.GUID);

                SaveData.SetSaveValue($"useVariationColour.{currentPresetColI}.{i}", $"{weaponType}{(alt ? ".a" : "")}", b);
            }


           




            //UKMod.SetPersistentModData($"customPresetOverride.{BetterWeaponColourMenu.currentPresetCollectionIndex}.{weaponType}.{index}.isCustom", "true", BetterWeaponColourMenu.GUID);


            //typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler").GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

            transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().SetPreset(index);
            //if (transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts != null) transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts[index] = text.text;

            base.OnButtonTriggered();
        }

        public int weaponType;
        public int index = 1;
        public UnityEngine.UI.Text text;
        public bool alt;

    }

    class NextPresetCollectionButton : ButtonBase
    {
        void OnGUI()
        {
            var currentPresetColI = int.Parse(SaveData.RetriveSaveValue("currentPresetCollectionIndex", $"{weaponType}{(alt ? ".a" : "")}", "-1"));

            if (side == Side.Left) Toggle(currentPresetColI > -1);
            if (side == Side.Right) Toggle(currentPresetColI < 24);
        }

        void Awake()
        {
            var currentPresetColI = int.Parse(SaveData.RetriveSaveValue("currentPresetCollectionIndex", $"{weaponType}{(alt ? ".a" : "")}", "-1"));
            currentPresetColI -= (int)side;
            SaveData.SetSaveValue("currentPresetCollectionIndex", $"{weaponType}{(alt ? ".a" : "")}", currentPresetColI);

            OnButtonTriggered();
        }

        public override void OnButtonTriggered()
        {
            //var key = $"customPresetOverride.{weaponType}{(alt ? ".a" : "")}.CurrentPresetCollectionIndex";
            //if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "-1", BetterWeaponColourMenu.GUID);

             //var currentPresetColI = UKMod.RetrieveIntPersistentModData(BetterWeaponColourMenu.GUID, key);
             var currentPresetColI = int.Parse(SaveData.RetriveSaveValue("currentPresetCollectionIndex", $"{weaponType}{(alt ? ".a" : "")}", "-1"));

            currentPresetColI += (int)side;

            if (currentPresetColI == -1)
            {
                for (int i = 1; i <= 4; i++)
                {
                    string text = "FUCK";
                    switch (weaponType)
                    {
                        case 1:
                            MonoSingleton<GunColorController>.Instance.revolverColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                        case 2:
                            MonoSingleton<GunColorController>.Instance.shotgunColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                        case 3:
                            MonoSingleton<GunColorController>.Instance.nailgunColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                        case 4:
                            MonoSingleton<GunColorController>.Instance.railcannonColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                        case 5:
                            MonoSingleton<GunColorController>.Instance.rocketLauncherColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                    }

                    var template = transform.parent.Find($"Template {i + 1}");

                    template.GetChild(0).GetComponentInChildren<UnityEngine.UI.Text>().text = text;

                    if (template.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts != null) transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts[i - 1] = text;

                    var saveButton = template.Find("Save").gameObject;
                    var clearButton = template.Find("Clear").gameObject;

                    saveButton.SetActive(false);
                    clearButton.SetActive(false);

                    template.transform.GetChild(0).localScale = Vector3.one;
                    template.transform.GetChild(0).GetChild(0).localScale = Vector3.one;
                }

            }
            else
            {
                for (int i = 1; i <= 4; i++)
                {
                    //gunColorTypeGetter.weaponNumber
                    GunColorPreset newPreset = BetterWeaponColourMenu.GetPreset(weaponType, i, currentPresetColI);

                    switch (weaponType)
                    {
                        case 1:
                            MonoSingleton<GunColorController>.Instance.revolverColors[i] = newPreset;
                            break;
                        case 2:
                            MonoSingleton<GunColorController>.Instance.shotgunColors[i] = newPreset;
                            break;
                        case 3:
                            MonoSingleton<GunColorController>.Instance.nailgunColors[i] = newPreset;
                            break;
                        case 4:
                            MonoSingleton<GunColorController>.Instance.railcannonColors[i] = newPreset;
                            break;
                        case 5:
                            MonoSingleton<GunColorController>.Instance.rocketLauncherColors[i] = newPreset;
                            break;
                    }

                    var template = transform.parent.Find($"Template {i + 1}");

                    template.GetChild(0).GetComponentInChildren<UnityEngine.UI.Text>().text = $"Custom {(4 * currentPresetColI) + i}";

                    if (template.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts != null) transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts[i - 1] = $"Custom {(4 * currentPresetColI) + i}";


                    var saveButton = template.Find("Save").gameObject;
                    var clearButton = template.Find("Clear").gameObject;

                    saveButton.SetActive(true);
                    clearButton.SetActive(true);

                    template.transform.GetChild(0).localScale = new Vector3(0.75f, 1, 1);
                    template.transform.GetChild(0).GetChild(0).localScale = new Vector3(1.5f, 1, 1);
                }
            }

            SaveData.SetSaveValue("currentPresetCollectionIndex", $"{weaponType}{(alt ? ".a" : "")}", currentPresetColI);
            //UKMod.SetPersistentModData(key, currentPresetColI.ToString(), BetterWeaponColourMenu.GUID);
            transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().SetPreset(MonoSingleton<PrefsManager>.Instance.GetInt("gunColorPreset." + weaponType + (alt ? ".a" : ""), 0));

            //typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler").GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);
        }


        void Toggle(bool toggle)
        {
            this.isEnabled = toggle;
            gameObject.GetComponent<UnityEngine.UI.Button>().enabled = toggle;
            gameObject.GetComponent<UnityEngine.UI.Image>().color = toggle ? Color.white : Color.gray;
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().color = toggle ? Color.white : Color.gray;
        }

        public int weaponType;
        public bool alt;
        public Side side;



        public enum Side
        {
            Right = 1,
            Left = -1,
        }
    }
}
