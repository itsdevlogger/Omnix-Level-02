using JetBrains.Annotations;
using System.Collections;

namespace MenuManagement.Base
{
    public interface IDynamicMenu<in TData>
    {
        public bool AllowDeselect { get; }
        public DynamicMenuBehaviour Behaviour { get; }
        public void SelectItem(TData data);
        public void ConfirmItem(TData data);
        public void DeselectItem();
    }

    public interface IMenuTransition
    {
        /// <summary> Animate the menus. </summary>
        /// <param name="unload"> Run unloading animation for this menu. </param>
        /// <param name="load"> Run loading animation for this menu. </param>
        public IEnumerator Animate([CanBeNull] BaseMenu unload, [CanBeNull] BaseMenu load);

        /// <summary> Cleanup any temporary mess-ups done during animate phase. </summary>
        public void Cleanup([CanBeNull] BaseMenu unload, [CanBeNull] BaseMenu load);
    }
}