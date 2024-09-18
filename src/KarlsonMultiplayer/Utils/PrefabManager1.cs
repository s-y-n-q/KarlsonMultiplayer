using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace KarlsonMultiplayer.Utils
{
    public class PrefabManager1 : MonoBehaviour  
    {
        public static List<GameObject> prefabs = new List<GameObject>();

        // the actual prefab

        public static GameObject ak47;
        public static GameObject pistol;
        public static GameObject shotgun;
        public static GameObject boomer;
        public static GameObject grappler;

        public static bool didalready;

        public static void Init()
        {
            if (!didalready)
            {
                prefabs.Add(ak47 = SpawnObject(GameObject.Find("Ak47")));
                prefabs.Add(pistol = SpawnObject(GameObject.Find("Pistol")));
                prefabs.Add(shotgun = SpawnObject(GameObject.Find("Shotgun")));
                prefabs.Add(boomer = SpawnObject(GameObject.Find("Boomer")));
                prefabs.Add(grappler = SpawnObject(GameObject.Find("Grappler")));
                ak47.SetActive(false);
                pistol.SetActive(false);
                shotgun.SetActive(false);
                boomer.SetActive(false);
                grappler.SetActive(false);
                didalready = true;
            }
        }

        public static GameObject GetItemFromName(string name)
        {
            if (name.Contains("Pistol"))
            {
                return pistol;
            }
            if (name.Contains("Ak47"))
            {
                return ak47;
            }
            if (name.Contains("Shotgun"))
            {
                return shotgun;
            }
            if (name.Contains("Boomer"))
            {
                return boomer;
            }
            if (name.Contains("Grappler"))
            {
                return grappler;
            }
            return null;
        }

        public static GameObject SpawnObject(GameObject obj, bool destroyOnLoad = false)
        {
            GameObject gameObject = Instantiate<GameObject>(obj);
            if (!destroyOnLoad)
            {
                Object.DontDestroyOnLoad(gameObject);
            }
            return gameObject;
        }
    }
}
