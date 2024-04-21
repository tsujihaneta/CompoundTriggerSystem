using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace CompoundTriggerSystem
{
	public class TriggerObserver : MonoBehaviour
	{
		public TriggerGroup Group
		{
			get; private set;
		}

		public void Init(TriggerGroup group)
		{
			this.Group = group;

			var self = GetComponent<Collider>();
			var others = new Collider[32];
			int count = OverlapNonAlloc(self, others);
			for (int i = 0; i < count; i++)
			{
				var other = others[i];
				if (other == self)
				{
					continue;
				}

				var otherObserver = other.GetComponent<TriggerObserver>();
				if (otherObserver == null)
				{
					continue;
				}

				OnObserverEnter(otherObserver);
				otherObserver.OnObserverEnter(this);
			}
		}

		private void OnTriggerEnter(Collider other)
		{
			var otherObserver = other.GetComponent<TriggerObserver>();
			if (otherObserver == null)
			{
				return;
			}

			OnObserverEnter(otherObserver);
		}

		private void OnTriggerExit(Collider other)
		{
			var otherObserver = other.GetComponent<TriggerObserver>();
			if (otherObserver == null)
			{
				return;
			}

			OnObserverExit(otherObserver);
		}

		private void OnObserverEnter(TriggerObserver otherObserver)
		{
			this.Group.OnObserverEnter(this, otherObserver);
		}

		private void OnObserverExit(TriggerObserver otherObserver)
		{
			this.Group.OnObserverExit(this, otherObserver);
		}

		private void OnDisable()
		{
			var group = this.Group;
			if (group == null)
			{
				return;
			}

			var table = group.Table;
			if (table == null)
			{
				return;
			}

			var observers = table.SelectMany(item => item.Value).ToArray();
			foreach (var observer in observers)
			{
				if (observer == null)
				{
					continue;
				}

				var otherObserver = observer.Other;
				otherObserver.OnObserverExit(this);
				OnObserverExit(otherObserver);
			}
		}

		private int OverlapNonAlloc(Collider self, Collider[] colliders)
		{
			// レイヤーマスクの作成
			var layerMask = LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer));

			var targetSphereCollider = self as SphereCollider;
			if (targetSphereCollider != null)
			{
				// 球の中心と半径を取得
				Vector3 center = targetSphereCollider.transform.TransformPoint(targetSphereCollider.center);
				float radius = targetSphereCollider.radius;

				return Physics.OverlapSphereNonAlloc(center, radius, colliders, layerMask);
			}

			var targetCapsuleCollider = self as CapsuleCollider;
			if (targetCapsuleCollider != null)
			{
				// カプセルの始点、終点、半径を取得
				Vector3 point1 = targetCapsuleCollider.transform.TransformPoint(targetCapsuleCollider.center + Vector3.up * targetCapsuleCollider.height * 0.5f - Vector3.up * targetCapsuleCollider.radius);
				Vector3 point2 = targetCapsuleCollider.transform.TransformPoint(targetCapsuleCollider.center - Vector3.up * targetCapsuleCollider.height * 0.5f + Vector3.up * targetCapsuleCollider.radius);
				float radius = targetCapsuleCollider.radius;

				// カプセルに沿ってオーバーラップ判定を行う
				return Physics.OverlapCapsuleNonAlloc(point1, point2, radius, colliders, layerMask);
			}

			var targetBoxCollider = self as BoxCollider;
			if (targetBoxCollider != null)
			{
				// ボックスの中心とサイズを取得
				Vector3 center = targetBoxCollider.transform.TransformPoint(targetBoxCollider.center);
				Vector3 size = targetBoxCollider.size * 0.5f; // Unityではサイズがハーフサイズなので修正

				// ボックスに沿ってオーバーラップ判定を行う
				return Physics.OverlapBoxNonAlloc(center, size, colliders, targetBoxCollider.transform.rotation, layerMask);
			}

			var targetCharacterController = self as CharacterController;
			if (targetCharacterController != null)
			{
				// カプセルの始点、終点、半径を取得
				Vector3 point1 = targetCharacterController.transform.TransformPoint(targetCharacterController.center + Vector3.up * targetCharacterController.height * 0.5f - Vector3.up * targetCharacterController.radius);
				Vector3 point2 = targetCharacterController.transform.TransformPoint(targetCharacterController.center - Vector3.up * targetCharacterController.height * 0.5f + Vector3.up * targetCharacterController.radius);
				float radius = targetCharacterController.radius;

				// カプセルに沿ってオーバーラップ判定を行う
				return Physics.OverlapCapsuleNonAlloc(point1, point2, radius, colliders, layerMask);
			}

			{
				Vector3 center = self.transform.position;
				float radius = 0.0f;

				return Physics.OverlapSphereNonAlloc(center, radius, colliders, layerMask);
			}
		}
	}
}
