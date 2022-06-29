using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    /* <Title Panel ���� ��ũ��Ʈ>
     * 
     * Title Panel Animation ����
     * Title Panel Effect ����
     * Title Panel Button ����
     * 
    */

    [SerializeField]
    private Animator titleAnimator;

    private void OnEnable()
    {
        // Title Panel ȣ�� �� Title ����
        titleAnimator.SetBool("isAppear", true);
    }

}
