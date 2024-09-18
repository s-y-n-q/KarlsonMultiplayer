using HarmonyLib;
using KarlsonMultiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[HarmonyPatch(typeof(RangedWeapon))]
internal class WeaponPatch
{
    [HarmonyPatch("SpawnProjectile")]
    [HarmonyPostfix]
    private static void PostShoot(Vector3 attackDirection)
    {
        foreach (Friend friend in Load.CurrentLobby.Members)
        {
            if (!friend.IsMe)
            {
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"shootgun|{attackDirection.x}|{attackDirection.y}|{attackDirection.z}"));
            }
        }
    }
}

