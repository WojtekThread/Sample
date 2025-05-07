using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;

[BurstCompile]
public partial class SetDestinationSystem : SystemBase
{
	private Camera _mainCamera;
	private Camera GetCamera()
	{
		if (_mainCamera == null)
		{
			_mainCamera = Camera.main;
		}

		return _mainCamera;
	}

	protected override void OnCreate()
	{
		MainInput.Actions.Player.Defend.performed += TrySetDestination;
		base.OnCreate();
	}

	protected override void OnDestroy()
	{
		MainInput.Actions.Player.Defend.performed -= TrySetDestination;
		base.OnDestroy();
	}

	protected override void OnUpdate()
	{
	}
	
	[BurstCompile]
	private void TrySetDestination(InputAction.CallbackContext obj)
	{
		if (SystemAPI.TryGetSingletonRW(out RefRW<SelectionSingleton> selection) && selection.ValueRW.CurrentlySelected != Entity.Null)
		{
			var ray = GetCamera().ScreenPointToRay(Mouse.current.position.ReadValue());
			NativeList<Unity.Physics.RaycastHit> hitsCollection = new NativeList<Unity.Physics.RaycastHit>(25, Allocator.Temp);
			var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
			if (physicsWorldSingleton.CastRay(new RaycastInput()
			    {
				    Start = ray.origin,
				    End = ray.direction * 1000000f,
				    Filter = CollisionFilter.Default
			    }, ref hitsCollection))
			{

				foreach (var hit in hitsCollection)
				{
					if (FindDestination(hit.Entity, out Entity destination))
					{
						var movable = SystemAPI.GetComponentRW<Movable>(selection.ValueRW.CurrentlySelected);
						movable.ValueRW.CurrentDestination = hit.Position;
						movable.ValueRW.ShouldMove = true;

						break;
					}
				}
			}
			Debug.Log($"Destination hits: {hitsCollection.Length}");
		}
	}
	
	private bool FindDestination(Entity entity, out Entity selectable)
	{
		selectable = Entity.Null;
		if (SystemAPI.HasComponent<Destination>(entity))
		{
			selectable = entity;
			return true;
		}

		if (SystemAPI.HasComponent<Parent>(entity))
		{
			Parent parentEntity = SystemAPI.GetComponent<Parent>(entity);

			if (parentEntity.Value != Entity.Null)
			{
				return FindDestination(parentEntity.Value, out selectable);
			}
		}

		return false;
	}
}