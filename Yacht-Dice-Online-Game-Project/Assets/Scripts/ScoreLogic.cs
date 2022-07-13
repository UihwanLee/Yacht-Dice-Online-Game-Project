using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLogic : MonoBehaviour
{
    /* Score 로직 스크립트 
     * 오르지 Score 값 / 참값만 반환한다.
     * 5개의 주사윈 눈 숫자를 매개변수로 받아들인다.
     * Normal Score은 각각의 점수를 반환한다.
     * Challenge Score는 참값을 판별하여 점수를 반환한다.
    */

    private string FOUROFKIND = "4 of a Kind"; private string FULLHOUSE = "Full House"; private string SMALLSTRAIGHTText = "Small Straight";
    private string LARGESTRAIGHTText = "Large Straight"; public string YACHTText = "Yacht";
    private int ZERO = 0; private int SMALLSTRAIGHT = 15; private int LARGESTRAIGHT = 30; private int YACHT = 50;

    // 인게임 주사위 오브젝트로 Normal Score 리스트 가져오기 
    public List<int> CalculateNormalScoreByDiceObject(List<Dice> scores)
    {
        if (scores.Count == 0) return null;

        List<int> newNormalScores = new List<int>();
        int aces = 0; int deuces = 0; int threes = 0;
        int fours = 0; int fives = 0; int sixes = 0;

        for(int i=0; i< scores.Count; i++)
        {
            int score = scores[i].score;

            switch(score)
            {
                case 1:
                    aces += 1;
                    break;
                case 2:
                    deuces += 2;
                    break;
                case 3:
                    threes += 3;
                    break;
                case 4:
                    fours += 4;
                    break;
                case 5:
                    fives += 5;
                    break;
                case 6:
                    sixes += 6;
                    break;
                default:
                    Debug.Log("[ERROR]주사위 눈이 일치하지 않습니다!");
                    break;
            }
        }

        newNormalScores.Add(aces); newNormalScores.Add(deuces); newNormalScores.Add(threes);
        newNormalScores.Add(fours); newNormalScores.Add(fives); newNormalScores.Add(sixes);


        return newNormalScores;
    }

    // 인게임 주사위 오브젝트로 Challenge Score 리스트 가져오기 
    public List<int> CalculateChallengeByDiceObject(List<Dice> dices)
    {
        if (dices.Count == 0) return null;

        List<int> newChallengeScores = new List<int>();
        int choice = 0; int fourOfaKind = 0; int fullHouse = 0;
        int smallStraight = 0; int largeStraight = 0; int yacht = 0;

        // 각각 상황에 맞게 검사하여 점수 계산

        // 미리 계산한 리스트 가져오기
        List<int> helpScore = new List<int>();
        helpScore = GetHelpScoreList(dices);

        int SUM = 0;
        foreach (var dice in dices) SUM += dice.score;

        // 초이스
        choice = SUM;

        // 포커
        fourOfaKind = CalculateFourOFaKind(helpScore, SUM);

        // 풀하우스
        fullHouse = CalculateFullHouseScore(helpScore, SUM);

        // 스몰 스트레이트
        smallStraight = CalculateSmallStraightScore(dices);

        // 라지 스트레이트
        largeStraight = CalculateLargeStraightScore(dices);

        // 야추
        yacht = CalculateYachtScore(helpScore);
        

        newChallengeScores.Add(choice); newChallengeScores.Add(fourOfaKind); newChallengeScores.Add(fullHouse);
        newChallengeScores.Add(smallStraight); newChallengeScores.Add(largeStraight); newChallengeScores.Add(yacht);


        return newChallengeScores;
    }

    #region Challenge 계산 함수

    // 미리 계산한 리스트 
    private List<int> GetHelpScoreList(List<Dice> dices)
    {
        int aces = 0; int deuces = 0; int threes = 0;
        int fours = 0; int fives = 0; int sixes = 0;

        for (int i = 0; i < dices.Count; i++)
        {
            int score = dices[i].score;

            switch (score)
            {
                case 1:
                    aces++;
                    break;
                case 2:
                    deuces++;
                    break;
                case 3:
                    threes++;
                    break;
                case 4:
                    fours++;
                    break;
                case 5:
                    fives++;
                    break;
                case 6:
                    sixes++;
                    break;
                default:
                    Debug.Log("[ERROR]주사위 눈이 일치하지 않습니다!");
                    break;
            }
        }

        List<int> scoreCount = new List<int>();
        scoreCount.Add(aces); scoreCount.Add(deuces); scoreCount.Add(threes);
        scoreCount.Add(fours); scoreCount.Add(fives); scoreCount.Add(sixes);

        return scoreCount;
    }    

    // 야추 계산
    // (1눈 5개), (2눈 5개), (3눈 5개), (4눈 5개), (5눈 5개), (6눈 5개) 중 하나로 조건에 맞으면 야추!
    private int CalculateYachtScore(List<int> scores)
    {
        if (scores[0] == 5 || scores[1] == 5 || scores[2] == 5 || scores[3] == 5 || scores[4] == 5 || scores[5] == 5)
        {
            return YACHT;
        }

        return ZERO;
    }

    // 라지 스트레이트 계산
    // 5개의 주사위 눈의 참값을 판별하여 (1,2,3,4,5), (2,3,4,5,6) 주사위 눈이 존재해야 라지 스트레이트!
    private int CalculateLargeStraightScore(List<Dice> dices)
    {
        List<bool> checkjLargeStraightList = new List<bool>();
        checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false);
        checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false);

        foreach (var dice in dices) checkjLargeStraightList[dice.score - 1] = true;

        // 라지 스트레이트 검사
        if ((checkjLargeStraightList[0] && checkjLargeStraightList[1] && checkjLargeStraightList[2] && checkjLargeStraightList[3] && checkjLargeStraightList[4])
            || (checkjLargeStraightList[1] && checkjLargeStraightList[2] && checkjLargeStraightList[3] && checkjLargeStraightList[4] && checkjLargeStraightList[5]))
        {
            return LARGESTRAIGHT;
        }

        return ZERO;
    }

    // 스몰 스트레이트 계산
    // (1,2,3,4) or (2,3,4,5) or (3,4,5,6) 으로 주사위 눈이 존재하면 스몰스트레이트!
    private int CalculateSmallStraightScore(List<Dice> dices)
    {
        List<bool> checkjLargeStraightList = new List<bool>();
        checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false);
        checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false);

        foreach (var dice in dices) checkjLargeStraightList[dice.score - 1] = true;

        // 스몰 스트레이트 검사
        if ((checkjLargeStraightList[0] && checkjLargeStraightList[1] && checkjLargeStraightList[2] && checkjLargeStraightList[3])
            || (checkjLargeStraightList[1] && checkjLargeStraightList[2] && checkjLargeStraightList[3] && checkjLargeStraightList[4])
            || (checkjLargeStraightList[2] && checkjLargeStraightList[3] && checkjLargeStraightList[4] && checkjLargeStraightList[5]))
        {
            return SMALLSTRAIGHT;
        }

        return ZERO;
    }

    // 풀하우스 계산
    // 주사위눈의 개수가 2 or 3일때 다른 주사위 눈의 수가 3 or 2인지 판별하여 참값확인
    // 참값 확인 결과 풀하우스 조건에 충족된다면 도합 계산하여 반환
    private int CalculateFullHouseScore(List<int> scores, int SUM)
    {
        // 야추 확인
        if (CalculateYachtScore(scores) == YACHT) return SUM;
        if (CheckFullHouseScore(scores)) return SUM;

        return ZERO;
    }

    // 주사위눈의 개수가 2 or 3일때 다른 주사위 눈의 수가 3 or 2인지 판별하여 참값확인
    private bool CheckFullHouseScore(List<int> scores)
    {
        for (int i = 0; i < scores.Count; i++)
        {
            if (scores[i] == 2)
            {
                for (int j = i; j < scores.Count; j++)
                {
                    if (scores[j] == 3) return true;
                }
            }

            if(scores[i] == 3)
            {
                for(int j = i; j < scores.Count; j++)
                {
                    if (scores[j] == 2) return true;
                }
            }
        }

        return false;
    }

    // 포커 계산
    private int CalculateFourOFaKind(List<int> scores, int SUM)
    {
        // 야추 확인
        if (CalculateYachtScore(scores) == YACHT) return SUM;
        // 포커 확인
        if (scores[0] == 4 || scores[1] == 4 || scores[2] == 4 || scores[3] == 4 || scores[4] == 4 || scores[5] == 4) return SUM;

        return ZERO;
    }

    #endregion

    // Challenge Success 내용 정보 반환 : 인덱스와 그 인덱스 점수로 판별
    public string GetChallengeSuccess(int index, int score)
    {
        string success = "";

        if(score != 0)
        {
            switch (index)
            {
                case 1:
                    success = FOUROFKIND;
                    break;
                case 2:
                    success = FULLHOUSE;
                    break;
                case 3:
                    success = SMALLSTRAIGHTText;
                    break;
                case 4:
                    success = LARGESTRAIGHTText;
                    break;
                case 5:
                    success = YACHTText;
                    break;
                default:
                    break;
            }
        }


        return success;
    }
}
