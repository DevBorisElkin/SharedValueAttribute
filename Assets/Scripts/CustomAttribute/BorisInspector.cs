using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Boris.CustomAttributes
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class BorisInspector : UnityEditor.Editor
    {
		private List<SerializedProperty> _serializedProperties = new List<SerializedProperty>();

		public override void OnInspectorGUI()
		{
			GetSerializedProperties(ref _serializedProperties);

			bool anyBorisAttribute = _serializedProperties.Any(p => PropertyUtility.GetAttribute<IBorisAttribute>(p) != null);

			if (anyBorisAttribute) ManageAttributesValues();

			DrawDefaultInspector();
		}

		protected void GetSerializedProperties(ref List<SerializedProperty> outSerializedProperties)
		{
			outSerializedProperties.Clear();
			using (var iterator = serializedObject.GetIterator())
			{
				if (iterator.NextVisible(true))
				{
					do
					{
						outSerializedProperties.Add(serializedObject.FindProperty(iterator.name));
					}
					while (iterator.NextVisible(false));
				}
			}
		}

		void ManageAttributesValues()
        {
			List<string> differentIds = new List<string>();
			Dictionary<string, List<SerializedProperty>> sortedDifferentProperties = new Dictionary<string, List<SerializedProperty>>();

			foreach (var a in _serializedProperties)
            {
				var attribute = PropertyUtility.GetAttribute<SharedValueAttribute>(a);
				if (attribute == null) continue;

				if (!differentIds.Contains(attribute.ValueName)) differentIds.Add(attribute.ValueName);
            }

			foreach(var a in differentIds)
            {
				List<SerializedProperty> propertiesWithCorrectName = new List<SerializedProperty>();

				foreach(var b in _serializedProperties)
                {
					var attribute = PropertyUtility.GetAttribute<SharedValueAttribute>(b);
					if (attribute == null) continue;

					if (attribute.ValueName == a) propertiesWithCorrectName.Add(b);
				}

				if (propertiesWithCorrectName.Count > 0)
					sortedDifferentProperties[a] = propertiesWithCorrectName;
            }

			if (sortedDifferentProperties.Count > 0) ManageValues(sortedDifferentProperties);
		}

		void ManageValues(Dictionary<string, List<SerializedProperty>> valueGroups)
        {
			if (recordedProperties == null)
            {
				RecordOldProperties(valueGroups);
				return;
			}

			foreach (var a in valueGroups.Keys)
            {
				bool valueFound = valueGroups.TryGetValue(a, out List<SerializedProperty> values);
				bool oldValueFound = recordedProperties.TryGetValue(a, out List<RecordedValue> oldValues);
				if (valueFound && oldValueFound) ManageValuesForSharedValueGroup(a, values, oldValues);
            }

			RecordOldProperties(valueGroups);
		}

		void ManageValuesForSharedValueGroup(string valueGroupName, List<SerializedProperty> properties, List<RecordedValue> oldProperties)
        {
			var attribute = PropertyUtility.GetAttribute<SharedValueAttribute>(properties[0]);

			ClampValuesIfNeeded(attribute, valueGroupName, properties);
			RedistributeValuesByGroup(attribute, valueGroupName, properties, oldProperties);
        }

		void ClampValuesIfNeeded(SharedValueAttribute attribute, string valueGroupName, List<SerializedProperty> properties)
        {
			foreach (var a in properties)
            {
				if (a.propertyType == SerializedPropertyType.Float)
				{
					if (a.floatValue < attribute.SharedMinValue)
					{
						UpdateSerializedPropertyValue(a, attribute.SharedMinValue);
					}
					if (a.floatValue > attribute.SharedMaxValue)
					{
						UpdateSerializedPropertyValue(a, attribute.SharedMaxValue);
					}
				}
			}
		}

		void RedistributeValuesByGroup(SharedValueAttribute attribute, string valueGroupName, List<SerializedProperty> properties, List<RecordedValue> oldProperties)
        {
			for (int i = 0; i < properties.Count; i++)
			{
				if (properties[i].floatValue != oldProperties[i].oldValue)
				{
					List<SerializedProperty> valuesToRedistribute = new List<SerializedProperty>();

					for (int j = 0; j < properties.Count; j++)
                    {
						if (j != i) valuesToRedistribute.Add(properties[j]);
                    }
					SmartRecalculateValues(attribute, properties[i], valuesToRedistribute);

					return;
				}
			}
		}

		void SmartRecalculateValues(SharedValueAttribute attribute, SerializedProperty valueInitiallyChanged, List<SerializedProperty> valuesToRedistribute)
		{
			float minValue = attribute.SharedMinValue;
			float maxValue = attribute.SharedMaxValue;
			float valueLeft = maxValue - valueInitiallyChanged.floatValue;

			if(valueLeft == 0)
            {
				foreach (var a in valuesToRedistribute)
					UpdateSerializedPropertyValue(a, 0f);
            }
            else
            {
				float tp = valuesToRedistribute.Sum(a => a.floatValue);
				if (tp <= 0f)
				{
					float avgValue = valueLeft / valuesToRedistribute.Count;
					foreach (var a in valuesToRedistribute)
						UpdateSerializedPropertyValue(a, avgValue);
                }
                else
                {
					foreach(var a in valuesToRedistribute)
                    {
						if (a.floatValue == 0) continue;

						float valueInPercents = a.floatValue / tp;
						UpdateSerializedPropertyValue(a, valueLeft * valueInPercents);
                    }
                }
			}
		}

		void UpdateSerializedPropertyValue(SerializedProperty a, float value)
        {
			a.serializedObject.Update();
			a.floatValue = value;
			a.serializedObject.ApplyModifiedProperties();
		}

		#region Recording Old Properties Related
		void RecordOldProperties(Dictionary<string, List<SerializedProperty>> valueGroups)
        {
			recordedProperties = new Dictionary<string, List<RecordedValue>>();
			foreach (var a in valueGroups.Keys)
            {
				List<RecordedValue> recordedValues = new List<RecordedValue>();

                for (int i = 0; i < valueGroups[a].Count; i++)
                {
					recordedValues.Add(new RecordedValue(valueGroups[a][i].floatValue, a, i));
                }
				recordedProperties[a] = recordedValues;
			}
        }

		bool HasValueChanged(SerializedProperty newProperty, RecordedValue oldProperty)
        {
			return newProperty.floatValue != oldProperty.oldValue;
		}

		Dictionary<string, List<RecordedValue>> recordedProperties;

		public class RecordedValue
        {
			public float oldValue;
			public string valueGroupName;
			public int id_InListGroup;

            public RecordedValue(float oldValue, string valueGroupName, int id_InListGroup)
            {
                this.oldValue = oldValue;
                this.valueGroupName = valueGroupName;
                this.id_InListGroup = id_InListGroup;
            }
        }
        #endregion
    }
}
