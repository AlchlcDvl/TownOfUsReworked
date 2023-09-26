namespace TownOfUsReworked.Data;

public static class Colors
{
    //Layer Colors
    public static Color Role => new Color32(255, 215, 0, 255); //#FFD700FF
    public static Color Modifier => new Color32(128, 128, 128, 255); //#7F7F7FFF
    public static Color Ability => new Color32(255, 153, 0, 255); //#FF9900FF
    public static Color Layer => new Color32(38, 132, 193, 255); //#2684C1FF
    public static Color Objectifier => new Color32(221, 88, 91, 255); //#DD585BFF

    //Faction Colors
    public static Color Crew => new Color32(140, 255, 255, 255); //#8CFFFFFF
    public static Color Neutral => new Color32(179, 179, 179, 255); //#B3B3B3FF
    public static Color Intruder => new Color32(255, 0, 0, 255); //#FF0000FF
    public static Color Syndicate => new Color32(0, 128, 0, 255); //#008000FF
    public static Color Faction => new Color32(0, 230, 109, 255); //#00E66DFF

    //Subfaction Colors
    public static Color Undead => new Color32(123, 137, 104, 255); //#7B8968FF
    public static Color Cabal => new Color32(87, 87, 87, 255); //#575757FF
    public static Color Reanimated => new Color32(230, 16, 138, 255); //#E6108AFF
    public static Color Sect => new Color32(249, 149, 252, 255); //#F995FCFF
    public static Color Apocalypse => new Color32(167, 197, 150, 255); //#A7C596FF
    public static Color SubFaction => new Color32(32, 77, 66, 255); //#204D42FF

    //Crew Colors
    public static Color Mayor => new Color32(112, 79, 168, 255); //#704FA8FF
    public static Color Vigilante => new Color32(255, 255, 0, 255); //#FFFF00FF
    public static Color Engineer => new Color32(255, 166, 10, 255); //#FFA60AFF
    public static Color Medic => new Color32(0, 102, 0, 255); //#006600FF
    public static Color Sheriff => new Color32(255, 204, 128, 255); //#FFCC80FF
    public static Color Altruist => new Color32(102, 0, 0, 255); //#660000FF
    public static Color Veteran => new Color32(153, 128, 64, 255); //#998040FF
    public static Color Tracker => new Color32(0, 153, 0, 255); //#009900FF
    public static Color Transporter => new Color32(0, 238, 255, 255); //#00EEFFFF
    public static Color Medium => new Color32(166, 128, 255, 255); //#A680FFFF
    public static Color Coroner => new Color32(77, 153, 230, 255); //#4D99E6FF
    public static Color Operative => new Color32(167, 209, 179, 255); //#A7D1B3FF
    public static Color Detective => new Color32(77, 77, 255, 255); //#4D4DFFFF
    public static Color Shifter => new Color32(223, 133, 31, 255); //#DF851FFF
    public static Color VampireHunter => new Color32(192, 192, 192, 255); //#C0C0C0FF
    public static Color Escort => new Color32(128, 51, 51, 255); //#803333FF
    public static Color Inspector => new Color32(126, 60, 100, 255); //#7E3C64FF
    public static Color Revealer => new Color32(211, 211, 211, 255); //#D3D3D3FF
    public static Color Mystic => new Color32(112, 142, 239, 255); //#708EEFFF
    public static Color Retributionist => new Color32(141, 15, 140, 255); //#8D0F8CFF
    public static Color Chameleon => new Color32(84, 17, 248, 255); //#5411F8FF
    public static Color Seer => new Color32(113, 54, 138, 255); //#71368AFF
    public static Color Monarch => new Color32(255, 0, 78, 255); //#FF004EFF
    public static Color Dictator => new Color32(0, 203, 151, 255); //#00CB97FF

