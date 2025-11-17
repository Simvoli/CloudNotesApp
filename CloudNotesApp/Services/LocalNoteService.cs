using CloudNotesApp.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;

namespace CloudNotesApp.Services
{
    public class LocalNoteService
    {
        private readonly string filePath;

        public LocalNoteService()
        {
            filePath = Path.Combine(FileSystem.AppDataDirectory, "notes.json");
        }

        public async Task<List<Note>> GetNotesAsync()
        {
            if (!File.Exists(filePath))
                return new List<Note>();

            var json = await File.ReadAllTextAsync(filePath);
            if (string.IsNullOrWhiteSpace(json))
                return new List<Note>();

            return JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
        }

        public async Task SaveNoteAsync(Note note)
        {
            var notes = await GetNotesAsync();
            var existingNote = notes.Find(n => n.Id == note.Id);
            if (existingNote != null)
            {
                existingNote.Content = note.Content;
                existingNote.Timestamp = note.Timestamp;
            }
            else
            {
                if (string.IsNullOrEmpty(note.Id))
                    note.Id = Guid.NewGuid().ToString();
                notes.Add(note);
            }
            var json = JsonSerializer.Serialize(notes);
            await File.WriteAllTextAsync(filePath, json);
        }

        public async Task DeleteNoteAsync(string noteId)
        {
            var notes = await GetNotesAsync();
            notes.RemoveAll(n => n.Id == noteId);
            var json = JsonSerializer.Serialize(notes);
            await File.WriteAllTextAsync(filePath, json);
        }
    }
}
