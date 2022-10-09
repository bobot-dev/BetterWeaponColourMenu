using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UMM;
using System.Reflection;

namespace BetterWeaponColourMenu
{

    class ButtonBase : MonoBehaviour
    {

        public GameObject activator;

        private void Update()
        {
            if (activator != null && activator.activeSelf)
            {
                activator.SetActive(false);
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
                case "Revolver":
                    MonoSingleton<GunColorController>.Instance.revolverColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
                case "Shotgun":
                    MonoSingleton<GunColorController>.Instance.shotgunColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
                case "Nailgun":
                    MonoSingleton<GunColorController>.Instance.nailgunColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
                case "Railcannon":
                    MonoSingleton<GunColorController>.Instance.railcannonColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
                case "RocketLauncher":
                    MonoSingleton<GunColorController>.Instance.rocketLauncherColors[index] = new GunColorPreset(Color.black, Color.black, Color.black);
                    //text.text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][index - 1];
                    break;
            }


            //UKMod.SetPersistentModData($"customPresetOverride.{BetterWeaponColourMenu.currentPresetCollectionIndex}.{weaponType}.{index}.isCustom", "false", BetterWeaponColourMenu.GUID);




            typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler").GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

            transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().SetPreset(index);
            //if (transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts != null) transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts[index] = text.text;

            base.OnButtonTriggered();
        }

        public string weaponType;
        public int index = 1;
        public UnityEngine.UI.Text text;
    }

    class SaveButton : ButtonBase
    {
        public override void OnButtonTriggered()
        {
            //text.text = "Custom " + index;

            switch(weaponType)
            {
                case "Revolver":
                    MonoSingleton<GunColorController>.Instance.revolverColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(1, alt);
                    break;
                case "Shotgun":
                    MonoSingleton<GunColorController>.Instance.shotgunColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(2, false);                   
                    break;
                case "Nailgun":
                    MonoSingleton<GunColorController>.Instance.nailgunColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(3, alt);
                    break;
                case "Railcannon":
                    MonoSingleton<GunColorController>.Instance.railcannonColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(4, false);
                    break;
                case "RocketLauncher":
                    MonoSingleton<GunColorController>.Instance.rocketLauncherColors[index] = MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(5, false);
                    break;
            }

            var key = $"customPresetOverride.{weaponType}{(alt ? ".a" : "")}.CurrentPresetCollectionIndex";
            if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "-1", BetterWeaponColourMenu.GUID);

            var currentPresetColI = UKMod.RetrieveFloatPersistentModData(BetterWeaponColourMenu.GUID, key);

            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.1.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.1.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.1.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.b.ToString(), BetterWeaponColourMenu.GUID);

            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.2.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.2.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.2.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.b.ToString(), BetterWeaponColourMenu.GUID);

            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.3.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.3.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{currentPresetColI}.{weaponType}.{index}.3.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.b.ToString(), BetterWeaponColourMenu.GUID);


            //UKMod.SetPersistentModData($"customPresetOverride.{BetterWeaponColourMenu.currentPresetCollectionIndex}.{weaponType}.{index}.isCustom", "true", BetterWeaponColourMenu.GUID);


            typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler").GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic).Invoke(null, null);

            transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().SetPreset(index);
            //if (transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts != null) transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts[index] = text.text;

            base.OnButtonTriggered();
        }

        public string weaponType;
        public int index = 1;
        public UnityEngine.UI.Text text;
        public bool alt;

        Dictionary<string, int> weaponNumThing = new Dictionary<string, int>()
        {
            { "Revolver", 1 },
            { "Shotgun", 2 },
            { "Nailgun", 3 },
            { "Railcannon", 4 },
            { "RocketLauncher", 5 },
        };
        
    }

    class NextPresetCollectionButton : ButtonBase
    {
        void OnGUI()
        {
            var key = $"customPresetOverride.{weaponType}{(alt ? ".a" : "")}.CurrentPresetCollectionIndex";
            if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "-1", BetterWeaponColourMenu.GUID);

            var currentPresetColI = UKMod.RetrieveFloatPersistentModData(BetterWeaponColourMenu.GUID, key);

            if (side == Side.Left) Toggle(currentPresetColI > -1);
            if (side == Side.Right) Toggle(currentPresetColI < 24);
        }

        void Awake()
        {
            var key = $"customPresetOverride.{weaponType}{(alt ? ".a" : "")}.CurrentPresetCollectionIndex";
            if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "-1", BetterWeaponColourMenu.GUID);

            var currentPresetColI = UKMod.RetrieveFloatPersistentModData(BetterWeaponColourMenu.GUID, key);
            currentPresetColI -= (int)side;
            UKMod.SetPersistentModData(key, currentPresetColI.ToString(), BetterWeaponColourMenu.GUID);

            OnButtonTriggered();
        }

        public override void OnButtonTriggered()
        {
            var key = $"customPresetOverride.{weaponType}{(alt ? ".a" : "")}.CurrentPresetCollectionIndex";
            if (!UKMod.PersistentModDataExists(key, BetterWeaponColourMenu.GUID)) UKMod.SetPersistentModData(key, "-1", BetterWeaponColourMenu.GUID);

             var currentPresetColI = UKMod.RetrieveIntPersistentModData(BetterWeaponColourMenu.GUID, key);

            currentPresetColI += (int)side;

            if (currentPresetColI == -1)
            {
                for (int i = 1; i <= 4; i++)
                {
                    string text = "FUCK";
                    switch (weaponType)
                    {
                        case "Revolver":
                            MonoSingleton<GunColorController>.Instance.revolverColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                        case "Shotgun":
                            MonoSingleton<GunColorController>.Instance.shotgunColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                        case "Nailgun":
                            MonoSingleton<GunColorController>.Instance.nailgunColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                        case "Railcannon":
                            MonoSingleton<GunColorController>.Instance.railcannonColors[i] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][i - 1];
                            text = BetterWeaponColourMenu.baseWeaponPresetNames[weaponType][i - 1];
                            break;
                        case "RocketLauncher":
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
                    
                    GunColorPreset newPreset = BetterWeaponColourMenu.GetPreset(weaponType, i, currentPresetColI);

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


            UKMod.SetPersistentModData(key, currentPresetColI.ToString(), BetterWeaponColourMenu.GUID);
            transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().SetPreset(MonoSingleton<PrefsManager>.Instance.GetInt("gunColorPreset." + BetterWeaponColourMenu.weaponNumberFromName[weaponType] + (alt ? ".a" : ""), 0));
        }


        void Toggle(bool toggle)
        {
            this.isEnabled = toggle;
            gameObject.GetComponent<UnityEngine.UI.Button>().enabled = toggle;
            gameObject.GetComponent<UnityEngine.UI.Image>().color = toggle ? Color.white : Color.gray;
            gameObject.GetComponentInChildren<UnityEngine.UI.Text>().color = toggle ? Color.white : Color.gray;
        }

        public string weaponType;
        public bool alt;
        public Side side;



        public enum Side
        {
            Right = 1,
            Left = -1,
        }
    }
}
