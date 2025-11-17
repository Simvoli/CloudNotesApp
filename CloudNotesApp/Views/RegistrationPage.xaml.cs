using CloudNotesApp.Services;
using System;
using Microsoft.Maui.Controls;

namespace CloudNotesApp.Views;

public partial class RegistrationPage : ContentPage
{
    FirebaseService firebaseService = new FirebaseService();

    public RegistrationPage()
    {
        InitializeComponent();
    }

    private async void OnRegisterClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string email = EmailEntry.Text;
        string password = PasswordEntry.Text;

        try
        {
            await firebaseService.RegisterUserAsync(username, email, password);
            await DisplayAlert("Success", "Registration was successful!", "OK");
            await Navigation.PopAsync(); 
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async void OnBackClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}