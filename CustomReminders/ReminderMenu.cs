using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewModdingAPI;
using StardewValley;
using StardewValley.BellsAndWhistles;
using StardewValley.Menus;
using System;
using System.Collections.Generic;
using System.IO;

namespace Dem1se.CustomReminders.UI
{
    /// <summary>The menu which lets the set new reminders.</summary>
    /// Contains parts of code from https://github.com/janavarro95/Stardew_Valley_Mods/blob/master/GeneralMods/HappyBirthday/Framework/BirthdayMenu.cs
    /// Thanks to janavarro95.
    internal class ReminderMenuPage1 : IClickableMenu
    {
        private ClickableTextureComponent DisplayRemindersButton;

        /// <summary>The labels to draw.</summary>
        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();

        /// <summary>The season buttons to draw.</summary>
        private readonly List<ClickableTextureComponent> SeasonButtons = new List<ClickableTextureComponent>();

        /// <summary>The day buttons to draw.</summary>
        private readonly List<ClickableTextureComponent> DayButtons = new List<ClickableTextureComponent>();

        /// <summary> The Textboxes to draw </summary>
        private TextBox ReminderTextBox;

        /// <summary>The OK button to draw.</summary>
        private ClickableTextureComponent OkButton;

        ///<summary> The Reminder message to display.</summary>
        private string ReminderMessage;

        /// <summary>The season the reminder is set to.</summary>
        private string ReminderSeason;

        /// <summary>The Date the reminder is set to.</summary>
        private int ReminderDate;

        /// <summary>The callback to invoke when the ok button is pressed</summary>
        private readonly Action<string, string, int> OnChanged;

        private readonly IModHelper Helper;
        private readonly SButton MenuButton;

        /// <summary>Construct an instance.</summary>
        /// <param name="season">The initial birthday season.</param>
        /// <param name="day">The initial birthday day.</param>
        /// <param name="onChanged">The callback to invoke when the birthday value changes.</param>
        public ReminderMenuPage1(Action<string, string, int> onChanged, IModHelper Helper)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            this.OnChanged = onChanged;
            this.Helper = Helper;
            this.MenuButton = Utilities.Utilities.GetMenuButton();

            this.SetUpPositions();
        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            this.xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            this.yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
            this.SetUpPositions();
        }


