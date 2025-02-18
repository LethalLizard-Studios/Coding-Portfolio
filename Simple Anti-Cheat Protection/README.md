# Tampering Protection in MoneyManager

This example demonstrates a simple tampering protection system implemented within the `MoneyManager` class in my game **SANCTION**. The goal of this implementation is to safeguard the in-game currency from unauthorized modifications, ensuring a fair gaming experience.

## Overview

This example uses a combination of techniques to detect and prevent such tampering. The main components of the `MoneyManager` class include:

- **ProjectedInt32Value**: A custom struct that obfuscates the stored balance to prevent direct access and tampering within memory using programs such as **Cheat Engine**.
- **Hashing Mechanism**: A simple hashing utility that generates and verifies the integrity of the balance.
- **Integrity Checks**: Methods that ensure the balance has not been altered during gameplay.

## Key Components

### ProjectedInt32Value

The `ProjectedInt32Value` struct protects against direct manipulation of the balance by using an offset technique. The balance is stored as an obfuscated value, and the actual balance can only be accessed through methods that manage this offset. This offset can be changed to anything but is set to 111 for the example.

```csharp
public struct ProjectedInt32Value
{
    private int _projected;

    public ProjectedInt32Value(int value)
    {
        _projected = value - 111;
    }

    public readonly int GetValue()
    {
        return _projected + 111;
    }

    public void SetValue(int value)
    {
        _projected = value - 111;
    }
}
