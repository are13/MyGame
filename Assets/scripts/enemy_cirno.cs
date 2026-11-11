using System.Collections.Generic;
using NUnit.Framework;
using UnityEditor.Rendering;
using UnityEngine;

public class enemy_cirno : enemy, IAttackAble
{

    public GameObject[] pillTypes;
    public float spawnHeight = 1.5f;
    public float walkCD = 2f;
    public float dirGap = 10;
    GameObject isReleasingIce;
    [SerializeField]
    public List<LootData> LootList = new List<LootData>();
    float walkTimer;
    Rigidbody2D cirnoRb;

    void Awake()
    {

    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        base.Start();
        attackTimer = attackCD;
        cirnoRb = GetComponent<Rigidbody2D>();
        // print(cirnoRb == null);

        LootList.Add(new LootData(Items.ICED_FROG, 1, 1, 3));
        LootList.Add(new LootData(Items.SCROLL_SPEED, 1, 1, 1));
    }

    // Update is called once per frame
    protected override void Update()
    {

        if (cirnoRb.linearVelocity.magnitude < 1e-3) animator.SetBool(isWalking, false);
        else animator.SetBool(isWalking, true);

        if (cirnoRb.linearVelocityX > 0) transform.eulerAngles = new Vector3(0, 180, 0);
        else transform.eulerAngles = new Vector3(0, 0, 0);

    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
        if (playerObject != null && attackTimer <= 0)
        {

            // print("aaaaa");
            animator.SetBool(isAttacking, true);
            attackTimer = attackCD;
            cirnoRb.linearVelocity = Vector2.zero;
            // print(attackTimer);
        }

        if (isReleasingIce == null)
        {
            cirnoRb.constraints = RigidbodyConstraints2D.None;
            if (walkTimer <= 0 && Random.Range(0, 100) <= 60)
            {
                walk();
                walkTimer = walkCD;
            }
            else walkTimer -= Time.deltaTime;
        }

    }



    public override void attack()
    {

        print(player == null);
        print(currentHealth);
        if (player != null && isReleasingIce == null && currentHealth > 0)
        {

            int type = Random.Range(0, 3);
            switch (type)
            {

                case 0:
                    {
                        Instantiate(pillTypes[0], new Vector3(transform.position.x, transform.position.y + spawnHeight, transform.position.z), Quaternion.identity);
                        break;
                    }
                case 1:
                    {
                        float dangle = 360f / 20f;
                        for (float angle = 360; angle >= 0; angle -= dangle)
                        {

                            float rad = Mathf.Deg2Rad * angle;
                            GameObject generated = Instantiate(pillTypes[1], new Vector3(transform.position.x, transform.position.y + spawnHeight, transform.position.z), Quaternion.identity);
                            generated.GetComponent<cirnoBullet2>().shoot(new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)));
                        }
                        break;
                    }
                case 2:
                    {
                        Vector2 temp = transform.position - player.transform.position;
                        print(Mathf.Rad2Deg * Mathf.Atan2(temp.y, temp.x));
                        isReleasingIce = Instantiate(pillTypes[2], transform.position, Quaternion.Euler( 0,0, Mathf.Rad2Deg * Mathf.Atan2(temp.y, temp.x)));
                        cirnoBullet3 bullet = isReleasingIce.GetComponent<cirnoBullet3>();
                        bullet.spin(Random.Range(0f, 8196f));
                        cirnoRb.constraints = RigidbodyConstraints2D.FreezePosition;
                        cirnoRb.linearVelocity = Vector2.zero;
                        break;
                    }
            }
            print(type);
            animator.SetBool(isAttacking, false);
        }
    }

    public void walk()
    {

        if (player != null)
        {
            float distance = (player.transform.position - transform.position).magnitude;
            Vector3 dir = (player.transform.position - transform.position).normalized;

            cirnoRb.linearVelocity += (Vector2)dir * speed * ((distance <= dirGap) ? 1f : -1f);
        }
    }

    public override void hurt(float damage)
    {
        if (isReleasingIce != null) return;
        base.hurt(damage);
        if (currentHealth <= 0)
        {

            LootData data = LootList[Random.Range(0, LootList.Count)];
            // LootData data = LootList[1];

            if (Random.Range(0f, 1f) < data.chance)
            {

                // print("loot");
                // GameObject loot = Instantiate(Resources.Load<GameObject>(data.item.preFab), transform.position, Quaternion.identity);
                GameObject loot = Instantiate(data.item.preFab, transform.position, Quaternion.identity);
                ItemData itemData = loot.GetComponent<ItemData>();
                itemData.create(new ItemStack(data.item, Random.Range(data.minAmount, data.maxAmount)));
            }
            Destroy(gameObject,1);
        }
    }

    public void attack(LivingEntity attacker, float damage)
    {

        hurt(damage);
    }
}
