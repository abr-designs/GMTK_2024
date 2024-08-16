using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform bulletBarrelPosition;

    public GameObject bulletPrefab;

    public bool firing = true;

    float bulletThrust = 5;
    float fireBulletDelay = 1;

    float fireBulletCountdown;

    // Start is called before the first frame update
    void Start()
    {
        fireBulletCountdown = fireBulletDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (!firing) return;

        fireBulletCountdown -= Time.deltaTime;
        if(fireBulletCountdown < 0 ) { fireBullet(); }
    }

    public void fireBullet() {
        GameObject bullet = Instantiate( bulletPrefab, bulletBarrelPosition.position, transform.rotation, null);
        bullet.GetComponent<Rigidbody>().AddForce(bullet.transform.up * bulletThrust);
        fireBulletCountdown = fireBulletDelay;
    }
}
