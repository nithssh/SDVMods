using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;

namespace Dem1se.CustomReminders
{
    public class ReminderMenu : IClickableMenu
    {
        protected int minLength = 1;
        public const int region_okButton = 101;
        public const int region_doneNamingButton = 102;
        //public const int region_randomButton = 103;
        public const int region_textBox_ReminderMessage = 104;
        public const int region_textBox_DateAndTime = 105;
        public ClickableTextureComponent doneNamingButton;
        protected TextBox textBox_ReminderMessage;
        protected TextBox textBox_DateAndTime;
        public ClickableComponent textBoxCC_ReminderMessage;
        public ClickableComponent textBoxCC_DateAndTime;
        private TextBoxEvent e;
        private ReminderMenu.doneNamingBehavior doneNaming;
        private string title;

        public ReminderMenu(ReminderMenu.doneNamingBehavior b, string title, string defaultName = null)
        {
            this.doneNaming = b;
            this.xPositionOnScreen = 0;
            this.yPositionOnScreen = 0;
            this.width = Game1.viewport.Width;
            this.height = Game1.viewport.Height;
            this.title = title;

            this.textBox_ReminderMessage = new TextBox((Texture2D)null, (Texture2D)null, Game1.smallFont, Game1.textColor);
            this.textBox_ReminderMessage.X = Game1.viewport.Width / 2 - 192;
            this.textBox_ReminderMessage.Y = Game1.viewport.Height / 2;
            this.textBox_ReminderMessage.Width = 384;
            this.textBox_ReminderMessage.Height = 192;

            this.textBox_DateAndTime = new TextBox((Texture2D)null, (Texture2D)null, Game1.dialogueFont, Game1.textColor);
            this.textBox_DateAndTime.X = Game1.viewport.Width / 2 - 192;
            this.textBox_DateAndTime.Y = (Game1.viewport.Height / 2) + 96;
            this.textBox_DateAndTime.Width = 384;
            this.textBox_DateAndTime.Height = 192;

            this.e = new TextBoxEvent(this.textBoxEnter);
            this.textBox_DateAndTime.OnEnterPressed += this.e;
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)this.textBox_ReminderMessage;

            this.textBox_ReminderMessage.Text = "Reminder message";
            this.textBox_ReminderMessage.Selected = true;

            this.textBox_DateAndTime.Text = "12 summer 1400";
            this.textBox_DateAndTime.Selected = false;

            // Random Button
            //ClickableTextureComponent textureComponent1 = new ClickableTextureComponent(new Rectangle(this.textBox_ReminderMessage.X + this.textBox_ReminderMessage.Width + 64 + 48 - 8, Game1.viewport.Height / 2 + 4, 64, 64), Game1.mouseCursors, new Rectangle(381, 361, 10, 10), 4f, false);
            //textureComponent1.myID = 103;
            //textureComponent1.leftNeighborID = 102;

            // Done naming button
            ClickableTextureComponent textureComponent2 = new ClickableTextureComponent(new Rectangle(this.textBox_DateAndTime.X + this.textBox_DateAndTime.Width + 32 + 4, Game1.viewport.Height / 2 - 8, 64, 64), Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46, -1, -1), 1f, false);
            textureComponent2.myID = 102;
            //textureComponent2.rightNeighborID = 103;
            textureComponent2.leftNeighborID = 105;
            this.doneNamingButton = textureComponent2;

            this.textBoxCC_ReminderMessage = new ClickableComponent(new Rectangle(this.textBox_ReminderMessage.X, this.textBox_ReminderMessage.Y, 384, 48), "")
            {
                myID = 104
            };
            this.textBoxCC_DateAndTime = new ClickableComponent(new Rectangle(this.textBox_DateAndTime.X, this.textBox_DateAndTime.Y, 384, 48), "")
            {
                myID = 105,
                rightNeighborID = 102
            };

            if (!Game1.options.SnappyMenus)
                return;
            this.populateClickableComponentList();
            this.snapToDefaultClickableComponent();
        }

        public override void snapToDefaultClickableComponent()
        {
            this.currentlySnappedComponent = this.getComponentWithID(104);
            this.snapCursorToCurrentSnappedComponent();
        }

        public void textBoxEnter(TextBox sender)
        {
            if (sender.Text.Length < this.minLength)
                return;
            if (this.doneNaming != null)
            {
                if (sender == textBox_ReminderMessage)
                {
                    // TODO:switch to second field
                    this.textBox_ReminderMessage.Selected = false;
                    this.textBox_DateAndTime.Selected = true;

                }
                else if (sender == textBox_DateAndTime)
                {
                    this.doneNaming(textBox_ReminderMessage.Text, textBox_DateAndTime.Text);
                }
            }
            else
                Game1.exitActiveMenu();
        }

        public override void receiveGamePadButton(Buttons b)
        {
            base.receiveGamePadButton(b);
            if (!this.textBox_ReminderMessage.Selected)
                return;
            switch (b)
            {
                case Buttons.DPadUp:
                case Buttons.DPadDown:
                case Buttons.DPadLeft:
                case Buttons.DPadRight:
                case Buttons.LeftThumbstickLeft:
                case Buttons.LeftThumbstickUp:
                case Buttons.LeftThumbstickDown:
                case Buttons.LeftThumbstickRight:
                    this.textBox_ReminderMessage.Selected = false;
                    break;
            }
        }

        public override void receiveKeyPress(Keys key)
        {
            if (this.textBox_ReminderMessage.Selected || Game1.options.doesInputListContain(Game1.options.menuButton, key))
                return;
            base.receiveKeyPress(key);
        }

        public override void receiveRightClick(int x, int y, bool playSound = true)
        {
        }

        public override void performHoverAction(int x, int y)
        {
            base.performHoverAction(x, y);
            if (this.doneNamingButton != null)
            {
                if (this.doneNamingButton.containsPoint(x, y))
                    this.doneNamingButton.scale = Math.Min(1.1f, this.doneNamingButton.scale + 0.05f);
                else
                    this.doneNamingButton.scale = Math.Max(1f, this.doneNamingButton.scale - 0.05f);
            }
        }

        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            base.receiveLeftClick(x, y, playSound);
            this.textBox_ReminderMessage.Update();
            this.textBox_DateAndTime.Update();
            if (this.doneNamingButton.containsPoint(x, y))
            {
                this.textBoxEnter(this.textBox_DateAndTime);
                Game1.playSound("smallSelect");
            }
            this.textBox_ReminderMessage.Update();
            this.textBox_DateAndTime.Update();
        }

        public override void draw(SpriteBatch b)
        {
            base.draw(b);
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);
            SpriteText.drawStringWithScrollCenteredAt(b, this.title, Game1.viewport.Width / 2, Game1.viewport.Height / 2 - 128, this.title, 1f, -1, 0, 0.88f, false);
            this.textBox_ReminderMessage.Draw(b, true);
            this.textBox_DateAndTime.Draw(b, true);
            this.doneNamingButton.draw(b);
            this.drawMouse(b);
        }

        public delegate void doneNamingBehavior(string RemMsg, string DateTime);
    }
}
