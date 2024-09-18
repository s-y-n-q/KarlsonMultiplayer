using HarmonyLib;
using KarlsonMultiplayer.Utils;
using KarlsonMultiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;
using UnityEngine;

[HarmonyPatch(typeof(Barrel))]
internal class BarrelPatch
{
    [HarmonyPatch("Explode")]
    [HarmonyPostfix]
    private static void PostExplode()
    {
        Console.WriteLine("barrel explosion");
    }
}