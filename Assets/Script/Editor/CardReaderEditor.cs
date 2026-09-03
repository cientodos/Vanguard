#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using GoogleSheetsToUnity;
using Vanguard.Data.Databases;

[CustomEditor(typeof(CardReader))]
public class CardReaderEditor : Editor
{
    CardReader data;

    void OnEnable()
    {
        data = (CardReader)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GUILayout.Space(15);
        if (GUILayout.Button("구글 시트 데이터 가져오기 (API 호출)", GUILayout.Height(35)))
        {
            data.DataList.Clear();

            // 3번째 인자를 제거하고 시트 ID와 탭 이름만 전달
            GSTU_Search search = new GSTU_Search(data.associatedSheet, data.associatedWorksheet);

            SpreadsheetManager.Read(
                search,
                (ss) =>
                {
                    // 시트에 존재하는 모든 행을 끝까지 순회
                    foreach (var pair in ss.rows.primaryDictionary)
                    {
                        int rowIndex = pair.Key;

                        // 1행(헤더)을 제외하고 2행부터 전부 파싱
                        if (rowIndex >= data.START_ROW_LENGTH)
                        {
                            data.AddDataFromRow(pair.Value);
                        }
                    }

                    EditorUtility.SetDirty(target);
                    AssetDatabase.SaveAssets();
                    Debug.Log($"<color=green>[GSTU] 동기화 완료!</color> 총 {data.DataList.Count}장 카드 파싱됨.");
                }
            );
        }
    }
}
#endif