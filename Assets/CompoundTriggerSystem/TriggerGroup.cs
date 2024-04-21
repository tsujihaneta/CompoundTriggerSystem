using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

namespace CompoundTriggerSystem
{
	public class TriggerGroup : MonoBehaviour
	{
		private readonly Dictionary<TriggerGroup, List<TriggerInfo>> m_table = new Dictionary<TriggerGroup, List<TriggerInfo>>();
		public Dictionary<TriggerGroup, List<TriggerInfo>> Table
		{
			get
			{
				return m_table;
			}
		}

		private void Awake()
		{
			Initialize();
		}

		private void Initialize()
		{
			AttachScript(transform);

			foreach (Transform child in transform)
			{
				AttachScriptChildren(child);
			}
		}

		private void AttachScript(Transform t)
		{
			if (t.GetComponent<Collider>() == null)
			{
				return;
			}

			var observer = t.GetComponent<TriggerObserver>();
			if (observer != null)
			{
				DestroyImmediate(observer);
			}

			observer = t.AddComponent<TriggerObserver>();
			observer.Init(this);
		}

		private void AttachScriptChildren(Transform parent)
		{
			if (parent.GetComponent<TriggerGroup>())
			{
				return;
			}

			AttachScript(parent);

			foreach (Transform child in parent)
			{
				AttachScriptChildren(child);
			}
		}

		public void OnObserverEnter(TriggerObserver selfObserver, TriggerObserver otherObserver)
		{
			if (selfObserver.Group != this)
			{
				return;
			}

			bool isGroupAdded = false;

			var otherGroup = otherObserver.Group;
			if (!Table.TryGetValue(otherGroup, out List<TriggerInfo> list))
			{
				list = new List<TriggerInfo>();
				Table.Add(otherGroup, list);
				isGroupAdded = true;
			}

			list.Add(new TriggerInfo(selfObserver, otherObserver));

			if (isGroupAdded)
			{
				foreach (var e in GetComponents<ITriggerGroupEventHandler>())
				{
					if (e == null)
					{
						continue;
					}

					e.OnTriggerGroupEnter(otherGroup);
				}
			}
		}

		public void OnObserverExit(TriggerObserver selfObserver, TriggerObserver otherObserver)
		{
			if (selfObserver.Group != this)
			{
				return;
			}

			bool isGroupRemoved = false;

			var otherGroup = otherObserver.Group;
			if (!Table.TryGetValue(otherGroup, out List<TriggerInfo> list))
			{
				return;
			}

			list.RemoveAll(item => item.Self == selfObserver && item.Other == otherObserver);

			if (!list.Any())
			{
				Table.Remove(otherGroup);
				isGroupRemoved = true;
			}

			if (isGroupRemoved)
			{
				foreach (var e in GetComponents<ITriggerGroupEventHandler>())
				{
					if (e == null)
					{
						continue;
					}

					e.OnTriggerGroupExit(otherGroup);
				}
			}
		}
	}
}
