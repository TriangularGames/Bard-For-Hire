using TMPro;
using UnityEngine;

enum ENEMY_STATE
{
    IDLE,
    DAMAGED,
    DEAD,
    DYING
};

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] public EnemyData enemyData;
    private ENEMY_STATE state = ENEMY_STATE.IDLE;

    [Header("Health/Damage Related")]
    private Color flashColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private float delayTime = 0.5f;
    private int flashTimes = 0;
    protected int scaledHealth = -1;
    protected int health;
    [SerializeField] private TMP_Text healthTxt;
    [SerializeField] private ObjectPool dmgTxtPool;
    [SerializeField] private ObjectPool dmgDisplayPool;

    [Header("Enemy Display")]
    [SerializeField] protected SpriteRenderer EnemySprite;
    [SerializeField] protected Animator anim;
    [SerializeField] private GameObject indicator;
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private ParticleSystem smoke;

    [Header("Flash Flags")]
    private bool Weak = false;
    private bool Resist = false;

    private float delayTimer;
    private int flashIndex = 0;

    public int GetHealth() {  return health; }

    public void SetIndicator() { indicator.SetActive(!indicator.activeSelf); }

    private void OnEnable()
    {
        EventBus.Subscribe<DamageTakenEvent>(TakeDamage);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<DamageTakenEvent>(TakeDamage);
    }

    #region Setup Funcs
    public virtual void Setup()
    {
        int startingHealth = scaledHealth > 0 ? scaledHealth : enemyData.health;
        health = startingHealth;
        SetSprite();
        SetAnimation();
        SetDamageTxt();
        smoke = transform.parent.GetChild(0).GetComponent<ParticleSystem>();
    }

    protected void SetSprite()
    {
        EnemySprite.sprite = enemyData.icon;
    }

    protected void SetAnimation()
    {
        anim.runtimeAnimatorController = enemyData.animator;
    }

    protected void SetDamageTxt()
    {
        healthTxt.text = health.ToString();
    }

    public void SetScaledHealth(int health)
    {
        scaledHealth = health;
    }
    #endregion

    #region State Funcs
    private void SetFlashColor()
    {
        // Set flashColor
        if (Weak)
        {
            flashColor = Color.orange;
        }
        if (Resist)
        {
            flashColor = Color.gray;
        }
        if (!Weak && !Resist)
        {
            flashColor = Color.red;
        }
    }

    private void DisplayWeakResistText()
    {
        // Check if damage taken is a Weakness or a Resistance
        if (Weak)
        {
            GameObject weak = dmgDisplayPool.GetObject();
            if (weak.transform.parent != transform)
            {
                weak.transform.SetParent(transform);
            }
            weak.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            weak.GetComponent<DestroyText>().Setup(dmgDisplayPool);
            weak.GetComponent<TMP_Text>().color = flashColor;
            weak.GetComponent<TMP_Text>().text = "Weak";
        }
        if (Resist)
        {
            GameObject resist = dmgDisplayPool.GetObject();
            if (resist.transform.parent != transform)
            {
                resist.transform.SetParent(transform);
            }
            resist.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            resist.GetComponent<DestroyText>().Setup(dmgDisplayPool);
            resist.GetComponent<TMP_Text>().color = flashColor;
            resist.GetComponent<TMP_Text>().text = "Resist";
        }
        // If both flags are active
        if (Weak && Resist)
        {
            // TODO: fill in this
            // Do something here
        }
    }

    private void SpawnDamageNum()
    {
        // Spawn Damage Text via ObjectPool
        GameObject text = dmgTxtPool.GetObject();
        if (text.transform.parent != transform)
        {
            text.transform.SetParent(transform);
        }
        text.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
        text.GetComponent<DestroyText>().Setup(dmgTxtPool);
        text.GetComponent<TMP_Text>().color = flashColor;
    }
    #endregion

    protected virtual void TakeDamage(DamageTakenEvent e)
    {
        if (e.id == gameObject.GetEntityId())
        {
            flashTimes = e.damage;
            AudioManager.Instance.PlayClip("Hit");

            Weak = e.weakness;
            Resist = e.resistance;

            //StartCoroutine("Flash");
            state = ENEMY_STATE.DAMAGED;
        }
    }

    private void Update()
    {
        switch (state)
        {
            case ENEMY_STATE.IDLE:
                delayTimer = delayTime;
                flashIndex = 0;
                break;

            case ENEMY_STATE.DAMAGED:
                Flash();
                break;
            case ENEMY_STATE.DEAD:
                Die();
                state = ENEMY_STATE.DYING;
                break;
            case ENEMY_STATE.DYING:
                break;
        }
    }

    private void Flash()
    {
        if (flashIndex < flashTimes)
        {
            SetFlashColor();

            delayTimer -= Time.deltaTime;

            if (delayTimer < 0 || flashIndex == 0)
            {
                if (flashIndex == 0)
                {
                    // On first hit, play the particle effect
                    hit.Play();

                    anim.SetTrigger("Hit");

                    // Check if Weak/Resist text is to be displayed and display
                    DisplayWeakResistText();
                }
                // Spawn the -1 health
                SpawnDamageNum();

                // Take -1 health
                health -= 1;
                SetDamageTxt();

                // If the enemy is dead or not
                if (health <= 0)
                {
                    health = 0;
                    // Enemy dies once flashes are completed
                    if (flashIndex == flashTimes - 1)
                    {
                        state = ENEMY_STATE.DEAD;
                        return;
                    }
                }

                flashIndex++;
                delayTimer = delayTime;
            }
            else
            {
                // set to white
            }
        }
        else
        {
            // set to white
            state = ENEMY_STATE.IDLE;
        }
    }

    private void Die()
    {
        anim.SetTrigger("Dead");
    }

    public void Smoke()
    {
        smoke.Play();
    }

    public void RemoveEnemy()
    {
        EventBus.Publish(new EnemyDefeatedEvent(gameObject));
    }
}

/// <summary>
/// For when an Item is scored for Damage
/// </summary>
public struct DamageTakenEvent
{
    public int damage;
    public EntityId id;
    public bool weakness;
    public bool resistance;

    public DamageTakenEvent(int _id, int _damage, bool _weakness, bool _resistance)
    {
        id = _id;
        damage = _damage;
        weakness = _weakness;
        resistance = _resistance;
    }
}

public struct EnemyDefeatedEvent
{
    public GameObject enemy;

    public EnemyDefeatedEvent(GameObject _enemy)
    {
        enemy = _enemy;
    }
}
