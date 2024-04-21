using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CompoundTriggerSystem
{
	public class TriggerInfo
	{
		public TriggerObserver Self { get; }
		public TriggerObserver Other { get; }

		public TriggerInfo(TriggerObserver self, TriggerObserver other)
		{
			this.Self = self;
			this.Other = other;
		}
	}
}
