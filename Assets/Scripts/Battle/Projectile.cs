using DG.Tweening;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;

    [Header("Sprites")]
    public Sprite queenBeeProjectileSprite;
    public Sprite spiderProjectileSprite;
    public Sprite mothProjectileSprite;

    public void Go(Vector3 targetPosition, UnitType projectileType, bool special = false)
    {
        spriteRenderer.sprite = GetSprite(projectileType, special);
        SetDirection(targetPosition);

        targetPosition.y = transform.position.y; // keep same elevation

        transform.DOMove(targetPosition, Simulation.TickDuration / 1.5f)
            .SetEase(Ease.InQuad)
            .OnComplete(() => transform.DOMove(transform.position, Simulation.TickDuration / 1.5f)
            .SetEase(Ease.OutQuad))
            .OnComplete(() => Destroy(this.gameObject)); // TODO: Play hit sound here if we have one
    }

    private Sprite GetSprite(UnitType projectileType, bool special = false)
    {
        //if (special)
        //{
        //    // TODO?: Different special projectiles
        //    return;
        //}

        switch (projectileType)
        {
            case UnitType.QueenBee:
                return queenBeeProjectileSprite;
            case UnitType.Spider:
                return spiderProjectileSprite;
            case UnitType.Moth:
                return mothProjectileSprite;

            default:
                return spiderProjectileSprite;
        }
    }

    /// <summary>
    /// Checks if the unit needs to change direction
    /// </summary>
    /// <param name="target"></param>
    /// <param name="right">The direction to face if needed</param>
    /// <returns></returns>
    private void SetDirection(Vector3 target) // literally coppied from unit lol
    {
        float horizontalDiff = target.x - transform.position.x;

        if (Mathf.Abs(horizontalDiff) < 0.5f) // same column - no change
            return;
        else if (horizontalDiff > 0)
            spriteRenderer.flipX = false; // going right
        else
            spriteRenderer.flipX = true; // going left
    }
}
