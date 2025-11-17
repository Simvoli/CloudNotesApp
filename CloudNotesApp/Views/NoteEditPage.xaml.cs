using CloudNotesApp.Models;
using CloudNotesApp.Services;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Devices;
using System;
using System.Threading.Tasks;


namespace CloudNotesApp.Views;

public partial class NoteEditPage : ContentPage
{
    private User currentUser;
    private Note currentNote;
    private InternetChecker internetChecker = new InternetChecker();
    FirebaseService firebaseService = new FirebaseService();
    LocalNoteService localNoteService = new LocalNoteService();

    public NoteEditPage(User user, Note note)
    {
        InitializeComponent();
        currentUser = user;
        currentNote = note ?? new Note { Timestamp = DateTime.Now };

        if (note != null)
        {
            NoteEditor.Text = note.Content;
        }
        else
        {
            DeleteButton.IsVisible = false;
        }


        NetworkIndicator.BindingContext = internetChecker;
        UpdateLastModifiedLabel();
    }

    private void UpdateNetworkIndicator(NetworkAccess access)
    {
        if (access == NetworkAccess.Internet)
            NetworkIndicator.Source = "online.png";
        else
            NetworkIndicator.Source = "offline.png";
    }

    private async void OnSaveClicked(object sender, EventArgs e)
    {
        currentNote.Content = NoteEditor.Text;
        currentNote.Timestamp = DateTime.Now;

        bool saveToFirebase = RadioFirebase.IsChecked;

        if (saveToFirebase)
        {
            if (Connectivity.NetworkAccess == NetworkAccess.Internet)
            {
                try
                {
                    await firebaseService.SaveNoteToFirebaseAsync(currentNote, currentUser);
                    await DisplayAlert("Success", "The note is saved to Firebase!", "OK");
                }
                catch (Exception ex)
                {
                    await DisplayAlert("Error", ex.Message, "OK");
                }
            }
            else
            {
                await DisplayAlert("Error", "No internet connection. Unable to save to Firebase.", "OK");
            }
        }
        else
        {
            try
            {
                await localNoteService.SaveNoteAsync(currentNote);
                await DisplayAlert("Success", "Note saved locally!", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }

        UpdateLastModifiedLabel();
    }
    private async void OnDeleteClicked(object sender, EventArgs e)
    {
        bool confirmed = await DisplayAlert("Confirmation", "Are you sure you want to delete the note?", "Yes", "No");
        if (!confirmed)
            return;

        if (currentNote.IsLocal)
        {
            try
            {
                await localNoteService.DeleteNoteAsync(currentNote.Id);
                await DisplayAlert("Success", "Note deleted locally", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        else
        {
            if (Connectivity.NetworkAccess != NetworkAccess.Internet)
            {
                await DisplayAlert("Error", "No internet connection to delete note from Firebase.", "OK");
                return;
            }
            try
            {
                await firebaseService.DeleteNoteFromFirebaseAsync(currentNote, currentUser);
                await DisplayAlert("Success", "Note removed from Firebase", "OK");
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", ex.Message, "OK");
            }
        }
        await Navigation.PopAsync();
    }
    private void UpdateLastModifiedLabel()
    {
        LastModifiedLabel.Text = $"Last change: {currentNote.Timestamp:dd.MM.yyyy HH:mm}";
    }
    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

 
}