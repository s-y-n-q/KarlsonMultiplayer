using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KarlsonMultiplayer.Utils
{
    public class Globals
    {
        public class SpawnPoints
        {
            public static Vector3 Tutorial = new Vector3(0f, -1.8f, 0f);
            public static Vector3 Sandbox0 = new Vector3(0.0203f, 0.7f, -50.0198f);
            public static Vector3 Sandbox1 = new Vector3(188.7914f, 51.5f, -46.9901f);
            public static Vector3 Sandbox2 = new Vector3(188.7913f, 156.2f, -46.9901f);
            public static Vector3 Escape0 = new Vector3(0f, 0.7f, 0f);
            public static Vector3 Escape1 = new Vector3(0f, 0.7f, 0f);
            public static Vector3 Escape2 = new Vector3(0.0648f, 0.7f, 132.5047f);
            public static Vector3 Escape3 = new Vector3(21.3442f, -0.3f, -1.28f);
            public static Vector3 Sky0 = new Vector3(-0.01f, 5.7f, -0.0326f);
            public static Vector3 Sky1 = new Vector3(-0.01f, 5.7f, -0.0326f);
            public static Vector3 Sky2 = new Vector3(-0.01f, 5.7f, -0.0326f);
        }

        public class Settings
        {
            public static bool ResetSceneOnDeath = false;
            public static bool ThrowWeaponsOnDeath = true;
        }

        public static Vector3 GetSpawnPointFromMap(string name)
        {
            switch (name)
            {
                case "0Tutorial":
                    return Globals.SpawnPoints.Tutorial;
                case "1Sandbox0":
                    return Globals.SpawnPoints.Sandbox0;
                case "2Sandbox1":
                    return Globals.SpawnPoints.Sandbox1;
                case "3Sandbox2":
                    return Globals.SpawnPoints.Sandbox2;
                case "4Escape0":
                    return Globals.SpawnPoints.Escape0;
                case "5Escape1":
                    return Globals.SpawnPoints.Escape1;
                case "6Escape2":
                    return Globals.SpawnPoints.Escape2;
                case "7Escape3":
                    return Globals.SpawnPoints.Escape3;
                case "8Sky0":
                    return Globals.SpawnPoints.Sky0;
                case "9Sky1":
                    return Globals.SpawnPoints.Sky1;
                case "10Sky2":
                    return Globals.SpawnPoints.Sky2;
            }
            return new Vector3();
        }
    }
}
