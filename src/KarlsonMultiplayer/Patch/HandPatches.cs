using HarmonyLib;
using KarlsonMultiplayer;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnityEngine;

[HarmonyPatch(typeof(DetectWeapons))]
internal class HandPatches
{
    [HarmonyPatch("Pickup")]
    [HarmonyPostfix]
    private static void PostPickup(GameObject ___gun)
    {
        try
        {
            Console.WriteLine("grab");
            string name = ___gun.name.Replace("(Clone)", "");
            string[] thing = name.Split(' ');
            //Load.currentweapon = thing[0];
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                //if (!friend.IsMe)
                //{
                //SteamNetworking.SendP2PPacket(friend.Id, byteString);
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"pickup {thing[0]}"));
                //}
            }
        }
        catch (NullReferenceException)
        {
            Load.currentweapon = "none";
        }
    }

    [HarmonyPatch("Throw")]
    [HarmonyPostfix]
    private static void PostThrow(Vector3 throwDir)
    {
        Load.currentweapon = "none";
        foreach (Friend friend in Load.CurrentLobby.Members)
        {
            SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"throw|{throwDir.x.ToString()}|{throwDir.y.ToString()}|{throwDir.z.ToString()}|"));
        }
    }
}
