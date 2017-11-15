using System.Collections;
using UnityEngine;

public class BlinkEffect : MonoBehaviour {

    public RectTransform topEyeLit;
    public RectTransform bottomEyeLit;
    public float duration = 0.5f;

    public bool EyesClosed { get; private set; }
    public float CurrentHeight { get; set; }
    private float initialWidth;
    private float initialHeight;
    public float targetHeight;

    private float t;

    void Start()
    {
        initialWidth = Screen.width;
        initialHeight = 0f;
        CurrentHeight = initialHeight;
        targetHeight = Screen.height;
        EyesClosed = false;
    }

    public IEnumerator Blink()
    {
        SetTargetHeight(Screen.height);

        while (CurrentHeight != targetHeight)
        {
            LerpToTargetHeight(duration / 2);
            yield return null;
        }

        SetTargetHeight(initialHeight);
        EyesClosed = true;

        while (CurrentHeight != targetHeight)
        {
            LerpToTargetHeight(duration / 2);
            yield return null;
        }

        EyesClosed = false;
    }

    void SetTargetHeight(float Height)
    {
        t = 0f;
        targetHeight = Height;
    }

    void LerpToTargetHeight(float dur)
    {
        t += Time.deltaTime / dur;
        CurrentHeight = Mathf.Lerp(CurrentHeight, targetHeight, t);
        topEyeLit.sizeDelta = new Vector2(initialWidth, CurrentHeight);
        bottomEyeLit.sizeDelta = new Vector2(initialWidth, CurrentHeight);
    }
}
