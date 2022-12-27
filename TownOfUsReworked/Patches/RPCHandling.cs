using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor.Utilities;
using TownOfUsReworked.PlayerLayers.Roles.Roles;
using Reactor.Utilities.Extensions;
using TownOfUsReworked.Enums;
using TownOfUsReworked.Lobby.CustomOption;
using TownOfUsReworked.Extensions;
using TownOfUsReworked.PlayerLayers.Modifiers;
using TownOfUsReworked.PlayerLayers.Abilities.Abilities;
using TownOfUsReworked.PlayerLayers.Modifiers.Modifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Objectifiers.PhantomMod;
using TownOfUsReworked.PlayerLayers.Objectifiers.Objectifiers;
using TownOfUsReworked.PlayerLayers.Abilities;
using TownOfUsReworked.PlayerLayers.Abilities.AssassinMod;
using TownOfUsReworked.PlayerLayers.Abilities.RevealerMod;
using TownOfUsReworked.PlayerLayers.Roles;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.MedicMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.CoronerMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.SwapperMod;
using TownOfUsReworked.PlayerLayers.Roles.CrewRoles.TimeLordMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.ExecutionerMod;
using TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.GuardianAngelMod;
using UnityEngine;
using Random = UnityEngine.Random;
using Object = UnityEngine.Object;
using TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.ConsigliereMod;
using Coroutine = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.JanitorMod.Coroutine;
using Eat = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CannibalMod.Coroutine;
using Revive = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.AltruistMod.Coroutine;
using PerformKillButton = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.AmnesiacMod.PerformKillButton;
using PerformDeclareButton = TownOfUsReworked.PlayerLayers.Roles.IntruderRoles.GodfatherMod.PerformKill;
using PerformSidekickButton = TownOfUsReworked.PlayerLayers.Roles.SyndicateRoles.RebelMod.PerformKill;
using PerformShiftButton = TownOfUsReworked.PlayerLayers.Roles.CrewRoles.ShifterMod.PerformShiftButton;
using Reveal = TownOfUsReworked.PlayerLayers.Abilities.RevealerMod;
using PerformConvertButton = TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.DraculaMod.PerformConvertButton;
using Reactor.Networking.Extensions;

namespace TownOfUsReworked.Patches
{
    public static class RPCHandling
    {
        private static readonly List<(Type, CustomRPC, int, bool)> CrewRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralNonKillingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralKillingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> IntruderRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> SyndicateRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralChaosRoles = new List<(Type, CustomRPC, int, bool)>();

        private static readonly List<(Type, CustomRPC, int)> GlobalModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ProfessionalModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> BaitModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> DiseasedModifiers = new List<(Type, CustomRPC, int)>();

        private static readonly List<(Type, CustomRPC, int)> GlobalAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> IntruderAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> CrewAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NonEvilAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> EvilAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> SyndicateAbilityGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NeutralAbilityGet = new List<(Type, CustomRPC, int)>();

        private static readonly List<(Type, CustomRPC, int)> CrewObjectifierGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NeutralObjectifierGet = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> GlobalObjectifierGet = new List<(Type, CustomRPC, int)>();

        private static void SortRoles(List<(Type, CustomRPC, int, bool)> roles, int max, int min)
        {
            roles.Shuffle();

            if (roles.Count < max)
                max = roles.Count;

            if (min > max)
                min = max;

            var amount = Random.RandomRangeInt(min, max + 1);
            roles.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });

            var certainRoles = 0;
            var odds = 0;

            foreach (var role in roles)
            {
                if (role.Item3 == 100) certainRoles += 1;
                else odds += role.Item3;
            }

            while (certainRoles < amount)
            {
                var num = certainRoles;
                var random = Random.RandomRangeInt(0, odds);
                var rolePicked = false;

                while (num < roles.Count && rolePicked == false)
                {
                    random -= roles[num].Item3;

                    if (random < 0)
                    {
                        odds -= roles[num].Item3;
                        var role = roles[num];
                        roles.Remove(role);
                        roles.Insert(0, role);
                        certainRoles += 1;
                        rolePicked = true;
                    }
                    num += 1;
                }
            }

