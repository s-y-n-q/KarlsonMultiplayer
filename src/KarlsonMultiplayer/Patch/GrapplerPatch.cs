using HarmonyLib;
using KarlsonMultiplayer;
using KarlsonMultiplayer.Players;
using Steamworks;
using Steamworks.ServerList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[HarmonyPatch(typeof(Grappler))]
internal class GrapplerPatch
{
    [HarmonyPatch("DrawGrapple")]
    [HarmonyPostfix]
    private static void PostUse()
    {
        
    }
}

