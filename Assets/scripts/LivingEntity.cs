using UnityEngine;
using UnityEngine.Events;

public class LivingEntity : MonoBehaviour
{

    public UnityEvent<float, float> onEntityHealthChanged;
    public UnityEvent onEntityDie;
    public float speed;
    public float damage;
    public float maxHealth;
    public float hurtCD;
    public float attackCD;
    public string PreFab;

    protected float currentHealth;
    protected Animator animator;
    protected float HurtTimer;
    protected float attackTimer;
    protected Rigidbody2D selfRb;


    protected virtual void Start()
    {

        selfRb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
    }

    protected virtual void Update()
    {
    }

    protected virtual void FixedUpdate()
    {

        HurtTimer -= Time.deltaTime;
        attackTimer -= Time.deltaTime;
    }

    public float getHealth()
    {

        return currentHealth;
    }

    public virtual float getDamage()
    {
        
        return damage;
    }

    public virtual void hurt(float _damage)
    {
        // print("hurt");
        if (HurtTimer > 0) return;
        currentHealth -= _damage;
        onEntityHealthChanged?.Invoke(currentHealth, _damage);
        HurtTimer = hurtCD;

        if (currentHealth <= 0)
        {
            onEntityDie?.Invoke();

        }
    }

    public virtual void setAttack(float attack)
    {
        
        damage = attack;
    }

    public virtual void setSpeed(float speed)
    {
        
        this.speed = speed;
    }

}