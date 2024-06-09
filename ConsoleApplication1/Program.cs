﻿using Gtk;
using System;

public class MemoryGame : Gtk.Window
{
    private int currentLevel = 1;
    private const int CardsIncrementPerLevel = 4;
    private Gtk.Button btnTimer;
    private Gtk.Button btnAttempts;
    private Label lblCurrentSetting;
    private Dictionary<Button, int> cardNumbers = new Dictionary<Button, int>();

    public MemoryGame() : base("Memory Game")
    {
        SetDefaultSize(1350, 800);
        SetPosition(WindowPosition.Center);
        DeleteEvent += delegate { Application.Quit(); };

        // Apply CSS
        CssProvider cssProvider = new CssProvider();
        cssProvider.LoadFromData(@"
            window {
                background-color: #5C7C89;
            }
            button {
                background-color: #FFFFFF;
                color: black;
                border-radius: 100px;
                padding: 10px 20px;
                font-size: 20px;
                font-family: 'Arial';
            }
            button#grey {
                color: #808080; /* Use for selected state */
            }
            label#gameSettingLabel {
                background-color: transparent;
                border-radius: 0;
                font-size: 15px;
                color: white;
                padding: 5px 10px;
            }
            button#card {
                background-color: #FFFFFF;
                color: black;
                border-radius: 10px;
                padding: 5px 10px;
                font-size: 14px;
                font-family: 'Arial';
            }
            label#levelLabel {
                font-size: 30px;
                color: white;
                margin-top: 50px; /* Adjusted for aesthetic spacing */
            }
            button#nextLevel {
                margin-top: 20px;
                color: green;
            }
        ");
        StyleContext.AddProviderForScreen(Gdk.Screen.Default, cssProvider, Gtk.StyleProviderPriority.Application);

        VBox vbox = new VBox(false, 0);
        Alignment vboxAlign = new Alignment(0.5f, 0.5f, 0, 0);
        vboxAlign.Add(vbox);
        Add(vboxAlign);

        Button btnStart = new Button("Start");
        Alignment alignStart = new Alignment(0.5f, 0.5f, 0, 0);
        alignStart.Add(btnStart);
        btnStart.Clicked += OnStartClicked;
        vbox.PackStart(alignStart, false, false, 10);

        Label lblSetting = new Label("CHOOSE A GAME SETTING") { Name = "gameSettingLabel" };
        Alignment alignLabel = new Alignment(0.5f, 0.5f, 0, 0);
        alignLabel.Add(lblSetting);
        vbox.PackStart(alignLabel, false, false, 10);

        HBox hbox = new HBox(true, 10);
        btnTimer = new Button("Timer");
        btnAttempts = new Button("Limited Attempts");
        btnTimer.Clicked += (sender, e) => ToggleButton(sender, "Timer");
        btnAttempts.Clicked += (sender, e) => ToggleButton(sender, "Limited Attempts");
        hbox.PackStart(btnTimer, true, true, 0);
        hbox.PackStart(btnAttempts, true, true, 0);

        Alignment alignHbox = new Alignment(0.5f, 0.5f, 0, 0);
        alignHbox.Add(hbox);
        vbox.PackStart(alignHbox, false, false, 10);

        ShowAll();
    }

    private void ToggleButton(object sender, string setting)
    {
        btnTimer.Name = setting == "Timer" ? "grey" : "default";
        btnAttempts.Name = setting == "Limited Attempts" ? "grey" : "default";
    }

    private void OnStartClicked(object sender, EventArgs e)
    {
        foreach (Widget child in this.Children)
        {
            this.Remove(child);
        }
        InitializeGameLevel(currentLevel);
    }
    
    private void InitializeGameLevel(int level)
    {
        VBox vbox = new VBox(false, 10) { MarginTop = 20 };
        Add(vbox);

        Label levelLabel = new Label($"LEVEL {level}") { Name = "levelLabel" };
        vbox.PackStart(levelLabel, false, false, 0);

        int cardsPerRow = CalculateCardsPerRow(level);
        int numRows = CalculateNumberOfRows(level);

        // Use an Alignment to center the Grid
        Alignment alignGrid = new Alignment(0.5f, 0.5f, 0, 0) { Name = "cardGridAlignment" };
        Grid cardGrid = new Grid
        {
            RowSpacing = 10,
            ColumnSpacing = 10
        };

        InitializeCards(cardGrid, numRows, cardsPerRow);

        alignGrid.Add(cardGrid);
        vbox.PackStart(alignGrid, true, true, 0);

        if (level < 5) {
            Button nextLevelButton = new Button("Next Level") { Name = "nextLevel" };
            nextLevelButton.Clicked += (sender, e) => {
                vbox.Destroy();
                currentLevel++;
                InitializeGameLevel(currentLevel);
            };
            vbox.PackEnd(nextLevelButton, false, false, 0);
        }

        ShowAll();
    }

    private int CalculateCardsPerRow(int level) {
        switch (level) {
            case 1: return 3;
            case 2: return 6;
            case 3: return 6;
            case 4: return 8;
            case 5: return 10;
            default: return 3; // Default to smallest number to avoid errors
        }
    }

    private int CalculateNumberOfRows(int level) {
        switch (level) {
            case 1:
            case 2: return 2;
            case 3:
            case 4:
            case 5: return 3;
            default: return 2;
        }
    }
    
    private (int width, int height) CalculateCardSize(int cardsPerRow, int numberOfRows)
    {
        int maxWidth = 1000;  // Maximum total width available
        int maxHeight = 500;  // Maximum total height available
        int horizontalSpacing = 10;  // Space between columns
        int verticalSpacing = 10;    // Space between rows
        int maxCardWidth = (maxWidth - (horizontalSpacing * (cardsPerRow - 1))) / cardsPerRow;
        int maxCardHeight = (maxHeight - (verticalSpacing * (numberOfRows - 1))) / numberOfRows;

        // Ensure cards are proportionally sized, assuming a card ratio of approximately 3:4 (width:height)
        int finalCardWidth = maxCardWidth;
        int finalCardHeight = finalCardWidth * 4 / 3;

        if (finalCardHeight > maxCardHeight)
        {
            finalCardHeight = maxCardHeight;
            finalCardWidth = finalCardHeight * 3 / 4;
        }

        return (finalCardWidth, finalCardHeight);
    }

    
    private void InitializeCards(Grid cardGrid, int rows, int cardsPerRow)
    {
        (int cardWidth, int cardHeight) = CalculateCardSize(10, 3);

        Gdk.Pixbuf originalPixbuf = new Gdk.Pixbuf("img/Card.jpg");
        Gdk.Pixbuf cardBackPixbuf = originalPixbuf.ScaleSimple(cardWidth, cardHeight, Gdk.InterpType.Bilinear);

        for (int i = 0; i < rows; i++) {
            for (int j = 0; j < cardsPerRow; j++) {
                Button card = new Button() { Name = "card" };
                card.WidthRequest = cardWidth;
                card.HeightRequest = cardHeight;

                Image cardImage = new Image(cardBackPixbuf);
                card.Add(cardImage);

                card.Clicked += OnCardClicked;

                cardGrid.Attach(card, j, i, 1, 1);
            }
        }
    }
    
    private void OnCardClicked(object sender, EventArgs e)
    {
        Button card = sender as Button;
        if (!cardNumbers.TryGetValue(card, out int num))
        {
            Random rnd = new Random();
            num = rnd.Next(100);
            cardNumbers[card] = num;
        }

        // Update the card's display to show the number
        Image cardImage = card.Child as Image;
        if (cardImage != null)
        {
            Gdk.Pixbuf numberPixbuf = CreateTextPixbuf(num.ToString(), card.WidthRequest, card.HeightRequest);
            cardImage.Pixbuf = numberPixbuf;
        }
    }
    
    private Gdk.Pixbuf CreateTextPixbuf(string text, int width, int height)
    {
        // Create a new Pixbuf with a white background
        Gdk.Pixbuf pixbuf = new Gdk.Pixbuf(Gdk.Colorspace.Rgb, true, 8, width, height);
        pixbuf.Fill(0xffffffff);  // Fill the pixbuf with white color

        // Create a Cairo surface from the Pixbuf
        using (Cairo.Surface surface = new Cairo.ImageSurface(pixbuf.Pixels, Cairo.Format.Argb32, pixbuf.Width, pixbuf.Height, pixbuf.Rowstride))
        {
            using (Cairo.Context cr = new Cairo.Context(surface))
            {
                // Configure text properties
                cr.SetSourceRGB(0, 0, 0); 
                cr.SelectFontFace("Arial", Cairo.FontSlant.Normal, Cairo.FontWeight.Bold);
                cr.SetFontSize(24);

                // Calculate text position for center alignment
                Cairo.TextExtents extents = cr.TextExtents(text);
                double xText = (width - extents.Width) / 2 - extents.XBearing;
                double yText = (height - extents.Height) / 2 - extents.YBearing;
                
                cr.MoveTo(xText, yText);
                cr.ShowText(text);
                cr.Fill();
            }
        }

        return pixbuf;
    }


    public static void Main()
    {
        Application.Init();
        new MemoryGame();
        Application.Run();
    }
}
