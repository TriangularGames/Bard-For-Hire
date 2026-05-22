using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    public Image enemyImage;
    public TMP_Text health;

    public void Setup(EnemyData enemy)
    {
        enemyImage.sprite = enemy.icon;
        enemyImage.gameObject.GetComponent<EnemyInfo>().enemyData = enemy;

        health.text = enemy.health.ToString();
    }
}
