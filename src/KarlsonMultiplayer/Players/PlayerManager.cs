using Audio;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ProBuilder;
using UnityEngine.UIElements;

namespace KarlsonMultiplayer.Players
{
    public class PlayerManager : MonoBehaviour
    {
        public static List<Player> PlayersClient = new List<Player>();
        public static List<Friend> PlayersFriend = new List<Friend>();
        public static Collider[] projectileColliders;
        public static Vector3 nearestPoint;
        public static Vector3 grapplePoint;
        public static SpringJoint joint;
        public static LineRenderer lr;
        public static Vector3 endPoint;
        public static float offsetMultiplier;
        public static float offsetVel;
        public static int positions = 100;
        public static Transform tip;

        public static void Spawn(Friend id, string username, Vector3 position)
        {
            if (!id.IsMe)
            {
                Player plr = new Player
                {
                    Name = username,
                    LinkedPlayer = id.Id,
                    PlayerObject = SpawnObject(GameObject.CreatePrimitive(PrimitiveType.Capsule))
                };
                plr.PlayerObject.transform.position = position;
                plr.PlayerObject.transform.localScale = new Vector3(1f, 1.5f, 1f);
                plr.PlayerObject.name = username;
                plr.PlayerObject.layer = 10;
                //plr.PlayerObject.AddComponent<PlayerHit>();
                //plr.PlayerObject.AddComponent<Rigidbody>();
                //Destroy(plr.PlayerObject.GetComponent<CapsuleCollider>());
                //plr.PlayerObject.AddComponent<DetectWeapons>();
                plr.WeaponHolderObject = new GameObject("weaphonholder");
                plr.WeaponHolderObject.transform.parent = plr.PlayerObject.transform;
                plr.WeaponHolderObject.transform.position = new Vector3(0.7f, -0.85f, 0f);
                plr.NameTagObject = SpawnObject(new GameObject("Text"));
                plr.NameTagObject.AddComponent<TextMesh>();
                plr.NameTagObject.GetComponent<TextMesh>().text = username;
                plr.NameTagObject.GetComponent<TextMesh>().alignment = TextAlignment.Center;
                plr.NameTagObject.GetComponent<TextMesh>().fontSize = 32;
                plr.NameTagObject.GetComponent<TextMesh>().characterSize = 0.1f;
                plr.NameTagObject.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
                plr.NameTagObject.transform.parent = plr.PlayerObject.transform;
                plr.NameTagObject.transform.localPosition = new Vector3(0f, 1.5f, 0f);
                plr.NameTagObject.transform.localRotation = Quaternion.Euler(0f, 180f, 0f);
                PlayersClient.Add(plr);
                PlayersFriend.Add(id);
            }
        }

        public static GameObject SpawnObject(GameObject obj, bool destroyOnLoad = false)
        {
            GameObject gameObject = Instantiate<GameObject>(obj);
            if (destroyOnLoad)
            {
                Object.DontDestroyOnLoad(gameObject);
            }
            return gameObject;
        }
        
        public static void ThrowGun(Vector3 dir, Player plr)
        {
            if (!plr.WeaponObject.GetComponent<Rigidbody>())
            {
                Rigidbody rigidbody = plr.WeaponObject.AddComponent<Rigidbody>();
                rigidbody.interpolation = RigidbodyInterpolation.Interpolate;
                rigidbody.maxAngularVelocity = 20f;
                rigidbody.AddForce(dir * 1000f);
                rigidbody.AddRelativeTorque(new Vector3(UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f), UnityEngine.Random.Range(-1f, 1f) * 0.4f), ForceMode.Impulse);
                plr.WeaponObject.layer = LayerMask.NameToLayer("Gun");
                plr.WeaponObject.transform.parent = null;
            }
            plr.WeaponObject.GetComponent<RangedWeapon>().pickedUp = false;
        }

        public static void SendGrappler(Vector3 dir, Player plr)
        {
            if (GameObject.Find("Grappler") != null)
            {
                var nearest = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (nearest != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("nearestPoint", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        nearestPoint = (Vector3)fieldInfo.GetValue(nearest);
                    }
                }

                var grapple = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (grapple != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("grapplePoint", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        grapplePoint = (Vector3)fieldInfo.GetValue(grapple);
                    }
                }

                var jointt = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (jointt != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("joint", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        joint = (SpringJoint)fieldInfo.GetValue(jointt);
                    }
                }

                var linerender = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (linerender != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("lr", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        lr = (LineRenderer)fieldInfo.GetValue(linerender);
                    }
                }

                var endpoint = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (endpoint != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("lr", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        endPoint = (Vector3)fieldInfo.GetValue(endpoint);
                    }
                }

                var offsetmultiplier = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (offsetmultiplier != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("lr", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        offsetMultiplier = (float)fieldInfo.GetValue(offsetmultiplier);
                    }
                }

                var offsetval = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (offsetval != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("lr", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        offsetVel = (float)fieldInfo.GetValue(offsetval);
                    }
                }

                var poss = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (poss != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("lr", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        positions = (int)fieldInfo.GetValue(poss);
                    }
                }

                var tipp = GameObject.Find("Grappler").GetComponent<Grappler>();

                if (tipp != null)
                {
                    FieldInfo fieldInfo = typeof(Grappler).GetField("lr", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        tip = (Transform)fieldInfo.GetValue(tipp);
                    }
                }
            }

            if (grapplePoint == Vector3.zero || joint == null)
            {
                lr.positionCount = 0;
                return;
            }
            endPoint = Vector3.Lerp(endPoint, grapplePoint, Time.deltaTime * 15f);
            offsetMultiplier = Mathf.SmoothDamp(offsetMultiplier, 0f, ref offsetVel, 0.1f);
            Vector3 position = tip.position;
            float num = Vector3.Distance(endPoint, position);
            lr.SetPosition(0, position);
            lr.SetPosition(positions - 1, endPoint);
            float num2 = num;
            float num3 = 1f;
            for (int i = 1; i < positions - 1; i++)
            {
                float num4 = (float)i / (float)positions;
                float num5 = num4 * offsetMultiplier;
                float num6 = (Mathf.Sin(num5 * num2) - 0.5f) * num3 * (num5 * 2f);
                Vector3 normalized = (endPoint - position).normalized;
                float num7 = Mathf.Sin(num4 * 180f * 0.017453292f);
                float num8 = Mathf.Cos(offsetMultiplier * 90f * 0.017453292f);
                Vector3 vector = position + (endPoint - position) / positions * i + ((Vector3)(num8 * num6 * Vector2.Perpendicular(normalized)) + offsetMultiplier * num7 * Vector3.down);
                lr.SetPosition(i, vector);
            }
        }

