using HarmonyLib;
using KarlsonMultiplayer;
using KarlsonMultiplayer.Utils;
using Steamworks;
using Steamworks.ServerList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

[HarmonyPatch(typeof(Game))]
internal class GamePatch
{
    [HarmonyPatch("RestartGame")]
    [HarmonyPrefix]
    private static bool PreRestart()
    {
        if (Load.CurrentLobby.Id != 0)
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes("okirespawn"));
            }
        }
        if (Load.CurrentLobby.GetData("rsod") == "True")
        {
            return true;
        }
        Game.Instance.EndGame();
        Time.timeScale = 1f;
        Game.Instance.StartGame();
        GameObject.Find("Player").transform.position = Globals.GetSpawnPointFromMap(SceneManager.GetActiveScene().name);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        GameObject.Find("Player").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeRotation;
        foreach (Friend friend in Load.CurrentLobby.Members)
        {
            //if (!friend.IsMe)
            //{
            //SteamNetworking.SendP2PPacket(friend.Id, byteString);
            SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"okirespawn"));
            //}
        }
        Console.WriteLine("patched restatr game (i cant spell its 12 am mane)");
        return false;
    }
}
