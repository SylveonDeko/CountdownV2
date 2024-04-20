using ReactiveUI;
using System;
using System.Reactive.Linq;

namespace CountdownV2.ViewModels;

public class MainViewModel : ReactiveObject
{
    private string _logText = "Initializing...";
    public string LogText
    {
        get => _logText;
        set => this.RaiseAndSetIfChanged(ref _logText, value);
    }

    public MainViewModel()
    {
        var targetTime = new DateTime(2024, 5, 4);
        var scheduler = RxApp.MainThreadScheduler;
        Observable.Interval(TimeSpan.FromSeconds(1), scheduler)
            .Subscribe(_ =>
            {
                var timeFromNow = targetTime - DateTime.Now;
                LogText = timeFromNow.TotalSeconds > 0
                    ? $"{timeFromNow.Days}d {timeFromNow.Hours}h {timeFromNow.Minutes}m {timeFromNow.Seconds}s"
                    : "Countdown completed!";
            });
    }
}