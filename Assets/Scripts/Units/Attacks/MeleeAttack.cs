using UnityEngine;

public class MeleeAttack : UnitAttack
{
    [SerializeField] private float colliderActiveTime;
    [SerializeField] private string[] hitSfx;
    [SerializeField] private float volume;

    private void Start()
    {
        User.CanMove = false;

        // Attack is tied to animation event in the animator
        // Kinda goofy way of implementing this 
        User.SetAnimTrigger("Attack");
        User.OnAnimEvent += OnAnimEvent;

        SfxPlayer.PlaySfx("Swoosh", transform.position);
    }

    private void Update()
    {
        if (User != null)
        {
            transform.position = User.transform.position;
        }
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

            foreach (var sfx in hitSfx)
            {
                SfxPlayer.PlaySfx(sfx, transform.position, volume);
            }

            Invoke(nameof(DisableCollider), colliderActiveTime);
        }

        if (name == "DoneAttack")
        {
            User.CanMove = true;

            Die();
        }
    }
}
