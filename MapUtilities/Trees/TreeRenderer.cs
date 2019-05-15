using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValley;
using StardewValley.TerrainFeatures;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
//using System.Drawing;
//using System.Drawing.Imaging;

namespace MapUtilities.Trees
{
    public class TreeRenderer
    {
        public Tree tree;
        public TreePart treeStructure;
        public IReflectedField<float> shakeRotation;
        public IReflectedField<Netcode.NetBool> falling;

        public int seed;

        public Random random;

        public Microsoft.Xna.Framework.Color transparency;

        //Some key words here:
        //Trunk sections, as a unit, refer to the sprite dimensions given to a trunk section for this tree type.  These can and will differ from tree to tree.
        //Stump refers to the base of the tree
        //Trunk refers to the main column of the tree
        //Limb refers to the large scaffolds from which branches grow
        //Branch refers to the scaffolds that hold leaves away from the trunk to gather sunlight optimally.

        //Minimum height for a mature tree of this type, measured in trunk sections.
        public int minHeight = 5;
        //Maximum variation in mature height for the tree, measured in trunk sections.
        public int heightVariation = 10;
        //The base chance any given trunk will have a limb
        public float limbFrequency = 0.3f;
        //The initial angle a limb will grow at, relative to the trunk section it is on
        public float limbAngle = 0.4f;
        //Minimum trunk sections before a limb will grow.  
        public int minLimbHeight = 2;
        //Variation on the minimum limb height, varied only positively.
        public float minLimbHeightVariation = 1.2f;
        //Average trunk sections between limbs, vertically.
        public float averageLimbInterval = 0.5f;
        //Variation on the trunk sections between limbs, varied both positively and negatively.
        public float limbIntervalVariation = 1f;
        //When limbs alternate, they will only protrude from one side of the trunk OR the other.  Otherwise, they will do both.
        public bool limbAlternate = true;
        //Average number of sections a limb should extend compared to how much tree is above it
        public float limbGrowth = 1f;
        //How randomly the branches extend from limbs.  0 results in branches entirely horizontally extending from the limb, 1 results in absolutely any direction
        public float branchWhorl = 0.2f;
        //Branch length multiplier.  How long a branch should be as compared to the remaining length of the limb past it.
        public float averageBranchLength = 0.3f;
        //How much the length of each branch should vary by.
        public float branchLengthVariation = 1.2f;
        //How many limb sections will have branches
        public float branchFrequency = 0.7f;
        //Minimum limb sections before branches will grow
        public int minBranchDistance = 3;

        public TreeRenderer(Tree tree, Vector2 tile)
        {
            transparency = Microsoft.Xna.Framework.Color.White;
            this.tree = tree;
            updateStructure(tile);
        }

        public void draw(SpriteBatch b, Vector2 treePos)
        {
            if(treePos.Y > (Game1.viewport.Y + Game1.viewport.Height) / 64)
            {
                float distance = Math.Min(treePos.Y * 64 - (Game1.viewport.Y + Game1.viewport.Height), 6f * 64f);
                transparency = (Microsoft.Xna.Framework.Color.White * (1 - distance / (6f * 64f)));
            }
            else
            {
                transparency = Microsoft.Xna.Framework.Color.White;
            }

            treeStructure.draw(b, treePos, treeStructure.sprite.Width * 2, treeStructure.sprite.Height * 4, 0f);
        }

        public void updateStructure(Vector2 tile)
        {
            //seed should remain constant for any tree, so it is tied to their location and whether they are flipped.
            seed = (int)(tile.X * 100 * tile.Y * Math.PI) * (tree.flipped ? 2 : 3);
            random = new Random(seed);
            Logger.log("Seed is " + seed);
            shakeRotation = Reflector.reflector.GetField<float>(tree, "shakeRotation");
            falling = Reflector.reflector.GetField<Netcode.NetBool>(tree, "falling");
            buildStructure();
            //treeStructure.draw(Game1.spriteBatch, tile, 0f, 0f);
        }

