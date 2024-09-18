using KarlsonMultiplayer.Utils;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace KarlsonMultiplayer.Players
{
    public class Player
    {
        public SteamId LinkedPlayer;
        public string Name;
        public GameObject PlayerObject;
        public GameObject WeaponObject;
        public GameObject WeaponHolderObject;
        public GameObject NameTagObject;
        public string CurrentWeapon;

        public void SendSpawn()
        {

        }

        public void SendSpawn(SteamId id)
        {

        }
    }
}
