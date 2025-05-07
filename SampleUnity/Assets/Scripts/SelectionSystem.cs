using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Transforms;
using UnityEngine;
using UnityEngine.InputSystem;
using Ray = UnityEngine.Ray;
using RaycastHit = Unity.Physics.RaycastHit;

public struct SelectionSingleton : IComponentData
{
	public Entity CurrentlySelected;
}
public struct Selectable : IComponentData
{
	
}

public class SelectableAuthoring : MonoBehaviour
{
	
}

public static class MainInput
{
	public static InputSystem_Actions Actions
	{
		get
		{
			if (_actions == null)
			{
				_actions = new InputSystem_Actions();
				_actions.Enable();
				_actions.Player.Enable();
			}

			return _actions;
		}
	}

	private static InputSystem_Actions _actions;
}

[BurstCompile]
public partial class SelectionSystem : SystemBase
{
	public event System.Action SelectionChanged;
	
	private Camera _mainCamera;

	protected override void OnCreate()
	{
		var entityManager = CheckedStateRef.EntityManager;
		var entity = entityManager.CreateEntity();
		entityManager.AddComponent<SelectionSingleton>(entity);
		var selection = SystemAPI.GetComponentRW<SelectionSingleton>(entity);
		selection.ValueRW.CurrentlySelected = Entity.Null;

		MainInput.Actions.Player.Attack.performed += SelectUnit;
	}

	private void SelectUnit(InputAction.CallbackContext obj)
	{
		SelectUnit();
	}
	
	private Camera GetCamera()
	{
		if (_mainCamera == null)
		{
			_mainCamera = Camera.main;
		}

		return _mainCamera;
	}
	
	[BurstCompile]
	private void SelectUnit()
	{
		Ray ray = GetCamera().ScreenPointToRay(Mouse.current.position.ReadValue());
		NativeList<RaycastHit> hitsCollection = new NativeList<RaycastHit>(25, Allocator.Temp);
		var physicsWorldSingleton = SystemAPI.GetSingleton<PhysicsWorldSingleton>().PhysicsWorld;
		if (physicsWorldSingleton.CastRay(new RaycastInput()
		    {
			    Start = ray.origin,
			    End = ray.direction * 10000f,
			    Filter = CollisionFilter.Default
		    }, ref hitsCollection))
		{
			RemoveCurrentSelection();

			foreach (var hit in hitsCollection)
			{
				if (FindSelectable(hit.Entity, out Entity selectable))
				{
					var selection = SystemAPI.GetSingletonRW<SelectionSingleton>();
					selection.ValueRW.CurrentlySelected = selectable;

					break;
				}
			}
		}
		else
		{
			RemoveCurrentSelection();
		}
		
		SelectionChanged?.Invoke();
		hitsCollection.Dispose();
	}

	private bool FindSelectable(Entity entity, out Entity selectable)
	{
		selectable = Entity.Null;
		if (SystemAPI.HasComponent<Selectable>(entity))
		{
			selectable = entity;
			return true;
		}

		if (SystemAPI.HasComponent<Parent>(entity))
		{
			Parent parentEntity = SystemAPI.GetComponent<Parent>(entity);

			if (parentEntity.Value != Entity.Null)
			{
				return FindSelectable(parentEntity.Value, out selectable);
			}
		}

		return false;
	}
	
	private void RemoveCurrentSelection()
	{
		var selection = SystemAPI.GetSingletonRW<SelectionSingleton>();
		selection.ValueRW.CurrentlySelected = Entity.Null;
	}

	protected override void OnUpdate()
	{
		
	}
}