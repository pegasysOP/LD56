using System.Collections.Generic;
using UnityEngine;

public class UnitData : MonoBehaviour
{
    private static UnitData Instance;

    public void Init()
    {
        // GameManager handles the check, trust that we are the only instance if we are being initialized
        Instance = this;

        InitUnitCount();
        InitSpriteMap();
        InitLevels();
        InitUpgrades();

        ValidateData();
    }

    private void ValidateData()
    {
        if (unitSprites.Length != unitCount)
            throw new System.Exception("Unit sprites count mismatch");

        if (levelEnemies.Count != levelUpgrades.Count)
            throw new System.Exception("Level enemies and level upgrades count mismatch");
    }

    #region Units

    private int unitCount;

    public Sprite[] unitSprites;
    private Dictionary<UnitType, Sprite> unitSpriteMap;

    private void InitUnitCount()
    {
        unitCount = System.Enum.GetValues(typeof(UnitType)).Length;
    }

    public static int GetUnitCount()
    {
        return Instance.unitCount;
    }

    private void InitSpriteMap()
    {
        unitSpriteMap = new Dictionary<UnitType, Sprite>
        {
            { UnitType.QueenBee, unitSprites[0] },
            { UnitType.Beetle, unitSprites[1] },
            { UnitType.Spider, unitSprites[2] },
            { UnitType.Moth, unitSprites[3] },
            { UnitType.WorkerBee, unitSprites[4] },
            { UnitType.FireAnt, unitSprites[5] }
        };
    }

    /// <summary>
    /// Get the sprite for a unit type
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>Can return null</returns>
    public static Sprite GetUnitSprite(UnitType unitType)
    {
        return Instance.unitSpriteMap[unitType];
    }

    /// <summary>
    /// Get the name of a unit type
    /// </summary>
    /// <param name="unitType"></param>
    /// <returns>Can return null</returns>
    public static string GetUnitNameText(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Beetle:
                return "Beetle";
            case UnitType.Moth:
                return "Moth";
            case UnitType.QueenBee:
                return "Queen Bee";
            case UnitType.Spider:
                return "Spider";
            case UnitType.WorkerBee:
                return "Worker Bee";
            case UnitType.FireAnt:
                return "Fire Ant";
            default:
                return "-";
        }
    }

    public static string GetUnitDescriptionText(UnitType unitType)
    {
        switch (unitType)
        {
            case UnitType.Beetle:
                return "A low damage but tanky melee unit with a special that flings the enemy away, opening up the front lines.";
            case UnitType.Moth:
                return "A two range unit with an 'area of effect' special that causes enemies to become confused.";
            case UnitType.QueenBee:
                return "A two range unit with a special that heals all friendly worker bees.";
            case UnitType.Spider:
                return "A three range glass canon unit with an 'area of effect' special that causes enemies to be slowed.";
            case UnitType.WorkerBee:
                return "A simple melee unit with a special attack that does extra damage.";
            case UnitType.FireAnt:
                return "Lorem ipsum dolor sit amet, consectetur adipiscing elit.";
            default:
                return "-";
        }
    }

    public static string GetIdleClipName(UnitType unitType)
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
            case UnitType.FireAnt:
                return "fire_ant_idle";


            default:
                return "moth_idle";
        }
    }

    public static string GetJumpClipName(UnitType unitType)
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
            case UnitType.FireAnt:
                return "fire_ant_jump";

            default:
                return "beetle_jump";
        }
    }

    public static string GetSpecialClipName(UnitType unitType)
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
            case UnitType.FireAnt:
                return "fire_ant_special";

            default:
                return "beetle_special";
        }
    }

    #endregion

    #region Levels

    private List<Dictionary<Vector2Int, UnitType>> levelEnemies;

    private void InitLevels()
    {
        levelEnemies = new List<Dictionary<Vector2Int, UnitType>>
        {
            // Level 0
            new Dictionary<Vector2Int, UnitType>
            {
                { new Vector2Int(7, 6), UnitType.WorkerBee }
            },
            // Level 1
            new Dictionary<Vector2Int, UnitType>
            {
                { new Vector2Int(6, 3), UnitType.WorkerBee },
                { new Vector2Int(7, 4), UnitType.Beetle }
            },
            // Level 2
            new Dictionary<Vector2Int, UnitType>
            {
                { new Vector2Int(6, 2), UnitType.QueenBee },
                { new Vector2Int(7, 2), UnitType.Spider },
                { new Vector2Int(7, 3), UnitType.Moth }
            },
            // Level 3
            new Dictionary<Vector2Int, UnitType>
            {
                { new Vector2Int(6, 2), UnitType.WorkerBee },
                { new Vector2Int(6, 4), UnitType.Beetle },
                { new Vector2Int(7, 2), UnitType.Moth },
                { new Vector2Int(7, 4), UnitType.QueenBee }
            },
            // Level 4
            new Dictionary<Vector2Int, UnitType>
            {
                { new Vector2Int(6, 3), UnitType.QueenBee },
                { new Vector2Int(5, 3), UnitType.WorkerBee },
                { new Vector2Int(6, 2), UnitType.WorkerBee },
                { new Vector2Int(6, 4), UnitType.WorkerBee },
                { new Vector2Int(7, 3), UnitType.WorkerBee },
                { new Vector2Int(7, 5), UnitType.WorkerBee }
            }
        };
    }

    public static Dictionary<Vector2Int, UnitType> GetLevelEnemies(int level)
    {
        return Instance.levelEnemies[level];
    }

    public static int GetLevelCount()
    {
        return Instance.levelEnemies.Count;
    }

    #endregion

    #region Upgrades

    private List<UnitType[]> levelUpgrades;

    private void InitUpgrades()
    {
        levelUpgrades = new List<UnitType[]>
        {
            new UnitType[] { UnitType.Spider, UnitType.Moth, UnitType.Beetle },
            new UnitType[] { UnitType.QueenBee, UnitType.Beetle, UnitType.WorkerBee },
            new UnitType[] { UnitType.Moth, UnitType.Spider, UnitType.Beetle },
            new UnitType[] { UnitType.QueenBee, UnitType.Moth, UnitType.Spider },
            new UnitType[] { UnitType.Beetle, UnitType.QueenBee, UnitType.WorkerBee }
        };
    }

    public static UnitType[] GetLevelUpgrades(int level)
    {
        return Instance.levelUpgrades[level];
    }

    #endregion
}
