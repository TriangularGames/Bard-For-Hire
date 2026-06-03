using UnityEngine;

public class Die : MonoBehaviour
{
    public void PassAway()
    {
        transform.parent.GetComponent<EnemyController>().RemoveEnemy();
    }
}