        /// <summary>Regenerate the UI.</summary>
        private void SetUpPositions()
        {
            this.Labels.Clear();
            this.DayButtons.Clear();
            
            this.OkButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            this.DisplayRemindersButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - Game1.tileSize * 5 - IClickableMenu.spaceToClearSideBorder * 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder, Game1.tileSize * 5 + Game1.tileSize / 4 + Game1.tileSize / 8, Game1.tileSize + Game1.tileSize / 8), Helper.Content.Load<Texture2D>("assets/DisplayReminders.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
            this.Labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8, 1, 1), "Reminder Message: "));

            this.ReminderTextBox = new TextBox(null, null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1,
                Y = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - Game1.tileSize / 4 + Game1.tileSize / 2 + Game1.tileSize,
                Width = this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3 - Game1.tileSize / 4,
                Height = 180
            };
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)this.ReminderTextBox;

            this.SeasonButtons.Add(new ClickableTextureComponent("Spring", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.10) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(188, 438, 32, 9), Game1.pixelZoom));
            this.SeasonButtons.Add(new ClickableTextureComponent("Summer", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.10) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(220, 438, 32, 8), Game1.pixelZoom));
            this.SeasonButtons.Add(new ClickableTextureComponent("Fall", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 5 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.1) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(188, 447, 32, 10), Game1.pixelZoom));
            this.SeasonButtons.Add(new ClickableTextureComponent("Winter", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 7 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.1) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(220, 448, 32, 8), Game1.pixelZoom));


            this.DayButtons.Add(new ClickableTextureComponent("1", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("2", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("3", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(24, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("4", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(32, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("5", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 5 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(40, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("6", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 6 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(48, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("7", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 7 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(56, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("8", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(64, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("9", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2 - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(72, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("10", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("10", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(0, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("11", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("11", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("12", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("12", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("13", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("13", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(24, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("14", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("14", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 7.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(32, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("15", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 0.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("15", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 1.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(40, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("16", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 1.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("16", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(48, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("17", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("17", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(56, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("18", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("18", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(64, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("19", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("19", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(72, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("20", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("20", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(0, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("21", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("21", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 7.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("22", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 0.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("22", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 1.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("23", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 1.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("23", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(24, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("24", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("24", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(32, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("25", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("25", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(40, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("26", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("26", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(48, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("27", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("27", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(56, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("28", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.75) - Game1.tileSize / 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            this.DayButtons.Add(new ClickableTextureComponent("28", new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 7.25) - Game1.tileSize / 3, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(64, 16, 8, 12), Game1.pixelZoom));
        }

        /// <summary>Handle a button click.</summary>
        /// <param name="name">The button name that was clicked.</param>
        private void HandleButtonClick(string name)
        {
            if (name == null)
                return;

            switch (name)
            {
                // season button
                case "Spring":
                case "Summer":
                case "Fall":
                case "Winter":
                    this.ReminderSeason = name.ToLower();
                    break;

                // OK button
                case "OK":
                    if ((this.ReminderDate >= 1 || this.ReminderDate <= 28) && this.IsOkButtonReady())
                    {
                        this.ReminderMessage = ReminderTextBox.Text;
                        this.OnChanged(this.ReminderMessage, this.ReminderSeason, this.ReminderDate);
                    }

                    break;

                default:
                    this.ReminderDate = Convert.ToInt32(name);
                    break;
            }
            Game1.playSound("coin");
        }

        /// <summary>
        /// Checks if the page1 inputs are all valid
        /// </summary>
        /// <returns>True if ok button is ready False if not</returns>
        private bool IsOkButtonReady()
        {
            if (this.ReminderDate != 0 && this.ReminderSeason != "" && this.ReminderTextBox.Text != null && this.ReminderTextBox.Text != "")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>The method invoked when the player left-clicks on the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        /// <param name="playSound">Whether to enable sound.</param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {

            this.ReminderTextBox.Update();

            //If the season is not selected then the day buttons can't be clicked. Thanks to @Potato#5266 on the SDV discord for this tip.
            if (this.ReminderSeason == "spring" || this.ReminderSeason == "summer" || this.ReminderSeason == "fall" || this.ReminderSeason == "winter")
            {
                foreach (ClickableTextureComponent button in this.DayButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        this.HandleButtonClick(button.name);
                        button.scale -= 0.5f;
                        button.scale = Math.Max(3.5f, button.scale);
                    }
                }
            }

            foreach (ClickableTextureComponent button in this.SeasonButtons)
            {
                if (button.containsPoint(x, y))
                {
                    this.HandleButtonClick(button.name);
                    button.scale -= 0.5f;
                    button.scale = Math.Max(3.5f, button.scale);
                }
            }

            if (this.OkButton.containsPoint(x, y) && this.IsOkButtonReady())
            {
                this.HandleButtonClick(this.OkButton.name);
                this.OkButton.scale -= 0.25f;
                this.OkButton.scale = Math.Max(0.75f, this.OkButton.scale);
            }

            if (DisplayRemindersButton.containsPoint(x, y))
                Game1.activeClickableMenu = new DisplayReminders(Helper, this.OnChanged);

            this.ReminderTextBox.Update();
        }


        /// <summary>The method invoked when the player hovers the cursor over the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void performHoverAction(int x, int y)
        {
            foreach (ClickableTextureComponent button in this.DayButtons)
            {
                button.scale = button.containsPoint(x, y)
                    ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                    : Math.Max(button.scale - 0.02f, button.baseScale);
            }

            foreach (ClickableTextureComponent button in this.SeasonButtons)
            {
                button.scale = button.containsPoint(x, y)
                    ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                    : Math.Max(button.scale - 0.02f, button.baseScale);
            }

            this.OkButton.scale = this.OkButton.containsPoint(x, y) && this.IsOkButtonReady()
                ? Math.Min(this.OkButton.scale + 0.02f, this.OkButton.baseScale + 0.1f)
                : Math.Max(this.OkButton.scale - 0.02f, this.OkButton.baseScale);
            
            this.DisplayRemindersButton.scale = this.DisplayRemindersButton.containsPoint(x, y)
                ? Math.Min(this.DisplayRemindersButton.scale + 0.02f, this.DisplayRemindersButton.baseScale + 0.1f)
                : Math.Max(this.DisplayRemindersButton.scale - 0.02f, this.DisplayRemindersButton.baseScale);
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="b">The sprite batch.</param>
        public override void draw(SpriteBatch b)
        {
            Helper.Input.Suppress(MenuButton);

            //draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            // draw title scroll
            SpriteText.drawStringWithScrollCenteredAt(b, "New Reminder", Game1.viewport.Width / 2, this.yPositionOnScreen, "New Reminder");

            // draw textbox
            this.ReminderTextBox.Draw(b, false);

            // draw day buttons
            foreach (ClickableTextureComponent button in this.DayButtons)
                button.draw(b);

            // draw season buttons
            foreach (ClickableTextureComponent button in this.SeasonButtons)
                button.draw(b);

            // draw labels
            foreach (ClickableComponent label in this.Labels)
            {
                Color color = Color.Violet;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
            }
            foreach (ClickableComponent label in this.Labels)
            {
                string text = "";
                Color color = Game1.textColor;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
                if (text.Length > 0)
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((label.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (label.bounds.Y + Game1.tileSize / 2)), color);
            }

            // draw OK button
            if (this.ReminderDate != 0 && this.ReminderSeason != "" && this.IsOkButtonReady())
                this.OkButton.draw(b);
            else
            {
                this.OkButton.draw(b);
                this.OkButton.draw(b, Color.Black * 0.5f, 0.97f);
            }

            // draw displayreminder button
            DisplayRemindersButton.draw(b);

            // draw cursor
            this.drawMouse(b);

        }
    }

    /// <summary>
    /// Second page of the menu that sets the reminder's time
    /// </summary>
    internal class ReminderMenuPage2 : IClickableMenu
    {
        /// <summary>The lables to draw</summary>
        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();
        /// <summary>The Ok button to draw</summary>
        private ClickableTextureComponent OkButton;
        /// <summary>The TextBox to enter the time</summary>
        private TextBox TimeTextBox;
        /// <summary>Field that contains the Time inputed</summary>
        internal int ReminderTime = 0;
        /// <summary>The callback to invoke when the birthday value changes.</summary>
        private readonly Action<int> OnChanged;
        private readonly IModHelper Helper;
        private readonly SButton MenuButton;

        /// <summary>The callback function that gets called when ok buttong is pressed</summary>
        public ReminderMenuPage2(Action<int> OnChanged, IModHelper Helper)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            this.Helper = Helper;
            this.OnChanged = OnChanged;
            this.MenuButton = Utilities.Utilities.GetMenuButton();
            this.SetupPositions();
        }

        /// <summary>Generates the UI</summary>
        private void SetupPositions()
        {
            this.Labels.Clear();
            this.OkButton = new ClickableTextureComponent("OK", new Rectangle(this.xPositionOnScreen + this.width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, this.yPositionOnScreen + this.height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            this.Labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8, 1, 1), "Reminder Time: "));
            this.Labels.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 3, 1, 1), $"Set the time of the reminder in 24hrs \nformat in multiples of 30 in-game minutes.\n\ne.g. 1400 (=2PM)\n     0730 (=7AM)"));
            this.TimeTextBox = new TextBox(null, null, Game1.smallFont, Game1.textColor)
            {
                X = this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize,
                Y = this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - Game1.tileSize / 4 + Game1.tileSize + Game1.tileSize / 2,
                Width = 384 + Game1.tileSize,
                Height = 180,
                numbersOnly = true,
                textLimit = 4
            };
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)this.TimeTextBox;
        }

        /// <summary>Handles the left clicks on the menu</summary>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            this.TimeTextBox.Update();
            if (this.OkButton.containsPoint(x, y))
            {
                if (this.TimeTextBox.Text == "" || this.TimeTextBox.Text == null) return;
                this.ReminderTime = Convert.ToInt32(this.TimeTextBox.Text);

                if (this.IsOkButtonReady())
                {
                    Game1.playSound("coin");
                    this.OkButton.scale -= 0.25f;
                    this.OnChanged(ReminderTime);
                    this.OkButton.scale = Math.Max(0.75f, this.OkButton.scale);
                    Game1.exitActiveMenu();
                }
            }
            this.TimeTextBox.Update();
        }

        /// <summary>Defines what to do when hovering over UI elements</summary>
        public override void performHoverAction(int x, int y)
        {
            this.OkButton.scale = this.OkButton.containsPoint(x, y) && this.IsOkButtonReady()
                ? Math.Min(this.OkButton.scale + 0.02f, this.OkButton.baseScale + 0.1f)
                : Math.Max(this.OkButton.scale - 0.02f, this.OkButton.baseScale);
        }

        /// <summary>
        /// Checks if the page1 inputs are all valid
        /// </summary>
        /// <returns>True if ok button is ready, False if not</returns>
        private bool IsOkButtonReady()
        {
            if (this.TimeTextBox.Text.Length == 4 && this.TimeTextBox.Text != null && this.TimeTextBox.Text != "" && (this.TimeTextBox.Text.ToString().EndsWith("30") || this.TimeTextBox.Text.ToString().EndsWith("00")) && (Convert.ToInt32(this.TimeTextBox.Text) >= 0600 && Convert.ToInt32(this.TimeTextBox.Text) <= 2600))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> Draws the UI</summary>
        public override void draw(SpriteBatch b)
        {
            // supress the Menu button
            Helper.Input.Suppress(MenuButton);

            // draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height, false, true);

            // draw textbox
            this.TimeTextBox.Draw(b, false);

            // draw labels
            foreach (ClickableComponent label in this.Labels)
            {
                Color color = Color.Violet;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
            }
            foreach (ClickableComponent label in this.Labels)
            {
                string text = "";
                Color color = Game1.textColor;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
                if (text.Length > 0)
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((label.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (label.bounds.Y + Game1.tileSize / 2)), color);
            }

            // draw OK button
            if (this.IsOkButtonReady())
                this.OkButton.draw(b);
            else
            {
                this.OkButton.draw(b);
                this.OkButton.draw(b, Color.Black * 0.5f, 0.97f);
            }

            // draw cursor
            this.drawMouse(b);
        }
    }

    /// <summary>
    /// UI to display the currently set reminders
    /// </summary>
    public class DisplayReminders : IClickableMenu
    {
        private List<ClickableTextureComponent> DeleteButtons = new List<ClickableTextureComponent>();
        private List<ClickableComponent> ReminderMessages = new List<ClickableComponent>();
        private List<ClickableTextureComponent> Boxes = new List<ClickableTextureComponent>();
        private List<ReminderModel> Reminders = new List<ReminderModel>();

        private readonly ClickableTextureComponent NextPageButton;
        private readonly ClickableTextureComponent PrevPageButton;
        private readonly ClickableTextureComponent NewReminderButton;
        private readonly ClickableComponent NoRemindersWarning;

        ///<summary>This is required for switching to New Reminders menu (for its constructor requires this call back function)</summary>
        private readonly Action<string, string, int> Page1OnChangeBehaviour;

        private readonly IModHelper Helper;
        private readonly SButton MenuButton;
        private ICursorPosition CursorPosition;
        private int PageIndex = 0;

        /// <summary>
        /// Construct an instance
        /// </summary>
        /// <param name="helper">The Helper that provides easy access to some useful methods and fields</param>
        /// <param name="Page1OnChangeBehaviour">Required to switch to the New Reminder menu (as its constructor requires this callback function)</param>
        public DisplayReminders(IModHelper helper, Action<string, string, int> Page1OnChangeBehaviour)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            this.Helper = helper;
            this.MenuButton = Utilities.Utilities.GetMenuButton();
            this.Page1OnChangeBehaviour = Page1OnChangeBehaviour;

            SetUpUI();

            this.NextPageButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + this.width + Game1.tileSize - Game1.tileSize / 2, this.yPositionOnScreen + this.height - Game1.tileSize, Game1.tileSize, Game1.tileSize), Helper.Content.Load<Texture2D>("assets/rightArrow.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
            this.PrevPageButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - Game1.tileSize, this.yPositionOnScreen + this.height - Game1.tileSize, Game1.tileSize, Game1.tileSize), Helper.Content.Load<Texture2D>("assets/leftArrow.png", ContentSource.ModFolder), new Rectangle(), 1.5f);

            this.NoRemindersWarning = new ClickableComponent(new Rectangle(this.xPositionOnScreen + this.width / 2 - this.width / 4 + Game1.tileSize / 2, this.yPositionOnScreen + this.height / 2, this.width, Game1.tileSize), "No reminders are set yet");
            this.NewReminderButton = new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - Game1.tileSize * 5 - IClickableMenu.spaceToClearSideBorder * 2, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder, Game1.tileSize * 5 + Game1.tileSize / 4 + Game1.tileSize / 8, Game1.tileSize + Game1.tileSize / 8), Helper.Content.Load<Texture2D>("assets/NewReminder.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
        }

        public void SetUpUI()
        {
            SetUpReminderMessages();
            SetUpBoxes();
            SetUpDeleteButtons();
        }

        /// <summary>Regenerates the reminder messages (for page switches and initializations)</summary>
        private void SetUpReminderMessages()
        {
            Reminders.Clear();
            PopulateRemindersList();
            this.ReminderMessages.Clear();
            // Setup the boxes for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                    this.ReminderMessages.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize - Game1.tileSize / 16, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, 1, 1), Reminders[i + (PageIndex * 5)].ReminderMessage));
            }
            // Setup the boxes for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                    this.ReminderMessages.Add(new ClickableComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize - Game1.tileSize / 16, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, 1, 1), Reminders[i + (PageIndex * 5)].ReminderMessage));
            }
        }

        /// <summary>Regenerates the boxes (for page switches nad initializations)</summary>
        private void SetUpBoxes()
        {
            this.Boxes.Clear();
            // Setup the boxes for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    this.Boxes.Add(new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 16, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * (1 + (i * 2)) - Game1.tileSize - (8 * i), this.width - IClickableMenu.spaceToClearSideBorder * 2 - 32, Game1.tileSize * 2 - 16), Helper.Content.Load<Texture2D>("assets/reminderBox.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
                    this.Boxes[i].hoverText = Utilities.Utilities.ConvertToPrettyTime(Reminders[i + (PageIndex * 5)].Time) + ", " + Utilities.Utilities.ConvertToPrettyDate(Reminders[i + (PageIndex * 5)].DaysSinceStart);
                }
            }
            // Setup the boxes for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                {
                    this.Boxes.Add(new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 16, this.yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * (1 + (i * 2)) - Game1.tileSize - (8 * i), this.width - IClickableMenu.spaceToClearSideBorder * 2 - 16, Game1.tileSize * 2 - 16), Helper.Content.Load<Texture2D>("assets/reminderBox.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
                    this.Boxes[i].hoverText = Utilities.Utilities.ConvertToPrettyTime(Reminders[i + (PageIndex * 5)].Time) + ", " + Utilities.Utilities.ConvertToPrettyDate(Reminders[i + (PageIndex * 5)].DaysSinceStart);
                }
            }
        }

        /// <summary>Regenerates the reminder messages (for page switches and initializations)</summary>
        private void SetUpDeleteButtons()
        {
            this.DeleteButtons.Clear();
            // Setup the delete buttons for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                    this.DeleteButtons.Add(new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth + this.width - Game1.tileSize * 1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Helper.Content.Load<Texture2D>("assets/deleteButton.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
            }
            // Setup the delete buttons for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                    this.DeleteButtons.Add(new ClickableTextureComponent(new Rectangle(this.xPositionOnScreen - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth + this.width - Game1.tileSize * 1, this.yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Helper.Content.Load<Texture2D>("assets/deleteButton.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
            }
        }

        /// <summary>This fills the Reminders list by reading all the reminder files</summary>
        private void PopulateRemindersList()
        {
            foreach (string AbsoulutePath in Directory.GetFiles(Path.Combine(Helper.DirectoryPath, "data", Constants.SaveFolderName)))
            {
                string RelativePath = Utilities.Utilities.MakeRelativePath(AbsoulutePath);
                Reminders.Add(Helper.Data.ReadJsonFile<ReminderModel>(RelativePath));
            }
        }

        /// <summary>
        /// Handles the left click on the UI elements
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="playSound"></param>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            // clicked next page
            if (NextPageButton.containsPoint(x, y))
            {
                if (Reminders.Count > (PageIndex + 1) * 5)
                {
                    this.PageIndex += 1;
                    SetUpUI();
                }
            }
            // clicked previous page
            else if (PrevPageButton.containsPoint(x, y))
            {
                if (PageIndex != 0)
                {
                    this.PageIndex -= 1;
                    SetUpUI();
                }
            }

            // clicked new reminder
            if (NewReminderButton.containsPoint(x, y))
                Game1.activeClickableMenu = new ReminderMenuPage1(this.Page1OnChangeBehaviour, Helper);

            // clicked delete button
            int reminderindex = 0;
            foreach (ClickableTextureComponent deleteButton in this.DeleteButtons)
            {
                reminderindex++;
                if (deleteButton.containsPoint(x, y))
                {
                    int ReminderIndex = (this.PageIndex * 5) + reminderindex;
                    Utilities.Utilities.DeleteReminder(ReminderIndex, Helper);
                    SetUpUI();
                    break;
                }
            }
        }

        /// <summary>Defines what to do when hovering over UI elements</summary>
        public override void performHoverAction(int x, int y)
        {
            this.NewReminderButton.scale = this.NewReminderButton.containsPoint(x, y)
                ? Math.Min(this.NewReminderButton.scale + 0.02f, this.NewReminderButton.baseScale + 0.1f)
                : Math.Max(this.NewReminderButton.scale - 0.02f, this.NewReminderButton.baseScale);
        }

        /// <summary>The draw calls for the UI elements</summary>
        public override void draw(SpriteBatch b)
        {
            this.CursorPosition = this.Helper.Input.GetCursorPosition();

            // supress the Menu button
            Helper.Input.Suppress(MenuButton);

            // draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(this.xPositionOnScreen, this.yPositionOnScreen, this.width, this.height - 12, false, true);

            // draw title scroll
            SpriteText.drawStringWithScrollCenteredAt(b, "Display Reminder", Game1.viewport.Width / 2, this.yPositionOnScreen, "Display Reminder");

            // draw boxes
            foreach (ClickableTextureComponent box in this.Boxes)
                box.draw(b);

            // draw labels
            foreach (ClickableComponent label in this.ReminderMessages)
            {
                string text = "";
                Color color = Game1.textColor;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
                if (text.Length > 0)
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((label.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (label.bounds.Y + Game1.tileSize / 2)), color);
            }
            if (Reminders.Count > (PageIndex + 1) * 5)
                NextPageButton.draw(b);
            if (PageIndex != 0)
                PrevPageButton.draw(b);

            // draw the delete buttons
            foreach (ClickableTextureComponent button in this.DeleteButtons)
            {
                button.draw(b);
            }

            // draw the warning for no reminder
            if (Reminders.Count <= 0)
                Utility.drawTextWithShadow(b, NoRemindersWarning.name, Game1.smallFont, new Vector2(NoRemindersWarning.bounds.X, NoRemindersWarning.bounds.Y), Game1.textColor);

            // draw new reminders button
            NewReminderButton.draw(b);

            // draw the boxes hover text
            foreach (ClickableTextureComponent box in Boxes)
            {
                if (box.containsPoint((int)CursorPosition.ScreenPixels.X, (int)CursorPosition.ScreenPixels.Y))
                {
                    if (box.hoverText != null)
                    {
                        int x = Game1.getMouseX() + 32;
                        int y = Game1.getMouseY() + 32 + 16;
                        IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y - 16, Utilities.Utilities.EstimateStringDimension(box.hoverText) + 8, Game1.tileSize + 16, Color.White, 1f, true);
                        SpriteText.drawString(b, box.hoverText, x + 32, y, 999, -1, 99, 1f, 0.88f, false, -1, "", 8, SpriteText.ScrollTextAlignment.Left);
                    }
                }
            }

            // draw cursor
            this.drawMouse(b);
        }
    }
}