using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static InGameNetWorkManager;

public class ScoreBoardManager : MonoBehaviour
{
    [Header("CurrentPlayerScoreBoard")]
    [SerializeField]
    private GameObject currentPlayerScoreBoard;
    [SerializeField]
    private List<GameObject> normalScoreList = new List<GameObject>();
    [SerializeField]
    private List<GameObject> challengeScoreList = new List<GameObject>();
    [SerializeField]
    private GameObject bonus;
    [SerializeField]
    private GameObject totalScore;

    // 이 스코어 보드가 활성화 되었을 때, 현재 플레이어의 정보를 가져와 갱신하게 한다.
    private void OnEnable()
    {
        
    }

    private void UpdateCurrentPlayerScoreBoard(PlayerController currentPlayer)
    {
        List<int> currentPlayerNormalSocreList = currentPlayer.GetNormalScoreList();
        List<int> currentPlayerChallengeScoreList = currentPlayer.GetChallangeScoreList();

        // Normal Score Board 업데이트
        
    }
}
