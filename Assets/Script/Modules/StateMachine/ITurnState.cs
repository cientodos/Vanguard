public interface ITurnState
{
    // 해당 페이즈 진입 시 실행 (초기화, 자동 액션 등)
    void OnEnter();

    // 매 프레임 업데이트 (입력 대기, 타이머 등 필요 시)
    void OnUpdate();

    // 해당 페이즈 탈출 시 실행 (정리 작업)
    void OnExit();
}