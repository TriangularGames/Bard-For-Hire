using UnityEngine;
using System.Collections.Generic;
using static ItemManager;

public class CombatTutorial: MonoBehaviour
{
    [Header("Highlight Targets")] // Different highlight targets for the zones to emphasize
    [SerializeField] private GameObject attackHandHighlight;
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
        if (!TutorialManager.Instance.WeShowTutorial()) return;

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
                message = "Haha adventurer you fell for my trap. You're stuck in this dungeon now. \nWell uh I guess while you're here I could show you the ropes.",
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
                message = "This is your arsenal of weapons. \n\nClicking on an attack adds it to your loadout.",
                highlight = attackHandHighlight,
            },

            new TutorialStep
            {
                message = "Each weapon has a damage value which is how much it deals on a hit, and a roll number which is the minimum you must roll on the 20-sided dice for the attack to go through.",
                highlight = attackHandHighlight,
            },

            new TutorialStep
            {
                message = "Try clicking on a weapon.",
                highlight = attackHandHighlight,
                waitForAction = true,
                onEnter = () => weaponClicked = false
            },

             new TutorialStep
            {
                message = "Hover over an enemy to see what type they are and what they're weak to. \n\nThese enemies are weak to piercing attacks!",
                highlight = attackHandHighlight,
            },

              new TutorialStep
            {
                 message = "You have 4 ray of frosts here, which deal magic damage won't be as effective against these kobolds who are weak to piercing. \nTry discarding them for something better.",
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
                message = "Now your hand is full of Daggers! \nYou can also press Clear at any time to deselect everything in your current loadout without losing any weapons.",
                highlight = clearButtonHighlight,
                onEnter = () =>
                {
                    ForceInput.Instance?.RequireSelectionCount(5);
                    itemManager.discardsLocked = true;
                }
            },

            new TutorialStep
            {
                message = "You can select up to 4 attacks. \n\nThe order they are played depends on the order you click them. Which is indicated by the number on top.",
                highlight = attackHandHighlight,
            },


             new TutorialStep
            {
                message = "Select 4 Daggers and press Attack!",
                highlight = attackHandHighlight,
                waitForAction = true,
                onEnter = () =>
                {
                    ForceInput.Instance?.RequireSelectionCount(4);
                    itemManager.noAttacking = false;
                    attackPressed = false;
                },
                onActionComplete = () =>
                {
                    attackHandHighlight.SetActive(false);
                    discardButtonHighlight.SetActive(false);
                    clearButtonHighlight.SetActive(false);
                    itemManager.blockNormalDraw = false;
                    itemManager.discardsLocked = false;
                }
            },

            new TutorialStep
            {
                message = "Rolling a 20 makes your attack a  <color=yellow>CRITICAL HIT  <color=white>which does double damage. \nRolling a 1 makes you attack a  <color=red>CRITICAL MISS  <color=black>which always fails no matter what.",
    },

              new TutorialStep
            {
                message = "Daggers deal 50% more damage against these enemies because of their weakness. \nMatching weapon types to enemy weaknesses is the key to survival",
            },

              new TutorialStep
            {
                message = "The bag will show the items in your inventory and the items you've used so that you can gauge future prospects while rerolling",
                highlight = inventoryButtonHighlight,
            },

            new TutorialStep
            {
                message = "This is your upgrade bar. It's your permanent power-ups that boost your attacks and rolls.",
                highlight = upgradeBarHighlight,
            },

            new TutorialStep
            {
                message = "And this is your consumable bar. It's your one time use items that can be used to save you in a pinch.",
                highlight = consumableBarHighlight,
            },

            new TutorialStep
            {
                message = "Now complete the encounter!",
                waitForAction = true,
                onEnter = () =>
                {
                    ForceInput.Instance?.ClearRequirements();
                }
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
        ForceInput.Instance?.ClearRequirements();
        TutorialManager.Instance.HidePanel();
    }


}
