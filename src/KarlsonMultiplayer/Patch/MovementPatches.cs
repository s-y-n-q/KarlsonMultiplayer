using EZCameraShake;
using HarmonyLib;
using KarlsonMultiplayer;
using KarlsonMultiplayer.Utils;
using Steamworks;
using Steamworks.ServerList;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;

[HarmonyPatch(typeof(PlayerMovement))]
internal class MovementPatches
{
    [HarmonyPatch("StartCrouch")]
    [HarmonyPostfix]
    private static void PostStartCrouch()
    {
        UnityEngine.Debug.Log("post start star");
        foreach (Friend friend in Load.CurrentLobby.Members)
        {
            //if (!friend.IsMe)
            //{
            //SteamNetworking.SendP2PPacket(friend.Id, byteString);
            SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes("startcrouch"));
            //}
        }
        UnityEngine.Debug.Log("post start end");
    }

    [HarmonyPatch("StopCrouch")]
    [HarmonyPostfix]
    private static void PostStopCrouch()
    {
        UnityEngine.Debug.Log("post stop start");
        if (Load.CurrentLobby.Id != 0)
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                //if (!friend.IsMe)
                //{
                //SteamNetworking.SendP2PPacket(friend.Id, byteString);
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes("stopcrouch"));
                //}
            }
        }
        UnityEngine.Debug.Log("post stop end");
    }

    [HarmonyPatch("KillPlayer")]
    [HarmonyPrefix]
    private static bool PreKillPlayer()
    {
        UnityEngine.Debug.Log("dead prefix");
        if (Load.CurrentLobby.Id != 0)
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes("lebronjamesdie"));
            }
        }
        if (Game.Instance.done)
        {
            return false;
        }
        CameraShaker.Instance.ShakeOnce(3f * GameState.Instance.cameraShake, 2f, 0.1f, 0.6f);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        UIManger.Instance.DeadUI(true);
        Timer.Instance.Stop();
        GameObject.Find("Player").GetComponent<Rigidbody>().constraints = RigidbodyConstraints.FreezeAll;
        if (Load.CurrentLobby.GetData("twod") == "True")
        {
            foreach (DetectWeapons dw in GameObject.FindObjectsOfType<DetectWeapons>())
            {
                dw.Throw(Vector3.down);
            }
        }
        return false;
    }

    [HarmonyPatch("StartGrab")]
    [HarmonyPostfix]
    private static void PostGrabStart()
    {
        //UnityEngine.Debug.Log("grab start postfix");
        Rigidbody rb = null;
        // this was made by mr. gpt
        var targetClass = typeof(PlayerMovement);
        var instance = GameObject.FindObjectOfType(targetClass);

        if (instance != null)
        {
            FieldInfo privateField = targetClass.GetField("objectGrabbing", BindingFlags.NonPublic | BindingFlags.Instance);

            if (privateField != null)
            {
                Rigidbody privateRigidbody = privateField.GetValue(instance) as Rigidbody;

                if (privateRigidbody != null)
                {
                    Console.WriteLine("rb: " + privateRigidbody.gameObject.name);
                    rb = privateRigidbody;
                }
                else
                {
                    if (rb != null)
                    {
                        rb = null;
                    }
                }
            }
            else
            {
                Console.WriteLine("failed to find field");
            }
        }
        else
        {
            Console.WriteLine("class not found");
        }
    }

    [HarmonyPatch("HoldGrab")]
    [HarmonyPostfix]
    private static void PostGrab()
    {
        //UnityEngine.Debug.Log("grab postfix");
        Rigidbody rb = null;
        var targetClass = typeof(PlayerMovement);
        var instance = GameObject.FindObjectOfType(targetClass);

        if (instance != null)
        {
            FieldInfo privateField = targetClass.GetField("objectGrabbing", BindingFlags.NonPublic | BindingFlags.Instance);

            if (privateField != null)
            {
                Rigidbody privateRigidbody = privateField.GetValue(instance) as Rigidbody;

                if (privateRigidbody != null)
                {
                    rb = privateRigidbody;
                }
                else
                {
                    if (rb != null)
                    {
                        rb = null;
                    }
                }
            }
            else
            {
                Console.WriteLine("failed to find field");
            }
        }
        else
        {
            Console.WriteLine("class not found");
        }
        if (rb != null)
        {
            if (Load.CurrentLobby.Id != 0)
            {
                foreach (Friend friend in Load.CurrentLobby.Members)
                {
                    SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"grabobj|{rb.gameObject.transform.position.x}|{rb.gameObject.transform.position.y}|{rb.gameObject.transform.position.z}|{rb.gameObject.name}|{rb.gameObject.transform.root.name}"));
                }
            }
        }
    }

    [HarmonyPatch("KillEnemy")]
    [HarmonyPostfix]
    private static void PostKillEnemy(Collision other)
    {
        UnityEngine.Debug.Log("enemy deaadd postfix");
        if (Load.CurrentLobby.Id != 0)
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                //if (!friend.IsMe)
                //{
                //SteamNetworking.SendP2PPacket(friend.Id, byteString);
                if (PlayerMovement.Instance.grounded && PlayerMovement.Instance.IsCrouching())
                {
                    SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"killenemy|crouch|{other.gameObject.name}|{PlayerMovement.Instance.rb.velocity.x}|{PlayerMovement.Instance.rb.velocity.y}|{PlayerMovement.Instance.rb.velocity.z}|{other.gameObject.transform.root.gameObject.name}"));
                }
                else
                {
                    SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"killenemy|gun|{other.gameObject.name}|{PlayerMovement.Instance.rb.velocity.x}|{PlayerMovement.Instance.rb.velocity.y}|{PlayerMovement.Instance.rb.velocity.z}|{other.gameObject.transform.root.gameObject.name}"));
                }
                //}
            }
        }
    }

    [HarmonyPatch("Respawn")]
    [HarmonyPostfix]
    private static void PostRespawn()
    {
        UnityEngine.Debug.Log("post respawn");
        if (Load.CurrentLobby.Id != 0)
        {
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                //if (!friend.IsMe)
                //{
                //SteamNetworking.SendP2PPacket(friend.Id, byteString);
                SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes("okirespawn"));
                //}
            }
        }
    }
}
