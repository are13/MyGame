using UnityEngine;

public class cirnoBullet2 : Bullet
{

    public Transform enemyTransform;

    // enemy e;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {

        // e = FindAnyObjectByType<enemy_cirno>();
        base.Start();

    }

    public void shoot(Vector2 direction)
    {
        
        GetComponent<Rigidbody2D>().linearVelocity = direction.normalized * speed;
    }

    // Update is called once per frame


    void Update()
    {


    }
}
