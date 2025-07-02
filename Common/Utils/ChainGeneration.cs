using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Terraria;

namespace GvMod.Common.Utils
{
    public static class ChainGeneration
    {
        public static Vector2 GenerateBorderPosition(Vector2 basePosition, int width = 1200,
            int height = 1200)
        {
            bool horizontal = Main.rand.NextBool();
            bool negative = Main.rand.NextBool();

            Vector2 borderPosition = basePosition;

            if (horizontal)
            {
                if (negative)
                {
                    borderPosition += new Vector2(-width, Main.rand.Next(-height, height));
                }
                else
                {
                    borderPosition += new Vector2(width, Main.rand.Next(-height, height));
                }
            }
            else
            {
                if (negative)
                {
                    borderPosition += new Vector2(Main.rand.Next(-width, width), -height);
                }
                else
                {
                    borderPosition += new Vector2(Main.rand.Next(-width, width), height);
                }
            }

            return borderPosition;
        }

        public static (Vector2, Vector2) GetPositionAndSpeed(Vector2 basePosition, int width = 1200,
            int height = 1200, uint framesToTarget = 10)
        {
            int border = Main.rand.Next(0, 4);
            Vector2 initialPosition = basePosition, endPosition = basePosition;

            switch (border)
            {
                // Top
                case 0:
                    initialPosition += new Vector2(Main.rand.Next(-width, width), -height);
                    endPosition += new Vector2(Main.rand.Next(-width, width), height);
                    break;
                // Right
                case 1:
                    initialPosition += new Vector2(width, Main.rand.Next(-height, height));
                    endPosition += new Vector2(-width, Main.rand.Next(-height, height));
                    break;
                // Bottom
                case 2:
                    initialPosition += new Vector2(Main.rand.Next(-width, width), height);
                    endPosition += new Vector2(Main.rand.Next(-width, width), -height);
                    break;
                // Left
                default:
                case 3:
                    initialPosition += new Vector2(-width, Main.rand.Next(-height, height));
                    endPosition += new Vector2(width, Main.rand.Next(-height, height));
                    break;
            }

            // A precausion, in case framesToTarget is set to 0
            float speed;
            try
            {
                speed = initialPosition.Distance(endPosition) / (framesToTarget + 0.0001f);
            }
            catch (DivideByZeroException)
            {
                speed = initialPosition.Distance(endPosition) / 10;
            }

            return (initialPosition, initialPosition.DirectionTo(endPosition) * speed);
        }
        public static (Vector2, Vector2) GetPositionAndSpeed(Vector2 basePosition, int radius = 1200, 
            uint framesToTarget = 10)
        {
            Vector2 initialPosition = basePosition, endPosition = basePosition;

            Vector2 direction = new Vector2(radius, 0).RotatedByRandom(MathHelper.TwoPi);
            initialPosition += direction;
            endPosition -= direction.RotatedByRandom(MathHelper.PiOver4);

            // A precausion, in case framesToTarget is set to 0
            float speed;
            try
            {
                speed = initialPosition.Distance(endPosition) / (framesToTarget + 0.0001f);
            } catch (DivideByZeroException)
            {
                speed = initialPosition.Distance(endPosition) / 30;
            }

            return (initialPosition, initialPosition.DirectionTo(endPosition) * speed);
        }
    }
}
