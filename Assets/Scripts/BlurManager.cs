using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

class BlurManager : GenericSingleton<BlurManager>
{
    [SerializeField]
    private List<Image> images;

    public void Blur()
    {
        var life = StateMachine.Instance.Plyr.CurrentLife;

        foreach (var image in images)
        {
            image.material.SetFloat("Blur_slider", (1 - (float)life / StateMachine.Instance.Plyr.MaxLife));
        }
    }
}
