using System;

namespace MenuManagement.Base
{
    public enum MenuStatus
    {
        /// <summary> Either the menu is preparing to load or load animation is running </summary>
        InLoading,

        /// <summary> Loaded successfully </summary>
        Loaded,

        /// <summary> Menu unload animation is running </summary>
        InUnloading,

        /// <summary> Unloaded successfully </summary>
        Unloaded,

        /// <summary> Menu is being refreshed </summary>
        InRefresh
    }

    public enum DynamicMenuBehaviour
    {
        /// <summary> Cannot interact with the sub-items </summary>
        NotInteractable = 0,

        /// <summary> Click on child to select, and click on ConfirmButton to confirm </summary>
        PeekIn = 1,

        /// <summary> Hover on a child to select it, and Click on a child to confirm. </summary>
        QuickSelect =2
    }

    [Flags]
    public enum TransformTransitionTargets
    {
        None = 0,
        LocalPosition = 1,
        GlobalPosition = 2,
        LocalRotation = 4,
        GlobalRotation = 8,
        Scale = 16,
        RectSizeDelta = 32,
        RectAnchorPosition = 64,
        RectPivot = 128,
        RectAnchorMin = 256,
        RectAnchorMax = 512,
    }
}