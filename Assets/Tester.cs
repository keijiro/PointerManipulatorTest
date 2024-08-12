using UnityEngine;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;

sealed class DragReceiver : PointerManipulator
{
    Tester _tester;

    public DragReceiver(Tester tester)
    {
        _tester = tester;
        activators.Add(new ManipulatorActivationFilter{button = MouseButton.LeftMouse});
    }

    protected override void RegisterCallbacksOnTarget()
      => target.RegisterCallback<PointerMoveEvent>(OnPointerMove);

    protected override void UnregisterCallbacksFromTarget()
      => target.UnregisterCallback<PointerMoveEvent>(OnPointerMove);

    void OnPointerMove(PointerMoveEvent e)
    {
        if (e.pressedButtons == 1) _tester.OnPointerDrag(e.position, e.deltaPosition);
        e.StopPropagation();
    }
}

sealed class Tester : MonoBehaviour
{
    Queue<string> _lines = new Queue<string>();

    VisualElement UIRoot => GetComponent<UIDocument>().rootVisualElement;

    public void OnPointerDrag(Vector3 position, Vector3 delta)
    {
        _lines.Enqueue($"{position} | {delta}");
        while (_lines.Count > 20) _lines.Dequeue();
        UIRoot.Q<Label>("text-label").text = string.Join("\n", _lines.Reverse());
    }

    void Start()
      => UIRoot.Q("empty-area").AddManipulator(new DragReceiver(this));
}