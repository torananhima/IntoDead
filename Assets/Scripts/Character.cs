using Unity.VisualScripting;
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
    protected float currentVelocity;
    [SerializeField] protected MovementType movementState;
    
    [Header("Audio")]
    [SerializeField] protected AudioData footstepAudio;
    [SerializeField] protected AudioData hurtAudio;
    [SerializeField] protected AudioData deathAudio;
    [SerializeField] protected float footstepRatio = 1f;
    protected float footstepProgress;
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
        get => _currentHp;
        protected set
        {
            _currentHp = Mathf.Clamp(value, 0, maxHp);

            if (_currentHp <= 0)
                Dead();
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
        if (!isDead) SoundManager.Instance.PlaySFX(deathAudio, transform.position);
    }

    public virtual void TakeDamage(int damageTaken)
    {
        Debug.Log(gameObject.name + " takes " + damageTaken + " damages");
        if (isDead || damageTaken <= 0)
            return;

        hp -= damageTaken;
        if (!isDead) SoundManager.Instance.PlaySFX(hurtAudio, transform.position);
    }

    public virtual void Heal(int healAmount)
    {
        if (isDead || healAmount <= 0)
            return;

        hp += healAmount;
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

    protected virtual void UpdateFootstepProgress()
    {
        footstepProgress += footstepRatio * currentVelocity;
        if(footstepProgress >= 1)
        {
            footstepProgress = 0;
            PlayFootstepSound();
        }
    }

    protected virtual void PlayFootstepSound()
    {
        SoundManager.Instance.PlaySFX(footstepAudio, transform.position);
    }
}