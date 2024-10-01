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
using KarlsonMultiplayer.Players;
using Steamworks.ServerList;

[HarmonyPatch(typeof(Bullet))]
internal class BulletPatch
{
    [HarmonyPatch("OnCollisionEnter")]
    [HarmonyPostfix]
    private static void PostCollision(Collision other)
    {
        //Console.WriteLine("bullet collision: " + other.gameObject.name);
        if (other.gameObject.name.StartsWith("default"))
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"explodeb|{other.gameObject.transform.root.gameObject.name}"));
            }
            Console.WriteLine("barrel explosion packet");
        }
        if (other.gameObject.transform.root.name.StartsWith("Enemy"))
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"killenemy|gun|{other.gameObject.name}|{other.gameObject.transform.root.gameObject.name}"));
            }
            Console.WriteLine("shoot enemy packet");
        }
        if (other.gameObject.name.StartsWith("Glass"))
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                //if (!friend.IsMe)
                //{
                //SteamNetworking.SendP2PPacket(friend.Id, byteString);
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"breakglass|{other.gameObject.name}"));
                //}
            }
            Console.WriteLine("sent breakglass packet");
        }
        if (PlayerManager.GetPlayerFromName(other.gameObject.name) != null)
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"initdeadtext|{PlayerManager.GetPlayerFromName(other.gameObject.name)}"));
            }
            SteamNetworking.SendP2PPacket(PlayerManager.GetPlayerFromName(other.gameObject.name).LinkedPlayer, Encoding.UTF8.GetBytes($"initdeath"));
        }
        if (other.gameObject.name.StartsWith("Door"))
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                //if (!friend.IsMe)
                //{
                //SteamNetworking.SendP2PPacket(friend.Id, byteString);
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"breakdoor|{other.gameObject.name}"));
                //}
            }
            Console.WriteLine("sent breakdoor packet");
        }
    }
}
