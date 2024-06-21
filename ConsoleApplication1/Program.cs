using Gtk;
using System;
using System.Collections.Generic;

public class Game
{
    public int[] CardValues { get; private set; }
    public bool[] MatchedCards { get; private set; }
    
    // Constructor initializes the game with a specific number of cards
    public Game(int numberOfCards)
    {
        CardValues = new int[numberOfCards];
        MatchedCards = new bool[numberOfCards];
        InitializeCards(numberOfCards);
    }
    
    // Initializes the cards with random, non-repeating pairs of numbers
    private void InitializeCards(int numberOfCards)
    {
        int halfNumberOfCards = numberOfCards / 2;
        Random rng = new Random();
        HashSet<int> usedNumbers = new HashSet<int>(); // To avoid repeating numbers across pairs
        
        for (int i = 0; i < halfNumberOfCards; i++)
        {
            int newNumber;
            do
            {
                newNumber = rng.Next(10, 100);
            } while (usedNumbers.Contains(newNumber)); // Ensure each number is unique within pairs

            usedNumbers.Add(newNumber);
            CardValues[i] = newNumber;
            CardValues[i + halfNumberOfCards] = newNumber;
        }
        
        ShuffleCards(CardValues);
    }
    
    // Shuffles the cards to randomize their positions
    private void ShuffleCards(int[] array)
    {
        Random rng = new Random();
        for (int i = array.Length - 1; i > 0; i--)
        {
            int swapIndex = rng.Next(i + 1);  // Generate a random position up to and including the element i
            (array[i], array[swapIndex]) = (array[swapIndex], array[i]);
        }
    }
}

public class MemoryGame : Gtk.Window
{
    
    private Game game;
    private int currentLevel = 1;
    private const int CardsIncrementPerLevel = 4;
    private Gtk.Button btnTimer;
    private Gtk.Button btnAttempts;
    private Dictionary<Button, int> cardNumbers = new Dictionary<Button, int>();
    private Button previousCard = null;
    private int previousCardNumber = -1;
    private Label lblTimer;
    private Label lblAttempts;
    private uint timerId = 0;
    private int remainingTime;
    private int remainingAttempts;
    private bool isTimerMode = false;
    private bool isAttemptMode = false;
    private bool isWaiting = false;
    private VBox vbox;
    
