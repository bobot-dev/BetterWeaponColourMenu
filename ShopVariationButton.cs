using System;
using UMM;
using UnityEngine;
using UnityEngine.UI;

namespace BetterWeaponColourMenu
{
    public class ShopVariationButton : MonoBehaviour
    {
        public GameObject Activator;
		public GameObject CheckBox;
		public GunColorSetter GunColSetter;
		public int ColorNum;
		public int weapo	nNum;
		public bool Alt;
		public bool isenabled;
		public bool wasActivated;

		private void Start()
		{
			//load the persistent mod data if it exists and use that to force the color update or someshit idk
			/*if(UKMod.PersistentModDataExists("Weapon." + weaponNum + ".Alt." + Alt + ".Col1", "Tony.GunColorVariation"))
			{
				wasActivated = UKMod.RetrieveBooleanPersistentModData("Weapon." + weaponNum + ".Alt." + Alt + ".Col1", "Tony.GunColorVariation");
				if(wasActivated)
				{
					OnButtonTriggered(true);
				}
			}
			 */
		}

		private void Update()
		{
			if (Activator != null && Activator.activeSelf)
			{
				Activator.SetActive(false);
				OnButtonTriggered();
			}
		}

		public virtual void OnButtonTriggered(bool awakeactive = false)
		{
			isenabled = awakeactive ? true : !isenabled;
			if(CheckBox && CheckBox.GetComponent<Image>())
			CheckBox.GetComponent<Image>().fillCenter = isenabled;

			GameObject GC = GameObject.FindGameObjectWithTag("GunControl");
			foreach (Renderer renderer in GC.GetComponentsInChildren<Renderer>(true))
			{
				if (renderer.gameObject.GetComponent<VariationColorHandler>())
				{
					if(renderer.gameObject.GetComponent<VariationColorHandler>().weaponnum == weaponNum && renderer.gameObject.GetComponent<VariationColorHandler>().alt == Alt)
					{
						VariationColorHandler VCH = renderer.gameObject.GetComponent<VariationColorHandler>();
						switch(ColorNum)
                        {
							case 1:
								VCH.swapColor1 = isenabled;
								break;
							case 2:
								VCH.swapColor2 = isenabled;
								break;
							case 3:
								VCH.swapColor3 = isenabled;
								break;
							default:
								break;
						}
						//update the persistent mod data for each color bool, i only did 1 here for testing
						//UKMod.SetPersistentModData("Weapon." + VCH.weaponnum + ".Alt." + VCH.alt + ".Col1", isenabled.ToString(), "Tony.GunColorVariation");
						if (renderer.GetComponent<GunColorSetter>()) renderer.GetComponent<GunColorSetter>().UpdateColor();
					}
				}
			}
		}
	}
}
