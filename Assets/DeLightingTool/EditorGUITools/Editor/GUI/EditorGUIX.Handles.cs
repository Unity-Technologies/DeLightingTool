using UnityEngine;

namespace UnityEditor.Experimental
{
    public static partial class EditorGUIX
    {
        const float kPickDistance = 5f;

        public static Vector2 PositionHandle2D(int controlId, Vector2 position, float size)
        {
            var evt = Event.current;;
            var mousePosition = evt.mousePosition;
            switch (evt.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    {
                        var sqrDistance = (mousePosition - position).sqrMagnitude;
                        if (sqrDistance < size * size)
                        {
                            EditorGUIUtility.hotControl = controlId;
                            EditorGUIUtility.keyboardControl = 0;
                            evt.Use();
                        }
                        break;
                    }
                case EventType.MouseDrag:
                case EventType.MouseMove:
                    {
                        if (EditorGUIUtility.hotControl == controlId)
                        {
                            position = mousePosition;
                            evt.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (EditorGUIUtility.hotControl == controlId)
                        {
                            EditorGUIUtility.hotControl = 0;
                            evt.Use();
                        }
                        break;
                    }
                case EventType.Repaint:
                    {
                        var primaryColor = Handles.color;
                        if (EditorGUIUtility.hotControl == controlId)
                            primaryColor = Handles.selectedColor;
                        
                        Handles.color = primaryColor;
                        Handles.DrawSolidDisc(position, Vector3.forward, size);
                        Handles.color = primaryColor * 0.3f;
                        Handles.DrawWireDisc(position, Vector3.forward, size * 0.9f);
                        break;
                    }
            }

            return position;
        }

        public static float RadiusHandle2D(int controlId, Vector2 position, float radius)
        {
            var evt = Event.current;
            switch (evt.GetTypeForControl(controlId))
            {
                case EventType.MouseDown:
                    {
                        var dst = HandleUtility.DistanceToDisc(position, Vector3.forward, radius);
                        if (dst <= kPickDistance)
                        {
                            EditorGUIUtility.hotControl = controlId;
                            EditorGUIUtility.keyboardControl = 0;
                            evt.Use();
                        }
                        break;
                    }
                case EventType.MouseDrag:
                case EventType.MouseMove:
                    {
                        if (EditorGUIUtility.hotControl == controlId)
                        {
                            var vector = evt.mousePosition - position;
                            radius = vector.magnitude;
                            evt.Use();
                        }
                        break;
                    }
                case EventType.MouseUp:
                    {
                        if (EditorGUIUtility.hotControl == controlId)
                        {
                            EditorGUIUtility.hotControl = 0;
                            evt.Use();
                        }
                        break;
                    }
                case EventType.Repaint:
                    var primaryColor = Handles.color;
                    if (EditorGUIUtility.hotControl == controlId)
                        primaryColor = Handles.selectedColor;

                    Handles.color = primaryColor;
                    Handles.DrawWireDisc(position, Vector3.forward, radius);
                    break;
            }

            return radius;
        }
    }
}
