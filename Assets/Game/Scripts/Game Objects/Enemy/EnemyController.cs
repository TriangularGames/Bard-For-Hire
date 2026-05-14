using NUnit.Framework.Interfaces;
using System;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    [SerializeField] public EnemyData enemyData;

    private int health;
    [SerializeField] private TMP_Text healthTxt;

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
            health -= e.damage;
            SetDamageTxt();
            if (health <= 0)
            {
                Debug.Log("Enemy killed.");
                //GameObject.FindWithTag("EnemyManager").GetComponent<EnemyManager>().enemies
                Destroy(gameObject);
            }
        }
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
