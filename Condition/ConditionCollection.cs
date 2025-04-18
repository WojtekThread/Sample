using System;
using UnityEngine;
using System.Collections.Generic;

namespace Core
{
	[Serializable]
	public class ConditionCollection
	{
		[SerializeReference]
		private List<Condition> _listOfConditions = new List<Condition>();

		public bool AreConditionsMet(IConditionFulfillment[] fulfillmentCollection)
		{
			foreach (var condition in _listOfConditions)
			{
				if (condition.IsFullFilled(fulfillmentCollection) == false)
				{
					return false;
				}
			}

			return true;
		}

		public void AddCondition(Condition condition)
		{
			_listOfConditions.Add(condition);
		}

		public void RemoveCondition(Condition condition)
		{
			if (_listOfConditions.Contains(condition))
			{
				_listOfConditions.Remove(condition);
			}
			
		}
	}
}