using System;
using Mooyash.Modules;
using Mooyash.Services;

namespace Mooyash.Modules
{
	public class ItemBox : GameObject // Equivalent of mario kart mystery box
	{
		public static string[] validItems = new string[] { "banana", "green_shell", "mushroom", "bread"};

		public ItemBox(Vector2 position) : base()
		{
			this.position = position;
			texture = Engine.LoadTexture("mystery_box.png");
			size = new Vector2(62.5f, 62.5f);
			resolution = new Vector2(32, 32);

			radius = 50f;
		}

		public override void collide(Kart k)
		{
			if (k.itemHeld == 0)
			{
				k.rollItemTime = 0;
				k.itemHeld = -1;
				if(!k.isAI)
                {
					Engine.PlaySound(Sounds.sounds["itemBox"]);
				}

				PhysicsEngine.gameObjects.Remove(this);
            }
		}
	}
}

