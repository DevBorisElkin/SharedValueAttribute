using Boris.CustomAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono_1 : MonoBehaviour
{

    [SharedValue("CoolValue_1", 0f, 1f)][Range(0f, 1f)]
    public float value_1;

    [SharedValue("CoolValue_1", 0f, 1f)][Range(0f, 1f)]
    public float value_2;

    [SharedValue("CoolValue_1", 0f, 1f)][Range(0f, 1f)]
    public float value_3;

    //[SharedValue("CoolValue", 0f, 1f)]
    //public float coolFloat;
    //
    //[SharedValue("CoolValue_GG", -15f, 15f)]
    //public float coolFloat_2;
    //
    //[SharedValue("CoolValue_GG", 0f, 1f)]
    //public float coolFloat_3;
    //
    //[SharedValue("CoolValue_Wowo", 0f, 1f)]
    //public float coolFloat_4;
}
