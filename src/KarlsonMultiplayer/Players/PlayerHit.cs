using EZCameraShake;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KarlsonMultiplayer.Players
{
    public class PlayerHit : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            if (collision.gameObject.name == "Bullet(Clone)")
            {
                if (Load.CurrentLobby.Id != 0)
                {
                    foreach (Friend friend in Load.CurrentLobby.Members)
                    {
                        SteamNetworking.SendP2PPacket(friend.Id, Encoding.UTF8.GetBytes($"lebronjamesdie"));
                    }
                }
                PlayerMovement.Instance.KillPlayer();
                Console.WriteLine("i ghot shooot.");
            }
        }
    }
}
