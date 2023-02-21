using System;
using System.Linq;
using HarmonyLib;
using Hazel;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Classes;
using UnityEngine;
using Object = UnityEngine.Object;
using Reactor.Networking.Extensions;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using TownOfUsReworked.CustomOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.MinerMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformAbility
    {
        public static bool Prefix(KillButton __instance)
        {
            if (Utils.NoButton(PlayerControl.LocalPlayer, RoleEnum.Miner))
                return false;

            var role = Role.GetRole<Miner>(PlayerControl.LocalPlayer);

            if (__instance == role.MineButton)
            {
                if (!Utils.ButtonUsable(__instance))
                    return false;

                if (!role.CanPlace)
                    return false;

                if (role.MineTimer() != 0f)
                    return false;

                if (SubmergedCompatibility.GetPlayerElevator(PlayerControl.LocalPlayer).Item1)
                    return false;
                
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable, -1);
                writer.Write((byte)ActionsRPC.Mine);
                var position = PlayerControl.LocalPlayer.transform.position;
                var id = GetAvailableId();
                writer.Write(id);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(position);
                writer.Write(position.z + 0.01f);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                SpawnVent(id, role, position, position.z + 0.01f);

                try
                {
                    //SoundManager.Instance.PlaySound(TownOfUsReworked.MineSound, false, 1f);
                } catch {}

                return false;
            }
            else if (__instance == role.KillButton)
            {
                if (role.KillTimer() != 0f)
                    return false;

                if (Utils.IsTooFar(role.Player, role.ClosestPlayer))
                    return false;
                
                if (!Utils.ButtonUsable(__instance))
                    return false;

                var interact = Utils.Interact(role.Player, role.ClosestPlayer, Role.GetRoleValue(RoleEnum.Pestilence), true);

                if (interact[3] == true || interact[0] == true)
                    role.LastKilled = DateTime.UtcNow;
                else if (interact[1] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.ProtectKCReset);
                else if (interact[2] == true)
                    role.LastKilled.AddSeconds(CustomGameOptions.VestKCReset);

                return false;
            }

            return false;
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