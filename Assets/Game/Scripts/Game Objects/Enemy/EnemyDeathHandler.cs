using UnityEngine;

public class EnemyDeathHandler : MonoBehaviour
{
    public void Smoke()
    {
        transform.parent.GetComponent<EnemyController>().Smoke();
    }
    public void PassAway()
    {
        transform.parent.GetComponent<EnemyController>().RemoveEnemy();
    }
}
