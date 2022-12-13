using Hazel;
using System;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using HarmonyLib;
using UnityEngine;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Patches;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using AmongUs.GameOptions;

namespace TownOfUsReworked.PlayerLayers.Roles.CrewRoles.InspectorMod
{
    [HarmonyPatch(typeof(KillButton), nameof(KillButton.DoClick))]
    public class PerformKill
    {
        public static bool Prefix(KillButton __instance)
        {
            if (__instance != DestroyableSingleton<HudManager>.Instance.KillButton)
                return true;

            if (!PlayerControl.LocalPlayer.Is(RoleEnum.Inspector))
                return true;

            var role = Role.GetRole<Inspector>(PlayerControl.LocalPlayer);

            if (!PlayerControl.LocalPlayer.CanMove | role.ClosestPlayer == null)
                return false;

            var flag2 = role.ExamineTimer() == 0f;

            if (!flag2)
                return false;

            if (!__instance.enabled)
                return false;

            var maxDistance = GameOptionsData.KillDistances[GameOptionsManager.Instance.currentNormalGameOptions.KillDistance];

            if (Vector2.Distance(role.ClosestPlayer.GetTruePosition(), PlayerControl.LocalPlayer.GetTruePosition()) > maxDistance)
                return false;

            if (role.ClosestPlayer == null)
                return false;

            if (role.ClosestPlayer.IsInfected() | role.Player.IsInfected())
            {
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(role.ClosestPlayer, role.Player);
            }

            if (role.ClosestPlayer.IsOnAlert() | role.ClosestPlayer.Is(RoleEnum.Pestilence))
            {
                if (role.Player.IsShielded())
                {
                    var writer2 = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound,
                        SendOption.Reliable, -1);
                    writer2.Write(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId);
                    writer2.Write(PlayerControl.LocalPlayer.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer2);

                    System.Console.WriteLine(CustomGameOptions.ShieldBreaks + "- shield break");

                    if (CustomGameOptions.ShieldBreaks)
                        role.LastExamined = DateTime.UtcNow;

                    StopKill.BreakShield(PlayerControl.LocalPlayer.GetMedic().Player.PlayerId, PlayerControl.LocalPlayer.PlayerId,
                        CustomGameOptions.ShieldBreaks);
                    return false;
                }
                else if (!role.Player.IsProtected())
                {
                    Utils.RpcMurderPlayer(role.ClosestPlayer, PlayerControl.LocalPlayer);
                    return false;
                }

                role.LastExamined = DateTime.UtcNow;
                return false;
            }

            role.Examined.Add(role.ClosestPlayer);

            foreach (var player in role.Examined)
            {
                var playerName = player.nameText().text;
                player.nameText().color = new Color32(255, 255, 255, 255);
                
                if (player.Is(InspResults.SherConsigInspBm))
                    playerName += $"\n<color=#FFCC80FF>Sher</color> <color=#FFFF99FF>Consig</color> <color=#7E3C64FF>Insp</color> <color=#02A752FF>BM</color>";
                else if (player.Is(InspResults.CrewImpAnMurd))
                    playerName += $"\n<color=#8BFDFDFF>Crew</color> <color=#FF0000FF>Imp</color> <color=#008000FF>Anarchist</color> <color=#6F7BEAFF>Murd</color>";
                else if (player.Is(InspResults.GAExeMedicPup))
                    playerName += $"\n<color=#FFFFFFFF>GA</color> <color=#CCCCCCFF>Exe</color> <color=#A680FFFF>Med</color> <color=#00FFFFFF>Pup</color>";
                else if (player.Is(InspResults.DisgMorphCamoAgent))
                    playerName += $"\n<color=#40B4FFFF>Disg</color> <color=#BB45B0FF>Morph</color> <color=#378AC0FF>Camo</color> <color=#CCA3CCFF>Agent</color>";
                else if (player.Is(InspResults.JestJuggWWInv))
                    playerName += $"\n<color=#F7B3DAFF>Jest</color> <color=#A12B56FF>Jugg</color> <color=#9F703AFF>WW</color> <color=#00B3B3FF>Inv</color>";
                else if (player.Is(InspResults.SurvVHVampVig))
                    playerName += $"\n<color=#DDDD00FF>Surv</color> <color=#C0C0C0FF>VH</color> <color=#7B8968FF>Vamp</color> <color=#FFFF00FF>Vig</color>";
                else if (player.Is(InspResults.GFMayorRebelPest))
                    playerName += $"\n<color=#404C08FF>GF</color> <color=#704FA8FF>Mayo</color> <color=#FFFCCEFF>Rebel</color> <color=#424242FF>Pest</color>";
                else if (player.Is(InspResults.EscConsGliPois))
                    playerName += $"\n<color=#803333FF>Esc</color> <color=#801780FF>Cons</color> <color=#00FF00FF>Gli</color> <color=#B5004CFF>Pois</color>";
                else if (player.Is(InspResults.MineMafiSideDamp))
                    playerName += $"\n<color=#AA7632FF>Mine</color> <color=#6400FFFF>Mafi</color> <color=#979C9FFF>Side</color> <color=#DF7AE8FF>Damp</color>";
                else if (player.Is(InspResults.TransWarpTeleTask))
                    playerName += $"\n<color=#00EEFFFF>Trans</color> <color=#8C7140FF>Warp</color> <color=#6AA84FFF>Tele</color> <color=#ABABFFFF>Task</color>";
                else if (player.Is(InspResults.EngiAmneThiefCann))
                    playerName += $"\n<color=#FFA60AFF>Engi</color> <color=#22FFFFFF>Amne</color> <color=#80FF00FF>Thief</color> <color=#8C4005FF>Can</color>";
                else if (player.Is(InspResults.WraithDetGrenVet))
                    playerName += $"\n<color=#FFB875FF>Wraith</color> <color=#4D4DFFFF>Det</color> <color=#85AA5BFF>Gren</color> <color=#998040FF>Vet</color>";
                else if (player.Is(InspResults.ShiftSwapSKDrac))
                    playerName += $"\n<color=#DF851FFF>Shift</color> <color=#66E666FF>Swap</color> <color=#336EFFFF>SK</color> <color=#AC8A00FF>Drac</color>";
                else if (player.Is(InspResults.CoroJaniUTMed))
                    playerName += $"\n<color=#4D99E6FF>Coro</color> <color=#2647A2FF>Jani</color> <color=#005643FF>UT</color> <color=#A680FFFF>Med</color>";
                else if (player.Is(InspResults.ArsoCryoPBOpTroll))
                    playerName += $"\n<color=#EE7600FF>Arso</color> <color=#642DEAFF>Cryo</color> <color=#CFFE61FF>PB</color> <color=#A7D1B3FF>Op</color>";
                else if (player.Is(InspResults.TrackAltTLTM))
                    playerName += $"\n<color=#009900FF>Track</color> <color=#660000FF>Alt</color> <color=#0000FFFF>TL</color> <color=#0000A7FF>TM</color>";

                player.nameText().text = playerName;                
            }
            
            unchecked
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.InspExamine, SendOption.Reliable, -1);
                writer.Write(PlayerControl.LocalPlayer.PlayerId);
                writer.Write(role.ClosestPlayer.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            role.Examined.Add(role.ClosestPlayer);
            role.LastExamined = DateTime.UtcNow;
            
            try
            {
                SoundManager.Instance.PlaySound(TownOfUsReworked.PhantomWin, false, 1f);
            } catch {}
            
            return false;
        }
    }
}
