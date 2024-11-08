using System;

public static class Actions
{
    public static Action<float> onTakeDamage;
    public static Action<float> onDamage;
    public static Action<float> onHeal;
    public static Action<float> onStaminaChange;
}