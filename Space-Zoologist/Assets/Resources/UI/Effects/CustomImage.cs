using UnityEngine;
using UnityEngine.UI;

public class CustomImage : Image {
    public override Material GetModifiedMaterial (Material baseMaterial) {
        Material cModifiedMat = base.GetModifiedMaterial (baseMaterial);
        // Do whatever you want with this "cModifiedMat"...
        // You can also hold this and process it in your grayscale code.
        // ...
        return cModifiedMat;
    }
}