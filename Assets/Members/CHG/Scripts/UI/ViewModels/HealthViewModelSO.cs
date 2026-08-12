using UnityEngine;
using UnityEngine.UIElements;

namespace CHG.Scripts.UI.ViewModels
{
    [CreateAssetMenu(fileName = "Health view model", menuName = "UI/Health view", order = 0)]
    public class HealthViewModelSO : AbstractCountViewModelSO
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterConverters()
        {
            var healthGroup = new ConverterGroup("Float to HealthBar StyleColor");
            
            healthGroup.AddConverter((ref float normalized) => 
                new StyleColor(Color.Lerp(Color.red,Color.green, normalized)));
            
            healthGroup.AddConverter((ref float normalized) => normalized switch
            {
                < 1f / 3f => "Danger",
                < 2f / 3f => "Warning",
                _ => "Healthy"
            });
            
            ConverterGroups.RegisterConverterGroup(healthGroup);
        }
        
    }
}