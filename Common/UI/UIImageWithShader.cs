using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.Graphics.Shaders;
using Terraria.UI;

namespace GvMod.Common.UI
{
    class UIImageWithShader : UIElement
    {
        private Asset<Texture2D> _texture;
        private MiscShaderData _shader;
        public float ImageScale = 1f;
        public float Rotation;
        public bool ScaleToFit;
        public bool AllowResizingDimensions = true;
        public Color Color = Color.White;
        public Vector2 NormalizedOrigin = Vector2.Zero;
        public bool RemoveFloatingPointsFromDrawPosition;
        private Texture2D _nonReloadingTexture;

        public UIImageWithShader(Asset<Texture2D> texture, MiscShaderData shader = null)
        {
            SetImage(texture);
            SetShader(shader);
        }

        public UIImageWithShader(Texture2D nonReloadingTexture, MiscShaderData shader = null)
        {
            SetImage(nonReloadingTexture);
            SetShader(shader);
        }

        public void SetImage(Asset<Texture2D> texture)
        {
            _texture = texture;
            _nonReloadingTexture = null;
            if (AllowResizingDimensions)
            {
                Width.Set(_texture.Width(), 0f);
                Height.Set(_texture.Height(), 0f);
            }
        }

        public void SetImage(Texture2D nonReloadingTexture)
        {
            _texture = null;
            _nonReloadingTexture = nonReloadingTexture;
            if (AllowResizingDimensions)
            {
                Width.Set(_nonReloadingTexture.Width, 0f);
                Height.Set(_nonReloadingTexture.Height, 0f);
            }
        }

        public void SetShader(MiscShaderData shader)
        {
            _shader = null;
            _shader = shader;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            CalculatedStyle dimensions = GetDimensions();
            Texture2D texture2D = null;
            SpriteBatchState prevState = SpriteBatchExt.GetState(spriteBatch);

            if (_shader != null)
            {
                SpriteBatchExt.Restart(spriteBatch, prevState, SpriteSortMode.Immediate);
                _shader.Apply();
            }

            if (_texture != null)
                texture2D = _texture.Value;

            if (_nonReloadingTexture != null)
                texture2D = _nonReloadingTexture;

            if (ScaleToFit)
            {
                spriteBatch.Draw(texture2D, dimensions.ToRectangle(), Color);
                if (_shader != null)
                {
                    SpriteBatchExt.Restart(spriteBatch, prevState);
                }
                return;
            }

            Vector2 vector = texture2D.Size();
            Vector2 vector2 = dimensions.Position() + vector * (1f - ImageScale) / 2f + vector * NormalizedOrigin;
            if (RemoveFloatingPointsFromDrawPosition)
                vector2 = vector2.Floor();

            spriteBatch.Draw(texture2D, vector2, null, Color, Rotation, vector * NormalizedOrigin, ImageScale, SpriteEffects.None, 0f);

            if (_shader != null)
            {
                SpriteBatchExt.Restart(spriteBatch, prevState);
            }
        }
    }
}
