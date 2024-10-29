
public class TreeNode
{
}

public class RootNode : TreeNode
{
    //同一程序集可用
    internal SubNode subNode;
    internal void Start()
    {
        subNode.Start();
    }
    
}

public class SubNode : TreeNode
{
    //同一程序集可用
    internal void Start()
    {
    }
}
