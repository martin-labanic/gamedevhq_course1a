using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class CameraUtils : MonoBehaviour {
    private const float DEFAULT_SHAKE_DURATION = 0.5f;

    public IEnumerator Shake(float strength, float duration = DEFAULT_SHAKE_DURATION) {
        Vector3 originalPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration) {
            float x = Random.Range(-1f, 1f) * strength;
            float y = Random.Range(-1f, 1f) * strength;
            transform.position = originalPosition + new Vector3(x, y, 0f);
            elapsedTime += Time.deltaTime;
            yield return 0;
        }

        transform.position = originalPosition;
    }
}