    //Neutral Colors
    public static Color Jester => new Color32(247, 179, 218, 255); //#F7B3DAFF
    public static Color Executioner => new Color32(204, 204, 204, 255); //#CCCCCCFF
    public static Color Glitch => new Color32(0, 255, 0, 255); //#00FF00FF
    public static Color Arsonist => new Color32(238, 118, 0, 255); //#EE7600FF
    public static Color Amnesiac => new Color32(34, 255, 255, 255); //#22FFFFFF
    public static Color Survivor => new Color32(221, 221, 0, 255); //#DDDD00FF
    public static Color GuardianAngel => new Color32(255, 255, 255, 255); //#FFFFFFFF
    public static Color Plaguebearer => new Color32(207, 254, 97, 255); //#CFFE61FF
    public static Color Pestilence => new Color32(66, 66, 66, 255); //#424242FF
    public static Color Werewolf => new Color32(159, 112, 58, 255); //#9F703AFF
    public static Color Cannibal => new Color32(140, 64, 5, 255); //#8C4005FF
    public static Color Juggernaut => new Color32(161, 43, 86, 255); //#A12B56FF
    public static Color Dracula => new Color32(172, 138, 0, 255); //#AC8A00FF
    public static Color Murderer => new Color32(111, 123, 234, 255); //#6F7BEAFF
    public static Color SerialKiller => new Color32(51, 110, 255, 255); //#336EFFFF
    public static Color Cryomaniac => new Color32(100, 45, 234, 255); //#642DEAFF
    public static Color Thief => new Color32(128, 255, 0, 255); //#80FF00FF
    public static Color Troll => new Color32(103, 141, 54, 255); //#678D36FF
    public static Color Jackal => new Color32(69, 7, 106, 255); //#45076AFF
    public static Color Phantom => new Color32(102, 41, 98, 255); //#662962FF
    public static Color Necromancer => new Color32(191, 95, 255, 255); //#BF5FFFFF
    public static Color Whisperer => new Color32(45, 106, 165, 255); //#2D6AA5FF
    public static Color Guesser => new Color32(238, 229, 190, 255); //#EEE5BEFF
    public static Color Actor => new Color32(0, 172, 194, 255); //#00ACC2FF
    public static Color BountyHunter => new Color32(181, 30, 57, 255); //#B51E39FF
    public static Color Betrayer => new Color32(17, 128, 106, 255); //#11806AFF

    //Intruder Colors
    public static Color Consigliere => new Color32(255, 255, 153, 255); //#FFFF99FF
    public static Color Grenadier => new Color32(133, 170, 91, 255); //#85AA5BFF
    public static Color Morphling => new Color32(187, 69, 176, 255); //#BB45B0FF
    public static Color Wraith => new Color32(92, 79, 117, 255); //#5C4F75FF
    public static Color Camouflager => new Color32(55, 138, 192, 255); //#378AC0FF
    public static Color Janitor => new Color32(38, 71, 162, 255); //#2647A2FF
    public static Color Miner => new Color32(170, 118, 50, 255); //#AA7632FF
    public static Color Blackmailer => new Color32(2, 167, 162, 255); //#02A752FF
    public static Color Disguiser => new Color32(64, 180, 255, 255); //#40B4FFFF
    public static Color Consort => new Color32(128, 23, 128, 255); //#801780FF
    public static Color Teleporter => new Color32(147, 149, 147, 255); //#939593FF
    public static Color Godfather => new Color32(64, 76, 8, 255); //#404C08FF
    public static Color Mafioso => new Color32(100, 0, 255, 255); //#6400FFFF
    public static Color Ambusher => new Color32(43, 210, 156, 255); //#2BD29CFF
    public static Color Ghoul => new Color32(241, 196, 15, 255); //#F1C40FFF
    public static Color Enforcer => new Color32(0, 86, 67, 255); //#005643FF

