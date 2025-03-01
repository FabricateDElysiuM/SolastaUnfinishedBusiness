﻿// Copyright < 2021 > Narria (github user Cabarius) - License: MIT

using JetBrains.Annotations;
using UnityEngine;

namespace SolastaUnfinishedBusiness.Api.ModKit;

internal static partial class UI
{
    public const string CheckGlyphOn = "<color=green><b>✔</b></color>";

    public const string CheckGlyphOff = "<color=#B8B8B8FF>✖</color>"; // #A0A0A0E0

    // public const string CheckGlyphEmpty = " <color=#B8B8B8FF>▪</color> ";
    public const string DisclosureGlyphOn = "<color=orange><b>▼</b></color>"; // ▼▲∧⋀
    public const string DisclosureGlyphOff = "<color=#C0C0C0FF><b>▶</b></color>"; // ▶▲∨⋁
    public const string DisclosureGlyphEmpty = " <color=#B8B8B8FF>▪</color> ";

    // Basic UI Elements (box, div, etc.)

    [UsedImplicitly]
    public static void GUIDrawRect(Rect position, Color color)
    {
        GUI.Box(position, GUIContent.none, FillStyle(color));
    }

    [UsedImplicitly]
    public static void Div(float indent = 0, float height = 0, float width = 0)
    {
        Div(FillColor, indent, height, width);
    }

    [UsedImplicitly]
    private static void DivLast(float height = 0)
    {
        var rect = GUILayoutUtility.GetLastRect();
        Div(FillColor, rect.x, height, rect.width + 3);
    }

    [UsedImplicitly]
    public static void Wrap(bool condition, float indent = 0, float space = 10)
    {
        if (!condition)
        {
            return;
        }

        EndHorizontal();
        Space(space);
        BeginHorizontal();
        Space(indent);
    }
}
