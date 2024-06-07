using Gtk;
using System;

public class MemoryGame : Gtk.Window
{
    private Gtk.Button btnTimer;
    private Gtk.Button btnAttempts;
    private Label lblCurrentSetting; // Correctly initialized to avoid nullable warnings

    public MemoryGame() : base("Memory Game")
    {
        SetDefaultSize(1100, 700);
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
                background-color: #808080; /* Use for selected state */
            }
            label#gameSettingLabel {
                background-color: transparent;
                border-radius: 0;
                font-size: 15px;
                color: white;
                padding: 5px 10px;
            }
            button#card {
                border-radius: 10px;
                background-color: #F5F5F5;
                color: #333333;
                font-size: 16px;
            }
            label#levelLabel {
                font-size: 30px;
                color: white;
                margin-top: 50px; /* Adjusted for aesthetic spacing */
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
        InitializeGameLevel();
    }

    private void InitializeGameLevel()
    {
        VBox vbox = new VBox(false, 10);
        Add(vbox);

        Label levelLabel = new Label("LEVEL 1") { Name = "levelLabel" };
        vbox.PackStart(levelLabel, false, false, 0);

        Grid cardGrid = new Grid
        {
            RowSpacing = 10,
            ColumnSpacing = 10,
            Halign = Align.Center,
            Valign = Align.Center
        };
        vbox.PackStart(cardGrid, true, true, 0);
        InitializeCards(cardGrid, 8); // Initialize 8 cards for Level 1

        ShowAll();
    }

    private void InitializeCards(Grid cardGrid, int numberOfCards)
    {
        int cardsPerRow = 4;
        for (int i = 0; i < 2; i++)
        {
            for (int j = 0; j < cardsPerRow; j++)
            {
                Button card = new Button { Name = "card" };
                card.WidthRequest = 150;
                card.HeightRequest = 200;
                card.Clicked += OnCardClicked;
                cardGrid.Attach(card, j, i, 1, 1);
            }
        }
    }

    private void OnCardClicked(object sender, EventArgs e)
    {
        Button card = sender as Button;
        Random rnd = new Random();
        int num = rnd.Next(100);
        card.Label = num.ToString();
    }

    public static void Main()
    {
        Application.Init();
        new MemoryGame();
        Application.Run();
    }
}
