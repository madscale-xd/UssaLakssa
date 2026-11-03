using UnityEngine;
using TMPro;

public class StepCount : MonoBehaviour
{
    [Header("Step Settings")]
    [SerializeField] private int startingSteps = 10000;

    [Header("UI")]
    [SerializeField] private TMP_Text stepsText;

    private int stepsLeft;

    private void Start()
    {
        stepsLeft = startingSteps;
        UpdateDisplay();
    }

    public void UseStep(int amount = 1)
    {
        if (stepsLeft <= 0)
            return;

        stepsLeft = Mathf.Max(0, stepsLeft - amount);
        UpdateDisplay();
    }

    private void UpdateDisplay()
    {
        if (stepsText != null)
            stepsText.text = "Steps Left: " + stepsLeft;
    }
}