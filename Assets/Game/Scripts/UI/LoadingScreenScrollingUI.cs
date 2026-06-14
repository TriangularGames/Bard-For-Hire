using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScreenScrollingUI : MonoBehaviour
{
    [SerializeField] private GameObject ContainerA;
    [SerializeField] private GameObject ContainerB;
    [SerializeField] private GameObject ContainerC;
    [SerializeField] private GameObject ContainerD;

    [SerializeField] private Vector3 MoveLeft = new Vector3(-0.5f, 0f, 0f);
    [SerializeField] private Vector3 MoveRight = new Vector3(0.5f, 0f, 0f);

    [Tooltip("Boundary for Containers moving Left")]
    [SerializeField] private int LeftBoundary = -479;

    [Tooltip("Boundary for Containers moving Right")]
    [SerializeField] private int RightBoundary = 401;

    private bool swapDirections = false;

    private List<Image> imgObjects;

    private void Start()
    {
        imgObjects = new List<Image>();
        foreach (Transform child in ContainerA.transform)
        {
            imgObjects.Add(child.GetComponent<Image>());
        }
        foreach (Transform child in ContainerB.transform)
        {
            imgObjects.Add(child.GetComponent<Image>());
        }
        foreach (Transform child in ContainerC.transform)
        {
            imgObjects.Add(child.GetComponent<Image>());
        }
        foreach (Transform child in ContainerD.transform)
        {
            imgObjects.Add(child.GetComponent<Image>());
        }

        int randomNum = Random.Range(0, ResourceManager.Instance.ItemData.Length);
        Sprite icon = ResourceManager.Instance.ItemData[randomNum].icon;

        foreach (Image image in imgObjects)
        {
            image.sprite = icon;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ContainerA.transform.localPosition.x <= LeftBoundary)
        {
            swapDirections = true;
        }
        else if (ContainerA.transform.localPosition.x >= RightBoundary)
        {
            swapDirections = false;
        }

        if (!swapDirections)
        {
            ContainerA.transform.localPosition += MoveLeft;
            ContainerB.transform.localPosition += MoveRight;
            ContainerC.transform.localPosition += MoveLeft;
            ContainerD.transform.localPosition += MoveRight;
        }
        else
        {
            ContainerA.transform.localPosition += MoveRight;
            ContainerB.transform.localPosition += MoveLeft;
            ContainerC.transform.localPosition += MoveRight;
            ContainerD.transform.localPosition += MoveLeft;
        }
    }
}