            while (roles.Count > amount)
                roles.RemoveAt(roles.Count - 1);
        }

        private static void SortOthers(List<(Type, CustomRPC, int)> roles, int max)
        {
            roles.Shuffle();

            roles.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });
            
            while (roles.Count > max)
                roles.RemoveAt(roles.Count - 1);
        }

        private static void GenEachRole(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);

            crewmates.Shuffle();
            impostors.Shuffle();

            if (CustomGameOptions.GameMode == GameMode.Classic | CustomGameOptions.GameMode == GameMode.Custom)
            {
                if (crewmates.Count > CustomGameOptions.MaxNeutralNonKillingRoles)
                    SortRoles(NeutralNonKillingRoles, CustomGameOptions.MaxNeutralNonKillingRoles, CustomGameOptions.MinNeutralNonKillingRoles);
                else
                    SortRoles(NeutralNonKillingRoles, crewmates.Count - 1, CustomGameOptions.MinNeutralNonKillingRoles);

                if (crewmates.Count - NeutralNonKillingRoles.Count > CustomGameOptions.MaxNeutralKillingRoles)
                    SortRoles(NeutralKillingRoles, CustomGameOptions.MaxNeutralKillingRoles, CustomGameOptions.MinNeutralKillingRoles);
                else
                    SortRoles(NeutralKillingRoles, crewmates.Count - NeutralNonKillingRoles.Count - 1, CustomGameOptions.MinNeutralKillingRoles);

                SortRoles(CrewRoles, crewmates.Count - NeutralNonKillingRoles.Count - NeutralKillingRoles.Count, crewmates.Count -
                    NeutralNonKillingRoles.Count - NeutralKillingRoles.Count);
                SortRoles(IntruderRoles, impostors.Count, impostors.Count);

                if (!CustomGameOptions.AltImps)
                    SortRoles(SyndicateRoles, CustomGameOptions.SyndicateCount, CustomGameOptions.SyndicateCount);
            }

            var NonIntruderRoles = new List<(Type, CustomRPC, int, bool)>();
            var neutrals = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral)).ToList();
            var syndicate = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Syndicate)).ToList();

            if (CustomGameOptions.GameMode == GameMode.Classic | CustomGameOptions.GameMode == GameMode.Custom)
                NonIntruderRoles.AddRange(CrewRoles);

            NonIntruderRoles.AddRange(NeutralNonKillingRoles);
            NonIntruderRoles.AddRange(NeutralKillingRoles);

            if (!CustomGameOptions.AltImps)
                NonIntruderRoles.AddRange(SyndicateRoles);

            var crewRoles = new List<(Type, CustomRPC, int, bool)>();
            var impRoles = new List<(Type, CustomRPC, int, bool)>();

            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                NonIntruderRoles.Shuffle();

                if (NonIntruderRoles.Count > 0)
                {
                    crewRoles.Add(NonIntruderRoles[0]);

                    if (NonIntruderRoles[0].Item4 == true)
                        NonIntruderRoles.Remove(NonIntruderRoles[0]);
                }

                if (CrewRoles.Count > 0)
                {
                    CrewRoles.Shuffle();
                    crewRoles.Add(CrewRoles[0]);

                    if (CrewRoles[0].Item4 == true)
                        CrewRoles.Remove(CrewRoles[0]);
                }
                else
                    crewRoles.Add((typeof(Crewmate), CustomRPC.SetCrewmate, 100, false));

                NonIntruderRoles.AddRange(CrewRoles);

                while (crewRoles.Count < crewmates.Count && NonIntruderRoles.Count > 0)
                {
                    NonIntruderRoles.Shuffle();
                    crewRoles.Add(NonIntruderRoles[0]);

                    if (NonIntruderRoles[0].Item4 == true)
                    {
                        if (CrewRoles.Contains(NonIntruderRoles[0]))
                            CrewRoles.Remove(NonIntruderRoles[0]);

                        NonIntruderRoles.Remove(NonIntruderRoles[0]);
                    }
                }

                while (impRoles.Count < impostors.Count && IntruderRoles.Count > 0)
                {
                    IntruderRoles.Shuffle();
                    impRoles.Add(IntruderRoles[0]);

                    if (IntruderRoles[0].Item4 == true)
                        IntruderRoles.Remove(IntruderRoles[0]);
                }
            }

            crewRoles.Shuffle();
            impRoles.Shuffle();

            var crewCount = PlayerControl.AllPlayerControls.Count - CustomGameOptions.SyndicateCount - CustomGameOptions.IntruderCount - neutrals.Count;

            SortOthers(GlobalModifiers, crewmates.Count + impostors.Count);
            SortOthers(ProfessionalModifiers, crewmates.Count + impostors.Count);
            SortOthers(DiseasedModifiers, crewmates.Count + impostors.Count);
            SortOthers(BaitModifiers, crewmates.Count + impostors.Count);

            SortOthers(IntruderAbilityGet, CustomGameOptions.IntruderCount);
            SortOthers(CrewAbilityGet, crewCount);
            SortOthers(NeutralAbilityGet, neutrals.Count);
            SortOthers(SyndicateAbilityGet, CustomGameOptions.SyndicateCount);

            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                foreach (var (type, rpc, _, unique) in crewRoles)
                    Role.Gen<Role>(type, crewmates, rpc);
                    
                foreach (var (type, rpc, _, unique) in impRoles)
                    Role.Gen<Role>(type, impostors, rpc);
            }
            else
            {
                foreach (var (type, rpc, _, unique) in NonIntruderRoles)
                    Role.Gen<Role>(type, crewmates, rpc);
                    
                foreach (var (type, rpc, _, unique) in IntruderRoles)
                    Role.Gen<Role>(type, impostors, rpc);
            }

            foreach (var crewmate in crewmates)
                Role.Gen<Role>(typeof(Crewmate), crewmate, CustomRPC.SetCrewmate);

            foreach (var impostor in impostors)
            {
                if (CustomGameOptions.AltImps)
                    Role.Gen<Role>(typeof(Anarchist), impostor, CustomRPC.SetAnarchist);
                else
                    Role.Gen<Role>(typeof(Impostor), impostor, CustomRPC.SetImpostor);
            }

            var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility2 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility3 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility4 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility5 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility6 = PlayerControl.AllPlayerControls.ToArray().ToList();

            canHaveAbility.RemoveAll(player => !player.Is(Faction.Intruder) | (player.Is(RoleEnum.Consigliere) && CustomGameOptions.ConsigInfo == ConsigInfo.Role));
            canHaveAbility.Shuffle();

            canHaveAbility2.RemoveAll(player => !(player.Is(RoleAlignment.NeutralPros) | player.Is(RoleAlignment.NeutralNeo) | player.Is(RoleAlignment.NeutralKill)));
            canHaveAbility2.Shuffle();

            canHaveAbility3.RemoveAll(player => !player.Is(Faction.Crew));
            canHaveAbility3.Shuffle();

            canHaveAbility4.RemoveAll(player => !player.Is(Faction.Syndicate));
            canHaveAbility4.Shuffle();

            canHaveAbility5.RemoveAll(player => !player.Is(Faction.Crew) | player.Is(RoleEnum.Engineer));
            canHaveAbility5.Shuffle();

            canHaveAbility6.RemoveAll(player => !player.Is(Faction.Neutral));
            canHaveAbility6.Shuffle();

            if (CustomGameOptions.WhoCanVent == WhoCanVentOptions.Default)
            {
                while (canHaveAbility5.Count > 0 && CrewAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = CrewAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility5.TakeFirst(), rpc);
                }
            }

            if (CustomGameOptions.GameMode == GameMode.Custom && (IntruderAbilityGet.Count + CrewAbilityGet.Count + SyndicateAbilityGet.Count + NeutralAbilityGet.Count) > 0)
            {
                var impAssassins = CustomGameOptions.NumberOfImpostorAssassins;
                var neutAssassins = CustomGameOptions.NumberOfNeutralAssassins;
                var crewAssassins = CustomGameOptions.NumberOfCrewAssassins;
                var synAssassins = CustomGameOptions.NumberOfSyndicateAssassins;

                while (canHaveAbility.Count > 0 && impAssassins > 0 && IntruderAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = IntruderAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility.TakeFirst(), rpc);
                    impAssassins -= 1;
                }

                while (canHaveAbility2.Count > 0 && neutAssassins > 0 && NeutralAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = NeutralAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility2.TakeFirst(), rpc);
                    neutAssassins -= 1;
                }

                while (canHaveAbility3.Count > 0 && crewAssassins > 0 && CrewAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = CrewAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility3.TakeFirst(), rpc);
                    crewAssassins -= 1;
                }

                while (canHaveAbility4.Count > 0 && synAssassins > 0 && SyndicateAbilityGet.Count > 0)
                {
                    var (type, rpc, _) = SyndicateAbilityGet.TakeFirst();
                    Role.Gen<Ability>(type, canHaveAbility4.TakeFirst(), rpc);
                    synAssassins -= 1;
                }
            }

            var canHaveLover = PlayerControl.AllPlayerControls.ToArray().ToList();

            canHaveLover.RemoveAll(player => player.Is(RoleEnum.Altruist) | player.Is(RoleEnum.Troll) | player.Is(RoleEnum.Jester) | player.Is(RoleEnum.Shifter));
            canHaveLover.Shuffle();

            foreach (var (type, rpc, _) in GlobalObjectifierGet)
            {
                if (canHaveLover.Count == 0 || canHaveLover.Count == 1)
                    break;

                Lovers.Gen(canHaveLover);
                Role.Gen<Objectifier>(type, canHaveLover.TakeFirst(), rpc);
            }

            var canHaveModifier1 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveModifier2 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveModifier3 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveModifier4 = PlayerControl.AllPlayerControls.ToArray().ToList();

            canHaveModifier1.RemoveAll(player => player.Is(RoleEnum.Vigilante) | player.Is(RoleEnum.Shifter) | player.Is(RoleEnum.Altruist) |
                player.Is(RoleEnum.Troll));
            canHaveModifier1.Shuffle();

            canHaveModifier2.RemoveAll(player => player.Is(RoleEnum.Altruist) | player.Is(RoleEnum.Troll));
            canHaveModifier2.Shuffle();

            canHaveModifier3.RemoveAll(player => !player.Is(AbilityEnum.Assassin));
            canHaveModifier3.Shuffle();

            while (canHaveModifier2.Count > 0 && BaitModifiers.Count > 0)
            {
                var (type, rpc, _) = BaitModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier2.TakeFirst(), rpc);
            }

            while (canHaveModifier2.Count > 0 && DiseasedModifiers.Count > 0)
            {
                var (type, rpc, _) = DiseasedModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier2.TakeFirst(), rpc);
            }

            while (canHaveModifier3.Count > 0 && ProfessionalModifiers.Count > 0)
            {
                var (type, rpc, _) = ProfessionalModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier3.TakeFirst(), rpc);
            }

            while (canHaveModifier4.Count > 0 && GlobalModifiers.Count > 0)
            {
                var (type, rpc, _) = GlobalModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier4.TakeFirst(), rpc);
            }

            var exeTargets = new List<PlayerControl>();
            var gaTargets = new List<PlayerControl>();
            var goodRecruits = new List<PlayerControl>();
            var evilRecruits = new List<PlayerControl>();
            
            foreach (var player in PlayerControl.AllPlayerControls)
            {
                if (player.Is(Faction.Crew))
                {
                    if (!player.Is(RoleEnum.Altruist))
                    {
                        gaTargets.Add(player);
                        goodRecruits.Add(player);
                    }
                        
                    if (!(player.Is(RoleAlignment.CrewSov) | player.Is(ObjectifierEnum.Traitor)))
                        exeTargets.Add(player);
                }
                else if (player.Is(Faction.Neutral))
                {
                    if (!(player.Is(RoleEnum.Executioner) | player.Is(RoleEnum.Troll) | player.Is(RoleEnum.GuardianAngel)))
                    {
                        if (!player.Is(RoleEnum.Jester))
                        {
                            gaTargets.Add(player);
                            
                            if (player.Is(RoleAlignment.NeutralKill))
                                evilRecruits.Add(player);
                            else if (player.Is(RoleAlignment.NeutralEvil))
                                goodRecruits.Add(player);
                        }

                        if (CustomGameOptions.ExeCanHaveNeutralTargets)
                            exeTargets.Add(player);
                    }
                }
                else if (player.Is(Faction.Intruder) | player.Is(Faction.Syndicate))
                {
                    gaTargets.Add(player);
                    evilRecruits.Add(player);

                    if ((player.Is(Faction.Intruder) && CustomGameOptions.ExeCanHaveIntruderTargets) || (player.Is(Faction.Syndicate) && CustomGameOptions.ExeCanHaveSyndicateTargets))
                        exeTargets.Add(player);
                }
            }

            if (CustomGameOptions.ExecutionerOn > 0)
            {
                foreach (Executioner exe in Role.GetRoles(RoleEnum.Executioner))
                {
                    exe.TargetPlayer = null;

                    if (exeTargets.Count > 0)
                    {
                        while (exe.TargetPlayer == null | exe.TargetPlayer == exe.Player)
                        {
                            exeTargets.Shuffle();
                            var exeNum = Random.RandomRangeInt(0, exeTargets.Count - 1);
                            exe.TargetPlayer = exeTargets[exeNum];
                        }

                        exeTargets.Remove(exe.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetTarget,
                            SendOption.Reliable, -1);
                        writer.Write(exe.Player.PlayerId);
                        writer.Write(exe.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
            
            if (CustomGameOptions.GuardianAngelOn > 0)
            {
                foreach (GuardianAngel ga in Role.GetRoles(RoleEnum.GuardianAngel))
                {
                    ga.TargetPlayer = null;
                    
                    if (gaTargets.Count > 0)
                    {
                        while (ga.TargetPlayer == null | ga.TargetPlayer == ga.Player)
                        {
                            gaTargets.Shuffle();
                            var gaNum = Random.RandomRangeInt(0, gaTargets.Count - 1);
                            ga.TargetPlayer = gaTargets[gaNum];
                        }

                        gaTargets.Remove(ga.TargetPlayer);

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGATarget,
                            SendOption.Reliable, -1);
                        writer.Write(ga.Player.PlayerId);
                        writer.Write(ga.TargetPlayer.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }

            if (CustomGameOptions.JackalOn > 0)
            {
                foreach (Jackal jackal in Role.GetRoles(RoleEnum.Jackal))
                {
                    jackal.GoodRecruit = null;
                    jackal.EvilRecruit = null;
                    
                    if (goodRecruits.Count > 0)
                    {
                        while (jackal.GoodRecruit == null)
                        {
                            goodRecruits.Shuffle();
                            var goodNum = Random.RandomRangeInt(0, goodRecruits.Count - 1);
                            jackal.GoodRecruit = goodRecruits[goodNum];
                        }

                        goodRecruits.Remove(jackal.GoodRecruit);
                        (Role.GetRole(jackal.GoodRecruit)).SubFaction = SubFaction.Cabal;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGoodRecruit,
                            SendOption.Reliable, -1);
                        writer.Write(jackal.Player.PlayerId);
                        writer.Write(jackal.GoodRecruit.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                    
                    if (evilRecruits.Count > 0)
                    {
                        while (jackal.EvilRecruit == null)
                        {
                            evilRecruits.Shuffle();
                            var evilNum = Random.RandomRangeInt(0, evilRecruits.Count - 1);
                            jackal.EvilRecruit = evilRecruits[evilNum];
                        }

                        evilRecruits.Remove(jackal.EvilRecruit);
                        (Role.GetRole(jackal.EvilRecruit)).SubFaction = SubFaction.Cabal;

                        var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetEvilRecruit,
                            SendOption.Reliable, -1);
                        writer.Write(jackal.Player.PlayerId);
                        writer.Write(jackal.EvilRecruit.PlayerId);
                        AmongUsClient.Instance.FinishRpcImmediately(writer);
                    }
                }
            }
        }

        private static void GenEachRoleKilling(List<GameData.PlayerInfo> players)
        {
            var impostors = Utils.GetImpostors(players);
            var crewmates = Utils.GetCrewmates(impostors);

            crewmates.Shuffle();
            impostors.Shuffle();

            if (!CustomGameOptions.AltImps)
            {   
                IntruderRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn, true));
                IntruderRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn, false));
                IntruderRoles.Add((typeof(Blackmailer), CustomRPC.SetBlackmailer, CustomGameOptions.BlackmailerOn, true));
                IntruderRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn, true));
                IntruderRoles.Add((typeof(Wraith), CustomRPC.SetWraith, CustomGameOptions.WraithOn, false));
                IntruderRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, CustomGameOptions.GrenadierOn, true));
                IntruderRoles.Add((typeof(TimeMaster), CustomRPC.SetTimeMaster, CustomGameOptions.TimeMasterOn, true));
                IntruderRoles.Add((typeof(Disguiser), CustomRPC.SetDisguiser, CustomGameOptions.DisguiserOn, true));
                IntruderRoles.Add((typeof(Consigliere), CustomRPC.SetConsigliere, CustomGameOptions.ConsigliereOn, true));
                IntruderRoles.Add((typeof(Teleporter), CustomRPC.SetTeleporter, CustomGameOptions.TeleporterOn, false));
                IntruderRoles.Add((typeof(Impostor), CustomRPC.SetImpostor, 5, true));
                IntruderRoles.Add((typeof(Godfather), CustomRPC.SetGodfather, CustomGameOptions.GodfatherOn, true));

                SyndicateRoles.Add((typeof(Anarchist), CustomRPC.SetAnarchist, 5, true));
                SyndicateRoles.Add((typeof(Puppeteer), CustomRPC.SetPuppeteer, CustomGameOptions.PuppeteerOn, true));
                SyndicateRoles.Add((typeof(Concealer), CustomRPC.SetConcealer, CustomGameOptions.ConcealerOn, false));
                SyndicateRoles.Add((typeof(Shapeshifter), CustomRPC.SetShapeshifter, CustomGameOptions.ShapeshifterOn, true));
                SyndicateRoles.Add((typeof(Warper), CustomRPC.SetWarper, CustomGameOptions.WarperOn, true));
                SyndicateRoles.Add((typeof(Gorgon), CustomRPC.SetGorgon, CustomGameOptions.GorgonOn, false));
                SyndicateRoles.Add((typeof(Rebel), CustomRPC.SetRebel, CustomGameOptions.RebelOn, true));
            }
            else
            {
                IntruderRoles.Add((typeof(Puppeteer), CustomRPC.SetPuppeteer, CustomGameOptions.PuppeteerOn, true));
                IntruderRoles.Add((typeof(Concealer), CustomRPC.SetConcealer, CustomGameOptions.ConcealerOn, false));
                IntruderRoles.Add((typeof(Shapeshifter), CustomRPC.SetShapeshifter, CustomGameOptions.ShapeshifterOn, true));
                IntruderRoles.Add((typeof(Warper), CustomRPC.SetWarper, CustomGameOptions.WarperOn, true));
                IntruderRoles.Add((typeof(Gorgon), CustomRPC.SetGorgon, CustomGameOptions.GorgonOn, false));
                IntruderRoles.Add((typeof(Anarchist), CustomRPC.SetAnarchist, CustomGameOptions.AnarchistOn, true));
                IntruderRoles.Add((typeof(Rebel), CustomRPC.SetRebel, CustomGameOptions.RebelOn, true));
            }

            SortRoles(IntruderRoles, impostors.Count, impostors.Count);

            if (!CustomGameOptions.AltImps)
                SortRoles(SyndicateRoles, CustomGameOptions.SyndicateCount, CustomGameOptions.SyndicateCount);

            NeutralKillingRoles.Add((typeof(Glitch), CustomRPC.SetGlitch, CustomGameOptions.GlitchOn, true));
            NeutralKillingRoles.Add((typeof(Werewolf), CustomRPC.SetWerewolf, CustomGameOptions.WerewolfOn, true));
            NeutralKillingRoles.Add((typeof(SerialKiller), CustomRPC.SetSerialKiller, CustomGameOptions.SerialKillerOn, true));
            NeutralKillingRoles.Add((typeof(Juggernaut), CustomRPC.SetJuggernaut, CustomGameOptions.JuggernautOn, true));
            NeutralKillingRoles.Add((typeof(Murderer), CustomRPC.SetMurderer, CustomGameOptions.MurdererOn, true));

            if (CustomGameOptions.AddArsonist)
                NeutralKillingRoles.Add((typeof(Arsonist), CustomRPC.SetArsonist, CustomGameOptions.ArsonistOn, true));

            if (CustomGameOptions.AddCryomaniac)
                NeutralKillingRoles.Add((typeof(Cryomaniac), CustomRPC.SetCryomaniac, CustomGameOptions.CryomaniacOn, true));

            if (CustomGameOptions.AddPlaguebearer)
                NeutralKillingRoles.Add((typeof(Plaguebearer), CustomRPC.SetPlaguebearer, CustomGameOptions.PlaguebearerOn, true));

            NeutralKillingRoles.Shuffle();

            var neutrals = 0;

            if (NeutralKillingRoles.Count < CustomGameOptions.NeutralRoles)
                neutrals = NeutralKillingRoles.Count;
            else
                neutrals = CustomGameOptions.NeutralRoles;

            var spareCrew = crewmates.Count - neutrals;

            if (spareCrew > 2)
                SortRoles(NeutralKillingRoles, neutrals, neutrals);
            else
                SortRoles(NeutralKillingRoles, crewmates.Count - 3, crewmates.Count - 3);

            if (CrewRoles.Count + NeutralKillingRoles.Count > crewmates.Count)
                SortRoles(CrewRoles, crewmates.Count - NeutralKillingRoles.Count, crewmates.Count - NeutralKillingRoles.Count);
            else if (CrewRoles.Count + NeutralKillingRoles.Count < crewmates.Count)
            {
                int vigis = (crewmates.Count - NeutralKillingRoles.Count - CrewRoles.Count) / 2;
                int vets = (crewmates.Count - NeutralKillingRoles.Count - CrewRoles.Count) / 2;

                while (vigis > 0)
                {
                    CrewRoles.Add((typeof(Vigilante), CustomRPC.SetVigilante, 100, false));
                    vigis -= 1;
                    CrewRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, 100, false));
                    vets -= 1;
                }
            }

            var crewAndNeutralRoles = new List<(Type, CustomRPC, int, bool)>();
            crewAndNeutralRoles.AddRange(CrewRoles);
            crewAndNeutralRoles.AddRange(NeutralKillingRoles);

            if (!CustomGameOptions.AltImps)
                crewAndNeutralRoles.AddRange(SyndicateRoles);
                
            crewAndNeutralRoles.Shuffle();
            IntruderRoles.Shuffle();

            foreach (var (type, rpc, _, unique) in IntruderRoles)
                Role.Gen<Role>(type, impostors, rpc);

            foreach (var (type, rpc, _, unique) in crewAndNeutralRoles)
                Role.Gen<Role>(type, crewmates, rpc);
        }

        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;

                switch ((CustomRPC)callId)
                {
                    case CustomRPC.SetMayor:
                        new Mayor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetRebel:
                        new Rebel(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetJackal:
                        new Jackal(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetJester:
                        new Jester(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVigilante:
                        new Vigilante(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetConsigliere:
                        new Consigliere(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetEngineer:
                        new Engineer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetJanitor:
                        new Janitor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSwapper:
                        new Swapper(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetShifter:
                        new Shifter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCamouflager:
                        new Camouflager(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetAmnesiac:
                        new Amnesiac(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetInvestigator:
                        new Investigator(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTimeLord:
                        new TimeLord(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCannibal:
                        new Cannibal(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTorch:
                        new Torch(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetLighter:
                        new Lighter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDiseased:
                        new Diseased(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetBait:
                        new Bait(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDwarf:
                        new Dwarf(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMedic:
                        new Medic(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDracula:
                        new Dracula(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMorphling:
                        new Morphling(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetPoisoner:
                        new Poisoner(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDrunk:
                        new Drunk(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetConcealer:
                        new Concealer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTunneler:
                        new Tunneler(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetFlincher:
                        new Flincher(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCryomaniac:
                        new Cryomaniac(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetPuppeteer:
                        new Puppeteer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetThief:
                        new Thief(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.FreezeDouse:
                        var cryomaniac = Utils.PlayerById(reader.ReadByte());
                        var freezedouseTarget = Utils.PlayerById(reader.ReadByte());
                        var cryomaniacRole = Role.GetRole<Cryomaniac>(cryomaniac);
                        cryomaniacRole.DousedPlayers.Add(freezedouseTarget.PlayerId);
                        cryomaniacRole.LastDoused = DateTime.UtcNow;
                        break;

                    case CustomRPC.AllFreeze:
                        var theCryomaniac = Utils.PlayerById(reader.ReadByte());
                        var theCryomaniacRole = Role.GetRole<Cryomaniac>(theCryomaniac);
                        global::TownOfUsReworked.PlayerLayers.Roles.NeutralRoles.CryomaniacMod.PerformKill.Freeze(theCryomaniacRole);
                        break;

                    case CustomRPC.CryomaniacWin:
                        var theCryomaniacTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Cryomaniac);
                        ((Cryomaniac) theCryomaniacTheRole).Wins();
                        break;

                    case CustomRPC.CryomaniacLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Cryomaniac)
                                ((Cryomaniac) role).Loses();
                        }

                        break;

                    case CustomRPC.ThiefLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Thief)
                                ((Thief)role).Loses();
                        }

                        break;

                    case CustomRPC.TrollWin:
                        var theTrollTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Troll);
                        ((Troll) theTrollTheRole).Wins();
                        break;

                    case CustomRPC.TrollLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Troll)
                                ((Troll) role).Loses();
                        }

                        break;

                    case CustomRPC.LoveWin:
                        var winnerlover = Utils.PlayerById(reader.ReadByte());
                        Objectifier.GetObjectifier<Lovers>(winnerlover).Win();
                        break;

                    case CustomRPC.JesterLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Jester)
                                ((Jester)role).Loses();
                        }

                        break;

                    case CustomRPC.PhantomLose:
                        foreach (var objectifier in Objectifier.AllObjectifiers)
                        {
                            if (objectifier.ObjectifierType == ObjectifierEnum.Phantom)
                                ((Phantom)objectifier).Loses();
                        }

                        break;

                    case CustomRPC.GlitchLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Glitch)
                                ((Glitch)role).Loses();
                        }

                        break;

                    case CustomRPC.JuggernautLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Juggernaut)
                                ((Juggernaut)role).Loses();
                        }

                        break;

                    case CustomRPC.MurdererLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Murderer)
                                ((Murderer)role).Loses();
                        }
                        
                        break;

                    case CustomRPC.AmnesiacLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Amnesiac)
                                ((Amnesiac)role).Loses();
                        }

                        break;

                    case CustomRPC.ExecutionerLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Executioner)
                                ((Executioner)role).Loses();
                        }

                        break;

                    case CustomRPC.NobodyWins:
                        Role.NobodyWinsFunc();
                        break;

                    case CustomRPC.NeutralsWin:
                        Role.NeutralsOnlyWin();
                        break;

                    case CustomRPC.SetCouple:
                        var id = reader.ReadByte();
                        var id2 = reader.ReadByte();
                        var lover1 = Utils.PlayerById(id);
                        var lover2 = Utils.PlayerById(id2);
                        var objectifierLover1 = new Lovers(lover1);
                        var objectifierLover2 = new Lovers(lover2);
                        objectifierLover1.OtherLover = objectifierLover2;
                        objectifierLover2.OtherLover = objectifierLover1;
                        break;

                    case CustomRPC.Start:
                        Utils.ShowDeadBodies = false;

                        Role.NobodyWins = false;
                        Role.NeutralsWin = false;

                        Role.CrewWin = false;
                        Role.SyndicateWin = false;
                        Role.IntruderWin = false;
                        
                        Role.UndeadWin = false;
                        Role.CabalWin = false;

                        CrewRoles.Clear();
                        NeutralNonKillingRoles.Clear();
                        NeutralKillingRoles.Clear();
                        IntruderRoles.Clear();
                        SyndicateRoles.Clear();
                        NeutralChaosRoles.Clear();

                        GlobalModifiers.Clear();
                        BaitModifiers.Clear();
                        DiseasedModifiers.Clear();
                        ProfessionalModifiers.Clear();

                        CrewAbilityGet.Clear();
                        GlobalAbilityGet.Clear();
                        IntruderAbilityGet.Clear();
                        CrewAbilityGet.Clear();
                        SyndicateAbilityGet.Clear();
                        NeutralAbilityGet.Clear();
                        EvilAbilityGet.Clear();
                        NonEvilAbilityGet.Clear();

                        CrewObjectifierGet.Clear();
                        NeutralObjectifierGet.Clear();
                        GlobalObjectifierGet.Clear();
                        
                        RecordRewind.points.Clear();
                        Murder.KilledPlayers.Clear();

                        PlayerLayers.Roles.CrewRoles.AltruistMod.KillButtonTarget.DontRevive = byte.MaxValue;
                        break;

                    case CustomRPC.JanitorClean:
                        readByte1 = reader.ReadByte();
                        var janitorPlayer = Utils.PlayerById(readByte1);
                        var janitorRole = Role.GetRole<Janitor>(janitorPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in deadBodies)
                        {
                            if (body.ParentId == readByte)
                                Coroutines.Start(Coroutine.CleanCoroutine(body, janitorRole));
                        }

                        break;

                    case CustomRPC.EngineerFix:
                        var engineer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Engineer>(engineer).UsedThisRound = true;
                        break;

                    case CustomRPC.FixLights:
                        var lights = ShipStatus.Instance.Systems[SystemTypes.Electrical].Cast<SwitchSystem>();
                        lights.ActualSwitches = lights.ExpectedSwitches;
                        break;

                    case CustomRPC.SetExtraVotes:
                        var mayor = Utils.PlayerById(reader.ReadByte());
                        var mayorRole = Role.GetRole<Mayor>(mayor);
                        mayorRole.ExtraVotes = reader.ReadBytesAndSize().ToList();

                        if (!mayor.Is(RoleEnum.Mayor))
                            mayorRole.VoteBank -= mayorRole.ExtraVotes.Count;

                        break;

                    case CustomRPC.SetSwaps:
                        readSByte = reader.ReadSByte();
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                        readSByte2 = reader.ReadSByte();
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Bytes received - " + readSByte + " - " + readSByte2);
                        break;

                    case CustomRPC.Remember:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var amnesiac = Utils.PlayerById(readByte1);
                        var other = Utils.PlayerById(readByte2);
                        PerformKillButton.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
                        break;

                    case CustomRPC.Declare:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var gf = Utils.PlayerById(readByte1);
                        var maf = Utils.PlayerById(readByte2);
                        PerformDeclareButton.Declare(Role.GetRole<Godfather>(gf), maf);
                        break;

                    case CustomRPC.Sidekick:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var reb = Utils.PlayerById(readByte1);
                        var side = Utils.PlayerById(readByte2);
                        PerformSidekickButton.Sidekick(Role.GetRole<Rebel>(reb), side);
                        break;

                    case CustomRPC.Shift:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var shifter = Utils.PlayerById(readByte1);
                        var other2 = Utils.PlayerById(readByte2);
                        PerformShiftButton.Shift(Role.GetRole<Shifter>(shifter), other2);
                        break;

                    case CustomRPC.Convert:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var drac = Utils.PlayerById(readByte1);
                        var other3 = Utils.PlayerById(readByte2);
                        PerformConvertButton.Convert(Role.GetRole<Dracula>(drac), other3);
                        break;
                    
                    case CustomRPC.SetTeleporter:
                        new Teleporter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Teleport:
                        var teleporter = Utils.PlayerById(reader.ReadByte());
                        var teleporterRole = Role.GetRole<Teleporter>(teleporter);
                        var teleportPos = reader.ReadVector2();
                        teleporterRole.TeleportPoint = teleportPos;
                        Teleporter.Teleport(teleporter);
                        break;

                    case CustomRPC.Rewind:
                        readByte = reader.ReadByte();
                        var TimeLordPlayer = Utils.PlayerById(readByte);
                        var TimeLordRole = Role.GetRole<TimeLord>(TimeLordPlayer);
                        StartStop.StartRewind(TimeLordRole);
                        break;

                    case CustomRPC.Protect:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var medic = Utils.PlayerById(readByte1);
                        var shield = Utils.PlayerById(readByte2);
                        Role.GetRole<Medic>(medic).ShieldedPlayer = shield;
                        Role.GetRole<Medic>(medic).UsedAbility = true;
                        break;

                    case CustomRPC.RewindRevive:
                        readByte = reader.ReadByte();
                        RecordRewind.ReviveBody(Utils.PlayerById(readByte));
                        break;

                    case CustomRPC.AttemptSound:
                        var medicId = reader.ReadByte();
                        readByte = reader.ReadByte();
                        StopKill.BreakShield(medicId, readByte, CustomGameOptions.ShieldBreaks);
                        break;

                    case CustomRPC.SetGlitch:
                        new Glitch(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetJuggernaut:
                        new Juggernaut(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMurderer:
                        new Murderer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.BypassKill:
                        var killer = Utils.PlayerById(reader.ReadByte());
                        var TargetPlayer = Utils.PlayerById(reader.ReadByte());
                        Utils.MurderPlayer(killer, TargetPlayer);
                        break;

                    case CustomRPC.AssassinKill:
                        var toDie = Utils.PlayerById(reader.ReadByte());
                        var assassin = Ability.GetAbilityValue<Assassin>(AbilityEnum.Assassin);
                        AssassinKill.MurderPlayer(assassin, toDie);
                        break;

                    case CustomRPC.Mimic:
                        var glitchPlayer = Utils.PlayerById(reader.ReadByte());
                        var mimicPlayer = Utils.PlayerById(reader.ReadByte());
                        var glitchRole = Role.GetRole<Glitch>(glitchPlayer);
                        glitchRole.TimeRemaining = CustomGameOptions.MimicDuration;
                        glitchRole.MimicTarget = mimicPlayer;
                        break;

                    case CustomRPC.GlitchWin:
                        var theGlitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                        ((Glitch)theGlitch).Wins();
                        break;

                    case CustomRPC.JuggernautWin:
                        var juggernaut = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Juggernaut);
                        ((Juggernaut)juggernaut).Wins();
                        break;

                    case CustomRPC.MurdererWin:
                        var murd = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Murderer);
                        ((Murderer)murd).Wins();
                        break;

                    case CustomRPC.CrewWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Crew)
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.CrewLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Crew)
                                role.Loses();
                        }

                        break;

                    case CustomRPC.UndeadWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.SubFaction == SubFaction.Undead)
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.UndeadLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.SubFaction == SubFaction.Undead)
                                role.Loses();
                        }

                        break;

                    case CustomRPC.IntruderWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Intruder)
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.IntruderLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Intruder)
                                role.Loses();
                        }

                        break;

                    case CustomRPC.SyndicateWin:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Syndicate)
                                role.Wins();
                        }
                            
                        break;

                    case CustomRPC.SyndicateLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.Faction == Faction.Syndicate)
                                role.Loses();
                        }

                        break;

                    case CustomRPC.Hack:
                        var hackPlayer = Utils.PlayerById(reader.ReadByte());

                        if (hackPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                            Utils.Block(glitch.Player, hackPlayer);
                        }

                        break;

                    case CustomRPC.Interrogate:
                        var sheriff = Utils.PlayerById(reader.ReadByte());
                        var otherPlayer = Utils.PlayerById(reader.ReadByte());
                        var sheriffRole = Role.GetRole<Sheriff>(sheriff);
                        sheriffRole.Interrogated.Add(otherPlayer.PlayerId);
                        sheriffRole.LastInterrogated = DateTime.UtcNow;
                        sheriffRole.UsedThisRound = true;
                        break;

                    case CustomRPC.InspExamine:
                        var inspector = Utils.PlayerById(reader.ReadByte());
                        var otherPlayer1 = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Inspector>(inspector).Examined.Add(otherPlayer1);
                        Role.GetRole<Inspector>(inspector).LastExamined = DateTime.UtcNow;
                        break;

                    case CustomRPC.SetSheriff:
                        new Sheriff(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Investigate:
                        var consig = Utils.PlayerById(reader.ReadByte());
                        var other22 = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Consigliere>(consig).Investigated.Add(other22.PlayerId);
                        Role.GetRole<Consigliere>(consig).LastInvestigated = DateTime.UtcNow;
                        break;

                    case CustomRPC.Morph:
                        var morphling = Utils.PlayerById(reader.ReadByte());
                        var morphTarget = Utils.PlayerById(reader.ReadByte());
                        var morphRole = Role.GetRole<Morphling>(morphling);
                        morphRole.TimeRemaining = CustomGameOptions.MorphlingDuration;
                        morphRole.MorphedPlayer = morphTarget;
                        break;

                    case CustomRPC.Disguise:
                        var disguiser = Utils.PlayerById(reader.ReadByte());
                        var disguiseTarget = Utils.PlayerById(reader.ReadByte());
                        var disguiseRole = Role.GetRole<Disguiser>(disguiser);
                        disguiseRole.TimeRemaining = CustomGameOptions.DisguiseDuration;
                        disguiseRole.DisguisedPlayer = disguiseTarget;
                        disguiseRole.LastDisguised = DateTime.UtcNow;
                        break;

                    case CustomRPC.Poison:
                        var poisoner = Utils.PlayerById(reader.ReadByte());
                        var poisoned = Utils.PlayerById(reader.ReadByte());
                        var poisonerRole = Role.GetRole<Poisoner>(poisoner);
                        poisonerRole.PoisonedPlayer = poisoned;
                        break;

                    case CustomRPC.SetExecutioner:
                        new Executioner(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetEscort:
                        new Escort(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetConsort:
                        new Consort(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTroll:
                        new Troll(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetGuardianAngel:
                        new GuardianAngel(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetGodfather:
                        new Godfather(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTarget:
                        var exe = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(exe);
                        exeRole.TargetPlayer = exeTarget;
                        break;

                    case CustomRPC.SetGATarget:
                        var ga = Utils.PlayerById(reader.ReadByte());
                        var gaTarget = Utils.PlayerById(reader.ReadByte());
                        var gaRole = Role.GetRole<GuardianAngel>(ga);
                        gaRole.TargetPlayer = gaTarget;
                        break;

                    case CustomRPC.SetGoodRecruit:
                        var jackal = Utils.PlayerById(reader.ReadByte());
                        var goodRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole = Role.GetRole<Jackal>(jackal);
                        jackalRole.GoodRecruit = goodRecruit;
                        (Role.GetRole(goodRecruit)).SubFaction = SubFaction.Cabal;
                        break;

                    case CustomRPC.SetEvilRecruit:
                        var jackal2 = Utils.PlayerById(reader.ReadByte());
                        var evilRecruit = Utils.PlayerById(reader.ReadByte());
                        var jackalRole2 = Role.GetRole<Jackal>(jackal2);
                        jackalRole2.GoodRecruit = evilRecruit;
                        (Role.GetRole(evilRecruit)).SubFaction = SubFaction.Cabal;
                        break;

                    case CustomRPC.SetBlackmailer:
                        new Blackmailer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVampireHunter:
                        new VampireHunter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Blackmail:
                        var blackmailer = Role.GetRole<Blackmailer>(Utils.PlayerById(reader.ReadByte()));
                        blackmailer.Blackmailed = Utils.PlayerById(reader.ReadByte());
                        break;

                    case CustomRPC.SetAgent:
                        new Agent(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTaskmaster:
                        new Taskmaster(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.ExeToJest:
                        TargetColor.ExeToJes(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.GAToSurv:
                        GATargetColor.GAToSurv(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSnitch:
                        new Snitch(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTracker:
                        new Tracker(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDetective:
                        new Detective(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSurvivor:
                        new Survivor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMiner:
                        new Miner(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Mine:
                        var ventId = reader.ReadInt32();
                        var miner = Utils.PlayerById(reader.ReadByte());
                        var minerRole = Role.GetRole<Miner>(miner);
                        var pos = reader.ReadVector2();
                        var zAxis = reader.ReadSingle();
                        PlayerLayers.Roles.IntruderRoles.MinerMod.PerformKill.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;

                    case CustomRPC.TimeFreeze:
                        var tm = Utils.PlayerById(reader.ReadByte());
                        var tmRole = Role.GetRole<TimeMaster>(tm);
                        tmRole.TimeRemaining = CustomGameOptions.FreezeDuration;
                        tmRole.TimeFreeze();
                        break;

                    case CustomRPC.SetTimeMaster:
                        new TimeMaster(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetWraith:
                        new Wraith(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Invis:
                        var wraith = Utils.PlayerById(reader.ReadByte());
                        var wraithRole = Role.GetRole<Wraith>(wraith);
                        wraithRole.TimeRemaining = CustomGameOptions.InvisDuration;
                        wraithRole.Invis();
                        break;

                    case CustomRPC.Alert:
                        var veteran = Utils.PlayerById(reader.ReadByte());
                        var veteranRole = Role.GetRole<Veteran>(veteran);
                        veteranRole.TimeRemaining = CustomGameOptions.AlertDuration;
                        veteranRole.Alert();
                        break;

                    case CustomRPC.Vest:
                        var surv = Utils.PlayerById(reader.ReadByte());
                        var survRole = Role.GetRole<Survivor>(surv);
                        survRole.TimeRemaining = CustomGameOptions.VestDuration;
                        survRole.Vest();
                        break;

                    case CustomRPC.GAProtect:
                        var ga2 = Utils.PlayerById(reader.ReadByte());
                        var ga2Role = Role.GetRole<GuardianAngel>(ga2);
                        ga2Role.TimeRemaining = CustomGameOptions.ProtectDuration;
                        ga2Role.Protect();
                        break;

                    case CustomRPC.Transport:
                        Coroutines.Start(Transporter.TransportPlayers(reader.ReadByte(), reader.ReadByte(), reader.ReadBoolean()));
                        break;

                    case CustomRPC.SetUntransportable:
                        if (PlayerControl.LocalPlayer.Is(RoleEnum.Transporter))
                            Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                        break;

                    case CustomRPC.Mediate:
                        var mediatedPlayer = Utils.PlayerById(reader.ReadByte());
                        var medium = Role.GetRole<Medium>(Utils.PlayerById(reader.ReadByte()));

                        if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer.PlayerId)
                            break;

                        medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                        break;

                    case CustomRPC.SetGrenadier:
                        new Grenadier(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDisguiser:
                        new Disguiser(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetWerewolf:
                        new Werewolf(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.FlashGrenade:
                        var grenadier = Utils.PlayerById(reader.ReadByte());
                        var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                        grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                        grenadierRole.Flash();
                        break;

                    case CustomRPC.Maul:
                        var ww = Utils.PlayerById(reader.ReadByte());
                        var wwRole = Role.GetRole<Werewolf>(ww);
                        wwRole.Maul(wwRole.Player);
                        break;

                    case CustomRPC.SetTiebreaker:
                        new Tiebreaker(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCoward:
                        new Coward(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVolatile:
                        new Volatile(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetArsonist:
                        new Arsonist(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetSerialKiller:
                        new SerialKiller(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Douse:
                        var arsonist = Utils.PlayerById(reader.ReadByte());
                        var douseTarget = Utils.PlayerById(reader.ReadByte());
                        var arsonistRole = Role.GetRole<Arsonist>(arsonist);
                        arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                        arsonistRole.LastDoused = DateTime.UtcNow;
                        arsonistRole.LastIgnited = DateTime.UtcNow;
                        break;

                    case CustomRPC.Ignite:
                        var theArsonist = Utils.PlayerById(reader.ReadByte());
                        var theArsonistRole = Role.GetRole<Arsonist>(theArsonist);
                        theArsonistRole.Ignite();
                        theArsonistRole.LastIgnited = DateTime.UtcNow;
                        break;

                    case CustomRPC.ArsonistWin:
                        var theArsonistTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist);
                        ((Arsonist)theArsonistTheRole).Wins();
                        break;

                    case CustomRPC.ArsonistLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Arsonist)
                                ((Arsonist)role).Loses();
                        }

                        break;

                    case CustomRPC.SerialKillerWin:
                        var theSerialTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.SerialKiller);
                        ((SerialKiller)theSerialTheRole).Wins();
                        break;

                    case CustomRPC.SerialKillerLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.SerialKiller)
                                ((SerialKiller)role).Loses();
                        }

                        break;

                    case CustomRPC.SurvivorLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Survivor && role.Player.Data.IsDead | role.Player.Data.Disconnected)
                                ((Survivor)role).Loses();
                        }

                        break;

                    case CustomRPC.GALose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.GuardianAngel && ((GuardianAngel)role).TargetPlayer.Data.IsDead)
                                ((GuardianAngel)role).Loses();
                        }

                        break;

                    case CustomRPC.PlaguebearerWin:
                        var thePlaguebearerTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Plaguebearer);
                        ((Plaguebearer)thePlaguebearerTheRole).Wins();
                        break;

                    case CustomRPC.PlaguebearerLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Plaguebearer)
                                ((Plaguebearer)role).Loses();
                        }

                        break;

                    case CustomRPC.CannibalEat:
                        readByte1 = reader.ReadByte();
                        var cannibalPlayer = Utils.PlayerById(readByte1);
                        var cannibalRole = Role.GetRole<Cannibal>(cannibalPlayer);
                        readByte = reader.ReadByte();
                        var deads = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in deads)
                        {
                            if (body.ParentId == readByte)
                                Coroutines.Start(Eat.EatCoroutine(body, cannibalRole));
                        }

                        cannibalRole.LastEaten = DateTime.UtcNow;
                        break;

                    case CustomRPC.CannibalWin:
                        var theCannibalRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Cannibal);
                        ((Cannibal)theCannibalRole).Wins();
                        break;

                    case CustomRPC.CannibalLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Cannibal)
                                ((Cannibal)role).Loses();
                        }

                        break;

                    case CustomRPC.Infect:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).InfectedPlayers.Add(reader.ReadByte());
                        break;

                    case CustomRPC.TurnPestilence:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                        break;

                    case CustomRPC.TurnVigilante:
                        Role.GetRole<VampireHunter>(Utils.PlayerById(reader.ReadByte())).TurnVigilante();
                        break;

                    case CustomRPC.TurnRebel:
                        Role.GetRole<Sidekick>(Utils.PlayerById(reader.ReadByte())).TurnRebel();
                        break;

                    case CustomRPC.TurnGodfather:
                        Role.GetRole<Mafioso>(Utils.PlayerById(reader.ReadByte())).TurnGodfather();
                        break;

                    case CustomRPC.PestilenceWin:
                        var thePestilenceTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pestilence);
                        ((Pestilence)thePestilenceTheRole).Wins();
                        break;

                    case CustomRPC.PestilenceLose:
                        foreach (var role in Role.AllRoles)
                        {
                            if (role.RoleType == RoleEnum.Pestilence)
                                ((Pestilence)role).Loses();
                        }

                        break;

                    case CustomRPC.SetImpostor:
                        new Impostor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCrewmate:
                        new Crewmate(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetAnarchist:
                        new Anarchist(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetWarper:
                        new Warper(Utils.PlayerById(reader.ReadByte()));
                        break;
                        
                    case CustomRPC.SetRadar:
                        new Radar(Utils.PlayerById(reader.ReadByte()));
                        break;
                        
                    case CustomRPC.SetInsepctor:
                        new Inspector(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SyncCustomSettings:
                        Rpc.ReceiveRpc(reader);
                        break;

                    case CustomRPC.SetAltruist:
                        new Altruist(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetGiant:
                        new Giant(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.AltruistRevive:
                        readByte1 = reader.ReadByte();
                        var altruistPlayer = Utils.PlayerById(readByte1);
                        var altruistRole = Role.GetRole<Altruist>(altruistPlayer);
                        readByte = reader.ReadByte();
                        var theDeadBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in theDeadBodies)
                        {
                            if (body.ParentId == readByte)
                            {
                                if (body.ParentId == PlayerControl.LocalPlayer.PlayerId)
                                    Coroutines.Start(Utils.FlashCoroutine(altruistRole.Color));

                                Coroutines.Start(Revive.AltruistRevive(body, altruistRole));
                            }
                        }

                        break;

                    case CustomRPC.Warp:
                        byte teleports = reader.ReadByte();
                        Dictionary<byte, Vector2> coordinates = new Dictionary<byte, Vector2>();

                        for (int i = 0; i < teleports; i++)
                        {
                            byte playerId = reader.ReadByte();
                            Vector2 location = reader.ReadVector2();
                            coordinates.Add(playerId, location);
                        }

                        Warper.WarpPlayersToCoordinates(coordinates);
                        break;

                    case CustomRPC.FixAnimation:
                        var player = Utils.PlayerById(reader.ReadByte());
                        player.MyPhysics.ResetMoveState();
                        player.Collider.enabled = true;
                        player.moveable = true;
                        player.NetTransform.enabled = true;
                        break;

                    case CustomRPC.SetButtonBarry:
                        new ButtonBarry(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.BarryButton:
                        var buttonBarry = Utils.PlayerById(reader.ReadByte());

                        if (AmongUsClient.Instance.AmHost)
                        {
                            MeetingRoomManager.Instance.reporter = buttonBarry;
                            MeetingRoomManager.Instance.target = null;
                            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance.Cast<IDisconnectHandler>());

                            if (ShipStatus.Instance.CheckTaskCompletion())
                                return;

                            DestroyableSingleton<HudManager>.Instance.OpenMeetingRoom(buttonBarry);
                            buttonBarry.RpcStartMeeting(null);
                        }

                        break;

                    case CustomRPC.BaitReport:
                        var baitKiller = Utils.PlayerById(reader.ReadByte());
                        var bait = Utils.PlayerById(reader.ReadByte());
                        baitKiller.ReportDeadBody(bait.Data);
                        break;

                    case CustomRPC.CheckMurder:
                        var murderKiller = Utils.PlayerById(reader.ReadByte());
                        var murderTarget = Utils.PlayerById(reader.ReadByte());
                        murderKiller.CheckMurder(murderTarget);
                        break;

                    case CustomRPC.SetUndertaker:
                        new Undertaker(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Drag:
                        readByte1 = reader.ReadByte();
                        var dienerPlayer = Utils.PlayerById(readByte1);
                        var dienerRole = Role.GetRole<Undertaker>(dienerPlayer);
                        readByte = reader.ReadByte();
                        var dienerBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in dienerBodies)
                        {
                            if (body.ParentId == readByte)
                                dienerRole.CurrentlyDragging = body;
                        }

                        break;

                    case CustomRPC.Drop:
                        readByte1 = reader.ReadByte();
                        var v2 = reader.ReadVector2();
                        var v2z = reader.ReadSingle();
                        var dienerPlayer2 = Utils.PlayerById(readByte1);
                        var dienerRole2 = Role.GetRole<Undertaker>(dienerPlayer2);
                        var body2 = dienerRole2.CurrentlyDragging;
                        dienerRole2.CurrentlyDragging = null;
                        body2.transform.position = new Vector3(v2.x, v2.y, v2z);
                        break;

                    case CustomRPC.SetAssassin:
                        new Assassin(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVeteran:
                        new Veteran(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTransporter:
                        new Transporter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetMedium:
                        new Medium(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCoroner:
                        new Coroner(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetUnderdog:
                        new Underdog(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetPhantom:
                        readByte = reader.ReadByte();
                        SetPhantom.WillBePhantom = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.PhantomDied:
                        var phantom = Utils.PlayerById(reader.ReadByte());
                        Role.RoleDictionary.Remove(phantom.PlayerId);
                        var phantomRole = new Phantom(phantom);
                        phantomRole.RegenTask();
                        phantom.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetPhantom.RemoveTasks(phantom);
                        SetPhantom.AddCollider(phantomRole);

                        if (!PlayerControl.LocalPlayer.Is(AbilityEnum.Revealer))
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                        System.Console.WriteLine("Become Phantom - Users");
                        break;

                    case CustomRPC.CatchPhantom:
                        var phantomPlayer = Utils.PlayerById(reader.ReadByte());
                        Objectifier.GetObjectifier<Phantom>(phantomPlayer).Caught = true;
                        break;

                    case CustomRPC.PhantomWin:
                        Objectifier.GetObjectifier<Phantom>(Utils.PlayerById(reader.ReadByte())).CompletedTasks = true;
                        break;

                    case CustomRPC.SetRevealer:
                        readByte = reader.ReadByte();
                        SetRevealer.WillBeRevealer = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.RevealerDied:
                        var haunter = Utils.PlayerById(reader.ReadByte());
                        Role.RoleDictionary.Remove(haunter.PlayerId);
                        var haunterRole = new Revealer(haunter);
                        haunterRole.RegenTask();
                        haunter.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetRevealer.RemoveTasks(haunter);
                        SetRevealer.AddCollider(haunterRole);

                        if (!PlayerControl.LocalPlayer.Is(ObjectifierEnum.Phantom))
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();

                        System.Console.WriteLine("Become Revealer - Users");
                        break;

                    case CustomRPC.CatchRevealer:
                        var haunterPlayer = Utils.PlayerById(reader.ReadByte());
                        Ability.GetAbility<Revealer>(haunterPlayer).Caught = true;
                        break;

                    case CustomRPC.RevealerFinished:
                        Reveal.HighlightImpostors.UpdateMeeting(MeetingHud.Instance);
                        break;

                    case CustomRPC.SetTraitor:
                        new Traitor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetPlaguebearer:
                        new Plaguebearer(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetInsider:
                        new Insider(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetFanatic:
                        new Fanatic(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetOperative:
                        new Operative(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.AddMayorVoteBank:
                        Role.GetRole<Mayor>(Utils.PlayerById(reader.ReadByte())).VoteBank += reader.ReadInt32();
                        break;

                    case CustomRPC.RemoveAllBodies:
                        var buggedBodies = Object.FindObjectsOfType<DeadBody>();

                        foreach (var body in buggedBodies)
                            body.gameObject.Destroy();

                        break;

                    case CustomRPC.SubmergedFixOxygen:
                        SubmergedCompatibility.RepairOxygen();
                        break;

                    case CustomRPC.SetPos:
                        var setplayer = Utils.PlayerById(reader.ReadByte());
                        setplayer.transform.position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), setplayer.transform.position.z);
                        break;

                    case CustomRPC.SetSettings:
                        readByte = reader.ReadByte();
                        PlayerControl.GameOptions.MapId = readByte == byte.MaxValue ? (byte)0 : readByte;
                        PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Scientist, 0, 0);
                        PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Engineer, 0, 0);
                        PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.GuardianAngel, 0, 0);
                        PlayerControl.GameOptions.RoleOptions.SetRoleRate(RoleTypes.Shapeshifter, 0, 0);
                        PlayerControl.GameOptions.CrewLightMod = CustomGameOptions.CrewVision;
                        PlayerControl.GameOptions.ImpostorLightMod = CustomGameOptions.IntruderVision;
                        PlayerControl.GameOptions.AnonymousVotes = CustomGameOptions.AnonymousVoting;
                        PlayerControl.GameOptions.VisualTasks = CustomGameOptions.VisualTasks;
                        PlayerControl.GameOptions.PlayerSpeedMod = CustomGameOptions.PlayerSpeed;
                        PlayerControl.GameOptions.NumImpostors = CustomGameOptions.IntruderCount;
                        PlayerControl.GameOptions.GhostsDoTasks = CustomGameOptions.GhostTasksCountToWin;
                        PlayerControl.GameOptions.TaskBarMode = (TaskBarMode)CustomGameOptions.TaskBarMode;
                        PlayerControl.GameOptions.ConfirmImpostor = CustomGameOptions.ConfirmEjects;
                        PlayerControl.GameOptions.VotingTime = CustomGameOptions.VotingTime;
                        PlayerControl.GameOptions.DiscussionTime = CustomGameOptions.DiscussionTime;
                        PlayerControl.GameOptions.KillDistance = CustomGameOptions.InteractionDistance;

                        if (CustomGameOptions.AutoAdjustSettings)
                            RandomMap.AdjustSettings(readByte);

                        break;

                    case CustomRPC.Camouflage:
                        var camouflager = Utils.PlayerById(reader.ReadByte());
                        var camouflagerRole = Role.GetRole<Camouflager>(camouflager);
                        camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                        Utils.Camouflage();
                        break;

                    case CustomRPC.Conceal:
                        var concealer = Utils.PlayerById(reader.ReadByte());
                        var concealerRole = Role.GetRole<Concealer>(concealer);
                        concealerRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                        Utils.Conceal();
                        break;

                    case CustomRPC.Shapeshift:
                        var ss = Utils.PlayerById(reader.ReadByte());
                        var ssRole = Role.GetRole<Concealer>(ss);
                        ssRole.TimeRemaining = CustomGameOptions.ConcealDuration;
                        Utils.Shapeshift();
                        break;

                    case CustomRPC.Possess:
                        var puppeteer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Puppeteer>(puppeteer).PossessPlayer = Utils.PlayerById(reader.ReadByte());
                        break;

                    case CustomRPC.UnPossess:
                        var puppeteer3 = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Puppeteer>(puppeteer3).UnPossess();
                        break;

                    case CustomRPC.PossessKill:
                        var puppeteer2 = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Puppeteer>(puppeteer2).KillUnPossess();
                        break;

                    case CustomRPC.Gaze:
                        readByte = reader.ReadByte();
                        readByte1 = reader.ReadByte();
                        var gorgon = Role.GetRole<Gorgon>(Utils.PlayerById(readByte));
                        gorgon.gazeList.Add(readByte1, 0);

                        if (readByte1 == PlayerControl.LocalPlayer.PlayerId)
                        {
                            PlayerControl.LocalPlayer.moveable = false;
                            PlayerControl.LocalPlayer.NetTransform.Halt();
                            ImportantTextTask freezeText;
                            freezeText = new GameObject("_Player").AddComponent<ImportantTextTask>();
                            freezeText.transform.SetParent(PlayerControl.LocalPlayer.transform, false);
                            freezeText.Text = "<color=#00ACC2FF>STONED</color>";
                            freezeText.Index = PlayerControl.LocalPlayer.PlayerId;
                            PlayerControl.LocalPlayer.myTasks.Insert(0, freezeText);
                            ShipStatus.Instance.StartCoroutine(Effects.SwayX(Camera.main.transform, 0.75f, 0.25f));
                        }

                        Utils.AirKill(gorgon.Player, Utils.PlayerById(readByte1));
                        break;

                    case CustomRPC.SetGorgon:
                        new Gorgon(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetShapeshifter:
                        new Shapeshifter(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetVIP:
                        new VIP(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetShy:
                        new Shy(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetProfessional:
                        new Gorgon(Utils.PlayerById(reader.ReadByte()));
                        break;
                    
                    case CustomRPC.Sendchat:
                        string report = reader.ReadString();
                        DestroyableSingleton<HudManager>.Instance.Chat.AddChat(PlayerControl.LocalPlayer, report);
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class RpcSetRole
        {
            public static void Postfix()
            {
                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("RPC SET ROLE");
                var infected = GameData.Instance.AllPlayers.ToArray().Where(o => o.IsImpostor());

                Utils.ShowDeadBodies = false;

                Role.NobodyWins = false;
                Role.NeutralsWin = false;

                Role.CrewWin = false;
                Role.SyndicateWin = false;
                Role.IntruderWin = false;
                
                Role.UndeadWin = false;
                Role.CabalWin = false;

                CrewRoles.Clear();
                NeutralNonKillingRoles.Clear();
                NeutralKillingRoles.Clear();
                IntruderRoles.Clear();
                SyndicateRoles.Clear();
                NeutralChaosRoles.Clear();

                GlobalModifiers.Clear();
                BaitModifiers.Clear();
                DiseasedModifiers.Clear();
                ProfessionalModifiers.Clear();

                CrewAbilityGet.Clear();
                GlobalAbilityGet.Clear();
                IntruderAbilityGet.Clear();
                CrewAbilityGet.Clear();
                SyndicateAbilityGet.Clear();
                NeutralAbilityGet.Clear();
                EvilAbilityGet.Clear();
                NonEvilAbilityGet.Clear();

                CrewObjectifierGet.Clear();
                NeutralObjectifierGet.Clear();
                GlobalObjectifierGet.Clear();

                RecordRewind.points.Clear();
                Murder.KilledPlayers.Clear();

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cleared Lists");

                PlayerLayers.Roles.CrewRoles.AltruistMod.KillButtonTarget.DontRevive = byte.MaxValue;

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cleared Functions");

                unchecked
                {
                    var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.Start,
                        SendOption.Reliable, -1);
                    AmongUsClient.Instance.FinishRpcImmediately(startWriter);
                }

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Unchecked Write Line");

                var customgflag = CustomGameOptions.GameMode == GameMode.Custom;
                int num2 = 0;

                if (CustomGameOptions.GameMode != GameMode.KillingOnly)
                {
                    #region Crew Roles
                    if (CustomGameOptions.MayorOn > 0)
                    {
                        var number = customgflag ? CustomGameOptions.MayorCount : 1;

                        while (number > 0)
                        {
                            CrewRoles.Add((typeof(Mayor), CustomRPC.SetMayor, CustomGameOptions.MayorOn, true));
                            number--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Mayor Done");
                    }

                    if (CustomGameOptions.SheriffOn > 0)
                    {
                        var number = customgflag ? CustomGameOptions.SheriffCount : 1;

                        while (number > 0)
                        {
                            CrewRoles.Add((typeof(Sheriff), CustomRPC.SetSheriff, CustomGameOptions.SheriffOn, true));
                            number--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Sheriff Done");
                    }

                    if (CustomGameOptions.InspectorOn > 0)
                    {
                        var number = customgflag ? CustomGameOptions.InspectorOn : 1;

                        while (number > 0)
                        {
                            CrewRoles.Add((typeof(Inspector), CustomRPC.SetInsepctor, CustomGameOptions.InspectorOn, true));
                            number--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Inspector Done");
                    }

                    if (CustomGameOptions.VigilanteOn > 0)
                    {
                        var number = CustomGameOptions.VigilanteCount;

                        while (number > 0)
                        {
                            CrewRoles.Add((typeof(Vigilante), CustomRPC.SetVigilante, CustomGameOptions.VigilanteOn, true));
                            number--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Vigilante Done");
                    }

                    if (CustomGameOptions.EngineerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.EngineerCount;

                            do
                            {
                                CrewRoles.Add((typeof(Engineer), CustomRPC.SetEngineer, CustomGameOptions.EngineerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Engineer), CustomRPC.SetEngineer, CustomGameOptions.EngineerOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Engineer Done");
                    }

                    if (CustomGameOptions.SwapperOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.SwapperCount;

                            do
                            {
                                CrewRoles.Add((typeof(Swapper), CustomRPC.SetSwapper, CustomGameOptions.SwapperOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Swapper), CustomRPC.SetSwapper, CustomGameOptions.SwapperOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Swapper Done");
                    }

                    if (CustomGameOptions.InvestigatorOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.InvestigatorCount;

                            do
                            {
                                CrewRoles.Add((typeof(Investigator), CustomRPC.SetInvestigator, CustomGameOptions.InvestigatorOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Investigator), CustomRPC.SetInvestigator, CustomGameOptions.InvestigatorOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Investigator Done");
                    }

                    if (CustomGameOptions.TimeLordOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.TimeLordCount;

                            do
                            {
                                CrewRoles.Add((typeof(TimeLord), CustomRPC.SetTimeLord, CustomGameOptions.TimeLordOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(TimeLord), CustomRPC.SetTimeLord, CustomGameOptions.TimeLordOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Time Lord Done");
                    }

                    if (CustomGameOptions.MedicOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MedicCount;

                            do
                            {
                                CrewRoles.Add((typeof(Medic), CustomRPC.SetMedic, CustomGameOptions.MedicOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Medic), CustomRPC.SetMedic, CustomGameOptions.MedicOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Medic Done");
                    }

                    if (CustomGameOptions.AgentOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.AgentCount;

                            do
                            {
                                CrewRoles.Add((typeof(Agent), CustomRPC.SetAgent, CustomGameOptions.AgentOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Agent), CustomRPC.SetAgent, CustomGameOptions.AgentOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Agent Done");
                    }

                    if (CustomGameOptions.AltruistOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.AltruistCount;

                            do
                            {
                                CrewRoles.Add((typeof(Altruist), CustomRPC.SetAltruist, CustomGameOptions.AltruistOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Altruist), CustomRPC.SetAltruist, CustomGameOptions.AltruistOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Altruist Done");
                    }

                    if (CustomGameOptions.VeteranOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.VeteranCount;

                            do
                            {
                                CrewRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, CustomGameOptions.VeteranOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, CustomGameOptions.VeteranOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Veteran Done");
                    }

                    if (CustomGameOptions.TrackerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.TrackerCount;

                            do
                            {
                                CrewRoles.Add((typeof(Tracker), CustomRPC.SetTracker, CustomGameOptions.TrackerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Tracker), CustomRPC.SetTracker, CustomGameOptions.TrackerOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Tracker Done");
                    }

                    if (CustomGameOptions.TransporterOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.TransporterCount;

                            do
                            {
                                CrewRoles.Add((typeof(Transporter), CustomRPC.SetTransporter, CustomGameOptions.TransporterOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Transporter), CustomRPC.SetTransporter, CustomGameOptions.TransporterOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Transporter Done");
                    }

                    if (CustomGameOptions.MediumOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MediumCount;

                            do
                            {
                                CrewRoles.Add((typeof(Medium), CustomRPC.SetMedium, CustomGameOptions.MediumOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Medium), CustomRPC.SetMedium, CustomGameOptions.MediumOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Medium Done");
                    }

                    if (CustomGameOptions.CoronerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.CoronerCount;

                            do
                            {
                                CrewRoles.Add((typeof(Coroner), CustomRPC.SetCoroner, CustomGameOptions.CoronerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Coroner), CustomRPC.SetCoroner, CustomGameOptions.CoronerOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Coroner Done");
                    }

                    if (CustomGameOptions.OperativeOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.OperativeCount;

                            do
                            {
                                CrewRoles.Add((typeof(Operative), CustomRPC.SetOperative, CustomGameOptions.OperativeOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Operative), CustomRPC.SetOperative, CustomGameOptions.OperativeOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Operative Done");
                    }

                    if (CustomGameOptions.DetectiveOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.DetectiveCount;

                            do
                            {
                                CrewRoles.Add((typeof(Detective), CustomRPC.SetDetective, CustomGameOptions.DetectiveOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Detective), CustomRPC.SetDetective, CustomGameOptions.DetectiveOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Detective Done");
                    }

                    if (CustomGameOptions.EscortOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.EscortCount;

                            do
                            {
                                CrewRoles.Add((typeof(Escort), CustomRPC.SetEscort, CustomGameOptions.EscortOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Escort), CustomRPC.SetEscort, CustomGameOptions.EscortOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Escort Done");
                    }

                    if (CustomGameOptions.ShifterOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.ShifterCount;

                            do
                            {
                                CrewRoles.Add((typeof(Shifter), CustomRPC.SetShifter, CustomGameOptions.ShifterOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(Shifter), CustomRPC.SetShifter, CustomGameOptions.ShifterOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shifter Done");
                    }

                    if (CustomGameOptions.CrewmateOn > 0 && CustomGameOptions.GameMode == GameMode.Custom)
                    {
                        var number = CustomGameOptions.CrewCount;

                        do
                        {
                            IntruderRoles.Add((typeof(Crewmate), CustomRPC.SetCrewmate, CustomGameOptions.CrewmateOn, true));
                            number--;
                        } while (number > 0);

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Crewmate Done");
                    }

                    //It is crucial VH comes with Dracula does, since I want VH to only spawn if there's a Dracula in the role spawning set
                    //Future AD here, I realised that when the game begins, the VH will automatically turn into Vig so I don't need to take any extra steps
                    if (CustomGameOptions.VampireHunterOn > 0 && CustomGameOptions.DraculaOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.VampireHunterCount;

                            do
                            {
                                CrewRoles.Add((typeof(VampireHunter), CustomRPC.SetVampireHunter, CustomGameOptions.VampireHunterOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewRoles.Add((typeof(VampireHunter), CustomRPC.SetVampireHunter, CustomGameOptions.VampireHunterOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Vampire Hunter Done");
                    }
                    #endregion

                    #region Neutral Roles
                    if (CustomGameOptions.JesterOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.JesterCount;

                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Jester), CustomRPC.SetJester, CustomGameOptions.JesterOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Jester), CustomRPC.SetJester, CustomGameOptions.JesterOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Jester Done");
                    }

                    if (CustomGameOptions.AmnesiacOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.AmnesiacCount;

                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Amnesiac), CustomRPC.SetAmnesiac, CustomGameOptions.AmnesiacOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Amnesiac), CustomRPC.SetAmnesiac, CustomGameOptions.AmnesiacOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Amnesiac Done");
                    }

                    if (CustomGameOptions.ExecutionerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.ExecutionerCount;

                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Executioner), CustomRPC.SetExecutioner, CustomGameOptions.ExecutionerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Executioner), CustomRPC.SetExecutioner, CustomGameOptions.ExecutionerOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Executioner Done");
                    }

                    if (CustomGameOptions.SurvivorOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.SurvivorCount;

                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Survivor), CustomRPC.SetSurvivor, CustomGameOptions.SurvivorOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Survivor), CustomRPC.SetSurvivor, CustomGameOptions.SurvivorOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Survivor Done");
                    }

                    if (CustomGameOptions.GuardianAngelOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.GuardianAngelCount;

                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(GuardianAngel), CustomRPC.SetGuardianAngel, CustomGameOptions.GuardianAngelOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(GuardianAngel), CustomRPC.SetGuardianAngel, CustomGameOptions.GuardianAngelOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Guardian Angel Done");
                    }

                    if (CustomGameOptions.GlitchOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.GlitchCount;

                            do
                            {
                                NeutralKillingRoles.Add((typeof(Glitch), CustomRPC.SetGlitch, CustomGameOptions.GlitchOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralKillingRoles.Add((typeof(Glitch), CustomRPC.SetGlitch, CustomGameOptions.GlitchOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Glitch Done");
                    }

                    if (CustomGameOptions.MurdererOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MurdCount;

                            do
                            {
                                NeutralKillingRoles.Add((typeof(Murderer), CustomRPC.SetMurderer, CustomGameOptions.MurdererOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralKillingRoles.Add((typeof(Murderer), CustomRPC.SetMurderer, CustomGameOptions.MurdererOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Murderer Done");
                    }

                    if (CustomGameOptions.CryomaniacOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.CryomaniacCount;

                            do
                            {
                                NeutralKillingRoles.Add((typeof(Cryomaniac), CustomRPC.SetCryomaniac, CustomGameOptions.CryomaniacOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralKillingRoles.Add((typeof(Cryomaniac), CustomRPC.SetCryomaniac, CustomGameOptions.CryomaniacOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cryomaniac Done");
                    }

                    if (CustomGameOptions.WerewolfOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.WerewolfCount;

                            do
                            {
                                NeutralKillingRoles.Add((typeof(Werewolf), CustomRPC.SetWerewolf, CustomGameOptions.WerewolfOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralKillingRoles.Add((typeof(Werewolf), CustomRPC.SetWerewolf, CustomGameOptions.WerewolfOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Werewolf Done");
                    }

                    if (CustomGameOptions.ArsonistOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.ArsonistCount;

                            do
                            {
                                NeutralKillingRoles.Add((typeof(Arsonist), CustomRPC.SetArsonist, CustomGameOptions.ArsonistOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralKillingRoles.Add((typeof(Arsonist), CustomRPC.SetArsonist, CustomGameOptions.ArsonistOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Arsonist Done");
                    }

                    if (CustomGameOptions.JackalOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.JackalCount;

                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Jackal), CustomRPC.SetJackal, CustomGameOptions.JackalOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Jackal), CustomRPC.SetJackal, CustomGameOptions.JackalOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Jackal Done");
                    }

                    if (CustomGameOptions.PlaguebearerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.PlaguebearerCount;

                            do
                            {
                                NeutralKillingRoles.Add((typeof(Plaguebearer), CustomRPC.SetPlaguebearer, CustomGameOptions.PlaguebearerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralKillingRoles.Add((typeof(Plaguebearer), CustomRPC.SetPlaguebearer, CustomGameOptions.PlaguebearerOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Plaguebearer Done");
                    }

                    if (CustomGameOptions.SerialKillerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.SKCount;

                            do
                            {
                                NeutralKillingRoles.Add((typeof(SerialKiller), CustomRPC.SetSerialKiller, CustomGameOptions.SerialKillerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralKillingRoles.Add((typeof(SerialKiller), CustomRPC.SetSerialKiller, CustomGameOptions.SerialKillerOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Serial Killer Done");
                    }

                    if (CustomGameOptions.JuggernautOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.JuggernautCount;

                            do
                            {
                                NeutralKillingRoles.Add((typeof(Juggernaut), CustomRPC.SetJuggernaut, CustomGameOptions.JuggernautOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralKillingRoles.Add((typeof(Juggernaut), CustomRPC.SetJuggernaut, CustomGameOptions.JuggernautOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Juggeraut Done");
                    }

                    if (CustomGameOptions.CannibalOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.CannibalCount;

                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Cannibal), CustomRPC.SetCannibal, CustomGameOptions.CannibalOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Cannibal), CustomRPC.SetCannibal, CustomGameOptions.CannibalOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Cannibal Done");
                    }

                    if (CustomGameOptions.ThiefOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.ThiefCount;

                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Thief), CustomRPC.SetThief, CustomGameOptions.ThiefOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Thief), CustomRPC.SetThief, CustomGameOptions.ThiefOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Thief Done");
                    }

                    if (CustomGameOptions.DraculaOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.DraculaCount;
                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Dracula), CustomRPC.SetDracula, CustomGameOptions.DraculaOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Dracula), CustomRPC.SetDracula, CustomGameOptions.DraculaOn, true));

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Dracula Done");
                    }

                    if (CustomGameOptions.TrollOn > 0 && CustomGameOptions.GameMode == GameMode.Custom)
                    {
                        var number = CustomGameOptions.TrollCount;

                        do
                        {
                            NeutralNonKillingRoles.Add((typeof(Troll), CustomRPC.SetTroll, CustomGameOptions.TrollOn, true));
                            number--;
                        } while (number > 0);

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Troll Done");
                    }
                    #endregion

                    #region Intruder Roles
                    if (!CustomGameOptions.AltImps)
                    {
                        if (CustomGameOptions.UndertakerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.UndertakerCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Undertaker Done");
                        }

                        if (CustomGameOptions.MorphlingOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.MorphlingCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Morphling Done");
                        }

                        if (CustomGameOptions.BlackmailerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.BlackmailerCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Blackmailer), CustomRPC.SetBlackmailer, CustomGameOptions.BlackmailerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Blackmailer), CustomRPC.SetBlackmailer, CustomGameOptions.BlackmailerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Blackmailer Done");
                        }

                        if (CustomGameOptions.MinerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.MinerCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Miner Done");
                        }

                        if (CustomGameOptions.TeleporterOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.TeleporterCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Teleporter), CustomRPC.SetTeleporter, CustomGameOptions.TeleporterOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Teleporter), CustomRPC.SetTeleporter, CustomGameOptions.TeleporterOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Teleporter Done");
                        }

                        if (CustomGameOptions.WraithOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.WraithCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Wraith), CustomRPC.SetWraith, CustomGameOptions.WraithOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Wraith), CustomRPC.SetWraith, CustomGameOptions.WraithOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Wraith Done");
                        }

                        if (CustomGameOptions.ConsortOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.ConsortCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Consort), CustomRPC.SetConsort, CustomGameOptions.ConsortOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Consort), CustomRPC.SetConsort, CustomGameOptions.ConsortOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Consort Done");
                        }

                        if (CustomGameOptions.JanitorOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.JanitorCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Janitor), CustomRPC.SetJanitor, CustomGameOptions.JanitorOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Janitor), CustomRPC.SetJanitor, CustomGameOptions.JanitorOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Janitor Done");
                        }

                        if (CustomGameOptions.CamouflagerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.CamouflagerCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Camouflager), CustomRPC.SetCamouflager, CustomGameOptions.CamouflagerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Camouflager), CustomRPC.SetCamouflager, CustomGameOptions.CamouflagerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Camouflager Done");
                        }

                        if (CustomGameOptions.GrenadierOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.GrenadierCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, CustomGameOptions.GrenadierOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, CustomGameOptions.GrenadierOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Grenadier Done");
                        }

                        if (CustomGameOptions.PoisonerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.PoisonerCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Poisoner), CustomRPC.SetPoisoner, CustomGameOptions.PoisonerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Poisoner), CustomRPC.SetPoisoner, CustomGameOptions.PoisonerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Poisoner Done");
                        }

                        if (CustomGameOptions.ImpostorOn > 0 && CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.ImpCount;

                            do
                            {
                                IntruderRoles.Add((typeof(Impostor), CustomRPC.SetImpostor, CustomGameOptions.ImpostorOn, true));
                                number--;
                            } while (number > 0);

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Impostor Done");
                        }

                        if (CustomGameOptions.ConsigliereOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.ConsigliereCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Consigliere), CustomRPC.SetConsigliere, CustomGameOptions.ConsigliereOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Consigliere), CustomRPC.SetConsigliere, CustomGameOptions.ConsigliereOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Consigliere Done");
                        }

                        if (CustomGameOptions.DisguiserOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.DisguiserCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Disguiser), CustomRPC.SetDisguiser, CustomGameOptions.DisguiserOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Disguiser), CustomRPC.SetDisguiser, CustomGameOptions.DisguiserOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Disguiser Done");
                        }

                        if (CustomGameOptions.TimeMasterOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.TimeMasterCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(TimeMaster), CustomRPC.SetTimeMaster, CustomGameOptions.TimeMasterOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(TimeMaster), CustomRPC.SetTimeMaster, CustomGameOptions.TimeMasterOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Time Master Done");
                        }

                        if (CustomGameOptions.GodfatherOn > 0 && CustomGameOptions.IntruderCount >= 3)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.GodfatherCount;
                                
                                do
                                {
                                    IntruderRoles.Add((typeof(Godfather), CustomRPC.SetGodfather, CustomGameOptions.GodfatherOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Godfather), CustomRPC.SetGodfather, CustomGameOptions.GodfatherOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Godfather Done");
                        }
                    }
                    else
                    {
                        if (CustomGameOptions.PuppeteerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.PuppeteerCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Puppeteer), CustomRPC.SetPuppeteer, CustomGameOptions.PuppeteerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Puppeteer), CustomRPC.SetPuppeteer, CustomGameOptions.PuppeteerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Puppeteer Done");
                        }

                        if (CustomGameOptions.AnarchistOn > 0 && CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.AnarchistCount;

                            do
                            {
                                IntruderRoles.Add((typeof(Anarchist), CustomRPC.SetAnarchist, CustomGameOptions.AnarchistOn, true));
                                number--;
                            } while (number > 0);

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Anarchist Done");
                        }

                        if (CustomGameOptions.ShapeshifterOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.ShapeshifterCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Shapeshifter), CustomRPC.SetShapeshifter, CustomGameOptions.ShapeshifterOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Shapeshifter), CustomRPC.SetShapeshifter, CustomGameOptions.ShapeshifterOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shapeshifter Done");
                        }

                        if (CustomGameOptions.ConcealerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.ConcealerCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Concealer), CustomRPC.SetConcealer, CustomGameOptions.ConcealerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Concealer), CustomRPC.SetConcealer, CustomGameOptions.ConcealerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Concealer Done");
                        }

                        if (CustomGameOptions.WarperOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.WarperCount;

                                do
                                {
                                    IntruderRoles.Add((typeof(Warper), CustomRPC.SetWarper, CustomGameOptions.WarperOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Warper), CustomRPC.SetWarper, CustomGameOptions.WarperOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Warper Done");
                        }

                        if (CustomGameOptions.RebelOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.RebelCount;
                                
                                do
                                {
                                    IntruderRoles.Add((typeof(Rebel), CustomRPC.SetRebel, CustomGameOptions.RebelOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Rebel), CustomRPC.SetRebel, CustomGameOptions.RebelOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Rebel Done");
                        }
                    }
                    #endregion

                    #region Syndicate Roles
                    if (!CustomGameOptions.AltImps)
                    {
                        if (CustomGameOptions.PuppeteerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.PuppeteerCount;

                                do
                                {
                                    SyndicateRoles.Add((typeof(Puppeteer), CustomRPC.SetPuppeteer, CustomGameOptions.PuppeteerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                SyndicateRoles.Add((typeof(Puppeteer), CustomRPC.SetPuppeteer, CustomGameOptions.PuppeteerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Puppeteer Done");
                        }

                        if (CustomGameOptions.AnarchistOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.AnarchistCount;

                                do
                                {
                                    SyndicateRoles.Add((typeof(Anarchist), CustomRPC.SetAnarchist, CustomGameOptions.AnarchistOn, true));
                                    number--;
                                } while (number > 0);
                            }

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Anarchist Done");
                        }

                        if (CustomGameOptions.ShapeshifterOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.ShapeshifterCount;

                                do
                                {
                                    SyndicateRoles.Add((typeof(Shapeshifter), CustomRPC.SetShapeshifter, CustomGameOptions.ShapeshifterOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                SyndicateRoles.Add((typeof(Shapeshifter), CustomRPC.SetShapeshifter, CustomGameOptions.ShapeshifterOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shapeshifter Done");
                        }

                        if (CustomGameOptions.RebelOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.RebelCount;
                                
                                do
                                {
                                    IntruderRoles.Add((typeof(Rebel), CustomRPC.SetRebel, CustomGameOptions.RebelOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                IntruderRoles.Add((typeof(Rebel), CustomRPC.SetRebel, CustomGameOptions.RebelOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Rebel Done");
                        }

                        if (CustomGameOptions.ConcealerOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.ConcealerCount;

                                do
                                {
                                    SyndicateRoles.Add((typeof(Concealer), CustomRPC.SetConcealer, CustomGameOptions.ConcealerOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                SyndicateRoles.Add((typeof(Concealer), CustomRPC.SetConcealer, CustomGameOptions.ConcealerOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Concealer Done");
                        }

                        if (CustomGameOptions.WarperOn > 0)
                        {
                            if (CustomGameOptions.GameMode == GameMode.Custom)
                            {
                                var number = CustomGameOptions.WarperCount;

                                do
                                {
                                    SyndicateRoles.Add((typeof(Warper), CustomRPC.SetWarper, CustomGameOptions.WarperOn, true));
                                    number--;
                                } while (number > 0);
                            }
                            else
                                SyndicateRoles.Add((typeof(Warper), CustomRPC.SetWarper, CustomGameOptions.WarperOn, true));

                            PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Warper Done");
                        }
                    }
                    #endregion

                    #region Modifiers
                    if (CustomGameOptions.DiseasedOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.DiseasedCount : 1;
                        
                        while (num2 > 0)
                        {
                            DiseasedModifiers.Add((typeof(Diseased), CustomRPC.SetDiseased, CustomGameOptions.DiseasedOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Diseased Done");
                    }

                    if (CustomGameOptions.BaitOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.BaitCount : 1;
                        
                        while (num2 > 0)
                        {
                            BaitModifiers.Add((typeof(Bait), CustomRPC.SetBait, CustomGameOptions.BaitOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Bait Done");
                    }
                    
                    if (CustomGameOptions.DwarfOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.DwarfCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalModifiers.Add((typeof(Dwarf), CustomRPC.SetDwarf, CustomGameOptions.DwarfOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Dwarf Done");
                    }
                    
                    if (CustomGameOptions.VIPOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.VIPCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalModifiers.Add((typeof(VIP), CustomRPC.SetVIP, CustomGameOptions.VIPOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("VIP Done");
                    }
                    
                    if (CustomGameOptions.ShyOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.ShyCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalModifiers.Add((typeof(Shy), CustomRPC.SetShy, CustomGameOptions.ShyOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Shy Done");
                    }

                    if (CustomGameOptions.GiantOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.GiantCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalModifiers.Add((typeof(Giant), CustomRPC.SetGiant, CustomGameOptions.GiantOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Giant Done");
                    }
                    
                    if (CustomGameOptions.DrunkOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.DrunkCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalModifiers.Add((typeof(Drunk), CustomRPC.SetDrunk, CustomGameOptions.DrunkOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Drunk Done");
                    }

                    if (CustomGameOptions.FlincherOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.FlincherCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalModifiers.Add((typeof(Flincher), CustomRPC.SetFlincher, CustomGameOptions.FlincherOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Flincher Done");
                    }

                    if (CustomGameOptions.CowardOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.CowardCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalModifiers.Add((typeof(Coward), CustomRPC.SetCoward, CustomGameOptions.CowardOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Coward Done");
                    }

                    if (CustomGameOptions.VolatileOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.VolatileCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalModifiers.Add((typeof(Volatile), CustomRPC.SetVolatile, CustomGameOptions.VolatileOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Volatile Done");
                    }

                    if (CustomGameOptions.ProfessionalOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.ProfessionalCount : 1;
                        
                        while (num2 > 0)
                        {
                            ProfessionalModifiers.Add((typeof(Professional), CustomRPC.SetProfessional, CustomGameOptions.ProfessionalOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Professional Done");
                    }
                    #endregion

                    #region Abilities
                    if (CustomGameOptions.GameMode != GameMode.Classic && CustomGameOptions.AssassinOn > 0)
                    {
                        var number = CustomGameOptions.NumberOfSyndicateAssassins;

                        while (number > 0)
                        {
                            SyndicateAbilityGet.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));
                            number--;
                        }

                        number = CustomGameOptions.NumberOfNeutralAssassins;

                        while (number > 0)
                        {
                            NeutralAbilityGet.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));
                            number--;
                        }

                        number = CustomGameOptions.NumberOfCrewAssassins;

                        while (number > 0)
                        {
                            CrewAbilityGet.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));
                            number--;
                        }

                        number = CustomGameOptions.NumberOfImpostorAssassins;

                        while (number > 0)
                        {
                            IntruderAbilityGet.Add((typeof(Assassin), CustomRPC.SetAssassin, CustomGameOptions.AssassinOn));
                            number--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Assassin Done");
                    }

                    if (CustomGameOptions.RevealerOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.RevealerCount : 1;
                        
                        while (num2 > 0)
                        {
                            CrewAbilityGet.Add((typeof(Revealer), CustomRPC.SetRevealer, CustomGameOptions.RevealerOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Revealer Done");
                    }

                    if (CustomGameOptions.SnitchOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.SnitchCount : 1;
                        
                        while (num2 > 0)
                        {
                            CrewAbilityGet.Add((typeof(Snitch), CustomRPC.SetSnitch, CustomGameOptions.SnitchOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Snitch Done");
                    }

                    if (CustomGameOptions.InsiderOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.InsiderCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalAbilityGet.Add((typeof(Insider), CustomRPC.SetInsider, CustomGameOptions.InsiderOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Insider Done");
                    }

                    if (CustomGameOptions.LighterOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.LighterCount : 1;
                        
                        while (num2 > 0)
                        {
                            CrewAbilityGet.Add((typeof(Lighter), CustomRPC.SetLighter, CustomGameOptions.LighterOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Lighter Done");
                    }

                    if (CustomGameOptions.MultitaskerOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.MultitaskerCount : 1;
                        
                        while (num2 > 0)
                        {
                            NonEvilAbilityGet.Add((typeof(Multitasker), CustomRPC.SetMultitasker, CustomGameOptions.MultitaskerOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Multitasker Done");
                    }

                    if (CustomGameOptions.RadarOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.RadarCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalAbilityGet.Add((typeof(Radar), CustomRPC.SetRadar, CustomGameOptions.RadarOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Radar Done");
                    }

                    if (CustomGameOptions.TiebreakerOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.TiebreakerCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalAbilityGet.Add((typeof(Tiebreaker), CustomRPC.SetTiebreaker, CustomGameOptions.TiebreakerOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Tiebreaker Done");
                    }

                    if (CustomGameOptions.TorchOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.TorchCount : 1;
                        
                        while (num2 > 0)
                        {
                            NonEvilAbilityGet.Add((typeof(Torch), CustomRPC.SetTorch, CustomGameOptions.TorchOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Torch Done");
                    }

                    if (CustomGameOptions.UnderdogOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.UnderdogCount : 1;
                        
                        while (num2 > 0)
                        {
                            EvilAbilityGet.Add((typeof(Underdog), CustomRPC.SetUnderdog, CustomGameOptions.UnderdogOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Underdog Done");
                    }

                    if (CustomGameOptions.TunnelerOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.TunnelerCount : 1;
                        
                        while (num2 > 0)
                        {
                            CrewAbilityGet.Add((typeof(Tunneler), CustomRPC.SetTunneler, CustomGameOptions.TunnelerOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Tunneler Done");
                    }
                    #endregion

                    #region Objectifiers
                    if (CustomGameOptions.LoversOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.LoversCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalObjectifierGet.Add((typeof(Lovers), CustomRPC.SetCouple, CustomGameOptions.LoversOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Lovers Done");
                    }

                    /*if (CustomGameOptions.RivalsOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.RivalsCount : 1;
                        
                        while (num2 > 0)
                        {
                            GlobalObjectifierGet.Add((typeof(Rivals), CustomRPC.SetDuo, CustomGameOptions.RivalsOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Rivals Done");
                    }*/

                    if (CustomGameOptions.FanaticOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.FanaticCount : 1;
                        
                        while (num2 > 0)
                        {
                            CrewObjectifierGet.Add((typeof(Fanatic), CustomRPC.SetFanatic, CustomGameOptions.FanaticOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Fanatic Done");
                    }

                    if (CustomGameOptions.TraitorOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.TraitorCount : 1;
                        
                        while (num2 > 0)
                        {
                            CrewObjectifierGet.Add((typeof(Traitor), CustomRPC.SetTraitor, CustomGameOptions.TraitorOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Traitor Done");
                    }

                    if (CustomGameOptions.TaskmasterOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.TaskmasterCount : 1;
                        
                        while (num2 > 0)
                        {
                            NeutralObjectifierGet.Add((typeof(Taskmaster), CustomRPC.SetTaskmaster, CustomGameOptions.TaskmasterOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Taskmaster Done");
                    }

                    if (CustomGameOptions.PhantomOn > 0)
                    {
                        num2 = customgflag ? CustomGameOptions.PhantomCount : 1;
                        
                        while (num2 > 0)
                        {
                            NeutralObjectifierGet.Add((typeof(Phantom), CustomRPC.SetPhantom, CustomGameOptions.PhantomOn));
                            num2--;
                        }

                        PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Phantom Done");
                    }
                    #endregion
                }

                if (CustomGameOptions.GameMode == GameMode.KillingOnly)
                    GenEachRoleKilling(infected.ToList());
                else
                    GenEachRole(infected.ToList());

                PluginSingleton<TownOfUsReworked>.Instance.Log.LogMessage("Role Gen Done");
            }
        }
    }
}
