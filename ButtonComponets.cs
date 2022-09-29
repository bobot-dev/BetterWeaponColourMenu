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
                OnButtonTriggered();
            }
        }

        public virtual void OnButtonTriggered()
        {
            if (triggerMessage) MonoSingleton<HudMessageReceiver>.Instance.SendHudMessage(messagePopupText, "", "", 0, false);
        }

        public string messagePopupText;

        public static Color copiedColor;

        public bool triggerMessage = true;
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
                        colourSetter.SetRed(presets[i - 1].color1.r);
                        colourSetter.SetGreen(presets[i - 1].color1.g);
                        colourSetter.SetBlue(presets[i - 1].color1.b);
                        break;
                    case 2:
                        colourSetter.SetRed(presets[i - 1].color2.r);
                        colourSetter.SetGreen(presets[i - 1].color2.g);
                        colourSetter.SetBlue(presets[i - 1].color2.b);
                        break;
                    case 3:
                        colourSetter.SetRed(presets[i - 1].color3.r);
                        colourSetter.SetGreen(presets[i - 1].color3.g);
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

            colourSetter.SetRed(copiedColor.r);
            colourSetter.SetGreen(copiedColor.g);
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
            text.text = "IMPLEMT THE THING " + index;

            switch (weaponType)
            {
                case "Revolver":
                    MonoSingleton<GunColorController>.Instance.revolverColors[index] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][index - 1];
                    break;
                case "Shotgun":
                    MonoSingleton<GunColorController>.Instance.shotgunColors[index] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][index - 1];
                    break;
                case "Nailgun":
                    MonoSingleton<GunColorController>.Instance.nailgunColors[index] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][index - 1];
                    break;
                case "Railcannon":
                    MonoSingleton<GunColorController>.Instance.railcannonColors[index] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][index - 1];
                    break;
                case "RocketLauncher":
                    MonoSingleton<GunColorController>.Instance.rocketLauncherColors[index] = BetterWeaponColourMenu.baseWeaponPresetColours[weaponType][index - 1];
                    break;
            }


            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.isCustom", "false", BetterWeaponColourMenu.GUID);


            Type t = typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler");

            MethodInfo method = t.GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic);

            method.Invoke(null, null);


            transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().SetPreset(index);
            if (transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts != null) transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts[index] = text.text;

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
            text.text = "Custom " + index;

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

            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.1.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.1.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.1.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color1.b.ToString(), BetterWeaponColourMenu.GUID);

            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.2.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.2.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.2.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color2.b.ToString(), BetterWeaponColourMenu.GUID);

            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.3.r", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.r.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.3.g", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.g.ToString(), BetterWeaponColourMenu.GUID);
            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.3.b", MonoSingleton<GunColorController>.Instance.CustomGunColorPreset(weaponNumThing[weaponType], alt).color3.b.ToString(), BetterWeaponColourMenu.GUID);


            UKMod.SetPersistentModData($"customPresetOverride.{weaponType}.{index}.isCustom", "true", BetterWeaponColourMenu.GUID);


            Type t = typeof(UKAPI).Assembly.GetType("UMM.UKAPI+SaveFileHandler");

            MethodInfo method = t.GetMethod("DumpFile", BindingFlags.Static | BindingFlags.NonPublic);

            method.Invoke(null, null);


            if (transform.parent.parent.GetComponentInParent<GunColorTypeGetter>() == null) Debug.LogError("SHIT!");
            transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().SetPreset(index);
            if (transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts != null) transform.parent.parent.GetComponentInParent<GunColorTypeGetter>().originalTemplateTexts[index] = text.text;

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
}
