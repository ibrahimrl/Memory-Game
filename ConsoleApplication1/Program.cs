#pragma warning disable CS0612 // Suppress obsolete warnings for this example

using Gtk;
using Gdk;

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
                color: #808080; /* Use background-color for clarity */
            }
            label#gameSettingLabel {
                background-color: transparent;
                border-radius: 0;
                font-size: 15px;
                color: white;
                padding: 5px 10px;
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
        string setting = btnTimer.Name == "grey" ? "Timer" : "Limited Attempts";
        Console.WriteLine($"Start with setting: {setting}");
    }

    public static void Main()
    {
        Application.Init();
        new MemoryGame();
        Application.Run();
    }
}
