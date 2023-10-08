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
    public class PortData
    {
        public string PassageName
        {
            get { return _passageName; }
            set
            {
                _passageName = value;
                if (Input != null) Input.portName = value;
            }
        }
        private string _passageName;
        public Port Input;
        public Port Output;

        public PortData(string passageName, Port input, Port output)
        {
            _passageName = passageName;
            Input = input;
            Output = output;
        }
    }

    public class NodeView : UnityEditor.Experimental.GraphView.Node
    {
        public SceneData Data { get; private set; }

        public List<PortData> PortDatas = new List<PortData>();
        
        public NodeView(SceneData data)
        {
            Data = data;
            if (data == null)
                return;

            base.title = data.SceneName;
            
            data.OnDataChanged += OnDataChanged;

            viewDataKey = data.Guid;
            style.left = data.graphPosition.x;
            style.top = data.graphPosition.y;

            CreatePorts();
        }
        void OnDataChanged()
        {
            base.title = Data.SceneName;

            if(PortDatas.Count == Data.PassageNames.Count)
            {
                for(int i=0; i<PortDatas.Count; i++)
                {
                    PortDatas[i].PassageName = Data.PassageNames[i];
                }
            }
            else if (PortDatas.Count < Data.PassageNames.Count)
            {
                foreach (string passageName in Data.PassageNames)
                {
                    int matchIndex = PortDatas.FindIndex(x => x.PassageName == passageName);
                    if (matchIndex == -1)
                        PortDatas.Add(CreatePortData(passageName));
                }
            }
            else
            {
                foreach(PortData portData in PortDatas.ToArray())
                {
                    if (Data.PassageNames.FindIndex(x => x == portData.PassageName) == -1)
                    {
                        inputContainer.Remove(portData.Input);
                        //TEMP : Seperate output
                        outputContainer.Remove(portData.Output);
                        PortDatas.Remove(portData);
                    }
                }
            }
            RefreshPorts();
        }


        void CreatePorts()
        {
            foreach(string passageName in Data.PassageNames)
            {
                PortData portData = CreatePortData(passageName);
                PortDatas.Add(portData);
            }
        }
        PortData CreatePortData(string passageName)
        {
            Port inputPort = CreatePort(Direction.Input, passageName);
            inputContainer.Add(inputPort);

            Port outputPort = CreateMatchingOutputPort(inputPort);

            PortData portData = new PortData(passageName, inputPort, outputPort);
            return portData;
        }
        Port CreatePort(Direction direction, string name)
        {
            Port port = InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Single, typeof(SceneData));
            port.portName = name;
            return port;
        }
        
        Port CreateMatchingOutputPort(Port inputPort)
        {
            Port outputPort = CreatePort(Direction.Output, inputPort.portName);
            outputPort.Q<Label>().visible = false;
            outputPort.Q<Label>().style.maxWidth = 0;
            outputContainer.Add(outputPort);
            return outputPort;
        }
        public override void SetPosition(Rect newPos)
        {
            base.SetPosition(newPos);
            Data.graphPosition = new Vector2(newPos.x, newPos.y);
        }

        public override void OnSelected()
        {
            base.OnSelected();
            GetFirstAncestorOfType<LevelGraphView>().OnNodeClicked(this);
        }
    }
}
