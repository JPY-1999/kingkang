using Unity.VisualScripting;
using UnityEngine;

public class player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer; //��������
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex; //���ڱ�����������

    private new Rigidbody2D rigidbody;  //���ﲻ��new�ᱨ���棬��Ϊrigibody��������Ѿ���unity�ﱻʹ����
    private new Collider2D collider;
    private Collider2D[] results;  
    private Vector2 direction;   //vector2����2ά
    public float MoveSpeed = 3f;  //����һ����ʼĬ���ƶ��ٶ�
    public float jumpStrengh = 3f;
    private bool grounded;
    private bool climbing;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>(); //���ﶼ�ǻ�ý�ɫ���
        collider = GetComponent<Collider2D>();
        results = new Collider2D[4]; //�������ص�����������
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f/12f, 1f/12f);  //InvokeRepeating  �����������״ε�����1/12���ÿ��1/12����һ�Σ�
    }

    private void OnDisable()
    {
        CancelInvoke();  //ȡ������ű������е� Invoke ����
    }


    private void CheakCollision()
    {
        grounded = false; //��ʼ��һ��
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f; //������x���ϵĿ�ȼ�СһЩ������¥�ݵ�ʱ���Ӿ��ϻ��¥����һ�����ص������Ӻ���
        int amount = Physics2D.OverlapBoxNonAlloc(transform.position, size, 0f, results);  //���ڷ�Χ��⣬���������Կ��г�����Ծ����Ľ��������һ��

        for(int i = 0;i < amount; i++)
        {
            GameObject hit = results[i].gameObject;

            if(hit.layer == LayerMask.NameToLayer("Ground"))  //������߼�������layer�ж��Ƿ��ڵ�����
            {
                grounded = hit.transform.position.y < (transform.position.y - 0.5f);   //���ﱣ֤����yλ��С��playerλ���Ա�֤����ײ��ͷ�ϵĵ���,-0.5f����Ϊ����µĽ��ڽ�����

                Physics2D.IgnoreCollision(collider, results[i], !grounded);  //����������ʱ����Ժ͵������ײ��Ҳ����˵���Ժ�����һ��������ײ���
            }else if(hit.layer == LayerMask.NameToLayer("Ladder")) //����Ƿ���¥��
            {
                climbing = true;
            }
        }
    }

    private void Update()  //updata ÿ֡������
    {
        CheakCollision();
        
        if (climbing)  //����Ҫ���ж��Ƿ�����������ȥ�ж���
        {
            direction.y = Input.GetAxis("Vertical")* MoveSpeed;
        }

        else if (grounded && Input.GetKeyDown(KeyCode.Space))  //�������õĿո���ʵ����ֱ����jump�������ˮƽ��ֱ��������unity����Ĭ�ϵ�,
        {
            direction = Vector2.up * jumpStrengh;
        }
        else
        {
            direction += Physics2D.gravity * Time.deltaTime;  //Physics2D.gravity��Ĭ��ֵ��y�ϵ�-9.81
        }

        direction.x = Input.GetAxis("Horizontal")*MoveSpeed; //ˮƽ

        if (grounded)
        {
            direction.y = Mathf.Max(direction.y, -1f);
        }
         

        if(direction.x > 0f)
        {
            transform.eulerAngles = Vector3.zero; //������ң����򲻱�
        }else if(direction.x < 0f)
        {
            transform.eulerAngles = new Vector3(0f, 180f ,0f);//�������yת180�㣨����ԭʼ�ز������ҵģ�
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if (climbing)  //����
        {
            spriteRenderer.sprite = climbSprite;
        }
        else if (direction.x != 0f)  //�ܶ�����,ֻ�����к����ƶ�ʱ�ű�����
        {
            spriteIndex++;
            if (spriteIndex >= runSprites.Length)  //�ڶ��������ﷴ��ѭ��
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
            enabled = false; //�ж�ʱ�����ƶ�
            FindAnyObjectByType<GameManager>().LevelComplete(); //�������������Ϸ
        }else if (collision.gameObject.CompareTag("Obstacle"))
        {
            enabled = false;
            FindAnyObjectByType<GameManager>().LevelFailed(); //����ľͰ���߽�ս���youxi
        }
    }

}


