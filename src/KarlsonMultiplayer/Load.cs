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
using System.IO;

namespace KarlsonMultiplayer
{
    [BepInPlugin("synq.karlson.multiplayer", "Karlson Multiplayer", "1.1.0")]
    [BepInDependency("synq.karlson.karlib", BepInDependency.DependencyFlags.HardDependency)]
    public class Load : BaseUnityPlugin
    {
        bool fetchinglobbies;
        Steamworks.ServerList.Internet f = new Steamworks.ServerList.Internet();
        List<Steamworks.Data.ServerInfo> serverList = new List<Steamworks.Data.ServerInfo>();
        string searhc;
        SteamId FAGRT = new SteamId();

        Texture2D mainTex;

        UnityEngine.Color fat;
        UnityEngine.Color divider;

        public static string currentweapon;

        public static Steamworks.Data.Lobby CurrentLobby;

        static bool showsettings;

        static bool spawners;

        static bool uiopen;

        private Vector2 scrollPosition = Vector2.zero;

        void Awake()
        {
            Init();
        }

        public static UnityEngine.Color HexToColor(string hex)
        {
            if (ColorUtility.TryParseHtmlString(hex, out UnityEngine.Color color))
            {
                return color;
            }
            else
            {
                return UnityEngine.Color.white; // Default color if parsing fails
            }
        }

        string host = "";
        string players = "";

        void Init()
        {
            SceneManager.sceneLoaded += this.OnSceneLoaded;
            Harmony harm = new Harmony("synq.karlson.multiplayer");
            harm.PatchAll(Assembly.GetExecutingAssembly());

            fat = HexToColor("#141414");
            divider = HexToColor("#595959");

            SteamMatchmaking.OnLobbyEntered += OnLobbyEntered;
            SteamMatchmaking.OnLobbyMemberJoined += OnLobbyMemberJoined;
            SteamMatchmaking.OnLobbyMemberDisconnected += OnLobbyMemberDisconnected;

            SteamMatchmaking.OnLobbyDataChanged += OnLobbyDataChanged;

            SteamClient.Init(480);

            f.OnChanges += OnServersUpdated;

            //NetworkManager.Init();

            // reqlobby();

            StartCoroutine(packetcheck());

            KarlLib.Backend.Section main = KarlLib.Library.AddSection("Karlson Multiplayer", () => Console.WriteLine("hi"));
            main.AddButton("Create Server", () => NetworkManager.CreateLobby());
            main.AddButton("Join Server ID From Clipboard", () => joinlobby(GUIUtility.systemCopyBuffer));
            main.AddButton("Copy Server ID", () => GUIUtility.systemCopyBuffer = CurrentLobby.Id.ToString());
            main.AddButton("Leave", () => CurrentLobby.Leave());
            main.AddLabel("Spawners");
            main.AddButton("Spawn Pistol", () => spawnitem(0));
            main.AddButton("Spawn Boomer", () => spawnitem(1));
            main.AddButton("Spawn Shotgun", () => spawnitem(2));
            main.AddButton("Spawn Grappler", () => spawnitem(3)); 
            main.AddButton("Spawn Uzi", () => spawnitem(4));
            main.AddLabel("Server");
            main.AddLabel("<color=#555555><size=12>Settings</size></color>");
            main.AddToggle("Reset Scene On Death", (state) =>
            {
                Console.WriteLine(state.ToString());
                Globals.Settings.ResetSceneOnDeath = state;
            });

            main.AddToggle("Throw Weapons On Death", (state) =>
            {
                Console.WriteLine(state.ToString());
                Globals.Settings.ThrowWeaponsOnDeath = state;
            });

            base.Logger.LogInfo($"init with {SteamClient.AppId}");
        }

        // stuff

