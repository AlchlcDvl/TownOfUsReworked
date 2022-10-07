using System;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Hazel;
using Reactor;
using Reactor.Extensions;
using TownOfUs.CustomOption;
using TownOfUs.Extensions;
using TownOfUs.Roles;
using TownOfUs.Roles.Modifiers;
using TownOfUs.Roles.Modifiers.Objectifiers;
using TownOfUs.CrewmateRoles.AltruistMod;
using TownOfUs.CrewmateRoles.MedicMod;
using TownOfUs.CrewmateRoles.SwapperMod;
using TownOfUs.CrewmateRoles.TimeLordMod;
using TownOfUs.CrewmateRoles.HaunterMod;
using TownOfUs.ImpostorRoles.TraitorMod;
using TownOfUs.ImpostorRoles.MinerMod;
using TownOfUs.NeutralRoles.ExecutionerMod;
using TownOfUs.NeutralRoles.GuardianAngelMod;
using TownOfUs.NeutralRoles.PhantomMod;
using TownOfUs.Objectifiers.AssassinMod;
using UnityEngine;
using Random = UnityEngine.Random; 
using Object = UnityEngine.Object;
using Coroutine = TownOfUs.ImpostorRoles.JanitorMod.Coroutine;
using Eat = TownOfUs.NeutralRoles.CannibalMod.Coroutine;
using PerformKillButton = TownOfUs.NeutralRoles.AmnesiacMod.PerformKillButton;
using PerformShiftButton = TownOfUs.CrewmateRoles.ShifterMod.PerformShiftButton;
using PerformConvertButton = TownOfUs.NeutralRoles.DraculaMod.PerformConvertButton;

namespace TownOfUs
{
    public static class RpcHandling
    {
        private static readonly List<(Type, CustomRPC, int, bool)> CrewmateRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralNonKillingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> NeutralKillingRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> ImpostorRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int, bool)> KillerRoles = new List<(Type, CustomRPC, int, bool)>();
        private static readonly List<(Type, CustomRPC, int)> CrewmateModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> GlobalModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ImpModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> KillerModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> TaskedModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> FactionedModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NonKillerModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> NeutralModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> GuessModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> BaseModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> SpecialModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ButtonModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> BaitModifiers = new List<(Type, CustomRPC, int)>();
        //private static readonly List<(Type, CustomRPC, int)> GuiltyModifiers = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ObjectifierAbility = new List<(Type, CustomRPC, int)>();
        private static readonly List<(Type, CustomRPC, int)> ObjectifierWinCon = new List<(Type, CustomRPC, int)>();
        private static bool PhantomOn;
        private static bool HaunterOn;
        private static bool TraitorOn;

        internal static bool Check(int probability)
        {
            if (probability == 0) return false;
            if (probability == 100) return true;
            var num = Random.RandomRangeInt(1, 101);
            return num <= probability;
        }

        private static void SortRoles(List<(Type, CustomRPC, int, bool)> roles, int max, int min)
        {
            roles.Shuffle();
            if (roles.Count < max) max = roles.Count;
            if (min > max) min = max;
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
                if (role.Item3 == 100) certainRoles += 1;
                else odds += role.Item3;
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
            while (roles.Count > amount) roles.RemoveAt(roles.Count - 1);
        }

        private static void SortModifiers(List<(Type, CustomRPC, int)> roles, int max)
        {
            roles.Shuffle();
            roles.Sort((a, b) =>
            {
                var a_ = a.Item3 == 100 ? 0 : 100;
                var b_ = b.Item3 == 100 ? 0 : 100;
                return a_.CompareTo(b_);
            });
            while (roles.Count > max) roles.RemoveAt(roles.Count - 1);
        }

        private static void GenEachRole(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);
            crewmates.Shuffle();
            impostors.Shuffle();

