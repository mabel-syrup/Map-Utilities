using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MapUtilities.Trees
{
    public class Limb : TreePart
    {
        public Limb(TreeRenderer renderer)
        {
            children = new List<TreePart>();
            spriteSheet = TreeHandler.treeImage;
            this.renderer = renderer;
            sprite = new Microsoft.Xna.Framework.Rectangle(22, 24, 4, 8);
            rotation = 0f;
            depth = 0;
        }


    }
}
