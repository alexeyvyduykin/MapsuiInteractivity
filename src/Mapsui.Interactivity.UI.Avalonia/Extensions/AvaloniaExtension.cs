﻿using Mapsui.Interactivity.UI.Input;
using Mapsui.Interactivity.UI.Input.Core;
using Mapsui.UI.Avalonia;
using Mapsui.UI.Avalonia.Extensions;
using aInput = Avalonia.Input;

namespace Mapsui.Interactivity.UI.Avalonia;

public static class AvaloniaExtension
{
    public static aInput.StandardCursorType ToStandartCursor(this CursorType cursorType)
    {
        return cursorType switch
        {
            CursorType.Default => aInput.StandardCursorType.Arrow,
            CursorType.Hand => aInput.StandardCursorType.Hand,
            CursorType.HandGrab => aInput.StandardCursorType.SizeAll,
            CursorType.Cross => aInput.StandardCursorType.Cross,
            _ => throw new System.Exception(),
        };
    }

    public static MouseButton Convert(this aInput.PointerUpdateKind state)
    {
        return state switch
        {
            aInput.PointerUpdateKind.LeftButtonPressed => MouseButton.Left,
            aInput.PointerUpdateKind.MiddleButtonPressed => MouseButton.Middle,
            aInput.PointerUpdateKind.RightButtonPressed => MouseButton.Right,
            aInput.PointerUpdateKind.XButton1Pressed => MouseButton.XButton1,
            aInput.PointerUpdateKind.XButton2Pressed => MouseButton.XButton2,
            aInput.PointerUpdateKind.LeftButtonReleased => MouseButton.Left,
            aInput.PointerUpdateKind.MiddleButtonReleased => MouseButton.Middle,
            aInput.PointerUpdateKind.RightButtonReleased => MouseButton.Right,
            aInput.PointerUpdateKind.XButton1Released => MouseButton.XButton1,
            aInput.PointerUpdateKind.XButton2Released => MouseButton.XButton2,
            aInput.PointerUpdateKind.Other => MouseButton.None,
            _ => MouseButton.None,
        };
    }

    public static MouseWheelEventArgs ToMouseWheelEventArgs(this aInput.PointerWheelEventArgs e, aInput.InputElement relativeTo)
    {
        return new MouseWheelEventArgs
        {
            Delta = (int)(e.Delta.Y + e.Delta.X) * 120
        };
    }

    public static MouseEventArgs ToMouseEventArgs(this aInput.PointerEventArgs e, aInput.InputElement relativeTo)
    {
        MapInfo? mapInfo = null;

        if (relativeTo is MapControl mapControl)
        {
            var position = e.GetPosition(relativeTo).ToMapsui();

            mapInfo = mapControl.GetMapInfo(position);
        }

        return new MouseEventArgs
        {
            MapInfo = mapInfo,
        };
    }

    public static MouseDownEventArgs ToMouseDownEventArgs(this aInput.PointerPressedEventArgs e, aInput.InputElement relativeTo)
    {
        MapInfo? mapInfo = null;

        if (relativeTo is MapControl mapControl)
        {
            var position = e.GetPosition(relativeTo).ToMapsui();

            mapInfo = mapControl.GetMapInfo(position);
        }

        return new MouseDownEventArgs
        {
#pragma warning disable CS0618 // Тип или член устарел
            //ChangedButton = e.GetPointerPoint(null).Properties.PointerUpdateKind.Convert(),
            ChangedButton = e.GetCurrentPoint(null).Properties.PointerUpdateKind.Convert(),
#pragma warning restore CS0618 // Тип или член устарел
            ClickCount = e.ClickCount,
            MapInfo = mapInfo
        };
    }

    public static MouseEventArgs ToMouseReleasedEventArgs(this aInput.PointerReleasedEventArgs e, aInput.InputElement relativeTo)
    {
        MapInfo? mapInfo = null;

        if (relativeTo is MapControl mapControl)
        {
            var position = e.GetPosition(relativeTo).ToMapsui();

            mapInfo = mapControl.GetMapInfo(position);
        }

        return new MouseEventArgs
        {
            MapInfo = mapInfo,
        };
    }
}
