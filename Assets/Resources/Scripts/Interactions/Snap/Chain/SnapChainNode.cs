using System.Collections.Generic;
using UnityEngine;

public class SnapChainNode : MonoBehaviour
{
    [SerializeField]
    private SnapHost localHost;

    private readonly List<SnapChainNode> children =
        new List<SnapChainNode>();

    public SnapChainNode Parent { get; private set; }

    void Awake()
    {
        if (localHost == null)
            localHost = GetComponent<SnapHost>();
    }

    public SnapHost RootHost =>
        localHost != null
        ? localHost.Root
        : null;

    public int CountSubtree()
    {
        return CountSubtreeSafe(
            new HashSet<SnapChainNode>()
        );
    }

    int CountSubtreeSafe(
        HashSet<SnapChainNode> visited)
    {
        if (!visited.Add(this))
            return 0;

        int count = 1;

        foreach (SnapChainNode child
            in children)
        {
            if (child != null)
            {
                count +=
                    child.CountSubtreeSafe(
                        visited
                    );
            }
        }

        return count;
    }

    public bool CanAttachTo(
        SnapChainNode newParent)
    {
        if (newParent == null)
            return false;

        if (newParent == this)
            return false;

        if (IsAncestorOf(newParent))
            return false;

        return true;
    }

    public void AttachTo(
        SnapChainNode newParent,
        SnapHost root)
    {
        if (!CanAttachTo(newParent))
            return;

        SnapChainNode oldRoot =
            GetRootNode();

        if (Parent != null)
            Parent.children.Remove(this);

        Parent = newParent;

        if (!newParent.children.Contains(this))
            newParent.children.Add(this);

        SetRootRecursive(
            root,
            new HashSet<SnapChainNode>()
        );

        RecalculateRoot(oldRoot);
        RecalculateRoot(newParent);
    }

    public void DetachAsNewRoot()
    {
        SnapChainNode oldRoot =
            GetRootNode();

        if (Parent != null)
            Parent.children.Remove(this);

        Parent = null;

        if (localHost != null)
        {
            SetRootRecursive(
                localHost,
                new HashSet<SnapChainNode>()
            );
        }

        RecalculateRoot(oldRoot);
        RecalculateRoot(this);
    }

    public void DetachFromParentOnly()
    {
        SnapChainNode oldRoot =
            GetRootNode();

        if (Parent != null)
        {
            Parent.children.Remove(this);
        }

        Parent = null;

        if (localHost != null)
        {
            SetRootRecursive(
                localHost,
                new HashSet<SnapChainNode>()
            );
        }

        RecalculateRoot(oldRoot);
        RecalculateRoot(this);
    }

    public void DetachChildrenBeforeDestroy()
    {
        if (children.Count <= 0)
            return;

        List<SnapChainNode> copy =
            new List<SnapChainNode>(
                children
            );

        children.Clear();

        foreach (SnapChainNode child
            in copy)
        {
            if (child == null)
                continue;

            child.Parent = null;

            if (child.localHost != null)
            {
                child.SetRootRecursive(
                    child.localHost,
                    new HashSet<SnapChainNode>()
                );
            }

            child.transform.SetParent(
                null,
                true
            );

            SnappableObject snap =
                child.GetComponent
                <SnappableObject>();

            if (snap != null &&
                snap.IsSnapped)
            {
                snap.ForceUnsnapWithoutSocketClear();
            }

            Rigidbody rb =
                child.GetComponent
                <Rigidbody>();

            if (rb != null)
            {
                rb.isKinematic = false;

                rb.useGravity = true;

                rb.WakeUp();
            }

            RecalculateRoot(child);
        }
    }

    public int GetDepth()
    {
        int depth = 0;

        SnapChainNode node =
            Parent;

        while (node != null)
        {
            depth++;

            node = node.Parent;
        }

        return depth;
    }

    bool IsAncestorOf(
        SnapChainNode possibleChild)
    {
        SnapChainNode node =
            possibleChild;

        while (node != null)
        {
            if (node == this)
                return true;

            node = node.Parent;
        }

        return false;
    }

    SnapChainNode GetRootNode()
    {
        SnapChainNode node = this;

        while (node.Parent != null)
            node = node.Parent;

        return node;
    }

    void SetRootRecursive(
        SnapHost root,
        HashSet<SnapChainNode> visited)
    {
        if (!visited.Add(this))
            return;

        if (localHost != null)
            localHost.SetRoot(root);

        foreach (SnapChainNode child
            in children)
        {
            if (child != null)
            {
                child.SetRootRecursive(
                    root,
                    visited
                );
            }
        }
    }

    void RecalculateRoot(
        SnapChainNode node)
    {
        if (node == null)
            return;

        SnapChainNode rootNode =
            node.GetRootNode();

        if (rootNode.localHost == null)
            return;

        rootNode.localHost.SetCurrentItems(
            rootNode.CountSubtree()
        );
    }
}