            if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Custom)
            {
                if (crewmates.Count > CustomGameOptions.MaxNeutralNonKillingRoles)
                    SortRoles(NeutralNonKillingRoles, CustomGameOptions.MaxNeutralNonKillingRoles, CustomGameOptions.MinNeutralNonKillingRoles);
                else
                    SortRoles(NeutralNonKillingRoles, crewmates.Count - 1, CustomGameOptions.MinNeutralNonKillingRoles);

                if (crewmates.Count - NeutralNonKillingRoles.Count > CustomGameOptions.MaxNeutralKillingRoles)
                    SortRoles(NeutralKillingRoles, CustomGameOptions.MaxNeutralKillingRoles, CustomGameOptions.MinNeutralKillingRoles);
                else
                    SortRoles(NeutralKillingRoles, crewmates.Count - NeutralNonKillingRoles.Count - 1, CustomGameOptions.MinNeutralKillingRoles);

                SortRoles(CrewmateRoles, crewmates.Count - NeutralNonKillingRoles.Count - NeutralKillingRoles.Count, crewmates.Count - NeutralNonKillingRoles.Count - NeutralKillingRoles.Count);
                SortRoles(ImpostorRoles, impostors.Count, impostors.Count);
            }

            var crewAndNeutralRoles = new List<(Type, CustomRPC, int, bool)>();

            if (CustomGameOptions.GameMode == GameMode.Classic || CustomGameOptions.GameMode == GameMode.Custom)
                crewAndNeutralRoles.AddRange(CrewmateRoles);

            crewAndNeutralRoles.AddRange(NeutralNonKillingRoles);
            crewAndNeutralRoles.AddRange(NeutralKillingRoles);

            var crewRoles = new List<(Type, CustomRPC, int, bool)>();
            var neutRoles = new List<(Type, CustomRPC, int, bool)>();
            var impRoles = new List<(Type, CustomRPC, int, bool)>();

            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                crewAndNeutralRoles.Shuffle();

                if (crewAndNeutralRoles.Count > 0)
                {
                    crewRoles.Add(crewAndNeutralRoles[0]);

                    if (crewAndNeutralRoles[0].Item4 == true)
                        crewAndNeutralRoles.Remove(crewAndNeutralRoles[0]);
                }

                if (CrewmateRoles.Count > 0)
                {
                    CrewmateRoles.Shuffle();
                    crewRoles.Add(CrewmateRoles[0]);

                    if (CrewmateRoles[0].Item4 == true)
                        CrewmateRoles.Remove(CrewmateRoles[0]);
                }
                else
                {
                    crewRoles.Add((typeof(Crewmate), CustomRPC.SetCrewmate, 100, false));
                }

                crewAndNeutralRoles.AddRange(CrewmateRoles);

                while (crewRoles.Count < crewmates.Count && crewAndNeutralRoles.Count > 0)
                {
                    crewAndNeutralRoles.Shuffle();
                    crewRoles.Add(crewAndNeutralRoles[0]);
                    if (crewAndNeutralRoles[0].Item4 == true)
                    {
                        if (CrewmateRoles.Contains(crewAndNeutralRoles[0])) CrewmateRoles.Remove(crewAndNeutralRoles[0]);
                        crewAndNeutralRoles.Remove(crewAndNeutralRoles[0]);
                    }
                }
                while (impRoles.Count < impostors.Count && ImpostorRoles.Count > 0)
                {
                    ImpostorRoles.Shuffle();
                    impRoles.Add(ImpostorRoles[0]);
                    if (ImpostorRoles[0].Item4 == true) ImpostorRoles.Remove(ImpostorRoles[0]);
                }
            }
            crewRoles.Shuffle();
            impRoles.Shuffle();

            SortModifiers(CrewmateModifiers, crewmates.Count);
            SortModifiers(GlobalModifiers, crewmates.Count + impostors.Count);
            SortModifiers(ButtonModifiers, crewmates.Count + impostors.Count);
            //SortModifiers(GuiltyModifiers, crewmates.Count + impostors.Count);
            SortModifiers(BaitModifiers, crewmates.Count + impostors.Count);

            if (CustomGameOptions.GameMode == GameMode.AllAny)
            {
                foreach (var (type, rpc, _, unique) in crewRoles)
                {
                    Role.Gen<Role>(type, crewmates, rpc);
                }
                foreach (var (type, rpc, _, unique) in impRoles)
                {
                    Role.Gen<Role>(type, impostors, rpc);
                }
            }
            else
            {
                foreach (var (type, rpc, _, unique) in crewAndNeutralRoles)
                {
                    Role.Gen<Role>(type, crewmates, rpc);
                }
                foreach (var (type, rpc, _, unique) in ImpostorRoles)
                {
                    Role.Gen<Role>(type, impostors, rpc);
                }
            }

            foreach (var crewmate in crewmates)
                Role.Gen<Role>(typeof(Crewmate), crewmate, CustomRPC.SetCrewmate);

            foreach (var impostor in impostors)
                Role.Gen<Role>(typeof(Impostor), impostor, CustomRPC.SetImpostor);

            var canHaveModifier = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveModifier2 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveModifier3 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility2 = PlayerControl.AllPlayerControls.ToArray().ToList();
            var canHaveAbility3 = PlayerControl.AllPlayerControls.ToArray().ToList();
            
            canHaveModifier.RemoveAll(player => player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Shifter));
            canHaveModifier.Shuffle();

            canHaveAbility.RemoveAll(player => !player.Is(Faction.Intruders));
            canHaveAbility.Shuffle();

            canHaveAbility2.RemoveAll(player => !player.Is(Faction.Neutral) || player.Is(RoleEnum.Amnesiac) || player.Is(RoleEnum.GuardianAngel)
            || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.Jester));
            canHaveAbility2.Shuffle();

            canHaveAbility3.RemoveAll(player => !player.Is(Faction.Crewmates));
            canHaveAbility3.Shuffle();

            var impAssassins = CustomGameOptions.NumberOfImpostorAssassins;
            var neutAssassins = CustomGameOptions.NumberOfNeutralAssassins;
            var crewAssassins = CustomGameOptions.NumberOfCrewAssassins;

            foreach (var (type, rpc, _) in GlobalModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                if(rpc == CustomRPC.SetCouple)
                {
                    if (canHaveModifier.Count == 1) continue;
                        Lover.Gen(canHaveModifier);
                }
                else
                {
                    Role.Gen<Modifier>(type, canHaveModifier, rpc);
                }
            }

            canHaveModifier.RemoveAll(player => player.Is(RoleEnum.Glitch));

            foreach (var (type, rpc, _) in ButtonModifiers)
            {
                if (canHaveModifier.Count == 0) break;
                Role.Gen<Modifier>(type, canHaveModifier, rpc);
            }

            canHaveModifier.RemoveAll(player => player.Is(Alignment.NeutralKill) || player.Is(Faction.Intruders));
            canHaveModifier.Shuffle();

            while (canHaveModifier.Count > 0 && CrewmateModifiers.Count > 0)
            {
                var (type, rpc, _) = CrewmateModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier.TakeFirst(), rpc);
            }

            canHaveModifier2.RemoveAll(player => player.Is(RoleEnum.Vigilante) || player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Altruist));
            canHaveModifier2.Shuffle();

            while (canHaveModifier2.Count > 0 && BaitModifiers.Count > 0)
            {
                var (type, rpc, _) = BaitModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier2.TakeFirst(), rpc);
            }

            canHaveModifier3.RemoveAll(player => player.Is(RoleEnum.Jester) || player.Is(RoleEnum.Executioner) || player.Is(RoleEnum.TimeLord)
            || player.Is(RoleEnum.Cannibal) || player.Is(RoleEnum.Survivor) || player.Is(RoleEnum.GuardianAngel) || player.Is(RoleEnum.Crewmate)
            || player.Is(RoleEnum.Engineer) || player.Is(RoleEnum.Mayor) || player.Is(RoleEnum.Swapper)|| player.Is(RoleEnum.Investigator)
            || player.Is(RoleEnum.Medic) || player.Is(RoleEnum.Sheriff) || player.Is(RoleEnum.Agent) || player.Is(RoleEnum.Snitch)
            || player.Is(RoleEnum.Altruist) || player.Is(RoleEnum.Tracker) || player.Is(RoleEnum.Operative)|| player.Is(RoleEnum.Transporter)
            || player.Is(RoleEnum.Medium) || player.Is(RoleEnum.Mystic) || player.Is(RoleEnum.Cannibal) || player.Is(RoleEnum.Detective)
            || player.Is(RoleEnum.Shifter) || player.Is(RoleEnum.Amnesiac));
            canHaveModifier3.Shuffle();

            /*while (canHaveModifier3.Count > 0 && GuiltyModifiers.Count > 0)
            {
                var (type, rpc, _) = GuiltyModifiers.TakeFirst();
                Role.Gen<Modifier>(type, canHaveModifier3.TakeFirst(), rpc);
            }*/

            if (CustomGameOptions.GameMode == GameMode.Custom)
            {
                while (canHaveAbility.Count > 0 && impAssassins > 0)
                {
                    var (type, rpc, _) = ObjectifierAbility.Ability();
                    Role.Gen<Ability>(type, canHaveAbility.TakeFirst(), rpc);
                    impAssassins -= 1;
                }

                while (canHaveAbility2.Count > 0 && neutAssassins > 0)
                {
                    var (type, rpc, _) = ObjectifierAbility.Ability();
                    Role.Gen<Ability>(type, canHaveAbility2.TakeFirst(), rpc);
                    neutAssassins -= 1;
                }

                while (canHaveAbility3.Count > 0 && crewAssassins > 0)
                {
                    var (type, rpc, _) = ObjectifierAbility.Ability();
                    Role.Gen<Ability>(type, canHaveAbility3.TakeFirst(), rpc);
                    crewAssassins -= 1;
                }
            }

            var toChooseFromCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates)).ToList();
            if (TraitorOn && toChooseFromCrew.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                var pc = toChooseFromCrew[rand];

                SetTraitor.WillBeTraitor = pc;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetTraitor, SendOption.Reliable, -1);
                writer.Write(pc.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetTraitor, SendOption.Reliable, -1);
                writer.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            var toChooseFromNeut = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Neutral)).ToList();
            if (PhantomOn && toChooseFromNeut.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromNeut.Count);
                var pc = toChooseFromNeut[rand];

                SetPhantom.WillBePhantom = pc;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                writer.Write(pc.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetPhantom, SendOption.Reliable, -1);
                writer.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            toChooseFromCrew = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && x != SetTraitor.WillBeTraitor).ToList();
            if (HaunterOn && toChooseFromCrew.Count != 0)
            {
                var rand = Random.RandomRangeInt(0, toChooseFromCrew.Count);
                var pc = toChooseFromCrew[rand];

                SetHaunter.WillBeHaunter = pc;

                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetHaunter, SendOption.Reliable, -1);
                writer.Write(pc.PlayerId);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }
            else
            {
                var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte)CustomRPC.SetHaunter, SendOption.Reliable, -1);
                writer.Write(byte.MaxValue);
                AmongUsClient.Instance.FinishRpcImmediately(writer);
            }

            var exeTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) && !x.Is(RoleEnum.Mayor) && !x.Is(RoleEnum.Swapper) && x != SetTraitor.WillBeTraitor).ToList();
            foreach (var role in Role.GetRoles(RoleEnum.Executioner))
            {
                var exe = (Executioner)role;
                if (exeTargets.Count > 0)
                {
                    exe.target = exeTargets[Random.RandomRangeInt(0, exeTargets.Count)];
                    exeTargets.Remove(exe.target);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetTarget, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(exe.target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }

            var gaTargets = PlayerControl.AllPlayerControls.ToArray().Where(x => x.Is(Faction.Crewmates) || (x.Is(Faction.Neutral) &&  !x.Is(RoleEnum.Jester) && !x.Is(RoleEnum.Executioner) && !x.Is(RoleEnum.GuardianAngel)) || x.Is(Faction.Intruders)).ToList();
            foreach (var role in Role.GetRoles(RoleEnum.GuardianAngel))
            {
                var ga = (GuardianAngel)role;
                if (gaTargets.Count > 0)
                {
                    ga.target = gaTargets[Random.RandomRangeInt(0, gaTargets.Count)];
                    gaTargets.Remove(ga.target);

                    var writer = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId, (byte)CustomRPC.SetGATarget, SendOption.Reliable, -1);
                    writer.Write(role.Player.PlayerId);
                    writer.Write(ga.target.PlayerId);
                    AmongUsClient.Instance.FinishRpcImmediately(writer);
                }
            }
        }

        private static void GenEachRoleKilling(List<GameData.PlayerInfo> infected)
        {
            var impostors = Utils.GetImpostors(infected);
            var crewmates = Utils.GetCrewmates(impostors);

            crewmates.Shuffle();
            impostors.Shuffle();

            ImpostorRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, 10, true));
            ImpostorRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, 10, false));
            ImpostorRoles.Add((typeof(Blackmailer), CustomRPC.SetBlackmailer, 10, true));
            ImpostorRoles.Add((typeof(Miner), CustomRPC.SetMiner, 10, true));
            ImpostorRoles.Add((typeof(Wraith), CustomRPC.SetWraith, 10, false));
            ImpostorRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, 10, true));
            ImpostorRoles.Add((typeof(TimeMaster), CustomRPC.SetTimeMaster, 10, true));
            ImpostorRoles.Add((typeof(Disguiser), CustomRPC.SetDisguiser, 10, true));
            ImpostorRoles.Add((typeof(Consigliere), CustomRPC.SetConsigliere, 10, true));

            SortRoles(ImpostorRoles, impostors.Count, impostors.Count);

            NeutralKillingRoles.Add((typeof(Glitch), CustomRPC.SetGlitch, 50, true));
            NeutralKillingRoles.Add((typeof(Werewolf), CustomRPC.SetWerewolf, 50, true));
            NeutralKillingRoles.Add((typeof(Juggernaut), CustomRPC.SetJuggernaut, 50, true));
            if (CustomGameOptions.AddArsonist)
                NeutralKillingRoles.Add((typeof(Arsonist), CustomRPC.SetArsonist, 50, true));
            if (CustomGameOptions.AddPlaguebearer)
                NeutralKillingRoles.Add((typeof(Plaguebearer), CustomRPC.SetPlaguebearer, 50, true));

            var neutrals = 0;
            if (NeutralKillingRoles.Count < CustomGameOptions.NeutralRoles) neutrals = NeutralKillingRoles.Count;
            else neutrals = CustomGameOptions.NeutralRoles;
            var spareCrew = crewmates.Count - neutrals;
            if (spareCrew > 2) SortRoles(NeutralKillingRoles, neutrals, neutrals);
            else SortRoles(NeutralKillingRoles, crewmates.Count - 3, crewmates.Count - 3);

            if (CrewmateRoles.Count + NeutralKillingRoles.Count > crewmates.Count)
            {
                SortRoles(CrewmateRoles, crewmates.Count - NeutralKillingRoles.Count, crewmates.Count - NeutralKillingRoles.Count);
            }
            else if (CrewmateRoles.Count + NeutralKillingRoles.Count < crewmates.Count)
            {
                int vigis = (crewmates.Count - NeutralKillingRoles.Count - CrewmateRoles.Count)/2;
                int vets = (crewmates.Count - NeutralKillingRoles.Count - CrewmateRoles.Count)/2;
                while (vigis > 0)
                {
                    CrewmateRoles.Add((typeof(Vigilante), CustomRPC.SetVigilante, 100, false));
                    vigis -= 1;
                    CrewmateRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, 100, false));
                    vets -= 1;
                }
            }

            var crewAndNeutralRoles = new List<(Type, CustomRPC, int, bool)>();
            crewAndNeutralRoles.AddRange(CrewmateRoles);
            crewAndNeutralRoles.AddRange(NeutralKillingRoles);
            crewAndNeutralRoles.Shuffle();
            ImpostorRoles.Shuffle();

            foreach (var (type, rpc, _, unique) in ImpostorRoles)
            {
                Role.Gen<Role>(type, impostors, rpc);
            }
            foreach (var (type, rpc, _, unique) in crewAndNeutralRoles)
            {
                Role.Gen<Role>(type, crewmates, rpc);
            }
        }


        [HarmonyPatch(typeof(PlayerControl), nameof(PlayerControl.HandleRpc))]
        public static class HandleRpc
        {
            public static void Postfix([HarmonyArgument(0)] byte callId, [HarmonyArgument(1)] MessageReader reader)
            {
                byte readByte, readByte1, readByte2;
                sbyte readSByte, readSByte2;
                switch ((CustomRPC) callId)
                {
                    case CustomRPC.SetMayor:
                        new Mayor(Utils.PlayerById(reader.ReadByte()));
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

                    /*case CustomRPC.SetShaman:
                        new Shaman(Utils.PlayerById(reader.ReadByte()));
                        break;*/

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

                    /*case CustomRPC.SetGuilty:
                        new Guilty(Utils.PlayerById(reader.ReadByte()));
                        break;*/

                    case CustomRPC.LoveWin:
                        var winnerlover = Utils.PlayerById(reader.ReadByte());
                        Modifier.GetModifier<Lover>(winnerlover).Win();
                        break;

                    case CustomRPC.JesterLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Jester)
                                ((Jester) role).Loses();
                        break;

                    case CustomRPC.PhantomLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Phantom)
                                ((Phantom) role).Loses();
                        break;

                    case CustomRPC.VampLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Vampire)
                                ((Vampire) role).Loses();
                        break;

                    case CustomRPC.DracLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Dracula)
                                ((Dracula) role).Loses();
                        break;

                    case CustomRPC.GlitchLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Glitch)
                                ((Glitch) role).Loses();
                        break;

                    case CustomRPC.JuggernautLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Juggernaut)
                                ((Juggernaut) role).Loses();
                        break;

                    case CustomRPC.AmnesiacLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Amnesiac)
                                ((Amnesiac) role).Loses();
                        break;

                    case CustomRPC.ExecutionerLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Executioner)
                                ((Executioner) role).Loses();
                        break;

                    case CustomRPC.NobodyWins:
                        Role.NobodyWinsFunc();
                        break;

                    case CustomRPC.SurvivorOnlyWin:
                        Role.SurvOnlyWin();
                        break;

                    case CustomRPC.SetCouple:
                        var id = reader.ReadByte();
                        var id2 = reader.ReadByte();
                        var lover1 = Utils.PlayerById(id);
                        var lover2 = Utils.PlayerById(id2);

                        var modifierLover1 = new Lover(lover1);
                        var modifierLover2 = new Lover(lover2);

                        modifierLover1.OtherLover = modifierLover2;
                        modifierLover2.OtherLover = modifierLover1;

                        break;

                    case CustomRPC.Start:
                        Utils.ShowDeadBodies = false;
                        Murder.KilledPlayers.Clear();
                        Role.NobodyWins = false;
                        Role.SurvOnlyWins = false;
                        RecordRewind.points.Clear();
                        KillButtonTarget.DontRevive = byte.MaxValue;
                        break;

                    case CustomRPC.JanitorClean:
                        readByte1 = reader.ReadByte();
                        var janitorPlayer = Utils.PlayerById(readByte1);
                        var janitorRole = Role.GetRole<Janitor>(janitorPlayer);
                        readByte = reader.ReadByte();
                        var deadBodies = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in deadBodies)
                            if (body.ParentId == readByte)
                                Coroutines.Start(Coroutine.CleanCoroutine(body, janitorRole));

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
                        if (!mayor.Is(RoleEnum.Mayor)) mayorRole.VoteBank -= mayorRole.ExtraVotes.Count;

                        break;

                    case CustomRPC.SetSwaps:
                        readSByte = reader.ReadSByte();
                        SwapVotes.Swap1 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte);
                        readSByte2 = reader.ReadSByte();
                        SwapVotes.Swap2 = MeetingHud.Instance.playerStates.FirstOrDefault(x => x.TargetPlayerId == readSByte2);
                        PluginSingleton<TownOfUs>.Instance.Log.LogMessage("Bytes received - " + readSByte + " - " + readSByte2);
                        break;

                    case CustomRPC.Remember:
                        readByte1 = reader.ReadByte();
                        readByte2 = reader.ReadByte();
                        var amnesiac = Utils.PlayerById(readByte1);
                        var other = Utils.PlayerById(readByte2);
                        PerformKillButton.Remember(Role.GetRole<Amnesiac>(amnesiac), other);
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
                        var GlitchId = reader.ReadByte();
                        var GlitchPlayer = Utils.PlayerById(GlitchId);
                        new Glitch(GlitchPlayer);
                        break;

                    case CustomRPC.SetJuggernaut:
                        var JuggernautId = reader.ReadByte();
                        var JuggernautPlayer = Utils.PlayerById(JuggernautId);
                        new Juggernaut(JuggernautPlayer);
                        break;

                    case CustomRPC.BypassKill:
                        var killer = Utils.PlayerById(reader.ReadByte());
                        var target = Utils.PlayerById(reader.ReadByte());
                        Utils.MurderPlayer(killer, target);
                        break;

                    case CustomRPC.AssassinKill:
                        var toDie = Utils.PlayerById(reader.ReadByte());
                        AssassinKill.MurderPlayer(toDie);
                        break;

                    case CustomRPC.SetMimic:
                        var glitchPlayer = Utils.PlayerById(reader.ReadByte());
                        var mimicPlayer = Utils.PlayerById(reader.ReadByte());
                        var glitchRole = Role.GetRole<Glitch>(glitchPlayer);
                        glitchRole.MimicTarget = mimicPlayer;
                        glitchRole.IsUsingMimic = true;
                        Utils.Morph(glitchPlayer, mimicPlayer);
                        break;

                    case CustomRPC.RpcResetAnim:
                        var animPlayer = Utils.PlayerById(reader.ReadByte());
                        var theGlitchRole = Role.GetRole<Glitch>(animPlayer);
                        theGlitchRole.MimicTarget = null;
                        theGlitchRole.IsUsingMimic = false;
                        Utils.Unmorph(theGlitchRole.Player);
                        break;

                    case CustomRPC.GlitchWin:
                        var theGlitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                        ((Glitch) theGlitch)?.Wins();
                        break;

                    case CustomRPC.VampWin:
                        var theVampire = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Vampire);
                        ((Vampire) theVampire)?.Wins();
                        break;

                    case CustomRPC.DracWin:
                        var theDracula = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Dracula);
                        ((Dracula) theDracula)?.Wins();
                        break;

                    case CustomRPC.JuggernautWin:
                        var juggernaut = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Juggernaut);
                        ((Juggernaut)juggernaut)?.Wins();
                        break;

                    case CustomRPC.SetHacked:
                        var hackPlayer = Utils.PlayerById(reader.ReadByte());
                        if (hackPlayer.PlayerId == PlayerControl.LocalPlayer.PlayerId)
                        {
                            var glitch = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Glitch);
                            ((Glitch) glitch)?.SetHacked(hackPlayer);
                        }
                        break;
                        
                    case CustomRPC.Interrogate:
                        var sheriff = Utils.PlayerById(reader.ReadByte());
                        var otherPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Sheriff>(sheriff).Interrogated.Add(otherPlayer.PlayerId);
                        Role.GetRole<Sheriff>(sheriff).LastInterrogated = DateTime.UtcNow;
                        Role.GetRole<Sheriff>(sheriff).UsedThisRound = true;
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

                    case CustomRPC.SetGuardianAngel:
                        new GuardianAngel(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetTarget:
                        var exe = Utils.PlayerById(reader.ReadByte());
                        var exeTarget = Utils.PlayerById(reader.ReadByte());
                        var exeRole = Role.GetRole<Executioner>(exe);
                        exeRole.target = exeTarget;
                        break;

                    case CustomRPC.SetGATarget:
                        var ga = Utils.PlayerById(reader.ReadByte());
                        var gaTarget = Utils.PlayerById(reader.ReadByte());
                        var gaRole = Role.GetRole<GuardianAngel>(ga);
                        gaRole.target = gaTarget;
                        break;

                    case CustomRPC.SetBlackmailer:
                        new Blackmailer(Utils.PlayerById(reader.ReadByte()));
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

                    case CustomRPC.ExecutionerToJester:
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
                        PerformKill.SpawnVent(ventId, minerRole, pos, zAxis);
                        break;

                    case CustomRPC.Freeze:
                        var tm = Utils.PlayerById(reader.ReadByte());
                        var tmRole = Role.GetRole<TimeMaster>(tm);
                        tmRole.TimeRemaining = CustomGameOptions.FreezeDuration;
                        Utils.TimeFreeze();
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
                        {
                            Role.GetRole<Transporter>(PlayerControl.LocalPlayer).UntransportablePlayers.Add(reader.ReadByte(), DateTime.UtcNow);
                        }
                        break;

                    case CustomRPC.Mediate:
                        var mediatedPlayer = Utils.PlayerById(reader.ReadByte());
                        var medium = Role.GetRole<Medium>(Utils.PlayerById(reader.ReadByte()));
                        if (PlayerControl.LocalPlayer.PlayerId != mediatedPlayer.PlayerId) break;
                        medium.AddMediatePlayer(mediatedPlayer.PlayerId);
                        break;

                    case CustomRPC.SetGrenadier:
                        new Grenadier(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetDisguiser:
                        new Disguiser(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.FlashGrenade:
                        var grenadier = Utils.PlayerById(reader.ReadByte());
                        var grenadierRole = Role.GetRole<Grenadier>(grenadier);
                        grenadierRole.TimeRemaining = CustomGameOptions.GrenadeDuration;
                        grenadierRole.Flash();
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

                    /*case CustomRPC.SetSleuth:
                        new Sleuth(Utils.PlayerById(reader.ReadByte()));
                        break;*/

                    case CustomRPC.SetArsonist:
                        new Arsonist(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetWerewolf:
                        new Werewolf(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.Douse:
                        var arsonist = Utils.PlayerById(reader.ReadByte());
                        var douseTarget = Utils.PlayerById(reader.ReadByte());
                        var arsonistRole = Role.GetRole<Arsonist>(arsonist);
                        arsonistRole.DousedPlayers.Add(douseTarget.PlayerId);
                        arsonistRole.LastDoused = DateTime.UtcNow;
                        break;

                    case CustomRPC.Ignite:
                        var theArsonist = Utils.PlayerById(reader.ReadByte());
                        var theArsonistRole = Role.GetRole<Arsonist>(theArsonist);
                        theArsonistRole.Ignite();
                        break;

                    case CustomRPC.ArsonistWin:
                        var theArsonistTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Arsonist);
                        ((Arsonist)theArsonistTheRole)?.Wins();
                        break;

                    case CustomRPC.ArsonistLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Arsonist) ((Arsonist)role).Loses();
                        break;

                    case CustomRPC.WerewolfWin:
                        var theWerewolfTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Werewolf);
                        ((Werewolf)theWerewolfTheRole)?.Wins();
                        break;

                    case CustomRPC.WerewolfLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Werewolf) ((Werewolf)role).Loses();
                        break;

                    case CustomRPC.SurvivorWin:
                        var theSurvTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Survivor);
                        ((Survivor)theSurvTheRole)?.Wins();
                        break;

                    case CustomRPC.SurvivorLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Survivor && role.Player.Data.IsDead || role.Player.Data.Disconnected)
                                ((Survivor)role).Loses();
                        break;

                    case CustomRPC.GAWin:
                        var theGATheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.GuardianAngel);
                        ((GuardianAngel)theGATheRole)?.Wins();
                        break;

                    case CustomRPC.GALose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.GuardianAngel && ((GuardianAngel)role).target.Data.IsDead)
                                ((GuardianAngel)role).Loses();
                        break;

                    case CustomRPC.PlaguebearerWin:
                        var thePlaguebearerTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Plaguebearer);
                        ((Plaguebearer)thePlaguebearerTheRole)?.Wins();
                        break;

                    case CustomRPC.PlaguebearerLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Plaguebearer)
                                ((Plaguebearer) role).Loses();
                        break;

                    case CustomRPC.TaskmasterWin:
                        var theTMTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Taskmaster);
                        ((Taskmaster)theTMTheRole)?.Wins();
                        break;

                    case CustomRPC.TaskmasterLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Taskmaster)
                                ((Taskmaster) role).Loses();
                        break;

                    case CustomRPC.CannibalEat:
                        readByte1 = reader.ReadByte();
                        var cannibalPlayer = Utils.PlayerById(readByte1);
                        var cannibalRole = Role.GetRole<Cannibal>(cannibalPlayer);
                        readByte = reader.ReadByte();
                        var deads = Object.FindObjectsOfType<DeadBody>();
                        foreach (var body in deads)
                            if (body.ParentId == readByte)
                                Coroutines.Start(Eat.EatCoroutine(body, cannibalRole));
                        cannibalRole.LastEaten = DateTime.UtcNow;
                        break;

                    case CustomRPC.CannibalWin:
                        var theCannibalRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Cannibal);
                        ((Cannibal)theCannibalRole)?.Wins();
                        break;

                    case CustomRPC.CannibalLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Cannibal)
                                ((Cannibal)role).Loses();
                        break;

                    case CustomRPC.Infect:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).InfectedPlayers.Add(reader.ReadByte());
                        break;

                    case CustomRPC.TurnPestilence:
                        Role.GetRole<Plaguebearer>(Utils.PlayerById(reader.ReadByte())).TurnPestilence();
                        break;

                    case CustomRPC.PestilenceWin:
                        var thePestilenceTheRole = Role.AllRoles.FirstOrDefault(x => x.RoleType == RoleEnum.Pestilence);
                        ((Pestilence)thePestilenceTheRole)?.Wins();
                        break;

                    case CustomRPC.PestilenceLose:
                        foreach (var role in Role.AllRoles)
                            if (role.RoleType == RoleEnum.Pestilence)
                                ((Pestilence)role).Loses();
                        break;

                    case CustomRPC.SetImpostor:
                        new Impostor(Utils.PlayerById(reader.ReadByte()));
                        break;

                    case CustomRPC.SetCrewmate:
                        new Crewmate(Utils.PlayerById(reader.ReadByte()));
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
                                    Coroutines.Start(Utils.FlashCoroutine(altruistRole.Color, CustomGameOptions.ReviveDuration, 0.5f));

                                Coroutines.Start(global::TownOfUs.CrewmateRoles.AltruistMod.Coroutine.AltruistRevive(body, altruistRole));
                            }
                        }
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
                            AmongUsClient.Instance.DisconnectHandlers.AddUnique(MeetingRoomManager.Instance
                                .Cast<IDisconnectHandler>());
                            if (ShipStatus.Instance.CheckTaskCompletion()) return;

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
                            if (body.ParentId == readByte)
                                dienerRole.CurrentlyDragging = body;
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

                    case CustomRPC.SetMystic:
                        new Mystic(Utils.PlayerById(reader.ReadByte()));
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
                        if (!PlayerControl.LocalPlayer.Is(RoleEnum.Haunter))
                        {
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        }
                        System.Console.WriteLine("Become Phantom - Users");
                        break;

                    case CustomRPC.CatchPhantom:
                        var phantomPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Phantom>(phantomPlayer).Caught = true;
                        break;

                    case CustomRPC.PhantomWin:
                        Role.GetRole<Phantom>(Utils.PlayerById(reader.ReadByte())).CompletedTasks = true;
                        break;

                    case CustomRPC.SetHaunter:
                        readByte = reader.ReadByte();
                        SetHaunter.WillBeHaunter = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.HaunterDied:
                        var haunter = Utils.PlayerById(reader.ReadByte());
                        Role.RoleDictionary.Remove(haunter.PlayerId);
                        var haunterRole = new Haunter(haunter);
                        haunterRole.RegenTask();
                        haunter.gameObject.layer = LayerMask.NameToLayer("Players");
                        SetHaunter.RemoveTasks(haunter);
                        SetHaunter.AddCollider(haunterRole);
                        if (!PlayerControl.LocalPlayer.Is(RoleEnum.Phantom))
                        {
                            PlayerControl.LocalPlayer.MyPhysics.ResetMoveState();
                        }
                        System.Console.WriteLine("Become Haunter - Users");
                        break;

                    case CustomRPC.CatchHaunter:
                        var haunterPlayer = Utils.PlayerById(reader.ReadByte());
                        Role.GetRole<Haunter>(haunterPlayer).Caught = true;
                        break;

                    case CustomRPC.HaunterFinished:
                        HighlightImpostors.UpdateMeeting(MeetingHud.Instance);
                        break;

                    case CustomRPC.SetTraitor:
                        readByte = reader.ReadByte();
                        SetTraitor.WillBeTraitor = readByte == byte.MaxValue ? null : Utils.PlayerById(readByte);
                        break;

                    case CustomRPC.TraitorSpawn:
                        var traitor = SetTraitor.WillBeTraitor;
                        var oldRole = Role.GetRole(traitor).RoleType;
                        Role.RoleDictionary.Remove(traitor.PlayerId);
                        var traitorRole = new Traitor(traitor);
                        traitorRole.formerRole = oldRole;
                        traitorRole.RegenTask();
                        SetTraitor.TurnImp(traitor);
                        break;

                    case CustomRPC.SetPlaguebearer:
                        new Plaguebearer(Utils.PlayerById(reader.ReadByte()));
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
                        Patches.SubmergedCompatibility.RepairOxygen();
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
                        if (CustomGameOptions.AutoAdjustSettings) RandomMap.AdjustSettings(readByte);
                        break;

                    case CustomRPC.Camouflage:
                        var camouflager = Utils.PlayerById(reader.ReadByte());
                        var camouflagerRole = Role.GetRole<Camouflager>(camouflager);
                        camouflagerRole.TimeRemaining = CustomGameOptions.CamouflagerDuration;
                        Utils.Camouflage();
                        break;
                }
            }
        }

        [HarmonyPatch(typeof(RoleManager), nameof(RoleManager.SelectRoles))]
        public static class RpcSetRole
        {
            public static void Postfix()
            {
                PluginSingleton<TownOfUs>.Instance.Log.LogMessage("RPC SET ROLE");
                var infected = GameData.Instance.AllPlayers.ToArray().Where(o => o.IsImpostor());

                Utils.ShowDeadBodies = false;
                Role.NobodyWins = false;
                Role.SurvOnlyWins = false;
                CrewmateRoles.Clear();
                NeutralNonKillingRoles.Clear();
                NeutralKillingRoles.Clear();
                ImpostorRoles.Clear();
                CrewmateModifiers.Clear();
                GlobalModifiers.Clear();
                ButtonModifiers.Clear();
                BaitModifiers.Clear();
                //GuiltyModifiers.Clear();
                ObjectifierAbility.Clear();

                RecordRewind.points.Clear();
                Murder.KilledPlayers.Clear();
                KillButtonTarget.DontRevive = byte.MaxValue;

                var startWriter = AmongUsClient.Instance.StartRpcImmediately(PlayerControl.LocalPlayer.NetId,
                    (byte) CustomRPC.Start, SendOption.Reliable, -1);
                AmongUsClient.Instance.FinishRpcImmediately(startWriter);

                if (CustomGameOptions.GameMode != GameMode.KillingOnly)
                {
                    PhantomOn = Check(CustomGameOptions.PhantomOn);
                    HaunterOn = Check(CustomGameOptions.HaunterOn);
                    TraitorOn = Check(CustomGameOptions.TraitorOn);
                }
                else
                {
                    PhantomOn = false;
                    HaunterOn = false;
                    TraitorOn = true;
                }

                if (CustomGameOptions.GameMode != GameMode.KillingOnly)
                {
                    #region Crewmate Roles
                    if (CustomGameOptions.MayorOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MayorCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Mayor), CustomRPC.SetMayor, CustomGameOptions.MayorOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Mayor), CustomRPC.SetMayor, CustomGameOptions.MayorOn, true));
                    }

                    if (CustomGameOptions.SheriffOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.SheriffCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Sheriff), CustomRPC.SetSheriff, CustomGameOptions.SheriffOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Sheriff), CustomRPC.SetSheriff, CustomGameOptions.SheriffOn, true));
                    }

                    if (CustomGameOptions.VigilanteOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.VigilanteCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Vigilante), CustomRPC.SetVigilante, CustomGameOptions.VigilanteOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Vigilante), CustomRPC.SetVigilante, CustomGameOptions.VigilanteOn, true));
                    }

                    if (CustomGameOptions.EngineerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.EngineerCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Engineer), CustomRPC.SetEngineer, CustomGameOptions.EngineerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Engineer), CustomRPC.SetEngineer, CustomGameOptions.EngineerOn, true));
                    }

                    if (CustomGameOptions.SwapperOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.SwapperCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Swapper), CustomRPC.SetSwapper, CustomGameOptions.SwapperOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Swapper), CustomRPC.SetSwapper, CustomGameOptions.SwapperOn, true));
                    }

                    if (CustomGameOptions.InvestigatorOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.InvestigatorCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Investigator), CustomRPC.SetInvestigator, CustomGameOptions.InvestigatorOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Investigator), CustomRPC.SetInvestigator, CustomGameOptions.InvestigatorOn, true));
                    }

                    if (CustomGameOptions.TimeLordOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.TimeLordCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(TimeLord), CustomRPC.SetTimeLord, CustomGameOptions.TimeLordOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(TimeLord), CustomRPC.SetTimeLord, CustomGameOptions.TimeLordOn, true));
                    }

                    if (CustomGameOptions.MedicOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MedicCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Medic), CustomRPC.SetMedic, CustomGameOptions.MedicOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Medic), CustomRPC.SetMedic, CustomGameOptions.MedicOn, true));
                    }

                    //if (CustomGameOptions.SeerOn > 0)
                        //CrewmateRoles.Add((typeof(Seer), CustomRPC.SetSeer, CustomGameOptions.SeerOn, false));

                    if (CustomGameOptions.AgentOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.AgentCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Agent), CustomRPC.SetAgent, CustomGameOptions.AgentOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Agent), CustomRPC.SetAgent, CustomGameOptions.AgentOn, true));
                    }

                    if (CustomGameOptions.SnitchOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.SnitchCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Snitch), CustomRPC.SetSnitch, CustomGameOptions.SnitchOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Snitch), CustomRPC.SetSnitch, CustomGameOptions.SnitchOn, true));
                    }

                    if (CustomGameOptions.AltruistOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.AltruistCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Altruist), CustomRPC.SetAltruist, CustomGameOptions.AltruistOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Altruist), CustomRPC.SetAltruist, CustomGameOptions.AltruistOn, true));
                    }

                    if (CustomGameOptions.VeteranOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.VeteranCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, CustomGameOptions.VeteranOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Veteran), CustomRPC.SetVeteran, CustomGameOptions.VeteranOn, true));
                    }

                    if (CustomGameOptions.TrackerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.TrackerCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Tracker), CustomRPC.SetTracker, CustomGameOptions.TrackerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Tracker), CustomRPC.SetTracker, CustomGameOptions.TrackerOn, true));
                    }

                    if (CustomGameOptions.TransporterOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.TransporterCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Transporter), CustomRPC.SetTransporter, CustomGameOptions.TransporterOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Transporter), CustomRPC.SetTransporter, CustomGameOptions.TransporterOn, true));
                    }

                    if (CustomGameOptions.MediumOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MediumCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Medium), CustomRPC.SetMedium, CustomGameOptions.MediumOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Medium), CustomRPC.SetMedium, CustomGameOptions.MediumOn, true));
                    }

                    if (CustomGameOptions.MysticOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MysticCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Mystic), CustomRPC.SetMystic, CustomGameOptions.MysticOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Mystic), CustomRPC.SetMystic, CustomGameOptions.MysticOn, true));
                    }

                    if (CustomGameOptions.OperativeOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.OperativeCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Operative), CustomRPC.SetOperative, CustomGameOptions.OperativeOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Operative), CustomRPC.SetOperative, CustomGameOptions.OperativeOn, true));
                    }

                    if (CustomGameOptions.DetectiveOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.DetectiveCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Detective), CustomRPC.SetDetective, CustomGameOptions.DetectiveOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Detective), CustomRPC.SetDetective, CustomGameOptions.DetectiveOn, true));
                    }

                    if (CustomGameOptions.ShifterOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.ShifterCount;
                            do
                            {
                                CrewmateRoles.Add((typeof(Shifter), CustomRPC.SetShifter, CustomGameOptions.ShifterOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            CrewmateRoles.Add((typeof(Shifter), CustomRPC.SetShifter, CustomGameOptions.ShifterOn, true));
                    }
                    #endregion
                    
                    #region Neutral Roles
                    if (CustomGameOptions.JesterOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MayorCount;
                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Jester), CustomRPC.SetJester, CustomGameOptions.JesterOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Jester), CustomRPC.SetJester, CustomGameOptions.JesterOn, true));
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
                    }

                    if (CustomGameOptions.TaskmasterOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.TaskmasterCount;
                            do
                            {
                                NeutralNonKillingRoles.Add((typeof(Taskmaster), CustomRPC.SetTaskmaster, CustomGameOptions.TaskmasterOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            NeutralNonKillingRoles.Add((typeof(Taskmaster), CustomRPC.SetTaskmaster, CustomGameOptions.TaskmasterOn, true));
                    }
                    #endregion

                    #region Impostor Roles
                    if (CustomGameOptions.UndertakerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.UndertakerCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Undertaker), CustomRPC.SetUndertaker, CustomGameOptions.UndertakerOn, true));
                    }

                    if (CustomGameOptions.UnderdogOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.UnderdogCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Underdog), CustomRPC.SetUnderdog, CustomGameOptions.UnderdogOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Underdog), CustomRPC.SetUnderdog, CustomGameOptions.UnderdogOn, true));
                    }

                    if (CustomGameOptions.MorphlingOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MorphlingCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Morphling), CustomRPC.SetMorphling, CustomGameOptions.MorphlingOn, true));
                    }

                    if (CustomGameOptions.BlackmailerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.BlackmailerCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Blackmailer), CustomRPC.SetBlackmailer, CustomGameOptions.BlackmailerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Blackmailer), CustomRPC.SetBlackmailer, CustomGameOptions.BlackmailerOn, true));
                    }

                    if (CustomGameOptions.MinerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.MinerCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Miner), CustomRPC.SetMiner, CustomGameOptions.MinerOn, true));
                    }

                    if (CustomGameOptions.WraithOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.WraithCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Wraith), CustomRPC.SetWraith, CustomGameOptions.WraithOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Wraith), CustomRPC.SetWraith, CustomGameOptions.WraithOn, true));
                    }

                    if (CustomGameOptions.JanitorOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.JanitorCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Janitor), CustomRPC.SetJanitor, CustomGameOptions.JanitorOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Janitor), CustomRPC.SetJanitor, CustomGameOptions.JanitorOn, true));
                    }

                    if (CustomGameOptions.CamouflagerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.CamouflagerCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Camouflager), CustomRPC.SetCamouflager, CustomGameOptions.CamouflagerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Camouflager), CustomRPC.SetCamouflager, CustomGameOptions.CamouflagerOn, true));
                    }

                    if (CustomGameOptions.GrenadierOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.GrenadierCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, CustomGameOptions.GrenadierOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Grenadier), CustomRPC.SetGrenadier, CustomGameOptions.GrenadierOn, true));
                    }

                    if (CustomGameOptions.PoisonerOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.PoisonerCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Poisoner), CustomRPC.SetPoisoner, CustomGameOptions.PoisonerOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Poisoner), CustomRPC.SetPoisoner, CustomGameOptions.PoisonerOn, true));
                    }
                    
                    if (CustomGameOptions.ConsigliereOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.ConsigliereCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Consigliere), CustomRPC.SetConsigliere, CustomGameOptions.ConsigliereOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Consigliere), CustomRPC.SetConsigliere, CustomGameOptions.ConsigliereOn, true));
                    }
                    
                    if (CustomGameOptions.DisguiserOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.DisguiserCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(Disguiser), CustomRPC.SetDisguiser, CustomGameOptions.DisguiserOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(Disguiser), CustomRPC.SetDisguiser, CustomGameOptions.DisguiserOn, true));
                    }
                    
                    if (CustomGameOptions.TMOn > 0)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                        {
                            var number = CustomGameOptions.TimeMasterCount;
                            do
                            {
                                ImpostorRoles.Add((typeof(TimeMaster), CustomRPC.SetTimeMaster, CustomGameOptions.TMOn, true));
                                number--;
                            } while (number > 0);
                        }
                        else
                            ImpostorRoles.Add((typeof(TimeMaster), CustomRPC.SetTimeMaster, CustomGameOptions.TMOn, true));
                    }
                    #endregion

                    #region Positive Modifiers
                    if (CustomGameOptions.TorchOn > 0)
                        CrewmateModifiers.Add((typeof(Torch), CustomRPC.SetTorch, CustomGameOptions.TorchOn));

                    if (CustomGameOptions.LighterOn > 0)
                        CrewmateModifiers.Add((typeof(Lighter), CustomRPC.SetLighter, CustomGameOptions.LighterOn));

                    if (CustomGameOptions.DiseasedOn > 0)
                        GlobalModifiers.Add((typeof(Diseased), CustomRPC.SetDiseased, CustomGameOptions.DiseasedOn));

                    if (CustomGameOptions.BaitOn > 0)
                        BaitModifiers.Add((typeof(Bait), CustomRPC.SetBait, CustomGameOptions.BaitOn));

                    if (CustomGameOptions.ButtonBarryOn > 0)
                        ButtonModifiers.Add((typeof(ButtonBarry), CustomRPC.SetButtonBarry, CustomGameOptions.ButtonBarryOn));

                    if (CustomGameOptions.TiebreakerOn > 0)
                        GlobalModifiers.Add((typeof(Tiebreaker), CustomRPC.SetTiebreaker, CustomGameOptions.TiebreakerOn));
                    #endregion

                    #region Neutral Modifiers
                    if (CustomGameOptions.DwarfOn > 0)
                        GlobalModifiers.Add((typeof(Dwarf), CustomRPC.SetDwarf, CustomGameOptions.DwarfOn));

                    if (CustomGameOptions.GiantOn > 0)
                        GlobalModifiers.Add((typeof(Giant), CustomRPC.SetGiant, CustomGameOptions.GiantOn));

                    if (CustomGameOptions.LoversOn > 0)
                        GlobalModifiers.Add((typeof(Lover), CustomRPC.SetCouple, CustomGameOptions.LoversOn));
                    #endregion

                    #region Negative Modifiers
                    if (CustomGameOptions.DrunkOn > 0)
                        GlobalModifiers.Add((typeof(Drunk), CustomRPC.SetDrunk, CustomGameOptions.DrunkOn));

                    if (CustomGameOptions.CowardOn > 0)
                        GlobalModifiers.Add((typeof(Coward), CustomRPC.SetCoward, CustomGameOptions.CowardOn));

                    if (CustomGameOptions.VolatileOn > 0)
                        GlobalModifiers.Add((typeof(Volatile), CustomRPC.SetVolatile, CustomGameOptions.VolatileOn));
                    #endregion

                    #region Ability Objectifiers
                    if (CustomGameOptions.GameMode == GameMode.Custom || CustomGameOptions.GameMode == GameMode.AllAny)
                    {
                        if (CustomGameOptions.GameMode == GameMode.Custom)
                            ObjectifierAbility.Add((typeof(Assassin), CustomRPC.SetAssassin, 100));
                        else if (CustomGameOptions.GameMode == GameMode.AllAny)
                            ObjectifierAbility.Add((typeof(Assassin), CustomRPC.SetAssassin, 50));
                    }
                    #endregion
                }

                if (CustomGameOptions.GameMode == GameMode.KillingOnly) GenEachRoleKilling(infected.ToList());
                else GenEachRole(infected.ToList());
            }
        }
    }
}
