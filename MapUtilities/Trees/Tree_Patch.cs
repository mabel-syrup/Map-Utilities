using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Trees
{
    class Tree_draw_Patch
    {
        public static bool Prefix(SpriteBatch spriteBatch, Vector2 tileLocation, Tree __instance)
        {
            if(TreeHandler.currentTrees != null && TreeHandler.currentTrees.ContainsKey(__instance))
            {
                TreeHandler.currentTrees[__instance].draw(spriteBatch, tileLocation);
                return false;
            }
            return true;
        }
    }
}