    //Syndicate Colors
    public static Color Warper => new Color32(140, 113, 64, 255); //#8C7140FF
    public static Color Framer => new Color32(0, 255, 255, 255); //#00FFFFFF
    public static Color Rebel => new Color32(255, 252, 206, 255); //#FFFCCEFF
    public static Color Sidekick => new Color32(151, 156, 159, 255); //#979C9FFF
    public static Color Concealer => new Color32(192, 37, 37, 255); //#C02525FF
    public static Color Shapeshifter => new Color32(45, 255, 0, 255); //#2DFF00FF
    public static Color Bomber => new Color32(201, 204, 63, 255); //#C9CC3FFF
    public static Color Poisoner => new Color32(181, 0, 76, 255); //#B5004CFF
    public static Color Crusader => new Color32(223, 122, 232, 255); //#DF7AE8FF
    public static Color Banshee => new Color32(230, 126, 34, 255); //#E67E22FF
    public static Color Collider => new Color32(179, 69, 255, 255); //#B345FFFF
    public static Color Stalker => new Color32(126, 77, 0, 255); //#7E4D00FF
    public static Color Spellslinger => new Color32(0, 40, 245, 255); //#0028F5FF
    public static Color Drunkard => new Color32(255, 121, 0, 255); //#FF7900FF
    public static Color Timekeeper => new Color32(55, 105, 254, 255); //#3769FEFF
    public static Color Silencer => new Color32(170, 180, 62, 255); //#AAB43EFF

    //Modifier Colors
    public static Color Bait => new Color32(0, 179, 179, 255); //#00B3B3FF
    public static Color Coward => new Color32(69, 107, 168, 255); //#456BA8FF
    public static Color Diseased => new Color32(55, 77, 30, 255); //#374D1EFF
    public static Color Drunk => new Color32(117, 128, 0, 255); //#758000FF
    public static Color Dwarf => new Color32(255, 128, 128, 255); //#FF8080FF
    public static Color Giant => new Color32(255, 179, 77, 255); //#FFB34DFF
    public static Color Volatile => new Color32(255, 166, 10, 255); //#FFA60AFF
    public static Color VIP => new Color32(220, 238, 133, 255); //#DCEE85FF
    public static Color Shy => new Color32(16, 2, 197, 255); //#1002C5FF
    public static Color Professional => new Color32(134, 11, 122, 255); //#860B7AFF
    public static Color Indomitable => new Color32(45, 229, 190, 255); //#2DE5BEFF
    public static Color Astral => new Color32(97, 43, 239, 255); //#612BEFFF
    public static Color Yeller => new Color32(246, 170, 183, 255); //#F6AAB7FF

    //Ability Colors
    public static Color Assassin => new Color32(7, 55, 99, 255); //#073763FF
    public static Color Torch => new Color32(255, 255, 153, 255); //#FFFF99FF
    public static Color Tunneler => new Color32(233, 30, 99, 255); //#E91E63FF
    public static Color ButtonBarry => new Color32(230, 0, 255, 255); //#E600FFFF
    public static Color Tiebreaker => new Color32(153, 230, 153, 255); //#99E699FF
    public static Color Snitch => new Color32(212, 174, 55, 255); //#D4AF37FF
    public static Color Underdog => new Color32(132, 26, 127, 255); //#841A7FFF
    public static Color Insider => new Color32(38, 252, 251, 255); //#26FCFBFF
    public static Color Radar => new Color32(255, 0, 128, 255); //#FF0080FF
    public static Color Multitasker => new Color32(255, 128, 77, 255); //#FF804DFF
    public static Color Ruthless => new Color32(33, 96, 221, 255); //#2160DDFF
    public static Color Ninja => new Color32(168, 67, 0, 255); //#A84300FF
    public static Color Politician => new Color32(204, 163, 204, 255); //#CCA3CCFF
    public static Color Swapper => new Color32(102, 230, 102, 255); //#66E666FF

