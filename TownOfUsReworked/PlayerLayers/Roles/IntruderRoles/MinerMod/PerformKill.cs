using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Extensions;
using UnityEngine;
using Object = UnityEngine.Object;
using Reactor.Networking.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MinerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            var flag = PlayerControl.LocalPlayer.Is(RoleEnum.Miner);

            if (!flag)
                return true;

            if (!PlayerControl.LocalPlayer.CanMove)
                return false;

            if (PlayerControl.LocalPlayer.Data.IsDead)
                return false;

            var role = Role.GetRole<Miner>(PlayerControl.LocalPlayer);

            if (__instance == role.MineButton)
            {
                if (__instance.isCoolingDown)
                    return false;

                if (!__instance.isActiveAndEnabled)
                    return false;

                if (!role.CanPlace)
                    return false;

                if (role.MineTimer() != 0)
                    return false;

                if (SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1) return false;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.Mine, SendOption.Reliable, -1);
                var position = PlayerControl.LocalPlayer.transform.position;
                var id = GetAvailableId();
                writer.Write(id);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(position);
                writer.Write(0.01f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                SpawnVent(id, role, position, 0.01f);

                try
                {
                    AudioClip MineSFX = TownOfUsReworked.loadAudioClipFromResources("TownOfUsReworked.Resources.Mine.raw");
                    SoundManager.Instance.PlaySound(MineSFX, false, 0.4f);
                } catch {}

                return false;
            }

            return true;
        }

        public static void SpawnVent(int ventId, Miner role, Vector2 position, float zAxis)
        {
            var ventPrefab = Object.FindObjectOfType<Vent>();
            var vent = Object.Instantiate(ventPrefab, ventPrefab.transform.parent);
            
            vent.Id = ventId;
            vent.transform.position = new Vector3(position.x, position.y, zAxis);

            if (role.Vents.Count > 0)
            {
                var leftVent = role.Vents[^1];
                vent.Left = leftVent;
                leftVent.Right = vent;
            }
            else
                vent.Left = null;

            vent.Right = null;
            vent.Center = null;

            var allVents = ShipStatus.Instance.AllVents.ToList();
            allVents.Add(vent);
            ShipStatus.Instance.AllVents = allVents.ToArray();

            role.Vents.Add(vent);
            role.LastMined = DateTime.UtcNow;

            if (SubmergedCompatibility.isSubmerged())
            {
                vent.gameObject.layer = 12;
                vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.Classes.ElevatorMover); // just in case elevator vent is not blocked

                if (vent.gameObject.transform.position.y > -7)
                    vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.03f);
                else
                {
                    vent.gameObject.transform.position = new Vector3(vent.gameObject.transform.position.x, vent.gameObject.transform.position.y, 0.0009f);
                    vent.gameObject.transform.localPosition = new Vector3(vent.gameObject.transform.localPosition.x, vent.gameObject.transform.localPosition.y, -0.003f);
                }
            }
        }

        public static int GetAvailableId()
        {
            var id = 0;

            while (true)
            {
                if (ShipStatus.Instance.AllVents.All(v => v.Id != id))
                    return id;
                
                id++;
            }
        }
    }
}