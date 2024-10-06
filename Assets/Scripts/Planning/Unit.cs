using DG.Tweening;
using UnityEngine;

public class Unit : MonoBehaviour
{
    public Vector3 previousPosition; 

    public SpriteRenderer spriteRenderer;

    public void MoveTo(Vector3 targetPosition)
    {
        transform.DOMove(targetPosition, Simulation.TickDuration);
    }

    public void Die()
    {
        spriteRenderer.DOColor(Color.clear, Simulation.TickDuration);
    }

    public void TakeDamage()
    {
        spriteRenderer.DOColor(Color.red, Simulation.TickDuration / 2f).OnComplete
        (() => 
            spriteRenderer.DOColor(Color.white, Simulation.TickDuration /2f)
        );
    }
}
