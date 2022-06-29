using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : MonoBehaviour
{
    /* <Title Panel 包府 胶农赋飘>
     * 
     * Title Panel Animation 包府
     * Title Panel Effect 包府
     * Title Panel Button 包府
     * 
    */

    [SerializeField]
    private Animator titleAnimator;

    private void OnEnable()
    {
        // Title Panel 龋免 矫 Title 剁快扁
        titleAnimator.SetBool("isAppear", true);
    }

}
