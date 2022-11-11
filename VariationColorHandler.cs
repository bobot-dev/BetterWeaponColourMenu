using System;
using UnityEngine;
using UMM;

namespace BetterWeaponColourMenu
{
    public class VariationColorHandler : MonoBehaviour
    {
        public int weaponnum;
        //public VarColorMain VCM;
        public bool alt; 
        public bool swapColor1;
        public bool swapColor2;
        public bool swapColor3;

        private void Start()
        {
           /* load which colors should be swapped from persistent data when this component is made to pass into the guncolorsetter
            if (!UKMod.PersistentModDataExists("Weapon." + weaponnum + ".Alt." + alt + ".Col1", "Tony.GunColorVariation"))
            {
                UKMod.SetPersistentModData("Weapon." + weaponnum + ".Alt." + alt + ".Col1", "false", "Tony.GunColorVariation");
            }
            else
            {
                test = UKMod.RetrieveStringPersistentModData("Weapon." + weaponnum + ".Alt." + alt + ".Col1", "Tony.GunColorVariation");
            }
           /* if (!UKMod.PersistentModDataExists("Weapon." + weaponnum + ".Alt." + alt + ".Col2." + swapColor2, "Tony.GunColorVariation"))
            {
                VCM.SetPersistentModData("Weapon." + weaponnum + ".Alt." + alt + ".Col2." + swapColor2, "Tony.GunColorVariation");
            }
            else
            {
                swapColor2 = VCM.RetrieveBooleanPersistentModData("Weapon." + weaponnum + ".Alt." + alt + ".Col1." + swapColor2);
            }
            if (!UKMod.PersistentModDataExists("Weapon." + weaponnum + ".Alt." + alt + ".Col3." + swapColor3, "Tony.GunColorVariation"))
            {
                VCM.SetPersistentModData("Weapon." + weaponnum + ".Alt." + alt + ".Col3." + swapColor3, "Tony.GunColorVariation");
            }
            else
            {
                swapColor3 = VCM.RetrieveBooleanPersistentModData("Weapon." + weaponnum + ".Alt." + alt + ".Col1." + swapColor3);
            }*/
        }
    }
}
