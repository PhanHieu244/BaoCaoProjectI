using System;
using System.Collections.Generic;
using System.Linq;

public static class ConvertShortcutExtensions
{
    public static Dictionary<K, V> ToDictionary<K, V>(this IEnumerable<Wrapper<K, V>> list)
    {
        return list.ToDictionary(wrapper => wrapper.key, wrapper => wrapper.value);
    }
    
    public static Dictionary<K, V> ToDictionary<K, V, K1, V1>(this IEnumerable<Wrapper<K1, V1>> list, Func<K1, K> fk, Func<V1, V> fv)
    {
        return list.ToDictionary(wrapper => fk.Invoke(wrapper.key), wrapper => fv.Invoke(wrapper.value));
    }
    
}