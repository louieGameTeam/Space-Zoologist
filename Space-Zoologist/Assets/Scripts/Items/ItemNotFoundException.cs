using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemNotFoundException : System.Exception
{
    public ItemNotFoundException() { }
    public ItemNotFoundException(string message) : base(message) { }
    public ItemNotFoundException(string message, System.Exception inner) : base(message, inner) { }
    protected ItemNotFoundException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}