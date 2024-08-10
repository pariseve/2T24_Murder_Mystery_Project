using UnityEngine;

public class RandomFlicker : MonoBehaviour
{
    public Light flickeringLight; // The spotlight to flicker
    public float minFlickerInterval = 5.0f; // Minimum time between flicker events
    public float maxFlickerInterval = 10.0f; // Maximum time between flicker events
    public float minFlickerDuration = 0.05f; // Minimum duration of each individual flicker
    public float maxFlickerDuration = 0.2f; // Maximum duration of each individual flicker
    public float minIntensity = 0.5f; // Minimum light intensity during flicker
    public float maxIntensity = 2.0f; // Maximum light intensity during flicker
    public int minFlickers = 3; // Minimum number of flickers in one event
    public int maxFlickers = 8; // Maximum number of flickers in one event

    private float timeToNextFlicker; // Time until the next flicker event
    private bool isFlickering = false; // Is the light currently flickering?

    void Start()
    {
        if (flickeringLight == null)
        {
            flickeringLight = GetComponent<Light>();
        }
        SetNextFlickerTime(); // Set the initial time for the next flicker event
    }

    void Update()
    {
        if (!isFlickering)
        {
            timeToNextFlicker -= Time.deltaTime;
            if (timeToNextFlicker <= 0f)
            {
                StartCoroutine(FlickerLight());
            }
        }
    }

    private void SetNextFlickerTime()
    {
        timeToNextFlicker = Random.Range(minFlickerInterval, maxFlickerInterval);
    }

    private System.Collections.IEnumerator FlickerLight()
    {
        isFlickering = true;

        // Determine the number of flickers in this event
        int flickerCount = Random.Range(minFlickers, maxFlickers);

        float originalIntensity = flickeringLight.intensity;

        for (int i = 0; i < flickerCount; i++)
        {
            // Randomly set the intensity during the flicker
            flickeringLight.intensity = Random.Range(minIntensity, maxIntensity);

            // Randomize the duration of each flicker
            float flickerTime = Random.Range(minFlickerDuration, maxFlickerDuration);
            yield return new WaitForSeconds(flickerTime);

            // Return to the original intensity between flickers
            flickeringLight.intensity = originalIntensity;

            // Optionally, add a small delay between individual flickers to make them distinct
            yield return new WaitForSeconds(flickerTime);
        }

        SetNextFlickerTime();
        isFlickering = false;
    }
}