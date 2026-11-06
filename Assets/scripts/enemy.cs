using UnityEngine;
using UnityEngine.Events;

public abstract class enemy : LivingEntity
{
    public GameObject playerObject;
    public UnityEvent onEnemyDie;
    public GameObject healthBar;
    protected Player player;

    protected static readonly int isWalking = Animator.StringToHash("isWalking");
    protected static readonly int hasDie = Animator.StringToHash("hasDie");
    protected static readonly int isAttacking = Animator.StringToHash("isAttacking");


    protected override void Start()
    {
        
        base.Start();
        playerObject = GameObject.FindGameObjectWithTag("Player");
        player = playerObject.transform.GetComponent<Player>();
        animator = GetComponent<Animator>();

        //UI
        healthBar.GetComponent<HealthBarUIController>().init(maxHealth,currentHealth);
    }

    // Update is called once per frame
    protected override void Update()
    {
        
    }

    protected override void FixedUpdate()
    {
        
        base.FixedUpdate();
    }

    public virtual void attack()
    {
        

    }

    public override void hurt(float damage)
    {
        
        base.hurt(damage);

        if(currentHealth <= 0)
        {
            
            animator.SetBool(hasDie,true);
        }
    }

    public virtual void trackPlayer(){}
}