        void spawnitem(int id)
        {
            System.Random random = new System.Random();
            switch (id)
            {
                case 0:
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|pistol|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobject = Instantiate(PrefabManager1.pistol, GameObject.Find("Player").transform.position, Quaternion.identity);
                    theobject.name = $"{theobject.name} ({random.Next(10000, 100000)})";
                    theobject.SetActive(true);
                    break;
                case 1:
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|boomer|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobjecta = Instantiate(PrefabManager1.boomer, GameObject.Find("Player").transform.position, Quaternion.identity);
                    theobjecta.name = $"{theobjecta.name} ({random.Next(10000, 100000)})";
                    theobjecta.SetActive(true);
                    break;
                case 2:
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|shotgun|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobjectb = Instantiate(PrefabManager1.shotgun, GameObject.Find("Player").transform.position, Quaternion.identity);
                    theobjectb.name = $"{theobjectb.name} ({random.Next(10000, 100000)})";
                    theobjectb.SetActive(true);
                    break;
                case 3:
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|grappler|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobjectc = Instantiate(PrefabManager1.grappler, GameObject.Find("Player").transform.position, Quaternion.identity);
                    theobjectc.name = $"{theobjectc.name} ({random.Next(10000, 100000)})";
                    theobjectc.SetActive(true);
                    break;
                case 4:
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"spawngun|ak47|{GameObject.Find("Player").transform.position.x}|{GameObject.Find("Player").transform.position.y}|{GameObject.Find("Player").transform.position.z}"));
                    }
                    GameObject theobjectd = Instantiate(PrefabManager1.ak47, GameObject.Find("Player").transform.position, Quaternion.identity);
                    theobjectd.name = $"{theobjectd.name} ({random.Next(10000, 100000)})";
                    theobjectd.SetActive(true);
                    break;
            }
        }

        void joinlobby(string id)
        {
            if (ulong.TryParse(id, out ulong lobbyId))
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
        }

        public void GUIButtonLayout(string name, UnityEngine.Color color, Action action, float width, float height)
        {
            // Make the background transparent and set the content color to white
            GUI.backgroundColor = new Color32(0, 0, 0, 0);
            GUI.contentColor = UnityEngine.Color.white;

            // Create a GUILayout button with specific dimensions
            if (GUILayout.Button("", GUILayout.Width(width), GUILayout.Height(height)))
            {
                action();
            }

            // Reset background color and draw custom outline
            GUI.backgroundColor = UnityEngine.Color.white;

            // Get the rect of the last GUILayout element (the button we just made)
            Rect rect = GUILayoutUtility.GetLastRect();

            // Draw the custom button using your provided `Draw` method
            Draw(rect, color, 8f);

            // Transparent background and content color reset again
            GUI.backgroundColor = new Color32(0, 0, 0, 0);
            GUI.contentColor = UnityEngine.Color.white;

            // Use GUI.Button to draw the button text
            if (GUI.Button(rect, name))
            {
                action();
            }
        }


        private void LoadEmbeddedDLL(string dllName)
        {
            try
            {
                var assembly = Assembly.GetExecutingAssembly();
                using (Stream stream = assembly.GetManifestResourceStream(dllName))
                {
                    if (stream == null)
                    {
                        Logger.LogError($"Failed to find embedded DLL: {dllName}");
                        return;
                    }

                    byte[] assemblyData = new byte[stream.Length];
                    stream.Read(assemblyData, 0, assemblyData.Length);

                    Assembly.Load(assemblyData);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
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
            if (Input.GetKeyDown(KeyCode.Insert))
            {
                Console.WriteLine("pressed insrt");
                uiopen = !uiopen;
                Console.WriteLine("ui is : " + uiopen);
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

        public void Draw(Rect size, UnityEngine.Color color, float r)
        {
            for (int i = 0; i < 5; i++)
                GUI.DrawTexture(
                    size,
                    mainTex,
                    (ScaleMode)(0 + Type.EmptyTypes.Length),
                    Type.EmptyTypes.Length != 0,
                    0f,
                    color,
                    0f,
                    r
                );
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
