using DG.Tweening;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    
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
        // set up sprite etc.
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
}
