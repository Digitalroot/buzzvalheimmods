﻿using System.Collections.Generic;

namespace OdinPlusRemakeJVL.Common
{
  public static class ItemNames
  {
    public static string Chest = nameof(Chest);
    public static string LegacyChest = nameof(LegacyChest);
    public static string LeatherScraps = nameof(LeatherScraps);
    public static string MeadTasty = nameof(MeadTasty);
    public static string OdinLegacy = nameof(OdinLegacy);
    public static string HelmetOdin = nameof(HelmetOdin);
    public static string ScrollTroll = nameof(ScrollTroll);
    public static string ScrollWolf = nameof(ScrollWolf);
    public static string TrophyGoblinShaman = nameof(TrophyGoblinShaman);
    public static string TrophyFrostTroll = nameof(TrophyFrostTroll);
    public static string TrophyWolf = nameof(TrophyWolf);
    public static string Coins = nameof(Coins);
    public static string Mushroom = nameof(Mushroom);
    public static string CookedMeat = nameof(CookedMeat);
    public static string Raspberry = nameof(Raspberry);
    public static string Stone = nameof(Stone);
    public static string Wood = nameof(Wood);
    public static string DeerHide = nameof(DeerHide);
    public static string Resin = nameof(Resin);
    public static string TrollHide = nameof(TrollHide);
    public static string BoneFragments = nameof(BoneFragments);
    public static string MushroomYellow = nameof(MushroomYellow);
    public static string Blueberries = nameof(Blueberries);
    public static string HardAntler = nameof(HardAntler);
    public static string Guck = nameof(Guck);
    public static string Ooze = nameof(Ooze);
    public static string SurtlingCore = nameof(SurtlingCore);
    public static string ElderBark = nameof(ElderBark);
    public static string Amber = nameof(Amber);
    public static string AmberPearl = nameof(AmberPearl);
    public static string DragonEgg = nameof(DragonEgg);
    public static string WolfFang = nameof(WolfFang);
    public static string WolfPelt = nameof(WolfPelt);
    public static string Bread = nameof(Bread);

    public static readonly IEnumerable<string> AllNames = Common.Utils.AllNames(typeof(MeadNames));
  }
}
