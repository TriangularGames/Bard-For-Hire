using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Data")]
    [SerializeField] public EnemyData enemyData;

    [Header("Health/Damage Related")]
    private Color flashColor = new Color(1f, 1f, 1f, 0.5f);
    [SerializeField] private float delayTime = 0.1f;
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

    [Header("Flash Flags")]
    private bool Weak = false;
    private bool Resist = false;

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

    public void SetScaledHealth(int health)
    {
        scaledHealth = health;
    }

    public virtual void Setup()
    {
        int startingHealth = scaledHealth > 0 ? scaledHealth : enemyData.health;
        health = startingHealth;
        SetSprite();
        SetAnimation();
        SetDamageTxt();
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

    protected virtual void TakeDamage(DamageTakenEvent e)
    {
        if (e.id == gameObject.GetEntityId())
        {
            flashTimes = e.damage;
            AudioManager.Instance.PlayClip("Hit");

            Weak = e.weakness;
            Resist = e.resistance;
            
            StartCoroutine("Flash");
        }
    }

    private IEnumerator Flash()
    {
        // Set flashColor based on if Weak, Resist, or Normal
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

        for (int i = 0; i < flashTimes; i++)
        {
            EnemySprite.material.color = Color.white;
            yield return new WaitForSeconds(delayTime);

            // If this is the first instance
            if (i == 0)
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
            }

            // Spawn Damage Text via ObjectPool
            GameObject text = dmgTxtPool.GetObject();
            if (text.transform.parent != transform)
            {
                text.transform.SetParent(transform);
            }
            text.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            text.GetComponent<DestroyText>().Setup(dmgTxtPool);
            text.GetComponent<TMP_Text>().color = flashColor;

            health -= 1;
            SetDamageTxt();

            if (health <= 0)
            {
                Debug.Log("Enemy killed.");
                EventBus.Publish(new EnemyDefeatedEvent(gameObject));
            }
            else
            {
                anim.SetTrigger("Hit");
            }

            EnemySprite.material.color = flashColor;
            yield return new WaitForSeconds(delayTime);
        }
        EnemySprite.material.color = Color.white;
        yield return null;
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
