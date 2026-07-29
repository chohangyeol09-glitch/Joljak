namespace CHG.Scripts.Weapon
{
    public interface IWeapon
    {
        WeaponDisplayInfo Display { get; }
        int Damage { get; }
        float FireRate { get; }
        bool CanFire { get; }
        void StartFire();
        void StopFire();
    }
}