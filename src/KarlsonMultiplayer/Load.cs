using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using BepInEx;
using Steamworks;
using HarmonyLib;
using System.Reflection;
using UnityEngine.SceneManagement;
using KarlsonMultiplayer.Utils;
using static UnityEngine.Application;
using Steamworks.Data;
using System.Windows.Forms;
using KarlsonMultiplayer.Players;
using Steamworks.ServerList;
using TMPro;
using System.Collections;

namespace KarlsonMultiplayer
{
    [BepInPlugin("synq.karlson.multiplayer", "Karlson Multiplayer", "1.0.0")]
    public class Load : BaseUnityPlugin
    {
        bool fetchinglobbies;
        Steamworks.ServerList.Internet f = new Steamworks.ServerList.Internet();
        List<Steamworks.Data.ServerInfo> serverList = new List<Steamworks.Data.ServerInfo>();
        string searhc;
        SteamId FAGRT = new SteamId();

        public static string currentweapon;

        public static Steamworks.Data.Lobby CurrentLobby;

        static bool showsettings;

        static bool spawners;

        void Awake()
        {
            Init();
        }

        void Init()
        {
            SceneManager.sceneLoaded += this.OnSceneLoaded;
            Harmony harm = new Harmony("synq.karlson.multiplayer");
            harm.PatchAll(Assembly.GetExecutingAssembly());

            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;

            SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataChanged;

            SteamClient.Init(480);

            f.OnChanges += OnServersUpdated;

            //NetworkManager.Init();

            // reqlobby();

            StartCoroutine(packetcheck());

            base.Logger.LogInfo($"init with {SteamClient.AppId}");
        }

        // stuff

        private void OnLobbyDataChanged(Steamworks.Data.Lobby lobby)
        {
            Console.WriteLine("--- settings ---");
            Console.WriteLine($"throw weapons on death : {lobby.GetData("twod")}");
            Console.WriteLine($"reset scene on death : {lobby.GetData("rsod")}");
        }

        private void OnLobbyEntered(Steamworks.Data.Lobby lobby)
        {
            Console.WriteLine($"joined: {lobby.Id}");
            SteamNetworking.AllowP2PPacketRelay(true);
            foreach (Friend frind in CurrentLobby.Members)
            {
                SteamNetworking.AcceptP2PSessionWithUser(frind.Id);
            }
            //PlayerManager.Spawn(PlayerManager.GetFriendFromID(SteamClient.SteamId), "dave meatball", new Vector3(0f, -1.8f, 0f));
            if (lobby.Members.Count() >= 2)
            {
                foreach (Friend frind in lobby.Members)
                {
                    if (!frind.IsMe)
                    {
                        PlayerManager.Spawn(frind, frind.Name, Globals.GetSpawnPointFromMap(SceneManager.GetActiveScene().name));
                    }
                }
            }    
            CurrentLobby = lobby;
        }

        void OnLobbyMemberDisconnected(Steamworks.Data.Lobby lobby, Friend member)
        {
            CurrentLobby = lobby;
            Console.WriteLine($"left: {member.Name}");
            foreach (Player plr in PlayerManager.PlayersClient)
            {
                if (plr.LinkedPlayer == member.Id)
                {
                    Destroy(plr.PlayerObject);
                    PlayerManager.PlayersClient.Remove(plr);
                    PlayerManager.PlayersFriend.Remove(member);
                }
            }
            PlayerManager.ThrowGun(PlayerManager.GetPlayerFromID(member.Id).PlayerObject.transform.position, PlayerManager.GetPlayerFromID(member.Id));
        }

        private void OnLobbyMemberJoined(Steamworks.Data.Lobby lobby, Friend member)
        {
            Console.WriteLine($"plr joined: {member.Name}");

            if (!member.IsMe)
            {
                SteamNetworking.SendP2PPacket(member.Id, Encoding.UTF8.GetBytes("currentmap " + SceneManager.GetActiveScene().name));
            }

            PlayerManager.Spawn(member, member.Name, new Vector3(0f, -1.8f, 0f));

            CurrentLobby = lobby;
        }


