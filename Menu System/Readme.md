# <font color="#a5d3f2">Introduction</font>
The Menu Management Framework is a powerful tool designed to empower game developers by simplifying the creation of
menus and user interfaces. In this context, a menu refers to any collection of UI elements. What sets this framework
apart is its ability to create menus that are versatile and can be reused across various parts of your game.

Each menu has four essential attributes, and the beauty of this framework is that they are all independent of one
another. This means you can create a menu once and then use it in a multitude of ways throughout your game.

1. **Behaviors**  
   Define what is displayed to the player and how the menu reacts to player actions like hovering or clicking.     
   This is the only part you'll need to code.

2. **Transitions**  
   Decide how the menu enters the screen.
   You can make it slide in, fade in, or simply appear.

3. **Perceptions**  
   Determine how the player interprets the menu.
   Is it an isolated menu, popup menu, a side panel, or a part of a larger screen?

4. **Layouts**  
   Decide how the menu elements are arranged.
   Should they be in a vertical list, a horizontal row, a grid, or perhaps in a circular layout, or maybe a predefined
   static layout?

# <font color="#a5d3f2">Base Menu Class</font>
Every menu you create using this framework must derive from the `BaseMenu` class. When you do this, you gain access to a
variety of fields, events, and methods that make menu management a breeze. The `BaseMenu` class handles all these core
elements, allowing you to focus on your specific menu functionality.

## Class Members

- `public void BeginLoading()`  
  Initiates the loading process for the menu.

- `public void BeginUnloading()`  
  Initiates the unloading process for the menu.

- `protected abstract IEnumerator PrepareLoad()`  
  A coroutine that runs before loading the menu. Download the essential data/assets for this menu.

- `protected abstract IEnumerator PrepareUnload()`  
  A coroutine that runs before unloading the menu. Clean up resources, close connections, or release any memory
  associated with the menu

## Stages for Loading & Unloading

Following sequence will be executed while loading <font color="#a9d4b2">LoadMenu</font> and
unloading <font color="#C9413B">UnloadMenu</font>
> **Note**    
> 1. All operation under Union Group will happen asynchronously & further execution will be suspended until all are done
> 2. You can access the <font color="#4EC9B0">Event Callback</font> from code or from inspector.
> 3. You can override the <font color="#DCDCAA">Methods / Coroutines</font> from code.
> 4. You can access <font color="#569CD6">Properties</font> from inspector.

1. Call Initialize:
    - if <font color="#a9d4b2">LoadMenu</font> is never loaded/unloaded before, then <font color="#a9d4b2">LoadMenu</font>.<font color="#DCDCAA">Initialize</font>
    - if <font color="#C9413B">UnloadMenu</font> is never loaded/unloaded before, then <font color="#C9413B">UnloadMenu</font>.<font color="#DCDCAA">Initialize</font>

2. Play Clips:
    - <font color="#a9d4b2">LoadMenu</font>.<font color="#569CD6">loadClip</font>.
    - <font color="#C9413B">UnloadMenu</font>.<font color="#569CD6">unloadClip</font>.

3. Wait (Union):
    - <font color="#a9d4b2">LoadMenu</font>.<font color="#569CD6">loadDelay</font>
    - <font color="#C9413B">UnloadMenu</font>.<font color="#569CD6">unloadDelay</font>
    - <font color="#a9d4b2">LoadMenu</font>.<font color="#569CD6">Status</font> == InLoading || <font color="#a9d4b2">LoadMenu</font>.<font color="#569CD6">Status</font> == InUnloading
    - <font color="#C9413B">UnloadMenu</font>.<font color="#569CD6">Status</font> == InLoading || <font color="#C9413B">UnloadMenu</font>.<font color="#569CD6">Status</font> == InUnloading

4. Status:
    - Set <font color="#a9d4b2">LoadMenu</font>.<font color="#569CD6">Status</font> to InLoading
    - Set <font color="#C9413B">UnloadMenu</font>.<font color="#569CD6">Status</font> to InUnloading

5. Execute <font color="#a9d4b2">LoadMenu</font>.<font color="#DCDCAA">BeforeLoad</font> Coroutine

6. Call <font color="#a9d4b2">LoadMenu</font>.<font color="#4EC9B0">onLoad</font>

7. Set State of Connected GameObjects 
   - Activate objects in <font color="#a9d4b2">LoadMenu</font>.<font color="#569CD6">activateWithMenu</font> list 
   - Deactivate objects in <font color="#a9d4b2">LoadMenu</font>.<font color="#569CD6">activateAgainstMenu</font> list

8. Transition (Union):
    - <font color="#a9d4b2">LoadMenu</font>.loadTransition
    - <font color="#C9413B">UnloadMenu</font>.unloadTransition

9. <font color="#C9413B">UnloadMenu</font>.<font color="#4EC9B0">onUnload</font>

10. Set State of Connected GameObjects
    - Deactivate objects in <font color="#C9413B">UnloadMenu</font>.<font color="#569CD6">activateWithMenu</font> list
    - Activate objects in <font color="#C9413B">UnloadMenu</font>.<font color="#569CD6">activateAgainstMenu</font> list

