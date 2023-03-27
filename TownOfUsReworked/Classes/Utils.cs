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
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.CamouflagerMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.Enums;
using TownOfUsReworked.CustomOptions;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;
using Reactor.Utilities;
using Il2CppInterop.Runtime.InteropTypes;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.SerialKillerMod;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.UndertakerMod;
using TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.SyndicateMod;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;
using TownOfUsReworked.Objects;
using System.Reflection;
using Il2CppInterop.Runtime;
using System.IO;
using TMPro;
using AmongUs.GameOptions;
using TownOfUsReworked.Modules;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.Data;

namespace TownOfUsReworked.Classes
{
    [HarmonyPatch]
    public static class Utils
    {
        private static DLoadImage _iCallLoadImage;

        private delegate bool DLoadImage(IntPtr tex, IntPtr data, bool markNonReadable);

        public static TextMeshPro NameText(this PlayerControl p) => p.cosmetics.nameText;

        public static TextMeshPro NameText(this PoolablePlayer p) => p.cosmetics.nameText;

        public static SpriteRenderer MyRend(this PlayerControl p) => p.cosmetics.currentBodySprite.BodySprite;

        public static KeyValuePair<byte, int> MaxPair(this Dictionary<byte, int> self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);

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

        public static KeyValuePair<byte, int> MaxPair(this byte[] self, out bool tie)
        {
            tie = true;
            var result = new KeyValuePair<byte, int>(byte.MaxValue, int.MinValue);

            for (byte i = 0; i < self.Length; i++)
            {
                if (self[i] > result.Value)
                {
                    result = new KeyValuePair<byte, int>(i, self[i]);
                    tie = false;
                }
                else if (self[i] == result.Value)
                    tie = true;
            }

            return result;
        }

        public static VisualAppearance GetDefaultAppearance(this PlayerControl player) => new() { Player = player };

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

        public static Texture2D CreateEmptyTexture(int width = 0, int height = 0) => new(width, height, TextureFormat.RGBA32, Texture.GenerateAllMips, false, IntPtr.Zero);

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer)
        {
            if (CamouflageUnCamouflage.IsCamoed)
                return;

            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void DefaultOutfit(PlayerControl player)
        {
            player.MyRend().color = new Color32(255, 255, 255, 255);
            player.SetOutfit(CustomPlayerOutfitType.Default);
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
            {
                var color = new Color32(0, 0, 0, 0);

                if (PlayerControl.LocalPlayer.Is(Faction.Syndicate) || PlayerControl.LocalPlayer.Data.IsDead)
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

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.PlayerId == id)
                    return player;
            }

            return null;
        }

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

