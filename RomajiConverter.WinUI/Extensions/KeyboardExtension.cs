using Windows.System;
using Windows.UI.Core;
using Microsoft.UI.Input;

namespace RomajiConverter.WinUI.Extensions;

public static class KeyboardExtension
{
    public static bool IsKeyDown(VirtualKey key)
    {
        return InputKeyboardSource.GetKeyStateForCurrentThread(key).HasFlag(CoreVirtualKeyStates.Down);
    }
}