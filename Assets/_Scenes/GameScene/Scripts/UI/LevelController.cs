﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class LevelController : MonoBehaviour
{
    private const string LEVEL_TEXT = "LEVEL {0}";

    public Text LevelText;
    public EnemyAiController EnemyAiController;
    public AudioSource AirRaidSiren;

	void Start ()
    {
        EnemyAiController = EnemyAiController.GetComponent<EnemyAiController>();
        NextLevel();
	}

    public void NextLevel()
    {
        Level.NextLevel();
        int newLevel = Level.GetCurrentLevel();
        LevelText.gameObject.SetActive(true);
        LevelText.text = string.Format(LEVEL_TEXT, newLevel);
        InformLevelEnded();

        AirRaidSiren.Play();
        StartCoroutine(FadeLevelTextInAndOut(2f));
    }

    public void InformLevelEnded()
    {
        Object[] listeners = Object.FindObjectsOfType(typeof(LevelChangeListener));

        foreach (LevelChangeListener l in listeners)
        {
            l.OnLevelEnded();
        }
    }

    void Update ()
    {
	
	}

    public IEnumerator FadeLevelTextInAndOut(float t)
    {
        LevelText.color = new Color(LevelText.color.r, LevelText.color.g, LevelText.color.b, 0);
        while (LevelText.color.a < 1.0f) {
            LevelText.color = new Color(LevelText.color.r, LevelText.color.g, LevelText.color.b, LevelText.color.a + (Time.deltaTime / t));
            yield return null;
        }

        yield return new WaitForSeconds(2f);

        while (LevelText.color.a > 0f) {
            LevelText.color = new Color(LevelText.color.r, LevelText.color.g, LevelText.color.b, LevelText.color.a - (Time.deltaTime / t));
            yield return null;
        }

        LevelText.gameObject.SetActive(false);
        InformNextLevel();
    }

    public void InformNextLevel()
    {
        Object[] listeners = Object.FindObjectsOfType(typeof(LevelChangeListener));

        foreach (LevelChangeListener l in listeners) {
            l.OnNewLevelStarted(Level.GetCurrentLevel());
        }
    }

    public void InformGameOver()
    {
        Object[] listeners = Object.FindObjectsOfType(typeof(LevelChangeListener));

        foreach (LevelChangeListener l in listeners) {
            l.OnGameOver();
        }
    }
}

public abstract class LevelChangeListener : MonoBehaviour
{
    public abstract void OnLevelEnded();

    public abstract void OnNewLevelStarted(int level);

    public abstract void OnGameOver();
}