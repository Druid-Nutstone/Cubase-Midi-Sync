using Cubase.Midi.Sync.Common.Keys;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cubase.Midi.Sync.Configuration.UI.Controls.Keys
{
    public class KeysTreeView : TreeView    
    {
        private CubaseKeyCommandCollection cubaseKeyCommands;

        private Panel dataPanel;

        public KeysTreeView() : base()
        {
            this.Dock = DockStyle.Fill; 
        }

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            if (e.Node is CategoryNode categoryNode)
            {
                this.dataPanel.Controls.Clear();
                var keysListView = new KeyDataControl();
                this.dataPanel.Controls.Add(keysListView);
                keysListView.Populate(cubaseKeyCommands);
                //var commands = cubaseKeyCommands.GetCommandsByCategory(categoryNode.Category);
                //OnCategorySelected?.Invoke(this, new CategorySelectedEventArgs(categoryNode.Category, commands));
            }
        }

        public void Populate(CubaseKeyCommandCollection cubaseKeyCommands, Panel dataPanel)
        {
            this.dataPanel = dataPanel; 
            this.cubaseKeyCommands = cubaseKeyCommands;
            this.Nodes.Clear();
            this.Nodes.Add(new CategoryTreeViewNode(cubaseKeyCommands));
        }
    }

    public class BaseTreeViewNode : TreeNode
    {
        public BaseTreeViewNode()
        {
        }
    }

    public class CategoryTreeViewNode : BaseTreeViewNode
    {
        public string Category { get; set; }
        public CategoryTreeViewNode(CubaseKeyCommandCollection cubaseKeyCommands) : base()
        {
            this.Text = "Categories";
            //var categories = cubaseKeyCommands.GetCategories();
            //foreach (var category in categories)
            //{
            //    var categoryNode = new CategoryNode(category);
            //    this.Nodes.Add(categoryNode);
            //}
            this.Expand();
        }
    }

    public class CategoryNode : BaseTreeViewNode
    {
        public string Category { get; set; }
        public CategoryNode(string category) : base()
        {
            this.Category = category;
            this.Text = category;
        }
    }
}
