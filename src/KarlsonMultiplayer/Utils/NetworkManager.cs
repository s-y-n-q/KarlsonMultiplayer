using EZCameraShake;
using KarlsonMultiplayer.Players;
using Steamworks;
using Steamworks.Data;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using static UnityEngine.Rendering.PostProcessing.HistogramMonitor;

namespace KarlsonMultiplayer.Utils
{
    public class NetworkManager : MonoBehaviour
    {
        static float ltD;
        public static void CreateLobby()
        {
            SteamMatchmaking.CreateLobbyAsync(10).ContinueWith(lobbyResult =>
            {
                if (lobbyResult.Result.HasValue)
                {
                    Console.WriteLine($"created w/ id {lobbyResult.Result.Value.Id}");
                }
                else
                {
                    Console.WriteLine("failed to create");
                }
            });
        }

        public static void DataRecieved()
        {
            while (SteamNetworking.IsP2PPacketAvailable())
            {
                var packet = SteamNetworking.ReadP2PPacket();
                if (packet.HasValue)
                {
                    //Console.WriteLine("PACKET!!!");
                    HandleOpponentDataPacket(packet.Value.Data, PlayerManager.GetFriendFromID(packet.Value.SteamId));
                }
            }
        }

        static void HandleOpponentDataPacket(byte[] dataPacket, Friend id)
        {
            try
            {
                if (!id.IsMe)
                {
                    string opponentDataSent = ConvertByteArrayToString(dataPacket);
                    if (opponentDataSent.StartsWith("currentmap"))
                    {
                        if (SceneManager.GetActiveScene().name != opponentDataSent.Replace("currentmap", "").Trim())
                        {
                            SceneManager.LoadScene(opponentDataSent.Replace("currentmap", "").Trim());
                        }
                        Console.WriteLine("map saent:: " + opponentDataSent.Replace("currentmap", "").Trim());
                    }
                    if (opponentDataSent.StartsWith("position"))
                    {
                        string[] posarray = opponentDataSent.Split('|');
                        //Console.WriteLine(id.Name + " | pos sent:: " + opponentDataSent);
                        foreach (Friend friend in Load.CurrentLobby.Members)
                        {
                            foreach (Player plr in PlayerManager.PlayersClient)
                            {
                                if (plr.LinkedPlayer == id.Id)
                                {
                                    //Console.WriteLine(plr.LinkedPlayer);
                                    plr.PlayerObject.transform.position = new Vector3(float.Parse(posarray[1]), float.Parse(posarray[2]), float.Parse(posarray[3]));
                                    plr.PlayerObject.transform.rotation = new Quaternion(float.Parse(posarray[4]), float.Parse(posarray[5]), float.Parse(posarray[6]), float.Parse(posarray[7]));
                                }
                            }
                        }
                    }
                    if (opponentDataSent.StartsWith("startcrouch"))
                    {
                        foreach (Friend friend in Load.CurrentLobby.Members)
                        {
                            foreach (Player plr in PlayerManager.PlayersClient)
                            {
                                if (plr.LinkedPlayer == id.Id)
                                {
                                    //Console.WriteLine(plr.LinkedPlayer);
                                    plr.PlayerObject.transform.localScale = new Vector3(1f, 0.5f, 1f);
                                }
                            }
                        }
                    }
                    if (opponentDataSent.StartsWith("stopcrouch"))
                    {
                        foreach (Friend friend in Load.CurrentLobby.Members)
                        {
                            foreach (Player plr in PlayerManager.PlayersClient)
                            {
                                if (plr.LinkedPlayer == id.Id)
                                {
                                    //Console.WriteLine(plr.LinkedPlayer);
                                    plr.PlayerObject.transform.localScale = new Vector3(1f, 1.5f, 1f);
                                }
                            }
                        }
                    }
                    if (opponentDataSent.StartsWith("pickup"))
                    {
                        foreach (Friend friend in Load.CurrentLobby.Members)
                        {
                            foreach (Player plr in PlayerManager.PlayersClient)
                            {
                                //if (plr.LinkedPlayer == id.Id)
                                // {
                                //Console.WriteLine(plr.LinkedPlayer);
                                Console.WriteLine(opponentDataSent);
                                plr.CurrentWeapon = opponentDataSent.Replace("pickup", "").Trim();
                                plr.WeaponObject = GameObject.Find(opponentDataSent.Replace("pickup", "").Trim());
                                plr.WeaponObject.transform.parent = plr.WeaponHolderObject.transform;
                                plr.WeaponObject.transform.localRotation = Quaternion.identity;
                                plr.WeaponObject.transform.localPosition = plr.WeaponHolderObject.transform.localPosition;
                                plr.WeaponObject.layer = 10;
                                plr.WeaponObject.transform.Rotate(0, 180, 0);
                                Destroy(plr.WeaponObject.GetComponent<Rigidbody>());
                            // }
                            }
                        }
                    }
                    if (opponentDataSent.StartsWith("throw"))
                    {
                        string[] whodied = opponentDataSent.Split('|');
                        foreach (Friend friend in Load.CurrentLobby.Members)
                        {
                            foreach (Player plr in PlayerManager.PlayersClient)
                            {
                                if (plr.LinkedPlayer == id.Id && !id.IsMe)
                                {
                                    PlayerManager.ThrowGun(new Vector3(float.Parse(whodied[1]), float.Parse(whodied[2]), float.Parse(whodied[3])), plr);
                                }
                            }
                        }
                    }
                    if (opponentDataSent.StartsWith("shootgun"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        //Console.WriteLine(opponentDataSent);
                        Vector3 bulletthing = new Vector3(float.Parse(thestuff[1]), float.Parse(thestuff[2]), float.Parse(thestuff[3]));
                        foreach (Player plr in PlayerManager.PlayersClient)
                        {
                            if (plr.LinkedPlayer == id.Id && !id.IsMe)
                            {
                                if (plr.WeaponObject != null)
                                {
                                    //plr.WeaponObject.GetComponent<RangedWeapon>().readyToUse = true;
                                    plr.WeaponObject.GetComponent<RangedWeapon>().pickedUp = true;
                                    plr.WeaponObject.GetComponent<RangedWeapon>().PickupWeapon(true);
                                    PlayerManager.SendProjectile(bulletthing, plr);
                                }
                                else
                                {
                                    Console.WriteLine("plrs weapon is null bruh");
                                }
                            }
                        }
                        //Console.WriteLine($" {id.Name} | shoot gun recieved, " + bulletthing.ToString());
                    }
                    if (opponentDataSent.StartsWith("lebronjamesdie"))
                    {
                        string[] whodied = opponentDataSent.Split('|');
                        foreach (Friend friend in Load.CurrentLobby.Members)
                        {
                            foreach (Player plr in PlayerManager.PlayersClient)
                            {
                                if (plr.LinkedPlayer == id.Id)
                                {
                                    plr.NameTagObject.GetComponent<TextMesh>().text = "<color=red><size=20>DEAD</size></color>\n" + plr.Name;
                                }
                            }
                        }
                    }
                    if (opponentDataSent.StartsWith("okirespawn"))
                    {
                        foreach (Friend friend in Load.CurrentLobby.Members)
                        {
                            foreach (Player plr in PlayerManager.PlayersClient)
                            {
                                //if (plr.LinkedPlayer == id.Id)
                                // {
                                //Console.WriteLine(plr.LinkedPlayer);
                                Console.WriteLine(opponentDataSent);
                                if (plr.LinkedPlayer == id.Id)
                                {
                                    plr.NameTagObject.GetComponent<TextMesh>().text = plr.Name;
                                }
                                // }
                            }
                        }
                    }
                    if (opponentDataSent.StartsWith("killenemy"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        Console.WriteLine(opponentDataSent);
                        if (thestuff[1] == "crouch")
                        {
                            Vector3 velo = new Vector3(float.Parse(thestuff[3]), float.Parse(thestuff[4]), float.Parse(thestuff[5]));
                            GameObject objectweneed = null;
                            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                            {
                                if (go.name == thestuff[2] && go.transform.root.gameObject.name == thestuff[6])
                                {
                                    objectweneed = go;
                                    break;
                                }
                            }
                            if (objectweneed != null)
                            {
                            }
                            else
                            {
                                return;
                            }
                            //Instantiate<GameObject>(PrefabManager.Instance.enemyHitAudio, GameObject.Find(thestuff[2]).GetComponent<Collision>().contacts[0].point, Quaternion.identity);
                            RagdollController rc = objectweneed.transform.root.GetComponent<RagdollController>();
                            if (rc != null)
                            {
                                Console.WriteLine("ragdoll isnt null");
                            }
                            else
                            {
                                Console.WriteLine("it is null what the heck");
                                return;
                            }
                            if (thestuff[1] == "crouch")
                            {
                                rc.MakeRagdoll(velo * 1.2f * 34f);
                            }
                            else
                            {
                                rc.MakeRagdoll(velo.normalized * 250f);
                            }
                            objectweneed.transform.root.gameObject.GetComponent<Enemy>().DropGun(velo.normalized * 2f);
                        }
                        if (thestuff[1] == "gun")
                        {
                            GameObject objectweneed = null;
                            foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                            {
                                if (go.name == thestuff[2] && go.transform.root.gameObject.name == thestuff[3])
                                {
                                    objectweneed = go;
                                    break;
                                }
                            }
                            if (objectweneed != null)
                            {
                            }
                            else
                            {
                                return;
                            }
                            //Instantiate<GameObject>(PrefabManager.Instance.enemyHitAudio, GameObject.Find(thestuff[2]).GetComponent<Collision>().contacts[0].point, Quaternion.identity);
                            RagdollController rc = objectweneed.transform.root.GetComponent<RagdollController>();
                            if (rc != null)
                            {
                                Console.WriteLine("ragdoll isnt null");
                            }
                            else
                            {
                                Console.WriteLine("it is null what the heck");
                                return;
                            }
                            rc.MakeRagdoll(-Vector3.down * 350f);
                            if (objectweneed.GetComponent<Rigidbody>())
                            {
                                objectweneed.gameObject.GetComponent<Rigidbody>().AddForce(-Vector3.down * 1500f);
                            }
                            objectweneed.transform.root.GetComponent<Enemy>().DropGun(Vector3.up);
                        }
                    }
                    if (opponentDataSent.StartsWith("grabobj"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        //Console.WriteLine(opponentDataSent);
                        Vector3 objpos = new Vector3(float.Parse(thestuff[1]), float.Parse(thestuff[2]), float.Parse(thestuff[3]));
                        GameObject objectweneed = null;
                        foreach (GameObject go in GameObject.FindObjectsOfType<GameObject>())
                        {
                            if (go.name == thestuff[4] && go.transform.root.gameObject.name == thestuff[5])
                            {
                                objectweneed = go;
                                break;
                            }
                        }
                        if (objectweneed != null)
                        {
                        }
                        else
                        {
                            return;
                        }
                        //Instantiate<GameObject>(PrefabManager.Instance.enemyHitAudio, GameObject.Find(thestuff[2]).GetComponent<Collision>().contacts[0].point, Quaternion.identity);
                        objectweneed.transform.position = objpos;
                    }
                    if (opponentDataSent.StartsWith("breakglass"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        Console.WriteLine(opponentDataSent);
                        GameObject glassobj = GameObject.Find(thestuff[1]);
                        if (glassobj == null)
                        {
                            Console.WriteLine("glass is null");
                            return;
                        }
                        Instantiate<GameObject>(glassobj.GetComponent<Glass>().glassSfx, glassobj.transform.position, Quaternion.identity);
                        glassobj.GetComponent<Glass>().glass.SetActive(true);
                        glassobj.GetComponent<Glass>().glass.transform.parent = null;
                        glassobj.GetComponent<Glass>().glass.transform.localScale = Vector3.one;
                        Destroy(glassobj);
                        Console.WriteLine($"broke {glassobj.name}");
                    }
                    if (opponentDataSent.StartsWith("breakdoor"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        Console.WriteLine(opponentDataSent);
                        GameObject doorobj = GameObject.Find(thestuff[1]);
                        if (doorobj == null)
                        {
                            Console.WriteLine("door is null");
                            return;
                        }
                        Vector3 vector = GameObject.Find("Player").GetComponent<Rigidbody>().velocity;
                        float magnitude = vector.magnitude;
                        if (magnitude > 20f)
                        {
                            float num = magnitude / 20f;
                            vector /= num;
                        }
                        Rigidbody[] componentsInChildren = Instantiate<GameObject>(doorobj.GetComponent<Break>().replace, doorobj.transform.position, doorobj.transform.rotation).GetComponentsInChildren<Rigidbody>();
                        for (int i = 0; i < componentsInChildren.Length; i++)
                        {
                            componentsInChildren[i].velocity = vector * 1.5f;
                        }
                        Instantiate<GameObject>(PrefabManager.Instance.destructionAudio, doorobj.transform.position, Quaternion.identity);
                        Destroy(doorobj);
                        Console.WriteLine($"broke {doorobj.name}");
                    }
                    if (opponentDataSent.StartsWith("explodeb"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        Console.WriteLine(opponentDataSent);
                        GameObject barrelobj = GameObject.Find(thestuff[1]);
                        GameObject defaultbarobj = null;
                        if (barrelobj == null)
                        {
                            Console.WriteLine("barrel is null");
                            return;
                        }
                        foreach (GameObject obj in GameObject.FindObjectsOfType<GameObject>())
                        {
                            if (obj.name == "default" && obj.transform.root.gameObject.name == barrelobj.name)
                            {
                                defaultbarobj = obj;
                            }
                        }
                        Instantiate<GameObject>(PrefabManager.Instance.explosion, defaultbarobj.transform.position, Quaternion.identity);
                        Destroy(defaultbarobj);
                        Console.WriteLine($"exploded barlrl {barrelobj.name}");
                    }
                    if (opponentDataSent.StartsWith("grapple"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        foreach (Player plr in PlayerManager.PlayersClient)
                        {
                            if (plr.LinkedPlayer == id.Id && !id.IsMe)
                            {
                                PlayerManager.SendGrappler(new Vector3(float.Parse(thestuff[1]), float.Parse(thestuff[2]), float.Parse(thestuff[3])), plr);
                            }
                        }
                    }
                    if (opponentDataSent.StartsWith("spawngun"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        Console.WriteLine(opponentDataSent);
                        Vector3 thepos = new Vector3(float.Parse(thestuff[2]), float.Parse(thestuff[3]), float.Parse(thestuff[4]));
                        if (thestuff[1] == "pistol")
                        {
                            GameObject theobject = Instantiate(PrefabManager1.pistol, thepos, Quaternion.identity);
                            System.Random random = new System.Random();
                            theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                            theobject.SetActive(true);
                        }
                        if (thestuff[1] == "ak47")
                        {
                            GameObject theobject = Instantiate(PrefabManager1.ak47, thepos, Quaternion.identity);
                            System.Random random = new System.Random();
                            theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                            theobject.SetActive(true);
                        }
                        if (thestuff[1] == "boomer")
                        {
                            GameObject theobject = Instantiate(PrefabManager1.boomer, thepos, Quaternion.identity);
                            System.Random random = new System.Random();
                            theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                            theobject.SetActive(true);
                        }
                        if (thestuff[1] == "grappler")
                        {
                            GameObject theobject = Instantiate(PrefabManager1.grappler, thepos, Quaternion.identity);
                            System.Random random = new System.Random();
                            theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                            theobject.SetActive(true);
                        }
                        if (thestuff[1] == "shotgun")
                        {
                            GameObject theobject = Instantiate(PrefabManager1.shotgun, thepos, Quaternion.identity);
                            System.Random random = new System.Random();
                            theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                            theobject.SetActive(true);
                        }
                        Console.WriteLine("spawned");
                    }
                    // deathstuffnow
                    if (opponentDataSent.StartsWith("initdeadtext"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        Console.WriteLine(opponentDataSent);
                        if (PlayerManager.GetPlayerFromName(thestuff[1]) != null && PlayerManager.GetPlayerFromName(thestuff[1]).LinkedPlayer != SteamClient.SteamId)
                        {
                            PlayerManager.GetPlayerFromName(thestuff[1]).NameTagObject.GetComponent<TextMesh>().text = "<color=red><size=20>DEAD</size></color>\n" + PlayerManager.GetPlayerFromName(thestuff[1]).Name;
                        }
                    }
                    if (opponentDataSent.StartsWith("initdeath"))
                    {
                        string[] thestuff = opponentDataSent.Split('|');
                        Console.WriteLine(opponentDataSent);
                        PlayerMovement.Instance.KillPlayer();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Failed to process incoming opponent data packet : " + ex.ToString());
            }
        }

        public static byte[] ConvertStringToByteArray(string byteArrayToConvert)
        {
            return System.Text.Encoding.UTF8.GetBytes(byteArrayToConvert);
        }

        public static string ConvertByteArrayToString(byte[] byteArrayToConvert)
        {
            return System.Text.Encoding.UTF8.GetString(byteArrayToConvert);
        }

    }
}
