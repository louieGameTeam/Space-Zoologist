using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResearchCategoryTypeEditableAttribute : PropertyAttribute
{
    public bool TypeEditable { get; private set; }

    public ResearchCategoryTypeEditableAttribute(bool TypeEditable)
    {
        this.TypeEditable = TypeEditable;
    }
}
