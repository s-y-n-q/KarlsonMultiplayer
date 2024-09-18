using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KarlsonMultiplayer.Players
{
    public class LocalPlayerColliders : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            //Console.WriteLine("plr collision: " + other.gameObject.name);
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

            if (other.gameObject.name.StartsWith("Bullet"))
            {
                if (Load.CurrentLobby.Id != 0)
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"lebronjamesdie"));
                    }
                }
                PlayerMovement.Instance.KillPlayer();
                Console.WriteLine("i got shoot");
            }

            if (other.gameObject.name.StartsWith("BulletHit"))
            {
                if (Load.CurrentLobby.Id != 0)
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"lebronjamesdie"));
                    }
                }
                PlayerMovement.Instance.KillPlayer();
                Console.WriteLine("i got shoot");
            }
        }

        private void OnCollisionEnter(Collision other)
        {
            if (other.gameObject.name.StartsWith("Door"))
            {
                if (PlayerMovement.Instance.IsCrouching())
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

            if (other.gameObject.name.StartsWith("Bullet"))
            {
                if (Load.CurrentLobby.Id != 0)
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"lebronjamesdie"));
                    }
                }
                PlayerMovement.Instance.KillPlayer();
                Console.WriteLine("i got shoot");
            }

            if (other.gameObject.name.StartsWith("BulletHit"))
            {
                if (Load.CurrentLobby.Id != 0)
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"lebronjamesdie"));
                    }
                }
                PlayerMovement.Instance.KillPlayer();
                Console.WriteLine("i got shoot");
            }
        }
    }
}
