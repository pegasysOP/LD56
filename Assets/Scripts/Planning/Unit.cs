using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public Animator animator;

    [Header("Bars")]
    public CanvasGroup hudCanvas;
    public Slider healthSlider;
    public Image healthSliderFill;
    public Slider specialSlider;
    public Color playerHealthColor;
    public Color enemyHealthColor;

    [Header("Status")]
    public CanvasGroup statusCanvas;
    public GameObject confusedIcon;
    public GameObject slowedIcon;


    [Header("Projectiles")]
    public Transform projectileSource;
    public Projectile projectilePrefab;

    [Space(12)] 
    
    public Vector3 previousPosition; 
    public UnitType unitType;

    private bool dead = false;
    private bool moving = false;

    private string movementTweenID;
    private string colorTweenID;
    private string barTweenID;

    private void Awake()
    {
        movementTweenID = gameObject.GetInstanceID() + "_Movement";
        colorTweenID = gameObject.GetInstanceID() + "_ColorChange";
        barTweenID = gameObject.GetInstanceID() + "_BarChange";
    }

    public void Init(UnitType unitType, bool player)
    {
        this.unitType = unitType;

        animator.Play(GetIdleClip(), 0, Random.Range(0f, 1f));
        //spriteRenderer.sprite = GameManager.UnitTypeToSprite[unitType];
        spriteRenderer.flipX = !player;

        healthSliderFill.color = player ? playerHealthColor : enemyHealthColor;

        healthSlider.value = 1f;
        specialSlider.value = 0f;
    }

    public void UpdateData(float healthFill, float specialFill, bool slowed, bool confused)
    {
        DOTween.Kill(barTweenID);

        healthSlider.DOValue(healthFill, Simulation.TickDuration / 2f).SetId(barTweenID);
        specialSlider.DOValue(specialFill, Simulation.TickDuration / 2f).SetId(barTweenID);

        slowedIcon.SetActive(slowed);
        confusedIcon.SetActive(confused);
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (dead)
            return;

        moving = true;
        SetDirection(targetPosition);

        DOTween.Kill(movementTweenID);

        transform.DOMove(targetPosition, Simulation.TickDuration)
            .OnComplete(() => moving = false)
            .SetId(movementTweenID);
    }

    public void Die()
    {
        dead = true;
        DOTween.Kill(colorTweenID);
        DOTween.Kill(barTweenID);

        spriteRenderer.color = Color.white;
        spriteRenderer.DOColor(Color.clear, Simulation.TickDuration)
            .SetId(colorTweenID);

        hudCanvas.DOFade(0f, Simulation.TickDuration);
        statusCanvas.DOFade(0f, Simulation.TickDuration);
    }

    public void TakeDamage(bool special)
    {
        if (dead)
            return;

        spriteRenderer.DOColor(special ? Color.blue : Color.red, Simulation.TickDuration / 2f)
            .OnComplete(() => spriteRenderer.DOColor(Color.white, Simulation.TickDuration /2f))
            .SetId(colorTweenID);
    }

    public void DoAttack(Vector3 targetPosition, int range)
    {
        if (dead || moving)
            return;

        SetDirection(targetPosition);

        // jolt
        Vector3 startPos = transform.position;
        Vector3 joltTarget = (targetPosition - startPos).normalized * 0.2f + startPos;

        transform.DOMove(joltTarget, Simulation.TickDuration / 4f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => transform.DOMove(startPos, Simulation.TickDuration / 4f)
            .SetEase(Ease.OutQuad))
            .SetId(movementTweenID);

        // shoot projectile
        if (range > 1) // for now ranged units do normal attack at 1 range but could be changed to check unit type instead
        {
            Instantiate(projectilePrefab, projectileSource.position, projectileSource.rotation, this.transform).Go(targetPosition, unitType);
        }
    }

    public void DoSpecial(Vector3 targetPosition, int range)
    {
        if (dead || moving)
            return;

        SetDirection(targetPosition);

        if (unitType == UnitType.WorkerBee)
        {
            // jolt
            Vector3 startPos = transform.position;
            Vector3 joltTarget = (targetPosition - startPos).normalized * 0.2f + startPos;

            transform.DOMove(joltTarget, Simulation.TickDuration / 4f)
                .SetEase(Ease.InQuad)
                .OnComplete(() => transform.DOMove(startPos, Simulation.TickDuration / 4f)
                .SetEase(Ease.OutQuad))
                .SetId(movementTweenID);
        }
        else
        {
            animator.Play(GetSpecialClip());
        }

        // shoot projectile
        if (range > 1) // for now ranged units do normal attack at 1 range but could be changed to check unit type instead
        {
            Instantiate(projectilePrefab, projectileSource.position, projectileSource.rotation, this.transform).Go(targetPosition, unitType);
        }

    }

    public void DoJump(bool infinite)
    {
        animator.SetBool("InfiniteJump", infinite);
        animator.Play(GetJumpClip());
    }

    /// <summary>
    /// Checks if the unit needs to change direction
    /// </summary>
    /// <param name="target"></param>
    /// <param name="right">The direction to face if needed</param>
    /// <returns></returns>
    private void SetDirection(Vector3 target)
    {
        float horizontalDiff = target.x - transform.position.x;

        if (Mathf.Abs(horizontalDiff) < 0.5f) // same column - no change
            return;
        else if (horizontalDiff > 0) 
            spriteRenderer.flipX = false; // going right
        else 
            spriteRenderer.flipX = true; // going left
    }

    private string GetIdleClip()
    {
        switch (unitType)
        {
            case UnitType.Beetle:
                return "beetle_idle";
            case UnitType.Moth:
                return "moth_idle";
            case UnitType.QueenBee:
                return "queen_bee_idle";
            case UnitType.Spider:
                return "spider_idle";
            case UnitType.WorkerBee:
                return "worker_bee_idle";

            default:
                return "beetle_idle";
        }
    }

    private string GetJumpClip()
    {
        switch (unitType)
        {
            case UnitType.Beetle:
                return "beetle_jump";
            case UnitType.Moth:
                return "moth_jump";
            case UnitType.QueenBee:
                return "queen_bee_jump";
            case UnitType.Spider:
                return "spider_jump";
            case UnitType.WorkerBee:
                return "worker_bee_jump";

            default:
                return "beetle_jump";
        }
    }

    private string GetSpecialClip()
    {
        switch (unitType)
        {
            case UnitType.Beetle:
                return "beetle_special";
            case UnitType.Moth:
                return "moth_special";
            case UnitType.QueenBee:
                return "queen_bee_special";
            case UnitType.Spider:
                return "spider_special";

            default:
                return "beetle_special";
        }
    }
}