    //Objectifier Colors
    public static Color Lovers => new Color32(255, 102, 204, 255); //#FF66CCFF
    public static Color Traitor => new Color32(55, 13, 67, 255); //#370D43FF
    public static Color Rivals => new Color32(61, 45, 44, 255); //#3D2D2CFF
    public static Color Fanatic => new Color32(103, 141, 54, 255); //#678D36FF
    public static Color Taskmaster => new Color32(171, 171, 255, 255); //#ABABFFFF
    public static Color Overlord => new Color32(0, 128, 128, 255); //#008080FF
    public static Color Corrupted => new Color32(69, 69, 255, 255); //#4545FFFF
    public static Color Allied => new Color32(69, 69, 169, 255); //#4545A9FF
    public static Color Mafia => new Color32(0, 238, 255, 255); //#00EEFFFF
    public static Color Defector => new Color32(225, 200, 73, 255); //#E1C849FF
    public static Color Linked => new Color32(255, 53, 31, 255); //# #FF351FFF

    //Other
    public static Color Stalemate => new Color32(239, 230, 230, 255); //#E6E6E6FF
    public static Color Alignment => new Color32(29, 124, 242, 255); //#1D7CF2FF
    public static Color Status => new Color32(155, 89, 182, 255); //#9B59B6FF
    public static Color Clear => new Color32(0, 0, 0, 0); //#00000000
    public static Color Objectives => new Color32(177, 72, 226, 255); //#B148E2FF
    public static Color Attributes => new Color32(236, 28, 69, 255); //#EC1C45FF
    public static Color Abilities => new Color32(32, 102, 148, 255); //#206694FF
    public static Color What => new Color32(102, 151, 255, 255); //#6697FFFF

    //Color Storage
    //#dcee85 #6c29ab #800000 #808000 #008000 #800080 #000080 #e74c3c #992d22 #00FFFD #917ac0 #Eac1d2 #286e58 #db4f20 #abd432 #2e3b97 #ffd100 #fffcce #40b4ff #a82626 #8ff731 #942b3b #80B3FF
    //#4e4e4e #fffead #1abc9c #2ecc71 #1f8b4c #3498db #ad1457 #c27c0e #ffd2fb #805bc4 #95a5a6 #979c9f #888888 #ff7272 #f25ff1 #FF00FF #916640 #1AFF74 #2672FF #8637C2 #e7dae2 #9B7038 #EDC240
    //#6a1515 #569d29 #f1612b #7d86e1 #EC62A5 #78c689 #fccc52 #6b2d2a #FCBA03 #F8CD46 #FF4D00 #7EFBC2 #4d4d4d #38b553 #0000FF #0000A7 #f25e75 #5865F2 #0437EF #7FFF00 #FB9327 #FAE57E #06DF0C
    //#1E300B #F3A6D3 #F9BFA7 #E1E4E4 #869919 #2B0538 #78081C #69201B #9000D7 #CF036F #FA1C79 #B0BF1A #A64D79 #B3D94D #73AD09 #41d1c3 #B0BF1A #80B2FF #33FF77 #AAFF00 #452112 #663366 #9C4A14
    //#1A3270 #C02A2C #E37C21 #20A1B7 #606168 #99007F #ECFF45 #BE1C8C #603FEF #610F69 #CBD542 #67A836 #B34D99 #06E00C #B545FF #DB7601 #1D4DFC #6699FF #9D7038 #FF004E #00CC99 #949797 #F5A6D4
    //#ECC23E #A9A9A9 #1F51FF #9C9A9A #A22929 #7500AF #B34D99 #1E49CF #FFC34F #A81538 #E6956A #404040

    //Symbol Storage
    //⟡ ☆ ♡ ♧ ♤ ▶ ❥ ✔ ι ν σ τ ψ ✧ ¢ 乂 ⁂ ¤ ∮ 彡 个 「 」 人 요 〖 〗 ロ 米 卄 王 ī l 【 】 · ㅇ ° ◈ ◆ ◇ ◥ ◤ ◢ ◣ 《 》 ︵ ︶ ☆ ☀ ☂ ☹ ☺ ♡ ♩ ♪ ♫ ♬ ✓ ☜ ☞ ☟ ☯ ☃ ✿ ❀ ÷ º ¿ ※ ⁑ ∞ ≠ +
}