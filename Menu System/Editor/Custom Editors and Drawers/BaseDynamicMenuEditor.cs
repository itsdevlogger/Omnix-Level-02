using System.Collections.Generic;
using MenuManagement.Behaviours;
using UnityEditor;

namespace MenuManagement.Editor
{
    [CustomEditor(typeof(BaseDynamicMenu<,,>), true)]
    [CanEditMultipleObjects]
    public class BaseDynamicMenuEditor : BaseMenuEditor
    {
        protected override IEnumerable<BasePropertyGroupDrawer> RegisterGroups()
        {
            yield return new GD_DynamicMenuSettingsDrawer(this);

            foreach (BasePropertyGroupDrawer group in base.RegisterGroups())
            {
                yield return group;
            }
        }
    }
}