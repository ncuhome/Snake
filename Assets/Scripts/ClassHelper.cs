using System.Reflection;
using System;
using UnityEngine;

public class ClassHelper : MonoBehaviour
{
  /// <summary>
  ///C#反射遍历对象属性
  /// </summary>
  public static void ForeachClassProperties<T>(T obj)
  {
    Type t = obj.GetType();
    PropertyInfo[] propertyInfo = t.GetProperties();
    foreach (PropertyInfo item in propertyInfo)
    {
      string name = item.Name;
      System.Object value = item.GetValue(obj, null);
      Debug.Log(name);
      if (item.PropertyType.IsGenericType && item.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
      {
        var columnType = item.PropertyType.GetGenericArguments()[0];
        Debug.Log(columnType);
      }
      else
      {
        Debug.Log(item.PropertyType.Name);
      }
    }
  }
}
