using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sequence : Node
{
    public Sequence() : base() { }
    public Sequence(List<Node> children) : base(children) { }

    public override NodeState Execute()
    {
        bool anyChildIsRunning = false;

        foreach (Node child in children)
        {
            switch (child.Execute())
            {
                case NodeState.FAILURE:
                    state = NodeState.FAILURE; return state;
                case NodeState.SUCCESS:
                    continue;
                case NodeState.RUNNING:
                    anyChildIsRunning = true; continue;
                default:
                    state = NodeState.SUCCESS; return state;
            }
        }

        state = anyChildIsRunning ? NodeState.RUNNING : NodeState.SUCCESS;
        return state;
    }
}
