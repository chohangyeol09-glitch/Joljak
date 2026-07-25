namespace Members.CHG.Scripts.Weapon
{
    public interface IWeapon
    {
        WeaponData Data { get; }
        bool CanFire { get; }
        void OnStartFire();
        void OnStopFire();
    }
}