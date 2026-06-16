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
    [SerializeField] private SpriteRenderer healthIcon;
    [SerializeField] private Sprite selectedIndicator;
    [SerializeField] private ParticleSystem hit;
    [SerializeField] private ParticleSystem smoke;

    [Header("Flash Flags")]
    private bool Weak = false;
    private bool Resist = false;

    private float delayTimer;
    private int flashIndex = 0;
    private bool FadeHealth = false;
    [SerializeField] private float fadeDuration = 0.3f;

    public int GetHealth() { return health; }

    public float GetDelayTime() {  return delayTime; }

    public void SetIndicator() { healthIcon.sprite = selectedIndicator; }

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

        // If enemy has a ypos value (meaning it needs to be raised)
        if (enemyData.yPos != 0)
        {
            // Change its ypos
            transform.localPosition = new Vector3(0f, enemyData.yPos, 0f);

            // if the ypos is greater than 0.1 (because it starts conflicting with the UI)
            if (0.1f < enemyData.yPos && enemyData.yPos <= 0.2f)
            {
                healthIcon.gameObject.transform.localPosition = new Vector3(healthIcon.gameObject.transform.localPosition.x,
                    healthIcon.gameObject.transform.localPosition.y + 0.3f,
                    healthIcon.gameObject.transform.localPosition.z);

                healthTxt.gameObject.transform.localPosition = new Vector3(healthTxt.gameObject.transform.localPosition.x,
                    healthTxt.gameObject.transform.localPosition.y + 0.4f,
                    healthTxt.gameObject.transform.localPosition.z);
            }
        }
    }

    protected void SetSprite()
    {
        EnemySprite.sprite = enemyData.icon;
    }

    protected void SetAnimation()
    {
        anim.runtimeAnimatorController = enemyData.animator;
        anim.SetFloat("IdleOffset", Random.Range(0.0f, 4.0f));
        anim.speed = Random.Range(0.5f, 1.0f);
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
        if (!Weak && !Resist || Weak && Resist)
        {
            flashColor = Color.red;
        }
    }

    private void DisplayWeakResistText()
    {
        // If both are active, don't display text
        if (Weak && Resist) return;

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
            // TODO: make sure this sound is ok (seems like it but I'm unsure)
            AudioManager.Instance.PlayClip("Weak");
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
            // TODO: make sure this sound is ok (seems like it but I'm unsure)
            AudioManager.Instance.PlayClip("Swipe");
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
            if (e.dmgType != null)
            {
                switch (e.dmgType)
                {
                    case "Piercing":
                        AudioManager.Instance.PlayClip("Pierce");
                        break;

                    case "Slashing":
                        AudioManager.Instance.PlayClip("Slash");
                        break;

                    case "Magical":
                        AudioManager.Instance.PlayClip("Magic");
                        break;
                }
            }
            //AudioManager.Instance.PlayClip("Hit");

            Weak = e.weakness;
            Resist = e.resistance;

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

        if (FadeHealth)
        {
            FadeAway();
            FadeHealth = false;
        }
    }

    private void FadeAway()
    {
        healthTxt.color = new Color(healthTxt.color.r, healthTxt.color.g, healthTxt.color.b, Mathf.Lerp(1, 0, fadeDuration));
        healthIcon.color = new Color(healthIcon.color.r, healthIcon.color.g, healthIcon.color.b, Mathf.Lerp(1, 0, fadeDuration));
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
                health = Mathf.Max(health, 0);
                SetDamageTxt();

                // If the enemy is dead or not
                if (health <= 0)
                {
                    health = 0;
                    state = ENEMY_STATE.DEAD;
                    return;
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
        if (enemyData.yPos > 0)
        {
            transform.localPosition = new Vector3(transform.localPosition.x, 0.1f, transform.localPosition.z);
        }
        anim.speed = 1.0f;
        anim.SetTrigger("Dead");
    }

    public void Smoke()
    {
        smoke.Play();
        AudioManager.Instance.PlayClip("Poof");
        FadeHealth = true;
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
    public string dmgType;
    public EntityId id;
    public bool weakness;
    public bool resistance;

    public DamageTakenEvent(int _id, int _damage, string _dmgType, bool _weakness, bool _resistance)
    {
        id = _id;
        damage = _damage;
        dmgType = _dmgType;
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
