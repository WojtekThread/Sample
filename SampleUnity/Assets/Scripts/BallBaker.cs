using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public class BallBaker : Baker<BallAuthoring>
{
	public override void Bake(BallAuthoring authoring)
	{
		var entity = GetEntity(TransformUsageFlags.Dynamic);
		AddComponent(entity, new Movable()
		{
			CurrentSpeed = 1f,
			CurrentDestination = float3.zero,
			ShouldMove = false,
		});
		AddComponent(entity, new LocalTransform()
		{
			Position = authoring.transform.position
		});
		AddComponent<Selectable>(entity);
	}
}