public static class CardIdUtil
{
    /// <summary>
    /// 8자리 카드 ID에서 3번째 자리(레어도)를 0으로 초기화하여 기본 카드 ID를 반환합니다.
    /// 예: 60200000 -> 60000000
    /// </summary>
    public static int GetBaseCardId(int rawCardId)
    {
        int rarityDigit = (rawCardId / 100000) % 10;
        return rawCardId - (rarityDigit * 100000);
    }
}