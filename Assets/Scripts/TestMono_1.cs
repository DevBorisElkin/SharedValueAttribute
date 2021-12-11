using Boris.CustomAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMono_1 : MonoBehaviour
{
    [Header("CoolValue_0")]
    [SharedValue("CoolValue_0", 0f, 1f)] [Range(0f, 1f)] public float value_0_1;
    [SharedValue("CoolValue_0", 0f, 1f)] [Range(0f, 1f)] public float value_0_2;


    [Header("CoolValue_1")]
    [SharedValue("CoolValue_1", 0f, 1f)][Range(0f, 1f)] public float value_1;
    [SharedValue("CoolValue_1", 0f, 1f)][Range(0f, 1f)] public float value_2;
    [SharedValue("CoolValue_1", 0f, 1f)][Range(0f, 1f)] public float value_3;


    [Header("CoolValue_2")]
    [SharedValue("CoolValue_2", 0f, 100f)] [Range(0f, 100f)] public float value_2_1;
    [SharedValue("CoolValue_2", 0f, 100f)] [Range(0f, 100f)] public float value_2_2;
    [SharedValue("CoolValue_2", 0f, 100f)] [Range(0f, 100f)] public float value_2_3;
    [SharedValue("CoolValue_2", 0f, 100f)] [Range(0f, 100f)] public float value_2_4;
    [SharedValue("CoolValue_2", 0f, 100f)] [Range(0f, 100f)] public float value_2_5;
}
