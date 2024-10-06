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
}
