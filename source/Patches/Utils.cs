using HarmonyLib;
using Hazel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Reactor.Extensions;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using UnhollowerBaseLib;
using UnityEngine;
using Object = UnityEngine.Object;
using PerformKill = TownOfUs.ImpostorRoles.UnderdogMod.PerformKill;
using Reactor;
using Random = UnityEngine.Random;

namespace TownOfUs
{
    [HarmonyPatch]
    public static class Utils
    {
        internal static bool ShowDeadBodies = false;
        private static GameData.PlayerInfo voteTarget = null;

        public static Dictionary<PlayerControl, Color> oldColors = new Dictionary<PlayerControl, Color>();

        public static List<WinningPlayerData> potentialWinners = new List<WinningPlayerData>();

        public static void Morph(PlayerControl player, PlayerControl MorphedPlayer, bool resetAnim = false)
        {
            if (CamouflageUnCamouflage.IsCamoed) return;
            if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Morph)
                player.SetOutfit(CustomPlayerOutfitType.Morph, MorphedPlayer.Data.DefaultOutfit);
        }

        public static void Unmorph(PlayerControl player)
        {
           player.SetOutfit(CustomPlayerOutfitType.Default);
        }

        public static void Camouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.GetCustomOutfitType() != CustomPlayerOutfitType.Camouflage &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.Invis &&
                    player.GetCustomOutfitType() != CustomPlayerOutfitType.PlayerNameOnly)
                {
                    player.SetOutfit(CustomPlayerOutfitType.Camouflage, new GameData.PlayerOutfit()
                    {
                        ColorId = player.GetDefaultOutfit().ColorId,
                        HatId = "",
                        SkinId = "",
                        VisorId = "",
                        PlayerName = " "
                    });
                    PlayerMaterial.SetColors(Color.grey, player.myRend());
                    player.nameText().color = Color.clear;
                    player.cosmetics.colorBlindText.color = Color.clear;
                  
                }
            }
        }

        public static void UnCamouflage()
        {
            foreach (var player in PlayerControl.AllPlayerControls) Unmorph(player);
        }

        public static void AddUnique<T>(this Il2CppSystem.Collections.Generic.List<T> self, T item)
            where T : IDisconnectHandler
        {
            if (!self.Contains(item)) self.Add(item);
        }

        public static bool IsLover(this PlayerControl player)
        {
            return player.Is(ModifierEnum.Lover);
        }

        public static bool Is(this PlayerControl player, RoleEnum roleType)
        {
            return Role.GetRole(player)?.RoleType == roleType;
        }

        public static bool Is(this PlayerControl player, ModifierEnum modifierType)
        {
            return Modifier.GetModifier(player)?.ModifierType == modifierType;
        }

        public static bool Is(this PlayerControl player, AbilityEnum abilityType)
        {
            return Ability.GetAbility(player)?.AbilityType == abilityType;
        }

        public static bool Is(this PlayerControl player, Faction faction)
        {
            return Role.GetRole(player)?.Faction == faction;
        }

        public static bool Is(this PlayerControl player, Alignment alignment)
        {
            return Role.GetRole(player)?.Alignment == alignment;
        }

        public static bool Is(this PlayerControl player, SubFaction subfaction)
        {
            return Role.GetRole(player)?.SubFaction == subfaction;
        }

        public static List<PlayerControl> GetCrewmates(List<PlayerControl> impostors)
        {
            return PlayerControl.AllPlayerControls.ToArray().Where(
                player => !impostors.Any(imp => imp.PlayerId == player.PlayerId)).ToList();
        }

        public static List<PlayerControl> GetImpostors(List<GameData.PlayerInfo> infected)
        {
            var impostors = new List<PlayerControl>();
            foreach (var impData in infected)
                impostors.Add(impData.Object);

            return impostors;
        }

        public static RoleEnum GetRole(PlayerControl player)
        {
            if (player == null) return RoleEnum.None;
            if (player.Data == null) return RoleEnum.None;

            var role = Role.GetRole(player);
            if (role != null) return role.RoleType;

            return player.Data.IsImpostor() ? RoleEnum.Impostor : RoleEnum.Crewmate;
        }

        public static PlayerControl PlayerById(byte id)
        {
            foreach (var player in PlayerControl.AllPlayerControls)
                if (player.PlayerId == id)
                    return player;

            return null;
        }

        public static bool IsShielded(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).Any(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            });
        }

        public static Medic GetMedic(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Medic).FirstOrDefault(role =>
            {
                var shieldedPlayer = ((Medic)role).ShieldedPlayer;
                return shieldedPlayer != null && player.PlayerId == shieldedPlayer.PlayerId;
            }) as Medic;
        }

        public static bool IsOnAlert(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Veteran).Any(role =>
            {
                var veteran = (Veteran)role;
                return veteran != null && veteran.OnAlert && player.PlayerId == veteran.Player.PlayerId;
            });
        }

        public static bool IsVesting(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Survivor).Any(role =>
            {
                var surv = (Survivor)role;
                return surv != null && surv.Vesting && player.PlayerId == surv.Player.PlayerId;
            });
        }

        public static bool IsProtected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.GuardianAngel).Any(role =>
            {
                var gaTarget = ((GuardianAngel)role).target;
                var ga = (GuardianAngel)role;
                return gaTarget != null && ga.Protecting && player.PlayerId == gaTarget.PlayerId;
            });
        }

        public static bool IsInfected(this PlayerControl player)
        {
            return Role.GetRoles(RoleEnum.Plaguebearer).Any(role =>
            {
                var plaguebearer = (Plaguebearer)role;
                return plaguebearer != null && (plaguebearer.InfectedPlayers.Contains(player.PlayerId) || player.PlayerId == plaguebearer.Player.PlayerId);
            });
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refPlayer, List<PlayerControl> AllPlayers)
        {
            var num = double.MaxValue;
            var refPosition = refPlayer.GetTruePosition();
            PlayerControl result = null;
            foreach (var player in AllPlayers)
            {
                if (player.Data.IsDead || player.PlayerId == refPlayer.PlayerId || !player.Collider.enabled) continue;
                var playerPosition = player.GetTruePosition();
                var distBetweenPlayers = Vector2.Distance(refPosition, playerPosition);
                var isClosest = distBetweenPlayers < num;
                if (!isClosest) continue;
                var vector = playerPosition - refPosition;
                if (PhysicsHelpers.AnyNonTriggersBetween(
                    refPosition, vector.normalized, vector.magnitude, Constants.ShipAndObjectsMask
                )) continue;
                num = distBetweenPlayers;
                result = player;
            }

            return result;
        }

        public static PlayerControl GetClosestPlayer(PlayerControl refplayer)
        {
            return GetClosestPlayer(refplayer, PlayerControl.AllPlayerControls.ToArray().ToList());
        }
        public static void SetTarget(
            ref PlayerControl closestPlayer,
            KillButton button,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (!button.isActiveAndEnabled) return;

            button.SetTarget(
                SetClosestPlayer(ref closestPlayer, maxDistance, targets)
            );
        }

        public static PlayerControl SetClosestPlayer(
            ref PlayerControl closestPlayer,
            float maxDistance = float.NaN,
            List<PlayerControl> targets = null
        )
        {
            if (float.IsNaN(maxDistance))
                maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
            var player = GetClosestPlayer(
                PlayerControl.LocalPlayer,
                targets ?? PlayerControl.AllPlayerControls.ToArray().ToList()
            );
            var closeEnough = player == null || (
                GetDistBetweenPlayers(PlayerControl.LocalPlayer, player) < maxDistance
            );
            return closestPlayer = closeEnough ? player : null;
        }

        public static double GetDistBetweenPlayers(PlayerControl player, PlayerControl refplayer)
        {
            var truePosition = refplayer.GetTruePosition();
            var truePosition2 = player.GetTruePosition();
            return Vector2.Distance(truePosition, truePosition2);
        }

        public static void RpcMurderPlayer(PlayerControl killer, PlayerControl target)
        {
            MurderPlayer(killer, target);
            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                (byte)CustomRPC.BypassKill, SendOption.Reliable, -1);
            writer.Write(killer.PlayerId);
            writer.Write(target.PlayerId);
            AmongUsClient.Instance.FinishRpcImmediately(writer);
        }

        public static void MurderPlayer(PlayerControl killer, PlayerControl target)
        {
            var data = target.Data;
            if (data != null && !data.IsDead)
            {
                if (killer == PlayerControl.LocalPlayer)
                    SoundManager.Instance.PlaySound(PlayerControl.LocalPlayer.KillSfx, false, 0.8f);

                target.gameObject.layer = LayerMask.NameToLayer("Ghost");
                target.Visible = false;

                if (PlayerControl.LocalPlayer.Is(RoleEnum.Mystic) && !PlayerControl.LocalPlayer.Data.IsDead)
                {
                    Coroutines.Start(FlashCoroutine(Patches.Colors.Crew));
                }

                if (target.AmOwner)
                {
                    try
                    {
                        if (Minigame.Instance)
                        {
                            Minigame.Instance.Close();
                            Minigame.Instance.Close();
                        }

                        if (MapBehaviour.Instance)
                        {
                            MapBehaviour.Instance.Close();
                            MapBehaviour.Instance.Close();
                        }
                    }
                    catch
                    {
                    }

                    DestroyableSingleton<HudManager>.Instance.KillOverlay.ShowKillAnimation(killer.Data, data);
                    DestroyableSingleton<HudManager>.Instance.ShadowQuad.gameObject.SetActive(false);
                    target.nameText().GetComponent<MeshRenderer>().material.SetInt("_Mask", 0);
                    target.RpcSetScanner(false);
                    var importantTextTask = new GameObject("_Player").AddComponent<ImportantTextTask>();
                    importantTextTask.transform.SetParent(AmongUsClient.Instance.transform, false);
                    if (!PlayerControl.GameOptions.GhostsDoTasks)
                    {
                        for (var i = 0; i < target.myTasks.Count; i++)
                        {
                            var playerTask = target.myTasks.ToArray()[i];
                            playerTask.OnRemove();
                            Object.Destroy(playerTask.gameObject);
                        }

                        target.myTasks.Clear();
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostIgnoreTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }
                    else
                    {
                        importantTextTask.Text = DestroyableSingleton<TranslationController>.Instance.GetString(
                            StringNames.GhostDoTasks,
                            new Il2CppReferenceArray<Il2CppSystem.Object>(0));
                    }

                    target.myTasks.Insert(0, importantTextTask);
                }

                if (!killer.Is(RoleEnum.Poisoner) && !killer.Is(RoleEnum.Arsonist) && !killer.Is(RoleEnum.TimeMaster))
                {
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(killer, target));
                }
                else
                {
                    killer.MyPhysics.StartCoroutine(killer.KillAnimations.Random().CoPerformKill(target, target));
                }
                var deadBody = new DeadPlayer
                {
                    PlayerId = target.PlayerId,
                    KillerId = killer.PlayerId,
                    KillTime = DateTime.UtcNow
                };

                Murder.KilledPlayers.Add(deadBody);

                if (!killer.AmOwner) return;

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Werewolf))
                {
                    var werewolf = Role.GetRole<Werewolf>(killer);
                    werewolf.LastKilled = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.RampageKillCd);
                    werewolf.Player.SetKillTimer(CustomGameOptions.RampageKillCd * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Glitch))
                {
                    var glitch = Role.GetRole<Glitch>(killer);
                    glitch.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * CustomGameOptions.GlitchKillCooldown);
                    glitch.Player.SetKillTimer(CustomGameOptions.GlitchKillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Juggernaut))
                {
                    var juggernaut = Role.GetRole<Juggernaut>(killer);
                    juggernaut.LastKill = DateTime.UtcNow.AddSeconds((CustomGameOptions.DiseasedMultiplier - 1f) * (CustomGameOptions.JuggKillCooldown + 5.0f - CustomGameOptions.JuggKillBonus * juggernaut.JuggKills));
                    juggernaut.Player.SetKillTimer((CustomGameOptions.JuggKillCooldown + 5.0f - CustomGameOptions.JuggKillBonus * juggernaut.JuggKills) * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Is(RoleEnum.Underdog))
                {
                    var lowerKC = (PlayerControl.GameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    var normalKC = PlayerControl.GameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier;
                    var upperKC = (PlayerControl.GameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus) * CustomGameOptions.DiseasedMultiplier;
                    killer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                    return;
                }

                if (target.Is(ModifierEnum.Diseased) && killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown * CustomGameOptions.DiseasedMultiplier);
                    return;
                }

                if (target.Is(ModifierEnum.Bait))
                {
                    BaitReport(killer, target);
                    return;
                }

                if (killer.Is(RoleEnum.Underdog))
                {
                    var lowerKC = PlayerControl.GameOptions.KillCooldown - CustomGameOptions.UnderdogKillBonus;
                    var normalKC = PlayerControl.GameOptions.KillCooldown;
                    var upperKC = PlayerControl.GameOptions.KillCooldown + CustomGameOptions.UnderdogKillBonus;
                    killer.SetKillTimer(PerformKill.LastImp() ? lowerKC : (PerformKill.IncreasedKC() ? normalKC : upperKC));
                    return;
                }

                if (killer.Data.IsImpostor())
                {
                    killer.SetKillTimer(PlayerControl.GameOptions.KillCooldown);
                    return;
                }
            }
        }

        public static void BaitReport(PlayerControl killer, PlayerControl target)
        {
            Coroutines.Start(BaitReportDelay(killer, target));
        }

        public static IEnumerator BaitReportDelay(PlayerControl killer, PlayerControl target)
        {
            var extraDelay = Random.RandomRangeInt(0, (int) (100 * (CustomGameOptions.BaitMaxDelay - CustomGameOptions.BaitMinDelay) + 1));
            if (CustomGameOptions.BaitMaxDelay <= CustomGameOptions.BaitMinDelay)
                yield return new WaitForSeconds(CustomGameOptions.BaitMaxDelay + 0.01f);
            else
                yield return new WaitForSeconds(CustomGameOptions.BaitMinDelay + 0.01f + extraDelay/100f);
            var bodies = Object.FindObjectsOfType<DeadBody>();
            if (AmongUsClient.Instance.AmHost)
            {
                foreach (var body in bodies)
                {
                    try
                    {
                        if (body.ParentId == target.PlayerId) { killer.ReportDeadBody(target.Data); break; }
                    }
                    catch
                    {
                    }
                }
                
            }
            else
            {
                foreach (var body in bodies)
                {
                    try
                    {
                        if (body.ParentId == target.PlayerId)
                        {
                            var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                                (byte)CustomRPC.BaitReport, SendOption.Reliable, -1);
                            writer.Write(killer.PlayerId);
                            writer.Write(target.PlayerId);
                            AmongUsClient.Instance.FinishRpcImmediately(writer);
                            break;
                        }
                    }
                    catch
                    {
                    }
                }
                
            }

        }

        public static IEnumerator FlashCoroutine(Color color, float waitfor = 1f, float alpha = 0.3f)
        {
            color.a = alpha;
            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                fullscreen.enabled = true;
                fullscreen.gameObject.active = true;
                fullscreen.color = color;
            }

            yield return new WaitForSeconds(waitfor);

            if (HudManager.InstanceExists && HudManager.Instance.FullScreen)
            {
                var fullscreen = DestroyableSingleton<HudManager>.Instance.FullScreen;
                if (fullscreen.color.Equals(color))
                {
                    fullscreen.color = new Color(1f, 0f, 0f, 0.37254903f);
                    fullscreen.enabled = false;
                }
            }
        }

        public static IEnumerable<(T1, T2)> Zip<T1, T2>(List<T1> first, List<T2> second)
        {
            return first.Zip(second, (x, y) => (x, y));
        }

        public static void DestroyAll(this IEnumerable<Component> listie)
        {
            foreach (var item in listie)
            {
                if (item == null) continue;
                Object.Destroy(item);
                if (item.gameObject == null) return;
                Object.Destroy(item.gameObject);
            }
        }

        public static void EndGame(GameOverReason reason = GameOverReason.ImpostorByVote, bool showAds = false)
        {
            ShipStatus.RpcEndGame(reason, showAds);
        }

        [HarmonyPatch(typeof(MedScanMinigame), nameof(MedScanMinigame.FixedUpdate))]
        class MedScanMinigameFixedUpdatePatch
        {
            static void Prefix(MedScanMinigame __instance)
            {
                if (CustomGameOptions.ParallelMedScans)
                {
                    //Allows multiple medbay scans at once
                    __instance.medscan.CurrentUser = PlayerControl.LocalPlayer.PlayerId;
                    __instance.medscan.UsersList.Clear();
                }
            }
        }
      
        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.StartMeeting))]
        class StartMeetingPatch {
            public static void Prefix(PlayerControl __instance, [HarmonyArgument(0)]GameData.PlayerInfo meetingTarget) {
                voteTarget = meetingTarget;
            }
        }

        [HarmonyPatch(typeof(MeetingHud), nameof(MeetingHud.Update))]
        class MeetingHudUpdatePatch {
            static void Postfix(MeetingHud __instance) {
                // Deactivate skip Button if skipping on emergency meetings is disabled 
                if ((voteTarget == null && CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Emergency) || (CustomGameOptions.SkipButtonDisable == DisableSkipButtonMeetings.Always)) {
                    __instance.SkipVoteButton.gameObject.SetActive(false);
                }
            }
        }

        //Submerged utils
        public static object TryCast(this Il2CppObjectBase self, Type type)
        {
            return AccessTools.Method(self.GetType(), nameof(Il2CppObjectBase.TryCast)).MakeGenericMethod(type).Invoke(self, Array.Empty<object>());
        }

        public static IList createList(Type myType)
        {
            Type genericListType = typeof(List<>).MakeGenericType(myType);
            return (IList)Activator.CreateInstance(genericListType);
        }

        public static void IsCrew(PlayerControl player)
        {
            ;
        }

        public static void ResetCustomTimers()
        {
            #region CrewmateRoles
            foreach (Medium role in Role.GetRoles(RoleEnum.Medium))
            {
                role.LastMediated = DateTime.UtcNow;
            }
            foreach (Sheriff role in Role.GetRoles(RoleEnum.Sheriff))
            {
                role.LastInterrogated = DateTime.UtcNow;
            }
            foreach (Vigilante role in Role.GetRoles(RoleEnum.Vigilante))
            {
                role.LastKilled = DateTime.UtcNow;
            }
            foreach (TimeLord role in Role.GetRoles(RoleEnum.TimeLord))
            {
                role.StartRewind = DateTime.UtcNow.AddSeconds(-10.0f);
                role.FinishRewind = DateTime.UtcNow;
            }
            foreach (Tracker role in Role.GetRoles(RoleEnum.Tracker))
            {
                role.LastTracked = DateTime.UtcNow;
            }
            foreach (Transporter role in Role.GetRoles(RoleEnum.Transporter))
            {
                role.LastTransported = DateTime.UtcNow;
            }
            foreach (Veteran role in Role.GetRoles(RoleEnum.Veteran))
            {
                role.LastAlerted = DateTime.UtcNow;
            }
            foreach (Operative role in Role.GetRoles(RoleEnum.Operative))
            {
                role.lastBugged = DateTime.UtcNow;
            }
            foreach (Detective role in Role.GetRoles(RoleEnum.Detective))
            {
                role.LastExamined = DateTime.UtcNow;
            }
            foreach (Shifter role in Role.GetRoles(RoleEnum.Shifter))
            {
                role.LastShifted = DateTime.UtcNow;
            }
            #endregion
            #region NeutralRoles
            foreach (Survivor role in Role.GetRoles(RoleEnum.Survivor))
            {
                role.LastVested = DateTime.UtcNow;
            }
            foreach (GuardianAngel role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                role.LastProtected = DateTime.UtcNow;
            }
            foreach (Arsonist role in Role.GetRoles(RoleEnum.Arsonist))
            {
                role.LastDoused = DateTime.UtcNow;
            }
            foreach (Glitch role in Role.GetRoles(RoleEnum.Glitch))
            {
                role.LastHack = DateTime.UtcNow;
                role.LastKill = DateTime.UtcNow;
                role.LastMimic = DateTime.UtcNow;
            }
            foreach (Juggernaut role in Role.GetRoles(RoleEnum.Juggernaut))
            {
                role.LastKill = DateTime.UtcNow;
            }
            foreach (Werewolf role in Role.GetRoles(RoleEnum.Werewolf))
            {
                role.LastRampaged = DateTime.UtcNow;
                role.LastKilled = DateTime.UtcNow;
            }
            foreach (Plaguebearer role in Role.GetRoles(RoleEnum.Plaguebearer))
            {
                role.LastInfected = DateTime.UtcNow;
            }
            foreach (Pestilence role in Role.GetRoles(RoleEnum.Pestilence))
            {
                role.LastKill = DateTime.UtcNow;
            }
            foreach (Cannibal role in Role.GetRoles(RoleEnum.Cannibal))
            {
                role.LastEaten = DateTime.UtcNow;
            }
            #endregion
            #region ImposterRoles
            foreach (Blackmailer role in Role.GetRoles(RoleEnum.Blackmailer))
            {
                role.LastBlackmailed = DateTime.UtcNow;
            }
            foreach (Grenadier role in Role.GetRoles(RoleEnum.Grenadier))
            {
                role.LastFlashed = DateTime.UtcNow;
            }
            foreach (Miner role in Role.GetRoles(RoleEnum.Miner))
            {
                role.LastMined = DateTime.UtcNow;
            }
            foreach (Morphling role in Role.GetRoles(RoleEnum.Morphling))
            {
                role.LastMorphed = DateTime.UtcNow;
            }
            foreach (Poisoner role in Role.GetRoles(RoleEnum.Poisoner))
            {
                role.LastPoisoned = DateTime.UtcNow;
            }
            foreach (Wraith role in Role.GetRoles(RoleEnum.Wraith))
            {
                role.LastInvis = DateTime.UtcNow;
            }
            foreach (Undertaker role in Role.GetRoles(RoleEnum.Undertaker))
            {
                role.LastDragged = DateTime.UtcNow;
            }
            foreach (Disguiser role in Role.GetRoles(RoleEnum.Disguiser))
            {
                role.LastDisguised = DateTime.UtcNow;
            }
            foreach (TimeMaster role in Role.GetRoles(RoleEnum.TimeMaster))
            {
                role.LastFrozen = DateTime.UtcNow;
            }
            foreach (Consigliere role in Role.GetRoles(RoleEnum.Consigliere))
            {
                role.LastInvestigated = DateTime.UtcNow;
            }
            #endregion
        }
        
        public static void AirKill(PlayerControl player, PlayerControl target)
        {
            Vector3 vector = target.transform.position;
            vector.z = vector.y / 1000f;
            player.transform.position = vector;
            player.NetTransform.SnapTo(vector);
        }

        public static PlayerControl SetClosestPlayerToPlayer(PlayerControl fromPlayer, ref PlayerControl closestPlayer, float maxDistance = float.NaN, List<PlayerControl> targets = null)
        {
            if (float.IsNaN(maxDistance)) 
                maxDistance = GameOptionsData.KillDistances[PlayerControl.GameOptions.KillDistance];
                
            var player = GetClosestPlayer(fromPlayer, targets ?? PlayerControl.AllPlayerControls.ToArray().ToList());
            var closeEnough = player == null || (GetDistBetweenPlayers(fromPlayer, player) < maxDistance);
            return closestPlayer = closeEnough ? player : null;
        }

        public static PlayerPhysics instance;

        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static void TimeFreeze()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (instance.AmOwner && GameData.Instance && instance.myPlayer.CanMove && !instance.myPlayer.Data.IsDead)
                {
                    if (!(player.Is(RoleEnum.TimeMaster) || player.Is(RoleEnum.TimeLord)))
                        instance.body.velocity = instance.body.velocity * 0;
                }
            }
        }
        
        [HarmonyPatch(typeof(PlayerPhysics), nameof(PlayerPhysics.FixedUpdate))]
        public static void TimeUnfreeze()
        {
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (instance.AmOwner && GameData.Instance && instance.myPlayer.CanMove && !instance.myPlayer.Data.IsDead)
                {
                    if (!(player.Is(RoleEnum.TimeMaster) || player.Is(RoleEnum.TimeLord)))
                    {
                        if (player.Is(ModifierEnum.Drunk))
                            instance.body.velocity = instance.body.velocity * -1;
                        else
                            instance.body.velocity = instance.body.velocity * 1;
                    }
                }
            }
        }
    }
}
