using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGas 
{
    public float Gas { get; set; }
    /// <summary>
    /// 發動無雙技
    /// </summary>
    public void Launch();
}
