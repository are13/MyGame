

using UnityEngine;

public abstract class Bullet : MonoBehaviour
{
    public GameObject pillPrefab;
    public GameObject ownerObj;
    public LivingEntity owner;
    public float speed = 5f;
    public float baseDamage = 20;
    public float lifeTime = 2f;

    protected virtual void Start()
    {

        owner = ownerObj.GetComponent<LivingEntity>();
    }

    void FixedUpdate()
    {

        lifeTime -= Time.deltaTime;

        // print(lifeTime);
        if (lifeTime <= 0) Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // print(collision.gameObject.GetComponent<Player>());
        if (collision.gameObject.CompareTag("Player"))
        {

            collision.gameObject.GetComponent<Player>().hurt(baseDamage * owner.getDamage());
            Destroy(gameObject);
        }
    }

    // public virtual void OnTriggerEnter2D(Collision2D collision2D)
    // {

    //     
    // }

}