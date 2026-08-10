using UnityEngine;

/// <summary>
/// プレイヤーの移動アニメーションと向きを制御する。
/// </summary>
public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;

    private void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveY = Input.GetAxisRaw("Vertical");

        // 移動中かどうか
        bool isWalking = (moveX != 0f || moveY != 0f);
        animator.SetBool("isWalking", isWalking);

        // 移動方向を設定
        if (moveY > 0f)
        {
            // 上
            animator.SetInteger("direction", 1);
        }
        else if (moveY < 0f)
        {
            // 下
            animator.SetInteger("direction", 0);
        }
        else if (moveX > 0f)
        {
            // 右
            animator.SetInteger("direction", 2);
        }
        else if (moveX < 0f)
        {
            // 左
            animator.SetInteger("direction", 3);
        }
    }
}