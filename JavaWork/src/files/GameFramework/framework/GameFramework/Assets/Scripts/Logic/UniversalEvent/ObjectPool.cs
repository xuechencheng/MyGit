using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool<Key, Value> where Value : class
{
    private readonly Dictionary<Key, Stack<Value>> stackMap = new Dictionary<Key, Stack<Value>>();
    private readonly Dictionary<Key, int> countAllMap = new Dictionary<Key, int>();
    public Value Get(Key key) {
        return null;
    }
}
