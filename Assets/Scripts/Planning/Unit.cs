using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Bars")]
    public CanvasGroup HudCanvas;
    public Slider healthSlider;
    public Image healthSliderFill;
    public Slider specialSlider;
    
    public Vector3 previousPosition; 
    public UnitType unitType;

    private bool dead = false;
    private bool moving = false;

    private string movementTweenID;
    private string colorTweenID;

    private void Awake()
    {
        movementTweenID = gameObject.GetInstanceID() + "_Movement";
        colorTweenID = gameObject.GetInstanceID() + "_ColorChange";
    }

    public void Init(UnitType unitType, bool player)
    {
        this.unitType = unitType;

        spriteRenderer.sprite = GameManager.UnitTypeToSprite[unitType];
        spriteRenderer.flipX = !player;

        healthSliderFill.color = player ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);

        healthSlider.value = 1f;
        specialSlider.value = 0f;
    }

    public void UpdateData(float healthFill, float specialFill)
    {
        healthSlider.value = healthFill;
        specialSlider.value = specialFill;
    }

    public void MoveTo(Vector3 targetPosition)
    {
        if (dead)
            return;

        moving = true;

        DOTween.Kill(movementTweenID);

        transform.DOMove(targetPosition, Simulation.TickDuration)
            .OnComplete(() => moving = false)
            .SetId(movementTweenID);
    }

    public void Die()
    {
        dead = true;
        DOTween.Kill(colorTweenID);

        spriteRenderer.color = Color.white;
        spriteRenderer.DOColor(Color.clear, Simulation.TickDuration)
            .SetId(colorTweenID);

        HudCanvas.DOFade(0f, Simulation.TickDuration);
    }

    public void TakeDamage(bool special)
    {
        if (dead)
            return;

        spriteRenderer.DOColor(special ? Color.blue : Color.red, Simulation.TickDuration / 2f)
            .OnComplete(() => spriteRenderer.DOColor(Color.white, Simulation.TickDuration /2f))
            .SetId(colorTweenID);
    }

    public void DoAttack(Vector3 targetPosition)
    {
        if (dead || moving)
            return;

        Vector3 startPos = transform.position;
        Vector3 joltTarget = (targetPosition - startPos).normalized * 0.2f + startPos;

        transform.DOMove(joltTarget, Simulation.TickDuration / 4f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => transform.DOMove(startPos, Simulation.TickDuration / 4f)
            .SetEase(Ease.OutQuad))
            .SetId(movementTweenID);
    }

    public void DoSpecial(Vector3 targetPosition)
    {
        if (dead || moving)
            return;

        // if queen bee for example there is no target so don't do jolt

        //Vector3 startPos = transform.position;
        //Vector3 joltTarget = (targetPosition - startPos).normalized * 0.2f + startPos;
        //
        //transform.DOMove(joltTarget, Simulation.TickDuration / 4f)
        //    .SetEase(Ease.InQuad)
        //    .OnComplete(() => transform.DOMove(startPos, Simulation.TickDuration / 4f)
        //    .SetEase(Ease.OutQuad))
        //    .SetId(movementTweenID);
    }
}
