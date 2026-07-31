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
        }
    }
}