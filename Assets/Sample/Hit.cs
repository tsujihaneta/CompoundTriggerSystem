using System.Collections;
using System.Collections.Generic;
using CompoundTriggerSystem;
using UnityEngine;

public class Hit : MonoBehaviour, ITriggerGroupEventHandler
{
	List<MeshRenderer> meshRenderers = new List<MeshRenderer>();

	[SerializeField]
	Material defaultMaterial;

	[SerializeField]
    Material hitMaterial;

	private void Awake()
	{
		meshRenderers.Add(GetComponent<MeshRenderer>());
	}

	public void OnTriggerGroupEnter(TriggerGroup group)
	{
		if (meshRenderers != null)
		{
			foreach (var meshRenderer in meshRenderers)
			{
				meshRenderer.material = hitMaterial;
			}
		}

		Debug.Log("Trigger Group Enter " + group.name);
	}

	public void OnTriggerGroupExit(TriggerGroup group)
	{
		if (meshRenderers != null)
		{
			foreach (var meshRenderer in meshRenderers)
			{
				meshRenderer.material = defaultMaterial;
			}
		}

		Debug.Log("Trigger Group Exit " + group.name);
	}
}
