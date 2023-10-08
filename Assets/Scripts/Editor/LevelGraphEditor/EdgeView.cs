using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Graphs;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine.UIElements;
using System.Drawing.Printing;
using UnityEditor.UIElements;
using Unity.VisualScripting;

namespace LevelGraph
{
    public class EdgeView : UnityEditor.Experimental.GraphView.Edge
    {
        private Label label;

        public EdgeView()
        {
            // Create a label element
            label = new Label();
            label.AddToClassList("edge-label"); // You can define a custom style for the label in your stylesheet
            Add(label);
        }

        // Custom method to set the text of the edge
        public void SetLabelText(string text)
        {
            label.text = text;
        }
    }
}
