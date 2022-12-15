
using UnityEngine;

public class Target : MonoBehaviour
{
     NPC n;
    public float maxHealth = 100;
    float Health;

    private void Start() {
        Health = maxHealth;
    }
    private void Update() {
        Health += 5;
        Health = Mathf.Clamp(Health, 0, maxHealth);
    }
    public void TakeDamage ( float amount)
    {
        Health -= amount;
        if (Health <= 0f)
        {
            Die();
        }
    }

    public void SetNPC(NPC n_){
        n = n_;
    }
    
    void Die()
    {
        n.UnPossess();
        //Destroy(gameObject);
    }


}
