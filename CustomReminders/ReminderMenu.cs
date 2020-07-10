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
    internal class NewReminder_Page1 : IClickableMenu
    {
        private ClickableTextureComponent DisplayRemindersButton;

        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();
        private readonly List<ClickableTextureComponent> SeasonButtons = new List<ClickableTextureComponent>();
        private readonly List<ClickableTextureComponent> DayButtons = new List<ClickableTextureComponent>();

        private TextBox ReminderTextBox;
        protected ClickableTextureComponent OkButton;

        private string ReminderMessage;
        private string ReminderSeason;
        private int ReminderDate;

        /// <summary>The callback to invoke when the ok button is pressed</summary>
        private readonly Action<string, string, int> OnChanged;

        /// <summary>Construct an instance of the first page.</summary>
        /// <param name="onChanged"></param>
        public NewReminder_Page1(Action<string, string, int> onChanged)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            OnChanged = onChanged;
            SetUpPositions();
        }

        /// <summary>The method called when the game window changes size.</summary>
        /// <param name="oldBounds">The former viewport.</param>
        /// <param name="newBounds">The new viewport.</param>
        public override void gameWindowSizeChanged(Rectangle oldBounds, Rectangle newBounds)
        {
            base.gameWindowSizeChanged(oldBounds, newBounds);
            xPositionOnScreen = Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2;
            yPositionOnScreen = Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize;
            SetUpPositions();
        }


        /// <summary>Regenerate the UI.</summary>
        private void SetUpPositions()
        {
            Labels.Clear();
            DayButtons.Clear();

            OkButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            DisplayRemindersButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - Game1.tileSize * 5 - IClickableMenu.spaceToClearSideBorder * 2, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder, Game1.tileSize * 5 + Game1.tileSize / 4 + Game1.tileSize / 8, Game1.tileSize + Game1.tileSize / 8), Utilities.Data.Helper.Content.Load<Texture2D>("assets/DisplayReminders.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8, 1, 1), "Reminder Message: "));

            ReminderTextBox = new TextBox(null, null, Game1.smallFont, Game1.textColor)
            {
                X = xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1,
                Y = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - Game1.tileSize / 4 + Game1.tileSize / 2 + Game1.tileSize,
                Width = width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize * 3 - Game1.tileSize / 4,
                Height = 180
            };
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)ReminderTextBox;

            SeasonButtons.Add(new ClickableTextureComponent("Spring", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.10) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(188, 438, 32, 9), Game1.pixelZoom));
            SeasonButtons.Add(new ClickableTextureComponent("Summer", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.10) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(220, 438, 32, 8), Game1.pixelZoom));
            SeasonButtons.Add(new ClickableTextureComponent("Fall", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 5 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.1) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(188, 447, 32, 10), Game1.pixelZoom));
            SeasonButtons.Add(new ClickableTextureComponent("Winter", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 7 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + (int)(Game1.tileSize * 3.1) - Game1.tileSize / 4, Game1.tileSize * 2, Game1.tileSize), "", "", Game1.mouseCursors, new Rectangle(220, 448, 32, 8), Game1.pixelZoom));


            DayButtons.Add(new ClickableTextureComponent("1", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("2", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("3", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 3 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(24, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("4", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 4 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(32, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("5", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 5 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(40, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("6", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 6 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(48, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("7", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 7 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 4 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(56, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("8", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(64, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("9", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 2 - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize * 1, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(72, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("10", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("10", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(0, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("11", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("11", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("12", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("12", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("13", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("13", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(24, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("14", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("14", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 7.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 5 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(32, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("15", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 0.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("15", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 1.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(40, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("16", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 1.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("16", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(48, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("17", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("17", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(56, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("18", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("18", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(64, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("19", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("19", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(72, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("20", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("20", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(0, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("21", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("21", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 7.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 6 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(8, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("22", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 0.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("22", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 1.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("23", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 1.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("23", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(24, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("24", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 2.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("24", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(32, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("25", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 3.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("25", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(40, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("26", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 4.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("26", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(48, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("27", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 5.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("27", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(56, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("28", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 6.75) - Game1.tileSize / 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(16, 16, 8, 12), Game1.pixelZoom));
            DayButtons.Add(new ClickableTextureComponent("28", new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + (int)(Game1.tileSize * 7.25) - Game1.tileSize / 3, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * 7 - Game1.tileSize / 4, Game1.tileSize / 2, Game1.tileSize), "", "", Game1.content.Load<Texture2D>("LooseSprites\\font_bold"), new Rectangle(64, 16, 8, 12), Game1.pixelZoom));
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
                    ReminderSeason = name.ToLower();
                    break;
                // OK button
                case "OK":
                    if ((ReminderDate >= 1 || ReminderDate <= 28) && IsOkButtonReady())
                    {
                        ReminderMessage = ReminderTextBox.Text;
                        OnChanged(ReminderMessage, ReminderSeason, ReminderDate);
                    }
                    break;
                default:
                    ReminderDate = Convert.ToInt32(name);
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
            if (ReminderDate != 0 && ReminderSeason != "" && ReminderTextBox.Text != null && ReminderTextBox.Text != "")
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

            ReminderTextBox.Update();

            //If the season is not selected then the day buttons can't be clicked. Thanks to @Potato#5266 on the SDV discord for this tip.
            if (ReminderSeason == "spring" || ReminderSeason == "summer" || ReminderSeason == "fall" || ReminderSeason == "winter")
            {
                foreach (ClickableTextureComponent button in DayButtons)
                {
                    if (button.containsPoint(x, y))
                    {
                        HandleButtonClick(button.name);
                        button.scale -= 0.5f;
                        button.scale = Math.Max(3.5f, button.scale);
                    }
                }
            }

            foreach (ClickableTextureComponent button in SeasonButtons)
            {
                if (button.containsPoint(x, y))
                {
                    HandleButtonClick(button.name);
                    button.scale -= 0.5f;
                    button.scale = Math.Max(3.5f, button.scale);
                }
            }

            if (OkButton.containsPoint(x, y) && IsOkButtonReady())
            {
                HandleButtonClick(OkButton.name);
                OkButton.scale -= 0.25f;
                OkButton.scale = Math.Max(0.75f, OkButton.scale);
            }

            if (DisplayRemindersButton.containsPoint(x, y))
                Game1.activeClickableMenu = new DisplayReminders(OnChanged);

            ReminderTextBox.Update();
        }


        /// <summary>The method invoked when the player hovers the cursor over the menu.</summary>
        /// <param name="x">The X-position of the cursor.</param>
        /// <param name="y">The Y-position of the cursor.</param>
        public override void performHoverAction(int x, int y)
        {
            foreach (ClickableTextureComponent button in DayButtons)
            {
                button.scale = button.containsPoint(x, y)
                    ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                    : Math.Max(button.scale - 0.02f, button.baseScale);
            }

            foreach (ClickableTextureComponent button in SeasonButtons)
            {
                button.scale = button.containsPoint(x, y)
                    ? Math.Min(button.scale + 0.02f, button.baseScale + 0.1f)
                    : Math.Max(button.scale - 0.02f, button.baseScale);
            }

            OkButton.scale = OkButton.containsPoint(x, y) && IsOkButtonReady()
                ? Math.Min(OkButton.scale + 0.02f, OkButton.baseScale + 0.1f)
                : Math.Max(OkButton.scale - 0.02f, OkButton.baseScale);

            DisplayRemindersButton.scale = DisplayRemindersButton.containsPoint(x, y)
                ? Math.Min(DisplayRemindersButton.scale + 0.02f, DisplayRemindersButton.baseScale + 0.1f)
                : Math.Max(DisplayRemindersButton.scale - 0.02f, DisplayRemindersButton.baseScale);
        }

        /// <summary>Draw the menu to the screen.</summary>
        /// <param name="b">The sprite batch.</param>
        public override void draw(SpriteBatch b)
        {
            Utilities.Data.Helper.Input.Suppress(Utilities.Data.MenuButton);

            //draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // draw title scroll
            SpriteText.drawStringWithScrollCenteredAt(b, "New Reminder", Game1.viewport.Width / 2, yPositionOnScreen, "New Reminder");

            // draw textbox
            ReminderTextBox.Draw(b, false);

            // draw day buttons
            foreach (ClickableTextureComponent button in DayButtons)
                button.draw(b);

            // draw season buttons
            foreach (ClickableTextureComponent button in SeasonButtons)
                button.draw(b);

            // draw labels
            foreach (ClickableComponent label in Labels)
            {
                Color color = Color.Violet;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
            }
            foreach (ClickableComponent label in Labels)
            {
                string text = "";
                Color color = Game1.textColor;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
                if (text.Length > 0)
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((label.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (label.bounds.Y + Game1.tileSize / 2)), color);
            }

            // draw OK button
            if (ReminderDate != 0 && ReminderSeason != "" && IsOkButtonReady())
                OkButton.draw(b);
            else
            {
                OkButton.draw(b);
                OkButton.draw(b, Color.Black * 0.5f, 0.97f);
            }

            // draw displayreminder button
            DisplayRemindersButton.draw(b);

            // draw cursor
            drawMouse(b);

        }
    }

    /// <summary>
    /// Second page of the menu that sets the reminder's time
    /// </summary>
    internal class NewReminder_Page2 : IClickableMenu
    {
        private readonly List<ClickableComponent> Labels = new List<ClickableComponent>();

        private TextBox TimeTextBox;
        private ClickableTextureComponent OkButton;

        /// <summary>Field that contains the Time inputed</summary>
        internal int ReminderTime = 0;

        /// <summary>The callback to invoke when the birthday value changes.</summary>
        private readonly Action<int> OnChanged;

        /// <summary>The callback function that gets called when ok buttong is pressed</summary>
        public NewReminder_Page2(Action<int> onChange)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            this.OnChanged = onChange;
            //this.MenuButton = Utilities.Utilities.GetMenuButton();
            SetupPositions();
        }

        /// <summary>Generates the UI</summary>
        private void SetupPositions()
        {
            Labels.Clear();
            OkButton = new ClickableTextureComponent("OK", new Rectangle(xPositionOnScreen + width - IClickableMenu.borderWidth - IClickableMenu.spaceToClearSideBorder - Game1.tileSize, yPositionOnScreen + height - IClickableMenu.borderWidth - IClickableMenu.spaceToClearTopBorder + Game1.tileSize / 4, Game1.tileSize, Game1.tileSize), "", null, Game1.mouseCursors, Game1.getSourceRectForStandardTileSheet(Game1.mouseCursors, 46), 1f);
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + 8, 1, 1), "Reminder Time: "));
            Labels.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize * 1 + 4, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 8 + Game1.tileSize * 3, 1, 1), $"Set the time of the reminder in 24hrs \nformat in multiples of 30 in-game minutes.\n\ne.g. 1400 (=2PM)\n     0730 (=7AM)"));
            TimeTextBox = new TextBox(null, null, Game1.smallFont, Game1.textColor)
            {
                X = xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize,
                Y = yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + IClickableMenu.borderWidth - Game1.tileSize / 4 + Game1.tileSize + Game1.tileSize / 2,
                Width = 384 + Game1.tileSize,
                Height = 180,
                numbersOnly = true,
                textLimit = 4
            };
            Game1.keyboardDispatcher.Subscriber = (IKeyboardSubscriber)TimeTextBox;
        }

        /// <summary>Handles the left clicks on the menu</summary>
        public override void receiveLeftClick(int x, int y, bool playSound = true)
        {
            TimeTextBox.Update();
            if (OkButton.containsPoint(x, y))
            {
                if (TimeTextBox.Text == "" || TimeTextBox.Text == null) return;
                ReminderTime = Convert.ToInt32(TimeTextBox.Text);

                if (IsOkButtonReady())
                {
                    Game1.playSound("coin");
                    OkButton.scale -= 0.25f;
                    OnChanged(ReminderTime);
                    OkButton.scale = Math.Max(0.75f, OkButton.scale);
                    Game1.exitActiveMenu();
                }
            }
            TimeTextBox.Update();
        }

        /// <summary>Defines what to do when hovering over UI elements</summary>
        public override void performHoverAction(int x, int y)
        {
            OkButton.scale = OkButton.containsPoint(x, y) && IsOkButtonReady()
                ? Math.Min(OkButton.scale + 0.02f, OkButton.baseScale + 0.1f)
                : Math.Max(OkButton.scale - 0.02f, OkButton.baseScale);
        }

        /// <summary>
        /// Checks if the page1 inputs are all valid
        /// </summary>
        /// <returns>True if ok button is ready, False if not</returns>
        private bool IsOkButtonReady()
        {
            if (TimeTextBox.Text.Length == 4 && TimeTextBox.Text != null && TimeTextBox.Text != "" && (TimeTextBox.Text.ToString().EndsWith("30") || TimeTextBox.Text.ToString().EndsWith("00")) && (Convert.ToInt32(TimeTextBox.Text) >= 0600 && Convert.ToInt32(TimeTextBox.Text) <= 2600))
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
            Utilities.Data.Helper.Input.Suppress(Utilities.Data.MenuButton);

            // draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height, false, true);

            // draw textbox
            TimeTextBox.Draw(b, false);

            // draw labels
            foreach (ClickableComponent label in Labels)
            {
                Color color = Color.Violet;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
            }
            foreach (ClickableComponent label in Labels)
            {
                string text = "";
                Color color = Game1.textColor;
                Utility.drawTextWithShadow(b, label.name, Game1.smallFont, new Vector2(label.bounds.X, label.bounds.Y), color);
                if (text.Length > 0)
                    Utility.drawTextWithShadow(b, text, Game1.smallFont, new Vector2((label.bounds.X + Game1.tileSize / 3) - Game1.smallFont.MeasureString(text).X / 2f, (label.bounds.Y + Game1.tileSize / 2)), color);
            }

            // draw OK button
            if (IsOkButtonReady())
                OkButton.draw(b);
            else
            {
                OkButton.draw(b);
                OkButton.draw(b, Color.Black * 0.5f, 0.97f);
            }

            // draw cursor
            drawMouse(b);
        }
    }

    /// <summary>
    /// UI to display the currently set reminders
    /// </summary>
    public class DisplayReminders : IClickableMenu
    {
        private readonly List<ClickableTextureComponent> DeleteButtons = new List<ClickableTextureComponent>();
        private readonly List<ClickableComponent> ReminderMessages = new List<ClickableComponent>();
        private readonly List<ClickableTextureComponent> Boxes = new List<ClickableTextureComponent>();
        private readonly List<ReminderModel> Reminders = new List<ReminderModel>();

        private readonly ClickableTextureComponent NextPageButton;
        private readonly ClickableTextureComponent PrevPageButton;
        private readonly ClickableTextureComponent NewReminderButton;
        private readonly ClickableComponent NoRemindersWarning;

        ///<summary>This is required for switching to New Reminders menu (for its constructor requires this call back function)</summary>
        private readonly Action<string, string, int> Page1OnChangeBehaviour;

        private ICursorPosition CursorPosition;
        private int PageIndex = 0;

        /// <summary>Construct an instance.</summary>
        /// <param name="page1OnChangeBehaviour">Required to switch to the New Reminder menu (as its constructor requires this callback function)</param>
        public DisplayReminders(Action<string, string, int> page1OnChangeBehaviour)
            : base(Game1.viewport.Width / 2 - (632 + IClickableMenu.borderWidth * 2) / 2, Game1.viewport.Height / 2 - (600 + IClickableMenu.borderWidth * 2) / 2 - Game1.tileSize, 632 + IClickableMenu.borderWidth * 2, 600 + IClickableMenu.borderWidth * 2 + Game1.tileSize)
        {
            //this.MenuButton = Utilities.Utilities.GetMenuButton();
            this.Page1OnChangeBehaviour = page1OnChangeBehaviour;

            SetUpUI();

            NextPageButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen + width + Game1.tileSize - Game1.tileSize / 2, yPositionOnScreen + height - Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Data.Helper.Content.Load<Texture2D>("assets/rightArrow.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
            PrevPageButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - Game1.tileSize, yPositionOnScreen + height - Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Data.Helper.Content.Load<Texture2D>("assets/leftArrow.png", ContentSource.ModFolder), new Rectangle(), 1.5f);

            NoRemindersWarning = new ClickableComponent(new Rectangle(xPositionOnScreen + width / 2 - width / 4 + Game1.tileSize / 2, yPositionOnScreen + height / 2, width, Game1.tileSize), "No reminders are set yet");
            NewReminderButton = new ClickableTextureComponent(new Rectangle(xPositionOnScreen - Game1.tileSize * 5 - IClickableMenu.spaceToClearSideBorder * 2, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder, Game1.tileSize * 5 + Game1.tileSize / 4 + Game1.tileSize / 8, Game1.tileSize + Game1.tileSize / 8), Utilities.Data.Helper.Content.Load<Texture2D>("assets/NewReminder.png", ContentSource.ModFolder), new Rectangle(), 1.5f);
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
            ReminderMessages.Clear();
            // Setup the boxes for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                    ReminderMessages.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize - Game1.tileSize / 16, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, 1, 1), Reminders[i + (PageIndex * 5)].ReminderMessage));
            }
            // Setup the boxes for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                    ReminderMessages.Add(new ClickableComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + IClickableMenu.borderWidth + Game1.tileSize - Game1.tileSize / 16, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, 1, 1), Reminders[i + (PageIndex * 5)].ReminderMessage));
            }
        }

        /// <summary>Regenerates the boxes (for page switches nad initializations)</summary>
        private void SetUpBoxes()
        {
            Boxes.Clear();
            // Setup the boxes for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                {
                    Boxes.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 16, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * (1 + (i * 2)) - Game1.tileSize - (8 * i), width - IClickableMenu.spaceToClearSideBorder * 2 - 32, Game1.tileSize * 2 - 16), Utilities.Data.Helper.Content.Load<Texture2D>("assets/reminderBox.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
                    Boxes[i].hoverText = Utilities.Converts.ConvertToPrettyTime(Reminders[i + (PageIndex * 5)].Time) + ", " + Utilities.Converts.ConvertToPrettyDate(Reminders[i + (PageIndex * 5)].DaysSinceStart);
                }
            }
            // Setup the boxes for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                {
                    Boxes.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen + IClickableMenu.spaceToClearSideBorder + 16, yPositionOnScreen + IClickableMenu.spaceToClearTopBorder + Game1.tileSize * (1 + (i * 2)) - Game1.tileSize - (8 * i), width - IClickableMenu.spaceToClearSideBorder * 2 - 16, Game1.tileSize * 2 - 16), Utilities.Data.Helper.Content.Load<Texture2D>("assets/reminderBox.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
                    Boxes[i].hoverText = Utilities.Converts.ConvertToPrettyTime(Reminders[i + (PageIndex * 5)].Time) + ", " + Utilities.Converts.ConvertToPrettyDate(Reminders[i + (PageIndex * 5)].DaysSinceStart);
                }
            }
        }

        /// <summary>Regenerates the reminder messages (for page switches and initializations)</summary>
        private void SetUpDeleteButtons()
        {
            DeleteButtons.Clear();
            // Setup the delete buttons for pages that are not the last
            if (Reminders.Count - (PageIndex * 5) >= 5)
            {
                for (int i = 0; i < 5; i++)
                    DeleteButtons.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth + width - Game1.tileSize * 1, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Data.Helper.Content.Load<Texture2D>("assets/deleteButton.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
            }
            // Setup the delete buttons for the last page
            else
            {
                for (int i = 0; i < Reminders.Count - (PageIndex * 5); i++)
                    DeleteButtons.Add(new ClickableTextureComponent(new Rectangle(xPositionOnScreen - IClickableMenu.spaceToClearSideBorder - IClickableMenu.borderWidth + width - Game1.tileSize * 1, yPositionOnScreen + IClickableMenu.borderWidth + IClickableMenu.spaceToClearTopBorder - Game1.tileSize / 2 + Game1.tileSize / 16 + Game1.tileSize * (i * 2) - Game1.tileSize / 2 - (i * 8) + Game1.tileSize, Game1.tileSize, Game1.tileSize), Utilities.Data.Helper.Content.Load<Texture2D>("assets/deleteButton.png", ContentSource.ModFolder), new Rectangle(), Game1.pixelZoom));
            }
        }

        /// <summary>This fills the Reminders list by reading all the reminder files</summary>
        private void PopulateRemindersList()
        {
            foreach (string AbsoulutePath in Directory.GetFiles(Path.Combine(Utilities.Data.Helper.DirectoryPath, "data", Utilities.Data.SaveFolderName)))
            {
                string RelativePath = Utilities.Extras.MakeRelativePath(AbsoulutePath);
                Reminders.Add(Utilities.Data.Helper.Data.ReadJsonFile<ReminderModel>(RelativePath));
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
                    PageIndex += 1;
                    SetUpUI();
                }
            }
            // clicked previous page
            else if (PrevPageButton.containsPoint(x, y))
            {
                if (PageIndex != 0)
                {
                    PageIndex -= 1;
                    SetUpUI();
                }
            }

            // clicked new reminder
            if (NewReminderButton.containsPoint(x, y))
                Game1.activeClickableMenu = new NewReminder_Page1(Page1OnChangeBehaviour);

            // clicked delete button
            int reminderindex = 0;
            foreach (ClickableTextureComponent deleteButton in DeleteButtons)
            {
                reminderindex++;
                if (deleteButton.containsPoint(x, y))
                {
                    int ReminderIndex = (PageIndex * 5) + reminderindex;
                    Utilities.Files.DeleteReminder(ReminderIndex);
                    SetUpUI();
                    break;
                }
            }
        }

        /// <summary>Defines what to do when hovering over UI elements</summary>
        public override void performHoverAction(int x, int y)
        {
            NewReminderButton.scale = NewReminderButton.containsPoint(x, y)
                ? Math.Min(NewReminderButton.scale + 0.02f, NewReminderButton.baseScale + 0.1f)
                : Math.Max(NewReminderButton.scale - 0.02f, NewReminderButton.baseScale);
        }

        /// <summary>The draw calls for the UI elements</summary>
        public override void draw(SpriteBatch b)
        {
            CursorPosition = Utilities.Data.Helper.Input.GetCursorPosition();

            // suppress the Menu button
            Utilities.Data.Helper.Input.Suppress(Utilities.Data.MenuButton);

            // draw screen fade
            b.Draw(Game1.fadeToBlackRect, Game1.graphics.GraphicsDevice.Viewport.Bounds, Color.Black * 0.75f);

            // draw menu box
            Game1.drawDialogueBox(xPositionOnScreen, yPositionOnScreen, width, height - 12, false, true);

            // draw title scroll
            SpriteText.drawStringWithScrollCenteredAt(b, "Display Reminders", Game1.viewport.Width / 2, yPositionOnScreen, "Display Reminder");

            // draw boxes
            foreach (ClickableTextureComponent box in Boxes)
                box.draw(b);

            // draw labels
            foreach (ClickableComponent label in ReminderMessages)
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
            foreach (ClickableTextureComponent button in DeleteButtons)
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
                        IClickableMenu.drawTextureBox(b, Game1.menuTexture, new Rectangle(0, 256, 60, 60), x, y - 16, Utilities.Extras.EstimateStringDimension(box.hoverText) + 8, Game1.tileSize + 16, Color.White, 1f, true);
                        SpriteText.drawString(b, box.hoverText, x + 32, y, 999, -1, 99, 1f, 0.88f, false, -1, "", 8, SpriteText.ScrollTextAlignment.Left);
                    }
                }
            }

            // draw cursor
            drawMouse(b);
        }
    }
}