    // Constructor sets up the window and UI elements
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
                border: none;
            }
            button#grey {
                color: #808080; /* Use for selected state */
            }
            label#gameSettingLabel, label#timerLabel {
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

        vbox = new VBox(false, 0);
        Alignment vboxAlign = new Alignment(0.5f, 0.5f, 0, 0);
        vboxAlign.Add(vbox);
        Add(vboxAlign);

        Button btnStart = new Button("Start");
        Alignment alignStart = new Alignment(0.5f, 0.5f, 0, 0);
        alignStart.Add(btnStart);
        btnStart.Clicked += OnStartClicked;
        vbox.PackStart(alignStart, false, false, 10);

        Label lblSetting = new Label("CHOOSE A GAME SETTING") {Name = "gameSettingLabel"};
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

        vbox.Remove(hbox);
        Alignment alignHbox = new Alignment(0.5f, 0.5f, 0, 0);
        alignHbox.Add(hbox);
        vbox.PackStart(alignHbox, false, false, 10);


        lblTimer = new Label("Remaining Time: 00:40") {Name = "timerLabel"};
        lblTimer.Hide();

        lblAttempts = new Label("Remaining Attempts: --") {Name = "attemptsLabel"};
        lblAttempts.Hide();

        ShowAll();
    }
    
    // Toggles between timer and limited attempts modes based on button clicks
    private void ToggleButton(object sender, string setting)
    {
        btnTimer.Name = setting == "Timer" ? "grey" : "default";
        btnAttempts.Name = setting == "Limited Attempts" ? "grey" : "default";
        isTimerMode = setting == "Timer";
        isAttemptMode = setting == "Limited Attempts";
    }
    
    // Initializes the game environment and game level when the start button is clicked
    private void OnStartClicked(object sender, EventArgs e)
    {
        ResetGameEnvironment();
        InitializeGameLevel(currentLevel);
        if (isTimerMode) 
        {
            if (lblTimer.Parent != null)
                ((Container)lblTimer.Parent).Remove(lblTimer);
            StartTimer(40);  // Start with 40 seconds in level 1
            vbox.PackStart(lblTimer, false, false, 0);
            lblTimer.Show();
        }
        else if (isAttemptMode)
        {
            if (lblAttempts.Parent != null)
                ((Container)lblAttempts.Parent).Remove(lblAttempts);
            remainingAttempts = CalculateCardsPerRow(currentLevel) * CalculateNumberOfRows(currentLevel) * 2; // Set attempts 2 times more than the number of cards in level 1
            lblAttempts.Text = $"Remaining Attempts: {remainingAttempts}";
            vbox.PackStart(lblAttempts, false, false, 0);
            lblAttempts.Show();
        }
    }
    
    // Resets the game environment to initial state
    private void ResetGameEnvironment()
    {
        
        foreach (Widget child in this.Children)
            if (child.Parent != null)
                ((Container)child.Parent).Remove(child);
        
        previousCard = null;
        previousCardNumber = -1;
        
        // If timer was running, stop it
        if (timerId != 0)
        {
            GLib.Source.Remove(timerId);
            timerId = 0;
        }
    }
    
    // Initializes the card grid and game state for the specified level
    private void InitializeGameLevel(int level)
    {
        
        vbox = new VBox(false, 10) { MarginTop = 20 };
        Add(vbox);

        Label levelLabel = new Label($"LEVEL {level}") { Name = "levelLabel" };
        vbox.PackStart(levelLabel, false, false, 0);

        int cardsPerRow = CalculateCardsPerRow(level);
        int numRows = CalculateNumberOfRows(level);
        int numberOfCards = cardsPerRow * numRows;
        
        game = new Game(numberOfCards);

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
        
        
        if (isTimerMode) 
        {
            StartTimer(40 + 30 * (level - 1) + 30 * (level - 2));  // Start with 40 seconds in level 1

            vbox.PackStart(lblTimer, false, false, 0);
            Alignment timerAlign = new Alignment(1.0f, 0.0f, 0, 0);
            timerAlign.Add(vbox);
            Add(timerAlign);
            lblTimer.Show();
        }
        else if (isAttemptMode)
        {
            remainingAttempts = level > 2 ? 3 * numberOfCards : 2 * numberOfCards; 
            lblAttempts.Text = $"Remaining Attempts: {remainingAttempts}";
            vbox.PackStart(lblAttempts, false, false, 0);
            Alignment attemotAlign = new Alignment(1.0f, 0.0f, 0, 0);
            attemotAlign.Add(vbox);
            Add(attemotAlign);
            lblAttempts.Show();
        }

        ShowAll();
    }
    
    // Starts or resets the timer for game countdown
    private void StartTimer(int seconds)
    {
        remainingTime = seconds;
        lblTimer.Text = $"Remaining Time: {remainingTime / 60:00}:{(remainingTime % 60):00}";
        if (timerId != 0)
            GLib.Source.Remove(timerId);
        timerId = GLib.Timeout.Add(1000, new GLib.TimeoutHandler(UpdateTimer));
    }
    
    // Updates the timer every second and handles timeout condition
    private bool UpdateTimer()
    {
        remainingTime--;
        lblTimer.Text = $"Remaining Time: {remainingTime / 60:00}:{(remainingTime % 60):00}";
        if (remainingTime <= 0)
        {
            GLib.Source.Remove(timerId);
            timerId = 0;
            GameOver();  // Implement this method to handle game over scenario
            return false;
        }
        return true;
    }
    
    // Displays the game over dialog and resets the game
    private void GameOver()
    {
        MessageDialog dialog = new MessageDialog(this, DialogFlags.Modal, MessageType.Info, ButtonsType.Ok, "Game over.");
        dialog.Run();
        dialog.Destroy();
        ResetGameEnvironment();
        new MemoryGame();
    }

    // Calculates the number of cards per row based on the level
    private int CalculateCardsPerRow(int level)
    {
        switch (level)
        {
            case 1: return 3;
            case 2: return 6;
            case 3: return 6;
            case 4: return 8;
            case 5: return 10;
            default: return 3; // Default to smallest number to avoid errors
        }
    }

    // Calculates the number of rows of cards based on the level
    private int CalculateNumberOfRows(int level)
    {
        switch (level)
        {
            case 1:
            case 2: return 2;
            case 3:
            case 4:
            case 5: return 3;
            default: return 2;
        }
    }
    
    // Calculates the appropriate card size based on the grid dimensions
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
    
    // Creates a pixbuf for the card back image based on the specified dimensions
    private Gdk.Pixbuf CreateCardBackPixbuf(int width, int height)
    {
        Gdk.Pixbuf originalPixbuf = new Gdk.Pixbuf("img/Card.jpg");
        return originalPixbuf.ScaleSimple(width, height, Gdk.InterpType.Bilinear);
    }

    // Initializes and places cards within the grid
    private void InitializeCards(Grid cardGrid, int rows, int cardsPerRow)
    {
        (int cardWidth, int cardHeight) = CalculateCardSize(CalculateCardsPerRow(5), CalculateNumberOfRows(5));
        
        Gdk.Pixbuf cardBackPixbuf = CreateCardBackPixbuf(cardWidth, cardHeight);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cardsPerRow; j++)
            {
                int index = i * cardsPerRow + j;
                Button card = new Button() { Name = "card"};
                card.WidthRequest = cardWidth;
                card.HeightRequest = cardHeight;
                card.Data.Add("index", index);  // Store index in the Button

                Image cardImage = new Image(cardBackPixbuf);
                card.Add(cardImage);

                card.Clicked += OnCardClicked;

                cardGrid.Attach(card, j, i, 1, 1);
            }
        }
    }

    // Handles card click events to flip cards and check for matches
    private void OnCardClicked(object sender, EventArgs e)
    {
        if (isWaiting) return;  // Prevent interaction while waiting
        
        Button card = sender as Button;
        
        int index = (int)card.Data["index"];
        int cardValue = game.CardValues[index];
        
    
        Label label = new Label(cardValue.ToString());
        label.ModifyFont(Pango.FontDescription.FromString("Arial Bold 15"));
        
    
        card.Add(label);
        card.ShowAll();
    
        // Update the card's display to show the number
        Image cardImage = card.Child as Image;
        if (cardImage != null)
        {
            Gdk.Pixbuf numberPixbuf = CreateTextPixbuf(cardValue.ToString(), card.WidthRequest, card.HeightRequest);
            cardImage.Pixbuf = numberPixbuf;
        }
        
        if (previousCard == null)
        {
            previousCard = card;
            previousCardNumber = cardValue;
        }
        else if (previousCard != card)
        {
            if (previousCardNumber != cardValue)
            {
                isWaiting = true;
                // Delay before flipping cards back
                GLib.Timeout.Add(1000, new GLib.TimeoutHandler(() =>
                {
                    // Flip both cards back to the hidden state
                    FlipCardBack(previousCard);
                    FlipCardBack(card);
        
                    if (isAttemptMode)
                    {
                        remainingAttempts--;
                        lblAttempts.Text = $"Remaining Attempts: {remainingAttempts}";
                        if (remainingAttempts <= 0)
                        {
                            GameOver();
                        }
                    }
        
                    previousCard = null;  // Reset for next try
                    isWaiting = false;
                    return false; // Stop the timeout from repeating
                }));
            }
            else
            {
                game.MatchedCards[index] = true;
                game.MatchedCards[(int)previousCard.Data["index"]] = true;
                previousCard = null;
    
                CheckGameProgress();
                
            }
        }
    }
    
    // Checks if all cards have been successfully matched
    private bool AllCardsMatched()
    {
    foreach (bool matched in game.MatchedCards)
    {
        if (!matched) return false;
    }
    return true;
    }
    
    // Checks game progress and handles level transitions
    private void CheckGameProgress()
    {
        if (AllCardsMatched())
        {
            if (currentLevel == 5)
            {
                ShowGameWonMessage();
            }
            else
            {
                AddNextLevelButton();
            }
        }
    }
    
    // Displays a congratulatory message when the game is won
    private void ShowGameWonMessage()
    {
        MessageDialog winDialog = new MessageDialog(
            this,
            DialogFlags.Modal,
            MessageType.Info,
            ButtonsType.Ok,
            "Congratulations! You've won the game!"
        );
        winDialog.Run();
        winDialog.Destroy();
        ResetGameEnvironment();
        new MemoryGame();
    }

    // Adds a button to proceed to the next level
    private void AddNextLevelButton()
    {
    Button nextLevelButton = new Button("Next Level") { Name = "nextLevel" };
    nextLevelButton.Clicked += (sender, e) =>
    {
        vbox.Destroy();
        currentLevel++;
        InitializeGameLevel(currentLevel);
    };
    vbox.PackEnd(nextLevelButton, false, false, 0);
    ShowAll();
    }
    
    // Flips a card back to its back image
    private void FlipCardBack(Button card)
    {
        Image cardImage = card.Child as Image;
        if (cardImage != null)
        {
            Gdk.Pixbuf backPixbuf = new Gdk.Pixbuf("img/Card.jpg", card.WidthRequest, card.HeightRequest);
            cardImage.Pixbuf = backPixbuf;
        }
    }

    // Creates a pixbuf with text for displaying card values
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

    // Entry point for the application
    public static void Main()
    {
        Application.Init();
        new MemoryGame();
        Application.Run();
    }
}
