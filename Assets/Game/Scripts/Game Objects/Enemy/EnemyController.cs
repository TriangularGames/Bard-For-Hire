using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public EnemyData enemyData;

    private Color flashColor = new Color(1f,1f,1f,0.5f);
    [SerializeField] private float delayTime = 0.1f;
    private int flashTimes = 0;

    private int health;
    [SerializeField] private TMP_Text healthTxt;

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

    public void Setup()
    {
        health = enemyData.health;
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
            StartCoroutine("Flash");
        }
    }

    private IEnumerator Flash()
    {
        for (int i = 0; i < flashTimes; i++)
        {
            EnemySprite.material.color = Color.white;
            yield return new WaitForSeconds(delayTime);

            // TODO: object pool or change to particle effect perhaps?
            var txt = Instantiate(AssetManager.Instance.GetPrefab("DmgTxt"), transform.position, Quaternion.identity, transform);
            txt.GetComponent<TMP_Text>().text = "-1";
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

    public DamageTakenEvent(int _id, int _damage)
    {
        id = _id;
        damage = _damage;
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
