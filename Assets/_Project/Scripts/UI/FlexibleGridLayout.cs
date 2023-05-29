using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Project
{
    using UnityEngine;
using UnityEngine.UI;

    public class FlexibleGridLayout : LayoutGroup
    {
        public enum FitType
        {
            UNIFORM,
            WIDTH,
            HEIGHT,
            FIXEDROWS,
            FIXEDCOLUMNS
        }

        [Header("Flexible Grid")] public FitType fitType = FitType.UNIFORM;

        [Min(1)] public int rows;
        [Min(1)] public int columns;
        public Vector2 cellSize;
        public Vector2 spacing;

        public bool fitX;
        public bool fitY;
        
        public override void CalculateLayoutInputHorizontal()
        {
            base.CalculateLayoutInputHorizontal();

            if (fitType == FitType.WIDTH || fitType == FitType.HEIGHT || fitType == FitType.UNIFORM)
            {
                float squareRoot = Mathf.Sqrt(transform.childCount);
                rows = columns = Mathf.CeilToInt(squareRoot);
                switch (fitType)
                {
                    case FitType.WIDTH:
                        fitX = true;
                        fitY = false;
                        break;
                    case FitType.HEIGHT:
                        fitX = false;
                        fitY = true;
                        break;
                    case FitType.UNIFORM:
                        fitX = fitY = true;
                        break;
                }
            }

            if (fitType == FitType.WIDTH || fitType == FitType.FIXEDCOLUMNS)
            {
                rows = Mathf.CeilToInt(transform.childCount / (float)columns);
            }

            if (fitType == FitType.HEIGHT || fitType == FitType.FIXEDROWS)
            {
                columns = Mathf.CeilToInt(transform.childCount / (float)rows);
            }


            float parentWidth = rectTransform.rect.width;
            float parentHeight = rectTransform.rect.height;

            float cellWidth = (parentWidth - padding.left - padding.right - spacing.x * (columns - 1)) / (float)columns;
            float cellHeight = (parentHeight - padding.top - padding.bottom - spacing.y * (rows - 1)) / (float)rows;

            cellSize.x = fitX ? cellWidth : cellSize.x;
            cellSize.y = fitY ? cellHeight : cellSize.y;
            
            int columnCount = 0;
            int rowCount = 0;

            for (int i = 0; i < rectChildren.Count; i++)
            {
                rowCount = i / columns;
                columnCount = i % columns;

                var item = rectChildren[i];

                var xPos = (cellSize.x * columnCount) + (spacing.x * columnCount) + padding.left;
                var yPos = (cellSize.y * rowCount) + (spacing.y * rowCount) + padding.top;

                SetChildAlongAxis(item, 0, xPos, cellSize.x);
                SetChildAlongAxis(item, 1, yPos, cellSize.y);

            }
        }

        public override void CalculateLayoutInputVertical()
        {
            //throw new System.NotImplementedException();
        }

        public override void SetLayoutHorizontal()
        {
            //throw new System.NotImplementedException();
        }

        public override void SetLayoutVertical()
        {
            //throw new System.NotImplementedException();
        }
    }
}
