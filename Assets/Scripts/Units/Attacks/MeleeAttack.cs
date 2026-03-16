using UnityEngine;

public class MeleeAttack : UnitAttack
{
    [SerializeField] private float colliderActiveTime;

    private void Start()
    {
        User.CanMove = false;

        // Attack is tied to animation event in the animator
        // Kinda goofy way of implementing this 
        User.SetAnimTrigger("Attack");
        User.OnAnimEvent += OnAnimEvent;
    }

    private void OnDestroy()
    {
        User.OnAnimEvent -= OnAnimEvent;

        User.CanMove = true;
    }

    private void OnAnimEvent(string name)
    {
        if (name == "Attack")
        {
            EnableCollider();

            Invoke(nameof(DisableCollider), colliderActiveTime);
        }

        if (name == "DoneAttack")
        {
            User.CanMove = true;

            Die();
        }
    }
}
