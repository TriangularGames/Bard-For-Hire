using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public TMP_Text headerTxt;
    public Transform data;
    public TMP_Text contentTxt;

    public LayoutElement layoutElement;

    public int characterWrapLimit;

    public void SetText(string content, string header = "", string attack = "", string roll = "")
    {
        if (string.IsNullOrEmpty(header))
        {
            transform.GetChild(0).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(0).gameObject.SetActive(true);
            headerTxt.text = header;
        }

        if (string.IsNullOrEmpty(attack) || string.IsNullOrEmpty(roll))
        {
            transform.GetChild(1).gameObject.SetActive(false);
        }
        else
        {
            transform.GetChild(1).gameObject.SetActive(true);
            data.GetChild(0).GetComponent<TMP_Text>().text = "Attack: " + attack;
            data.GetChild(1).GetComponent<TMP_Text>().text = "Roll: " + roll;
        }

        contentTxt.text = content;

        int headerLength = headerTxt.text.Length;
        int contentLength = contentTxt.text.Length;

        layoutElement.enabled = (headerLength > characterWrapLimit || contentLength > characterWrapLimit) ? true : false;
    }

    private void Update()
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
