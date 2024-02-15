using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DayNightCycle : MonoBehaviour
{
    public Image fadeImage;
    private float fadeSpeed = 1.0f;

    [Range(0.0f, 1.0f)]
    public float time; // 하루를 몇분으로 할건지
    public float fullDayLength;
    public float startTime = 0.4f;
    private float timeRate;
    public Vector3 noon;

    [Header("Sun")]
    public Light sun;
    public Gradient sunColor;
    public AnimationCurve sunIntensity;

    [Header("Moon")]
    public Light moon;
    public Gradient moonColor;
    public AnimationCurve moonIntensity;

    [Header("Other Lighting")]
    public AnimationCurve lightingIntensityMultiplier;
    public AnimationCurve reflectionIntensityMultiplier;

    private void Start()
    {
        timeRate = 1.0f / fullDayLength;
        time = startTime;
    }

    private void Update()
    {
        time = (time + timeRate * Time.deltaTime) % 1.0f; // timerate 를 퍼센트로 쓰므로 1.0 으로 나눔

        UpdateLighting(sun, sunColor, sunIntensity);
        UpdateLighting(moon, moonColor, moonIntensity);

        RenderSettings.ambientIntensity = lightingIntensityMultiplier.Evaluate(time);
        RenderSettings.ambientIntensity = reflectionIntensityMultiplier.Evaluate(time);
    }

    void UpdateLighting(Light lightSource, Gradient colorGradiant, AnimationCurve intensityCurve)
    {
        float intensity = intensityCurve.Evaluate(time); // 애니메이션커브는 시간을 주면 시간에 맞는 그래프 값을 가져온다

        lightSource.transform.eulerAngles = (time - (lightSource == sun ? 0.25f : 0.75f)) * noon * 4.0f; // 각도에 따라 빛변화 변경  noon 90도* 4 = 360, 1/4 지점이 중앙이 되므로 0.25 달이 꼭데기 0.75  
        lightSource.color = colorGradiant.Evaluate(time);
        lightSource.intensity = intensity;

        GameObject go = lightSource.gameObject;
        if (lightSource.intensity == 0 && go.activeInHierarchy)
            go.SetActive(false);
        else if (lightSource.intensity > 0 && !go.activeInHierarchy)
            go.SetActive(true);
    }

    public void Sleep()
    {
        StartCoroutine(FadeInAndOut());
    }

    IEnumerator FadeInAndOut()
    {
        for (float i = 0; i <= 1; i += Time.deltaTime * fadeSpeed)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        for (float i = 1; i >= 0; i -= Time.deltaTime * fadeSpeed)
        {
            fadeImage.color = new Color(0, 0, 0, i);
            yield return null;
        }

        time = 0.2f;
    }
}
