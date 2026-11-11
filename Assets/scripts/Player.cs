using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class Player : LivingEntity
{

    public float chargeSpeed;
    public UnityEvent<List<ItemStack>, List<int>> onPlayerInventoryChange;
    public UnityEvent<Player, Vector2> onPlayerSelectingOptions;
    public float chargeCD;
    public float attackRange = 1.5f;
    public Inventory inventoryUI;
    public int inventorySize;
    public GameObject healthBar;
    public TitleInfo titleInfo;
    public TitleInfo globalInfo;
    public Prompt globalPrompt;
    public TextMeshProUGUI speedText;
    public TextMeshProUGUI chargeText;
    public TextMeshProUGUI attackText;
    public int lookingOptions = 0;
    public Prompt prompt;
    public InfoText infoText;
    public Option dieOption;
    public bool hasDie = false;
    List<ItemStack> inventory;
    int idxSelected = 0;
    float chargeTimer;
    bool isChargingLogical = false;
    bool isOpeningInventory = false;
    bool isLookingInfo = false;
    Vector2 move;
    Animator playerAnime;
    static readonly int isWalking = Animator.StringToHash("isWalking");
    static readonly int isAttacking = Animator.StringToHash("isAttacking");
    static readonly int isCharging = Animator.StringToHash("isCharging");
    // Vector2 selecting = new Vector2();

    void Awake()
    {
        
        DontDestroyOnLoad(transform.parent.gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {

        base.Start();
        playerAnime = GetComponent<Animator>();
        inventory = new List<ItemStack>();
        for (int i = 0; i < inventorySize; i++)
        {
            inventory.Add(null);
        }

        //UI
        healthBar.GetComponent<HealthBarUIController>().init(maxHealth, currentHealth);
        inventoryUI.init(inventorySize);
        attackText.text = "Attack : " + damage;
        speedText.text = "Speed : " + speed;
        chargeText.text = "Charge Speed : " + chargeSpeed;
        // prompt = new Prompt();

    }

    // Update is called once per frame
    protected override void Update()
    {

        if (Keyboard.current.eKey.wasReleasedThisFrame)
        {
            isOpeningInventory = !isOpeningInventory;
        }

        idxSelected = Mathf.Clamp(idxSelected + (Keyboard.current.rightArrowKey.wasPressedThisFrame ? 1 : 0) - (Keyboard.current.leftArrowKey.wasPressedThisFrame ? 1 : 0), 0, isOpeningInventory ? inventorySize - 1 : inventoryUI.hotBarSize - 1);
        if (isOpeningInventory)
        {

            int temp = idxSelected + ((Keyboard.current.upArrowKey.wasPressedThisFrame ? 1 : 0) - (Keyboard.current.downArrowKey.wasPressedThisFrame ? 1 : 0)) * inventoryUI.hotBarSize;
            idxSelected = (temp >= inventorySize || temp < 0) ? idxSelected : temp;
        }
        inventoryUI.updateSelected(idxSelected);

        for (int i = 0; i < inventoryUI.hotBarSize; i++)
        {
            if (Keyboard.current[Key.Digit1 + i].wasPressedThisFrame)
            {

                ItemStack temp = inventory[idxSelected];
                inventory[idxSelected] = inventory[i];
                inventory[i] = temp;
                // inventoryUI.updateDisplay(inventory,null);
                onPlayerInventoryChange?.Invoke(inventory, new List<int> { idxSelected, i });
                break;
            }
        }

        if (isOpeningInventory)
        {

            inventoryUI.gameObject.SetActive(true);
        }
        else inventoryUI.gameObject.SetActive(false);
        // print(dtBtwHurt);
        if (move.magnitude > 1e-3)
        {
            playerAnime.SetBool(isWalking, true);

            if (Keyboard.current.aKey.isPressed) transform.eulerAngles = new Vector3(0, 180, 0);
            else if (Keyboard.current.dKey.isPressed) transform.eulerAngles = new Vector3(0, 0, 0);
            // else if (Keyboard.current.upArrowKey.isPressed) transform.eulerAngles = new Vector3(0, 90, 0);
            // else if (Keyboard.current.downArrowKey.isPressed) transform.eulerAngles = new Vector3(0, -90, 0);
        }
        else playerAnime.SetBool(isWalking, false);

        if (Mouse.current.rightButton.isPressed)
        {

            playerAnime.SetBool(isCharging, true);
        }

        if (inventory[idxSelected] != null && inventory[idxSelected].amount <= 0) inventory[idxSelected] = null;

        // print(lookingOptions);
        if (lookingOptions > 0)
        {

            prompt.gameObject.SetActive(true);
            prompt.updateSelected((Keyboard.current.jKey.wasPressedThisFrame ? 1 : 0) - (Keyboard.current.uKey.wasPressedThisFrame ? 1 : 0));
            if (Keyboard.current.bKey.wasPressedThisFrame)
            {

                prompt.execute(this);
            }
        }
        else prompt.gameObject.SetActive(false);

        if (Keyboard.current.rKey.wasPressedThisFrame) isLookingInfo = !isLookingInfo;
        infoText.setActive(isLookingInfo);
    }

    protected override void FixedUpdate()
    {

        base.FixedUpdate();
        // print(player.linearVelocity);
        move.x = ((Keyboard.current.dKey.isPressed) ? 1 : 0) - ((Keyboard.current.aKey.isPressed) ? 1 : 0);
        move.y = ((Keyboard.current.wKey.isPressed) ? 1 : 0) - ((Keyboard.current.sKey.isPressed) ? 1 : 0);



        if (chargeTimer <= 0)
        {

            // isChargingLogical = false;
            if (Mouse.current.rightButton.isPressed)
            {

                if (inventory[idxSelected] != null && inventory[idxSelected].item is IUsable usable)
                {

                    usable.use(this, inventory[idxSelected]);
                    inventory[idxSelected].amount--;
                    if (inventory[idxSelected].amount < 0) inventory[idxSelected] = null;
                    onPlayerInventoryChange?.Invoke(inventory, new List<int> { idxSelected });
                }
                else
                {
                    charge();
                    chargeTimer = chargeCD;
                }

            }
        }
        else chargeTimer -= Time.deltaTime;

        if (attackTimer <= 0 && Mouse.current.leftButton.isPressed)
        {

            attack();
            attackTimer = attackCD;
        }

        if (!isChargingLogical)
        {

            if (move.magnitude > 1e-2)
            {

                move.Normalize();
                move *= speed;

                selfRb.linearVelocity = move;
            }
            else selfRb.linearVelocity = Vector2.zero;

        }

        // selecting.x = (Keyboard.current.kKey.wasPressedThisFrame ? 1 : 0) - (Keyboard.current.hKey.wasPressedThisFrame ? 1 : 0);
        // selecting.y = 
        // if (selecting.magnitude > 1e-2) onPlayerSelectingOptions?.Invoke(this, selecting);

    }

    void attack()
    {

        playerAnime.SetBool(isAttacking, true);
        Collider2D[] result = Physics2D.OverlapCircleAll(transform.position, attackRange);

        foreach (Collider2D target in result)
        {

            if (target.gameObject.TryGetComponent<IAttackAble>(out var attackAble))
            {
                if (attackAble != null)
                {

                    attackAble.attack(this, damage);
                }
            }
        }
    }

    void charge()
    {

        selfRb.linearVelocity = move.normalized * chargeSpeed;
        isChargingLogical = true;
    }

    public void endAttack()
    {

        playerAnime.SetBool(isAttacking, false);
    }

    public void endCharge()
    {

        playerAnime.SetBool(isCharging, false);
        isChargingLogical = false;
        // player.linearVelocity -= player.linearVelocity.normalized*chargeSpeed;
    }

    void OnTriggerEnter2D(Collider2D collision)
    {


        if (collision.gameObject.CompareTag("item"))
        {
            ItemData temp = collision.gameObject.GetComponent<ItemData>();
            if (temp.s != null)
            {
                if(pickItem(temp.s)) Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.CompareTag("NPC"))
        {

            if (collision.gameObject.GetComponent<NPCEntity>() is IInteractable interactable)
            {
                print("entered");
                prompt.addOptions(interactable.interact(this));
                lookingOptions++;
                prompt.updateSelected(0);
            }
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.CompareTag("NPC"))
        {
            print("entered");
            if (collision.gameObject.GetComponent<NPCEntity>() is IInteractable interactable)
            {

                prompt.removeOptions(interactable.interact(this));
                lookingOptions--;
            }
        }
    }

    public override void hurt(float _damage)
    {

        // print(_damage);
        base.hurt(_damage);

        if (currentHealth <= 0)
        {

            // Destroy(gameObject);
            gameObject.SetActive(false);
            hasDie = true;
        }
    }

    public override void setSpeed(float speed)
    {
        base.setSpeed(speed);
        speedText.text = "Speed : " + speed;
    }

    public override void setAttack(float attack)
    {
        base.setAttack(attack);
        attackText.text = "Attack : " + damage;
    }

    public void setChargeSpeed(float chargeSpeed)
    {

        this.chargeSpeed = chargeSpeed;
        chargeText.text = "Speed : " + chargeSpeed;
    }

    public void addItem(ItemStack itemStack)
    {

        pickItem(itemStack);
    }

    public bool pickItem(ItemStack itemStack)
    {

        bool isAllPicked = false;
        List<int> firstIn = new List<int>();
        for (int i = 0; i < inventorySize; i++)
        {
            if (inventory[i] == null)
            {

                inventory[i] = itemStack;
                firstIn.Add(i);
                isAllPicked = true;
                firstIn.Add(i);
                break;
            }
            else if (itemStack.item.Equals(inventory[i].item))
            {

                int added = inventory[i].amount + itemStack.amount;
                if (added > inventory[i].item.maxStackNum)
                {

                    int d = inventory[i].item.maxStackNum - inventory[i].amount;
                    if (d > 0) firstIn.Add(i);
                    itemStack.amount -= d;
                    inventory[i].amount = inventory[i].item.maxStackNum;
                }
                else
                {
                    inventory[i].amount += itemStack.amount;
                    isAllPicked = true;
                    firstIn.Add(i);
                    break;
                }
            }
        }

        if (firstIn.Count > 0)
        {

            onPlayerInventoryChange?.Invoke(inventory, firstIn);
            // inventoryUI.updateDisplay(inventory,firstIn);
        }

        return isAllPicked;
    }

    public void restart()
    {
        hasDie = false;
        currentHealth = maxHealth;
        setHealth(maxHealth);
        transform.position = Vector2.zero;
    }

    public void dieInfo()
    {
        
        print(globalInfo);
        globalInfo.setShow("You Died.",-1);
        // globalPrompt.addOptions(new List<Option>{new Option().init()});

        globalPrompt.addOptions(new List<Option>{dieOption.init((p) =>
        {
            
            p.restart();
            p.globalPrompt.removeAll();
            p.globalInfo.endShow();
        },"Restart")});
        
    }
}