        public static void SendProjectile(Vector3 prk, Player plr)
        {
            Vector3 vector = plr.WeaponObject.transform.GetChild(0).position - plr.WeaponObject.transform.GetChild(0).transform.right / 4f;
            Vector3 normalized = (prk - vector).normalized;
            List<Collider> list = new List<Collider>();
            for (int i = 0; i < plr.WeaponObject.GetComponent<RangedWeapon>().bullets; i++)
            {
                UnityEngine.Object.Instantiate(PrefabManager.Instance.muzzle, vector, Quaternion.identity);
                GameObject gameObject = UnityEngine.Object.Instantiate(plr.WeaponObject.GetComponent<RangedWeapon>().projectile, vector, plr.WeaponObject.transform.rotation);
                Rigidbody componentInChildren = gameObject.GetComponentInChildren<Rigidbody>();
                // Get the RangedWeapon component (or any other component that contains the private field)
                RangedWeapon rangedWeapon = plr.WeaponObject.GetComponent<RangedWeapon>();
                if (rangedWeapon != null)
                {
                    // Use reflection to get the private field "projectileColliders"
                    FieldInfo fieldInfo = typeof(RangedWeapon).GetField("projectileColliders", BindingFlags.NonPublic | BindingFlags.Instance);

                    if (fieldInfo != null)
                    {
                        // Get the value of the private field
                        projectileColliders = (Collider[])fieldInfo.GetValue(rangedWeapon);
                    }
                }
                projectileColliders = gameObject.GetComponentsInChildren<Collider>();
                Collider[] array = ( new Collider[1] { PlayerMovement.Instance.GetPlayerCollider() });
                for (int f = 0; f < array.Length; f++)
                {
                    for (int j = 0; j < projectileColliders.Length; j++)
                    {
                        Physics.IgnoreCollision(array[f], projectileColliders[j], ignore: true);
                    }
                }
                componentInChildren.transform.rotation = plr.WeaponObject.transform.rotation;
                Vector3 vector2 = normalized + (plr.WeaponObject.transform.GetChild(0).transform.up * UnityEngine.Random.Range(0f - plr.WeaponObject.GetComponent<RangedWeapon>().accuracy, plr.WeaponObject.GetComponent<RangedWeapon>().accuracy) + plr.WeaponObject.transform.GetChild(0).transform.forward * UnityEngine.Random.Range(0f - plr.WeaponObject.GetComponent<RangedWeapon>().accuracy, plr.WeaponObject.GetComponent<RangedWeapon>().accuracy));
                componentInChildren.AddForce(componentInChildren.mass * plr.WeaponObject.GetComponent<RangedWeapon>().force * vector2);
                Bullet bullet = (Bullet)gameObject.GetComponent(typeof(Bullet));
                if (bullet != null)
                {
                    Color col = Color.magenta;

                    if (bullet.explosive)
                    {
                        UnityEngine.Object.Instantiate(PrefabManager.Instance.thumpAudio, plr.WeaponObject.transform.position, Quaternion.identity);
                    }
                    else
                    {
                        AudioManager.Instance.PlayPitched("GunBass", 0.3f);
                        AudioManager.Instance.PlayPitched("GunHigh", 0.3f);
                        AudioManager.Instance.PlayPitched("GunLow", 0.3f);
                    }

                    UnityEngine.Object.Instantiate(PrefabManager.Instance.gunShotAudio, plr.WeaponObject.transform.position, Quaternion.identity);

                    componentInChildren.AddForce(componentInChildren.mass * plr.WeaponObject.GetComponent<RangedWeapon>().force * vector2);

                    bullet.SetBullet(plr.WeaponObject.GetComponent<RangedWeapon>().damage, plr.WeaponObject.GetComponent<RangedWeapon>().pushBackForce, col);
                    bullet.player = false;
                }

                foreach (Collider item in list)
                {
                    Physics.IgnoreCollision(item, projectileColliders[0]);
                }

                list.Add(projectileColliders[0]);
            }
        }

        public static Player GetPlayerFromID(SteamId id)
        {
            Player plr = new Player();
            foreach (Player plrf in PlayersClient)
            {
                if (plrf.LinkedPlayer == id)
                {
                    plr = plrf;
                }
            }
            return plr;
        }

        public static Friend GetFriendFromID(SteamId id)
        {
            Friend fid = new Friend();
            foreach (Friend friend in Load.CurrentLobby.Members)
            {
                if (friend.Id == id)
                {
                    fid = friend;
                }
            }
            return fid;
        }

        public static Player GetPlayerFromName(string name)
        {
            foreach (Player plr in PlayersClient)
            {
                if (plr.Name == name)
                {
                    return plr;
                }
            }
            return null;
        }
    }
}
