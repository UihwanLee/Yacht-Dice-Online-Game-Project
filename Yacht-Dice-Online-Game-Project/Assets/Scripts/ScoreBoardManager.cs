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

    // �� ���ھ� ���尡 Ȱ��ȭ �Ǿ��� ��, ���� �÷��̾��� ������ ������ �����ϰ� �Ѵ�.
    private void OnEnable()
    {
        
    }

    private void UpdateCurrentPlayerScoreBoard(PlayerController currentPlayer)
    {
        List<int> currentPlayerNormalSocreList = currentPlayer.GetNormalScoreList();
        List<int> currentPlayerChallengeScoreList = currentPlayer.GetChallangeScoreList();

        // Normal Score Board ������Ʈ
        
    }
}
