using UnityEngine;
public class cirnoPill1 : Bullet
{
    public Transform enemyTransform;
    public Transform playerTransform;
    
    Player player;
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        
        owner = FindFirstObjectByType<enemy_cirno>();
        player = FindFirstObjectByType<Player>();
        if(player != null && owner != null)
        {
            enemyTransform = owner.transform;
            playerTransform = player.transform;
            Fire();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public void Fire()
    {
        if (enemyTransform == null || playerTransform == null) return;
        Vector2 direction = (Vector2)(playerTransform.position - transform.position);
        if (direction.sqrMagnitude < 0.000001f) direction = Vector2.up;
        direction.Normalize();
        direction*=speed;

        GetComponent<Rigidbody2D>().linearVelocity = direction;
    }
}