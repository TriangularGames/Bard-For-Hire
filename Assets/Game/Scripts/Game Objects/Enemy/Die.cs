using UnityEngine;

public class Die : MonoBehaviour
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
