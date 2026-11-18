// GridBackground.cs
using UnityEngine;
using UnityEngine.UIElements;

public class GridBackground : VisualElement
{
    public GridBackground()
    {
        this.pickingMode = PickingMode.Ignore;
        this.generateVisualContent += DrawGrid;
    }

    private void DrawGrid(MeshGenerationContext ctx)
    {
        var painter = ctx.painter2D;
        painter.lineWidth = 1;
        painter.strokeColor = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        
        float spacing = 20f;
        for (float x = 0; x < resolvedStyle.width; x += spacing)
        {
            painter.BeginPath(); painter.MoveTo(new Vector2(x, 0)); painter.LineTo(new Vector2(x, resolvedStyle.height)); painter.Stroke();
        }
        for (float y = 0; y < resolvedStyle.height; y += spacing)
        {
            painter.BeginPath(); painter.MoveTo(new Vector2(0, y)); painter.LineTo(new Vector2(resolvedStyle.width, y)); painter.Stroke();
        }
    }
}