        public void buildStructure()
        {
            treeStructure = new Stump(this);
            int trunkLength = minHeight + (seed % heightVariation);
            Logger.log("Tree is " + trunkLength + " units tall.");

            int firstLimbHeight = (int)(minLimbHeight + random.NextDouble() * minLimbHeightVariation);
            //int sectionsBetweenLimbs = 0;

            for(int i = 0; i < trunkLength; i++)
            {
                Trunk newSection = new Trunk(this);
                newSection.rotation = ((float)random.NextDouble() * 1f) - 0.5f;
                //newSection.rotation = 0.3f;
                treeStructure.findAnyEnd(new Type[] { typeof(Trunk), typeof(Stump) }).addPart(newSection);
                newSection.performSetup();
                //&& i - firstLimbHeight >= sectionsBetweenLimbs
                if (i >= firstLimbHeight && random.NextDouble() <= limbFrequency)
                {
                    Logger.log("Adding limb to section " + i);
                    if (limbAlternate)
                    {
                        addLimbToPart(newSection, random.Next(2) == 0, i, trunkLength - i);
                    }
                    else
                    {
                        addLimbToPart(newSection, true, i, trunkLength - i);
                        addLimbToPart(newSection, false, i, trunkLength - i);
                    }
                    //sectionsBetweenLimbs = (int)(averageLimbInterval + (random.NextDouble() * limbIntervalVariation) - (limbIntervalVariation / 2));
                }
                //else if (i >= firstLimbHeight)
                //{
                //    sectionsBetweenLimbs--;
                //}
            }
            addLimbToPart(treeStructure.findAnyEnd(new Type[] { typeof(Trunk), typeof(Stump) }), random.Next(2) == 0, trunkLength, minHeight / 2);
        }

        public void addLimbToPart(TreePart part, bool left, int sectionsUp, int sectionsDown)
        {
            Logger.log("Limb was on the " + (left ? "left" : "right"));
            Limb newLimb = new Limb(this);
            float initialRot = limbAngle * (left ? -1 : 1);

            float depth = (float)(random.NextDouble() * 2 + 1) * (random.Next(2) == 0 ? -1 : 1);
            newLimb.depth = depth;
            newLimb.rotation = initialRot;
            part.addPart(newLimb);
            newLimb.left = left;
            newLimb.performSetup();
            int sectionsToGrow = (int)(sectionsDown * limbGrowth);
            Logger.log("Growing " + sectionsToGrow + " sections on limb...");
            for(int i = 0; i < sectionsToGrow; i++)
            {
                Limb newSection = new Limb(this);
                newLimb.findAnyEnd(new Type[] { typeof(Limb) }).addPart(newSection);
                newSection.rotation = ((float)(random.NextDouble() * 0.5 - 0.25f));
                newSection.performSetup();
                if (i >= minBranchDistance && random.NextDouble() < branchFrequency)
                {
                    Logger.log("Adding branch to limb section " + i);
                    if (limbAlternate)
                    {
                        addBranchesToLimb(newSection, sectionsToGrow - i);
                    }
                    else
                    {
                        addBranchesToLimb(newSection, sectionsToGrow - i);
                        addBranchesToLimb(newSection, sectionsToGrow - i);
                    }
                    //sectionsBetweenLimbs = (int)(averageLimbInterval + (random.NextDouble() * limbIntervalVariation) - (limbIntervalVariation / 2));
                }
            }
            addBranchesToLimb(newLimb.findAnyEnd(new Type[] { typeof(Limb) }), sectionsToGrow / 2);
        }

        public void addBranchesToLimb(TreePart limb, int limbRemaining)
        {
            int branchSections = (int)(limbRemaining * averageBranchLength + (random.NextDouble() * branchLengthVariation - branchLengthVariation / 2));
            Branch newBranch = new Branch(this);
            limb.addPart(newBranch);
            float initialRot = (float)(random.NextDouble() * (Math.PI * 2f));
            newBranch.rotation = initialRot;
            newBranch.performSetup();
            for(int i = 0; i < branchSections; i++)
            {
                Branch newSection = new Branch(this);
                newBranch.findAnyEnd().addPart(newSection);
                newSection.rotation = ((float)(random.NextDouble() * 0.5 - 0.25f));
                newSection.performSetup();
            }

            LeafCluster leafCluster = new LeafCluster(this);
            newBranch.findAnyEnd().addPart(leafCluster);
            leafCluster.depth -= (float)(random.NextDouble() + 0.5);
            leafCluster.performSetup();
            Logger.log("Leaf cluster is on " + (leafCluster.left ? "left" : "right"));
        }
    }
}
