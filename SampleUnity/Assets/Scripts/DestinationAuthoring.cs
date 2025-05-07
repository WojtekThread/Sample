using Unity.Entities;
using UnityEngine;

public struct Destination : IComponentData
{
	
}

public class DestinationBaker : Baker<DestinationAuthoring>
{
	public override void Bake(DestinationAuthoring authoring)
	{
		var entity = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent(entity, new Destination());
	}
}

public class DestinationAuthoring : MonoBehaviour
{
	
}