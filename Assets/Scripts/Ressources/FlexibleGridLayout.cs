using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlexibleGridLayout : LayoutGroup
{
    public enum FitType{
        Uniform, Width, Height, FixedRows, FixedColumns
    }
    public int rows;
    public int columns;
    public Vector2 cellSize;
    public Vector2 spacing;
    public FitType fitType;

    public bool fitX;
    public bool fitY;
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();

        if (this.fitType == FitType.Uniform || this.fitType == FitType.Height || this.fitType == FitType.Width){
            this.fitX = true;
            this.fitY = true;
            float sqrt = Mathf.Sqrt(transform.childCount);
            rows = Mathf.CeilToInt(sqrt);
            columns = Mathf.CeilToInt(sqrt);
        }

        if (this.fitType == FitType.Width || this.fitType == FitType.FixedColumns){
            rows = Mathf.CeilToInt(transform.childCount / (float) columns);
        }
        else if (this.fitType == FitType.Height || this.fitType == FitType.FixedRows){
            columns = Mathf.CeilToInt(transform.childCount / (float) rows);
        }

        float parentWidth = rectTransform.rect.width;
        float parentHeight = rectTransform.rect.height;
        float cellWidth = (parentWidth / (float) columns) - ((spacing.x / (float) columns) * 2) - (padding.left / (float) columns) - (padding.right / (float) columns);
        float cellHeight = (parentHeight / (float) rows) - ((spacing.y / (float) rows) * 2) - (padding.top / (float) columns) - (padding.bottom / (float) columns);

        cellSize.x = fitX ? cellWidth : cellSize.x;
        cellSize.y = fitY ? cellHeight : cellSize.y;

        int columnCount = 0;
        int rowCount = 0;

        for (int i = 0; i < rectChildren.Count; i++){
            rowCount = i / columns;
            columnCount = i % rows;

            var item = rectChildren[i];

            var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
            var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

            SetChildAlongAxis(item, 0, xPos, cellSize.x);
            SetChildAlongAxis(item, 1, yPos, cellSize.y);

        }
    }
    public override void CalculateLayoutInputVertical()
    {
        throw new System.NotImplementedException();
    }

    public override void SetLayoutHorizontal()
    {
        throw new System.NotImplementedException();
    }

    public override void SetLayoutVertical()
    {
        throw new System.NotImplementedException();
    }
}
