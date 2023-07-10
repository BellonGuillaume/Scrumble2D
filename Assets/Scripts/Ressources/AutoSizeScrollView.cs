using UnityEngine;
using UnityEngine.UI;

public class AutoSizeScrollView : MonoBehaviour
{
    public ScrollRect scrollRect;
    public RectTransform content;
    public float padding = 10f;
    public int numberOfObjectsAlreadyShowed;

    FlexibleGridLayout flex;

    private void Start()
    {
        flex = content.GetComponent<FlexibleGridLayout>();
        ResizeContent();
    }

    private void Update()
    {
        ResizeContent();
    }

    private void ResizeContent()
    {
        if (content.childCount <= numberOfObjectsAlreadyShowed){
            content.sizeDelta = scrollRect.viewport.rect.size;
        } else {
            float contentHeight = CalculateContentHeight();
            Vector2 contentSize = new Vector2(content.sizeDelta.x, contentHeight + padding);
            content.sizeDelta = contentSize;
        }
    }

    private float CalculateContentHeight()
    {
        int countCase = 0;
        int countSpace = 0;
        float totalHeight = 0f;
        float spacing = 0f;
        int valueToBegin = this.numberOfObjectsAlreadyShowed / flex.columns;
        valueToBegin = (valueToBegin - 1) * flex.columns;
        for (int i = 0; i < content.childCount; i++)
        {
            if (i % flex.columns == 0){
                countCase++;
                countSpace++;
                RectTransform child = content.GetChild(i) as RectTransform;
                totalHeight += child.sizeDelta.y;
                spacing += flex.spacing.y;
            }
        }
        spacing += flex.spacing.y;
        return totalHeight + spacing;
    }
}
