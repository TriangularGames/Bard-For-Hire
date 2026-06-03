// Script containing all Project Enums

/// <summary>
/// Rarity level of an Item
/// </summary>
public enum ObjectRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}

/// <summary>
/// Types of Item damage
/// </summary>
public enum ItemType
{
    Piercing,
    Slashing,
    Magical
}

/// <summary>
/// Types of Upgrades
/// </summary>
public enum UpgradeType
{
    AttackHand,
    Dice
}

public enum UpgradeID
{
    ActionSurge,
    BattleTactics,
    LuckyStrike,
    RhythmicAttacks,
    OverwhelmingBlows,
    AdaptiveCombat,
    ShiningStar,
    SkillProficiency,
    SecondChance,
    EarlyAdvantage,
    Consistency,
    TimedSwings,
    FlowState,
    PerfectBattle,
    QuickSave,
    ComboChain,
    DoubleCrit,
    EchoStrike,
    MercenaryContract,
    Natural20,
    WeightedDice,
    FullHouse,
    Comprehension
}

public enum ConsumableID
{
    FocusPotion,
    PoisonPotion,
    SharpeningStone,
    LuckPotion,
    RerollPotion,
    PotionOfMelting,
    PotionOfPolymorph,
    PotionOfCloning
}

public enum WeaponBonus 
{ 
    None, 
    PercentHealth, 
    GrowingDamage
}

public enum BossAbilities
{
    EvenNumberReduce,
    DisableAction,
    None
}
