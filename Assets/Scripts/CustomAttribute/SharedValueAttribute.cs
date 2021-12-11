using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Boris.CustomAttributes
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class SharedValueAttribute : Attribute, IBorisAttribute
    {
        public string ValueName { get; private set; }
        public float SharedMinValue { get; private set; }
        public float SharedMaxValue { get; private set; }

        public SharedValueAttribute(string valueName, float sharedMinValue, float sharedMaxValue)
        {
            ValueName = valueName;
            SharedMinValue = sharedMinValue;
            SharedMaxValue = sharedMaxValue;
        }
    }
}

