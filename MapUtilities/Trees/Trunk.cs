using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;

namespace MapUtilities.Trees
{
    public class Trunk : TreePart
    {

        public Trunk(TreeRenderer renderer)
        {
            children = new List<TreePart>();
            spriteSheet = TreeHandler.treeImage;
            this.renderer = renderer;
            sprite = new Microsoft.Xna.Framework.Rectangle(0, 0, 16, 16);
            rotation = 0f;
            depth = 0;
        }

        public override void performSetup()
        {
            seekSun(0.4f, 0.2f);
        }

        public void seekSun(float seeking, float seekingVariation)
        {
            float currentRot = renderer.treeStructure.findTotalRotationOfChild(this);
            //float seekingAmount = seeking + (seed % seekingVariation - (seekingVariation / 2));
            float seekingAmount = (((float)renderer.random.NextDouble() * seekingVariation) - (seekingVariation / 2)) + seeking;
            float newRot = ((currentRot * (1 - seekingAmount)) - currentRot) + rotation;
            //Logger.log("Rotation was " + part.rotation + " local, " + currentRot + " global, seeking the sun with " + seekingAmount + " force.  New rotation is " + newRot + " to reach the goal of " + (currentRot * seekingAmount) + ".");
            rotation = newRot;
        }
    }
}
