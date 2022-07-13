using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreLogic : MonoBehaviour
{
    /* Score ���� ��ũ��Ʈ 
     * ������ Score �� / ������ ��ȯ�Ѵ�.
     * 5���� �ֻ��� �� ���ڸ� �Ű������� �޾Ƶ��δ�.
     * Normal Score�� ������ ������ ��ȯ�Ѵ�.
     * Challenge Score�� ������ �Ǻ��Ͽ� ������ ��ȯ�Ѵ�.
    */

    private string FOUROFKIND = "4 of a Kind"; private string FULLHOUSE = "Full House"; private string SMALLSTRAIGHTText = "Small Straight";
    private string LARGESTRAIGHTText = "Large Straight"; public string YACHTText = "Yacht";
    private int ZERO = 0; private int SMALLSTRAIGHT = 15; private int LARGESTRAIGHT = 30; private int YACHT = 50;

    // �ΰ��� �ֻ��� ������Ʈ�� Normal Score ����Ʈ �������� 
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
                    Debug.Log("[ERROR]�ֻ��� ���� ��ġ���� �ʽ��ϴ�!");
                    break;
            }
        }

        newNormalScores.Add(aces); newNormalScores.Add(deuces); newNormalScores.Add(threes);
        newNormalScores.Add(fours); newNormalScores.Add(fives); newNormalScores.Add(sixes);


        return newNormalScores;
    }

    // �ΰ��� �ֻ��� ������Ʈ�� Challenge Score ����Ʈ �������� 
    public List<int> CalculateChallengeByDiceObject(List<Dice> dices)
    {
        if (dices.Count == 0) return null;

        List<int> newChallengeScores = new List<int>();
        int choice = 0; int fourOfaKind = 0; int fullHouse = 0;
        int smallStraight = 0; int largeStraight = 0; int yacht = 0;

        // ���� ��Ȳ�� �°� �˻��Ͽ� ���� ���

        // �̸� ����� ����Ʈ ��������
        List<int> helpScore = new List<int>();
        helpScore = GetHelpScoreList(dices);

        int SUM = 0;
        foreach (var dice in dices) SUM += dice.score;

        // ���̽�
        choice = SUM;

        // ��Ŀ
        fourOfaKind = CalculateFourOFaKind(helpScore, SUM);

        // Ǯ�Ͽ콺
        fullHouse = CalculateFullHouseScore(helpScore, SUM);

        // ���� ��Ʈ����Ʈ
        smallStraight = CalculateSmallStraightScore(dices);

        // ���� ��Ʈ����Ʈ
        largeStraight = CalculateLargeStraightScore(dices);

        // ����
        yacht = CalculateYachtScore(helpScore);
        

        newChallengeScores.Add(choice); newChallengeScores.Add(fourOfaKind); newChallengeScores.Add(fullHouse);
        newChallengeScores.Add(smallStraight); newChallengeScores.Add(largeStraight); newChallengeScores.Add(yacht);


        return newChallengeScores;
    }

    #region Challenge ��� �Լ�

    // �̸� ����� ����Ʈ 
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
                    Debug.Log("[ERROR]�ֻ��� ���� ��ġ���� �ʽ��ϴ�!");
                    break;
            }
        }

        List<int> scoreCount = new List<int>();
        scoreCount.Add(aces); scoreCount.Add(deuces); scoreCount.Add(threes);
        scoreCount.Add(fours); scoreCount.Add(fives); scoreCount.Add(sixes);

        return scoreCount;
    }    

    // ���� ���
    // (1�� 5��), (2�� 5��), (3�� 5��), (4�� 5��), (5�� 5��), (6�� 5��) �� �ϳ��� ���ǿ� ������ ����!
    private int CalculateYachtScore(List<int> scores)
    {
        if (scores[0] == 5 || scores[1] == 5 || scores[2] == 5 || scores[3] == 5 || scores[4] == 5 || scores[5] == 5)
        {
            return YACHT;
        }

        return ZERO;
    }

    // ���� ��Ʈ����Ʈ ���
    // 5���� �ֻ��� ���� ������ �Ǻ��Ͽ� (1,2,3,4,5), (2,3,4,5,6) �ֻ��� ���� �����ؾ� ���� ��Ʈ����Ʈ!
    private int CalculateLargeStraightScore(List<Dice> dices)
    {
        List<bool> checkjLargeStraightList = new List<bool>();
        checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false);
        checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false);

        foreach (var dice in dices) checkjLargeStraightList[dice.score - 1] = true;

        // ���� ��Ʈ����Ʈ �˻�
        if ((checkjLargeStraightList[0] && checkjLargeStraightList[1] && checkjLargeStraightList[2] && checkjLargeStraightList[3] && checkjLargeStraightList[4])
            || (checkjLargeStraightList[1] && checkjLargeStraightList[2] && checkjLargeStraightList[3] && checkjLargeStraightList[4] && checkjLargeStraightList[5]))
        {
            return LARGESTRAIGHT;
        }

        return ZERO;
    }

    // ���� ��Ʈ����Ʈ ���
    // (1,2,3,4) or (2,3,4,5) or (3,4,5,6) ���� �ֻ��� ���� �����ϸ� ������Ʈ����Ʈ!
    private int CalculateSmallStraightScore(List<Dice> dices)
    {
        List<bool> checkjLargeStraightList = new List<bool>();
        checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false);
        checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false); checkjLargeStraightList.Add(false);

        foreach (var dice in dices) checkjLargeStraightList[dice.score - 1] = true;

        // ���� ��Ʈ����Ʈ �˻�
        if ((checkjLargeStraightList[0] && checkjLargeStraightList[1] && checkjLargeStraightList[2] && checkjLargeStraightList[3])
            || (checkjLargeStraightList[1] && checkjLargeStraightList[2] && checkjLargeStraightList[3] && checkjLargeStraightList[4])
            || (checkjLargeStraightList[2] && checkjLargeStraightList[3] && checkjLargeStraightList[4] && checkjLargeStraightList[5]))
        {
            return SMALLSTRAIGHT;
        }

        return ZERO;
    }

    // Ǯ�Ͽ콺 ���
    // �ֻ������� ������ 2 or 3�϶� �ٸ� �ֻ��� ���� ���� 3 or 2���� �Ǻ��Ͽ� ����Ȯ��
    // ���� Ȯ�� ��� Ǯ�Ͽ콺 ���ǿ� �����ȴٸ� ���� ����Ͽ� ��ȯ
    private int CalculateFullHouseScore(List<int> scores, int SUM)
    {
        // ���� Ȯ��
        if (CalculateYachtScore(scores) == YACHT) return SUM;
        if (CheckFullHouseScore(scores)) return SUM;

        return ZERO;
    }

    // �ֻ������� ������ 2 or 3�϶� �ٸ� �ֻ��� ���� ���� 3 or 2���� �Ǻ��Ͽ� ����Ȯ��
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

    // ��Ŀ ���
    private int CalculateFourOFaKind(List<int> scores, int SUM)
    {
        // ���� Ȯ��
        if (CalculateYachtScore(scores) == YACHT) return SUM;
        // ��Ŀ Ȯ��
        if (scores[0] == 4 || scores[1] == 4 || scores[2] == 4 || scores[3] == 4 || scores[4] == 4 || scores[5] == 4) return SUM;

        return ZERO;
    }

    #endregion

    // Challenge Success ���� ���� ��ȯ : �ε����� �� �ε��� ������ �Ǻ�
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
