using UnityEngine;
using System;
using System.Collections.Generic; // Assuming you use a List

public class ProjectileManager : MonoBehaviour
{
    public GameObject[] projectiles;

    // Assuming you have a list or similar collection to track active projectiles
    private List<GameObject> activeProjectiles = new List<GameObject>();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        GameManager.Instance.projectileManager = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateProjectile(int which, string trajectory, Vector3 where, Vector3 direction, float speed, Action<Hittable,Vector3> onHit)
    {
        GameObject new_projectile = Instantiate(projectiles[which], where + direction.normalized*1.1f, Quaternion.Euler(0,0,Mathf.Atan2(direction.y, direction.x)*Mathf.Rad2Deg));
        new_projectile.GetComponent<ProjectileController>().movement = MakeMovement(trajectory, speed);
        new_projectile.GetComponent<ProjectileController>().OnHit += onHit;
        AddProjectile(new_projectile);
    }

    public void CreateProjectile(int which, string trajectory, Vector3 where, Vector3 direction, float speed, Action<Hittable, Vector3> onHit, float lifetime)
    {
        GameObject new_projectile = Instantiate(projectiles[which], where + direction.normalized * 1.1f, Quaternion.Euler(0, 0, Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg));
        new_projectile.GetComponent<ProjectileController>().movement = MakeMovement(trajectory, speed);
        new_projectile.GetComponent<ProjectileController>().OnHit += onHit;
        new_projectile.GetComponent<ProjectileController>().SetLifetime(lifetime);
        AddProjectile(new_projectile);
    }

    public ProjectileMovement MakeMovement(string name, float speed)
    {
        if (name == "straight")
        {
            return new StraightProjectileMovement(speed);
        }
        if (name == "homing")
        {
            return new HomingProjectileMovement(speed);
        }
        if (name == "spiraling")
        {
            return new SpiralingProjectileMovement(speed);
        }
        return null;
    }

    // Method to destroy all tracked projectiles
    public void ClearProjectiles()
    {
        // Create a copy to iterate over while destroying
        List<GameObject> projectilesToDestroy = new List<GameObject>(activeProjectiles);
        foreach (GameObject proj in projectilesToDestroy)
        {
            if (proj != null)
            {
                Destroy(proj); // Destroy the GameObject
            }
        }
        activeProjectiles.Clear(); // Clear the tracking list
        Debug.Log("Cleared active projectiles."); // Optional log
    }

    // Example methods for tracking (adjust as needed)
    public void AddProjectile(GameObject proj)
    {
        if (!activeProjectiles.Contains(proj))
        {
            activeProjectiles.Add(proj);
        }
    }

    public void RemoveProjectile(GameObject proj)
    {
        activeProjectiles.Remove(proj);
    }

    // Example of how a projectile might report its destruction
    // (Call this from the projectile script's OnDestroy or similar)
    public void ProjectileDestroyed(GameObject proj)
    {
        RemoveProjectile(proj);
    }
}
