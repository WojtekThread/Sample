using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
	public interface IConditionFulfillment
	{

	}

	[Serializable]
	public abstract class Condition : IEqualityComparer<Condition>
	{
		[SerializeField][HideInInspector]
		private string _name;

		protected Condition()
		{
			_name = this.GetType().Name;
		}

		public abstract bool IsFullFilled(IConditionFulfillment[] fulfillmentCollection);

		public bool Equals(Condition x, Condition y)
		{
			if (ReferenceEquals(x, y)) return true;
			if (ReferenceEquals(x, null)) return false;
			if (ReferenceEquals(y, null)) return false;
			if (x.GetType() != y.GetType()) return false;

			return string.Equals(x._name, y._name);
		}

		public int GetHashCode(Condition obj)
		{
			return obj._name.GetHashCode();
		}
	}
}