using Unity.VisualScripting;
using UnityEngine;

public class player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; //制作动画
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex; //用于遍历动画数组

    private new Rigidbody2D rigidbody;  //这里不加new会报警告，因为rigibody这个名字已经在unity里被使用了
    private new Collider2D collider;
    private Collider2D[] results;  
    private Vector2 direction;   //vector2代表2维
    public float MoveSpeed = 3f;  //定义一个初始默认移动速度
    public float jumpStrengh = 3f;
    private bool grounded;
    private bool climbing;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>(); //这里都是获得角色组件
        collider = GetComponent<Collider2D>();
        results = new Collider2D[4]; //这里是重叠的物体数量
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f/12f, 1f/12f);  //InvokeRepeating  （方法名，首次调用在1/12秒后，每隔1/12秒用一次）
    }

    private void OnDisable()
    {
        CancelInvoke();  //取消这个脚本中所有的 Invoke 调用
    }


    private void CheakCollision()
    {
        grounded = false; //初始化一下
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f; //这里让x轴上的宽度减小一些，在爬楼梯的时候视觉上会和楼梯有一部分重叠，更加合理
        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, results);  //用于范围检测，这里和银鸟对空中持续跳跃问题的解决方案不一样

        for(int i = 0;i < amount; i++)
        {
            GameObject hit = results[i].gameObject;

            if(hit.layer == LayerMask.NameToLayer("Ground"))  //这里的逻辑就是以layer判断是否在地面上
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);   //这里保证地面y位置小于player位置以保证不是撞到头上的地面,-0.5f是因为马里奥的脚在胶囊外

                Physics2D.IgnoreCollision(collider, results[i], !grounded);  //当跳起来的时候忽略和地面的碰撞，也就是说忽略和上面一层地面的碰撞检测
            }else if(hit.layer == LayerMask.NameToLayer("Ladder")) //检测是否有楼梯
            {
                climbing = true;
            }
        }
    }

    private void Update()  //updata 每帧都调用
    {
        CheakCollision();
        
        if (climbing)  //这里要先判断是否在攀爬，再去判断跳
        {
            direction.y = Input.GetAxis("Vertical")* MoveSpeed;
        }

        else if (grounded && Input.GetKeyDown(KeyCode.Space))  //我这里用的空格，其实可以直接用jump，这里的水平垂直跳键都是unity里面默认的,
        {
            direction = Vector2.up * jumpStrengh;
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;  //Physics2D.gravity的默认值是y上的-9.81
        }

        direction.x = Input.GetAxis("Horizontal")*MoveSpeed; //水平

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }
         

        if(direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero; //如果向右，方向不变
        }else if(direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f ,0f);//如果向左，y转180°（这里原始素材是向右的）
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if (climbing)  //攀爬
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)  //跑动动画,只有在有横向移动时才被调用
        {
            spriteIndex++;
            if (spriteIndex >= runSprites.Length)  //在动画数组里反复循环
            {
                spriteIndex = 0;
            }

            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Objective"))
        {
            enabled = false; //判定时候不能移动
            FindAnyObjectByType<GameManager>().LevelComplete(); //碰到公主完成游戏
        }else if (collision.gameObject.CompareTag("Obstacle"))
        {
            enabled = false;
            FindAnyObjectByType<GameManager>().LevelFailed(); //碰到木桶或者金刚结束youxi
        }
    }

}


