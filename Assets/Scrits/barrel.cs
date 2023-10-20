using UnityEngine;

public class barrel : MonoBehaviour
{
    private new Rigidbody2D  rigidbody;
    public float speed = 1f;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.layer ==  LayerMask.NameToLayer("Ground"))
        {
            rigidbody.AddForce(collision.transform.right * speed , ForceMode2D.Impulse); //这里有个细节，为了代码一致性，施加向右的力都是沿着y轴方向的
                                                                                         //所以在场景搭建的时候一定要看好平台的坐标方向，部分y轴要旋转180°
                                                                                         // ForceMode2D.Impulse:冲击波
        }
    }




}
