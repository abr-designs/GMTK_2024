using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShipController : MonoBehaviour
{
    public List<Reward> rewards = new List<Reward>();
    public ThrusterController thrusterController;

    private void OnCollisionEnter(Collision collision) {
        if (collision.collider.tag == "Bullet") {

            // destroy bullet
            Destroy(collision.gameObject);

            // invincible player
            if (tag == "Player") return;
            
            DestroyShip();

        }

        if (collision.collider.tag == "Reward") {
            CollectReward(collision.gameObject.GetComponent<Reward>());
        }
    }

    public void DestroyShip() {

        foreach(Reward reward in rewards) {
            reward.transform.parent = null;
            reward.parentShip = null;
            reward.SetPartActive(false);

            // give part random rotation
            Rigidbody gameObjectsRigidBody = reward.GetComponent<Rigidbody>();
            if (gameObjectsRigidBody == null) {
                gameObjectsRigidBody = reward.AddComponent<Rigidbody>();
            }
            gameObjectsRigidBody.useGravity = false;
            gameObjectsRigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY;
            gameObjectsRigidBody.mass = 10;
            gameObjectsRigidBody.drag = 5;
            float randomTorqueMagnitde = Random.Range(-20, 20);
            Vector3 torqueVector = Vector3.forward * randomTorqueMagnitde * gameObjectsRigidBody.mass;
            gameObjectsRigidBody.angularDrag = 0.67f;
            gameObjectsRigidBody.AddTorque(torqueVector);
        }

        Destroy(gameObject);
    }

    public void CollectReward(Reward reward) {

        if(reward.parentShip != null) { return; }

        Destroy(reward.GetComponent<Rigidbody>());

        reward.transform.parent = transform;
        reward.parentShip = this;

        // add to lists depending on reward type
        Thruster thruster = reward.GetComponent<Thruster>();
        if (thruster != null) {
            thrusterController.thrusters.Add(thruster);
            GetComponent<ThrusterController>().CalcAngularThrust(thrusterController.thrusters.Count);
        }

        reward.SetPartActive(true);
    }
}
