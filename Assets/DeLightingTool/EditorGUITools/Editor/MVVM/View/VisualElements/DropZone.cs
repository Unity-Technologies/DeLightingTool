using System;
using UnityEditor.Experimental.MVVM;
using UnityEngine;
using Object = UnityEngine.Object;

namespace UnityEditor.Experimental.VisualElements
{
    public class DropZone : IMGUIVisualContainer
    {
        public static readonly DependencyProperty<DragAndDropVisualMode> propertyCanAcceptDrop = new DependencyProperty<DropZone, DragAndDropVisualMode>(
            "canAcceptDrop",
            elt => elt.canAcceptDrop,
            (elt, v) => elt.canAcceptDrop = v,
            v => (DragAndDropVisualMode)v);

        public static readonly DependencyProperty<DropEventArgs> propertyOnPotentialDrop = new DependencyProperty<DropZone, DropEventArgs>(
            "onPotentialDrop",
            new RoutedEvent<DropZone, DropEventArgs>((d, a) => d.PotentialDrop += a, (d, a) => d.PotentialDrop -= a));

        public static readonly DependencyProperty<DropEventArgs> propertyOnDrop = new DependencyProperty<DropZone, DropEventArgs>(
            "onDrop",
            new RoutedEvent<DropZone, DropEventArgs>((d, a) => d.Dropped += a, (d, a) => d.Dropped -= a));

        public class DropEventArgs
        {
            public readonly Object[] objects;
            public readonly string[] paths;

            public DropEventArgs(Object[] objects, string[] paths)
            {
                this.objects = objects;
                this.paths = paths;
            }
        }

        static readonly int s_DropZoneHash = "DropZoneHash".GetHashCode();

        public event Action<DropEventArgs> Dropped;
        public event Action<DropEventArgs> PotentialDrop;

        DragAndDropVisualMode m_CanAcceptDrop = DragAndDropVisualMode.Rejected;
        public DragAndDropVisualMode canAcceptDrop
        {
            get { return m_CanAcceptDrop; }
            set { m_CanAcceptDrop = value; }
        }

        Rect m_Rect;
        public override void OnGUI()
        {
            var controlId = GUIUtility.GetControlID(s_DropZoneHash, FocusType.Passive);
            var rect = GUILayoutUtility.GetRect(0, float.MaxValue, 0, float.MaxValue, GUIStyle.none, GUILayout.ExpandHeight(true), GUILayout.ExpandWidth(true));
            // On layout event, the rect returned is a dummy (0, 0, 1, 1) and the request is stored
            // On repaint event, the rect has the coorect value
            // But, GUILayout.BeginArea register the size on the layout event only
            // So we need to give to GUILayout.BeginArea the rect calculated from the previous repaint event
            if (Event.current.type == EventType.Repaint)
                m_Rect = rect;
            if (m_Rect.x == 0 && m_Rect.y == 0 && m_Rect.width == 0 && m_Rect.height == 0)
                Repaint();
            GUILayout.BeginArea(m_Rect);
            base.OnGUI();
            GUILayout.EndArea();
            if (EditorGUIX.DropZone(controlId, m_Rect, DropCheck) && Dropped != null)
                Dropped(new DropEventArgs(DragAndDrop.objectReferences, DragAndDrop.paths));
        }

        DragAndDropVisualMode DropCheck(Object[] objs, string[] paths)
        {
            if (PotentialDrop != null)
                PotentialDrop(new DropEventArgs(objs, paths));
            return canAcceptDrop;
        }
    }
}
