# Object Pooling in Procedural Generation System

This project demonstrates the implementation of an object pooling system to optimize resource management in a procedural generation environment. Object pooling is a design pattern used to minimize the performance overhead associated with instantiating and destroying game objects frequently.

## Overview

In procedural generation, it's common to create and destroy many game objects dynamically, which can lead to performance issues due to frequent allocations and garbage collection. Object pooling helps mitigate this by maintaining a pool of pre-instantiated objects that can be reused, significantly improving performance and reducing frame drops.

## Key Components

### ObjectPool Struct

The `ObjectPool` struct is responsible for creating and managing a pool of game objects. It allows for the efficient generation of objects that can be reused throughout the game. The main components of the `ObjectPool` struct include:

- **Private Fields**:
  - `_objects`: An array of game object prefabs to instantiate.
  - `_amount`: The number of objects to create in the pool.
  - `_parent`: The transform that will act as the parent for the instantiated objects.

- **Constructor**: Initializes the object pool with the specified parameters.

- **Generate Method**: Instantiates the specified number of game objects from the provided prefabs, setting their parent transform and initial state.

```csharp
public struct ObjectPool
{
    private GameObject[] _objects;
    private int _amount;
    private Transform _parent;

    public ObjectPool(GameObject[] objects, int amount, Transform parent)
    {
        _objects = objects;
        _amount = amount;
        _parent = parent;
    }

    public GameObject[] Generate()
    {
        int prefabIndex = 0;
        GameObject[] resultObjects = new GameObject[_amount];

        for (int i = 0; i < _amount; i++)
        {
            if (prefabIndex >= _objects.Length)
                prefabIndex = 0;

            resultObjects[i] = Object.Instantiate(_objects[prefabIndex], Vector3.zero, _objects[prefabIndex].transform.rotation);
            resultObjects[i].SetActive(false);
            resultObjects[i].SetActive(true);
            resultObjects[i].SetActive(false);
            resultObjects[i].transform.parent = _parent;

            prefabIndex++;
        }
        return resultObjects;
    }
}
