using CloudNotesApp.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CloudNotesApp.Services
{
    public class FirebaseService
    {
        private static readonly HttpClient client = new HttpClient();
        public async Task<User> RegisterUserAsync(string username, string email, string password)
        {
            string signUpUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signUp?key={FirebaseConfig.ApiKey}";

            var signUpData = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(signUpData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(signUpUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Error registration: {responseString}");
            }

            var result = JsonSerializer.Deserialize<FirebaseAuthResponse>(responseString);

            var userData = new
            {
                username = username,
                email = email
            };

            string userUrl = $"{FirebaseConfig.DatabaseUrl}users/{result.localId}.json?auth={result.idToken}";
            var userJson = JsonSerializer.Serialize(userData);
            var userContent = new StringContent(userJson, Encoding.UTF8, "application/json");

            var userResponse = await client.PutAsync(userUrl, userContent);
            var userResponseString = await userResponse.Content.ReadAsStringAsync();

            if (!userResponse.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to save user data: {userResponseString}");
            }

            return new User
            {
                LocalId = result.localId,
                Email = email,
                IdToken = result.idToken
            };
        }

        public async Task<User> LoginUserAsync(string email, string password)
        {
            string signInUrl = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={FirebaseConfig.ApiKey}";

            var signInData = new
            {
                email = email,
                password = password,
                returnSecureToken = true
            };

            var json = JsonSerializer.Serialize(signInData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PostAsync(signInUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Login error: {responseString}");
            }

            var result = JsonSerializer.Deserialize<FirebaseAuthResponse>(responseString);

            return new User
            {
                LocalId = result.localId,
                Email = email,
                IdToken = result.idToken
            };
        }

        public async Task SaveNoteToFirebaseAsync(Note note, User user)
        {
            string noteId = string.IsNullOrEmpty(note.Id) ? Guid.NewGuid().ToString() : note.Id;
            note.Id = noteId;
            string noteUrl = $"{FirebaseConfig.DatabaseUrl}notes/{user.LocalId}/{noteId}.json?auth={user.IdToken}";

            var json = JsonSerializer.Serialize(note);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await client.PutAsync(noteUrl, content);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to save note: {responseString}");
            }
        }

        public async Task<Note[]> GetNotesFromFirebaseAsync(User user)
        {
            string notesUrl = $"{FirebaseConfig.DatabaseUrl}notes/{user.LocalId}.json?auth={user.IdToken}";
            var response = await client.GetAsync(notesUrl);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Failed to retrieve notes: {responseString}");
            }

            if (responseString == "null")
            {
                return new Note[0];
            }

            var notesDict = JsonSerializer.Deserialize<Dictionary<string, Note>>(responseString);
            var notes = new List<Note>();
            foreach (var kv in notesDict)
            {
                notes.Add(kv.Value);
            }
            return notes.ToArray();
        }

        public async Task DeleteNoteFromFirebaseAsync(Note note, Models.User user)
        {
            string noteUrl = $"{FirebaseConfig.DatabaseUrl}notes/{user.LocalId}/{note.Id}.json?auth={user.IdToken}";
            var response = await client.DeleteAsync(noteUrl);
            var responseString = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception($"Не удалось удалить заметку: {responseString}");
            }
        }

        private class FirebaseAuthResponse
        {
            public string idToken { get; set; }
            public string email { get; set; }
            public string refreshToken { get; set; }
            public string expiresIn { get; set; }
            public string localId { get; set; }
        }
    }
}
