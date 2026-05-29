using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TMP_Text headerTxt;
    public TMP_Text rarityTxt;
    public TMP_Text damageTxt;
    public TMP_Text rollTxt;
    public TMP_Text typeTxt;
    public TMP_Text contentTxt;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    public void SetText(string content, string header, string rarity = "", string type = "", bool isWeakness = false, string damage = "", string roll = "")
    {
        // Header Text
        headerTxt.text = header;

        // Rarity Text
        if (string.IsNullOrEmpty(rarity))
        {
            transform.GetChild(0).GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).GetChild(1).gameObject.SetActive(true);
            rarityTxt.text = rarity;
            
        }

        // Type Text
        if (string.IsNullOrEmpty(type))
        {
            transform.GetChild(2).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(2).gameObject.SetActive(true);
            switch (type)
            {
                case "Magical":
                    typeTxt.color = Color.purple;
                    break;
                case "Piercing":
                    typeTxt.color = Color.darkCyan;
                    break;
                case "Slashing":
                    typeTxt.color = Color.darkGreen;
                    break;
            }
            typeTxt.text = type;
        }

        // Damage / Roll OR Weakness
        if (string.IsNullOrEmpty(damage))
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            // Attack/Weakness Text
            transform.GetChild(1).gameObject.SetActive(true);
            if (isWeakness)
            {
                switch (damage)
                {
                    case "Magical":
                        damageTxt.color = Color.purple;
                        break;
                    case "Piercing":
                        damageTxt.color = Color.darkCyan;
                        break;
                    case "Slashing":
                        damageTxt.color = Color.darkGreen;
                        break;
                }
                damageTxt.text = "Weakness: " + damage;
            }
            else
            {
                damageTxt.color = Color.darkRed;
                damageTxt.text = "Damage: " + damage;
            }

            if (string.IsNullOrEmpty(roll))
            {
                transform.GetChild(1).GetChild(1).gameObject.SetActive(false);
            }
            else
            {
                transform.GetChild(1).GetChild(1).gameObject.SetActive(true);
                rollTxt.text = "Roll: " + roll;
            }
        }

        contentTxt.text = content;

        int headerLength = headerTxt.text.Length;
        int contentLength = contentTxt.text.Length;

        layoutElement.enabled = headerLength > characterWrapLimit || contentLength > characterWrapLimit;
    }

    private void Update()
    {
        Pivot();
    }

    public void Pivot()
    {
        Vector2 position = InputManager.Instance.inputActions.UI.Point.ReadValue<Vector2>();
        var normalizedPosition = new Vector2(position.x / Screen.width, position.y / Screen.height);
        var pivot = CalculatePivot(normalizedPosition);
        GetComponent<RectTransform>().pivot = pivot;
        transform.position = position;
    }

    private Vector2 CalculatePivot(Vector2 normalizedPosition)
    {
        var pivotTopLeft = new Vector2(-0.05f, 1.05f);
        var pivotTopRight = new Vector2(1.05f, 1.05f);
        var pivotBottomLeft = new Vector2(-0.05f, -0.05f);
        var pivotBottomRight = new Vector2(1.05f, -0.05f);

        if (normalizedPosition.x < 0.5f && normalizedPosition.y >= 0.5f)
        {
            return pivotTopLeft;
        }
        else if (normalizedPosition.x > 0.5f && normalizedPosition.y >= 0.5f)
        {
            return pivotTopRight;
        }
        else if (normalizedPosition.x <= 0.5f && normalizedPosition.y < 0.5f)
        {
            return pivotBottomLeft;
        }
        else
        {
            return pivotBottomRight;
        }
    }
}
