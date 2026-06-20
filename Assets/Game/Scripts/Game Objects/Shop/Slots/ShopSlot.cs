using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class ShopSlot : MonoBehaviour
{
    [SerializeField] public TMP_Text value;
    [SerializeField] public Image icon;
    [SerializeField] public Button buy;

    [HideInInspector] public bool _Purchased = false;

    private void Start()
    {
        buy.onClick.AddListener(Purchase);
        _Purchased = false;
    }

    public abstract void Purchase();

    public virtual void ClearInfo()
    {
        value.text = "";
        icon.sprite = null;
        icon.color = new Color(1f, 1f, 1f, 0f);
        buy.gameObject.SetActive(false);
    }
}
