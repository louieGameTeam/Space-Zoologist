using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LinkOpener", menuName = "Util/LinkOpener")]
public class LinkOpener : ScriptableObject
{
    
    public void OpenLink (string hyperlink) {
        Application.OpenURL (hyperlink);
    }
}
