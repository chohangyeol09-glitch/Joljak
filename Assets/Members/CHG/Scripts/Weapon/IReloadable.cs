namespace Members.CHG.Scripts.Weapon
{
    public interface IReloadable
    {
        int CurrentAmmo { get; }
        int MaxAmmo { get; }
        bool IsReloading { get; }
        float ReloadTime { get; }
        void Reload();
    }
}