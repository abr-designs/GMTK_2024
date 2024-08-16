using System.Collections.Generic;
using UnityEngine;

public class ThrusterController : MonoBehaviour {

    public bool playerControlled = false;

    public float thrustForce = 10f; // Force applied for thrust
    public float baseAngularForce = 30f; // Angular force for rotation
    public float angularThrustForce = 50f; // Angular force for rotation

    private Rigidbody rb;

    public List<Thruster> thrusters = new List<Thruster>();

    void Start() {
        // Get the Rigidbody component
        rb = GetComponent<Rigidbody>();
    }

    public void CalcAngularThrust(int i) {
        angularThrustForce = angularThrustForce * i;
    }

    void Update() {

        if (!playerControlled) return;

        // Check for thrust input
        if (Input.GetKeyDown(KeyCode.W)) SetVisualizeThrust(true);
        if (Input.GetKey(KeyCode.W)) {
            // Apply upward thrust
            //rb.AddForce(transform.up * thrustForce);
            rb.AddForce(GetShipThrust());
        }
        if (Input.GetKeyUp(KeyCode.W)) SetVisualizeThrust(false);

        // Check for angular thrust input
        if (Input.GetKey(KeyCode.A)) {
            // Apply angular thrust around the y-axis (roll)
            rb.AddTorque(Vector3.forward * angularThrustForce);
        }

        if (Input.GetKey(KeyCode.D)) {
            // Apply angular thrust around the y-axis (roll)
            rb.AddTorque(Vector3.back * angularThrustForce);
        }
    }

    public void SetVisualizeThrust(bool b) {
        foreach (Thruster thruster in thrusters) {
            thruster.SetVisualDisplayed(b);
        }
    }

    public Vector3 GetShipThrust() {
        Vector3 totalThrust = Vector3.zero;
        foreach (Thruster thruster in thrusters) {
            totalThrust += thruster.ProvideThrust();
        }
        return totalThrust;
    }
}
