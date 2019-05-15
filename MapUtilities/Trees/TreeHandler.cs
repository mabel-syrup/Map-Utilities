using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewModdingAPI;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MapUtilities.Trees
{
    public static class TreeHandler
    {
        public static Dictionary<Tree, TreeRenderer> currentTrees;

        public static Texture2D treeImage;

        public static void init()
        {
            currentTrees = new Dictionary<Tree, TreeRenderer>();
            treeImage = Loader.loader.Load<Texture2D>("Content/Trees/Test.png", ContentSource.ModFolder);
        }

        public static void createAllTrees(GameLocation location)
        {
            currentTrees.Clear();

            if(location == null)
            {
                return;
            }

            foreach(Vector2 position in location.terrainFeatures.Keys)
            {
                if(location.terrainFeatures[position] is Tree)
                {
                    Tree tree = (Tree)(location.terrainFeatures[position]);
                    if(tree.growthStage > 3)
                        currentTrees[tree] = new TreeRenderer(tree, position);
                }
            }
        }
    }
}
