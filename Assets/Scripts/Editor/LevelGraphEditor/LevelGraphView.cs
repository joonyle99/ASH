using System;
using UnityEngine;
using UnityEditor.Experimental.GraphView;
using UnityEditor;
using UnityEngine.UIElements;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.UIElements;
using System.Drawing.Printing;

namespace LevelGraph
{
    public class LevelGraphView : GraphView
    {
        public new class UxmlFactory : UxmlFactory<LevelGraphView, GraphView.UxmlTraits> { }
        LevelGraphData _graphData;
        bool _hasGraph;

        VisualElement _nodeInspector;

        public LevelGraphView()
        {
            style.flexGrow = 1;
            Insert(0, new GridBackground());

            this.AddManipulator(new ContentZoomer());
            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ClickSelector());

        }
        public void SetNodeInspector(VisualElement nodeInspector)
        {
            _nodeInspector = nodeInspector;
        }
        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            TypeCache.TypeCollection types = TypeCache.GetTypesDerivedFrom<SceneData>();
            foreach (Type type in types)
            {
                if (type.IsAbstract) continue;

                evt.menu.AppendAction(
                  $"{type.Name}",
                  _ => CreateNode(type));
            }
        }
        public void ClearView()
        {
            
            _graphData = null;
            _hasGraph = false;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);

            _nodeInspector.Clear();
        }

        public void PopulateView(LevelGraphData graphData)
        {
            _graphData = graphData;
            _hasGraph = true;

            graphViewChanged -= OnGraphViewChanged;
            DeleteElements(graphElements);
            graphViewChanged += OnGraphViewChanged;

            foreach(SceneData data in _graphData.Nodes)
            {
                if (data is ExploreSceneData)
                    CreateNodeView(data as ExploreSceneData);
            }

            List<PassagePairData> _badPairs = new List<PassagePairData>();
            //TODO : Exception Danger Zone !!
            foreach(PassagePairData passagePair in _graphData.Edges)
            {
                NodeView entranceNode = GetNodeByGuid(passagePair.EntranceScene.Guid) as NodeView;
                NodeView exitNode = GetNodeByGuid(passagePair.ExitScene.Guid) as NodeView;

                try
                {
                    Port entrance = entranceNode.PortDatas.Find(x => x.PassageName == passagePair.EntrancePassage).Output;
                    Port exit = exitNode.PortDatas.Find(x => x.PassageName == passagePair.ExitPassgage).Input;

                    Edge edge = entrance.ConnectTo(exit);
                    AddTextToEdge(edge, ">>>>>>");
                    AddElement(edge);
                }
                catch(Exception ex)
                {
                    Debug.Log(ex);
                    _badPairs.Add(passagePair);
                    continue;
                }

            }
            _graphData.Edges.RemoveAll(x=> _badPairs.Contains(x));
        }
        private void AddTextToEdge(Edge edge, string text)
        {
            /*
            Label label = new Label(text);

            label.AddToClassList("edge-label"); // You can define a custom style for the label in your stylesheet

            edge.edgeControl.Add(label);
            label.style.left = new Length(50, LengthUnit.Percent);
            label.style.top = new Length(50, LengthUnit.Percent);
            */
        }
        private GraphViewChange OnGraphViewChanged(GraphViewChange graphViewChange)
        {
            if (graphViewChange.elementsToRemove != null)
            {
                foreach (GraphElement element in
                         graphViewChange.elementsToRemove)
                {
                    NodeView nodeView = element as NodeView;

                    if (nodeView != null)
                    {
                        DeleteNode(nodeView);
                    }

                    Edge edge = element as Edge;
                    if (edge != null)
                    {
                        UnlinkNodes(edge);
                    }
                }
            }
            if (graphViewChange.edgesToCreate != null)
            {
                foreach (Edge edge in graphViewChange.edgesToCreate)
                {
                    AddTextToEdge(edge, ">>>>>>");
                    LinkNodes(edge);
                }
            }
            return graphViewChange;
        }
        public void OnNodeClicked(NodeView clickedNodeView)
        {
            SerializedObject so = new SerializedObject(clickedNodeView.Data);
            _nodeInspector.Bind(so);

            Editor editor = Editor.CreateEditor(clickedNodeView.Data);
            IMGUIContainer inspectorIMGUI = new IMGUIContainer(() => { editor.OnInspectorGUI(); });
            _nodeInspector.Clear();
            _nodeInspector.Add(inspectorIMGUI);
        }

        void LinkNodes(Edge edgeToCreate)
        {
            PassagePairData passagePair = new PassagePairData();
            passagePair.EntrancePassage = (edgeToCreate.output).portName;
            passagePair.ExitPassgage = edgeToCreate.input.portName;
            passagePair.EntranceScene = (edgeToCreate.output.node as NodeView).Data;
            passagePair.ExitScene = (edgeToCreate.input.node as NodeView).Data;
            _graphData.Edges.Add(passagePair);
            AssetDatabase.SaveAssets();
        }
        void UnlinkNodes(Edge edgeToDelete)
        {
            PassagePairData passagePair = new PassagePairData();
            passagePair.EntrancePassage = (edgeToDelete.output).portName;
            passagePair.ExitPassgage = edgeToDelete.input.portName;
            passagePair.EntranceScene = (edgeToDelete.output.node as NodeView).Data;
            passagePair.ExitScene = (edgeToDelete.input.node as NodeView).Data;
            _graphData.Edges.Remove(passagePair);
            AssetDatabase.SaveAssets();
        }
        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            return ports.ToList()!.Where(endPort =>
                          endPort.direction != startPort.direction &&
                          endPort.node != startPort.node &&
                          endPort.portType == startPort.portType
                          )
              .ToList();
        }

        void CreateNodeView(ExploreSceneData node)
        {
            NodeView nodeView = new NodeView(node, _graphData);
            AddElement(nodeView);
        }
        void CreateNode(System.Type type)
        {
            if (!_hasGraph) 
                return;

            ExploreSceneData node = ScriptableObject.CreateInstance(type) as ExploreSceneData;
            node.name = type.Name;
            node.Guid = GUID.Generate().ToString();

            _graphData.Nodes.Add(node);

            CreateNodeView(node);

            AssetDatabase.AddObjectToAsset(node, _graphData);
            AssetDatabase.SaveAssets();
        }
        private void DeleteNode(NodeView node)
        {
            _nodeInspector.Unbind();
            _nodeInspector.Clear();

            foreach (PortData portData in node.PortDatas)
            {
                foreach (Edge edge in portData.Output.connections)
                    RemoveElement(edge);
                foreach (Edge edge in portData.Input.connections)
                    RemoveElement(edge);

            }
            var relatedEdges = _graphData.Edges.FindAll(x => x.EntranceScene == node.Data || x.ExitScene == node.Data);
            foreach (PassagePairData edge in relatedEdges)
                _graphData.Edges.Remove(edge);

            _graphData.Nodes.Remove(node.Data);

            AssetDatabase.RemoveObjectFromAsset(node.Data);
            AssetDatabase.SaveAssets();
        }

    }
}
