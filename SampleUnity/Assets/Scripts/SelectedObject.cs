using Unity.Entities;
using UnityEngine;
using UnityEngine.UI;

public class SelectedObject : MonoBehaviour
{
	[SerializeField]
	private Image _img; 
	
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
	    var selectionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SelectionSystem>();
	    selectionSystem.SelectionChanged += UpdateState;
    }

    private void OnDestroy()
    {
	    if (World.DefaultGameObjectInjectionWorld != null)
	    {
		    var selectionSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystemManaged<SelectionSystem>();
		    selectionSystem.SelectionChanged -= UpdateState;
	    }
    }

    private void UpdateState()
    {
	    var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
	    var interactableQuery = entityManager.CreateEntityQuery(new ComponentType[]{typeof(SelectionSingleton)});

	    if (interactableQuery.TryGetSingleton(out SelectionSingleton currentSelection))
	    {
		    _img.enabled = currentSelection.CurrentlySelected != Entity.Null;
	    }
	    else
	    {
		    _img.enabled = false;
	    }
    }
}
