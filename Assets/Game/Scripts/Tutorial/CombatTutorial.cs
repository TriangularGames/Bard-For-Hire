using UnityEngine;
using System.Collections.Generic;
using static ItemManager;

public class CombatTutorial: MonoBehaviour
{
    [Header("Highlight Targets")] // Different highlight targets for the zones to emphasize
    [SerializeField] private GameObject attackHandHighlight;
    [SerializeField] private GameObject attackHighlight;
    [SerializeField] private GameObject dmgHandHighlight;
    [SerializeField] private GameObject rollHandHighlight;
    [SerializeField] private GameObject enemyHighlight;
    [SerializeField] private GameObject upgradeBarHighlight;
    [SerializeField] private GameObject consumableBarHighlight;
    [SerializeField] private GameObject discardButtonHighlight;
    [SerializeField] private GameObject clearButtonHighlight;
    [SerializeField] private GameObject inventoryButtonHighlight;

    private bool weaponClicked = false;
    private bool attackPressed = false;
    private ItemManager itemManager;

    private void Awake()
    {
        itemManager = FindFirstObjectByType<ItemManager>();
    }
    private void Start() // EventBus + Start tutorial up
    {
        if (TutorialManager.Instance == null || !TutorialManager.Instance.WeShowTutorial()) return;

        EventBus.Subscribe<RoundStartedEvent>(OnScoringStarted);
        EventBus.Subscribe<ScoringCompletedEvent>(OnRoundComplete);
        EventBus.Subscribe<TutorialDiscardCompletedEvent>(OnTutorialDiscardCompleted);
        itemManager.blockNormalDraw = true;
        Invoke(nameof(BeginTutorial), 0.05f);
    }


    private void Update()  // Checks for the weapon selected
    {
        if (TutorialManager.Instance.IsTutorialActive)
        {
            if (TutorialManager.Instance.IsWaitingForAction)
            {
                if (!weaponClicked && itemManager.ItemsSelected.Count > 0)
                {
                    weaponClicked = true;
                    TutorialManager.Instance.ActionComplete();
                    return;
                }
            }
        }
    }

    private void SetupForcedHand() // Forces the player hand so they have 2 daggers and 4 ray of frost
    {
        List<ItemData> forcedItems = new List<ItemData>();

        for (int i = 0; i < 2; i++)
        {
            ItemData piercing = PlayerManager.Instance.GetItemOfType(ItemType.Piercing);
            if (piercing != null) forcedItems.Add(piercing);
        }

        for (int i = 0; i < 4; i++)
        {
            ItemData magical = PlayerManager.Instance.GetItemOfType(ItemType.Magical);
            if (magical != null) forcedItems.Add(magical);
        }

        itemManager.ForceHandContents(forcedItems);
    }

    private void RefillWithPiercing() // Forces player hand to be refilled with daggers after they discard the ray of frosts
    {
        for (int i = 0; i < 4; i++)
        {
            ItemData piercing = PlayerManager.Instance.GetItemOfType(ItemType.Piercing);
            if (piercing != null)
                itemManager.itemPool.InstantiateItem(piercing);
        }
    }

    private void OnDestroy() // Unsubscribes
    {
        EventBus.Unsubscribe<RoundStartedEvent>(OnScoringStarted);
        EventBus.Unsubscribe<ScoringCompletedEvent>(OnRoundComplete);
        EventBus.Unsubscribe<TutorialDiscardCompletedEvent>(OnTutorialDiscardCompleted);
    }

    private void OnTutorialDiscardCompleted(TutorialDiscardCompletedEvent e) // Calls the refill when items discarded in tutorial
    {
        RefillWithPiercing();
        itemManager.noAttacking = false;
        TutorialManager.Instance.ActionComplete();
    }

