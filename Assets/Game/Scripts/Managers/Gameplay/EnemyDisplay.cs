using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyDisplay : MonoBehaviour
{
    public Image enemyImage;
    public TMP_Text health;

    public void Setup(EnemyData enemy, int scaledHP = -1)
    {
        int displayHealth = scaledHP > 0 ? scaledHP : enemy.health;
        health.text = displayHealth.ToString();
        enemyImage.sprite = enemy.icon;
        enemyImage.gameObject.GetComponent<EnemyInfo>().enemyData = enemy;

        health.text = displayHealth.ToString() + " hp";
    }
}
