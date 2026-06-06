using UnityEngine;

public class MimicController : EnemyController
{
    [Header("Disguised EnemyData")]
    [SerializeField] public EnemyData mimicData;

    private int startingHealth = 0;

    public override void Setup()
    {
        // Set health as normal
        startingHealth = scaledHealth > 0 ? scaledHealth : enemyData.health;
        health = startingHealth;
        SetDamageTxt();

        // Set display based on MimicData
        EnemySprite.sprite = mimicData.icon;
        anim.runtimeAnimatorController = mimicData.animator;
    }

    protected override void TakeDamage(DamageTakenEvent e)
    {
        if (e.id == gameObject.GetEntityId())
        {
            // If this is the first time being hit
            if (health == startingHealth)
            {
                // Set to Mimic
                SetSprite();
                SetAnimation();

                // Remove the Disguised Enemy Data
                mimicData = null;
            }
        }

        base.TakeDamage(e);
    }
}
