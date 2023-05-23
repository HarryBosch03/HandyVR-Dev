using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Toolbarinator.Editor
{
    [InitializeOnLoad]
    public static class Toolbarinator
    {
        private static readonly Type ToolbarType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
        
        private static readonly List<ToolbarElement> Elements;

        private static ScriptableObject toolbar;
        
        static Toolbarinator()
        {
            Elements = new List<ToolbarElement>();
            
            var types = typeof(ToolbarElement).Assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ToolbarElement)));
            foreach (var type in types)
            {
                Elements.Add(Activator.CreateInstance(type) as ToolbarElement);
            }

            EditorApplication.update += Update;
        }

        private static Action ImGuiCallback(Func<ToolbarElement, Action> drawMethod)
        {
            return () =>
            {
                foreach (var element in Elements)
                {
                    drawMethod(element)();
                }
            };
        }
        
        private static void Update()
        {
            if (toolbar) return;
            
            var toolbars = Resources.FindObjectsOfTypeAll(ToolbarType);
            toolbar = toolbars.Length > 0 ? (ScriptableObject)toolbars[0] : null;
            if (!toolbar) return;

            var rootField = toolbar.GetType().GetField("m_Root", BindingFlags.NonPublic | BindingFlags.Instance);
            if (rootField == null) return;
            
            var root = rootField.GetValue(toolbar) as VisualElement;
            registerCallback("ToolbarZoneLeftAlign", ImGuiCallback(e => e.ToolbarLeft));
            registerCallback("ToolbarZoneRightAlign", ImGuiCallback(e => e.ToolbarRight));
            
            void registerCallback(string toolbarParentReference, Action callback) 
            {
                var toolbarZone = root.Q(toolbarParentReference);

                var parent = new VisualElement()
                {
                    style = {
                        flexGrow = 1,
                        flexDirection = FlexDirection.Row,
                    }
                };
                var imguiContainer = new IMGUIContainer();
                imguiContainer.style.flexGrow = 1;
                imguiContainer.onGUIHandler += callback;
                parent.Add(imguiContainer);
                toolbarZone.Add(parent);
            }
        }
    }
}