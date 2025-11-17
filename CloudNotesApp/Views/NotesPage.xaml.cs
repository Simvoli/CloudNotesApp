using CloudNotesApp.Models;
using CloudNotesApp.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;

namespace CloudNotesApp.Views;

public partial class NotesPage : ContentPage
{
    private CloudNotesApp.Models.User currentUser;
    FirebaseService firebaseService = new FirebaseService();
    LocalNoteService localNoteService = new LocalNoteService();

    private List<Note> allNotes = new List<Note>();

    public NotesPage(User user)
    {
        InitializeComponent();
        currentUser = user;
        LoadNotes();
    }

    private async void LoadNotes()
    {
        try
        {
            List<Note> mergedNotes = new List<Note>();
            var localNotes = await localNoteService.GetNotesAsync();
            localNotes.ForEach(n => n.IsLocal = true);
            mergedNotes.AddRange(localNotes);

            if (currentUser != null && Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                var firebaseNotes = await firebaseService.GetNotesFromFirebaseAsync(currentUser);
                firebaseNotes.ToList().ForEach(n => n.IsLocal = false);
                mergedNotes.AddRange(firebaseNotes);
            }

            allNotes = mergedNotes.GroupBy(n => n.Id).Select(g => g.First()).ToList();
            NotesCollectionView.ItemsSource = allNotes;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnNewNoteClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NoteEditPage(currentUser, null));
    }
    private async void OnNoteTapped(object sender, EventArgs e)
    {
        if (sender is Frame frame && frame.BindingContext is Note selectedNote)
        {
            await Navigation.PushAsync(new NoteEditPage(currentUser, selectedNote));
        }
    }

    private async void OnLogoutClicked(object sender, EventArgs e)
    {
        await Navigation.PopToRootAsync();
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        var keyword = e.NewTextValue?.ToLower() ?? "";
        var filteredNotes = allNotes.Where(n => n.Content.ToLower().Contains(keyword)).ToList();
        NotesCollectionView.ItemsSource = filteredNotes;
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadNotes();
    }

    private async void OnNoteSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var selectedNote = e.CurrentSelection.FirstOrDefault() as Note;
        if (selectedNote == null)
            return;
        ((CollectionView)sender).SelectedItem = null;
        await Navigation.PushAsync(new NoteEditPage(currentUser, selectedNote));
    }

}