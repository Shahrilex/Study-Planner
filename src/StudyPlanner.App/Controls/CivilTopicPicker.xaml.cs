using System.Windows;
using System.Windows.Controls;
using StudyPlanner.Core.Models;

namespace StudyPlanner.App.Controls;

/// <summary>انتخاب‌گر چندگزینه‌ای مباحث مدنی همراه با شماره مواد.</summary>
public partial class CivilTopicPicker : UserControl
{
    private bool _isLoading;

    public CivilTopicPicker()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty TopicProperty = DependencyProperty.Register(
        nameof(Topic), typeof(string), typeof(CivilTopicPicker),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, OnSelectionTextChanged));

    public static readonly DependencyProperty ArticleNumbersProperty = DependencyProperty.Register(
        nameof(ArticleNumbers), typeof(string), typeof(CivilTopicPicker),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty SelectedKeysProperty = DependencyProperty.Register(
        nameof(SelectedKeys), typeof(string), typeof(CivilTopicPicker),
        new FrameworkPropertyMetadata(string.Empty, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault));

    public static readonly DependencyProperty SelectedSubjectProperty = DependencyProperty.Register(
        nameof(SelectedSubject), typeof(SubjectType), typeof(CivilTopicPicker), new PropertyMetadata(SubjectType.Civil));

    public static readonly DependencyProperty DisplayTextProperty = DependencyProperty.Register(
        nameof(DisplayText), typeof(string), typeof(CivilTopicPicker), new PropertyMetadata("انتخاب مبحث/مباحث مدنی"));

    public string Topic { get => (string)GetValue(TopicProperty); set => SetValue(TopicProperty, value); }
    public string ArticleNumbers { get => (string)GetValue(ArticleNumbersProperty); set => SetValue(ArticleNumbersProperty, value); }
    public string SelectedKeys { get => (string)GetValue(SelectedKeysProperty); set => SetValue(SelectedKeysProperty, value); }
    public SubjectType SelectedSubject { get => (SubjectType)GetValue(SelectedSubjectProperty); set => SetValue(SelectedSubjectProperty, value); }

    public string DisplayText { get => (string)GetValue(DisplayTextProperty); private set => SetValue(DisplayTextProperty, value); }

    public event EventHandler? SelectionSaved;

    private static void OnSelectionTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        => ((CivilTopicPicker)d).DisplayText = string.IsNullOrWhiteSpace((string?)e.NewValue) ? "انتخاب مبحث/مباحث مدنی" : (string)e.NewValue;

    private void OpenPicker_Click(object sender, RoutedEventArgs e)
    {
        var selected = SelectedKeys.Split(';', StringSplitOptions.RemoveEmptyEntries).ToHashSet();
        _isLoading = true;
        OptionsPanel.Children.Clear();
        foreach (var option in CivilTopicCatalog.GetOptions(SelectedSubject))
        {
            var checkBox = new CheckBox
            {
                Content = option.Title,
                Tag = option,
                IsChecked = selected.Contains(option.Key),
                Margin = new Thickness(2, 3, 2, 3)
            };
            checkBox.Checked += TopicCheckBox_Changed;
            checkBox.Unchecked += TopicCheckBox_Changed;
            OptionsPanel.Children.Add(checkBox);
        }
        _isLoading = false;
        PickerPopup.IsOpen = true;
    }

    private void TopicCheckBox_Changed(object sender, RoutedEventArgs e)
    {
        if (_isLoading) return;
        ApplySelection();
        // اگر کاربر با کلیک بیرون از پنجره فهرست را ببندد، انتخاب‌ها از دست نمی‌روند.
        SelectionSaved?.Invoke(this, EventArgs.Empty);
    }

    private void ClosePicker_Click(object sender, RoutedEventArgs e)
    {
        ApplySelection();
        PickerPopup.IsOpen = false;
        SelectionSaved?.Invoke(this, EventArgs.Empty);
    }

    private void ApplySelection()
    {
        var selected = new List<CivilTopicOption>();
        foreach (var checkBox in OptionsPanel.Children.OfType<CheckBox>())
        {
            if (checkBox.Tag is CivilTopicOption option && checkBox.IsChecked == true)
                selected.Add(option);
        }

        SelectedKeys = string.Join(';', selected.Select(x => x.Key));
        Topic = string.Join("، ", selected.Select(x => x.Title));
        ArticleNumbers = string.Join("، ", selected.Select(x => x.ArticleNumbers));
    }

}
