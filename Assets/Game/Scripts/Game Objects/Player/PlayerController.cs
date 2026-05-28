using System;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Animator anim;

    private void OnEnable()
    {
        EventBus.Subscribe<HitEvent>(Attack);
        EventBus.Subscribe<MissEvent>(Miss);
    }

    private void OnDisable()
    {
        EventBus.Unsubscribe<HitEvent>(Attack);
        EventBus.Unsubscribe<MissEvent>(Miss);
    }

    private void Attack(HitEvent e)
    {
        anim.SetTrigger("Attack");
    }

    private void Miss(MissEvent @event)
    {
        anim.SetTrigger("Miss");
    }

}
