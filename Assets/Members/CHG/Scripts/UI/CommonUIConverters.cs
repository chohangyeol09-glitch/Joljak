using UnityEngine;
using UnityEngine.UIElements;

namespace CHG.Scripts.UI
{
    public static class CommonUIConverters
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        public static void RegisterConverters()
        {
            var percentGroup = new ConverterGroup("Float to StyleLength Percent");
            percentGroup.AddConverter((ref float normalized) =>
                new StyleLength(new Length(normalized * 100f, LengthUnit.Percent)));
            ConverterGroups.RegisterConverterGroup(percentGroup);

            var intGroup = new ConverterGroup("Int to String");
            intGroup.AddConverter((ref int value) => value.ToString());
            ConverterGroups.RegisterConverterGroup(intGroup);

            // true 면 보이고 false 면 숨긴다. style.display 바인딩용.
            var displayGroup = new ConverterGroup("Bool to Display");
            displayGroup.AddConverter((ref bool visible) =>
                new StyleEnum<DisplayStyle>(visible ? DisplayStyle.Flex : DisplayStyle.None));
            ConverterGroups.RegisterConverterGroup(displayGroup);

            // Sprite 를 style.backgroundImage 에 꽂기 위한 변환.
            var backgroundGroup = new ConverterGroup("Sprite to Background");
            backgroundGroup.AddConverter((ref Sprite sprite) => new StyleBackground(sprite));
            ConverterGroups.RegisterConverterGroup(backgroundGroup);
        }
    }
}