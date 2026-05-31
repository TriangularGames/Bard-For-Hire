using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public EnemyData enemyData;

    private Color flashColor = new Color(1f,1f,1f,0.5f);
    [SerializeField] private float delayTime = 0.1f;
    private int flashTimes = 0;
    private int scaledHealth = -1;
    private int health;
    [SerializeField] private TMP_Text healthTxt;
    [SerializeField] private ObjectPool dmgTxtPool;

    [SerializeField] private SpriteRenderer EnemySprite;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject indicator;

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

    public void Setup()
    {
        int startingHealth = scaledHealth > 0 ? scaledHealth : enemyData.health;
        health = startingHealth;
        SetSprite();
        SetAnimation();
        SetDamageTxt();
    }

    private void SetSprite()
    {
        EnemySprite.sprite = enemyData.icon;
    }

    private void SetAnimation()
    {
        anim.runtimeAnimatorController = enemyData.animator;
    }

    private void SetDamageTxt()
    {
        healthTxt.text = health.ToString();
    }

    private void TakeDamage(DamageTakenEvent e)
    {
        if (e.id == gameObject.GetEntityId())
        {
            flashTimes = e.damage;
            AudioManager.Instance.PlayClip("Hit");
            if (e.weakness)
            {
                StartCoroutine("WeakFlash");
            }
            else if (e.resistance)
            {
                StartCoroutine("ResistFlash");
            }
            else
            {
                StartCoroutine("Flash");
            }
        }
    }

    private IEnumerator Flash()
    {
        for (int i = 0; i < flashTimes; i++)
        {
            EnemySprite.material.color = Color.white;
            yield return new WaitForSeconds(delayTime);

            // Spawn Damage Text via ObjectPool
            GameObject text = dmgTxtPool.GetObject();
            if (text.transform.parent != transform)
            {
                text.transform.SetParent(transform);
            }
            text.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            text.GetComponent<DestroyText>().Setup(dmgTxtPool);

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

    // Flash but wth ORANGE text and flash
    private IEnumerator WeakFlash()
    {
        for (int i = 0; i < flashTimes; i++)
        {
            EnemySprite.material.color = Color.white;
            yield return new WaitForSeconds(delayTime);

            // If this is the first instance, showcase this damage is due to Weakness
            if (i == 0)
            {
                // TODO: adjust position of spawned text
                var resistTxt = Instantiate(AssetManager.Instance.GetPrefab("DmgTxt"), transform.position, Quaternion.identity, transform);
                resistTxt.GetComponent<TMP_Text>().color = Color.orange;
                resistTxt.GetComponent<TMP_Text>().text = "Weak";
            }

            // Spawn Damage Text via ObjectPool
            var text = dmgTxtPool.GetObject();
            text.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            text.GetComponent<DestroyText>().Setup(dmgTxtPool);

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

            EnemySprite.material.color = Color.orange;
            yield return new WaitForSeconds(delayTime);
        }
        EnemySprite.material.color = Color.white;
        yield return null;
    }

    private IEnumerator ResistFlash()
    {
        for (int i = 0; i < flashTimes; i++)
        {
            EnemySprite.material.color = Color.white;
            yield return new WaitForSeconds(delayTime);

            // If this is the first instance, showcase this is being Resisted
            if (i == 0)
            {
                // TODO: adjust position of spawned text
                var resistTxt = Instantiate(AssetManager.Instance.GetPrefab("DmgTxt"), transform.position, Quaternion.identity, transform);
                resistTxt.GetComponent<TMP_Text>().color = Color.grey;
                resistTxt.GetComponent<TMP_Text>().text = "Resist";
            }

            // Spawn Damage Text via ObjectPool
            var text = dmgTxtPool.GetObject();
            text.transform.SetPositionAndRotation(transform.position, Quaternion.identity);
            text.GetComponent<DestroyText>().Setup(dmgTxtPool);

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

            EnemySprite.material.color = Color.grey;
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
