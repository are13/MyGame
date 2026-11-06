using UnityEngine;
public class slime : MonoBehaviour
{
    public float health;
    public float damagePhit;
    public float speed;
    public float jumpingAbility;
    public Transform playerTransform;
    [Header("Animation")]
    [SerializeField, Min(0f)] float landingWait = 0.8f;    // 落地后等待多久再起跳，给 jump -> idle 的过渡留出时间
    [SerializeField, Min(0f)] float attackHoldTime = 1.5f; // hasEnterPlayer 最长保持 true 的保险时间（正常情况下攻击播完就会收回）
    [SerializeField, Min(0.1f)] float patrolTurnTime = 2f; // 巡逻时每隔多久换一次方向
    // 动画参数用哈希，避免每帧拼字符串
    static readonly int IsJumpingHash = Animator.StringToHash("isJumping");
    static readonly int IsMovingHash = Animator.StringToHash("isMoving");
    static readonly int HasEnterPlayerHash = Animator.StringToHash("hasEnterPlayer");
    static readonly int AttackStateHash = Animator.StringToHash("attack");
    bool isGrounded;
    bool facingRight = true;
    float currentHp;
    float groundedTimer;
    float attackTimer;
    float patrolTimer;
    float findPlayerTimer;
    Rigidbody2D self;
    SpriteRenderer spriteRenderer;
    Player player;
    Animator anime;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHp = health;
        self = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        anime = GetComponent<Animator>();
        if (playerTransform != null) player = playerTransform.GetComponent<Player>();
        FindPlayer();
        // enemySlime 控制器里三个 bool 参数的默认值都是 true，不显式清零会一进场就切到 attack / jump
        anime.SetBool(IsJumpingHash, false);
        anime.SetBool(IsMovingHash, false);
        anime.SetBool(HasEnterPlayerHash, false);
        patrolTimer = patrolTurnTime;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateAttackTimer();
        if (isGrounded) groundedTimer += Time.deltaTime;
        if (player == null)
        {
            // 场上暂时没有玩家时，每隔一会儿再找一次（玩家可能比史莱姆晚生成）
            findPlayerTimer -= Time.deltaTime;
            if (findPlayerTimer <= 0f)
            {
                findPlayerTimer = 0.5f;
                FindPlayer();
            }
            patrol();
        }
        else trackPlayer();
    }
    void FindPlayer()
    {
        if (player != null) return;
        if (playerTransform != null)
        {
            player = playerTransform.GetComponent<Player>();
            if (player != null) return;
        }
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        if (playerObject == null) return;
        if (playerTransform == null) playerTransform = playerObject.transform;
        player = playerObject.GetComponent<Player>();
    }
    void UpdateAttackTimer()
    {
        if (attackTimer <= 0f) return;
        // 攻击动画已经播到尾声才收回参数，这样不管过渡有没有立刻触发，attack 都能完整播出来
        AnimatorStateInfo state = anime.GetCurrentAnimatorStateInfo(0);
        if (state.shortNameHash == AttackStateHash && state.normalizedTime >= 0.9f)
        {
            attackTimer = 0f;
            anime.SetBool(HasEnterPlayerHash, false);
            return;
        }
        attackTimer -= Time.deltaTime;
        if (attackTimer > 0f) return;
        // 兜底：万一没切进 attack，也不能让参数一直卡在 true，否则后面所有过渡都会失效
        anime.SetBool(HasEnterPlayerHash, false);
    }
    void trackPlayer()
    {
        float offsetX = player.transform.position.x - transform.position.x;
        if (Mathf.Abs(offsetX) > 0.05f) SetFacing(offsetX > 0f);
        // 追逐时不播 move 动画：控制器里 move 只有一条回 idle 的过渡，一直开着 isMoving 会把 jump / attack 全挡住
        anime.SetBool(IsMovingHash, false);
        if (isGrounded && groundedTimer >= landingWait) Jump();
    }
    void Jump()
    {
        isGrounded = false;
        groundedTimer = 0f;
        Vector2 velocity = self.linearVelocity;
        velocity.x = (facingRight ? 1f : -1f) * speed;
        velocity.y = jumpingAbility;
        self.linearVelocity = velocity;
        anime.SetBool(IsJumpingHash, true);
    }
    void patrol()
    {
        patrolTimer -= Time.deltaTime;
        if (patrolTimer <= 0f)
        {
            SetFacing(!facingRight);
            patrolTimer = patrolTurnTime;
        }
        // move -> idle 的过渡只在 isMoving 为 false 时成立，巡逻时必须真的把它保持为 true
        anime.SetBool(IsMovingHash, true);
        Vector2 velocity = self.linearVelocity;
        velocity.x = (facingRight ? 1f : -1f) * speed * 0.5f;
        self.linearVelocity = velocity;
    }
    void SetFacing(bool faceRight)
    {
        if (facingRight == faceRight) return;
        facingRight = faceRight;
        // 用 SpriteRenderer.flipX 翻转精灵：Rigidbody2D 每个物理步都会把 transform 的旋转同步回去，
        // 直接改 transform.eulerAngles 的 Y 轴会被冲掉（原来的实现还误改过 transform.position）
        if (spriteRenderer != null) spriteRenderer.flipX = !faceRight;
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // 攻击：让 hasEnterPlayer 保持一段时间，控制器才有时间从 idle / jump 过渡到 attack
            anime.SetBool(HasEnterPlayerHash, true);
            attackTimer = attackHoldTime;
            if (player == null) player = collision.gameObject.GetComponent<Player>();
            if (player != null) player.hurt(damagePhit);
        }
        else if (collision.gameObject.CompareTag("ground"))
        {
            isGrounded = true;
            groundedTimer = 0f;
            // 落地必须关掉 isJumping，否则 jump 状态永远回不到 idle
            anime.SetBool(IsJumpingHash, false);
        }
    }
    void hurt(float _damage)
    {
        currentHp -= _damage;
    }
}