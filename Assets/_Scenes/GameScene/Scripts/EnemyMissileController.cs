﻿using UnityEngine;
using System.Collections;

public class EnemyMissileController : MissileController 
{
    protected override void PreStart()
    {
        InitialSpeed = GetInitialSpeed();
    }

    private int GetInitialSpeed()
    {
        int level = Level.GetCurrentLevel();
        switch (Difficulty.GetDifficultyLevel())
        {
            case Difficulty.Level.Easy:
                return 550 + (10 * (level - 1));
            case Difficulty.Level.Normal:
                return 600 + (20 * (level - 1));
            case Difficulty.Level.Hard:
                return 700 + (30 * (level - 1));
            default:
                throw new System.SystemException("Unkown difficulty level");
        }
    }

    protected override void PostStart()
    {
        Object[] enemies = Object.FindObjectsOfType(typeof(Enemy));
        foreach (Enemy e in enemies)
        {
            if (e.gameObject.GetComponent<Collider2D>() != null)
            {
                Physics2D.IgnoreCollision(GetComponent<Collider2D>(), e.gameObject.GetComponent<Collider2D>());
            }
        }
    }

    new protected bool HasReachedTarget()
	{
		return (transform.position.x - TargetPosition.x) <= MinimumTargetProximity;
	}

	void LateUpdate() 
	{
		Vector3[] vertices = { transform.position, StartPosition, };
		GetComponent<LineRenderer> ().SetPositions (vertices);
	}

    protected override void CollidedWith(Collision2D coll)
    {
        if(WasDestroyedByFriendlyMissile(coll)) {
            InformScoreAdded();
        }
    }

    // This enemy missile was destroyed by a friendly missile IF the collider was a friendly and was either a missile or a friendly explosion.
    private bool WasDestroyedByFriendlyMissile(Collision2D coll)
    {
        return Friendly.IsFriendly(coll.gameObject) &&
            (coll.gameObject.GetComponent<FriendlyMissileController>() != null
            || coll.gameObject.GetComponent<ExplosionParticleSystemColliderController>() != null);
    }

    private void InformScoreAdded()
    {
        int scoreAdded = GetScoreAddedForMissileDestruction();
        HUDInventoryController controller = (HUDInventoryController)Object.FindObjectOfType(typeof(HUDInventoryController));
        controller.AddScore(scoreAdded);
    }

    protected int GetScoreAddedForMissileDestruction()
    {
        switch (Difficulty.GetDifficultyLevel())
        {
            case Difficulty.Level.Easy:
                return 100;
            case Difficulty.Level.Normal:
                return 150;
            case Difficulty.Level.Hard:
                return 200;
            default:
                throw new System.SystemException("Unkown difficulty level");
        }
    }
}
