using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MapUtilities.Trees
{
    public class Branch : TreePart
    {
        public Branch(TreeRenderer renderer)
        {
            children = new List<TreePart>();
            spriteSheet = TreeHandler.treeImage;
            this.renderer = renderer;
            sprite = new Microsoft.Xna.Framework.Rectangle(22, 8, 3, 8);
            rotation = 0f;
            depth = 0;
        }
    }
}
