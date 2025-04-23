using UnityEngine;
using System.Collections.Generic;
using System;

public class Unit : MonoBehaviour
{
    
    public Vector2 movement;
    public float distance;
    public event Action<float> OnMove;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Move(new Vector2(movement.x, 0) * Time.fixedDeltaTime);
        Move(new Vector2(0, movement.y) * Time.fixedDeltaTime);
        distance += movement.magnitude*Time.fixedDeltaTime;
        if (distance > 0.5f)
        {
            OnMove?.Invoke(distance);
            distance = 0;
        }
    }

    public void Move(Vector2 ds)
    {
        if (ds == Vector2.zero) return; // Don't cast if not trying to move

        Rigidbody2D rb = GetComponent<Rigidbody2D>(); // Cache Rigidbody2D
        if (rb == null)
        {
            Debug.LogError($"{gameObject.name} is missing Rigidbody2D!", gameObject);
            return;
        }

        List<RaycastHit2D> hits = new List<RaycastHit2D>();
        // Use the Rigidbody's layer for casting to respect physics settings
        ContactFilter2D filter = new ContactFilter2D();
        filter.SetLayerMask(Physics2D.GetLayerCollisionMask(gameObject.layer)); // Use collision mask based on Physics2D settings
        filter.useTriggers = false; // Ignore triggers for blocking movement

        int n = rb.Cast(ds, filter, hits, ds.magnitude);

        if (n == 0)
        {
            // No hits, safe to move the full distance
            // Debug.Log($"{gameObject.name} (Layer: {LayerMask.LayerToName(gameObject.layer)}): Moving by {ds}", gameObject);
            transform.Translate(ds);
        }
        else
        {
            // Find the closest non-trigger hit
            RaycastHit2D closestHit = new RaycastHit2D();
            float closestDistance = float.MaxValue;
            bool foundBlockingHit = false;
            for(int i = 0; i < n; i++)
            {
                // Check if the hit is valid, not a trigger, and closer than previous hits
                // Also ensure we didn't hit ourselves (can happen with complex colliders)
                if (hits[i].collider != null && !hits[i].collider.isTrigger && hits[i].rigidbody != rb && hits[i].distance < closestDistance)
                {
                    closestHit = hits[i];
                    closestDistance = hits[i].distance;
                    foundBlockingHit = true;
                }
            }

            if (foundBlockingHit)
            {
                // --- Improved Log: Report collision with layers ---
                // Debug.LogWarning($"{gameObject.name} (Layer: {LayerMask.LayerToName(gameObject.layer)}): Movement blocked by {closestHit.collider.name} (Layer: {LayerMask.LayerToName(closestHit.collider.gameObject.layer)}) at distance {closestHit.distance:F4}. Tried moving {ds}.", gameObject);

                // --- Implement Movement Adjustment ---
                // Move only up to the hit point, minus a small skin width
                // Use defaultContactOffset instead of colliderDistance
                float moveDistance = closestHit.distance - Physics2D.defaultContactOffset; // Use defaultContactOffset
                if (moveDistance > 0) // Only move if the calculated distance is positive
                {
                    // Debug.Log($"{gameObject.name}: Adjusting movement to {ds.normalized * moveDistance}", gameObject);
                    transform.Translate(ds.normalized * moveDistance);
                }
                // else: Hit is too close (at or within defaultContactOffset), don't move in this direction this frame.
                // Debug.Log($"{gameObject.name}: Hit distance ({closestHit.distance:F4}) too close, not moving.", gameObject);
            }
            else
            {
                 // Cast hit something, but all hits were triggers or self. Allow full movement.
                 // Debug.Log($"{gameObject.name} (Layer: {LayerMask.LayerToName(gameObject.layer)}): Cast hit only triggers/self. Moving by {ds}", gameObject);
                 transform.Translate(ds);
            }
        }
    }

}