    private void BeginTutorial() // Begin the tutorial
    {
        // Highlights start as false
        attackHandHighlight.SetActive(false);
        upgradeBarHighlight.SetActive(false);
        consumableBarHighlight.SetActive(false);
        discardButtonHighlight.SetActive(false);
        clearButtonHighlight.SetActive(false);
        var steps = new List<TutorialStep> // These are the necessary steps to complete in the tutorial
        {
            new TutorialStep
            {
                message = "Haha adventurer you fell for my trap. You're stuck in this dungeon now.\nWell uh I guess while you're here I could show you the ropes.",
                onEnter = () =>
                {
                   ForceDiceRoll.Instance?.StartForcedSequence();
                   SetupForcedHand();
                   ForceInput.Instance?.RequireSelectionCount(5);
                    itemManager.discardsLocked = true;
                }
            },

            new TutorialStep
            {
                message = "This is your arsenal of weapons.\n\nClicking on an attack adds it to your loadout.",
                highlight = attackHandHighlight,
            },

            new TutorialStep
            {
                message = "Each weapon has a damage value which is how much health the attack deals to the enemy.",
                highlight = dmgHandHighlight,
            },

            new TutorialStep
            {
                message = "They also have a roll value which must be rolled on a 20-sided die for the attack to succeed.",
                highlight = rollHandHighlight,
            },

            new TutorialStep
            {
                message = "Try clicking on a weapon.",
                highlight = attackHighlight,
                waitForAction = true,
                onEnter = () => weaponClicked = false
            },

             new TutorialStep
            {
                message = "Hover over an enemy to see their name and weakness.\n\nThe first enemy in this lineup is weak to Piercing attacks!",
                highlight = enemyHighlight,
            },

              new TutorialStep
            {
                 message = "You have 4 Ray of Frosts here, which deal Magical damage. These won't be as effective against Kobolds who are weak to Piercing.\nTry discarding them for something better.",
                 highlight = discardButtonHighlight,
                 waitForAction = true,
                 onEnter = () =>
            {
                 itemManager.ClearItems();
                 itemManager.tutorialDiscardMode = true;
                 itemManager.noAttacking = true;
                 itemManager.discardsLocked = false;
                 ForceInput.Instance?.RequireSelectionCount(4);
            }
            },

               new TutorialStep
            {
                message = "Now your hand is full of Daggers!\nWhen you select attacks, you can press Clear at any time to remove them from your loadout.",
                highlight = clearButtonHighlight,
                onEnter = () =>
                {
                    ForceInput.Instance?.RequireSelectionCount(5);
                    itemManager.discardsLocked = true;
                }
            },

            new TutorialStep
            {
                message = "You can select up to 4 attacks.\n\nTheir attack order depends on the order you click on them. Which is indicated by the overlayed number.",
                highlight = attackHighlight,
            },


             new TutorialStep
            {
                message = "Select 4 Daggers and press Attack!",
                waitForAction = true,
                onEnter = () =>
                {
                    ForceInput.Instance?.RequireSelectionCount(4);
                    itemManager.noAttacking = false;
                    attackPressed = false;
                },
                onActionComplete = () =>
                {
                    discardButtonHighlight.SetActive(false);
                    clearButtonHighlight.SetActive(false);
                    itemManager.blockNormalDraw = false;
                    itemManager.discardsLocked = false;
                }
            },

            new TutorialStep
            {
                message = "Rolling a 20 makes your attack a <b><color=yellow>CRITICAL HIT </b><color=black>which does double damage.\nRolling a 1 makes your attack a <b><color=red>CRITICAL MISS </b><color=black>which always fails no matter what.",
                onEnter = () =>
                {
                    itemManager.noAttacking = true;
                },
    },

              new TutorialStep
            {
                message = "Hitting a weakness makes your attacks deal <b>50%</b> more damage.\nMatching weapon types to enemy weaknesses is the key to survival!",
            },

              new TutorialStep
            {
                message = "You can click the backpack at any time to check which items you've used this day and which are still in your active inventory.",
                highlight = inventoryButtonHighlight,
            },

              new TutorialStep
            {
                message = "Once you've run out of items to refill your arsenal, your used items will reshuffle into your active inventory.",
            },

            new TutorialStep
            {
                message = "This is your Emblem sash. Emblems are permanent upgrades obtained for a run that boost your attacks and rolls!",
                highlight = upgradeBarHighlight,
            },

            new TutorialStep
            {
                message = "And this is your Potion bar. Potions are one-time use items that can give temporary boosts, strengthen your deck, or deal damage to enemies!",
                highlight = consumableBarHighlight,
            },

            new TutorialStep
            {
             message = "Now complete the encounter! Show my kin that Pimp the Imp <i>IS</i> a great teacher!",
             onEnter = () =>
             {

             },
             onContinue = () =>
             {
                 itemManager.noAttacking = false;
                 itemManager.discardsLocked = false;
                 itemManager.blockNormalDraw = false;
                 ForceInput.Instance?.ClearRequirements();
             }
            },
            new TutorialStep
            {
             message = "",
            },
        };

        TutorialManager.Instance.ShowTutorial(steps, OnTutorialComplete);
    }

    private void OnScoringStarted(RoundStartedEvent e) // scoring started
    {
        attackPressed = true;

        TutorialManager.Instance.HidePanel();

        //if (TutorialManager.Instance.IsWaitingForAction)
        //    TutorialManager.Instance.ActionComplete();
    }

    private void OnRoundComplete(ScoringCompletedEvent e) // round completed so we show the panel again
    {
        TutorialManager.Instance.ShowPanel();

        if (TutorialManager.Instance.IsWaitingForAction)
            TutorialManager.Instance.ActionComplete();
    }

    private void OnTutorialComplete()
    {
        itemManager.noAttacking = false;
        itemManager.discardsLocked = false;
        itemManager.blockNormalDraw = false;
        ForceInput.Instance?.ClearRequirements();
        TutorialManager.Instance.HidePanel();
    }


}
