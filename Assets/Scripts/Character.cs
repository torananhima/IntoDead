using UnityEngine;

public enum MovementType
{
    Walk,
    Sprint,
    Crouch
}
public class Character : MonoBehaviour
{
    [Header("Basic Character")]
    [SerializeField] protected int _maxHp = 100;
    protected int _currentHp;
    [SerializeField] protected float _walkSpeed = 10f;
    [SerializeField] protected float _runSpeed = 15f;
    [SerializeField] protected float _crouchSpeed = 5f;
    [SerializeField] protected float changeSpeedRatio = 5f;
    protected float newSpeed;
    [SerializeField] protected MovementType movementState;
    public int maxHp
    {
        get
        {
            return _maxHp;
        }
        protected set { _maxHp = value; }
    }
    public int hp
    {
        get
        {
            return _currentHp;
        }
        protected set
        {
            if (value > maxHp)
            {
                value = maxHp;
            }
            else if (value <= 0)
            {
                value = 0;
                Dead();
            }
            _currentHp = value;
        }
    }
    public float walkSpeed
    {
        get
        {
            return _walkSpeed;
        }
        protected set { _walkSpeed = value; }
    }
    public float runSpeed
    {
        get
        {
            return _runSpeed;
        }
        protected set { _runSpeed = value; }
    }
    public float crouchSpeed
    {
        get
        {
            return _crouchSpeed;
        }
        protected set { _crouchSpeed = value; }
    }
    public float moveSpeed
    {
        get;
        protected set;
    }
    public bool isSprinting;
    public bool isCrouching;
    public bool isDead;

    protected virtual void Start()
    {
        _currentHp = maxHp;
        isDead = false;
    }

    protected virtual void Update()
    {
        moveSpeed = Mathf.Lerp(moveSpeed, newSpeed, Time.deltaTime * changeSpeedRatio);
    }

    protected virtual void Dead()
    {
        if (isDead) return;
        isDead = true;
        Debug.Log(gameObject.name + " is dead.");
    }

    public virtual void TakeDamage(int damageTaken)
    {
        _currentHp -= damageTaken;
    }

    public virtual void Heal(int healAmount)
    {
        _currentHp += healAmount;
    }
    public virtual void ChangeMovement(MovementType newMovement)
    {
        switch (newMovement)
        {
            case MovementType.Walk:
                newSpeed = walkSpeed;
                break;
            case MovementType.Sprint:
                newSpeed = runSpeed;
                break;
            case MovementType.Crouch:
                newSpeed = crouchSpeed;
                break;
        }
    }
}