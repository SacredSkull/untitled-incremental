using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class Notification : MonoBehaviour {
    public TextMesh title; 
    public Asset resource;

    public void Initialise() {
        title.text = resource.name;
        StartCoroutine(fadeOut());
    }

    private IEnumerator fadeOut() {
        yield return new WaitForSeconds(4);
        GetComponent<FadeObjectInOut>().FadeOut();
        yield return new WaitForSeconds(1.5f);
        Destroy(this.gameObject);
    }
}