11. Execute <font color="#C9413B">UnloadMenu</font>.<font color="#DCDCAA">AfterUnload</font> coroutine
12. onComplete callback passed as the parameter of the method

13. Cleanup any mess made during transition

14. Delay of 2 frames

15. Delayed Callbacks:
    - <font color="#a9d4b2">LoadMenu</font>.<font color="#4EC9B0">onLoadDelayed</font>
    - <font color="#C9413B">UnloadMenu</font>.<font color="#4EC9B0">onUnloadDelayed</font>

# <font color="#a5d3f2">Behaviors</font>

## Static Menu

A static menu is one with a fixed user interface. Simply adding a "StaticMenu" component to any GameObject is sufficient
to convert it into a static menu. It's important to note that any GameObject with the StaticMenu component must be
active in the hierarchy to function properly. You can choose the default state of the menu by setting the value of '
loadInAwake' boolean of the static menu component.

## Dynamic Menu

Dynamic menus are generated at runtime. Users can derive from the `DynamicMenuBase` class and provide an IEnumerable
containing all the items to display. When implementing a base class, you have the flexibility to override various
methods and callbacks. While overriding these, you only need to call base methods for Awake and SetupItem.

You can also override all the callbacks mentioned in the previous section, such as Initialize, OnBeginLoad, PrepareLoad,
OnLoad, OnEndLoad, OnEndLoadDelayed, OnBeginUnload, PrepareUnload, OnUnload, OnEndUnload, and OnEndUnloadDelayed.

# <font color="#a5d3f2">Transitions</font>
The framework includes numerous transition classes for various animations. To apply any transition to a menu, simply add 
the transition component to the gameObject with menu component.
> **Note**
> 1. Adding transition to GameObject in runtime might not apply that transition. This is because the transition are cached during runtime to avoid repeated GetComponent callbacks.
> 2. If you do wish to load the menu with a different transition (via code) then you can pass that transition to Load/Unload method. 

## Custom Transitions
If you have a unique transition in mind, you can create your own by implementing the "IMenuTransition" interface. 
If this new transition is a MonoBehavior class, then adding it to menu gameObject will apply it.
If this is not a  MonoBehavior class, then you can pass it while calling Load/Unload method.

## Custom Transitions with Builtin Attributes
For most transitions, you can implement "BaseTransitionBlendable" class that provides multiple attributes to customize
the transition through the inspector. All these attributes are managed by the class itself. You will only have to implement 
the abstract members.

<b>These attributes include:</b>
1. Transition Duration: Control the duration of the transition.
2. In Out Blend: Control how loading and unloading animations blend together. This allows you to have loading animations
   play partially before or after unloading animations.
3. Ease: Define the animation progress.
4. Transition Orders: Determine the transition play order to maintain consistency while moving between menus.

<b>Abstract Members:</b>
1. `void Prepare([CanBeNull] BaseMenu unload, [CanBeNull] BaseMenu load)`  
   Prepare for a transition and cache the properties of menus that will be animated to save processing power.
2. `void SetUnloadingFrame([NotNull] BaseMenu unload, float t, bool playingInReversed)`    
   Set properties of the unloading menu based on the current progress in the transition.
3. `void SetLoadingFrame([NotNull] BaseMenu load, float t, bool playingInReversed)`   
   Set properties of the loading menu based on the current progress in the transition.

# <font color="#a5d3f2">Perception</font>

## Menu Screen

The `MenuScreenManager` allows you to create a screen with multiple menus. Each menu is associated with a toggle, and
players can click these toggles to load the relevant menu. Note that if a menu is added, removed, or destroyed at
runtime while using `MenuScreenManager`, the respective manager must be notified by
calling `MenuScreenManager.RegisterMenu` or `MenuScreenManager.UnregisterMenu`.

## Popup Menu

To use a menu as a popup menu, simply add the `PopupMenuSettings` component to the menu GameObject.

## Isolated Menu or Side Panel

You can call `BeginLoading` and/or `BeginUnloading` on a button click to load/unload a menu.

# <font color="#a5d3f2">Layouts</font>

In order to set a layout for the DynamicMenu you can add any one of LayoutGroup components to Items Parent (Check
itemsParent property of BaseDynamicMenu class)   
Following are the LayoutGroup components that you can use.
Unity's built-in components:

- VerticalLayoutGroup
- HorizontalLayoutGroup
- GridLayoutGroup

In addition to these the framework also provides some other LayoutGroups:

- CircularLayoutGroup - Arrange children on circumference of a circle

# <font color="#a5d3f2">Bonus</font>

All menus and transitions come with custom inspectors by default. These inspectors neatly organize all properties into
different groups, making it easier to manage them. This grouping is accomplished using a few attributes. You can add
these attributes to any class that inherits from the `BaseMenu` class to specify how properties should be grouped:

- `GroupEvents`: Add properties to the "Events" group, which are shown as events that can be toggled on or off.
- `GroupRuntimeConstant`: Add properties to the "Runtime Constants" group, which shouldn't be edited during Play Mode.
- `GroupReadOnly`: Add properties to the "Read Only" group, which shouldn't be edited at all.
- `GroupProperties`: Add properties to an existing group or create a new