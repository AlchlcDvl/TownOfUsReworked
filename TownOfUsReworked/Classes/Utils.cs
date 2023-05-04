using HarmonyLib;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Patches;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.Data;
using TownOfUsReworked.CustomOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Reactor.Utilities;
using Il2CppInterop.Runtime.InteropTypes;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUsReworked.Objects;
using Il2CppInterop.Runtime;
using System.IO;
using TMPro;
using AmongUs.GameOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using Reactor.Networking.Extensions;
using TownOfUsReworked.PlayerLayers.Abilities;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class Utils
    {
        private static DLoadImage _iCallLoadImage;
        public readonly static List<PlayerControl> RecentlyKilled = new();

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);

        public static TextMeshPro NameText(this PlayerControl p) => p.cosmetics.nameText;

        public static TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

        public static SpriteRenderer MyRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;

        public static VisualAppearance GetDefaultAppearance(this PlayerControl player) => new(player);

        public static bool TryGetAppearance(this PlayerControl player, IVisualAlteration modifier, out VisualAppearance appearance)
        {
            if (modifier != null)
                return modifier.TryGetModifiedAppearance(out appearance);

            appearance = player.GetDefaultAppearance();
            return false;
        }

        public static VisualAppearance GetAppearance(this PlayerControl player)
        {
            if (player.TryGetAppearance(Role.GetRole(player) as IVisualAlteration, out var appearance))
                return appearance;
            else if (player.TryGetAppearance(Modifier.GetModifier(player) as IVisualAlteration, out appearance))
                return appearance;
            else
                return player.GetDefaultAppearance();
        }

        public static bool IsImpostor(this GameData.PlayerInfo playerinfo) => playerinfo?.Role?.TeamType == RoleTeamTypes.Impostor;

        public static bool IsImpostor(this PlayerVoteArea playerinfo) => PlayerByVoteArea(playerinfo).Data?.Role?.TeamType == RoleTeamTypes.Impostor;

        public static GameData.PlayerOutfit GetDefaultOutfit(this PlayerControl playerControl) => playerControl.Data.DefaultOutfit;

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType, GameData.PlayerOutfit outfit)
        {
            playerControl.Data.SetOutfit((PlayerOutfitType)CustomOutfitType, outfit);
            playerControl.SetOutfit(CustomOutfitType);
        }

        public static void SetOutfit(this PlayerControl playerControl, CustomPlayerOutfitType CustomOutfitType)
        {
            if (playerControl == null)
                return;

            var outfitType = (PlayerOutfitType)CustomOutfitType;

            if (!playerControl.Data.Outfits.ContainsKey(outfitType))
                return;

            var newOutfit = playerControl.Data.Outfits[outfitType];
            playerControl.CurrentOutfitType = outfitType;
            playerControl.RawSetName(newOutfit.PlayerName);
            playerControl.RawSetColor(newOutfit.ColorId);
            playerControl.RawSetHat(newOutfit.HatId, newOutfit.ColorId);
            playerControl.RawSetVisor(newOutfit.VisorId, newOutfit.ColorId);
            playerControl.RawSetPet(newOutfit.PetId, newOutfit.ColorId);
            playerControl.RawSetSkin(newOutfit.SkinId, newOutfit.ColorId);
            playerControl.cosmetics.colorBlindText.color = Color.white;
        }

        public static CustomPlayerOutfitType GetCustomOutfitType(this PlayerControl playerControl) => (CustomPlayerOutfitType)playerControl.CurrentOutfitType;

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer)
        {
            if (DoUndo.IsCamoed)
                return;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void DefaultOutfit(PlayerControl player)
        {
            player.SetOutfit(CustomPlayerOutfitType.Default);
            player.MyRend().color = new Color32(255, 255, 255, 255);
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage && player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis && player.GetCustomOutfitType() !=
                    CustomPlayerOutfitType.PlayerNameOnly && !player.Data.IsDead && !PlayerControl.LocalPlayer.Data.IsDead && player != PlayerControl.LocalPlayer)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                    {
                        ColorId = player.GetDefaultOutfit().ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = " "
                    });

                    PlayerMaterial.SetColors(Color.grey, player.MyRend());
                    player.NameText().color = Color.clear;
                    player.cosmetics.colorBlindText.color = Color.clear;

                    if (CustomGameOptions.CamoHideSize)
                        player.GetAppearance().SizeFactor.Set(1f, 1f, 1f);

                    if (CustomGameOptions.CamoHideSpeed)
                        player.MyPhysics.body.velocity.Set(CustomGameOptions.PlayerSpeed, CustomGameOptions.PlayerSpeed);
                }
            }
        }

        public static void Conceal()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                Invis(player, PlayerControl.LocalPlayer.Is(Faction.Syndicate));
        }

        public static void Invis(PlayerControl player, bool condition = false)
        {
            var color = new Color32(0, 0, 0, 0);

            if (condition || PlayerControl.LocalPlayer.Data.IsDead || player == PlayerControl.LocalPlayer)
                color.a = 26;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis)
            {
                player.SetOutfit(CustomPlayerOutfitType.Invis, new GameData.PlayerOutfit()
                {
                    ColorId = player.CurrentOutfit.ColorId,
                    HatId = "",
                    SkinId = "",
                    VisorId = "",
                    PlayerName = " "
                });

                player.MyRend().color = color;
                player.NameText().color = new Color32(0, 0, 0, 0);
                player.cosmetics.colorBlindText.color = new Color32(0, 0, 0, 0);
            }
        }

        public static void Shapeshift()
        {
            var allPlayers = PlayerControl.AllPlayerControls;
            var shifted = new List<PlayerControl>();

            foreach (var player in allPlayers)
            {
                var random = Random.RandomRangeInt(0, allPlayers.Count);

                while (player == allPlayers[random] || shifted.Contains(allPlayers[random]))
                    random = Random.RandomRangeInt(0, allPlayers.Count);

                var otherPlayer = allPlayers[random];
                Morph(player, otherPlayer);
                shifted.Add(otherPlayer);
            }
        }

        public static void DefaultOutfitAll()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                DefaultOutfit(player);
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item) where T : IDisconnectHandler
        {
            if (!self.Contains(item))
                self.Add(item);
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors) => PlayerControl.AllPlayerControls.ToArray().Where(player => !impostors.Any(imp => imp ==
            player)).ToList();

        public static List<PlayerControl> GetImpostors(List<GameData.PlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();

            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static PlayerControl PlayerById(byte id) => PlayerControl.AllPlayerControls.ToArray().ToList().Find(x => x.PlayerId == id);

        public static PlayerVoteArea VoteAreaById(byte id) => MeetingHud.Instance.playerStates.ToList().Find(x => x.TargetPlayerId == id);

        public static DeadBody BodyById(byte id) => Object.FindObjectsOfType<DeadBody>().ToArray().ToList().Find(x => x.ParentId == id);

        public static PlayerControl PlayerByBody(DeadBody body) => PlayerById(body.ParentId);

        public static Vent VentById(int id) => Object.FindObjectsOfType<Vent>().ToArray().ToList().Find(x => x.Id == id);

        public static PlayerControl PlayerByVoteArea(PlayerVoteArea state) => PlayerById(state.TargetPlayerId);

        public static Vector2 GetSize() => Vector2.Scale(Object.FindObjectsOfType<Vent>()[0].GetComponent<BoxCollider2D>().size, Object.FindObjectsOfType<Vent>()[0].transform.localScale)
            * 0.75f;

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static double GetDistBetweenPlayers(PlayerControl player, Vent refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.transform.position;
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static double GetDistBetweenPlayers(PlayerControl player, DeadBody refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.TruePosition;
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason = DeathReasonEnum.Killed, bool lunge = true)
        {
            if (killer == null || target == null)
                return;

            MurderPlayer(killer, target, reason, lunge);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.BypassKill);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write((byte)reason);
            writer.Write(lunge);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, DeathReasonEnum reason, bool lunge)
        {
            if (killer == null || target == null)
                return;

            var data = target.Data;

            if (data == null)
                return;

            lunge = !killer.Is(AbilityEnum.Ninja) && lunge && killer != target;

            if (!data.IsDead)
            {
                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false);

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && !PlayerControl.LocalPlayer.Data.IsDead)
                    Flash(Colors.Coroner);

                if (PlayerControl.LocalPlayer.Data.IsDead)
                    Flash(Colors.Stalemate);

                var targetRole = Role.GetRole(target);

                if (target.Is(ModifierEnum.VIP))
                    Flash(targetRole.Color);

                var killerRole = Role.GetRole(killer);

                if (target.AmOwner)
                {
                    if (Minigame.Instance)
                        Minigame.Instance.Close();

                    if (MapBehaviour.Instance)
                        MapBehaviour.Instance.Close();

                    HudManager.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    HudManager.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.NameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                }

                if (lunge)
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                else
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(target, target));

                if (killer != target)
                {
                    targetRole.KilledBy = " By " + killerRole.PlayerName;
                    targetRole.DeathReason = reason;
                }
                else
                    targetRole.DeathReason = DeathReasonEnum.Suicide;

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                if (target.Is(RoleEnum.Framer))
                    ((Framer)killerRole).Framed.Clear();

                target.RegenTask();
                killer.RegenTask();
                RecentlyKilled.Add(target);

                if (target == Role.DriveHolder)
                    RoleGen.AssignChaosDrive();

                Murder.KilledPlayers.Add(deadBody);

                if (MeetingHud.Instance)
                    target.Exiled();

                ReassignPostmortals(target);

                if (target.Is(ObjectifierEnum.Lovers))
                {
                    var lover = target.GetOtherLover();

                    if (!lover.Is(RoleEnum.Pestilence) && CustomGameOptions.BothLoversDie && !lover.Data.IsDead)
                        MurderPlayer(lover, lover, DeathReasonEnum.Suicide, lunge);
                }

                if (target.Is(ModifierEnum.Diseased))
                    killerRole.Diseased = true;
                else if (target.Is(ModifierEnum.Bait))
                    BaitReport(killer, target);

                if (killer.Is(AbilityEnum.Politician))
                    Ability.GetAbility<Politician>(killer).VoteBank++;

                if (target.Is(RoleEnum.Troll))
                {
                    var troll = Role.GetRole<Troll>(target);
                    troll.Killed = true;
                    RpcMurderPlayer(target, killer, DeathReasonEnum.Trolled, false);
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                    writer.Write((byte)WinLoseRPC.TrollWin);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            if (killer == null || target == null || killer == target)
                yield break;

            var extraDelay = Random.RandomRangeInt(0, (int)((100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay)) + 1));

            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + (extraDelay / 100f));

            var body = BodyById(target.PlayerId);

            if (body != null)
            {
                if (AmongUsClient.Instance.AmHost)
                    killer.ReportDeadBody(target.Data);
                else
                {
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                    writer.Write((byte)ActionsRPC.BaitReport);
                    writer.Write(killer.PlayerId);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second) => first.Zip(second, (x, y) => (x, y));

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null)
                    continue;

                item.Destroy();

                if (item.gameObject == null)
                    continue;

                item.gameObject.Destroy();
            }
        }

        public static void EndGame()
        {
            Ash.DestroyAll();
            GameManager.Instance.RpcEndGame(GameOverReason.ImpostorByVote, false);
        }

        public static object TryCast(this Il2CppObjectBase self, Type type) => AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self,
            Array.Empty<object>());

        public static bool TasksDone()
        {
            var allCrew = new List<PlayerControl>();
            var crewWithNoTasks = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.CanDoTasks() && player.Is(Faction.Crew))
                {
                    allCrew.Add(player);

                    if (Role.GetRole(player)?.TasksDone == true)
                        crewWithNoTasks.Add(player);
                }
            }

            return allCrew.Count == crewWithNoTasks.Count;
        }

        public static bool Sabotaged()
        {
            if (ShipStatus.Instance.Systems != null)
            {
                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.LifeSupp))
                {
                    var lifeSuppSystemType = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();

                    if (lifeSuppSystemType.Countdown < 0f)
                        return true;
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Laboratory))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }

                if (ShipStatus.Instance.Systems.ContainsKey(SystemTypes.Reactor))
                {
                    var reactorSystemType = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ICriticalSabotage>();

                    if (reactorSystemType.Countdown < 0f)
                        return true;
                }
            }

            return false;
        }

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool toKill = false, bool toConvert = false, bool bypass = false)
        {
            var fullReset = false;
            var gaReset = false;
            var survReset = false;
            var abilityUsed = false;
            bypass = bypass || player.Is(AbilityEnum.Ruthless);
            Spread(player, target);

            if (target.IsOnAlert() || target.IsAmbushed() || target.IsGFAmbushed() || target.Is(RoleEnum.Pestilence) || (target.Is(RoleEnum.VampireHunter) &&
                player.Is(SubFaction.Undead)) || (target.Is(RoleEnum.SerialKiller) && (player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Consort) || (player.Is(RoleEnum.Glitch) &&
                !toKill)) && !bypass))
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Medic.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsRetShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetRetMedic().PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if (player.IsShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Medic.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsRetShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = target.GetRetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else
                    RpcMurderPlayer(target, player, target.IsAmbushed() ? DeathReasonEnum.Ambushed : DeathReasonEnum.Killed);

                if (target.IsShielded() && (toKill || toConvert))
                {
                    var medic = target.GetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Medic.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (target.IsRetShielded() && (toKill || toConvert))
                {
                    var medic = target.GetRetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            else if ((target.IsCrusaded() || target.IsRebCrusaded()) && !bypass)
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Medic.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsRetShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetRetMedic().PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        fullReset = CustomGameOptions.ShieldBreaks;
                        Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if (player.IsShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Medic.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsRetShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetRetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Retributionist.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else
                {
                    var crus = target.GetCrusader();
                    var reb = target.GetRebCrus();

                    if (crus?.HoldsDrive == true || reb?.HoldsDrive == true)
                        Crusader.RadialCrusade(target);
                    else
                        RpcMurderPlayer(target, player, DeathReasonEnum.Crusaded);
                }

                if (target.IsShielded() && (toKill || toConvert))
                {
                    var medic = target.GetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Medic.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (target.IsRetShielded() && (toKill || toConvert))
                {
                    var medic = target.GetRetMedic().PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                    fullReset = CustomGameOptions.ShieldBreaks;
                    Retributionist.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            else if (target.IsShielded() && (toKill || toConvert) && !bypass)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                writer.Write(target.GetMedic().PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                fullReset = CustomGameOptions.ShieldBreaks;
                Medic.BreakShield(target.GetMedic().PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsRetShielded() && (toKill || toConvert) && !bypass)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                writer.Write(target.GetRetMedic().PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
                fullReset = CustomGameOptions.ShieldBreaks;
                Retributionist.BreakShield(target.GetMedic().PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsVesting() && (toKill || toConvert) && !bypass)
                survReset = true;
            else if (target.IsProtected() && (toKill || toConvert) && !bypass)
                gaReset = true;
            else if (player.IsOtherRival(target) && (toKill || toConvert))
                fullReset = true;
            else
            {
                if (toKill)
                {
                    if (target.Is(ObjectifierEnum.Fanatic) && (player.Is(Faction.Intruder) || player.Is(Faction.Syndicate)) && target.IsUnturnedFanatic())
                    {
                        var role = Role.GetRole(player);
                        Fanatic.TurnFanatic(target, role.Faction);
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Change, SendOption.Reliable);
                        writer.Write((byte)TurnRPC.TurnFanatic);
                        writer.Write(player.PlayerId);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    else
                        RpcMurderPlayer(player, target);
                }

                if (toConvert && RoleGen.Convertible <= 1)
                    RpcMurderPlayer(player, target, DeathReasonEnum.Failed);

                abilityUsed = true;
                fullReset = true;
            }

            return new List<bool> { fullReset, gaReset, survReset, abilityUsed };
        }

        public static Il2CppSystem.Collections.Generic.List<PlayerControl> GetClosestPlayers(Vector2 truePosition, float radius)
        {
            var playerControlList = new Il2CppSystem.Collections.Generic.List<PlayerControl>();
            var allPlayers = GameData.Instance.AllPlayers;
            var lightRadius = radius * ShipStatus.Instance.MaxLightRadius;

            for (var index = 0; index < allPlayers.Count; ++index)
            {
                var playerInfo = allPlayers[index];

                if (!playerInfo.Disconnected)
                {
                    var vector2 = new Vector2(playerInfo.Object.GetTruePosition().x - truePosition.x, playerInfo.Object.GetTruePosition().y - truePosition.y);
                    var magnitude = vector2.magnitude;

                    if (magnitude <= lightRadius)
                    {
                        var playerControl = playerInfo.Object;
                        playerControlList.Add(playerControl);
                    }
                }
            }

            return playerControlList;
        }

        public static bool IsTooFar(PlayerControl player, PlayerControl target)
        {
            if (player == null || target == null)
                return true;

            var maxDistance = CustomGameOptions.InteractionDistance;
            return GetDistBetweenPlayers(player, target) > maxDistance;
        }

        public static bool IsTooFar(PlayerControl player, DeadBody target)
        {
            if (player == null || target == null)
                return true;

            var maxDistance = CustomGameOptions.InteractionDistance;
            return GetDistBetweenPlayers(player, target) > maxDistance;
        }

        public static bool IsTooFar(PlayerControl player, Vent target)
        {
            if (player == null || target == null)
                return true;

            var maxDistance = CustomGameOptions.InteractionDistance;
            return GetDistBetweenPlayers(player, target) > maxDistance;
        }

        public static bool NoButton(PlayerControl target, RoleEnum role) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(role) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static bool NoButton(PlayerControl target, ModifierEnum mod) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(mod) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static bool NoButton(PlayerControl target, Faction faction) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(faction) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static bool NoButton(PlayerControl target, ObjectifierEnum obj) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(obj) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static bool NoButton(PlayerControl target, AbilityEnum ability) => PlayerControl.AllPlayerControls.Count <= 1 || target == null || target.Data == null || !target.CanMove ||
            !target.Is(ability) || !ConstantVariables.IsRoaming || MeetingHud.Instance || target != PlayerControl.LocalPlayer;

        public static void Spread(PlayerControl interacter, PlayerControl target)
        {
            if (interacter.IsInfected() || target.IsInfected() || target.Is(RoleEnum.Plaguebearer))
            {
                foreach (var pb in Role.GetRoles<Plaguebearer>(RoleEnum.Plaguebearer))
                    pb.RpcSpreadInfection(interacter, target);
            }

            if (target.Is(RoleEnum.Arsonist))
            {
                foreach (var arso in Role.GetRoles<Arsonist>(RoleEnum.Arsonist))
                    arso.RpcSpreadDouse(target, interacter);
            }

            if (target.Is(RoleEnum.Cryomaniac))
            {
                foreach (var cryo in Role.GetRoles<Cryomaniac>(RoleEnum.Cryomaniac))
                    cryo.RpcSpreadDouse(target, interacter);
            }
        }

        public static bool Check(int probability)
        {
            if (probability == 0)
                return false;

            if (probability == 100)
                return true;

            var num = Random.RandomRangeInt(1, 100);
            return num <= probability;
        }

        public static void StopDragging(byte id)
        {
            foreach (var janitor in Role.GetRoles<Janitor>(RoleEnum.Janitor).Where(x => x.CurrentlyDragging != null && x.CurrentlyDragging.ParentId == id))
                janitor.Drop();

            foreach (var godfather in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather).Where(x => x.CurrentlyDragging != null && x.CurrentlyDragging.ParentId == id))
                godfather.Drop();
        }

        public static Sprite CreateSprite(string name)
        {
            try
            {
                var tex = CreatTexture(name);
                var sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
                sprite.hideFlags |= HideFlags.HideAndDontSave | HideFlags.DontSaveInEditor;
                sprite.DontDestroy();
                return sprite;
            }
            catch
            {
                LogSomething($"Error Loading {name}");
                return null;
            }
        }

        public static Sprite CreateSprite2(string name)
        {
            try
            {
                name += ".png";
                var tex = new Texture2D(0, 0, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);
                var imageStream = TownOfUsReworked.Executing.GetManifestResourceStream(name);
                var img = imageStream.ReadFully();
                LoadImage(tex, img, true);
                tex.DontDestroy();
                var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
                sprite.DontDestroy();
                return sprite;
            }
            catch
            {
                LogSomething($"Error Loading {name}");
                return null;
            }
        }

        /*public static Sprite CreateSprite3(string name)
        {
            try
            {
                var stream = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Hats}{name}.png");
                var mainImg = stream.ReadFully();
                var tex2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                LoadImage(tex2D, mainImg, false);
                var sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100);
                sprite.DontDestroy();
                return sprite;
            }
            catch
            {
                LogSomething($"Error Loading {name}");
                return null;
            }
        }

        public static Sprite CreateSprite4(string name)
        {
            try
            {
                var stream = TownOfUsReworked.Assembly.GetManifestResourceStream($"{TownOfUsReworked.Visors}{name}.png");
                var mainImg = stream.ReadFully();
                var tex2D = new Texture2D(1, 1, TextureFormat.ARGB32, false);
                LoadImage(tex2D, mainImg, false);
                var sprite = Sprite.Create(tex2D, new Rect(0, 0, tex2D.width, tex2D.height), new Vector2(0.5f, 0.5f), 100);
                sprite.DontDestroy();
                return sprite;
            }
            catch
            {
                LogSomething($"Error Loading {name}");
                return null;
            }
        }*/

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        public static unsafe Texture2D CreatTexture(string path)
        {
            try
            {
                var texture = new Texture2D(2, 2, TextureFormat.ARGB32, true);
                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(path);
                var length = stream.Length;
                var byteTexture = new Il2CppStructArray<byte>(length);
                stream.Read(new Span<byte>(IntPtr.Add(byteTexture.Pointer, IntPtr.Size * 4).ToPointer(), (int)length));
                ImageConversion.LoadImage(texture, byteTexture, false);
                return texture;
            }
            catch
            {
                LogSomething("Error loading texture from resources: " + path);
                return null;
            }
        }

        public static void LogSomething(object message) => PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(message);

        public static string CreateText(string itemName, string folder = "", string subfolder = "")
        {
            try
            {
                string resourceName;

                if (subfolder != "" && folder != "")
                    resourceName = $"{TownOfUsReworked.Resources}{folder}.{subfolder}.{itemName}";
                else if (subfolder.Length == 0 && folder != "")
                    resourceName = $"{TownOfUsReworked.Resources}{folder}.{itemName}";
                else
                    resourceName = TownOfUsReworked.Resources + itemName;

                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(resourceName);
                var reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
            catch
            {
                LogSomething($"Error Loading {itemName}");
                return "";
            }
        }

        public static AudioClip CreateAudio(string path, string name = "NoName")
        {
            try
            {
                var stream = TownOfUsReworked.Executing.GetManifestResourceStream(path);
                var byteAudio = new byte[stream.Length];
                _ = stream.Read(byteAudio, 0, (int)stream.Length);
                var samples = new float[byteAudio.Length / 4];

                for (var i = 0; i < samples.Length; i++)
                {
                    var offset = i * 4;
                    samples[i] = (float)BitConverter.ToInt32(byteAudio, offset) / int.MaxValue;
                }

                var audioClip = AudioClip.Create(name, samples.Length, 2, 24000, false);
                audioClip.SetData(samples, 0);
                return audioClip;
            }
            catch
            {
                LogSomething($"Error Loading {path}");
                return null;
            }
        }

        public static bool IsInRange(this float num, float min, float max, bool minInclusive = false, bool maxInclusive = false)
        {
            if (minInclusive && maxInclusive)
                return num >= min && num <= max;
            else if (minInclusive)
                return num >= min && num < max;
            else if (maxInclusive)
                return num > min && num <= max;
            else
                return num > min && num < max;
        }

        public static void ShareGameVersion()
        {
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.VersionHandshake, Hazel.SendOption.Reliable);
            writer.Write((byte)TownOfUsReworked.Version.Major);
            writer.Write((byte)TownOfUsReworked.Version.Minor);
            writer.Write((byte)TownOfUsReworked.Version.Build);
            writer.Write(AmongUsClient.Instance.AmHost ? GameStartManagerPatch.timer : -1f);
            writer.WritePacked(AmongUsClient.Instance.ClientId);
            writer.Write((byte)(TownOfUsReworked.Version.Revision < 0 ? 0xFF : TownOfUsReworked.Version.Revision));
            writer.Write(TownOfUsReworked.Executing.ManifestModule.ModuleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            VersionHandshake(TownOfUsReworked.Version.Major, TownOfUsReworked.Version.Minor, TownOfUsReworked.Version.Build, TownOfUsReworked.Version.Revision,
                TownOfUsReworked.Executing.ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            Version ver;

            if (revision < 0)
                ver = new Version(major, minor, build);
            else
                ver = new Version(major, minor, build, revision);

            GameStartManagerPatch.PlayerVersions[clientId] = new PlayerVersion(ver, guid);
        }

        public static IEnumerable<T> GetFastEnumerator<T>(this Il2CppSystem.Collections.Generic.List<T> list) where T : Il2CppSystem.Object => new Il2CppListEnumerable<T>(list);

        public static string GetRandomisedName()
        {
            const string everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π" +
                "★ηΛγΣΦΘξ✧¢";
            var length = Random.RandomRangeInt(1, 11);
            var position = 0;
            var name = "";

            while (position < length)
            {
                var random = Random.RandomRangeInt(0, everything.Length);
                name += everything[random];
                position++;
            }

            return name;
        }

        public static IEnumerator FadeBody(DeadBody body)
        {
            if (body == null)
                yield break;

            foreach (var renderer in body.bodyRenderers)
            {
                var backColor = renderer.material.GetColor(Shader.PropertyToID("_BackColor"));
                var bodyColor = renderer.material.GetColor(Shader.PropertyToID("_BodyColor"));
                var newColor = new Color(1f, 1f, 1f, 0f);

                for (var i = 0; i < 60; i++)
                {
                    renderer.color = Color.Lerp(backColor, newColor, i / 60f);
                    renderer.color = Color.Lerp(bodyColor, newColor, i / 60f);
                    yield return null;
                }
            }

            body.gameObject.Destroy();
            Role.Cleaned.Add(PlayerById(body.ParentId));
        }

        public static void SpawnVent(int ventId, Role role, Vector2 position, float zAxis)
        {
            var ventPrefab = Object.FindObjectOfType<Vent>();
            var vent = Object.Instantiate(ventPrefab, ventPrefab.transform.parent);

            vent.Id = ventId;
            vent.transform.position = new Vector3(position.x, position.y, zAxis);

            if (role.RoleType is not RoleEnum.Godfather and not RoleEnum.Miner)
                return;

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

            if (SubmergedCompatibility.IsSubmerged())
            {
                vent.gameObject.layer = 12;
                vent.gameObject.AddSubmergedComponent(SubmergedCompatibility.ElevatorMover); //Just in case elevator vent is not blocked

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

        public static void Flash(Color color, float duration = 1f, string message = "", float size = 100f) => Coroutines.Start(FlashCoroutine(color, duration, message, size));

        public static IEnumerator FlashCoroutine(Color color, float duration, string message, float size)
        {
            color.a = 0.3f;

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = HudManager.Instance.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            // Message Text
            var messageText = Object.Instantiate(HudManager.Instance.KillButton.cooldownTimerText, HudManager.Instance.transform);
            messageText.text = $"<size={size}%>{message}</size>";
            messageText.enableWordWrapping = false;
            messageText.transform.localScale = Vector3.one * 0.5f;
            messageText.transform.localPosition = new(0, 0, 0);
            messageText.gameObject.SetActive(true);

            yield return new WaitForSeconds(duration);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = HudManager.Instance.FullScreen;

                if (fullscreen.color.Equals(color))
                    fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);

                var fs = false;

                switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
                {
                    case 0:
                    case 1:
                    case 3:
                        var reactor1 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                        var oxygen1 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        fs = reactor1.IsActive || oxygen1.IsActive;
                        break;

                    case 2:
                        var seismic = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        fs = seismic.IsActive;
                        break;

                    case 4:
                        var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();
                        fs = reactor.IsActive;
                        break;

                    case 5:
                        fs = PlayerControl.LocalPlayer.myTasks.ToArray().Any(x => x.TaskType == SubmergedCompatibility.RetrieveOxygenMask);
                        break;

                    case 6:
                        var reactor3 = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<ReactorSystemType>();
                        var oxygen3 = ShipStatus.Instance.Systems[SystemTypes.LifeSupp].Cast<LifeSuppSystemType>();
                        var seismic2 = ShipStatus.Instance.Systems[SystemTypes.Laboratory].Cast<ReactorSystemType>();
                        fs = reactor3.IsActive || seismic2.IsActive || oxygen3.IsActive;
                        break;
                }

                fullscreen.enabled = fs;
                fullscreen.gameObject.active = fs;
                messageText.gameObject.SetActive(false);
                messageText.gameObject.Destroy();
            }
        }

        public static void Warp()
        {
            var coordinates = GenerateWarpCoordinates();
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.WarpAll);
            writer.Write((byte)coordinates.Count);

            foreach (var (key, value) in coordinates)
            {
                writer.Write(key);
                writer.Write(value);
            }

            AmongUsClient.Instance.FinishRpcImmediately(writer);
            WarpPlayersToCoordinates(coordinates);
        }

        public static void WarpPlayersToCoordinates(Dictionary<byte, Vector2> coordinates)
        {
            if (coordinates.ContainsKey(PlayerControl.LocalPlayer.PlayerId))
            {
                Flash(Colors.Warper);

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();

                if (PlayerControl.LocalPlayer.inVent)
                {
                    PlayerControl.LocalPlayer.MyPhysics.RpcExitVent(Vent.currentVent.Id);
                    PlayerControl.LocalPlayer.MyPhysics.ExitAllVents();
                }
            }

            foreach (var (key, value) in coordinates)
            {
                var player = PlayerById(key);
                player.transform.position = value;
                LogSomething($"Warping {player.Data.PlayerName} to ({value.x}, {value.y})");
            }

            foreach (var janitor in Role.GetRoles<Janitor>(RoleEnum.Janitor).Where(x => x.CurrentlyDragging != null))
                janitor.Drop();

            foreach (var godfather in Role.GetRoles<PromotedGodfather>(RoleEnum.PromotedGodfather).Where(x => x.CurrentlyDragging != null))
                godfather.Drop();
        }

        public static Dictionary<byte, Vector2> GenerateWarpCoordinates()
        {
            var targets = PlayerControl.AllPlayerControls.ToArray().Where(player => !player.Data.IsDead && !player.Data.Disconnected).ToList();
            var vents = Object.FindObjectsOfType<Vent>();
            var coordinates = new Dictionary<byte, Vector2>(targets.Count);

            var SkeldPositions = new List<Vector3>()
            {
                new Vector3(-2.2f, 2.2f, 0f), //cafeteria. botton. top left.
                new Vector3(0.7f, 2.2f, 0f), //caffeteria. button. top right.
                new Vector3(-2.2f, -0.2f, 0f), //caffeteria. button. bottom left.
                new Vector3(0.7f, -0.2f, 0f), //caffeteria. button. bottom right.
                new Vector3(10.0f, 3.0f, 0f), //weapons top
                new Vector3(9.0f, 1.0f, 0f), //weapons bottom
                new Vector3(6.5f, -3.5f, 0f), //O2
                new Vector3(11.5f, -3.5f, 0f), //O2-nav hall
                new Vector3(17.0f, -3.5f, 0f), //navigation top
                new Vector3(18.2f, -5.7f, 0f), //navigation bottom
                new Vector3(11.5f, -6.5f, 0f), //nav-shields top
                new Vector3(9.5f, -8.5f, 0f), //nav-shields bottom
                new Vector3(9.2f, -12.2f, 0f), //shields top
                new Vector3(8.0f, -14.3f, 0f), //shields bottom
                new Vector3(2.5f, -16f, 0f), //coms left
                new Vector3(4.2f, -16.4f, 0f), //coms middle
                new Vector3(5.5f, -16f, 0f), //coms right
                new Vector3(-1.5f, -10.0f, 0f), //storage top
                new Vector3(-1.5f, -15.5f, 0f), //storage bottom
                new Vector3(-4.5f, -12.5f, 0f), //storrage left
                new Vector3(0.3f, -12.5f, 0f), //storrage right
                new Vector3(4.5f, -7.5f, 0f), //admin top
                new Vector3(4.5f, -9.5f, 0f), //admin bottom
                new Vector3(-9.0f, -8.0f, 0f), //elec top left
                new Vector3(-6.0f, -8.0f, 0f), //elec top right
                new Vector3(-8.0f, -11.0f, 0f), //elec bottom
                new Vector3(-12.0f, -13.0f, 0f), //elec-lower hall
                new Vector3(-17f, -10f, 0f), //lower engine top
                new Vector3(-17.0f, -13.0f, 0f), //lower engine bottom
                new Vector3(-21.5f, -3.0f, 0f), //reactor top
                new Vector3(-21.5f, -8.0f, 0f), //reactor bottom
                new Vector3(-13.0f, -3.0f, 0f), //security top
                new Vector3(-12.6f, -5.6f, 0f), // security bottom
                new Vector3(-17.0f, 2.5f, 0f), //upper engibe top
                new Vector3(-17.0f, -1.0f, 0f), //upper engine bottom
                new Vector3(-10.5f, 1.0f, 0f), //upper-mad hall
                new Vector3(-10.5f, -2.0f, 0f), //medbay top
                new Vector3(-6.5f, -4.5f, 0f) //medbay bottom
            };

            var MiraPositions = new List<Vector3>()
            {
                new Vector3(-4.5f, 3.5f, 0f), //launchpad top
                new Vector3(-4.5f, -1.4f, 0f), //launchpad bottom
                new Vector3(8.5f, -1f, 0f), //launchpad- med hall
                new Vector3(14f, -1.5f, 0f), //medbay
                new Vector3(16.5f, 3f, 0f), // comms
                new Vector3(10f, 5f, 0f), //lockers
                new Vector3(6f, 1.5f, 0f), //locker room
                new Vector3(2.5f, 13.6f, 0f), //reactor
                new Vector3(6f, 12f, 0f), //reactor middle
                new Vector3(9.5f, 13f, 0f), //lab
                new Vector3(15f, 9f, 0f), //bottom left cross
                new Vector3(17.9f, 11.5f, 0f), //middle cross
                new Vector3(14f, 17.3f, 0f), //office
                new Vector3(19.5f, 21f, 0f), //admin
                new Vector3(14f, 24f, 0f), //greenhouse left
                new Vector3(22f, 24f, 0f), //greenhouse right
                new Vector3(21f, 8.5f, 0f), //bottom right cross
                new Vector3(28f, 3f, 0f), //caf right
                new Vector3(22f, 3f, 0f), //caf left
                new Vector3(19f, 4f, 0f), //storage
                new Vector3(22f, -2f, 0f), //balcony
            };

            var PolusPositions = new List<Vector3>()
            {
                new Vector3(16.6f, -1f, 0f), //dropship top
                new Vector3(16.6f, -5f, 0f), //dropship bottom
                new Vector3(20f, -9f, 0f), //above storrage
                new Vector3(22f, -7f, 0f), //right fuel
                new Vector3(25.5f, -6.9f, 0f), //drill
                new Vector3(29f, -9.5f, 0f), //lab lockers
                new Vector3(29.5f, -8f, 0f), //lab weather notes
                new Vector3(35f, -7.6f, 0f), //lab table
                new Vector3(40.4f, -8f, 0f), //lab scan
                new Vector3(33f, -10f, 0f), //lab toilet
                new Vector3(39f, -15f, 0f), //specimen hall top
                new Vector3(36.5f, -19.5f, 0f), //specimen top
                new Vector3(36.5f, -21f, 0f), //specimen bottom
                new Vector3(28f, -21f, 0f), //specimen hall bottom
                new Vector3(24f, -20.5f, 0f), //admin tv
                new Vector3(22f, -25f, 0f), //admin books
                new Vector3(16.6f, -17.5f, 0f), //office coffe
                new Vector3(22.5f, -16.5f, 0f), //office projector
                new Vector3(24f, -17f, 0f), //office figure
                new Vector3(27f, -16.5f, 0f), //office lifelines
                new Vector3(32.7f, -15.7f, 0f), //lavapool
                new Vector3(31.5f, -12f, 0f), //snowmad below lab
                new Vector3(10f, -14f, 0f), //below storrage
                new Vector3(21.5f, -12.5f, 0f), //storrage vent
                new Vector3(19f, -11f, 0f), //storrage toolrack
                new Vector3(12f, -7f, 0f), //left fuel
                new Vector3(5f, -7.5f, 0f), //above elec
                new Vector3(10f, -12f, 0f), //elec fence
                new Vector3(9f, -9f, 0f), //elec lockers
                new Vector3(5f, -9f, 0f), //elec window
                new Vector3(4f, -11.2f, 0f), //elec tapes
                new Vector3(5.5f, -16f, 0f), //elec-O2 hall
                new Vector3(1f, -17.5f, 0f), //O2 tree hayball
                new Vector3(3f, -21f, 0f), //O2 middle
                new Vector3(2f, -19f, 0f), //O2 gas
                new Vector3(1f, -24f, 0f), //O2 water
                new Vector3(7f, -24f, 0f), //under O2
                new Vector3(9f, -20f, 0f), //right outside of O2
                new Vector3(7f, -15.8f, 0f), //snowman under elec
                new Vector3(11f, -17f, 0f), //comms table
                new Vector3(12.7f, -15.5f, 0f), //coms antenna pult
                new Vector3(13f, -24.5f, 0f), //weapons window
                new Vector3(15f, -17f, 0f), //between coms-office
                new Vector3(17.5f, -25.7f, 0f), //snowman under office
            };

            var dlekSPositions = new List<Vector3>()
            {
                new Vector3(2.2f, 2.2f, 0f), //cafeteria. botton. top left.
                new Vector3(-0.7f, 2.2f, 0f), //caffeteria. button. top right.
                new Vector3(2.2f, -0.2f, 0f), //caffeteria. button. bottom left.
                new Vector3(-0.7f, -0.2f, 0f), //caffeteria. button. bottom right.
                new Vector3(-10.0f, 3.0f, 0f), //weapons top
                new Vector3(-9.0f, 1.0f, 0f), //weapons bottom
                new Vector3(-6.5f, -3.5f, 0f), //O2
                new Vector3(-11.5f, -3.5f, 0f), //O2-nav hall
                new Vector3(-17.0f, -3.5f, 0f), //navigation top
                new Vector3(-18.2f, -5.7f, 0f), //navigation bottom
                new Vector3(-11.5f, -6.5f, 0f), //nav-shields top
                new Vector3(-9.5f, -8.5f, 0f), //nav-shields bottom
                new Vector3(-9.2f, -12.2f, 0f), //shields top
                new Vector3(-8.0f, -14.3f, 0f), //shields bottom
                new Vector3(-2.5f, -16f, 0f), //coms left
                new Vector3(-4.2f, -16.4f, 0f), //coms middle
                new Vector3(-5.5f, -16f, 0f), //coms right
                new Vector3(1.5f, -10.0f, 0f), //storage top
                new Vector3(1.5f, -15.5f, 0f), //storage bottom
                new Vector3(4.5f, -12.5f, 0f), //storrage left
                new Vector3(-0.3f, -12.5f, 0f), //storrage right
                new Vector3(-4.5f, -7.5f, 0f), //admin top
                new Vector3(-4.5f, -9.5f, 0f), //admin bottom
                new Vector3(9.0f, -8.0f, 0f), //elec top left
                new Vector3(6.0f, -8.0f, 0f), //elec top right
                new Vector3(8.0f, -11.0f, 0f), //elec bottom
                new Vector3(12.0f, -13.0f, 0f), //elec-lower hall
                new Vector3(17f, -10f, 0f), //lower engine top
                new Vector3(17.0f, -13.0f, 0f), //lower engine bottom
                new Vector3(21.5f, -3.0f, 0f), //reactor top
                new Vector3(21.5f, -8.0f, 0f), //reactor bottom
                new Vector3(13.0f, -3.0f, 0f), //security top
                new Vector3(12.6f, -5.6f, 0f), // security bottom
                new Vector3(17.0f, 2.5f, 0f), //upper engibe top
                new Vector3(17.0f, -1.0f, 0f), //upper engine bottom
                new Vector3(10.5f, 1.0f, 0f), //upper-mad hall
                new Vector3(10.5f, -2.0f, 0f), //medbay top
                new Vector3(6.5f, -4.5f, 0f) //medbay bottom
            };

            var allLocations = new List<Vector3>();

            foreach (var player in PlayerControl.AllPlayerControls)
                allLocations.Add(player.transform.position);

            foreach (var vent in vents)
                allLocations.Add(GetVentPosition(vent));

            switch (GameOptionsManager.Instance.currentNormalGameOptions.MapId)
            {
                case 0:
                    allLocations.AddRange(SkeldPositions);
                    break;

                case 1:
                    allLocations.AddRange(MiraPositions);
                    break;

                case 2:
                    allLocations.AddRange(PolusPositions);
                    break;

                case 3:
                    allLocations.AddRange(dlekSPositions);
                    break;
            }

            foreach (var target in targets)
            {
                var destination = allLocations.Random();
                coordinates.Add(target.PlayerId, destination);
            }

            return coordinates;
        }

        public static Vector3 GetVentPosition(Vent vent)
        {
            var destination = vent.transform.position;
            destination.y += 0.3636f;
            return destination;
        }

        public static void Revive(DeadBody body)
        {
            var player = PlayerById(body.ParentId);
            player.Revive();
            var position = body.TruePosition;
            Murder.KilledPlayers.Remove(Murder.KilledPlayers.Find(x => x.PlayerId == player.PlayerId));
            RecentlyKilled.Remove(player);
            ReassignPostmortals(player);
            player.Data.SetImpostor(player.Data.IsImpostor());
            player.NetTransform.SnapTo(new Vector2(position.x, position.y + 0.3636f));

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer == player)
                SubmergedCompatibility.ChangeFloor(player.transform.position.y > -7);

            if (player.Data.IsImpostor())
                RoleManager.Instance.SetRole(player, RoleTypes.Impostor);
            else
                RoleManager.Instance.SetRole(player, RoleTypes.Crewmate);

            body?.gameObject.Destroy();
        }

        public static void Revive(PlayerControl player) => Revive(BodyById(player.PlayerId));

        public static IEnumerator Fade(bool fadeAway, bool enableAfterFade)
        {
            HudManager.Instance.FullScreen.enabled = true;

            if (fadeAway)
            {
                for (float i = 1; i >= 0; i -= Time.deltaTime)
                {
                    HudManager.Instance.FullScreen.color = new Color(0, 0, 0, i);
                    yield return null;
                }
            }
            else
            {
                for (float i = 0; i <= 1; i += Time.deltaTime)
                {
                    HudManager.Instance.FullScreen.color = new Color(0, 0, 0, i);
                    yield return null;
                }
            }

            if (enableAfterFade)
            {
                var fs = false;
                var reactor = ShipStatus.Instance.Systems[SystemTypes.Reactor].Cast<HeliSabotageSystem>();

                if (reactor.IsActive)
                    fs = true;

                HudManager.Instance.FullScreen.enabled = fs;
            }
        }

        public static void Teleport(PlayerControl player, Vector3 position)
        {
            player.MyPhysics.ResetMoveState();
            player.NetTransform.SnapTo(new Vector2(position.x, position.y));

            if (SubmergedCompatibility.IsSubmerged() && PlayerControl.LocalPlayer == player)
            {
                SubmergedCompatibility.ChangeFloor(player.GetTruePosition().y > -7);
                SubmergedCompatibility.CheckOutOfBoundsElevator(PlayerControl.LocalPlayer);
            }

            if (PlayerControl.LocalPlayer == player)
            {
                Flash(Colors.Teleporter);

                if (Minigame.Instance)
                    Minigame.Instance.Close();

                if (MapBehaviour.Instance)
                    MapBehaviour.Instance.Close();
            }

            player.moveable = true;
            player.Collider.enabled = true;
            player.NetTransform.enabled = true;
        }

        public static Dictionary<byte, int> CalculateAllVotes(this MeetingHud __instance, out bool tie, out KeyValuePair<byte, int> max)
        {
            var dictionary = new Dictionary<byte, int>();

            for (var i = 0; i < __instance.playerStates.Length; i++)
            {
                var playerVoteArea = __instance.playerStates[i];

                if (!playerVoteArea.DidVote || playerVoteArea.AmDead || playerVoteArea.VotedFor == PlayerVoteArea.MissedVote || playerVoteArea.VotedFor == PlayerVoteArea.DeadVote)
                    continue;

                if (dictionary.TryGetValue(playerVoteArea.VotedFor, out var num))
                    dictionary[playerVoteArea.VotedFor] = num + 1;
                else
                    dictionary[playerVoteArea.VotedFor] = 1;
            }

            foreach (var role in Ability.GetAbilities<Politician>(AbilityEnum.Politician))
            {
                foreach (var number in role.ExtraVotes)
                {
                    if (dictionary.TryGetValue(number, out var num))
                        dictionary[number] = num + 1;
                    else
                        dictionary[number] = 1;
                }
            }

            foreach (var role in Role.GetRoles<Mayor>(RoleEnum.Mayor))
            {
                if (role.Revealed)
                {
                    if (dictionary.TryGetValue(role.Voted, out var num))
                        dictionary[role.Voted] = num + CustomGameOptions.MayorVoteCount;
                    else
                        dictionary[role.Voted] = CustomGameOptions.MayorVoteCount;
                }
            }

            var knighted = new List<byte>();

            foreach (var role in Role.GetRoles<Monarch>(RoleEnum.Monarch))
            {
                foreach (var id in role.Knighted)
                {
                    if (!knighted.Contains(id))
                    {
                        var area = VoteAreaById(id);

                        if (dictionary.TryGetValue(area.VotedFor, out var num))
                            dictionary[area.VotedFor] = num + CustomGameOptions.KnightVoteCount;
                        else
                            dictionary[area.VotedFor] = CustomGameOptions.KnightVoteCount;

                        knighted.Add(id);
                    }
                }
            }

            foreach (var swapper in Ability.GetAbilities<Swapper>(AbilityEnum.Swapper))
            {
                if (swapper.IsDead || swapper.Disconnected || swapper.Swap1 == null || swapper.Swap2 == null)
                    continue;

                var swapPlayer1 = PlayerByVoteArea(swapper.Swap1);
                var swapPlayer2 = PlayerByVoteArea(swapper.Swap2);

                if (swapPlayer1 == null || swapPlayer2 == null || swapPlayer1.Data.IsDead || swapPlayer1.Data.Disconnected || swapPlayer2.Data.IsDead || swapPlayer2.Data.Disconnected)
                    continue;

                var swap1 = 0;
                var swap2 = 0;

                if (dictionary.TryGetValue(swapper.Swap1.TargetPlayerId, out var value))
                    swap1 = value;

                if (dictionary.TryGetValue(swapper.Swap2.TargetPlayerId, out var value2))
                    swap2 = value2;

                dictionary[swapper.Swap2.TargetPlayerId] = swap1;
                dictionary[swapper.Swap1.TargetPlayerId] = swap2;
            }

            max = dictionary.MaxPair(out tie);

            if (tie)
            {
                foreach (var player in __instance.playerStates)
                {
                    if (!player.DidVote || player.AmDead || player.VotedFor == PlayerVoteArea.MissedVote || player.VotedFor == PlayerVoteArea.DeadVote)
                        continue;

                    var ability = Ability.GetAbility(player);

                    if (ability.AbilityType == AbilityEnum.Tiebreaker)
                    {
                        if (dictionary.TryGetValue(player.VotedFor, out var num))
                            dictionary[player.VotedFor] = num + 1;
                        else
                            dictionary[player.VotedFor] = 1;
                    }
                }
            }

            dictionary.MaxPair(out tie);
            return dictionary;
        }

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(255, int.MinValue);

            foreach (var keyValuePair in self)
            {
                if (keyValuePair.Value > result.Value)
                {
                    result = keyValuePair;
                    tie = false;
                }
                else if (keyValuePair.Value == result.Value)
                    tie = true;
            }

            return result;
        }

        public static void ReassignPostmortals(PlayerControl player)
        {
            if (AmongUsClient.Instance.AmHost)
            {
                if (SetPostmortals.WillBeRevealer == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crew) && x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetRevealer, SendOption.Reliable, -1);

                    if (toChooseFrom.Count == 0)
                    {
                        SetPostmortals.WillBeRevealer = null;
                        writer.Write(255);
                    }
                    else
                    {
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeRevealer = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBePhantom == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral) && x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);

                    if (toChooseFrom.Count == 0)
                    {
                        SetPostmortals.WillBePhantom = null;
                        writer.Write(255);
                    }
                    else
                    {
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBePhantom = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBeBanshee == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate) && x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetBanshee, SendOption.Reliable, -1);

                    if (toChooseFrom.Count == 0)
                    {
                        SetPostmortals.WillBeBanshee = null;
                        writer.Write(255);
                    }
                    else
                    {
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeBanshee = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
                else if (SetPostmortals.WillBeGhoul == player)
                {
                    var toChooseFrom = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral) && x.Data.IsDead && !x.Data.Disconnected).ToList();
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGhoul, SendOption.Reliable, -1);

                    if (toChooseFrom.Count == 0)
                    {
                        SetPostmortals.WillBeGhoul = null;
                        writer.Write(255);
                    }
                    else
                    {
                        var rand = Random.RandomRangeInt(0, toChooseFrom.Count);
                        var pc = toChooseFrom[rand];
                        SetPostmortals.WillBeGhoul = pc;
                        writer.Write(pc.PlayerId);
                    }

                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        public static PlayerControl GetClosestPlayer(this PlayerControl refPlayer, List<PlayerControl> AllPlayers = null, float maxDistance = 0f)
        {
            if (refPlayer.Data.IsDead && !refPlayer.Is(RoleEnum.Jester) && !refPlayer.Is(RoleEnum.Ghoul))
                return null;

            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            PlayerControl closestPlayer = null;
            AllPlayers ??= PlayerControl.AllPlayerControls.Il2CppToSystem();

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player == refPlayer || !player.Collider.enabled || player.inMovingPlat || (player.inVent && !CustomGameOptions.VentTargetting))
                    continue;

                var distance = Vector2.Distance(truePosition, player.GetTruePosition());
                var vector = player.GetTruePosition() - truePosition;

                if (distance > closestDistance || distance > maxDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask))
                    continue;

                closestPlayer = player;
                closestDistance = distance;
            }

            return closestPlayer;
        }

        public static Vent GetClosestVent(this PlayerControl refPlayer)
        {
            var truePosition = refPlayer.GetTruePosition();
            var maxDistance = CustomGameOptions.InteractionDistance / 2;
            var closestDistance = double.MaxValue;
            Vent closestVent = null;

            foreach (var vent in Object.FindObjectsOfType<Vent>())
            {
                var distance = Vector2.Distance(truePosition, new Vector2(vent.transform.position.x, vent.transform.position.y));
                var vector = new Vector2(vent.transform.position.x, vent.transform.position.y) - truePosition;

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask))
                    continue;

                closestVent = vent;
                closestDistance = distance;
            }

            return closestVent;
        }

        public static DeadBody GetClosestDeadPlayer(this PlayerControl refPlayer, float maxDistance = 0f)
        {
            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            DeadBody closestBody = null;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var body in Object.FindObjectsOfType<DeadBody>())
            {
                var distance = Vector2.Distance(truePosition, body.TruePosition);
                var vector = body.TruePosition - truePosition;

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask))
                    continue;

                closestBody = body;
                closestDistance = distance;
            }

            return closestBody;
        }

        public static Console GetClosestConsole(this PlayerControl refPlayer, float maxDistance = 0f)
        {
            var truePosition = refPlayer.GetTruePosition();
            var closestDistance = double.MaxValue;
            Console closestConsole = null;

            if (maxDistance == 0f)
                maxDistance = CustomGameOptions.InteractionDistance;

            foreach (var console in Object.FindObjectsOfType<Console>())
            {
                var distance = Vector2.Distance(truePosition, console.transform.position);
                var vector = new Vector2(console.transform.position.x, console.transform.position.y) - truePosition;

                if (distance > maxDistance || distance > closestDistance)
                    continue;

                if (PhysicsHelpers.AnyNonTriggersBetween(truePosition, vector.normalized, distance, Constants.ShipAndObjectsMask))
                    continue;

                closestConsole = console;
                closestDistance = distance;
            }

            return closestConsole;
        }
    }
}