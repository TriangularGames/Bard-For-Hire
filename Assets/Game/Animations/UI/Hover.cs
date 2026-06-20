using UnityEngine;

public class Hover : MonoBehaviour
{
    private RectTransform rectTransform;

    [Tooltip("Movement Speed that also indicates direction")]
    [SerializeField] private Vector2 movementSpeed;

    [Tooltip("Upper Boundary of the Hover- the Highest point")]
    [SerializeField] private Vector2 UpperBoundary;

    [Tooltip("Lower Boundary of the Hover- the Lowest point")]
    [SerializeField] private Vector2 LowerBoundary;

    private bool SwapDirections = false;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        SwapDirections = false;
    }

    void Update()
    {
        if (movementSpeed.x != 0 && movementSpeed.y != 0)
        {
            if (rectTransform.anchoredPosition.x <= UpperBoundary.x &&
                rectTransform.anchoredPosition.y <= UpperBoundary.y)
            {
                SwapDirections = true;
            }
            if (rectTransform.anchoredPosition.x <= LowerBoundary.x &&
                rectTransform.anchoredPosition.y <= LowerBoundary.y)
            {
                SwapDirections = false;
            }

            if (!SwapDirections)
            {
                rectTransform.anchoredPosition += new Vector2(movementSpeed.x,0f);
                rectTransform.anchoredPosition -= new Vector2(0f,movementSpeed.y);
            }
            else
            {
                rectTransform.anchoredPosition -= new Vector2(movementSpeed.x, 0f);
                rectTransform.anchoredPosition += new Vector2(0f, movementSpeed.y);
            }
        }
        else
        {
            if (movementSpeed.x != 0)
            {
                if (rectTransform.anchoredPosition.x >= UpperBoundary.x)
                {
                    SwapDirections = true;
                }
                if (rectTransform.anchoredPosition.x <= LowerBoundary.x)
                {
                    SwapDirections = false;
                }
            }

            if (movementSpeed.y != 0)
            {
                if (rectTransform.anchoredPosition.y >= UpperBoundary.y)
                {
                    SwapDirections = true;
                }
                if (rectTransform.anchoredPosition.y <= LowerBoundary.y)
                {
                    SwapDirections = false;
                }
            }

            if (!SwapDirections)
            {
                rectTransform.anchoredPosition += movementSpeed;
            }
            else
            {
                rectTransform.anchoredPosition -= movementSpeed;
            }
        }

        
    }
}
