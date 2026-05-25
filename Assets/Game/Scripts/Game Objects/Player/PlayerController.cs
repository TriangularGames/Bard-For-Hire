using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private void OnEnable()
    {
        EventBus.Subscribe<AttackEvent>(Attack);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<AttackEvent>(Attack);
    }

    private void Attack(AttackEvent e)
    {
        anim.SetTrigger("Attack");
    }

}
