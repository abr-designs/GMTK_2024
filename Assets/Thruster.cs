using UnityEngine;

public class Thruster : MonoBehaviour
{
    public GameObject outputAnchorPosition;

    public float thrust = 10f;

    public Vector3 ProvideThrust() {
        return transform.up * thrust;
    }

    public void SetVisualDisplayed(bool display) {
        outputAnchorPosition.SetActive(display);
    }
}