        void OnGUI()
        {
            if (!PrefabManager1.didalready)
            {
                GUILayout.Label("Weapon prefabs are NOT synced!, load into tutorial to sync prefabs.");
            }
            if (GUILayout.Button("Create Server"))
            {
                NetworkManager.CreateLobby();
            }
            if (GUILayout.Button("Copy Server ID"))
            {
                GUIUtility.systemCopyBuffer = CurrentLobby.Id.ToString();
                Console.WriteLine($"fixed current id : " + CurrentLobby.Id);
            }
            if (GUILayout.Button("Leave"))
            {
                CurrentLobby.Leave();
                foreach (Player plr in PlayerManager.PlayersClient)
                {
                    PlayerManager.ThrowGun(plr.PlayerObject.transform.position, plr);
                    PlayerManager.PlayersClient.Remove(plr);
                }
            }
            searhc = GUILayout.TextField(searhc);
            if (GUILayout.Button("Join from ID"))
            {
                if (ulong.TryParse(searhc, out ulong lobbyId))
                {
                    SteamId steamId = (SteamId)lobbyId;
                    SteamMatchmaking.JoinLobbyAsync(steamId).ContinueWith(task =>
                    {
                        if (task.IsCompleted)
                        {
                            Console.WriteLine("joined");
                        }
                        else
                        {
                            Console.WriteLine("failed 2 join loby");
                        }
                    });
                }
                else
                {
                    Console.WriteLine("invalid loby id string");
                }
            }
            if (CurrentLobby.Id != 0)
            {
                GUILayout.Label($"Host: {CurrentLobby.Owner.Name}");
            }
            if (CurrentLobby.Id != 0)
            {
                GUILayout.Label($"Player Count: {CurrentLobby.MemberCount}");
            }
            if (CurrentLobby.IsOwnedBy(SteamClient.SteamId))
            {
                showsettings = GUILayout.Toggle(showsettings, "Host Settings");
                if (showsettings)
                {
                    Globals.Settings.ThrowWeaponsOnDeath = GUILayout.Toggle(Globals.Settings.ThrowWeaponsOnDeath, "Throw Weapons On Death");
                    Globals.Settings.ResetSceneOnDeath = GUILayout.Toggle(Globals.Settings.ResetSceneOnDeath, "Reset Levels On Death");
                }
            }
            spawners = GUILayout.Toggle(spawners, "Spawners");
            if (spawners)
            {
                GUILayout.Label("Weapon Spawners");
                if (GUILayout.Button("Spawn Pistol"))
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|pistol|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobject = Instantiate(PrefabManager1.pistol, GameObject.Find("Player").transform.position, Quaternion.identity);
                    System.Random random = new System.Random();
                    theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                    theobject.SetActive(true);
                }
                if (GUILayout.Button("Spawn Boomer"))
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|boomer|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobject = Instantiate(PrefabManager1.boomer, GameObject.Find("Player").transform.position, Quaternion.identity);
                    System.Random random = new System.Random();
                    theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                    theobject.SetActive(true);
                }
                if (GUILayout.Button("Spawn Shotgun"))
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|shotgun|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobject = Instantiate(PrefabManager1.shotgun, GameObject.Find("Player").transform.position, Quaternion.identity);
                    System.Random random = new System.Random();
                    theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                    theobject.SetActive(true);
                }
                if (GUILayout.Button("Spawn Grappler"))
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|grappler|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobject = Instantiate(PrefabManager1.grappler, GameObject.Find("Player").transform.position, Quaternion.identity);
                    System.Random random = new System.Random();
                    theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                    theobject.SetActive(true);
                }
                if (GUILayout.Button("Spawn Uzi"))
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|ak47|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobject = Instantiate(PrefabManager1.ak47, GameObject.Find("Player").transform.position, Quaternion.identity);
                    System.Random random = new System.Random();
                    theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                    theobject.SetActive(true);
                }
            }
        }

        void OnServersUpdated()
        {
            if (f.Responsive.Count == 0)
                return;

            foreach (var s in f.Responsive)
            {
                serverList.Add(s);
            }

            f.Responsive.Clear();
        }

        bool MatchesSearchQuery(string serverName)
        {
            return string.IsNullOrEmpty(searhc) || serverName.IndexOf(searhc, System.StringComparison.OrdinalIgnoreCase) >= 0;
        }

        void Update()
        {
            if (CurrentLobby.Id != 0)
            {
                if (GameObject.Find("Player") != null)
                {
                    Vector3 position = GameObject.Find("Player").transform.position;
                    Quaternion rotation = GameObject.Find("Camera").transform.rotation;

                    byte[] byteString = Encoding.UTF8.GetBytes($"position|{position.x.ToString()}|{position.y.ToString()}|{position.z.ToString()}|{rotation.x.ToString()}|{rotation.y.ToString()}|{rotation.z.ToString()}|{rotation.w.ToString()}");

                    foreach (Friend friend in CurrentLobby.Members)
                    {
                        if (!friend.IsMe)
                        {
                            SteamNetworking.SendP2PPacket(friend.Id, byteString);
                        }
                    }
                }

                NetworkManager.DataRecieved();

                if (CurrentLobby.IsOwnedBy(SteamClient.SteamId))
                {
                    if (CurrentLobby.GetData("twod") != Globals.Settings.ThrowWeaponsOnDeath.ToString())
                    {
                        CurrentLobby.SetData("twod", Globals.Settings.ThrowWeaponsOnDeath.ToString());
                    }
                    if (CurrentLobby.GetData("rsod") != Globals.Settings.ResetSceneOnDeath.ToString())
                    {
                        CurrentLobby.SetData("rsod", Globals.Settings.ResetSceneOnDeath.ToString());
                    }
                }
            }
        }

        static IEnumerator packetcheck()
        {
            while (true)
            {
                if (CurrentLobby.Id != 0)
                {
                    foreach (Friend frind in CurrentLobby.Members)
                    {
                        SteamNetworking.AcceptP2PSessionWithUser(frind.Id);
                    }
                }
                Console.WriteLine("packets checked");
                yield return new WaitForSeconds(5f);
            }
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            Console.WriteLine("yeah");

            Game.Instance.EndGame();

            foreach (Friend frind in CurrentLobby.Members)
            {
                SteamNetworking.AcceptP2PSessionWithUser(frind.Id);
                Console.WriteLine("current:::::: " + frind.Id);
                SteamNetworking.SendP2PPacket(frind.Id, Encoding.UTF8.GetBytes("currentmap " + SceneManager.GetActiveScene().name));
            }

            if (CurrentLobby.IsOwnedBy(SteamClient.SteamId))
            {
                CurrentLobby.SetData("twod", Globals.Settings.ThrowWeaponsOnDeath.ToString());
                CurrentLobby.SetData("rsod", Globals.Settings.ResetSceneOnDeath.ToString());
            }

            if (scene.name == "6Escape2")
            {
                GameObject gaymen = Instantiate(GameObject.Find("Grappler"));
                gaymen.transform.position = new Vector3(-0.8187f, 1.1583f, 137.397f);
            }

            if (scene.name == "0Tutorial")
            {
                PrefabManager1.Init();
            }


            if (scene.name != "MainMenu")
            {
                PlayerManager.PlayersClient.Clear();
                foreach (Friend frind in CurrentLobby.Members)
                {
                    PlayerManager.Spawn(frind, frind.Name, Globals.GetSpawnPointFromMap(SceneManager.GetActiveScene().name));
                }
                GameObject.Find("Player").AddComponent<LocalPlayerColliders>();
                Game.Instance.StartGame();
            }

            if (scene.name == "MainMenu")
            {
                Game.Instance.EndGame();
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;

                GameObject.Find("UI/Always/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "KARLSON MULTIPLAYER (1.0.0)";
                GameObject.Find("UI/Always/Text (TMP)").GetComponent<TextMeshProUGUI>().fontSize = 22;

                GameObject credsbtn = PlayerManager.SpawnObject(Instantiate(GameObject.Find("UI/Menu/Options")));
                credsbtn.transform.position = new Vector3(-6.6021f, 9.5012f, 185.61f);
                credsbtn.name = "Credits";
                credsbtn.transform.parent = GameObject.Find("UI/Menu").transform;
                credsbtn.transform.localScale = new Vector3(1.1707f, 1.1707f, 1.1707f);
                GameObject.Find("UI/Menu/Credits/Text (TMP)").GetComponent<TextMeshProUGUI>().text = "CREDITS";

                GameObject.Find("UI/Menu/Map").SetActive(false);
            }
            base.Logger.LogInfo($"Scene changed to {scene.name}");
        }
    }
}
