using System;
using UnityEngine;

public interface ITagNewInfo
{
    TagNewType TagNewType { get; }
    string Id { get; }
}

