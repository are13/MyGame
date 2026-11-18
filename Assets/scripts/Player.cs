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
    // public Prompt prompt;
    // public InfoText infoText;
    public Option dieOption;
    public bool hasDie = false;
    List<ItemStack> inventory;
    List<IInteractable> nearBy;
    int idxSelected = 0;
    float chargeTimer;
    bool isChargingLogical = false;
    bool isOpeningInventory = false;
    bool isLookingInfo = false;
    bool isInteracting = false;
    state interactingType = state.NONE;
    bool isImmune = false;
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
        nearBy = new List<IInteractable>();
        // if (prompt == null) prompt = GetComponentInChildren<Prompt>(true);
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


        if (Keyboard.current.bKey.wasPressedThisFrame)
        {

            if (isInteracting)
            {
                switch (interactingType)
                {
                    case state.DIALOG:
                        {

                            GameManager.manager.dialogManager.updateNext();
                            break;
                        }
                    case state.CONTAINER:
                        {
                            GameManager.manager.containerManager.close();
                            break;
                        }
                }
            }
            else
            {

                if (nearBy.Count > 0)
                {
                    if (Keyboard.current.bKey.wasPressedThisFrame)
                    {
                        GameManager.manager.promptManager.execute();
                    }
                }
            }
        }



        // if (GameManager.manager.isOpenAnyContainer)
        // {

        //     if (Keyboard.current.bKey.wasPressedThisFrame)
        //     {
        //         print("close");
        //         GameManager.manager.containerManager.close();
        //     }
        // }

        if (Keyboard.current.eKey.wasReleasedThisFrame)
        {
            isOpeningInventory = !isOpeningInventory;
        }

        idxSelected = Mathf.Clamp(idxSelected + (Keyboard.current.rightArrowKey.wasPressedThisFrame ? 1 : 0) - (Keyboard.current.leftArrowKey.wasPressedThisFrame ? 1 : 0), 0, isOpeningInventory ? inventorySize - 1 : inventoryUI.hotBarSize - 1);
        if (isOpeningInventory)
        {

            inventoryUI.gameObject.SetActive(true);
            int temp = idxSelected + ((Keyboard.current.upArrowKey.wasPressedThisFrame ? 1 : 0) - (Keyboard.current.downArrowKey.wasPressedThisFrame ? 1 : 0)) * inventoryUI.hotBarSize;
            idxSelected = (temp >= inventorySize || temp < 0) ? idxSelected : temp;

        }
        else inventoryUI.gameObject.SetActive(false);
        inventoryUI.updateSelected(idxSelected);

        if (GameManager.manager.isFrozen)
        {
            return;
        }

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

        if (!isInteracting && Mouse.current.rightButton.isPressed)
        {

            playerAnime.SetBool(isCharging, true);
        }

        if (inventory[idxSelected] != null && inventory[idxSelected].amount <= 0) inventory[idxSelected] = null;



        if (Keyboard.current.rKey.wasPressedThisFrame) isLookingInfo = !isLookingInfo;
    }

    protected override void FixedUpdate()
    {

        if (GameManager.manager.isFrozen)
        {
            return;
        }
        base.FixedUpdate();
        // print(player.linearVelocity);

        if (isInteracting)
        {
            move = Vector2.zero;
            selfRb.linearVelocity = Vector2.zero;
            playerAnime.SetBool(isWalking, false);
            playerAnime.SetBool(isCharging, false);
            playerAnime.SetBool(isAttacking, false);
            isChargingLogical = false;
            chargeTimer = 0f;
            return;
        }

        moves();

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

    void moves()
    {

        if (isChargingLogical || isInteracting) return;

        move.x = ((Keyboard.current.dKey.isPressed) ? 1 : 0) - ((Keyboard.current.aKey.isPressed) ? 1 : 0);
        move.y = ((Keyboard.current.wKey.isPressed) ? 1 : 0) - ((Keyboard.current.sKey.isPressed) ? 1 : 0);

        if (move.magnitude > 1e-2)
        {

            move.Normalize();
            move *= speed;

            selfRb.linearVelocity = move;
        }
        else selfRb.linearVelocity = Vector2.zero;

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
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("item"))
        {
            ItemData temp = collision.gameObject.GetComponent<ItemData>();
            if (temp.s != null)
            {
                if (pickItem(temp.s)) Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.TryGetComponent<IInteractable>(out var interactable))
        {
            print($"{collision.tag} entered.");
            nearBy.Add(interactable);
            GameManager.manager.promptManager.updateNearBy(nearBy);


        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {

        if (collision.gameObject.TryGetComponent<IInteractable>(out var interactable))
        {

            nearBy.Remove(interactable);
            GameManager.manager.promptManager.updateNearBy(nearBy);
        }
    }

    public override void hurt(float _damage)
    {

        // print(_damage);
        if (isImmune) return;
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

    public List<ItemStack> GetInventory()
    {

        return inventory;
    }

    public void removeItemWithoutDestory(int idx)
    {

        inventory.RemoveAt(idx);
        onPlayerInventoryChange?.Invoke(inventory, new List<int> { idx });
    }

    public void dieInfo()
    {

        print(globalInfo);
        globalInfo.setShow("You Died.", -1);
        // globalPrompt.addOptions(new List<Option>{new Option().init()});

        // globalPrompt.addOptions(new List<Option>{dieOption.init((p) =>
        // {

        //     p.restart();
        //     p.globalPrompt.removeAll();
        //     p.globalInfo.endShow();
        // },"Restart")});

    }

    public void inDialog()
    {
        isInteracting = true;
        interactingType = state.DIALOG;
    }
    public void outDialog()
    {
        isInteracting = false;
        interactingType = state.DIALOG;
    }

    public void setInteracting(state s)
    {
        isInteracting = s != state.NONE;
        interactingType = s;
    }
}

public enum state
{

    DIALOG, CONTAINER, NONE
}