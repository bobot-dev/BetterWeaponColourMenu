using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BetterWeaponColourMenu
{
    class SaveData
    {

        public static void LoadData()
        {
            path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf("\\")) + "\\saveData.json";
            Debug.Log("Saving colour menu data to " + path);

            if (!File.Exists(path))
            {
                File.Create(path).Close();
            }

            FileInfo fInfo = new FileInfo(path);
            if (fInfo.Exists)
            {
                using (StreamReader jFile = fInfo.OpenText())
                {
                    colourMenuBoolData = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, string>>>(jFile.ReadToEnd());
                    if (colourMenuBoolData == null)
                        colourMenuBoolData = new Dictionary<string, Dictionary<string, string>>();
                    jFile.Close();
                }
            }
            else
            {
                Debug.Log("Couldn't find a save file, making one now");
                fInfo.Create();
            }

            //instance = UnityEngine.JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
        }


        public static void SaveDataToFile()
        {
            path = Assembly.GetExecutingAssembly().Location;
            path = path.Substring(0, path.LastIndexOf("\\")) + "\\saveData.json";
            Debug.Log("Saving colour menu data to " + path);


            FileInfo fInfo = new FileInfo(path);
            File.WriteAllText(fInfo.FullName, JsonConvert.SerializeObject(colourMenuBoolData));
            //if (!File.Exists(path))
            //{
            //    File.Create(path).Close();
            //}                
        }

        public static void SetSaveValue(string dataKey, string weapon, object value, bool forceNotSave = false)
        {
            if (colourMenuBoolData == null) LoadData();


            if (!colourMenuBoolData.ContainsKey(dataKey))
            {
                var dict = new Dictionary<string, string>();
                dict.Add(weapon, value.ToString());

                colourMenuBoolData.Add(dataKey, dict);
            }
            else if (!colourMenuBoolData[dataKey].ContainsKey(weapon))
            {
                colourMenuBoolData[dataKey].Add(weapon, value.ToString());
            }
            else
            {
                colourMenuBoolData[dataKey][weapon] = value.ToString();
            }

            if (!forceNotSave) SaveDataToFile();
        }

        public static string RetriveSaveValue(string dataKey, string weapon, string fallback = "MISSING DATA")
        {
            if (colourMenuBoolData == null) LoadData();

            if (!colourMenuBoolData.ContainsKey(dataKey))
            {
                var dict = new Dictionary<string, string>();
                dict.Add(weapon, fallback);

                colourMenuBoolData.Add(dataKey, dict);
                SaveDataToFile();
            }
            else if (!colourMenuBoolData[dataKey].ContainsKey(weapon))
            {
                colourMenuBoolData[dataKey].Add(weapon, fallback);
                SaveDataToFile();
            }
            return colourMenuBoolData[dataKey][weapon];
        }


        //public static SaveData instance;


        static Dictionary<string, Dictionary<string, string>> colourMenuBoolData;
        
        //Dictionary<string, Dictionary<string, Color>> colourMenuColourData;

        static string path = "";
    }
}
