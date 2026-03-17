using System.Collections;
using TMPro;
using UnityEngine;
using System.Collections.Generic;

public class OrbitalCannonController : MonoBehaviour
{
    public OrbitalCannon orbitalCannon;

    [Header("Round Settings")]
    public int roundsToCharge = 3;
    private int currentRound = 0;

    [Header("UI References")]
    public GameObject sequenceCanvas;      // Canvas for sequence
    public TextMeshProUGUI sequenceText;   // Text for displaying the sequence

    public GameObject failedCanvas;        // Canvas for failed message
    public TextMeshProUGUI failedText;     // Text for failure message

    public GameObject launchCanvas;        // Canvas for countdown
    public TextMeshProUGUI launchText;     // Text for countdown (3, 2, 1, FIRE)

    private List<int> sequence = new List<int>();
    private int currentIndex = 0;
    private bool awaitingInput = false;

    void Start()
    {
        currentRound = 0;

        // Disable all canvases initially
        sequenceCanvas.SetActive(false);
        failedCanvas.SetActive(false);
        launchCanvas.SetActive(false);
    }

    public void NextRound()
    {
        currentRound++;
        if (currentRound > roundsToCharge)
            currentRound = roundsToCharge;
    }

    public void ActivateCannon()
    {
        if (currentRound <= roundsToCharge)
        {
            ShowFailed("Cannon not ready yet!");
            return;
        }

        GenerateSequence();
        ShowSequence();
        awaitingInput = true;
        currentIndex = 0;
    }

    void GenerateSequence()
    {
        sequence.Clear();
        int sequenceLength = 4; // Length of the sequence
        for (int i = 0; i < sequenceLength; i++)
            sequence.Add(Random.Range(2, 5)); // Random between 2, 3, or 4
    }

    void ShowSequence()
    {
        sequenceCanvas.SetActive(true);
        failedCanvas.SetActive(false);
        launchCanvas.SetActive(false);

        sequenceText.text = "Sequence: " + string.Join(",", sequence);
    }

    void ShowFailed(string message)
    {
        failedCanvas.SetActive(true);
        sequenceCanvas.SetActive(false);
        launchCanvas.SetActive(false);

        failedText.text = message;
    }

    IEnumerator FireCountdown()
    {
        awaitingInput = false;

        sequenceCanvas.SetActive(false);
        failedCanvas.SetActive(false);
        launchCanvas.SetActive(true);

        launchText.text = "3...";
        yield return new WaitForSeconds(1);

        launchText.text = "2...";
        yield return new WaitForSeconds(1);

        launchText.text = "1...";
        yield return new WaitForSeconds(1);

        launchText.text = "FIRE!";
        orbitalCannon?.TryFire();  // Fire the cannon

        currentRound = 0;

        yield return new WaitForSeconds(2);

        launchCanvas.SetActive(false); // Hide launch canvas
    }

    public void InputButton(int button)
    {
        if (!awaitingInput)
            return;

        if (sequence[currentIndex] == button)
        {
            currentIndex++;

            if (currentIndex >= sequence.Count)
            {
                StartCoroutine(FireCountdown());
            }
        }
        else
        {
            ShowFailed("Orbital cannon failed!");
            awaitingInput = false;
            currentIndex = 0;
        }
    }
}