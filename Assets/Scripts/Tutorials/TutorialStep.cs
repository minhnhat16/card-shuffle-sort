using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class TutorialStep : MonoBehaviour
{
    [SerializeField] private List<Collider2D> colliderStep = new List<Collider2D>();

    // Start is called before the first frame update
    public virtual void Start()
    {
        // Initialization code if needed
    }

    // Update is called once per frame
    public virtual void Update()
    {
        // Update logic if needed
    }

    // Method to add a collider to the list
    public void AddCollider(Collider2D collider)
    {
        if (collider != null && !colliderStep.Contains(collider))
        {
            colliderStep.Add(collider);
        }
    }

    // Method to remove a collider from the list
    public void RemoveCollider(Collider2D collider)
    {
        if (collider != null && colliderStep.Contains(collider))
        {
            colliderStep.Remove(collider);
        }
    }

    // Method to get all colliders
    public List<Collider2D> GetColliders()
    {
        return colliderStep;
    }

    public void PlayerHit(Action callback)
    {
        callback?.Invoke(); 
    }
}
