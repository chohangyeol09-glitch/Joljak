using CHG.Scripts.CombatSystem;
using UnityEngine;

public class TestTakeDamage : MonoBehaviour
{
    [SerializeField] private HealthModule healthModule;
    [SerializeField] private int damageAmount;
 
    [ContextMenu("Take Damage")]
    public void TakeDamage()
    {
        healthModule.ApplyDamage(damageAmount);
    }
}
