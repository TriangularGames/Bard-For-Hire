using NUnit.Framework.Interfaces;
using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public EnemyData enemyData;

    private Color flashColor = new Color(1f,1f,1f,0.5f);
    [SerializeField] private float delayTime = 0.1f;
    private int flashTimes = 0;

    private int health;
    [SerializeField] private TMP_Text healthTxt;
    // TODO: use ResourceManager
    [SerializeField] private GameObject DmgTxtPrefab;

    public int GetHealth() {  return health; }

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
        SetDamageTxt();
    }

    private void SetSprite()
    {
        GetComponent<SpriteRenderer>().sprite = enemyData.icon;
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
            GetComponent<SpriteRenderer>().material.color = Color.white;
            yield return new WaitForSeconds(delayTime);

            // TODO: object pool or change to particle effect perhaps?
            var txt = Instantiate(DmgTxtPrefab, transform.position, Quaternion.identity, transform);
            txt.GetComponent<TMP_Text>().text = "-1";
            health -= 1;
            SetDamageTxt();
            if (health <= 0)
            {
                Debug.Log("Enemy killed.");
                EventBus.Publish(new EnemyDefeatedEvent(gameObject));
            }

            GetComponent<SpriteRenderer>().material.color = flashColor;
            yield return new WaitForSeconds(delayTime);
        }
        GetComponent<SpriteRenderer>().material.color = Color.white;
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
