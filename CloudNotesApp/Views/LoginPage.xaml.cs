using CloudNotesApp.Models;
using CloudNotesApp.Services;
using System;
using Microsoft.Maui.Controls;

namespace CloudNotesApp.Views;

public partial class LoginPage : ContentPage
{
    FirebaseService firebaseService = new FirebaseService();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        try
        {
            User user = await firebaseService.LoginUserAsync(email, password);
            await Navigation.PushAsync(new NotesPage(user));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }
    private async void OnOfflineModeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new NotesPage(null));
    }


    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new RegistrationPage());
    }
}