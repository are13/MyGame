

using UnityEngine;

public class cirnoBullet3 : Bullet
{

    public float scale;
    public static float dt = 25 * Mathf.Deg2Rad;
    public Rigidbody2D rb;

    protected override void Start()
    {
        base.Start();
        rb = GetComponent<Rigidbody2D>();
        lifeTime = 3000;
    }

    void FixedUpdate()
    {
        if (scale == 0) return;

        // print(rb.rotation);
        if (scale < 0) Destroy(gameObject);
        else
        {
            scale -= dt;
            rb.SetRotation(rb.rotation + dt);
        }
    }


    public void spin(float scale = 120)
    {
        this.scale = scale * Mathf.Deg2Rad;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        
    }

    void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("Player"))
        {
            
            collision.GetComponent<Player>().hurt(baseDamage);
        }
    }
}