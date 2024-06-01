using UnityEngine;

/* All Rights Reserved to Leland T Carter of LethalLizard Studios.
 * @status COMPLETE
 * @date 2022-10-28
*/

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