        public static double GetDistBetweenPlayers(PlayerControl player, DeadBody refplayer)
        {
            if (player == null || refplayer == null)
                return double.MaxValue;

            var truePosition = refplayer.TruePosition;
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target, bool lunge = true)
        {
            if (killer == null || target == null)
                return;

            MurderPlayer(killer, target, lunge);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
            writer.Write((byte)ActionsRPC.BypassKill);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            writer.Write(lunge);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target, bool lunge = true)
        {
            if (killer == null || target == null)
                return;

            var data = target.Data;
            lunge = !killer.Is(AbilityEnum.Ninja) && lunge && killer != target;

            if (data?.IsDead == false)
            {
                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 1f);

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Coroner) && !PlayerControl.LocalPlayer.Data.IsDead)
                    Flash(Colors.Coroner, "Someone died!");

                if (PlayerControl.LocalPlayer.Data.IsDead)
                    Flash(Colors.Stalemate, "Someone died!");

                var targetRole = Role.GetRole(target);

                if (target.Is(ModifierEnum.VIP))
                    Flash(targetRole.Color, "The <color=#DCEE85FF>VIP</color> was killed!");

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

                target.RegenTask();
                killer.RegenTask();

                if (lunge)
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                else
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(target, target));

                if (killer != target)
                {
                    targetRole.KilledBy = " By " + killerRole.PlayerName;
                    targetRole.DeathReason = DeathReasonEnum.Killed;
                }
                else
                    targetRole.DeathReason = DeathReasonEnum.Suicide;

                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                if (killer == target)
                    return;

                Murder.KilledPlayers.Add(deadBody);
                target.RpcSetRole(target.Data.IsImpostor() ? RoleTypes.ImpostorGhost : RoleTypes.CrewmateGhost);

                if (!killer.AmOwner)
                    return;

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.SerialKiller))
                {
                    var sk = Role.GetRole<SerialKiller>(killer);
                    sk.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.LustKillCd);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.GlitchKillCooldown);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Juggernaut))
                {
                    var jugg = Role.GetRole<Juggernaut>(killer);
                    var cooldown = Mathf.Clamp(CustomGameOptions.JuggKillCooldown - (CustomGameOptions.JuggKillBonus * jugg.JuggKills), 5, CustomGameOptions.JuggKillCooldown);
                    jugg.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * cooldown);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Pestilence))
                {
                    var pest = Role.GetRole<Pestilence>(killer);
                    pest.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.PestKillCd);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Werewolf))
                {
                    var ww = Role.GetRole<Werewolf>(killer);
                    ww.LastMauled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.MaulCooldown);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Murderer))
                {
                    var murd = Role.GetRole<Murderer>(killer);
                    murd.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.MurdKCD);
                }
                else if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Vigilante))
                {
                    var vig = Role.GetRole<Vigilante>(killer);
                    vig.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.VigiKillCd);
                }
                else if (target.Is(ModifierEnum.Diseased) && (killer.Is(Faction.Intruder) || killer.Is(Faction.Syndicate)))
                {
                    var cooldown = killer.Is(Faction.Intruder) ? CustomGameOptions.IntKillCooldown : CustomGameOptions.ChaosDriveKillCooldown;
                    var last = Last(killer) && killer.Is(AbilityEnum.Underdog);
                    var lowerKC = (cooldown - CustomGameOptions.UnderdogKillBonus) * (CustomGameOptions.DiseasedMultiplier - 1f);
                    var normalKC = cooldown * (CustomGameOptions.DiseasedMultiplier - 1f);
                    var upperKC = (cooldown + CustomGameOptions.UnderdogKillBonus) * (CustomGameOptions.DiseasedMultiplier - 1f);
                    var role = Role.GetRole(killer);

                    switch (role.Faction)
                    {
                        case Faction.Syndicate:
                            var role2 = (SyndicateRole)role;
                            role2.LastKilled = DateTime.UtcNow.AddSeconds(last ? lowerKC : (CustomGameOptions.UnderdogIncreasedKC ? upperKC : normalKC));
                            break;

                        case Faction.Intruder:
                            var role3 = (IntruderRole)role;
                            role3.LastKilled = DateTime.UtcNow.AddSeconds(last ? lowerKC : (CustomGameOptions.UnderdogIncreasedKC ? upperKC : normalKC));
                            break;
                    }
                }
                else if (target.Is(ModifierEnum.Bait))
                    BaitReport(killer, target);
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target) => Coroutines.Start(BaitReportDelay(killer, target));

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            if (killer == null || target == null || killer == target)
                yield break;

            var extraDelay = Random.RandomRangeInt(0, (int) ((100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay)) + 1));

            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + (extraDelay / 100f));

            var bodies = Object.FindObjectsOfType<DeadBody>();

            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var body in bodies)
                {
                    if (body.ParentId == target.PlayerId)
                    {
                        killer.ReportDeadBody(target.Data);
                        break;
                    }
                }
            }
            else
            {
                foreach (var body in bodies)
                {
                    if (body.ParentId == target.PlayerId)
                    {
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Action, SendOption.Reliable);
                        writer.Write((byte)ActionsRPC.BaitReport);
                        writer.Write(killer.PlayerId);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                        break;
                    }
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

                Object.Destroy(item);

                if (item.gameObject == null)
                    return;

                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false) => GameManager.Instance.RpcEndGame(reason, showAds);

        public static object TryCast(this Il2CppObjectBase self, Type type) => AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self,
            Array.Empty<object>());

        public static bool LastImp() => PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Intruder) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;

        public static bool LastSyn() => PlayerControl.AllPlayerControls.ToArray().Count(x => x.Is(Faction.Syndicate) && !(x.Data.IsDead || x.Data.Disconnected)) == 1;

        public static bool Last(PlayerControl player) => (player.Is(Faction.Intruder) && LastImp()) || (player.Is(Faction.Syndicate) && LastSyn());

        public static bool TasksDone()
        {
            var allCrew = new List<PlayerControl>();
            var crewWithNoTasks = new List<PlayerControl>();

            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.CanDoTasks() && player.Is(Faction.Crew))
                {
                    allCrew.Add(player);

                    if (Role.GetRole(player).TasksDone)
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

        public static bool CanVent(PlayerControl player, GameData.PlayerInfo playerInfo)
        {
            if (ConstantVariables.IsHnS)
                return !playerInfo.IsImpostor();

            if (player == null || playerInfo == null || (playerInfo.IsDead && !(player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Phantom) || player.Is(RoleEnum.Ghoul) ||
                player.Is(RoleEnum.Banshee))) || playerInfo.Disconnected || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Noone || !ConstantVariables.IsRoaming || ConstantVariables.IsLobby ||
                player.inMovingPlat || ConstantVariables.IsMeeting)
            {
                return false;
            }
            else if (player.inVent || CustomGameOptions.WhoCanVent == WhoCanVentOptions.Everyone)
                return true;

            var playerRole = Role.GetRole(player);

            bool mainflag;

            if (playerRole == null)
                mainflag = playerInfo.IsImpostor();
            else if (playerRole.IsBlocked)
                mainflag = false;
            else if (player.IsRecruit())
                mainflag = CustomGameOptions.RecruitVent;
            else if (player.IsResurrected())
                mainflag = CustomGameOptions.ResurrectVent;
            else if (player.IsPersuaded())
                mainflag = CustomGameOptions.PersuadedVent;
            else if (player.IsBitten())
                mainflag = CustomGameOptions.UndeadVent;
            else if (player.Is(Faction.Syndicate))
            {
                mainflag = (Role.SyndicateHasChaosDrive && CustomGameOptions.SyndicateVent == SyndicateVentOptions.ChaosDrive) || CustomGameOptions.SyndicateVent ==
                    SyndicateVentOptions.Always;
            }
            else if (player.Is(Faction.Intruder))
            {
                if (CustomGameOptions.IntrudersVent)
                {
                    var flag = (player.Is(RoleEnum.Morphling) && CustomGameOptions.MorphlingVent) || (player.Is(RoleEnum.Wraith) && CustomGameOptions.WraithVent) ||
                        (player.Is(RoleEnum.Grenadier) && CustomGameOptions.GrenadierVent) || (player.Is(RoleEnum.Teleporter) && CustomGameOptions.TeleVent);

                    if (flag)
                        mainflag = true;
                    else if (player.Is(RoleEnum.Undertaker))
                    {
                        var undertaker = (Undertaker)playerRole;

                        mainflag = CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Always || (undertaker.CurrentlyDragging != null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Body) || (undertaker.CurrentlyDragging == null &&
                            CustomGameOptions.UndertakerVentOptions == UndertakerOptions.Bodyless);
                    }
                    else
                        mainflag = true;
                }
                else
                    mainflag = false;
            }
            else if (player.Is(Faction.Crew) && !player.Is(RoleEnum.Revealer))
            {
                if (player.Is(AbilityEnum.Tunneler) && !player.Is(RoleEnum.Engineer))
                {
                    var tunneler = Role.GetRole(player);
                    mainflag = tunneler.TasksDone;
                }
                else if (player.Is(RoleEnum.Engineer))
                    mainflag = true;
                else if (CustomGameOptions.CrewVent)
                    mainflag = true;
                else
                    mainflag = false;
            }
            else if (player.Is(Faction.Neutral))
            {
                var flag = (player.Is(RoleEnum.Murderer) && CustomGameOptions.MurdVent) || (player.Is(RoleEnum.Glitch) && CustomGameOptions.GlitchVent) ||
                    (player.Is(RoleEnum.Juggernaut) && CustomGameOptions.JuggVent) || (player.Is(RoleEnum.Pestilence) && CustomGameOptions.PestVent) ||
                    (player.Is(RoleEnum.Jester) && CustomGameOptions.JesterVent) || (player.Is(RoleEnum.Plaguebearer) && CustomGameOptions.PBVent) ||
                    (player.Is(RoleEnum.Arsonist) && CustomGameOptions.ArsoVent) || (player.Is(RoleEnum.Executioner) && CustomGameOptions.ExeVent) ||
                    (player.Is(RoleEnum.Cannibal) && CustomGameOptions.CannibalVent) || (player.Is(RoleEnum.Dracula) && CustomGameOptions.DracVent) ||
                    (player.Is(RoleEnum.Survivor) && CustomGameOptions.SurvVent) || (player.Is(RoleEnum.Actor) && CustomGameOptions.ActorVent) ||
                    (player.Is(RoleEnum.GuardianAngel) && CustomGameOptions.GAVent) || (player.Is(RoleEnum.Amnesiac) && CustomGameOptions.AmneVent) ||
                    (player.Is(RoleEnum.Werewolf) && CustomGameOptions.WerewolfVent) || (player.Is(RoleEnum.Jackal) && CustomGameOptions.JackalVent) ||
                    (player.Is(RoleEnum.BountyHunter) && CustomGameOptions.BHVent);

                if (flag)
                    mainflag = flag;
                else if (player.Is(RoleEnum.SerialKiller))
                {
                    var role2 = (SerialKiller)playerRole;

                    if (CustomGameOptions.SKVentOptions == SKVentOptions.Always)
                        mainflag = true;
                    else if (role2.Lusted && CustomGameOptions.SKVentOptions == SKVentOptions.Bloodlust)
                        mainflag = true;
                    else if (!role2.Lusted && CustomGameOptions.SKVentOptions == SKVentOptions.NoLust)
                        mainflag = true;
                    else
                        mainflag = false;
                }
                else
                    mainflag = false;
            }
            else if ((player.Is(RoleEnum.Revealer) || player.Is(RoleEnum.Phantom) || player.Is(RoleEnum.Banshee) || player.Is(RoleEnum.Ghoul)) && player.inVent)
                mainflag = true;
            else
                mainflag = false;

            return mainflag;
        }

        public static List<bool> Interact(PlayerControl player, PlayerControl target, bool toKill = false, bool toConvert = false, bool bypass = false)
        {
            if (!CustomButtons.CanInteract(player))
                return new List<bool>() { false, false, false, false };

            var fullCooldownReset = false;
            var gaReset = false;
            var survReset = false;
            var abilityUsed = false;
            bypass = bypass || player.Is(AbilityEnum.Ruthless);

            Spread(player, target);

            if (target.IsOnAlert() || target.IsAmbushed() || target.Is(RoleEnum.Pestilence) || (target.Is(RoleEnum.VampireHunter) && player.Is(SubFaction.Undead)) ||
                (target.Is(RoleEnum.SerialKiller) && (player.Is(RoleEnum.Escort) || player.Is(RoleEnum.Consort) || (player.Is(RoleEnum.Glitch) && !toKill)) && !bypass))
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks)
                            fullCooldownReset = true;

                        StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if (player.IsShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        fullCooldownReset = true;

                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else
                    RpcMurderPlayer(target, player);

                if (target.IsShielded() && (toKill || toConvert))
                {
                    var medic = target.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        fullCooldownReset = true;

                    StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            else if (target.IsCrusaded() && !bypass)
            {
                if (player.Is(RoleEnum.Pestilence))
                {
                    if (target.IsShielded() && (toKill || toConvert))
                    {
                        var medic = target.GetMedic().Player.PlayerId;
                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                        writer.Write(medic);
                        writer.Write(target.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);

                        if (CustomGameOptions.ShieldBreaks)
                            fullCooldownReset = true;

                        StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                    }
                    else if (target.IsProtected())
                        gaReset = true;
                }
                else if (player.IsShielded() && !target.Is(AbilityEnum.Ruthless))
                {
                    var medic = player.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(player.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        fullCooldownReset = true;

                    StopKill.BreakShield(medic, player.PlayerId, CustomGameOptions.ShieldBreaks);
                }
                else if (player.IsProtected() && !target.Is(AbilityEnum.Ruthless))
                    gaReset = true;
                else if (Role.SyndicateHasChaosDrive)
                    Crusader.RadialCrusade(target);
                else
                    RpcMurderPlayer(target, player);

                if (target.IsShielded() && (toKill || toConvert))
                {
                    var medic = target.GetMedic().Player.PlayerId;
                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                    writer.Write(medic);
                    writer.Write(target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);

                    if (CustomGameOptions.ShieldBreaks)
                        fullCooldownReset = true;

                    StopKill.BreakShield(medic, target.PlayerId, CustomGameOptions.ShieldBreaks);
                }
            }
            else if (target.IsShielded() && (toKill || toConvert) && !bypass)
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.AttemptSound, SendOption.Reliable);
                writer.Write(target.GetMedic().Player.PlayerId);
                writer.Write(target.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);

                if (CustomGameOptions.ShieldBreaks)
                    fullCooldownReset = true;

                StopKill.BreakShield(target.GetMedic().Player.PlayerId, target.PlayerId, CustomGameOptions.ShieldBreaks);
            }
            else if (target.IsVesting() && (toKill || toConvert) && !bypass)
                survReset = true;
            else if (target.IsProtected() && (toKill || toConvert) && !bypass)
                gaReset = true;
            else if (player.IsOtherRival(target) && (toKill || toConvert))
                fullCooldownReset = true;
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
                    {
                        RpcMurderPlayer(player, target);

                        if (target.Is(RoleEnum.Troll))
                        {
                            var troll = Role.GetRole<Troll>(target);
                            troll.Killed = true;
                            RpcMurderPlayer(target, player, false);

                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.WinLose, SendOption.Reliable);
                            writer.Write((byte)WinLoseRPC.TrollWin);
                            writer.Write(target.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                        }
                    }
                }

                abilityUsed = true;
                fullCooldownReset = true;
            }

            return new List<bool>
            {
                fullCooldownReset,
                gaReset,
                survReset,
                abilityUsed
            };
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

        public static bool Inactive() => PlayerControl.AllPlayerControls.Count <= 1 || PlayerControl.LocalPlayer == null || PlayerControl.LocalPlayer.Data == null ||
            !PlayerControl.LocalPlayer.CanMove || !ConstantVariables.IsRoaming;

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
                foreach (var pb in Role.GetRoles(RoleEnum.Plaguebearer))
                    ((Plaguebearer)pb).RpcSpreadInfection(interacter, target);
            }

            if (target.Is(RoleEnum.Arsonist))
            {
                foreach (var arso in Role.GetRoles(RoleEnum.Arsonist))
                    ((Arsonist)arso).RpcSpreadDouse(target, interacter);
            }

            if (target.Is(RoleEnum.Cryomaniac))
            {
                foreach (var cryo in Role.GetRoles(RoleEnum.Cryomaniac))
                    ((Cryomaniac)cryo).RpcSpreadDouse(target, interacter);
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

        public static Sprite CreateSprite(string name)
        {
            name += ".png";
            var assembly = Assembly.GetExecutingAssembly();
            var tex = CreateEmptyTexture();
            var imageStream = assembly.GetManifestResourceStream(name);
            var img = imageStream.ReadFully();
            LoadImage(tex, img, true);
            tex.DontDestroy();
            var sprite = Sprite.Create(tex, new Rect(0.0f, 0.0f, tex.width, tex.height), new Vector2(0.5f, 0.5f), 100);
            sprite.DontDestroy();
            return sprite;
        }

        public static void LoadImage(Texture2D tex, byte[] data, bool markNonReadable)
        {
            _iCallLoadImage ??= IL2CPP.ResolveICall<DLoadImage>("UnityEngine.ImageConversion::LoadImage");
            var il2CPPArray = (Il2CppStructArray<byte>) data;
            _iCallLoadImage.Invoke(tex.Pointer, il2CPPArray.Pointer, markNonReadable);
        }

        public static void LogSomething(object message) => PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage(message);

        public static string CreateText(string itemName, string folder = "", string subfolder = "")
        {
            string resourceName;

            if (subfolder != "" && folder != "")
                resourceName = $"{TownOfUsReworked.Resources}{folder}.{subfolder}.{itemName}";
            else if (subfolder.Length == 0 && folder != "")
                resourceName = $"{TownOfUsReworked.Resources}{folder}.{itemName}";
            else
                resourceName = TownOfUsReworked.Resources + itemName;

            var assembly = Assembly.GetExecutingAssembly();
            var stream = assembly.GetManifestResourceStream(resourceName);
            var reader = new StreamReader(stream);
            return reader.ReadToEnd();
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
            writer.Write(Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId.ToByteArray());
            AmongUsClient.Instance.FinishRpcImmediately(writer);
            VersionHandshake(TownOfUsReworked.Version.Major, TownOfUsReworked.Version.Minor, TownOfUsReworked.Version.Build, TownOfUsReworked.Version.Revision,
                Assembly.GetExecutingAssembly().ManifestModule.ModuleVersionId, AmongUsClient.Instance.ClientId);
        }

        public static void VersionHandshake(int major, int minor, int build, int revision, Guid guid, int clientId)
        {
            Version ver;

            if (revision < 0)
                ver = new Version(major, minor, build);
            else
                ver = new Version(major, minor, build, revision);

            GameStartManagerPatch.PlayerVersions[clientId] = new GameStartManagerPatch.PlayerVersion(ver, guid);
        }

        public static IEnumerable<T> GetFastEnumerator<T>(this Il2CppSystem.Collections.Generic.List<T> list) where T : Il2CppSystem.Object => new Il2CppListEnumerable<T>(list);

        public static string GetRandomisedName()
        {
            const string everything = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz1234567890!@#$%^&*()|{}[],.<>;':\"-+=*/`~_\\ ⟡☆♡♧♤ø▶❥✔εΔΓικνστυφψΨωχӪζδ♠♥βαµ♣✚Ξρλς§π★ηΛγΣΦΘξ";
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

        public static void Flash(Color color, string message = "", float duration = 1f, float size = 100f)
        {
            if (!HudManager.Instance || HudManager.Instance.FullScreen == null)
                return;

            HudManager.Instance.FullScreen.gameObject.SetActive(true);
            HudManager.Instance.FullScreen.enabled = true;

            // Message Text
            var messageText = Object.Instantiate(HudManager.Instance.KillButton.cooldownTimerText, HudManager.Instance.transform);
            messageText.text = $"<size={size}%>{message}</size>";
            messageText.enableWordWrapping = false;
            messageText.transform.localScale = Vector3.one * 0.5f;
            messageText.transform.localPosition = new Vector3(0f, 0f, 0f);
            messageText.gameObject.SetActive(true);
            HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
            {
                var fullscreen = HudManager.Instance.FullScreen;

                if (p < 0.5)
                {
                    if (fullscreen != null)
                        fullscreen.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 1.5f));
                }
                else if (fullscreen != null)
                    fullscreen.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 1.5f));

                if (p == 1f && fullscreen != null)
                    fullscreen.enabled = false;

                if (p == 1f)
                    messageText.gameObject.Destroy();
            })));
        }

        public static IEnumerator FlashCoro(Color color, string message = "", float duration = 1f, float size = 100f)
        {
            if (!HudManager.Instance || HudManager.Instance.FullScreen == null)
                yield break;

            HudManager.Instance.FullScreen.gameObject.SetActive(true);
            HudManager.Instance.FullScreen.enabled = true;

            // Message Text
            var messageText = Object.Instantiate(HudManager.Instance.KillButton.cooldownTimerText, HudManager.Instance.transform);
            messageText.text = $"<size={size}%>{message}</size>";
            messageText.enableWordWrapping = false;
            messageText.transform.localScale = Vector3.one * 0.5f;
            messageText.transform.localPosition = new Vector3(0f, 0f, 0f);
            messageText.gameObject.SetActive(true);
            HudManager.Instance.StartCoroutine(Effects.Lerp(duration, new Action<float>((p) =>
            {
                var fullscreen = HudManager.Instance.FullScreen;

                if (p < 0.5)
                {
                    if (fullscreen != null)
                        fullscreen.color = new Color(color.r, color.g, color.b, Mathf.Clamp01(p * 1.5f));
                }
                else if (fullscreen != null)
                    fullscreen.color = new Color(color.r, color.g, color.b, Mathf.Clamp01((1 - p) * 1.5f));

                if (p == 1f && fullscreen != null)
                    fullscreen.enabled = false;

                if (p == 1f)
                    messageText.gameObject.Destroy();
            })));
        }

        public static IEnumerator FadeBody(DeadBody body)
        {
            var renderer = body.bodyRenderer;
            var backColor = renderer.material.GetColor(Shader.PropertyToID("_BackColor"));
            var bodyColor = renderer.material.GetColor(Shader.PropertyToID("_BodyColor"));
            var newColor = new Color(1f, 1f, 1f, 0f);

            for (var i = 0; i < 60; i++)
            {
                if (body == null)
                    yield break;

                renderer.color = Color.Lerp(backColor, newColor, i / 60f);
                renderer.color = Color.Lerp(bodyColor, newColor, i / 60f);
                yield return null;
            }

            Object.Destroy(body.gameObject);
        }
    }
}