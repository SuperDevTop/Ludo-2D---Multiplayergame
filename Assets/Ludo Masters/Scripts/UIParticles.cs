using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class UIParticles : MonoBehaviour
{
    public GameObject template;
    public float cycles;
    public float cycleDelay;
    public float particlesPerCycle;
    public float minForce;
    public float maxForce;
    public float minRotation;
    public float maxRotation;
    public float minScale;
    public float maxScale;
    public float lifetime;
    public Vector2 positionRandomness;
    public float timeScaleMultiplier;
    public Gradient color;

    IEnumerator Start()
    {
        Time.timeScale = timeScaleMultiplier;
        Invoke("Reset", lifetime);

        for (int i = 0; i < cycles; i++)
        {
            for (int j = 0; j < particlesPerCycle; j++)
            {
                Rigidbody2D rb = Instantiate(template, transform).GetComponent<Rigidbody2D>();
                rb.GetComponent<Image>().color = color.Evaluate(Random.Range(0f, 1f));
                rb.transform.position += new Vector3(Random.Range(-positionRandomness.x, positionRandomness.x),
                    Random.Range(-positionRandomness.y, positionRandomness.y));
                rb.gameObject.SetActive(true);
                AddExplosionForce(rb, Random.Range(minForce, maxForce), transform.position, 1000);
                rb.AddTorque(Random.Range(minRotation,maxRotation));
                rb.transform.localScale = Vector3.one * Random.Range(minScale,maxScale);
            }
            yield return new WaitForSecondsRealtime(cycleDelay);
        }
    }

    private void Reset()
    {
        Time.timeScale = 1;
        Destroy(gameObject);
    }

    public void AddExplosionForce(Rigidbody2D body, float explosionForce, Vector3 explosionPosition, float explosionRadius)
    {
        var dir = (body.transform.position - explosionPosition);
        float wearoff = 1 - (dir.magnitude / explosionRadius);
        body.AddForce(dir.normalized * explosionForce * wearoff);
    }
}
