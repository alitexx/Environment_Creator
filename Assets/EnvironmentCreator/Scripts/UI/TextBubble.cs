using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;

public class TextBubble : MonoBehaviour
{
    public TextMeshProUGUI textMeshProUGUI;
    public Button continueButton;
    private string[] textSegments;
    private int currentSegmentIndex = 0;
    private float typingSpeed = 0.05f;
    private float textSpeedMultiplier;
    private KeyCode hurryUpKey;
    [SerializeField]private float normalTypingSpeed = 0.05f;
    [SerializeField]private float fastTypingSpeed = 0.025f; // Halved typing speed
    private Coroutine typingCoroutine;
    private bool isTyping = false;

    public void Initialize(string fullText, float textSpeedMultiplier)
    {
        this.textSpeedMultiplier = textSpeedMultiplier;
        hurryUpKey = GameSettings.hurryUpKey;
        textSegments = fullText.Split(new string[] { "\\n" }, System.StringSplitOptions.None);
        continueButton.onClick.AddListener(OnContinueButtonClicked);
        continueButton.gameObject.SetActive(false);
        textMeshProUGUI.gameObject.SetActive(true);
        ShowNextSegment();
    }

    private void Update()
    {
        if (isTyping && Input.GetKeyDown(hurryUpKey))
        {
            typingSpeed = fastTypingSpeed;
        }
    }

    private void ShowNextSegment()
    {
        if (currentSegmentIndex < textSegments.Length)
        {
            typingCoroutine = StartCoroutine(TypeText(textSegments[currentSegmentIndex]));
        }
        else
        {
            PauseManager.FreezeAllMovingObject(false); //Freeze NPCs and Players
            Destroy(gameObject);
        }
    }

    private IEnumerator TypeText(string segment)
    {
        isTyping = true;
        textMeshProUGUI.text = "";
        foreach (char letter in segment.ToCharArray())
        {
            textMeshProUGUI.text += letter;
            yield return new WaitForSeconds(typingSpeed / textSpeedMultiplier);
        }
        FinishTyping();
    }

    private void FinishTyping()
    {
        isTyping = false;
        typingSpeed = normalTypingSpeed; // Reset to normal speed
        continueButton.gameObject.SetActive(true);
    }

    private void OnContinueButtonClicked()
    {
        continueButton.gameObject.SetActive(false);
        currentSegmentIndex++;
        ShowNextSegment();
    }
}

