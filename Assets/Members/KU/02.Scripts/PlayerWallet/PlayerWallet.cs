using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private int money;
    [SerializeField] private Chest targetChest;

    public int Money => money;

    private void Update()
    {
        if (Keyboard.current == null)
            return;

        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            if (targetChest == null)
            {
                Debug.LogWarning("테스트할 상자가 연결되지 않았습니다.");
                return;
            }

            bool opened = targetChest.TryOpen(this);

            Debug.Log(
                opened
                    ? "상자 열기 성공"
                    : "상자 열기 실패: 돈 부족 또는 이미 열린 상자"
            );
        }
    }


    public void AddMoney(int amount)
    {
        money += Mathf.Max(0, amount);
    }

    public bool TrySpend(int amount)
    {
        if (amount < 0)
            return false;

        if (money < amount)
            return false;

        money -= amount;
        return true;
    }
}