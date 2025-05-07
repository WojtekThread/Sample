using Unity.Burst;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

public struct Movable : IComponentData
{
	public float3 CurrentDestination;
	public float CurrentSpeed;
	public bool ShouldMove;
}

[BurstCompile]
public partial struct MoveSystem : ISystem
{  
	public void OnCreate(ref SystemState state) { }

	public void OnDestroy(ref SystemState state) { }

	[BurstCompile]
	public void OnUpdate(ref SystemState state)
	{
		// Creates a new instance of the job, assigns the necessary data, and schedules the job in parallel.
		new ProcessMoveJob
		{
			DeltaTime = SystemAPI.Time.DeltaTime,
		}.ScheduleParallel();
	}
	
	[BurstCompile]
	public partial struct ProcessMoveJob : IJobEntity
	{
		public float DeltaTime;

		private void Execute(ref Movable movable, ref LocalTransform localTransform)
		{
			if (movable.ShouldMove)
			{
				var direction = movable.CurrentDestination - localTransform.Position;
				var distance = math.length(direction);
				
				//check if arrived at destination
				if (distance < math.EPSILON)
				{
					localTransform.Position = movable.CurrentDestination;
					movable.ShouldMove = false;
					movable.CurrentDestination = float3.zero;
				}
				else
				{
					var normalizedDirection = math.normalize(direction);
					var move = DeltaTime * movable.CurrentSpeed * normalizedDirection;
					var moveLength = math.length(move);

					if (moveLength > distance)
					{
						localTransform.Position += direction;
					}
					else
					{
						localTransform.Position += move;
					}
				}
			}
		}
	}
}
