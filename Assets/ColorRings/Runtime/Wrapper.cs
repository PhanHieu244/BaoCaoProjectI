using System;

[Serializable]
public class Wrapper<K, V>
{
    public K key;
    public V value;
}