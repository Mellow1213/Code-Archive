using System;

public class HealthPoint
{
    private float _currentHealth; // 백킹 필드 추가

    public float currentHealth
    {
        get => _currentHealth;
        private set
        {
            _currentHealth = Math.Clamp(value, 0, maxHealth);
            if (_currentHealth == 0)
            {
                OnDeath?.Invoke();
            }
        }
    }
    public float maxHealth { get; private set; }
    public float healthPercent => (float)Math.Round(currentHealth / maxHealth * 100f, 2); // 소수점 둘째 자리까지만 반영
    public bool isAlive => currentHealth > 0;
  
    public event Action OnDeath;

    public HealthPoint(float maxHealth)
    {
        this.maxHealth = maxHealth;
        currentHealth = this.maxHealth;
    }

    public void SetMaxHealth(float amount)
    {
        maxHealth = amount;
        if (currentHealth > maxHealth)
            currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if (!isAlive) return;
        currentHealth -= damage;
    }

    public void Heal(float amount)
    {
        if (!isAlive) return;
        currentHealth += amount;
    }
}
